using System;
using System.Collections.Generic;

namespace apuracao.johnmarlon.com.br.Models
{
    public class ApuracaoViewModel
    {
        public string Pleito { get; set; } = "3221";
        public string Ano { get; set; } = "2026-2T";
        public string CodigoEleicao { get; set; } = "6258";
        public string TipoEleicao { get; set; } = "Federal"; // Federal ou Estadual
        public string CargoCodigo { get; set; } = "1";
        public string CargoNome { get; set; } = "Presidente";
        public string Uf { get; set; } = "br";
        public string? CodigoMunicipio { get; set; }
        public string? NomeMunicipio { get; set; }

        public int TotalSecoes { get; set; }
        public int SecoesTotalizadas { get; set; }
        public decimal PercentualTotalizado { get; set; }

        public int TotalEleitores { get; set; }
        public int Comparecimento { get; set; }
        public decimal PercentualComparecimento { get; set; }
        public int Abstencao { get; set; }
        public decimal PercentualAbstencao { get; set; }

        public int VotosValidos { get; set; }
        public decimal PercentualVotosValidos { get; set; }
        public int VotosBrancos { get; set; }
        public decimal PercentualVotosBrancos { get; set; }
        public int VotosNulos { get; set; }
        public decimal PercentualVotosNulos { get; set; }

        public bool Finalizado { get; set; }
        public bool MatematicamenteEleito { get; set; }
        public string? NomeEleitoMatematico { get; set; }
        public string? PartidoEleitoMatematico { get; set; }
        public decimal? PercentualEleitoMatematico { get; set; }
        public bool AoVivo { get; set; } = true;
        public string? DataHoraTse { get; set; }
        public DateTime DataHoraAtualizacao { get; set; } = DateTime.Now;

        public UrnasPos17hResumoDto? UrnasPos17h { get; set; }

        public List<CandidatoItemViewModel> Candidatos { get; set; } = new();

        public List<MunicipioItem> MunicipiosDisponiveis { get; set; } = new();

        public List<EleicaoConfiguracao> EleicoesDisponiveis { get; set; } = new();
        public List<EleicaoConfiguracao> EleicoesHistoricas { get; set; } = new();

        public CidadeDetalhesInfo? InformacoesCidade { get; set; }

        public List<NoticiaItem> Noticias { get; set; } = new();
        public List<MapaRegiaoItemDto> Regioes { get; set; } = new();

        public bool IsHistorico => Ano != "2026" && !Ano.StartsWith("2026");
    }

    public class EleicaoConfiguracao
    {
        public string Ano { get; set; } = "";
        public string Pleito { get; set; } = "";
        public string Descricao { get; set; } = "";
        public string Tipo { get; set; } = "Geral"; // "Geral" ou "Municipal"
        public string EleicaoFederal { get; set; } = "";
        public string EleicaoEstadual { get; set; } = "";
        public string? EleicaoDistrital { get; set; }
        public bool Ativa { get; set; } = true;
        public bool AoVivo { get; set; }
        public bool Historica { get; set; } = false;
    }

    public class CidadeDetalhesInfo
    {
        public string Nome { get; set; } = "";
        public string CodigoTse { get; set; } = "";
        public string CodigoIbge { get; set; } = "";
        public int PopulacaoIbge { get; set; }
        public int EleitoradoTotalApto { get; set; }
        public string ZonasEleitorais { get; set; } = "";
        public int TotalSecoes { get; set; }
        public string AreaKm2 { get; set; } = "";
        public string RegiaoMetropolitana { get; set; } = "";
        public string Idhm { get; set; } = "";
        public string PercentualBiometria { get; set; } = "";
    }

    public class CandidatoItemViewModel
    {
        public string Numero { get; set; } = "";
        public string? Sequencial { get; set; }
        public string NomeUrna { get; set; } = "";
        public string? NomeCompleto { get; set; }
        public string? Partido { get; set; }
        public string? Coligacao { get; set; }
        public int Votos { get; set; }
        public decimal Percentual { get; set; }
        public string? Situacao { get; set; }
        public bool Eleito { get; set; }
        public string? ViceNome { get; set; }
        public string? VicePartido { get; set; }
        public string? FotoUrl { get; set; }
        public string? CorPartido { get; set; }
    }

    public class MunicipioItem
    {
        public string Codigo { get; set; } = "";
        public string Uf { get; set; } = "";
        public string Nome { get; set; } = "";
        public bool Destaque { get; set; }
    }
}
