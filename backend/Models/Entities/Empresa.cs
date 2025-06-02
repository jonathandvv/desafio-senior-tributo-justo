using System.ComponentModel.DataAnnotations;

namespace TributoJustoBackend.Models.Entities
{
    public class Empresa
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(18)]
        public string Cnpj { get; set; }

        [Required]
        [MaxLength(255)]
        public string RazaoSocial { get; set; }

        [Required]
        [MaxLength(100)]
        public string RegimeTributario { get; set; }

        public ICollection<NotaFiscal> NotasFiscais { get; set; }
    }
}