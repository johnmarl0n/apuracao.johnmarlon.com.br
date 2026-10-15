using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
