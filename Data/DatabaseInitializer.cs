using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace apuracao.johnmarlon.com.br.Data
{
    public class DatabaseInitializer
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(IConfiguration config, ILogger<DatabaseInitializer> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            var connStr = _config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connStr))
            {
                _logger.LogWarning("Connection string 'DefaultConnection' não configurada. Histórico em MSSQL desabilitado.");
                return;
            }

            try
            {
                using var conn = new SqlConnection(connStr);
                await conn.OpenAsync();

                var sql = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ApuracaoSnapshots' AND xtype='U')
                BEGIN
                    CREATE TABLE ApuracaoSnapshots (
                        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                        CodigoEleicao VARCHAR(10) NOT NULL,
                        Uf VARCHAR(5) NOT NULL,
                        CodigoMunicipio VARCHAR(10) NULL,
                        Cargo VARCHAR(10) NOT NULL,
                        NomeCargo NVARCHAR(100) NULL,
                        TotalSecoes INT NOT NULL DEFAULT 0,
                        SecoesTotalizadas INT NOT NULL DEFAULT 0,
                        PercentualTotalizado DECIMAL(5,2) NOT NULL DEFAULT 0,
                        EleitoradoTotal INT NOT NULL DEFAULT 0,
                        Comparecimento INT NOT NULL DEFAULT 0,
                        Abstencao INT NOT NULL DEFAULT 0,
                        PercentualComparecimento DECIMAL(5,2) NOT NULL DEFAULT 0,
                        PercentualAbstencao DECIMAL(5,2) NOT NULL DEFAULT 0,
                        VotosValidos INT NOT NULL DEFAULT 0,
                        VotosBrancos INT NOT NULL DEFAULT 0,
                        VotosNulos INT NOT NULL DEFAULT 0,
                        IdGeracaoTse VARCHAR(50) NULL,
                        DataHoraTse VARCHAR(30) NULL,
                        DataHoraColeta DATETIME NOT NULL DEFAULT GETDATE(),
                        PayloadJson NVARCHAR(MAX) NULL
                    );

                    CREATE INDEX IX_ApuracaoSnapshots_Eleicao_Cargo_Uf ON ApuracaoSnapshots (CodigoEleicao, Cargo, Uf, CodigoMunicipio);
                    CREATE INDEX IX_ApuracaoSnapshots_Coleta ON ApuracaoSnapshots (DataHoraColeta DESC);
                END;

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ApuracaoCandidatos' AND xtype='U')
                BEGIN
                    CREATE TABLE ApuracaoCandidatos (
                        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                        SnapshotId BIGINT NOT NULL,
                        NumeroCandidato VARCHAR(10) NOT NULL,
                        SequencialCandidato VARCHAR(30) NULL,
                        NomeUrna NVARCHAR(150) NOT NULL,
                        NomeCompleto NVARCHAR(250) NULL,
                        Partido NVARCHAR(50) NULL,
                        Coligacao NVARCHAR(250) NULL,
                        TotalVotos INT NOT NULL DEFAULT 0,
                        PercentualVotos DECIMAL(5,2) NOT NULL DEFAULT 0,
                        Situacao NVARCHAR(50) NULL,
                        Eleito BIT NOT NULL DEFAULT 0,
                        FotoUrl NVARCHAR(500) NULL,
                        DataHoraColeta DATETIME NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT FK_ApuracaoCandidatos_Snapshot FOREIGN KEY (SnapshotId) REFERENCES ApuracaoSnapshots(Id) ON DELETE CASCADE
                    );

                    CREATE INDEX IX_ApuracaoCandidatos_Snapshot ON ApuracaoCandidatos (SnapshotId);
                    CREATE INDEX IX_ApuracaoCandidatos_Numero ON ApuracaoCandidatos (NumeroCandidato);
                END;

                BEGIN TRY
                    ALTER TABLE ApuracaoSnapshots ALTER COLUMN DataHoraTse NVARCHAR(100) NULL;
                END TRY
                BEGIN CATCH
                END CATCH;
                ";

                await conn.ExecuteAsync(sql);
                _logger.LogInformation("Tabelas do MSSQL (ApuracaoSnapshots e ApuracaoCandidatos) verificadas/criadas com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Não foi possível conectar ao MSSQL agora ({Message}). O sistema continuará com cache em memória.", ex.Message);
            }
        }
    }
}
