namespace TributoJustoBackend.Models.Entities
{
    public class NotaFiscal
    {
        public int Id { get; set; }

        public string Numero { get; set; }

        public DateTimeOffset DataEmissao { get; set; }

        public decimal Total { get; set; }

        public decimal Impostos { get; set; }

        public int EmpresaId { get; set; }

        public Empresa Empresa { get; set; }

        public int NaturezaOperacaoId { get; set; }

        public NaturezaOperacao NaturezaOperacao { get; set; }

        public ICollection<ItemNotaFiscal> Itens { get; set; }

        public int RiscoFiscal { get; set; }
    }
}