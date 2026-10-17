using System.Diagnostics;
using System.Threading.Tasks;
using apuracao.johnmarlon.com.br.Models;
using apuracao.johnmarlon.com.br.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace apuracao.johnmarlon.com.br.Controllers
{
    public class HomeController : Controller
    {
        private readonly TseService _tseService;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public HomeController(
            TseService tseService,
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache)
        {
            _tseService = tseService;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
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

        [HttpGet]
        public async Task<IActionResult> DetectarLocalidade()
        {
            try
            {
                string? rawIp = null;
                if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
                {
                    rawIp = forwarded.ToString().Split(',')[0].Trim();
                }
                if (string.IsNullOrWhiteSpace(rawIp))
                {
                    rawIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                bool isValido = System.Net.IPAddress.TryParse(rawIp, out var ipAddr);
                bool isLocal = !isValido || System.Net.IPAddress.IsLoopback(ipAddr!) || rawIp!.StartsWith("192.168.") || rawIp.StartsWith("10.");

                string cacheKey = $"geo_ip_{rawIp ?? "local"}";
                if (_cache.TryGetValue(cacheKey, out string? cachedGeoJson) && !string.IsNullOrEmpty(cachedGeoJson))
                {
                    return Content(cachedGeoJson, "application/json");
                }

                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(3);
                if (!client.DefaultRequestHeaders.Contains("User-Agent"))
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "ApuracaoEleicoes/1.0");
                }

                string url = isLocal ? "https://ipwho.is/" : $"https://ipwho.is/{Uri.EscapeDataString(ipAddr!.ToString())}";
                var res = await client.GetAsync(url);
                if (res.IsSuccessStatusCode)
                {
                    var content = await res.Content.ReadAsStringAsync();
                    _cache.Set(cacheKey, content, TimeSpan.FromHours(1));
                    return Content(content, "application/json");
                }
            }
            catch
            {
                // Fallback silencioso sem interromper a tela
            }

            return Json(new
            {
                success = true,
                country_code = "BR",
                region_code = "SP",
                region = "São Paulo",
                city = "Campinas"
            });
        }

        [HttpPost]
        public async Task<IActionResult> SincronizarHistorico()
        {
            var validToken = _config["Security:ApiToken"];
            if (string.IsNullOrWhiteSpace(validToken))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    sucesso = false,
                    mensagem = "Sincronização administrativa temporariamente desabilitada por segurança (Token não configurado)."
                });
            }

            if (!Request.Headers.TryGetValue("X-Api-Token", out var headerToken) || string.IsNullOrWhiteSpace(headerToken))
            {
                return Unauthorized(new { sucesso = false, mensagem = "Acesso não autorizado. Cabeçalho 'X-Api-Token' obrigatório." });
            }

            byte[] expectedBytes = System.Text.Encoding.UTF8.GetBytes(validToken);
            byte[] providedBytes = System.Text.Encoding.UTF8.GetBytes(headerToken.ToString());

            if (expectedBytes.Length != providedBytes.Length || 
                !System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes))
            {
                return Unauthorized(new { sucesso = false, mensagem = "Token administrativo inválido." });
            }

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
