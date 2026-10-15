using System.Diagnostics;
using System.Threading.Tasks;
using apuracao.johnmarlon.com.br.Models;
using apuracao.johnmarlon.com.br.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apuracao.johnmarlon.com.br.Controllers
{
    public class HomeController : Controller
    {
        private readonly TseService _tseService;

        public HomeController(TseService tseService)
        {
            _tseService = tseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] string? cargo = null,
            [FromQuery] string uf = "br",
            [FromQuery] string? municipio = null,
            [FromQuery] string ano = "2026")
        {
            var model = await _tseService.ObterApuracaoAsync(cargo, uf, municipio, ano);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DadosApuracao(
            [FromQuery] string? cargo = null,
            [FromQuery] string uf = "br",
            [FromQuery] string? municipio = null,
            [FromQuery] string ano = "2026")
        {
            var model = await _tseService.ObterApuracaoAsync(cargo, uf, municipio, ano);
            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> DadosMapaBrasil([FromQuery] string ano = "2026")
        {
            var mapa = await _tseService.ObterMapaBrasilAsync(ano);
            return Json(mapa);
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> SincronizarHistorico()
        {
            try
            {
                var total = await _tseService.ImportarHistorico2024Async();
                return Json(new
                {
                    sucesso = true,
                    totalSnapshots = total,
                    mensagem = $"Carga histórica das eleições 2024 sincronizada com sucesso ({total} rotinas processadas: Campinas e São Paulo)."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    sucesso = false,
                    mensagem = $"Erro ao sincronizar com o banco: {ex.Message}"
                });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
