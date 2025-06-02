using System.ComponentModel.DataAnnotations;

namespace TributoJustoBackend.Models.Entities
{
    public class NaturezaOperacao
    {
        public int Id { get; set; }

        public int Cfop { get; set; }

        [MaxLength(100)]
        public string TipoProduto { get; set; }

        public string Descricao { get; set; }

        public ICollection<NotaFiscal> NotasFiscais { get; set; }
    }
}