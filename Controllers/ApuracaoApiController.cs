using System.Threading.Tasks;
using apuracao.johnmarlon.com.br.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;

namespace apuracao.johnmarlon.com.br.Controllers
{
    [ApiController]
    [Route("api/apuracao")]
    public class ApuracaoApiController : ControllerBase
    {
        private readonly TseService _tseService;
        private readonly IConfiguration _config;
        private readonly OnlinePresenceService _presenceService;

        public ApuracaoApiController(TseService tseService, IConfiguration config, OnlinePresenceService presenceService)
        {
            _tseService = tseService;
            _config = config;
            _presenceService = presenceService;
        }

        [HttpGet("presenca")]
        [HttpPost("presenca")]
        [EnableRateLimiting("ApiLimiter")]
        public IActionResult RegistrarPresenca([FromQuery] string? id = null)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                _presenceService.Ping(id);
            }
            int online = _presenceService.GetOnlineCount();
            return Ok(new
            {
                sucesso = true,
                online = online,
                texto = online == 1 ? "1 pessoa agora" : $"{online} pessoas agora",
                dataHora = DateTime.Now.ToString("HH:mm:ss")
            });
        }

        [HttpGet("timeline")]
        public IActionResult ObterTimeline()
        {
            var pontos = new[]
            {
                new { pct = 0, hora = "17:00", secoes = "0%", cand1 = 49.80, cand2 = 50.20, diff = 0.40, status = "Início da Totalização" },
                new { pct = 15, hora = "17:45", secoes = "15%", cand1 = 48.90, cand2 = 51.10, diff = 2.20, status = "Apuração Sudeste e Sul aceleradas" },
                new { pct = 35, hora = "18:30", secoes = "35%", cand1 = 47.60, cand2 = 52.40, diff = 4.80, status = "Interior de SP e PR avançados" },
                new { pct = 50, hora = "19:15", secoes = "50%", cand1 = 46.85, cand2 = 53.15, diff = 6.30, status = "Pico da diferença pró-oposição" },
                new { pct = 68, hora = "19:55", secoes = "68%", cand1 = 48.50, cand2 = 51.50, diff = 3.00, status = "Nordeste entra com força na contagem" },
                new { pct = 85, hora = "20:30", secoes = "85%", cand1 = 50.10, cand2 = 49.90, diff = 0.20, status = "Virada matemática na contagem" },
                new { pct = 95, hora = "21:10", secoes = "95%", cand1 = 50.85, cand2 = 49.15, diff = 1.70, status = "Definição matemática iminente" },
                new { pct = 100, hora = "22:00", secoes = "100%", cand1 = 50.90, cand2 = 49.10, diff = 1.80, status = "Totalização 100% Concluída" }
            };
            return Ok(new { sucesso = true, timeline = pontos });
        }

        [HttpGet("resumo")]
        public async Task<IActionResult> ObterResumo(
            [FromQuery] string? cargo = null,
            [FromQuery] string? uf = "br",
            [FromQuery] string? municipio = null,
            [FromQuery] string? ano = null)
        {
            var dados = await _tseService.ObterApuracaoAsync(cargo, uf ?? "br", municipio, ano);

            return Ok(new
            {
                sucesso = true,
                eleicao = new
                {
                    ano = dados.Ano,
                    pleito = dados.Pleito,
                    codigoEleicao = dados.CodigoEleicao,
                    tipo = dados.TipoEleicao,
                    cargoCodigo = dados.CargoCodigo,
                    cargoNome = dados.CargoNome,
                    uf = dados.Uf,
                    municipioCodigo = dados.CodigoMunicipio,
                    municipioNome = dados.NomeMunicipio
                },
                apuracao = new
                {
                    secoesTotalizadas = dados.SecoesTotalizadas,
                    totalSecoes = dados.TotalSecoes,
                    percentualApurado = dados.PercentualTotalizado,
                    finalizado = dados.Finalizado,
                    dataHoraTse = dados.DataHoraTse,
                    dataHoraColeta = dados.DataHoraAtualizacao
                },
                estatisticas = new
                {
                    eleitorado = dados.TotalEleitores,
                    comparecimento = dados.Comparecimento,
                    percentualComparecimento = dados.PercentualComparecimento,
                    abstencao = dados.Abstencao,
                    percentualAbstencao = dados.PercentualAbstencao,
                    votosValidos = dados.VotosValidos,
                    percentualVotosValidos = dados.PercentualVotosValidos,
                    votosBrancos = dados.VotosBrancos,
                    percentualVotosBrancos = dados.PercentualVotosBrancos,
                    votosNulos = dados.VotosNulos,
                    percentualVotosNulos = dados.PercentualVotosNulos
                },
                candidatos = dados.Candidatos,
                noticias = dados.Noticias
            });
        }

        [HttpGet("noticias")]
        public async Task<IActionResult> ObterNoticias([FromQuery] string? ano = null)
        {
            var dados = await _tseService.ObterApuracaoAsync(cargo: "1", uf: "br", municipio: null, ano: ano);
            return Ok(new
            {
                sucesso = true,
                ano = dados.Ano,
                total = dados.Noticias.Count,
                noticias = dados.Noticias
            });
        }

        [HttpGet("municipios")]
        public IActionResult ObterMunicipios([FromQuery] string? uf = null, [FromQuery] string? q = null)
        {
            var lista = _tseService.ObterMunicipios(uf, apenasDestaque: false);
            if (!string.IsNullOrWhiteSpace(q))
            {
                var termo = TseService.RemoverAcentos(q);
                lista = lista.Where(m => TseService.RemoverAcentos(m.Nome).Contains(termo) || m.Codigo == q.Trim()).ToList();
            }

            return Ok(new
            {
                sucesso = true,
                total = lista.Count,
                municipios = lista.Take(100)
            });
        }

        [HttpPost("importar-historico")]
        [HttpPost("historico/importar")]
        [EnableRateLimiting("ApiLimiter")]
        public async Task<IActionResult> ImportarHistorico([FromQuery] string? token = null)
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

            string? providedToken = null;
            if (Request.Headers.TryGetValue("X-Api-Token", out var headerToken))
            {
                providedToken = headerToken.ToString();
            }
            else if (!string.IsNullOrWhiteSpace(token))
            {
                providedToken = token;
            }

            if (string.IsNullOrWhiteSpace(providedToken))
            {
                return Unauthorized(new { sucesso = false, mensagem = "Acesso não autorizado. Forneça o token administrativo via cabeçalho 'X-Api-Token'." });
            }

            byte[] expectedBytes = System.Text.Encoding.UTF8.GetBytes(validToken);
            byte[] providedBytes = System.Text.Encoding.UTF8.GetBytes(providedToken);

            if (expectedBytes.Length != providedBytes.Length || 
                !System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes))
            {
                return Unauthorized(new { sucesso = false, mensagem = "Acesso não autorizado. Token administrativo inválido." });
            }

            try
            {
                var total = await _tseService.ImportarHistorico2024Async();
                return Ok(new
                {
                    sucesso = true,
                    totalSnapshots = total,
                    mensagem = $"Carga histórica das eleições 2024 importada com sucesso no MSSQL ({total} rotinas sincronizadas: Campinas e São Paulo)."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    sucesso = false,
                    mensagem = $"Falha ao sincronizar com o banco: {ex.Message}"
                });
            }
        }
    }
}
