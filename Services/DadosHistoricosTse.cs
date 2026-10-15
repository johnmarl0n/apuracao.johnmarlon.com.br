using System;
using System.Collections.Generic;
using apuracao.johnmarlon.com.br.Models;

namespace apuracao.johnmarlon.com.br.Services
{
    public static class DadosHistoricosTse
    {
        public static bool PreencherDadosHistoricos(ApuracaoViewModel vm, string ano, string cargo, string uf)
        {
            // Apenas cargos principais gerais (Presidente, Governador, Senador, Dep. Federal, Dep. Estadual)
            vm.Finalizado = true;
            vm.AoVivo = false;
            vm.DataHoraTse = $"Resultado Oficial Homologado TSE ({ano})";
            vm.PercentualTotalizado = 100.00m;

            switch (ano)
            {
                case "2022":
                    return Preencher2022(vm, cargo, uf);
                case "2018":
                    return Preencher2018(vm, cargo, uf);
                case "2014":
                    return Preencher2014(vm, cargo, uf);
                case "2010":
                    return Preencher2010(vm, cargo, uf);
                case "2006":
                    return Preencher2006(vm, cargo, uf);
                case "2002":
                    return Preencher2002(vm, cargo, uf);
                default:
                    return false;
            }
        }

        private static bool Preencher2022(ApuracaoViewModel vm, string cargo, string uf)
        {
            if (cargo == "1") // Presidente 2022
            {
                vm.TotalSecoes = 472075;
                vm.SecoesTotalizadas = 472075;
                vm.TotalEleitores = 156454011;
                vm.Comparecimento = 123682372;
                vm.PercentualComparecimento = 79.05m;
                vm.Abstencao = 32770982;
                vm.PercentualAbstencao = 20.95m;
                vm.VotosValidos = 118229719;
                vm.PercentualVotosValidos = 95.59m;
                vm.VotosBrancos = 1964779;
                vm.PercentualVotosBrancos = 1.59m;
                vm.VotosNulos = 3487874;
                vm.PercentualVotosNulos = 2.82m;

                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "13", NomeUrna = "Lula", NomeCompleto = "Luiz Inácio Lula da Silva", Partido = "PT", Votos = 57259504, Percentual = 48.43m, Situacao = "2º Turno", Eleito = true, ViceNome = "Geraldo Alckmin", VicePartido = "PSB", CorPartido = "#cc0000" },
                    new() { Numero = "22", NomeUrna = "Jair Bolsonaro", NomeCompleto = "Jair Messias Bolsonaro", Partido = "PL", Votos = 51072345, Percentual = 43.20m, Situacao = "2º Turno", Eleito = false, ViceNome = "Braga Netto", VicePartido = "PL", CorPartido = "#223974" },
                    new() { Numero = "15", NomeUrna = "Simone Tebet", NomeCompleto = "Simone Nassar Tebet", Partido = "MDB", Votos = 4915423, Percentual = 4.16m, Situacao = "Não Eleito", Eleito = false, ViceNome = "Mara Gabrilli", VicePartido = "PSDB", CorPartido = "#009b3a" },
                    new() { Numero = "12", NomeUrna = "Ciro Gomes", NomeCompleto = "Ciro Ferreira Gomes", Partido = "PDT", Votos = 3599287, Percentual = 3.04m, Situacao = "Não Eleito", Eleito = false, ViceNome = "Ana Paula Matos", VicePartido = "PDT", CorPartido = "#ff8c00" },
                    new() { Numero = "44", NomeUrna = "Soraya Thronicke", NomeCompleto = "Soraya Vieira Thronicke", Partido = "UNIÃO", Votos = 600955, Percentual = 0.51m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#005baa" },
                    new() { Numero = "30", NomeUrna = "Felipe d'Avila", NomeCompleto = "Luiz Felipe d'Avila", Partido = "NOVO", Votos = 559708, Percentual = 0.47m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#f58220" },
                    new() { Numero = "14", NomeUrna = "Padre Kelmon", NomeCompleto = "Kelmon Luis da Silva Souza", Partido = "PTB", Votos = 81129, Percentual = 0.07m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#555555" },
                    new() { Numero = "80", NomeUrna = "Léo Péricles", NomeCompleto = "Leonardo Péricles Vieira Roque", Partido = "UP", Votos = 53519, Percentual = 0.05m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#000000" }
                };
                return true;
            }
            if (cargo == "3") // Governador SP 2022
            {
                vm.TotalSecoes = 101480;
                vm.SecoesTotalizadas = 101480;
                vm.TotalEleitores = 34667793;
                vm.Comparecimento = 27150537;
                vm.PercentualComparecimento = 78.32m;
                vm.Abstencao = 7517256;
                vm.PercentualAbstencao = 21.68m;
                vm.VotosValidos = 23351492;
                vm.PercentualVotosValidos = 86.01m;
                vm.VotosBrancos = 1684347;
                vm.PercentualVotosBrancos = 6.20m;
                vm.VotosNulos = 2114698;
                vm.PercentualVotosNulos = 7.79m;

                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "10", NomeUrna = "Tarcísio de Freitas", NomeCompleto = "Tarcísio Gomes de Freitas", Partido = "REPUBLICANOS", Votos = 9881995, Percentual = 42.32m, Situacao = "2º Turno", Eleito = true, ViceNome = "Felicio Ramuth", VicePartido = "PSD", CorPartido = "#003366" },
                    new() { Numero = "13", NomeUrna = "Fernando Haddad", NomeCompleto = "Fernando Haddad", Partido = "PT", Votos = 8337139, Percentual = 35.70m, Situacao = "2º Turno", Eleito = false, ViceNome = "Lúcia França", VicePartido = "PSB", CorPartido = "#cc0000" },
                    new() { Numero = "45", NomeUrna = "Rodrigo Garcia", NomeCompleto = "Rodrigo Garcia", Partido = "PSDB", Votos = 4296293, Percentual = 18.40m, Situacao = "Não Eleito", Eleito = false, ViceNome = "Eugênio Zuliani", VicePartido = "UNIÃO", CorPartido = "#0047ba" },
                    new() { Numero = "30", NomeUrna = "Vinicius Poit", NomeCompleto = "Vinicius de Carvalho Poit", Partido = "NOVO", Votos = 388974, Percentual = 1.67m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#f58220" },
                    new() { Numero = "12", NomeUrna = "Elvis Cezar", NomeCompleto = "Elvis Leonardo Cezar", Partido = "PDT", Votos = 281712, Percentual = 1.21m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#ff8c00" },
                    new() { Numero = "80", NomeUrna = "Carol Vigliar", NomeCompleto = "Carolina Vigliar", Partido = "UP", Votos = 89743, Percentual = 0.38m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#000000" }
                };
                return true;
            }
            if (cargo == "5") // Senador SP 2022
            {
                vm.TotalSecoes = 101480;
                vm.SecoesTotalizadas = 101480;
                vm.TotalEleitores = 34667793;
                vm.VotosValidos = 21567871;
                vm.PercentualVotosValidos = 79.44m;
                vm.VotosBrancos = 2365287;
                vm.PercentualVotosBrancos = 8.71m;
                vm.VotosNulos = 3217379;
                vm.PercentualVotosNulos = 11.85m;
                vm.Comparecimento = 27150537;
                vm.PercentualComparecimento = 78.32m;
                vm.Abstencao = 7517256;
                vm.PercentualAbstencao = 21.68m;

                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "222", NomeUrna = "Marcos Pontes", NomeCompleto = "Marcos Cesar Pontes", Partido = "PL", Votos = 10714913, Percentual = 49.68m, Situacao = "Eleito", Eleito = true, CorPartido = "#223974" },
                    new() { Numero = "400", NomeUrna = "Márcio França", NomeCompleto = "Márcio Luiz França Gomes", Partido = "PSB", Votos = 7822518, Percentual = 36.27m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#ffcc00" },
                    new() { Numero = "155", NomeUrna = "Edson Aparecido", NomeCompleto = "Edson Aparecido dos Santos", Partido = "MDB", Votos = 1655224, Percentual = 7.67m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#009b3a" },
                    new() { Numero = "288", NomeUrna = "Janaina Paschoal", NomeCompleto = "Janaina Conceição Paschoal", Partido = "PRTB", Votos = 447550, Percentual = 2.07m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#113355" },
                    new() { Numero = "300", NomeUrna = "Ricardo Mellão", NomeCompleto = "Ricardo Mellão", Partido = "NOVO", Votos = 311321, Percentual = 1.44m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#f58220" },
                    new() { Numero = "800", NomeUrna = "Vivian Mendes", NomeCompleto = "Vivian Mendes da Silva", Partido = "UP", Votos = 280460, Percentual = 1.30m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#000000" }
                };
                return true;
            }
            if (cargo == "6") // Deputado Federal SP 2022
            {
                vm.TotalSecoes = 101480;
                vm.SecoesTotalizadas = 101480;
                vm.TotalEleitores = 34667793;
                vm.VotosValidos = 23719876;
                vm.PercentualVotosValidos = 87.36m;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "5010", NomeUrna = "Guilherme Boulos", NomeCompleto = "Guilherme Castro Boulos", Partido = "PSOL", Votos = 1001472, Percentual = 4.22m, Situacao = "Eleito", Eleito = true, CorPartido = "#ffff00" },
                    new() { Numero = "2210", NomeUrna = "Carla Zambelli", NomeCompleto = "Carla Zambelli Salgado", Partido = "PL", Votos = 946244, Percentual = 3.99m, Situacao = "Eleito", Eleito = true, CorPartido = "#223974" },
                    new() { Numero = "2222", NomeUrna = "Eduardo Bolsonaro", NomeCompleto = "Eduardo Nantes Bolsonaro", Partido = "PL", Votos = 741701, Percentual = 3.13m, Situacao = "Eleito", Eleito = true, CorPartido = "#223974" },
                    new() { Numero = "2233", NomeUrna = "Ricardo Salles", NomeCompleto = "Ricardo de Aquino Salles", Partido = "PL", Votos = 640918, Percentual = 2.70m, Situacao = "Eleito", Eleito = true, CorPartido = "#223974" },
                    new() { Numero = "1100", NomeUrna = "Delegado Bruno Lima", NomeCompleto = "Bruno Lima", Partido = "PP", Votos = 461218, Percentual = 1.94m, Situacao = "Eleito", Eleito = true, CorPartido = "#0047ba" },
                    new() { Numero = "4000", NomeUrna = "Tabata Amaral", NomeCompleto = "Tabata Claudia Amaral de Pontes", Partido = "PSB", Votos = 337873, Percentual = 1.42m, Situacao = "Eleito", Eleito = true, CorPartido = "#ffcc00" },
                    new() { Numero = "1010", NomeUrna = "Celso Russomanno", NomeCompleto = "Celso Ubirajara Russomanno", Partido = "REPUBLICANOS", Votos = 305520, Percentual = 1.29m, Situacao = "Eleito", Eleito = true, CorPartido = "#003366" },
                    new() { Numero = "4433", NomeUrna = "Kim Kataguiri", NomeCompleto = "Kim Patroca Kataguiri", Partido = "UNIÃO", Votos = 295460, Percentual = 1.25m, Situacao = "Eleito", Eleito = true, CorPartido = "#005baa" },
                    new() { Numero = "5000", NomeUrna = "Erika Hilton", NomeCompleto = "Erika Santos Silva", Partido = "PSOL", Votos = 256903, Percentual = 1.08m, Situacao = "Eleito", Eleito = true, CorPartido = "#ffff00" },
                    new() { Numero = "1500", NomeUrna = "Delegado Palumbo", NomeCompleto = "Mario Palumbo Junior", Partido = "MDB", Votos = 254898, Percentual = 1.07m, Situacao = "Eleito", Eleito = true, CorPartido = "#009b3a" },
                    new() { Numero = "4040", NomeUrna = "Jonas Donizette", NomeCompleto = "Jonas Donizette Ferreira", Partido = "PSB", Votos = 84044, Percentual = 0.35m, Situacao = "Eleito (Campinas)", Eleito = true, CorPartido = "#ffcc00" }
                };
                return true;
            }
            if (cargo == "7") // Deputado Estadual SP 2022
            {
                vm.TotalSecoes = 101480;
                vm.SecoesTotalizadas = 101480;
                vm.TotalEleitores = 34667793;
                vm.VotosValidos = 23450912;
                vm.PercentualVotosValidos = 86.37m;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "13133", NomeUrna = "Eduardo Suplicy", NomeCompleto = "Eduardo Matarazzo Suplicy", Partido = "PT", Votos = 807015, Percentual = 3.44m, Situacao = "Eleito", Eleito = true, CorPartido = "#cc0000" },
                    new() { Numero = "50123", NomeUrna = "Carlos Giannazi", NomeCompleto = "Carlos Alberto Giannazi", Partido = "PSOL", Votos = 276814, Percentual = 1.18m, Situacao = "Eleito", Eleito = true, CorPartido = "#ffff00" },
                    new() { Numero = "50000", NomeUrna = "Paula da Bancada Feminista", NomeCompleto = "Paula Aparecida Ferreira", Partido = "PSOL", Votos = 259771, Percentual = 1.11m, Situacao = "Eleito", Eleito = true, CorPartido = "#ffff00" },
                    new() { Numero = "10123", NomeUrna = "Tomé Abduch", NomeCompleto = "Tomé Abduch", Partido = "REPUBLICANOS", Votos = 221656, Percentual = 0.94m, Situacao = "Eleito", Eleito = true, CorPartido = "#003366" },
                    new() { Numero = "22123", NomeUrna = "André do Prado", NomeCompleto = "André do Prado", Partido = "PL", Votos = 216268, Percentual = 0.92m, Situacao = "Eleito", Eleito = true, CorPartido = "#223974" },
                    new() { Numero = "22222", NomeUrna = "Gil Diniz", NomeCompleto = "Gil Diniz", Partido = "PL", Votos = 196216, Percentual = 0.84m, Situacao = "Eleito", Eleito = true, CorPartido = "#223974" },
                    new() { Numero = "22000", NomeUrna = "Lucas Bove", NomeCompleto = "Lucas Bove", Partido = "PL", Votos = 130451, Percentual = 0.56m, Situacao = "Eleito", Eleito = true, CorPartido = "#223974" },
                    new() { Numero = "23123", NomeUrna = "Rafa Zimbaldi", NomeCompleto = "Rafael Zimbaldi", Partido = "CIDADANIA", Votos = 76910, Percentual = 0.33m, Situacao = "Eleito (Campinas)", Eleito = true, CorPartido = "#ea1d2c" }
                };
                return true;
            }
            return false;
        }

        private static bool Preencher2018(ApuracaoViewModel vm, string cargo, string uf)
        {
            if (cargo == "1") // Presidente 2018
            {
                vm.TotalSecoes = 454490;
                vm.SecoesTotalizadas = 454490;
                vm.TotalEleitores = 147305696;
                vm.Comparecimento = 117364560;
                vm.PercentualComparecimento = 79.67m;
                vm.Abstencao = 29941136;
                vm.PercentualAbstencao = 20.33m;
                vm.VotosValidos = 107050674;
                vm.PercentualVotosValidos = 91.21m;
                vm.VotosBrancos = 3106936;
                vm.PercentualVotosBrancos = 2.65m;
                vm.VotosNulos = 7206205;
                vm.PercentualVotosNulos = 6.14m;

                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "17", NomeUrna = "Jair Bolsonaro", NomeCompleto = "Jair Messias Bolsonaro", Partido = "PSL", Votos = 49276990, Percentual = 46.03m, Situacao = "2º Turno", Eleito = true, ViceNome = "General Mourão", VicePartido = "PRTB", CorPartido = "#003366" },
                    new() { Numero = "13", NomeUrna = "Fernando Haddad", NomeCompleto = "Fernando Haddad", Partido = "PT", Votos = 31342005, Percentual = 29.28m, Situacao = "2º Turno", Eleito = false, ViceNome = "Manuela d'Ávila", VicePartido = "PCdoB", CorPartido = "#cc0000" },
                    new() { Numero = "12", NomeUrna = "Ciro Gomes", NomeCompleto = "Ciro Ferreira Gomes", Partido = "PDT", Votos = 13344366, Percentual = 12.47m, Situacao = "Não Eleito", Eleito = false, ViceNome = "Kátia Abreu", VicePartido = "PDT", CorPartido = "#ff8c00" },
                    new() { Numero = "45", NomeUrna = "Geraldo Alckmin", NomeCompleto = "Geraldo José Rodrigues Alckmin Filho", Partido = "PSDB", Votos = 5096349, Percentual = 4.76m, Situacao = "Não Eleito", Eleito = false, ViceNome = "Ana Amélia", VicePartido = "PP", CorPartido = "#0047ba" },
                    new() { Numero = "30", NomeUrna = "João Amoêdo", NomeCompleto = "João Dionisio Filgueiras Amoêdo", Partido = "NOVO", Votos = 2679744, Percentual = 2.50m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#f58220" },
                    new() { Numero = "51", NomeUrna = "Cabo Daciolo", NomeCompleto = "Benevenuto Daciolo Fonseca dos Santos", Partido = "PATRIOTA", Votos = 1348323, Percentual = 1.26m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#223974" },
                    new() { Numero = "15", NomeUrna = "Henrique Meirelles", NomeCompleto = "Henrique de Campos Meirelles", Partido = "MDB", Votos = 1288948, Percentual = 1.20m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#009b3a" },
                    new() { Numero = "18", NomeUrna = "Marina Silva", NomeCompleto = "Maria Osmarina Marina Silva Vaz de Lima", Partido = "REDE", Votos = 1069538, Percentual = 1.00m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#00a859" }
                };
                return true;
            }
            if (cargo == "3") // Governador SP 2018
            {
                vm.TotalSecoes = 97000;
                vm.SecoesTotalizadas = 97000;
                vm.TotalEleitores = 33000000;
                vm.VotosValidos = 20244601;
                vm.PercentualVotosValidos = 83.75m;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "45", NomeUrna = "João Doria", NomeCompleto = "João Agripino da Costa Doria Junior", Partido = "PSDB", Votos = 6431555, Percentual = 31.77m, Situacao = "2º Turno", Eleito = true, ViceNome = "Rodrigo Garcia", VicePartido = "DEM", CorPartido = "#0047ba" },
                    new() { Numero = "40", NomeUrna = "Márcio França", NomeCompleto = "Márcio Luiz França Gomes", Partido = "PSB", Votos = 4358998, Percentual = 21.53m, Situacao = "2º Turno", Eleito = false, ViceNome = "Coronel Eliane Nikoluk", VicePartido = "PR", CorPartido = "#ffcc00" },
                    new() { Numero = "15", NomeUrna = "Paulo Skaf", NomeCompleto = "Paulo Antonio Skaf", Partido = "MDB", Votos = 4269865, Percentual = 21.09m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#009b3a" },
                    new() { Numero = "13", NomeUrna = "Luiz Marinho", NomeCompleto = "Luiz Marinho", Partido = "PT", Votos = 2563922, Percentual = 12.66m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#cc0000" },
                    new() { Numero = "27", NomeUrna = "Major Costa e Silva", NomeCompleto = "Paulo Sergio Costa e Silva", Partido = "DC", Votos = 747462, Percentual = 3.69m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#003366" },
                    new() { Numero = "30", NomeUrna = "Rogério Chequer", NomeCompleto = "Rogério Chequer", Partido = "NOVO", Votos = 673102, Percentual = 3.32m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#f58220" }
                };
                return true;
            }
            if (cargo == "5") // Senador SP 2018 (2 eleitos)
            {
                vm.TotalSecoes = 97000;
                vm.SecoesTotalizadas = 97000;
                vm.VotosValidos = 35021720;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "177", NomeUrna = "Major Olimpio", NomeCompleto = "Sérgio Olímpio Gomes", Partido = "PSL", Votos = 9039717, Percentual = 25.81m, Situacao = "Eleito", Eleito = true, CorPartido = "#003366" },
                    new() { Numero = "457", NomeUrna = "Mara Gabrilli", NomeCompleto = "Mara Cristina Gabrilli", Partido = "PSDB", Votos = 6513282, Percentual = 18.59m, Situacao = "Eleito", Eleito = true, CorPartido = "#0047ba" },
                    new() { Numero = "131", NomeUrna = "Eduardo Suplicy", NomeCompleto = "Eduardo Matarazzo Suplicy", Partido = "PT", Votos = 4667565, Percentual = 13.32m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#cc0000" },
                    new() { Numero = "456", NomeUrna = "Ricardo Tripoli", NomeCompleto = "José Ricardo Alvarenga Tripoli", Partido = "PSDB", Votos = 3154673, Percentual = 9.00m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#0047ba" },
                    new() { Numero = "400", NomeUrna = "Maurren Maggi", NomeCompleto = "Maurren Higa Maggi", Partido = "PSB", Votos = 2844414, Percentual = 8.12m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#ffcc00" },
                    new() { Numero = "132", NomeUrna = "Jilmar Tatto", NomeCompleto = "Jilmar Augustinho Tatto", Partido = "PT", Votos = 2103377, Percentual = 6.00m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#cc0000" }
                };
                return true;
            }
            if (cargo == "6" || cargo == "7") // Deputados 2018
            {
                vm.TotalSecoes = 97000;
                vm.SecoesTotalizadas = 97000;
                vm.VotosValidos = 21000000;
                vm.Candidatos = cargo == "6"
                    ? new List<CandidatoItemViewModel>
                    {
                        new() { Numero = "1722", NomeUrna = "Eduardo Bolsonaro", NomeCompleto = "Eduardo Nantes Bolsonaro", Partido = "PSL", Votos = 1843735, Percentual = 8.74m, Situacao = "Eleito", Eleito = true, CorPartido = "#003366" },
                        new() { Numero = "1771", NomeUrna = "Joice Hasselmann", NomeCompleto = "Joice Cristina Hasselmann", Partido = "PSL", Votos = 1078666, Percentual = 5.11m, Situacao = "Eleito", Eleito = true, CorPartido = "#003366" },
                        new() { Numero = "1010", NomeUrna = "Celso Russomanno", NomeCompleto = "Celso Ubirajara Russomanno", Partido = "PRB", Votos = 521728, Percentual = 2.47m, Situacao = "Eleito", Eleito = true, CorPartido = "#003366" },
                        new() { Numero = "2533", NomeUrna = "Kim Kataguiri", NomeCompleto = "Kim Patroca Kataguiri", Partido = "DEM", Votos = 465310, Percentual = 2.21m, Situacao = "Eleito", Eleito = true, CorPartido = "#005baa" },
                        new() { Numero = "2222", NomeUrna = "Tiririca", NomeCompleto = "Francisco Everardo Oliveira Silva", Partido = "PR", Votos = 445528, Percentual = 2.11m, Situacao = "Eleito", Eleito = true, CorPartido = "#223974" },
                        new() { Numero = "1200", NomeUrna = "Tabata Amaral", NomeCompleto = "Tabata Claudia Amaral de Pontes", Partido = "PDT", Votos = 264450, Percentual = 1.25m, Situacao = "Eleito", Eleito = true, CorPartido = "#ff8c00" }
                    }
                    : new List<CandidatoItemViewModel>
                    {
                        new() { Numero = "17000", NomeUrna = "Janaina Paschoal", NomeCompleto = "Janaina Conceição Paschoal", Partido = "PSL", Votos = 2060786, Percentual = 9.88m, Situacao = "Eleito", Eleito = true, CorPartido = "#003366" },
                        new() { Numero = "25555", NomeUrna = "Arthur do Val", NomeCompleto = "Arthur Moledo do Val", Partido = "DEM", Votos = 478280, Percentual = 2.29m, Situacao = "Eleito", Eleito = true, CorPartido = "#005baa" },
                        new() { Numero = "50123", NomeUrna = "Carlos Giannazi", NomeCompleto = "Carlos Alberto Giannazi", Partido = "PSOL", Votos = 218705, Percentual = 1.05m, Situacao = "Eleito", Eleito = true, CorPartido = "#ffff00" },
                        new() { Numero = "11190", NomeUrna = "Coronel Telhada", NomeCompleto = "Paulo Adriano Lopes Lucinda Telhada", Partido = "PP", Votos = 214445, Percentual = 1.03m, Situacao = "Eleito", Eleito = true, CorPartido = "#0047ba" }
                    };
                return true;
            }
            return false;
        }

        private static bool Preencher2014(ApuracaoViewModel vm, string cargo, string uf)
        {
            if (cargo == "1") // Presidente 2014
            {
                vm.TotalSecoes = 428894;
                vm.SecoesTotalizadas = 428894;
                vm.TotalEleitores = 142822337;
                vm.Comparecimento = 115122902;
                vm.PercentualComparecimento = 80.61m;
                vm.Abstencao = 27699435;
                vm.PercentualAbstencao = 19.39m;
                vm.VotosValidos = 104023834;
                vm.PercentualVotosValidos = 90.36m;
                vm.VotosBrancos = 4420488;
                vm.PercentualVotosBrancos = 3.84m;
                vm.VotosNulos = 6678580;
                vm.PercentualVotosNulos = 5.80m;

                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "13", NomeUrna = "Dilma Rousseff", NomeCompleto = "Dilma Vana Rousseff", Partido = "PT", Votos = 43267668, Percentual = 41.59m, Situacao = "2º Turno", Eleito = true, ViceNome = "Michel Temer", VicePartido = "PMDB", CorPartido = "#cc0000" },
                    new() { Numero = "45", NomeUrna = "Aécio Neves", NomeCompleto = "Aécio Neves da Cunha", Partido = "PSDB", Votos = 34897211, Percentual = 33.55m, Situacao = "2º Turno", Eleito = false, ViceNome = "Aloysio Nunes", VicePartido = "PSDB", CorPartido = "#0047ba" },
                    new() { Numero = "40", NomeUrna = "Marina Silva", NomeCompleto = "Maria Osmarina Marina Silva Vaz de Lima", Partido = "PSB", Votos = 22176619, Percentual = 21.32m, Situacao = "Não Eleito", Eleito = false, ViceNome = "Beto Albuquerque", VicePartido = "PSB", CorPartido = "#ffcc00" },
                    new() { Numero = "50", NomeUrna = "Luciana Genro", NomeCompleto = "Luciana Krebs Genro", Partido = "PSOL", Votos = 1612186, Percentual = 1.55m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#ffff00" },
                    new() { Numero = "20", NomeUrna = "Pastor Everaldo", NomeCompleto = "Everaldo Dias Pereira", Partido = "PSC", Votos = 780513, Percentual = 0.75m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#008000" },
                    new() { Numero = "43", NomeUrna = "Eduardo Jorge", NomeCompleto = "Eduardo Jorge Martins Alves Sobrinho", Partido = "PV", Votos = 630099, Percentual = 0.61m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#00a859" }
                };
                return true;
            }
            if (cargo == "3") // Governador SP 2014
            {
                vm.TotalSecoes = 92000;
                vm.SecoesTotalizadas = 92000;
                vm.VotosValidos = 21341670;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "45", NomeUrna = "Geraldo Alckmin", NomeCompleto = "Geraldo José Rodrigues Alckmin Filho", Partido = "PSDB", Votos = 12230807, Percentual = 57.31m, Situacao = "Eleito em 1º Turno", Eleito = true, ViceNome = "Márcio França", VicePartido = "PSB", CorPartido = "#0047ba" },
                    new() { Numero = "15", NomeUrna = "Paulo Skaf", NomeCompleto = "Paulo Antonio Skaf", Partido = "PMDB", Votos = 4594708, Percentual = 21.53m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#009b3a" },
                    new() { Numero = "13", NomeUrna = "Alexandre Padilha", NomeCompleto = "Alexandre Rocha Santos Padilha", Partido = "PT", Votos = 3888584, Percentual = 18.22m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#cc0000" },
                    new() { Numero = "43", NomeUrna = "Gilberto Natalini", NomeCompleto = "Gilberto Natalini", Partido = "PV", Votos = 260696, Percentual = 1.22m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#00a859" }
                };
                return true;
            }
            if (cargo == "5") // Senador SP 2014
            {
                vm.TotalSecoes = 92000;
                vm.SecoesTotalizadas = 92000;
                vm.VotosValidos = 18987000;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "456", NomeUrna = "José Serra", NomeCompleto = "José Serra", Partido = "PSDB", Votos = 11105874, Percentual = 58.49m, Situacao = "Eleito", Eleito = true, CorPartido = "#0047ba" },
                    new() { Numero = "131", NomeUrna = "Eduardo Suplicy", NomeCompleto = "Eduardo Matarazzo Suplicy", Partido = "PT", Votos = 6176499, Percentual = 32.53m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#cc0000" },
                    new() { Numero = "555", NomeUrna = "Gilberto Kassab", NomeCompleto = "Gilberto Kassab", Partido = "PSD", Votos = 1128582, Percentual = 5.94m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#003366" }
                };
                return true;
            }
            if (cargo == "6" || cargo == "7")
            {
                vm.TotalSecoes = 92000;
                vm.SecoesTotalizadas = 92000;
                vm.VotosValidos = 21000000;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "1010", NomeUrna = "Celso Russomanno", NomeCompleto = "Celso Ubirajara Russomanno", Partido = "PRB", Votos = 1524361, Percentual = 7.07m, Situacao = "Eleito", Eleito = true, CorPartido = "#003366" },
                    new() { Numero = "2222", NomeUrna = "Tiririca", NomeCompleto = "Francisco Everardo Oliveira Silva", Partido = "PR", Votos = 1016796, Percentual = 4.72m, Situacao = "Eleito", Eleito = true, CorPartido = "#223974" },
                    new() { Numero = "2020", NomeUrna = "Marco Feliciano", NomeCompleto = "Marco Antônio Feliciano", Partido = "PSC", Votos = 398087, Percentual = 1.85m, Situacao = "Eleito", Eleito = true, CorPartido = "#008000" },
                    new() { Numero = "4500", NomeUrna = "Bruno Covas", NomeCompleto = "Bruno Covas Lopes", Partido = "PSDB", Votos = 352708, Percentual = 1.64m, Situacao = "Eleito", Eleito = true, CorPartido = "#0047ba" },
                    new() { Numero = "4567", NomeUrna = "Carlos Sampaio", NomeCompleto = "Carlos Henrique Focesi Sampaio", Partido = "PSDB", Votos = 295623, Percentual = 1.37m, Situacao = "Eleito (Campinas)", Eleito = true, CorPartido = "#0047ba" }
                };
                return true;
            }
            return false;
        }

        private static bool Preencher2010(ApuracaoViewModel vm, string cargo, string uf)
        {
            if (cargo == "1") // Presidente 2010
            {
                vm.TotalSecoes = 400000;
                vm.SecoesTotalizadas = 400000;
                vm.TotalEleitores = 135804235;
                vm.Comparecimento = 111194012;
                vm.PercentualComparecimento = 81.88m;
                vm.Abstencao = 24610223;
                vm.PercentualAbstencao = 18.12m;
                vm.VotosValidos = 101590418;
                vm.PercentualVotosValidos = 91.36m;
                vm.VotosBrancos = 3479340;
                vm.PercentualVotosBrancos = 3.13m;
                vm.VotosNulos = 6124254;
                vm.PercentualVotosNulos = 5.51m;

                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "13", NomeUrna = "Dilma Rousseff", NomeCompleto = "Dilma Vana Rousseff", Partido = "PT", Votos = 47651434, Percentual = 46.91m, Situacao = "2º Turno", Eleito = true, ViceNome = "Michel Temer", VicePartido = "PMDB", CorPartido = "#cc0000" },
                    new() { Numero = "45", NomeUrna = "José Serra", NomeCompleto = "José Serra", Partido = "PSDB", Votos = 33132283, Percentual = 32.61m, Situacao = "2º Turno", Eleito = false, ViceNome = "Indio da Costa", VicePartido = "DEM", CorPartido = "#0047ba" },
                    new() { Numero = "43", NomeUrna = "Marina Silva", NomeCompleto = "Maria Osmarina Marina Silva Vaz de Lima", Partido = "PV", Votos = 19636359, Percentual = 19.33m, Situacao = "Não Eleito", Eleito = false, ViceNome = "Guilherme Leal", VicePartido = "PV", CorPartido = "#00a859" },
                    new() { Numero = "50", NomeUrna = "Plínio de Arruda Sampaio", NomeCompleto = "Plínio Soares de Arruda Sampaio", Partido = "PSOL", Votos = 886816, Percentual = 0.87m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#ffff00" }
                };
                return true;
            }
            if (cargo == "3") // Governador SP 2010
            {
                vm.TotalSecoes = 87000;
                vm.SecoesTotalizadas = 87000;
                vm.VotosValidos = 22753564;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "45", NomeUrna = "Geraldo Alckmin", NomeCompleto = "Geraldo José Rodrigues Alckmin Filho", Partido = "PSDB", Votos = 11519014, Percentual = 50.63m, Situacao = "Eleito em 1º Turno", Eleito = true, ViceNome = "Guilherme Afif Domingos", VicePartido = "DEM", CorPartido = "#0047ba" },
                    new() { Numero = "13", NomeUrna = "Aloizio Mercadante", NomeCompleto = "Aloizio Mercadante Oliva", Partido = "PT", Votos = 8016466, Percentual = 35.23m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#cc0000" },
                    new() { Numero = "11", NomeUrna = "Celso Russomanno", NomeCompleto = "Celso Ubirajara Russomanno", Partido = "PP", Votos = 1233897, Percentual = 5.42m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#0047ba" },
                    new() { Numero = "40", NomeUrna = "Paulo Skaf", NomeCompleto = "Paulo Antonio Skaf", Partido = "PSB", Votos = 1038430, Percentual = 4.56m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#ffcc00" },
                    new() { Numero = "43", NomeUrna = "Fabio Feldmann", NomeCompleto = "Fabio Feldmann", Partido = "PV", Votos = 933003, Percentual = 4.10m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#00a859" }
                };
                return true;
            }
            if (cargo == "5") // Senador SP 2010 (2 eleitos)
            {
                vm.TotalSecoes = 87000;
                vm.SecoesTotalizadas = 87000;
                vm.VotosValidos = 36780000;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "456", NomeUrna = "Aloysio Nunes", NomeCompleto = "Aloysio Nunes Ferreira Filho", Partido = "PSDB", Votos = 11189168, Percentual = 30.42m, Situacao = "Eleito", Eleito = true, CorPartido = "#0047ba" },
                    new() { Numero = "131", NomeUrna = "Marta Suplicy", NomeCompleto = "Marta Teresa Smith de Vasconcellos Suplicy", Partido = "PT", Votos = 8314027, Percentual = 22.61m, Situacao = "Eleito", Eleito = true, CorPartido = "#cc0000" },
                    new() { Numero = "651", NomeUrna = "Netinho de Paula", NomeCompleto = "José de Paula Neto", Partido = "PCdoB", Votos = 7773327, Percentual = 21.14m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#b22222" },
                    new() { Numero = "433", NomeUrna = "Ricardo Young", NomeCompleto = "Ricardo Young Silva", Partido = "PV", Votos = 4117634, Percentual = 11.20m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#00a859" },
                    new() { Numero = "144", NomeUrna = "Romeu Tuma", NomeCompleto = "Romeu Tuma", Partido = "PTB", Votos = 3970169, Percentual = 10.80m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#555555" }
                };
                return true;
            }
            if (cargo == "6" || cargo == "7")
            {
                vm.TotalSecoes = 87000;
                vm.SecoesTotalizadas = 87000;
                vm.VotosValidos = 21000000;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "2222", NomeUrna = "Tiririca", NomeCompleto = "Francisco Everardo Oliveira Silva", Partido = "PR", Votos = 1353820, Percentual = 6.35m, Situacao = "Eleito", Eleito = true, CorPartido = "#223974" },
                    new() { Numero = "4000", NomeUrna = "Gabriel Chalita", NomeCompleto = "Gabriel Benedito Issaac Chalita", Partido = "PSB", Votos = 560022, Percentual = 2.63m, Situacao = "Eleito", Eleito = true, CorPartido = "#ffcc00" },
                    new() { Numero = "1111", NomeUrna = "Paulo Maluf", NomeCompleto = "Paulo Salim Maluf", Partido = "PP", Votos = 497203, Percentual = 2.33m, Situacao = "Eleito", Eleito = true, CorPartido = "#0047ba" },
                    new() { Numero = "4567", NomeUrna = "Carlos Sampaio", NomeCompleto = "Carlos Henrique Focesi Sampaio", Partido = "PSDB", Votos = 145585, Percentual = 0.68m, Situacao = "Eleito (Campinas)", Eleito = true, CorPartido = "#0047ba" }
                };
                return true;
            }
            return false;
        }

        private static bool Preencher2006(ApuracaoViewModel vm, string cargo, string uf)
        {
            if (cargo == "1") // Presidente 2006
            {
                vm.TotalSecoes = 375000;
                vm.SecoesTotalizadas = 375000;
                vm.TotalEleitores = 125970305;
                vm.Comparecimento = 105380061;
                vm.PercentualComparecimento = 83.25m;
                vm.Abstencao = 20590244;
                vm.PercentualAbstencao = 16.75m;
                vm.VotosValidos = 95996672;
                vm.PercentualVotosValidos = 91.09m;
                vm.VotosBrancos = 2866051;
                vm.PercentualVotosBrancos = 2.72m;
                vm.VotosNulos = 6517338;
                vm.PercentualVotosNulos = 6.18m;

                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "13", NomeUrna = "Luiz Inácio Lula da Silva", NomeCompleto = "Luiz Inácio Lula da Silva", Partido = "PT", Votos = 46662365, Percentual = 48.61m, Situacao = "2º Turno", Eleito = true, ViceNome = "José Alencar", VicePartido = "PRB", CorPartido = "#cc0000" },
                    new() { Numero = "45", NomeUrna = "Geraldo Alckmin", NomeCompleto = "Geraldo José Rodrigues Alckmin Filho", Partido = "PSDB", Votos = 39968369, Percentual = 41.64m, Situacao = "2º Turno", Eleito = false, ViceNome = "José Jorge", VicePartido = "PFL", CorPartido = "#0047ba" },
                    new() { Numero = "50", NomeUrna = "Heloísa Helena", NomeCompleto = "Heloísa Helena Lima de Moraes Carvalho", Partido = "PSOL", Votos = 6575393, Percentual = 6.85m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#ffff00" },
                    new() { Numero = "12", NomeUrna = "Cristovam Buarque", NomeCompleto = "Cristovam Ricardo Cavalcanti Buarque", Partido = "PDT", Votos = 2538844, Percentual = 2.64m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#ff8c00" }
                };
                return true;
            }
            if (cargo == "3") // Governador SP 2006
            {
                vm.TotalSecoes = 82000;
                vm.SecoesTotalizadas = 82000;
                vm.VotosValidos = 21371493;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "45", NomeUrna = "José Serra", NomeCompleto = "José Serra", Partido = "PSDB", Votos = 12381038, Percentual = 57.93m, Situacao = "Eleito em 1º Turno", Eleito = true, ViceNome = "Alberto Goldman", VicePartido = "PSDB", CorPartido = "#0047ba" },
                    new() { Numero = "13", NomeUrna = "Aloizio Mercadante", NomeCompleto = "Aloizio Mercadante Oliva", Partido = "PT", Votos = 6771582, Percentual = 31.68m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#cc0000" },
                    new() { Numero = "15", NomeUrna = "Orestes Quércia", NomeCompleto = "Orestes Quércia", Partido = "PMDB", Votos = 977770, Percentual = 4.57m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#009b3a" },
                    new() { Numero = "12", NomeUrna = "Carlos Apolinário", NomeCompleto = "Carlos Apolinário", Partido = "PDT", Votos = 430747, Percentual = 2.02m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#ff8c00" }
                };
                return true;
            }
            if (cargo == "5") // Senador SP 2006
            {
                vm.TotalSecoes = 82000;
                vm.SecoesTotalizadas = 82000;
                vm.VotosValidos = 18789000;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "131", NomeUrna = "Eduardo Suplicy", NomeCompleto = "Eduardo Matarazzo Suplicy", Partido = "PT", Votos = 8986803, Percentual = 47.82m, Situacao = "Eleito", Eleito = true, CorPartido = "#cc0000" },
                    new() { Numero = "251", NomeUrna = "Guilherme Afif Domingos", NomeCompleto = "Guilherme Afif Domingos", Partido = "PFL", Votos = 8212177, Percentual = 43.70m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#005baa" },
                    new() { Numero = "151", NomeUrna = "Alda Marco Antônio", NomeCompleto = "Alda Marco Antônio", Partido = "PMDB", Votos = 929179, Percentual = 4.94m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#009b3a" }
                };
                return true;
            }
            if (cargo == "6" || cargo == "7")
            {
                vm.TotalSecoes = 82000;
                vm.SecoesTotalizadas = 82000;
                vm.VotosValidos = 20000000;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "1111", NomeUrna = "Paulo Maluf", NomeCompleto = "Paulo Salim Maluf", Partido = "PP", Votos = 739827, Percentual = 3.58m, Situacao = "Eleito", Eleito = true, CorPartido = "#0047ba" },
                    new() { Numero = "1100", NomeUrna = "Celso Russomanno", NomeCompleto = "Celso Ubirajara Russomanno", Partido = "PP", Votos = 573520, Percentual = 2.78m, Situacao = "Eleito", Eleito = true, CorPartido = "#0047ba" },
                    new() { Numero = "3611", NomeUrna = "Clodovil Hernandes", NomeCompleto = "Clodovil Hernandes", Partido = "PTC", Votos = 493451, Percentual = 2.39m, Situacao = "Eleito", Eleito = true, CorPartido = "#555555" },
                    new() { Numero = "4040", NomeUrna = "Luiza Erundina", NomeCompleto = "Luiza Erundina de Sousa", Partido = "PSB", Votos = 395275, Percentual = 1.91m, Situacao = "Eleito", Eleito = true, CorPartido = "#ffcc00" },
                    new() { Numero = "4567", NomeUrna = "Carlos Sampaio", NomeCompleto = "Carlos Henrique Focesi Sampaio", Partido = "PSDB", Votos = 174141, Percentual = 0.84m, Situacao = "Eleito (Campinas)", Eleito = true, CorPartido = "#0047ba" }
                };
                return true;
            }
            return false;
        }

        private static bool Preencher2002(ApuracaoViewModel vm, string cargo, string uf)
        {
            if (cargo == "1") // Presidente 2002
            {
                vm.TotalSecoes = 350000;
                vm.SecoesTotalizadas = 350000;
                vm.TotalEleitores = 115261607;
                vm.Comparecimento = 94803854;
                vm.PercentualComparecimento = 82.25m;
                vm.Abstencao = 20457753;
                vm.PercentualAbstencao = 17.75m;
                vm.VotosValidos = 84954004;
                vm.PercentualVotosValidos = 89.61m;
                vm.VotosBrancos = 2873743;
                vm.PercentualVotosBrancos = 3.03m;
                vm.VotosNulos = 6976107;
                vm.PercentualVotosNulos = 7.36m;

                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "13", NomeUrna = "Luiz Inácio Lula da Silva", NomeCompleto = "Luiz Inácio Lula da Silva", Partido = "PT", Votos = 39455233, Percentual = 46.44m, Situacao = "2º Turno", Eleito = true, ViceNome = "José Alencar", VicePartido = "PL", CorPartido = "#cc0000" },
                    new() { Numero = "45", NomeUrna = "José Serra", NomeCompleto = "José Serra", Partido = "PSDB", Votos = 19705445, Percentual = 23.19m, Situacao = "2º Turno", Eleito = false, ViceNome = "Rita Camata", VicePartido = "PMDB", CorPartido = "#0047ba" },
                    new() { Numero = "40", NomeUrna = "Anthony Garotinho", NomeCompleto = "Anthony William Garotinho Matheus de Oliveira", Partido = "PSB", Votos = 15180097, Percentual = 17.86m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#ffcc00" },
                    new() { Numero = "23", NomeUrna = "Ciro Gomes", NomeCompleto = "Ciro Ferreira Gomes", Partido = "PPS", Votos = 10170882, Percentual = 11.97m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#ff8c00" }
                };
                return true;
            }
            if (cargo == "3") // Governador SP 2002
            {
                vm.TotalSecoes = 76000;
                vm.SecoesTotalizadas = 76000;
                vm.VotosValidos = 19605908;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "45", NomeUrna = "Geraldo Alckmin", NomeCompleto = "Geraldo José Rodrigues Alckmin Filho", Partido = "PSDB", Votos = 7505486, Percentual = 38.28m, Situacao = "2º Turno", Eleito = true, ViceNome = "Cláudio Lembo", VicePartido = "PFL", CorPartido = "#0047ba" },
                    new() { Numero = "13", NomeUrna = "José Genoino", NomeCompleto = "José Genoino Neto", Partido = "PT", Votos = 6368768, Percentual = 32.48m, Situacao = "2º Turno", Eleito = false, CorPartido = "#cc0000" },
                    new() { Numero = "11", NomeUrna = "Paulo Maluf", NomeCompleto = "Paulo Salim Maluf", Partido = "PPB", Votos = 4190106, Percentual = 21.37m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#0047ba" },
                    new() { Numero = "14", NomeUrna = "Antonio Ermírio de Moraes", NomeCompleto = "Antonio Ermírio de Moraes", Partido = "PTB", Votos = 1455334, Percentual = 7.42m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#555555" }
                };
                return true;
            }
            if (cargo == "5") // Senador SP 2002 (2 eleitos)
            {
                vm.TotalSecoes = 76000;
                vm.SecoesTotalizadas = 76000;
                vm.VotosValidos = 35123000;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "131", NomeUrna = "Aloizio Mercadante", NomeCompleto = "Aloizio Mercadante Oliva", Partido = "PT", Votos = 10497348, Percentual = 29.88m, Situacao = "Eleito", Eleito = true, CorPartido = "#cc0000" },
                    new() { Numero = "251", NomeUrna = "Romeu Tuma", NomeCompleto = "Romeu Tuma", Partido = "PFL", Votos = 7277407, Percentual = 20.72m, Situacao = "Eleito", Eleito = true, CorPartido = "#005baa" },
                    new() { Numero = "456", NomeUrna = "Antonio Carlos Pannunzio", NomeCompleto = "Antonio Carlos Pannunzio", Partido = "PSDB", Votos = 6787893, Percentual = 19.32m, Situacao = "Não Eleito", Eleito = false, CorPartido = "#0047ba" }
                };
                return true;
            }
            if (cargo == "6" || cargo == "7")
            {
                vm.TotalSecoes = 76000;
                vm.SecoesTotalizadas = 76000;
                vm.VotosValidos = 19600000;
                vm.Candidatos = new List<CandidatoItemViewModel>
                {
                    new() { Numero = "5656", NomeUrna = "Enéas Carneiro", NomeCompleto = "Enéas Ferreira Carneiro", Partido = "PRONA", Votos = 1573987, Percentual = 8.03m, Situacao = "Eleito", Eleito = true, CorPartido = "#003366" },
                    new() { Numero = "1313", NomeUrna = "José Dirceu", NomeCompleto = "José Dirceu de Oliveira e Silva", Partido = "PT", Votos = 556768, Percentual = 2.84m, Situacao = "Eleito", Eleito = true, CorPartido = "#cc0000" },
                    new() { Numero = "1515", NomeUrna = "Michel Temer", NomeCompleto = "Michel Miguel Elias Temer Lulia", Partido = "PMDB", Votos = 252229, Percentual = 1.29m, Situacao = "Eleito", Eleito = true, CorPartido = "#009b3a" },
                    new() { Numero = "1398", NomeUrna = "Antonio Palocci", NomeCompleto = "Antonio Palocci Filho", Partido = "PT", Votos = 247458, Percentual = 1.26m, Situacao = "Eleito", Eleito = true, CorPartido = "#cc0000" },
                    new() { Numero = "4567", NomeUrna = "Carlos Sampaio", NomeCompleto = "Carlos Henrique Focesi Sampaio", Partido = "PSDB", Votos = 146904, Percentual = 0.75m, Situacao = "Eleito (Campinas)", Eleito = true, CorPartido = "#0047ba" }
                };
                return true;
            }
            return false;
        }
    }
}
