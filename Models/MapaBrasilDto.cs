using System;
using System.Collections.Generic;

namespace apuracao.johnmarlon.com.br.Models
{
    public class MapaBrasilDto
    {
        public string Ano { get; set; } = "2026-2T";
        public string Cargo { get; set; } = "1";
        public int TotalEstados { get; set; } = 27;
        public int EstadosTotalizados { get; set; }
        public Dictionary<string, MapaUfItemDto> Estados { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public List<PlacarLiderDto> Placar { get; set; } = new();
        public List<MapaRegiaoItemDto> Regioes { get; set; } = new();
    }

    public class MapaUfItemDto
    {
        public string Uf { get; set; } = "";
        public string Nome { get; set; } = "";
        public decimal PercentualTotalizado { get; set; }
        public bool Finalizado { get; set; }
        public int SecoesTotalizadas { get; set; }
        public int TotalSecoes { get; set; }
        public MapaCandidatoResumoDto? Lider { get; set; }
        public MapaCandidatoResumoDto? ViceLider { get; set; }
    }

    public class MapaCandidatoResumoDto
    {
        public string Numero { get; set; } = "";
        public string NomeUrna { get; set; } = "";
        public string Partido { get; set; } = "";
        public string CorPartido { get; set; } = "#3b82f6";
        public decimal Percentual { get; set; }
        public int Votos { get; set; }
        public string? FotoUrl { get; set; }
    }

    public class PlacarLiderDto
    {
        public string Numero { get; set; } = "";
        public string NomeUrna { get; set; } = "";
        public string Partido { get; set; } = "";
        public string CorPartido { get; set; } = "#3b82f6";
        public int TotalEstadosLiderando { get; set; }
        public bool LideraExterior { get; set; }
        public List<string> Ufs { get; set; } = new();
    }

    public class MapaRegiaoItemDto
    {
        public string Nome { get; set; } = "";
        public decimal PercentualTotalizado { get; set; }
        public int SecoesTotalizadas { get; set; }
        public int TotalSecoes { get; set; }
        public string? LiderNumero { get; set; }
        public string? LiderNome { get; set; }
        public string? LiderPartido { get; set; }
        public string? LiderCor { get; set; }
        public string? LiderFotoUrl { get; set; }
        public decimal LiderPercentual { get; set; }
        public decimal Diferenca { get; set; }
        public decimal Cand1Perc { get; set; }
        public decimal Cand2Perc { get; set; }
        public bool TemDados => PercentualTotalizado > 0 && (Cand1Perc > 0 || Cand2Perc > 0);
    }

    public class TimelinePontoDto
    {
        public int Pct { get; set; }
        public string Hora { get; set; } = "";
        public string Secoes { get; set; } = "";
        public decimal Cand1 { get; set; }
        public decimal Cand2 { get; set; }
        public int Cand1Votos { get; set; }
        public int Cand2Votos { get; set; }
        public decimal Diff { get; set; }
        public string Status { get; set; } = "";
    }
}

