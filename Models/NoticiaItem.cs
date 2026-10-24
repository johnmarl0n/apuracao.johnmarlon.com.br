using System;

namespace apuracao.johnmarlon.com.br.Models
{
    public class NoticiaItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Hora { get; set; } = DateTime.Now.ToString("HH\\hmm");
        public DateTime DataHora { get; set; } = DateTime.Now;
        public string? Titulo { get; set; }
        public string ConteudoHtml { get; set; } = "";
        public string? AvatarUrl { get; set; }
        public string? Icone { get; set; }
        public bool Destaque { get; set; }
        public string Tipo { get; set; } = "apuracao"; // apuracao, eleito, lideranca, editorial, aviso
        public string? CandidatoNumero { get; set; }
        public decimal? Percentual { get; set; }
        public int? Votos { get; set; }
    }
}
