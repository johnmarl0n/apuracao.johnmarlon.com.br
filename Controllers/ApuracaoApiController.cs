using System.Threading.Tasks;
using apuracao.johnmarlon.com.br.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace apuracao.johnmarlon.com.br.Controllers
{
    [ApiController]
    [Route("api/apuracao")]
    public class ApuracaoApiController : ControllerBase
    {
        private readonly TseService _tseService;
        private readonly IConfiguration _config;

        public ApuracaoApiController(TseService tseService, IConfiguration config)
        {
            _tseService = tseService;
            _config = config;
        }

        [HttpGet("resumo")]
        public async Task<IActionResult> ObterResumo(
            [FromQuery] string? cargo = null,
            [FromQuery] string? uf = "br",
            [FromQuery] string? municipio = null,
            [FromQuery] string? ano = "2026")
        {
            var dados = await _tseService.ObterApuracaoAsync(cargo, uf ?? "br", municipio, ano ?? "2026");

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
                candidatos = dados.Candidatos
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

        [HttpGet("importar-historico")]
        [HttpPost("importar-historico")]
        [HttpGet("historico/importar")]
        [HttpPost("historico/importar")]
        public async Task<IActionResult> ImportarHistorico([FromQuery] string? token = null)
        {
            var validToken = _config["Security:ApiToken"];
            string? providedToken = token;
            if (string.IsNullOrWhiteSpace(providedToken) && Request.Headers.TryGetValue("X-Api-Token", out var headerToken))
            {
                providedToken = headerToken.ToString();
            }

            bool isValidToken = !string.IsNullOrWhiteSpace(validToken) && (!string.IsNullOrWhiteSpace(providedToken) && providedToken == validToken);

            if (!isValidToken && !string.IsNullOrWhiteSpace(validToken))
            {
                return Unauthorized(new { sucesso = false, mensagem = "Acesso não autorizado. Forneça o token administrativo de API configurado." });
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
