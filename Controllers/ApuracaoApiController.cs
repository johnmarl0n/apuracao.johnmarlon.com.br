using System;
using System.Threading.Tasks;
using apuracao.johnmarlon.com.br.Models;
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
        public async Task<IActionResult> ObterTimeline(
            [FromQuery] string? cargo = "1",
            [FromQuery] string? uf = "br",
            [FromQuery] string? ano = null)
        {
            var dados = await _tseService.ObterApuracaoAsync(cargo, uf ?? "br", null, ano);
            if (dados == null || dados.PercentualTotalizado == 0)
            {
                return Ok(new
                {
                    sucesso = true,
                    totalizado = 0,
                    timeline = Array.Empty<TimelinePontoDto>()
                });
            }

            var pontos = await _tseService.ObterTimelineAsync(dados.CodigoEleicao, cargo ?? "1", uf ?? "br");

            // Se o banco ainda não tiver snapshots históricos mas a apuração já estiver em andamento, incluir ponto atual
            if (pontos.Count == 0 && dados.PercentualTotalizado > 0 && dados.Candidatos.Count >= 2)
            {
                var c1 = dados.Candidatos[0];
                var c2 = dados.Candidatos[1];
                pontos.Add(new TimelinePontoDto
                {
                    Pct = (int)Math.Round(dados.PercentualTotalizado),
                    Hora = DateTime.Now.ToString("HH:mm"),
                    Secoes = $"{dados.PercentualTotalizado:F1}%",
                    Cand1 = c1.Percentual,
                    Cand2 = c2.Percentual,
                    Cand1Votos = c1.Votos,
                    Cand2Votos = c2.Votos,
                    Diff = Math.Abs(c1.Percentual - c2.Percentual),
                    Status = $"Totalização em {dados.PercentualTotalizado:F1}%"
                });
            }

            return Ok(new
            {
                sucesso = true,
                totalizado = dados.PercentualTotalizado,
                timeline = pontos
            });
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

        [HttpGet("urnas-pos-17h")]
        public async Task<IActionResult> ObterUrnasPos17h([FromQuery] string? uf = null, [FromQuery] string? busca = null)
        {
            var resumo = await _tseService.ObterUrnasPos17hResumoAsync();
            var todasSecoes = await _tseService.ObterTodasUrnasPos17hAsync();

            var query = todasSecoes.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(uf) && !uf.Equals("todas", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(s => s.Uf.Equals(uf.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(busca))
            {
                string termo = busca.Trim().ToLowerInvariant();
                query = query.Where(s =>
                    s.Municipio.ToLowerInvariant().Contains(termo) ||
                    s.LocalVotacao.ToLowerInvariant().Contains(termo) ||
                    s.Zona.ToString() == termo ||
                    s.Secao.ToString() == termo ||
                    s.MotivoExtensao.ToLowerInvariant().Contains(termo)
                );
            }

            var filtradas = query.ToList();

            return Ok(new
            {
                sucesso = true,
                resumo = resumo,
                totalFiltradas = filtradas.Count,
                secoes = filtradas
            });
        }
    }
}
