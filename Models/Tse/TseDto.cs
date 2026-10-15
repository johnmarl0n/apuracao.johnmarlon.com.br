using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace apuracao.johnmarlon.com.br.Models.Tse
{
    public class TseResponseDto
    {
        [JsonPropertyName("ele")]
        public string? Ele { get; set; }

        [JsonPropertyName("t")]
        public string? Turno { get; set; }

        [JsonPropertyName("tpabr")]
        public string? TipoAbrangencia { get; set; }

        [JsonPropertyName("cdabr")]
        public string? CodigoAbrangencia { get; set; }

        [JsonPropertyName("dg")]
        public string? DataGeracao { get; set; }

        [JsonPropertyName("hg")]
        public string? HoraGeracao { get; set; }

        [JsonPropertyName("idg")]
        public string? IdGeracao { get; set; }

        [JsonPropertyName("tf")]
        public string? TotalizacaoFinal { get; set; }

        [JsonPropertyName("and")]
        public string? Andamento { get; set; }

        [JsonPropertyName("carg")]
        public List<TseCargoDto>? Cargos { get; set; }

        [JsonPropertyName("s")]
        public TseSecoesDto? Secoes { get; set; }

        [JsonPropertyName("e")]
        public TseEleitoradoDto? Eleitorado { get; set; }

        [JsonPropertyName("v")]
        public TseVotosTotaisDto? Votos { get; set; }
    }

    public class TseCargoDto
    {
        [JsonPropertyName("cd")]
        public string? Codigo { get; set; }

        [JsonPropertyName("nmn")]
        public string? Nome { get; set; }

        [JsonPropertyName("agr")]
        public List<TseAgrupamentoDto>? Agrupamentos { get; set; }
    }

    public class TseAgrupamentoDto
    {
        [JsonPropertyName("nm")]
        public string? Nome { get; set; }

        [JsonPropertyName("com")]
        public string? Composicao { get; set; }

        [JsonPropertyName("par")]
        public List<TsePartidoDto>? Partidos { get; set; }
    }

    public class TsePartidoDto
    {
        [JsonPropertyName("n")]
        public string? Numero { get; set; }

        [JsonPropertyName("sg")]
        public string? Sigla { get; set; }

        [JsonPropertyName("nm")]
        public string? Nome { get; set; }

        [JsonPropertyName("cand")]
        public List<TseCandidatoDto>? Candidatos { get; set; }
    }

    public class TseCandidatoDto
    {
        [JsonPropertyName("n")]
        public string? Numero { get; set; }

        [JsonPropertyName("sqcand")]
        public string? Sequencial { get; set; }

        [JsonPropertyName("nm")]
        public string? NomeCompleto { get; set; }

        [JsonPropertyName("nmu")]
        public string? NomeUrna { get; set; }

        [JsonPropertyName("seq")]
        public string? Posicao { get; set; }

        [JsonPropertyName("e")]
        public string? Eleito { get; set; } // "s" ou "n"

        [JsonPropertyName("st")]
        public string? Situacao { get; set; } // "Eleito", "2º turno", "Não eleito"

        [JsonPropertyName("vap")]
        public string? VotosApurados { get; set; }

        [JsonPropertyName("pvap")]
        public string? PercentualVotos { get; set; }

        [JsonPropertyName("vs")]
        public List<TseViceDto>? Vice { get; set; }
    }

    public class TseViceDto
    {
        [JsonPropertyName("nm")]
        public string? Nome { get; set; }

        [JsonPropertyName("nmu")]
        public string? NomeUrna { get; set; }

        [JsonPropertyName("sgp")]
        public string? SiglaPartido { get; set; }
    }

    public class TseSecoesDto
    {
        [JsonPropertyName("ts")]
        public string? TotalSecoes { get; set; }

        [JsonPropertyName("st")]
        public string? SecoesTotalizadas { get; set; }

        [JsonPropertyName("pst")]
        public string? PercentualTotalizadas { get; set; }
    }

    public class TseEleitoradoDto
    {
        [JsonPropertyName("te")]
        public string? TotalEleitores { get; set; }

        [JsonPropertyName("c")]
        public string? Comparecimento { get; set; }

        [JsonPropertyName("pc")]
        public string? PercentualComparecimento { get; set; }

        [JsonPropertyName("a")]
        public string? Abstencao { get; set; }

        [JsonPropertyName("pa")]
        public string? PercentualAbstencao { get; set; }
    }

    public class TseVotosTotaisDto
    {
        [JsonPropertyName("tv")]
        public string? TotalVotos { get; set; }

        [JsonPropertyName("vv")]
        public string? VotosValidos { get; set; }

        [JsonPropertyName("pvv")]
        public string? PercentualVotosValidos { get; set; }

        [JsonPropertyName("vb")]
        public string? VotosBrancos { get; set; }

        [JsonPropertyName("pvb")]
        public string? PercentualVotosBrancos { get; set; }

        [JsonPropertyName("tvn")]
        public string? VotosNulos { get; set; }

        [JsonPropertyName("ptvn")]
        public string? PercentualVotosNulos { get; set; }
    }
}
