using Microsoft.EntityFrameworkCore;
using TributoJustoBackend.Data;
using TributoJustoBackend.Models.Entities;
using TributoJustoBackend.Models.ViewModels;

namespace TributoJustoBackend.Services
{
    public class AnaliseFiscalService
    {
        private readonly AppDbContext _contexto;

        public AnaliseFiscalService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        private string LimparCnpj(string cnpj)
        {
            return new string(cnpj?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
        }

        public async Task<List<ResultadoAnaliseFiscalViewModel>> AnalisarNotasAsync()
        {
            var notas = await ObterNotasComRelacionamentosAsync();
            var resultados = new List<ResultadoAnaliseFiscalViewModel>();

            var grupos = AgruparNotasPorEmpresaEAnoMes(notas);

            foreach (var grupo in grupos)
            {
                var aliquotas = CalcularAliquotas(grupo);
                var media = CalcularMedia(aliquotas);
                var desvioPadrao = CalcularDesvioPadrao(aliquotas, media);

                foreach (var nota in grupo)
                {
                    var resultado = AvaliarNota(nota, media, desvioPadrao);
                    resultados.Add(resultado);
                }
            }

            await _contexto.SaveChangesAsync();
            return resultados;
        }

        private async Task<List<NotaFiscal>> ObterNotasComRelacionamentosAsync()
        {
            return await _contexto.NotasFiscais
                .Include(n => n.Empresa)
                .Include(n => n.Itens)
                .ToListAsync();
        }

        private IEnumerable<IGrouping<object, NotaFiscal>> AgruparNotasPorEmpresaEAnoMes(List<NotaFiscal> notas)
        {
            return notas.GroupBy(n => new { n.EmpresaId, n.DataEmissao.Year, n.DataEmissao.Month });
        }

        private List<decimal> CalcularAliquotas(IEnumerable<NotaFiscal> grupo)
        {
            return grupo
                .Select(n => n.Total > 0 ? (n.Impostos / n.Total) * 100 : 0)
                .ToList();
        }

        private decimal CalcularMedia(List<decimal> valores)
        {
            return valores.Average();
        }

        private double CalcularDesvioPadrao(List<decimal> valores, decimal media)
        {
            return Math.Sqrt(valores.Average(a => Math.Pow((double)(a - media), 2)));
        }

        private ResultadoAnaliseFiscalViewModel AvaliarNota(NotaFiscal nota, decimal media, double desvioPadrao)
        {
            var score = 0;
            var inconsistencias = new List<string>();

            var aliquotaNota = nota.Total > 0 ? (nota.Impostos / nota.Total) * 100 : 0;

            if (Math.Abs((double)(aliquotaNota - media)) > 2 * desvioPadrao)
            {
                score += 30;
                inconsistencias.Add("Alíquota fora da média mensal da empresa");
            }

            foreach (var item in nota.Itens)
            {
                var valorItem = item.Quantidade * item.ValorUnitario;

                if (valorItem > 300 && item.Aliquota < 14.01M)
                {
                    score += 40;
                    inconsistencias.Add($"Item com valor alto ({valorItem:C}) e imposto muito baixo");
                }
            }

            nota.RiscoFiscal = score;

            return new ResultadoAnaliseFiscalViewModel
            {
                NotaFiscalId = nota.Id,
                NumeroNota = nota.Numero,
                CnpjEmpresa = LimparCnpj(nota.Empresa.Cnpj),
                Total = nota.Total,
                Impostos = nota.Impostos,
                RiscoFiscal = score,
                Inconsistencias = inconsistencias
            };
        }
    }
}
