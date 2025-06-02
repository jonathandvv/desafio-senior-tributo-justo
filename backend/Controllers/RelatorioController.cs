using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TributoJustoBackend.Models.DTOs;
using TributoJustoBackend.Services;
using TributoJustoBackend.Servicos;

namespace TributoJustoBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RelatorioController : ControllerBase
    {
        private readonly InterpretacaoFiscalService _servicoInterpretacao;
        private readonly AnaliseFiscalService _servicoAnalise;
        private readonly NotaFiscalService _servicoNotas;
        private readonly EmpresaService _servicoEmpresas;

        public RelatorioController(
            InterpretacaoFiscalService servicoInterpretacao,
            AnaliseFiscalService servicoAnalise,
            NotaFiscalService servicoNotas,
            EmpresaService servicoEmpresas)
        {
            _servicoInterpretacao = servicoInterpretacao;
            _servicoAnalise = servicoAnalise;
            _servicoNotas = servicoNotas;
            _servicoEmpresas = servicoEmpresas;
        }

        [HttpPost("interpretar")]
        public async Task<IActionResult> Interpretar([FromBody] ParametrosRelatorioDto parametro)
        {
            if (parametro == null || string.IsNullOrWhiteSpace(parametro.Cnpj))
                return BadRequest("CNPJ é obrigatório.");

            var resultado = await _servicoInterpretacao.GerarResumoFiscalParaIAAsync(parametro);

            if (resultado != null && resultado.GetType().GetProperty("Erro") != null)
                return NotFound(resultado);

            return Ok(resultado);
        }

        [HttpGet("inconsistencias")]
        public async Task<IActionResult> ExecutarAnalise()
        {
            var resultado = await _servicoAnalise.AnalisarNotasAsync();
            return Ok(resultado);
        }

        [HttpGet("inconsistencias-por-risco-fiscal")]
        public async Task<IActionResult> ObterPorRiscoFiscal([FromQuery] int minimo = 50)
        {
            var resultado = await _servicoAnalise.AnalisarNotasAsync();
            var filtradas = resultado.Where(r => r.RiscoFiscal >= minimo).ToList();
            return Ok(filtradas);
        }

        [HttpPost("dashboard")]
        public async Task<IActionResult> ObterDashboard([FromBody] ParametroDashboardDto parametro)
        {
            if (parametro == null || string.IsNullOrWhiteSpace(parametro.Cnpj))
                return BadRequest("CNPJ é obrigatório.");

            var empresa = await _servicoEmpresas.ObterEmpresaPorCnpjAsync(parametro.Cnpj);

            if (empresa == null || empresa.Id == 0)
                return NotFound("Empresa não encontrada.");

            var notas = await _servicoNotas.ListarPorEmpresaAsync(empresa.Id);

            if (notas == null || !notas.Any())
                return NotFound("Nenhuma nota fiscal encontrada.");

            var valorTotalNotas = notas.Sum(n => n.Total);
            var mediaNotas = notas.Average(n => n.Total);
            var totalImpostos = notas.Sum(n => n.Impostos);
            var mediaImpostos = notas.Average(n => n.Impostos);
            var riscoTotal = notas.Sum(n => n.RiscoFiscal);
            var notasComRisco = notas.Count(n => n.RiscoFiscal > 0);

            var dashboard = new
            {
                resumoNotasFiscais = new
                {
                    valorTotalNotas = Math.Round(valorTotalNotas, 2),
                    mediaValorNotas = Math.Round(mediaNotas, 2),
                    totalImpostos = Math.Round(totalImpostos, 2),
                    mediaValorImpostos = Math.Round(mediaImpostos, 2),
                    riscoFiscalTotal = riscoTotal,
                    quantidadeNotasComRisco = notasComRisco
                }
            };

            return Ok(dashboard);
        }
    }
}
