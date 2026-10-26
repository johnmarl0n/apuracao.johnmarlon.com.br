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

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CandidatosFotos' AND xtype='U')
                BEGIN
                    CREATE TABLE CandidatosFotos (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        NumeroCandidato VARCHAR(10) NOT NULL,
                        Ano VARCHAR(10) NOT NULL DEFAULT '2026',
                        NomeUrna NVARCHAR(150) NOT NULL,
                        Partido VARCHAR(20) NOT NULL,
                        FotoUrl NVARCHAR(500) NULL,
                        FotoBlob VARBINARY(MAX) NULL,
                        ContentType VARCHAR(50) DEFAULT 'image/webp',
                        DataCriacao DATETIME NOT NULL DEFAULT GETDATE()
                    );
                    CREATE UNIQUE INDEX UQ_CandidatosFotos ON CandidatosFotos (NumeroCandidato, Ano);
                END;

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ApuracaoTimeline' AND xtype='U')
                BEGIN
                    CREATE TABLE ApuracaoTimeline (
                        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                        MinutoIndice INT NOT NULL,
                        HorarioLabel VARCHAR(10) NOT NULL,
                        PercentualApurado DECIMAL(5,2) NOT NULL,
                        LulaPerc DECIMAL(5,2) NOT NULL,
                        BolsonaroPerc DECIMAL(5,2) NOT NULL,
                        LulaVotos BIGINT NOT NULL DEFAULT 0,
                        BolsonaroVotos BIGINT NOT NULL DEFAULT 0,
                        NoticiaTitulo NVARCHAR(300) NULL,
                        NoticiaAvatar NVARCHAR(200) NULL,
                        PayloadJson NVARCHAR(MAX) NULL,
                        DataHora DATETIME NOT NULL DEFAULT GETDATE()
                    );
                    CREATE INDEX IX_ApuracaoTimeline_Minuto ON ApuracaoTimeline (MinutoIndice);
                END;

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UrnasVotacaoEstendida' AND xtype='U')
                BEGIN
                    CREATE TABLE UrnasVotacaoEstendida (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Ano VARCHAR(10) NOT NULL DEFAULT '2026-2T',
                        Uf VARCHAR(5) NOT NULL,
                        Municipio NVARCHAR(150) NOT NULL,
                        CodigoMunicipio VARCHAR(10) NULL,
                        Zona INT NOT NULL,
                        Secao INT NOT NULL,
                        LocalVotacao NVARCHAR(250) NULL,
                        HorarioFechamento VARCHAR(10) NOT NULL,
                        MotivoExtensao NVARCHAR(250) NULL,
                        EleitoresAptos INT NOT NULL DEFAULT 0,
                        Comparecimento INT NOT NULL DEFAULT 0,
                        VotosCand1 INT NOT NULL DEFAULT 0,
                        VotosCand2 INT NOT NULL DEFAULT 0,
                        VotosBrancos INT NOT NULL DEFAULT 0,
                        VotosNulos INT NOT NULL DEFAULT 0,
                        TotalVotos INT NOT NULL DEFAULT 0,
                        DataCriacao DATETIME NOT NULL DEFAULT GETDATE()
                    );
                    CREATE INDEX IX_UrnasVotacaoEstendida_Uf_Mun ON UrnasVotacaoEstendida (Uf, CodigoMunicipio);
                END;

                BEGIN TRY
                    ALTER TABLE ApuracaoSnapshots ALTER COLUMN DataHoraTse NVARCHAR(100) NULL;
                END TRY
                BEGIN CATCH
                END CATCH;
                ";

                await conn.ExecuteAsync(sql);
                _logger.LogInformation("Tabelas do MSSQL (ApuracaoSnapshots, ApuracaoCandidatos, CandidatosFotos, ApuracaoTimeline e UrnasVotacaoEstendida) verificadas/criadas com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Não foi possível conectar ao MSSQL agora ({Message}). O sistema continuará com cache em memória.", ex.Message);
            }
        }
    }
}
