using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TributoJustoBackend.Data;
using TributoJustoBackend.Models;
using TributoJustoBackend.Models.Entities;

namespace TributoJustoBackend.Servicos
{
    public class ImportacaoService
    {
        private readonly AppDbContext _contexto;

        public ImportacaoService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task ImportarCsvAsync(string caminho)
        {
            ValidarArquivo(caminho);

            var natureza = await ObterOuCriarNaturezaOperacaoAsync();

            var registros = LerRegistrosCsv(caminho);

            var cacheEmpresas = new Dictionary<string, Empresa>();
            var cacheNotas = new Dictionary<(string numeroNota, int empresaId), NotaFiscal>();

            foreach (var registro in registros)
            {
                var empresa = await ObterOuCriarEmpresaAsync(registro, cacheEmpresas);
                var nota = await ObterOuCriarNotaFiscalAsync(registro, empresa, natureza, cacheNotas);

                AdicionarItemNaNotaFiscal(registro, nota);

                AtualizarTotaisNota(nota, registro);
            }

            await _contexto.SaveChangesAsync();
        }

        private static void ValidarArquivo(string caminho)
        {
            if (string.IsNullOrEmpty(caminho) || !File.Exists(caminho))
                throw new FileNotFoundException($"Arquivo CSV não encontrado: {caminho}");
        }

        private async Task<NaturezaOperacao> ObterOuCriarNaturezaOperacaoAsync()
        {
            var natureza = await _contexto.NaturezasOperacao.FirstOrDefaultAsync();
            if (natureza == null)
            {
                natureza = new NaturezaOperacao
                {
                    Cfop = 5102,
                    TipoProduto = "Produto",
                    Descricao = "Venda de mercadoria adquirida para revenda"
                };
                _contexto.NaturezasOperacao.Add(natureza);
                await _contexto.SaveChangesAsync();
            }
            return natureza;
        }

        private List<RegistroCsvDto> LerRegistrosCsv(string caminho)
        {
            using var leitor = new StreamReader(caminho);
            using var csv = new CsvReader(leitor, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true
            });

            return csv.GetRecords<RegistroCsvDto>().ToList();
        }

        private async Task<Empresa> ObterOuCriarEmpresaAsync(RegistroCsvDto registro, Dictionary<string, Empresa> cacheEmpresas)
        {
            var cnpjLimpo = new string(registro.Cnpj.Where(char.IsDigit).ToArray());

            if (!cacheEmpresas.TryGetValue(cnpjLimpo, out var empresa))
            {
                empresa = await _contexto.Empresas.FirstOrDefaultAsync(e => e.Cnpj == cnpjLimpo);
                if (empresa == null)
                {
                    empresa = new Empresa
                    {
                        Cnpj = cnpjLimpo,
                        RazaoSocial = registro.RazaoSocial,
                        RegimeTributario = "Lucro Presumido"
                    };
                    _contexto.Empresas.Add(empresa);
                    await _contexto.SaveChangesAsync();
                }
                cacheEmpresas[cnpjLimpo] = empresa;
            }

            return empresa;
        }

        private async Task<NotaFiscal> ObterOuCriarNotaFiscalAsync(RegistroCsvDto registro, Empresa empresa, NaturezaOperacao natureza, Dictionary<(string, int), NotaFiscal> cacheNotas)
        {
            var chaveNota = (registro.NumeroNota, empresa.Id);

            if (!cacheNotas.TryGetValue(chaveNota, out var nota))
            {
                nota = await _contexto.NotasFiscais
                    .FirstOrDefaultAsync(n => n.Numero == registro.NumeroNota && n.EmpresaId == empresa.Id);

                if (nota == null)
                {
                    nota = new NotaFiscal
                    {
                        Numero = registro.NumeroNota,
                        DataEmissao = new DateTimeOffset(registro.DataEmissao, TimeSpan.Zero),
                        Total = 0,
                        Impostos = 0,
                        EmpresaId = empresa.Id,
                        NaturezaOperacaoId = natureza.Id,
                    };
                    _contexto.NotasFiscais.Add(nota);
                    await _contexto.SaveChangesAsync();
                }
                cacheNotas[chaveNota] = nota;
            }

            return nota;
        }

        private void AdicionarItemNaNotaFiscal(RegistroCsvDto registro, NotaFiscal nota)
        {
            var valorTotalItem = registro.Quantidade * registro.ValorUnitario;
            var aliquota = valorTotalItem > 0 ? Math.Round(registro.ImpostoItem / valorTotalItem * 100, 2) : 0;

            var item = new ItemNotaFiscal
            {
                Ncm = registro.CodigoItem,
                Descricao = registro.DescricaoItem,
                Quantidade = registro.Quantidade,
                ValorUnitario = registro.ValorUnitario,
                Aliquota = aliquota,
                NotaFiscalId = nota.Id
            };

            _contexto.ItensNotaFiscal.Add(item);
        }

        private void AtualizarTotaisNota(NotaFiscal nota, RegistroCsvDto registro)
        {
            var valorTotalItem = registro.Quantidade * registro.ValorUnitario;
            nota.Total += valorTotalItem;
            nota.Impostos += registro.ImpostoItem;
        }
    }
}
