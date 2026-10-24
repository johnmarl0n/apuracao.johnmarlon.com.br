using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apuracao.johnmarlon.com.br.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace apuracao.johnmarlon.com.br.Data
{
    public class ApuracaoRepository
    {
        private readonly string? _connStr;
        private readonly ILogger<ApuracaoRepository> _logger;

        public ApuracaoRepository(IConfiguration config, ILogger<ApuracaoRepository> logger)
        {
            _connStr = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<long> SalvarSnapshotAsync(
            string codigoEleicao,
            string uf,
            string? codigoMunicipio,
            string cargo,
            string? nomeCargo,
            int totalSecoes,
            int secoesTotalizadas,
            decimal percentualTotalizado,
            int eleitoradoTotal,
            int comparecimento,
            int abstencao,
            decimal percentualComparecimento,
            decimal percentualAbstencao,
            int votosValidos,
            int votosBrancos,
            int votosNulos,
            string? idGeracaoTse,
            string? dataHoraTse,
            string? payloadJson,
            IEnumerable<ApuracaoCandidatoRecord> candidatos)
        {
            if (string.IsNullOrWhiteSpace(_connStr)) return 0;

            try
            {
                using var conn = new SqlConnection(_connStr);
                await conn.OpenAsync();

                // Verificar se o último snapshot deste mesmo cargo/uf/município já tem o mesmo idGeracaoTse
                var checkSql = @"
                    SELECT TOP 1 Id FROM ApuracaoSnapshots 
                    WHERE CodigoEleicao = @CodigoEleicao 
                      AND Uf = @Uf 
                      AND ISNULL(CodigoMunicipio, '') = ISNULL(@CodigoMunicipio, '') 
                      AND Cargo = @Cargo 
                      AND IdGeracaoTse = @IdGeracaoTse
                    ORDER BY Id DESC;";

                var existingId = await conn.QueryFirstOrDefaultAsync<long?>(checkSql, new {
                    CodigoEleicao = codigoEleicao,
                    Uf = uf,
                    CodigoMunicipio = codigoMunicipio,
                    Cargo = cargo,
                    IdGeracaoTse = idGeracaoTse
                });

                if (existingId.HasValue && existingId.Value > 0)
                {
                    return existingId.Value; // Já salvo previamente, evita duplicidade idêntica
                }

                using var trans = conn.BeginTransaction();

                var insertSnapshotSql = @"
                INSERT INTO ApuracaoSnapshots (
                    CodigoEleicao, Uf, CodigoMunicipio, Cargo, NomeCargo,
                    TotalSecoes, SecoesTotalizadas, PercentualTotalizado,
                    EleitoradoTotal, Comparecimento, Abstencao,
                    PercentualComparecimento, PercentualAbstencao,
                    VotosValidos, VotosBrancos, VotosNulos,
                    IdGeracaoTse, DataHoraTse, DataHoraColeta, PayloadJson
                ) VALUES (
                    @CodigoEleicao, @Uf, @CodigoMunicipio, @Cargo, @NomeCargo,
                    @TotalSecoes, @SecoesTotalizadas, @PercentualTotalizado,
                    @EleitoradoTotal, @Comparecimento, @Abstencao,
                    @PercentualComparecimento, @PercentualAbstencao,
                    @VotosValidos, @VotosBrancos, @VotosNulos,
                    @IdGeracaoTse, @DataHoraTse, GETDATE(), @PayloadJson
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                var snapshotId = await conn.ExecuteScalarAsync<long>(insertSnapshotSql, new
                {
                    CodigoEleicao = codigoEleicao,
                    Uf = uf,
                    CodigoMunicipio = codigoMunicipio,
                    Cargo = cargo,
                    NomeCargo = nomeCargo,
                    TotalSecoes = totalSecoes,
                    SecoesTotalizadas = secoesTotalizadas,
                    PercentualTotalizado = percentualTotalizado,
                    EleitoradoTotal = eleitoradoTotal,
                    Comparecimento = comparecimento,
                    Abstencao = abstencao,
                    PercentualComparecimento = percentualComparecimento,
                    PercentualAbstencao = percentualAbstencao,
                    VotosValidos = votosValidos,
                    VotosBrancos = votosBrancos,
                    VotosNulos = votosNulos,
                    IdGeracaoTse = idGeracaoTse,
                    DataHoraTse = dataHoraTse != null && dataHoraTse.Length > 30 ? dataHoraTse.Substring(0, 30) : dataHoraTse,
                    PayloadJson = payloadJson
                }, trans);

                var insertCandSql = @"
                INSERT INTO ApuracaoCandidatos (
                    SnapshotId, NumeroCandidato, SequencialCandidato, NomeUrna,
                    NomeCompleto, Partido, Coligacao, TotalVotos,
                    PercentualVotos, Situacao, Eleito, FotoUrl, DataHoraColeta
                ) VALUES (
                    @SnapshotId, @NumeroCandidato, @SequencialCandidato, @NomeUrna,
                    @NomeCompleto, @Partido, @Coligacao, @TotalVotos,
                    @PercentualVotos, @Situacao, @Eleito, @FotoUrl, GETDATE()
                );";

                foreach (var c in candidatos)
                {
                    await conn.ExecuteAsync(insertCandSql, new
                    {
                        SnapshotId = snapshotId,
                        c.NumeroCandidato,
                        c.SequencialCandidato,
                        c.NomeUrna,
                        c.NomeCompleto,
                        c.Partido,
                        c.Coligacao,
                        c.TotalVotos,
                        c.PercentualVotos,
                        c.Situacao,
                        c.Eleito,
                        c.FotoUrl
                    }, trans);
                }

                trans.Commit();
                _logger.LogInformation("Snapshot {SnapshotId} salvo no MSSQL para {Cargo}/{Uf}/{Municipio}.", snapshotId, cargo, uf, codigoMunicipio);
                return snapshotId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar snapshot de apuração no MSSQL.");
                return 0;
            }
        }

        public async Task<List<TimelinePontoDto>> ObterTimelineAsync(string codigoEleicao, string cargo, string uf)
        {
            if (string.IsNullOrWhiteSpace(_connStr)) return new List<TimelinePontoDto>();

            try
            {
                using var conn = new SqlConnection(_connStr);
                await conn.OpenAsync();

                var sqlSnapshots = @"
                    SELECT TOP 100
                        s.Id,
                        s.PercentualTotalizado,
                        s.DataHoraTse,
                        s.DataHoraColeta,
                        s.SecoesTotalizadas,
                        s.TotalSecoes
                    FROM ApuracaoSnapshots s
                    WHERE s.CodigoEleicao = @CodigoEleicao
                      AND s.Cargo = @Cargo
                      AND s.Uf = @Uf
                      AND ISNULL(s.CodigoMunicipio, '') = ''
                      AND s.PercentualTotalizado > 0
                    ORDER BY s.PercentualTotalizado ASC, s.Id ASC;";

                var snapshots = (await conn.QueryAsync<dynamic>(sqlSnapshots, new { CodigoEleicao = codigoEleicao, Cargo = cargo, Uf = uf })).ToList();
                if (!snapshots.Any()) return new List<TimelinePontoDto>();

                var snapshotIds = snapshots.Select(s => (long)s.Id).ToList();

                var sqlCands = @"
                    SELECT 
                        c.SnapshotId,
                        c.NumeroCandidato,
                        c.NomeUrna,
                        c.TotalVotos,
                        c.PercentualVotos
                    FROM ApuracaoCandidatos c
                    WHERE c.SnapshotId IN @Ids
                    ORDER BY c.SnapshotId, c.PercentualVotos DESC;";

                var cands = (await conn.QueryAsync<dynamic>(sqlCands, new { Ids = snapshotIds })).ToList();
                var candsBySnap = cands.GroupBy(c => (long)c.SnapshotId).ToDictionary(g => g.Key, g => g.ToList());

                var result = new List<TimelinePontoDto>();
                decimal ultimoPct = -1;

                foreach (var s in snapshots)
                {
                    decimal pct = (decimal)s.PercentualTotalizado;
                    if (Math.Abs(pct - ultimoPct) < 0.5m) continue;
                    ultimoPct = pct;

                    long sId = (long)s.Id;
                    candsBySnap.TryGetValue(sId, out var snapCands);

                    var c1 = snapCands != null && snapCands.Count > 0 ? snapCands[0] : null;
                    var c2 = snapCands != null && snapCands.Count > 1 ? snapCands[1] : null;

                    decimal p1 = c1 != null ? (decimal)c1.PercentualVotos : 0m;
                    decimal p2 = c2 != null ? (decimal)c2.PercentualVotos : 0m;
                    int v1 = c1 != null ? (int)c1.TotalVotos : 0;
                    int v2 = c2 != null ? (int)c2.TotalVotos : 0;

                    string horaFormatada = "";
                    if (s.DataHoraTse != null)
                    {
                        string dht = (string)s.DataHoraTse;
                        if (dht.Length >= 5 && dht.Contains(":"))
                        {
                            var parts = dht.Split(' ');
                            horaFormatada = parts.Length > 1 ? parts[1].Substring(0, Math.Min(5, parts[1].Length)) : dht.Substring(0, 5);
                        }
                    }
                    if (string.IsNullOrEmpty(horaFormatada))
                    {
                        horaFormatada = ((DateTime)s.DataHoraColeta).ToString("HH:mm");
                    }

                    result.Add(new TimelinePontoDto
                    {
                        Pct = (int)Math.Round(pct),
                        Hora = horaFormatada,
                        Secoes = $"{pct:F1}%",
                        Cand1 = p1,
                        Cand2 = p2,
                        Cand1Votos = v1,
                        Cand2Votos = v2,
                        Diff = Math.Abs(p1 - p2),
                        Status = pct >= 100m ? "Totalização 100% Concluída" : $"Totalização em {pct:F1}%"
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar timeline no MSSQL.");
                return new List<TimelinePontoDto>();
            }
        }
    }

    public class ApuracaoCandidatoRecord
    {
        public string NumeroCandidato { get; set; } = "";
        public string? SequencialCandidato { get; set; }
        public string NomeUrna { get; set; } = "";
        public string? NomeCompleto { get; set; }
        public string? Partido { get; set; }
        public string? Coligacao { get; set; }
        public int TotalVotos { get; set; }
        public decimal PercentualVotos { get; set; }
        public string? Situacao { get; set; }
        public bool Eleito { get; set; }
        public string? FotoUrl { get; set; }
    }
}
