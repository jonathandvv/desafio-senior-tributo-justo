namespace TributoJustoBackend.Models.ViewModels
{
    public class ResultadoAnaliseFiscalViewModel
    {
        public int NotaFiscalId { get; set; }
        public string NumeroNota { get; set; }
        public string CnpjEmpresa { get; set; }
        public decimal Total { get; set; }
        public decimal Impostos { get; set; }
        public int RiscoFiscal { get; set; }
        public List<string> Inconsistencias { get; set; } = new();
    }
}
