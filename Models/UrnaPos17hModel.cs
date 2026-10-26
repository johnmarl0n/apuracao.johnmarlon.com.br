using System;
using System.Collections.Generic;

namespace apuracao.johnmarlon.com.br.Models
{
    public class UrnaPos17hItem
    {
        public string Uf { get; set; } = "";
        public string Municipio { get; set; } = "";
        public string CodigoMunicipio { get; set; } = "";
        public int Zona { get; set; }
        public int Secao { get; set; }
        public string LocalVotacao { get; set; } = "";
        public string HorarioFechamento { get; set; } = ""; // Ex: "17h45", "18h30"
        public string MotivoExtensao { get; set; } = "Fila às 17h com distribuição de senhas (Res. TSE 23.736)";
        public int EleitoresAptos { get; set; }
        public int Comparecimento { get; set; }
        public int VotosCand1 { get; set; }
        public decimal PercentualCand1 { get; set; }
        public int VotosCand2 { get; set; }
        public decimal PercentualCand2 { get; set; }
        public int VotosBrancos { get; set; }
        public int VotosNulos { get; set; }
        public int TotalVotosValidos => VotosCand1 + VotosCand2;
        public int TotalVotosApurados => TotalVotosValidos + VotosBrancos + VotosNulos;
    }

    public class UrnasPos17hResumoDto
    {
        public int TotalUrnas { get; set; }
        public int TotalVotos { get; set; }
        public int TotalEleitoresAptos { get; set; }
        public decimal PercentualComparecimento { get; set; }
        public string HorarioUltimoFechamento { get; set; } = "19h00";
        public int VotosCand1 { get; set; }
        public decimal PercentualCand1 { get; set; }
        public string NomeCand1 { get; set; } = "Flávio Bolsonaro";
        public string PartidoCand1 { get; set; } = "PL";
        public string CorCand1 { get; set; } = "#3b82f6";
        public int VotosCand2 { get; set; }
        public decimal PercentualCand2 { get; set; }
        public string NomeCand2 { get; set; } = "Lula";
        public string PartidoCand2 { get; set; } = "PT";
        public string CorCand2 { get; set; } = "#ef4444";
        public int VotosBrancosNulos { get; set; }
        public List<string> UfsAfetadas { get; set; } = new();
        public List<UrnaPos17hItem> SeçõesDestaque { get; set; } = new();
    }
}
