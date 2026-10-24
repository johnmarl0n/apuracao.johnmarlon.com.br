using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using apuracao.johnmarlon.com.br.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace apuracao.johnmarlon.com.br.Services
{
    public class NoticiasApuracaoService
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger<NoticiasApuracaoService> _logger;
        private static readonly object _lock = new();

        public NoticiasApuracaoService(IHostEnvironment env, ILogger<NoticiasApuracaoService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public List<NoticiaItem> ObterNoticias(ApuracaoViewModel vm)
        {
            // Se for explicitamente o 1º Turno (Histórico das Eleições Gerais 2026)
            if (vm.Ano == "2026")
            {
                return ObterNoticiasPrimeiroTurno();
            }

            // Caso seja o 2º Turno (2026-2T)
            return GerarNoticiasSegundoTurno(vm);
        }

        private List<NoticiaItem> ObterNoticiasPrimeiroTurno()
        {
            return new List<NoticiaItem>
            {
                new NoticiaItem
                {
                    Hora = "12h51",
                    AvatarUrl = "/img/candidatos/douglas_ruas.webp",
                    ConteudoHtml = "Rio de Janeiro: <strong>Douglas Ruas</strong> é eleito governador.",
                    Tipo = "eleito"
                },
                new NoticiaItem
                {
                    Hora = "03h00",
                    AvatarUrl = "/img/candidatos/eduardo_braga.webp",
                    ConteudoHtml = "Amazonas: <strong>Eduardo Braga</strong> e <strong>Plínio Valério</strong> são eleitos para o Senado.",
                    Tipo = "eleito"
                },
                new NoticiaItem
                {
                    Hora = "03h00",
                    Icone = "bi-circle",
                    ConteudoHtml = "<div class=\"fw-bold mb-1\" style=\"color: var(--text-title);\">+1 seção</div><div><strong>Flávio Bolsonaro</strong> <span class=\"font-mono fw-bold\" style=\"color: #3b82f6;\">47,0%</span></div><div><strong>Lula</strong> <span class=\"font-mono fw-bold\" style=\"color: #ef4444;\">45,2%</span></div>",
                    Tipo = "apuracao"
                },
                new NoticiaItem
                {
                    Hora = "01h56",
                    Icone = "bi-circle",
                    ConteudoHtml = "<div class=\"fw-bold mb-1\" style=\"color: var(--text-title);\">+6 seções</div><div><strong>Flávio Bolsonaro</strong> <span class=\"font-mono fw-bold\" style=\"color: #3b82f6;\">47,0%</span></div><div><strong>Lula</strong> <span class=\"font-mono fw-bold\" style=\"color: #ef4444;\">45,2%</span></div>",
                    Tipo = "apuracao"
                },
                new NoticiaItem
                {
                    Hora = "00h47",
                    AvatarUrl = "/img/candidatos/domingos_savio.webp",
                    ConteudoHtml = "Minas Gerais: <strong>Domingos Sávio</strong> e <strong>Marília Campos</strong> são eleitos para o Senado.",
                    Tipo = "eleito"
                },
                new NoticiaItem
                {
                    Hora = "00h47",
                    AvatarUrl = "/img/candidatos/fufuca.webp",
                    ConteudoHtml = "Maranhão: <strong>Fufuca</strong> e <strong>Lahésio Bonfim</strong> são eleitos para o Senado.",
                    Tipo = "eleito"
                },
                new NoticiaItem
                {
                    Hora = "00h25",
                    AvatarUrl = "/img/candidatos/tarcisio.webp",
                    ConteudoHtml = "São Paulo: <strong>Tarcísio</strong> lidera apuração com <strong>62,7%</strong> dos votos válidos.",
                    Tipo = "lideranca"
                },
                new NoticiaItem
                {
                    Hora = "00h12",
                    AvatarUrl = "/img/candidatos/lula.webp",
                    ConteudoHtml = "Exterior: Urnas 100% totalizadas em Lisboa, Porto, Paris e Tóquio.",
                    Tipo = "apuracao"
                }
            };
        }

        private List<NoticiaItem> GerarNoticiasSegundoTurno(ApuracaoViewModel vm)
        {
            var noticias = new List<NoticiaItem>();

            // 1. Tentar ler notícias manuais de wwwroot/data/noticias.json
            var noticiasCustom = CarregarNoticiasCustomizadas();
            if (noticiasCustom.Count > 0)
            {
                noticias.AddRange(noticiasCustom);
            }

            var cand1 = vm.Candidatos.Count > 0 ? vm.Candidatos[0] : null;
            var cand2 = vm.Candidatos.Count > 1 ? vm.Candidatos[1] : null;

            // 2. Se a apuração ainda não começou (0% apurado)
            if (vm.PercentualTotalizado == 0 && vm.SecoesTotalizadas == 0)
            {
                // Marcos informativos do dia do 2º turno
                if (!noticias.Any(n => n.Id == "abertura_urnas"))
                {
                    noticias.Add(new NoticiaItem
                    {
                        Id = "abertura_urnas",
                        Hora = "17h00",
                        Icone = "bi-shield-check",
                        ConteudoHtml = "TSE: Urnas encerradas às 17h em todo o Brasil. Início da totalização oficial do 2º Turno.",
                        Tipo = "tse_urna",
                        Destaque = true
                    });
                }

                if (!noticias.Any(n => n.Id == "preparacao_tse"))
                {
                    noticias.Add(new NoticiaItem
                    {
                        Id = "preparacao_tse",
                        Hora = "16h00",
                        Icone = "bi-clock-history",
                        ConteudoHtml = "Reta final: Faltam poucos minutos para o encerramento da votação no horário de Brasília.",
                        Tipo = "aviso"
                    });
                }

                if (!noticias.Any(n => n.Id == "exterior_europa"))
                {
                    noticias.Add(new NoticiaItem
                    {
                        Id = "exterior_europa",
                        Hora = "13h00",
                        Icone = "bi-globe2",
                        ConteudoHtml = "Exterior: Votação encerrada nos consulados da Europa (Lisboa, Londres, Paris e Roma).",
                        Tipo = "aviso"
                    });
                }

                if (!noticias.Any(n => n.Id == "exterior_asia"))
                {
                    noticias.Add(new NoticiaItem
                    {
                        Id = "exterior_asia",
                        Hora = "10h00",
                        Icone = "bi-globe2",
                        ConteudoHtml = "Exterior: Votação 100% concluída na Ásia e Oceania (Tóquio, Seul, Sydney e Wellington).",
                        Tipo = "aviso"
                    });
                }

                if (!noticias.Any(n => n.Id == "abertura_nacional"))
                {
                    noticias.Add(new NoticiaItem
                    {
                        Id = "abertura_nacional",
                        Hora = "08h00",
                        Icone = "bi-box-arrow-in-right",
                        ConteudoHtml = "Abertura oficial: Seções eleitorais abertas em todo o território nacional para o 2º Turno das Eleições 2026.",
                        Tipo = "aviso"
                    });
                }
            }
            else
            {
                // 3. Apuração EM ANDAMENTO ou CONCLUÍDA (> 0% apurado)
                string horaAtual = !string.IsNullOrWhiteSpace(vm.DataHoraTse) && vm.DataHoraTse.Length >= 5
                    ? vm.DataHoraTse.Split(' ').LastOrDefault()?.Substring(0, 5).Replace(":", "h") ?? DateTime.Now.ToString("HH\\hmm")
                    : DateTime.Now.ToString("HH\\hmm");

                // Se a eleição foi concluída / matematicamente definida
                if (vm.Finalizado && cand1 != null)
                {
                    noticias.Insert(0, new NoticiaItem
                    {
                        Id = "eleicao_definida",
                        Hora = horaAtual,
                        AvatarUrl = ObterAvatarCandidato(cand1.Numero),
                        ConteudoHtml = $"TSE homologa resultado: <strong>{cand1.NomeUrna}</strong> ({cand1.Partido}) é eleito <strong>Presidente da República</strong> com <strong>{cand1.Percentual:F2}%</strong> dos votos válidos.",
                        Tipo = "eleito",
                        Destaque = true
                    });
                }

                // Marco da última atualização de seções transmitidas pelo TSE
                if (cand1 != null && cand2 != null)
                {
                    string cor1 = cand1.Numero == "22" ? "#3b82f6" : "#ef4444";
                    string cor2 = cand2.Numero == "22" ? "#3b82f6" : "#ef4444";

                    noticias.Insert(0, new NoticiaItem
                    {
                        Id = $"parcial_{vm.SecoesTotalizadas}",
                        Hora = horaAtual,
                        Icone = "bi-circle",
                        ConteudoHtml = $"<div class=\"fw-bold mb-1\" style=\"color: var(--text-title);\">+{vm.SecoesTotalizadas:N0} seções ({vm.PercentualTotalizado:F2}%)</div>" +
                                       $"<div><strong>{cand1.NomeUrna}</strong> <span class=\"font-mono fw-bold\" style=\"color: {cor1};\">{cand1.Percentual:F2}%</span> ({cand1.Votos:N0} votos)</div>" +
                                       $"<div><strong>{cand2.NomeUrna}</strong> <span class=\"font-mono fw-bold\" style=\"color: {cor2};\">{cand2.Percentual:F2}%</span> ({cand2.Votos:N0} votos)</div>",
                        Tipo = "apuracao",
                        Destaque = true
                    });
                }

                // Marcos de avanço percentual
                if (vm.PercentualTotalizado >= 90 && cand1 != null)
                {
                    noticias.Add(new NoticiaItem
                    {
                        Hora = "Reta Final",
                        AvatarUrl = ObterAvatarCandidato(cand1.Numero),
                        ConteudoHtml = $"Apuração atinge a reta final no país com <strong>{vm.PercentualTotalizado:F1}%</strong> das urnas totalizadas. <strong>{cand1.NomeUrna}</strong> mantém a dianteira.",
                        Tipo = "lideranca"
                    });
                }
                else if (vm.PercentualTotalizado >= 50 && cand1 != null)
                {
                    long diffVotos = (cand1 != null && cand2 != null) ? Math.Abs(cand1.Votos - cand2.Votos) : 0;
                    noticias.Add(new NoticiaItem
                    {
                        Hora = "50%+",
                        Icone = "bi-bar-chart-steps",
                        ConteudoHtml = $"Mais da metade das urnas apuradas no Brasil (<strong>{vm.PercentualTotalizado:F1}%</strong>). Diferença atual de <strong>{diffVotos:N0} votos</strong> entre os candidatos.",
                        Tipo = "apuracao"
                    });
                }
                else if (vm.PercentualTotalizado >= 15 && cand1 != null)
                {
                    noticias.Add(new NoticiaItem
                    {
                        Hora = "Parciais",
                        AvatarUrl = ObterAvatarCandidato(cand1.Numero),
                        ConteudoHtml = $"Grandes colégios eleitorais aceleram a apuração. <strong>{cand1.NomeUrna}</strong> lidera com <strong>{cand1.Percentual:F2}%</strong>.",
                        Tipo = "lideranca"
                    });
                }

                // Exterior consolidado
                noticias.Add(new NoticiaItem
                {
                    Hora = "Exterior",
                    Icone = "bi-globe2",
                    ConteudoHtml = "Exterior (ZZ): Urnas totalizadas nos consulados do mundo (Lisboa, Londres, Paris, Tóquio).",
                    Tipo = "apuracao"
                });

                // Abertura das urnas
                noticias.Add(new NoticiaItem
                {
                    Hora = "17h00",
                    Icone = "bi-shield-check",
                    ConteudoHtml = "TSE: Urnas encerradas em todo o país. Transmissão e totalização de votos iniciadas.",
                    Tipo = "tse_urna"
                });
            }

            return noticias.Take(15).ToList();
        }

        private static string ObterAvatarCandidato(string? numero)
        {
            if (numero == "22") return "/img/candidatos/bolsonaro.webp";
            if (numero == "13") return "/img/candidatos/lula.webp";
            return "/img/candidatos/22.jpg";
        }

        private List<NoticiaItem> CarregarNoticiasCustomizadas()
        {
            try
            {
                var paths = new[]
                {
                    Path.Combine(_env.ContentRootPath, "wwwroot", "data", "noticias.json"),
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "noticias.json"),
                    Path.Combine(AppContext.BaseDirectory, "wwwroot", "data", "noticias.json")
                };

                string? jsonFile = paths.FirstOrDefault(File.Exists);
                if (jsonFile == null) return new List<NoticiaItem>();

                var json = File.ReadAllText(jsonFile).Trim();
                if (string.IsNullOrWhiteSpace(json)) return new List<NoticiaItem>();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var itens = JsonSerializer.Deserialize<List<NoticiaItem>>(json, options);
                return itens ?? new List<NoticiaItem>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Falha ao carregar noticias customizadas do JSON: {Msg}", ex.Message);
                return new List<NoticiaItem>();
            }
        }
    }
}
