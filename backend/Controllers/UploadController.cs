using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TributoJustoBackend.Models.DTOs;
using TributoJustoBackend.Servicos;

namespace TributoJustoBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly ImportacaoService _servicoImportacao;

        public UploadController(ImportacaoService servicoImportacao)
        {
            _servicoImportacao = servicoImportacao;
        }

        [HttpPost("arquivo")]
        public async Task<IActionResult> EnviarArquivo([FromForm] UploadArquivoDto dto)
        {
            if (dto.Arquivo == null || dto.Arquivo.Length == 0)
                return BadRequest("Arquivo inválido.");

            var caminhoTemporario = Path.GetTempFileName();

            using (var stream = System.IO.File.Create(caminhoTemporario))
            {
                await dto.Arquivo.CopyToAsync(stream);
            }

            await _servicoImportacao.ImportarCsvAsync(caminhoTemporario);
            System.IO.File.Delete(caminhoTemporario);

            return Ok("Arquivo importado com sucesso.");
        }
    }
}
