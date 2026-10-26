-- ============================================================
-- SCRIPT DE CRIAÇÃO DO BANCO DE DADOS DA APURAÇÃO TSE (MSSQL)
-- Compatível com SQL Server (SmartASP / LocalDB / Azure SQL)
-- ============================================================

-- 1. Tabela de Snapshots de Totalização
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
GO

-- 2. Tabela de Votação Nominal dos Candidatos
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
    CREATE INDEX IX_ApuracaoCandidatos_DataHora ON ApuracaoCandidatos (DataHoraColeta DESC);
END;
GO

-- 3. Tabela de Urnas com Votação Estendida (Pós-17h autorizadas por fila/senhas STF/TSE)
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
GO

-- ============================================================
-- EXEMPLOS DE CONSULTAS ANALÍTICAS POSTERIORES
-- ============================================================

-- A. Último resultado apurado para Presidente no Brasil:
-- SELECT TOP 1 s.PercentualTotalizado, s.SecoesTotalizadas, s.TotalSecoes, s.DataHoraColeta
-- FROM ApuracaoSnapshots s
-- WHERE s.Cargo = '1' AND s.Uf = 'br'
-- ORDER BY s.Id DESC;

-- B. Ranking dos candidatos no último snapshot de Campinas (62910):
-- SELECT c.NumeroCandidato, c.NomeUrna, c.Partido, c.TotalVotos, c.PercentualVotos, c.Situacao
-- FROM ApuracaoCandidatos c
-- WHERE c.SnapshotId = (
--     SELECT TOP 1 Id FROM ApuracaoSnapshots 
--     WHERE CodigoMunicipio = '62910' AND Cargo = '1'
--     ORDER BY Id DESC
-- )
-- ORDER BY c.TotalVotos DESC;

-- C. Linha do tempo de votos de um candidato (evolução minuto a minuto):
-- SELECT s.DataHoraColeta, s.PercentualTotalizado, c.NomeUrna, c.TotalVotos, c.PercentualVotos
-- FROM ApuracaoCandidatos c
-- JOIN ApuracaoSnapshots s ON s.Id = c.SnapshotId
-- WHERE c.NumeroCandidato = '22' AND s.Uf = 'br'
-- ORDER BY s.Id ASC;
