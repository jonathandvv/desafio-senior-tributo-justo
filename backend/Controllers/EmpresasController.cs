using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TributoJustoBackend.Services;

namespace TributoJustoBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EmpresasController : ControllerBase
    {
        private readonly EmpresaService _servicoEmpresas;

        public EmpresasController(EmpresaService servicoEmpresas)
        {
            _servicoEmpresas = servicoEmpresas;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var empresas = await _servicoEmpresas.ListarEmpresasAsync();
            var resultado = empresas.Select(e => new { e.Id, e.Cnpj, e.RazaoSocial });
            return Ok(resultado);
        }
    }
}