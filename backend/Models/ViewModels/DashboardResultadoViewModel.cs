using TributoJustoBackend.Models.DTOs;

namespace TributoJustoBackend.Models.ViewModels
{
    public class DashboardResultadoViewModel
    {
        public decimal ImpostoMedio { get; set; }
        public decimal RiscoFiscal { get; set; }
        public int TotalNotas { get; set; }
        public int Inconsistencias { get; set; }
    }
}