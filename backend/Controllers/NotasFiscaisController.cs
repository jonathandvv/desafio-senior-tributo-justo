using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TributoJustoBackend.Data;
using TributoJustoBackend.Services;

namespace TributoJustoBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly AppDbContext _contexto;
        private readonly NotaFiscalService _servicoNotas;

        public NotasFiscaisController(AppDbContext contexto, NotaFiscalService servicoNotas)
        {
            _contexto = contexto;
            _servicoNotas = servicoNotas;
        }

        [HttpGet("empresa/{empresaId}")]
        public async Task<IActionResult> ListarPorEmpresa(int empresaId)
        {
            var notas = await _servicoNotas.ListarPorEmpresaAsync(empresaId);
            return Ok(notas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detalhar(int id)
        {
            var nota = await _contexto.NotasFiscais
                .Include(n => n.Itens)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (nota == null)
                return NotFound();

            return Ok(nota);
        }
    }
}
