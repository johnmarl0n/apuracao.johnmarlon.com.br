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
}
