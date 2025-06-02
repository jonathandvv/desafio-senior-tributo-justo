using System.Text.Json.Serialization;

namespace TributoJustoBackend.Models.Entities
{
    public class ItemNotaFiscal
    {
        public int Id { get; set; }

        public int Ncm { get; set; }

        public string Descricao { get; set; }

        public int Quantidade { get; set; }

        public decimal ValorUnitario { get; set; }

        public decimal Aliquota { get; set; }

        public int NotaFiscalId { get; set; }

        [JsonIgnore]
        public NotaFiscal NotaFiscal { get; set; }
    }
}