using Microsoft.EntityFrameworkCore;
using System.Text;
using TributoJustoBackend.Data;
using TributoJustoBackend.ExternalServices;
using TributoJustoBackend.Models.DTOs;

namespace TributoJustoBackend.Servicos
{
    public class InterpretacaoFiscalService
    {
        private readonly AppDbContext _contexto;
        private readonly IClienteIA _clienteIA;

        public InterpretacaoFiscalService(AppDbContext contexto, IClienteIA clienteIA)
        {
            _contexto = contexto;
            _clienteIA = clienteIA;
        }

        public async Task<object> GerarResumoFiscalParaIAAsync(ParametrosRelatorioDto parametros)
        {
            var cnpjLimpo = LimparCnpj(parametros.Cnpj);

            var empresa = await ObterEmpresaPorCnpjAsync(cnpjLimpo);
            if (empresa == null)
                return new { Erro = "Empresa não encontrada." };

            var nota = await ObterNotaFiscalAsync(empresa.Id, parametros.NumeroNota);
            if (nota == null)
                return new { Erro = "Nota fiscal não encontrada com os parâmetros informados." };

            var prompt = ConstruirPrompt(empresa, nota);

            var resposta = await _clienteIA.AnalisarTextoAsync(prompt);

            return new
            {
                Empresa = empresa.RazaoSocial,
                Cnpj = empresa.Cnpj,
                NumeroNota = nota.Numero,
                DataEmissao = nota.DataEmissao,
                PromptEnviado = prompt,
                RespostaGerada = resposta
            };
        }

        private string LimparCnpj(string cnpj)
        {
            return new string(cnpj.Where(char.IsDigit).ToArray());
        }

        private async Task<Models.Entities.Empresa?> ObterEmpresaPorCnpjAsync(string cnpjLimpo)
        {
            return await _contexto.Empresas
                .FirstOrDefaultAsync(e => e.Cnpj == cnpjLimpo);
        }

        private async Task<Models.Entities.NotaFiscal?> ObterNotaFiscalAsync(int empresaId, string numeroNota)
        {
            return await _contexto.NotasFiscais
                .Include(n => n.Itens)
                .FirstOrDefaultAsync(n => n.EmpresaId == empresaId && n.Numero == numeroNota);
        }

        private string ConstruirPrompt(Models.Entities.Empresa empresa, Models.Entities.NotaFiscal nota)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Com base na seguinte nota fiscal da empresa {empresa.RazaoSocial} ({empresa.Cnpj}) emitida em {nota.DataEmissao:dd/MM/yyyy}.");
            sb.AppendLine($"Nota {nota.Numero}, Total: R$ {nota.Total}, Impostos: R$ {nota.Impostos}, Risco: {nota.RiscoFiscal}");

            foreach (var item in nota.Itens)
            {
                var totalItem = item.Quantidade * item.ValorUnitario;
                sb.AppendLine($"- Item {item.Descricao} (NCM {item.Ncm}), Quantidade: {item.Quantidade}, Total: R$ {totalItem}, Alíquota: {item.Aliquota:P}");
            }

            sb.AppendLine();
            sb.AppendLine("**Existe alguma inconsistência fiscal ou possível irregularidade nesta nota? Se sim, aponte quais são e sugira ações corretivas. Responda em português.**");

            return sb.ToString();
        }
    }
}
