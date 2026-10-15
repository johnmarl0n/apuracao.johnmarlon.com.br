# 🗳️ Monitor de Apuração TSE & Base Histórica Eleitoral

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C# 12](https://img.shields.io/badge/C%23-12-239120?logo=csharp)](https://docs.microsoft.com/dotnet/csharp/)
[![SmartASP MSSQL](https://img.shields.io/badge/Database-SQL%20Server-CC292B?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![License: No License](https://img.shields.io/badge/License-No%20License%20(Proprietary)-red.svg)](#licença)
[![Author](https://img.shields.io/badge/Author-John%20Dias-blue.svg)](#autor)

Plataforma corporativa de alto desempenho para acompanhamento, auditoria e estudo de apurações eleitorais oficiais do **Tribunal Superior Eleitoral (TSE)**. Desenvolvida em **ASP.NET Core 8.0 MVC**, com arquitetura resiliente de cache em memória, persistência em SQL Server (SmartASP), interface moderna responsiva com suporte a temas Claro/Escuro (*Dark/Light Mode*), **mapa cartográfico vetorial do Brasil com coloração por candidato líder em cada UF**, módulo de urnas no exterior (**Exterior - ZZ**), mapa do Estado de São Paulo com dados demográficos do IBGE, acervo histórico consolidado (2002 a 2022) e total extensibilidade para eleições futuras sem necessidade de recompilação.

---

## 📋 Sumário

- [Visão Geral](#-visão-geral)
- [Funcionalidades Principais](#-funcionalidades-principais)
- [Mapa da Apuração do Brasil & Urnas no Mundo](#-mapa-da-apuração-do-brasil--urnas-no-mundo)
- [Base Histórica de Eleições Gerais (2002 a 2022)](#-base-histórica-de-eleições-gerais-2002-a-2022)
- [Arquitetura & Tecnologias](#-arquitetura--tecnologias)
- [Configuração de Eleições (Atuais & Históricas)](#-configuração-de-eleições-atuais--históricas)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Instalação & Execução Local](#-instalação--execução-local)
- [Deploy e Hospedagem (SmartASP / IIS)](#-deploy-e-hospedagem-smartasp--iis)
- [API RESTful](#-api-restful)
- [Gestão de Fotos de Candidatos](#-gestão-de-fotos-de-candidatos)
- [Autor](#-autor)
- [Licença](#-licença)

---

## 🎯 Visão Geral

O sistema conecta-se diretamente à rede de distribuição de dados abertos do TSE (`resultados.tse.jus.br`) para processar e exibir a apuração de votos em tempo real no dia da votação, além de integrar uma base histórica abrangente dos pleitos gerais homologados de 2002 a 2022.

O layout foi concebido sob rigorosos padrões de UI/UX, apresentando:
- **Painel de Eleição Principal:** Focado nas eleições vigentes ao vivo (Eleições Gerais 2026 - 1º Turno) com atualização instantânea de votos e seções sem recarregamento de página.
- **Mapa da Apuração em Tempo Real:** Cartografia vetorial de alta fidelidade de todos os 27 estados e DF, coloridos dinamicamente de acordo com o candidato líder em cada unidade da federação.
- **Card Consular (Exterior - ZZ):** Totalização dedicada das urnas de brasileiros no exterior, com líder, votos e percentual em destaque.
- **Placar Geral de Estados:** Sumário instantâneo com a contagem de estados liderados por cada candidato.
- **Base Histórica Integrada:** Módulo isolado acessível via menu dropdown na barra de filtros e modal rápido na barra de navegação.
- **Indicadores Dinâmicos:** Seções totalizadas, votos válidos, brancos, nulos, comparecimento e abstenção.
- **Cartões de Candidatos:** Destaque para situação eleitoral (Eleito, 2º Turno, Não Eleito), percentual, número e cores oficiais das legendas partidárias.
- **Gerenciamento Inteligente do Fim da Votação:** Remoção automática do banner de *"Votação em Andamento"* a partir das 17:00 ou assim que as primeiras urnas começam a ser apuradas.
- **Ficha Demográfica Regional:** Dados do Censo IBGE e TRE-SP integrados com mapa vetorial interativo do Estado de São Paulo.

---

## 🚀 Funcionalidades Principais

- **Monitoramento em Tempo Real:** Leitura dos arquivos oficiais de totalização do TSE com auto-refresh a cada 30 segundos (atualiza votos, percentuais, posições dos candidatos e mapa sem refresh de tela).
- **Mapa Cartográfico Oficial do Brasil:** Vetores contíguos de alta fidelidade (viewBox `0 0 1000 912`) com todos os 27 estados desenhados com precisão geográfica e fronteiras contíguas.
- **Pintura Dinâmica por Liderança Eleitoral:** Estados coloridos automaticamente com as cores oficiais do partido do candidato em 1º lugar na respectiva UF.
- **Módulo de Votos no Exterior (ZZ):** Acompanhamento exclusivo das seções de consulados e embaixadas espalhadas pelo mundo.
- **Alternância Claro / Escuro (Dark/Light Theme):** Design System semântico adaptativo, com contraste otimizado (WCAG AA), cards claros com alto contraste e `MutationObserver` para re-estilização instantânea do mapa ao alternar o tema.
- **Padronização de Encoding & Ícones Vetoriais:** Arquivos de views Razor gravados com **UTF-8 com BOM** para blindagem total contra *mojibake* em servidores Windows/IIS, além da adoção de **Bootstrap Icons** nos seletores de mapa (`Brasil` e `SP`) no lugar de emojis multibyte, assegurando fidelidade visual em qualquer plataforma.
- **Ciclo Inteligente das 17:00:** O aviso de *"Votação em Andamento (encerramento às 17:00)"* desaparece automaticamente assim que a votação encerra às 17:00 ou no início da totalização de seções.
- **Base Histórica em MSSQL (SmartASP):** Módulo reservado com interface modal para sincronização, auditoria e consulta de snapshots consolidados diretamente no banco de dados.
- **Autenticação Segura:** Proteção por cookies de sessão criptografados (`CookieAuthentication`), tela de login exclusiva e rota protegida para gerenciamento de dados.
- **API REST Documentada:** Endpoints protegidos por token de segurança (`X-Api-Token`) para integração com painéis externos e dashboards.

---

## 🗺️ Mapa da Apuração do Brasil & Urnas no Mundo

Para eleições presidenciais, a plataforma disponibiliza um módulo geográfico completo:

1. **Visão Cartográfica Contígua (27 UFs):**
   - Vetores SVG oficiais derivados de malhas geográficas nacionais, garantindo que os estados fiquem perfeitamente encaixados, sem distorções ou efeito de ilhas desconectadas.
   - Estados ainda não totalizados recebem preenchimento neutro elegante (`#1e293b` no tema escuro e `#e2e8f0` no tema claro).
   - Conforme as seções são apuradas, os estados são pintados dinamicamente com a cor do líder (ex: vermelho para PT, azul para PL, etc.).
2. **Urnas no Mundo (Exterior - ZZ):**
   - Card interativo dedicado no topo do mapa com badge pulsar de globo, percentual de apuração consular, líder e número de votos.
   - Clique direto para navegar e abrir a apuração detalhada de todas as seções do exterior (`uf=zz`).
3. **Placar de Estados Liderados:**
   - Barra analítica com contadores de estados conquistados por cada candidato (ex: `16 estados`, `11 estados + Exterior`), com tags de alto contraste em ambos os temas.
4. **Tooltip Interativo:**
   - Ao passar o mouse ou tocar em qualquer estado, um popover suspenso escuro de alto contraste exibe instantaneamente:
     - Nome completo do estado e percentual apurado.
     - Candidato líder (nome, partido, percentual e cor).
     - Vice-líder da UF (nome, partido e percentual).
     - Clique no estado para filtrar diretamente os resultados daquela UF.
5. **Alternador de Visões Vetoriais (Brasil / SP):**
   - Grupo de botões estilizados com ícones vetoriais do Bootstrap Icons (`bi-globe-americas` e `bi-building`) para alternar instantaneamente entre o mapa contíguo de todas as 27 UFs do Brasil e o mapa estadual de São Paulo com coordenadas geográficas do IBGE.

---

## 🏛️ Base Histórica de Eleições Gerais (2002 a 2022)

Para contornar o comportamento do TSE — que remove pleitos anteriores de suas CDNs de tempo real após a homologação oficial gerando erros HTTP 404 —, a plataforma incorpora um repositório nativo de dados históricos oficiais ([DadosHistoricosTse.cs](file:///e:/99.Projetos/CSharp/VS2022/apuracao.johnmarlon.com.br/Services/DadosHistoricosTse.cs)) com persistência automática no SQL Server.

### Pleitos Abrangidos
| Ano | Pleito Principal | Vencedor / Disputa |
|---|---|---|
| **2022** | Eleições Gerais 2022 | Lula (48,43%) vs Jair Bolsonaro (43,20%) - 1º Turno |
| **2018** | Eleições Gerais 2018 | Jair Bolsonaro (46,03%) vs Fernando Haddad (29,28%) |
| **2014** | Eleições Gerais 2014 | Dilma Rousseff (41,59%) vs Aécio Neves (33,55%) |
| **2010** | Eleições Gerais 2010 | Dilma Rousseff (46,91%) vs José Serra (32,61%) |
| **2006** | Eleições Gerais 2006 | Lula (48,61%) vs Geraldo Alckmin (41,64%) |
| **2002** | Eleições Gerais 2002 | Lula (46,44%) vs José Serra (23,20%) |

### Cargos Considerados na Base Histórica
As consultas históricas restringem-se aos **5 cargos mais estratégicos**, omitindo propositalmente dados municipais pontuais:
1. **Presidente da República** (Brasil Nacional e Recorte SP)
2. **Governador de Estado** (São Paulo)
3. **Senador da República** (São Paulo)
4. **Deputado Federal** (São Paulo)
5. **Deputado Estadual** (São Paulo)

---

## 🛠️ Arquitetura & Tecnologias

| Camada | Tecnologia | Descrição |
|---|---|---|
| **Backend** | .NET 8.0 (C# 12) | ASP.NET Core MVC & Web API com controllers tipados |
| **Acesso a Dados** | Dapper 2.1 | Micro-ORM de altíssima performance para queries SQL brutas |
| **Banco de Dados** | Microsoft SQL Server | Hospedado na nuvem (SQL Server / Azure SQL / SmartASP) |
| **Cache de Dados** | `IMemoryCache` | Cache em memória volátil de 20s para prevenir throttling e absorver picos |
| **Agregação Paralela** | `Task.WhenAll` | Consulta simultânea das 27 UFs + ZZ para o mapa nacional |
| **Frontend** | HTML5 / CSS3 / Vanilla JS | Estilização moderna via Glassmorphism, CSS Custom Properties e Bootstrap 5.3 |
| **Ícones & Tipografia** | Bootstrap Icons & Google Fonts | Fontes *Inter* e *JetBrains Mono* para clareza numérica |
| **Vetorização Cartográfica** | SVG Vetorial Oficial (Brasil e SP) | Contornos oficiais de alta precisão com suporte a hover e eventos de toque |
| **Codificação & Encoding** | UTF-8 com BOM | Views Razor (.cshtml) salvas em UTF-8 com BOM para prevenir incompatibilidades ANSI/Windows-1252 no IIS |
| **Servidor Web** | IIS / Kestrel | Configurado em modo `OutOfProcess` no `web.config` |

---

## ⚙️ Configuração de Eleições (Atuais & Históricas)

O arquivo [appsettings.json](file:///e:/99.Projetos/CSharp/VS2022/apuracao.johnmarlon.com.br/appsettings.json) permite gerenciar separadamente os pleitos ao vivo e as bases históricas:

```json
{
  "Tse": {
    "BaseUrl": "https://resultados.tse.jus.br/oficial/",
    "AnoEleicao": "2026",
    "Pleito": "3220",
    "EleicaoFederal": "6257",
    "EleicaoEstadual": "6259",
    "EleicaoDistrital": "6261",
    "UfPadrao": "sp",
    "MunicipioPadrao": "62910",
    "IntervaloCacheSegundos": 20,
    "EleicoesConfiguradas": [
      {
        "Ano": "2026",
        "Pleito": "3220",
        "Descricao": "Eleições Gerais 2026 (1º Turno)",
        "Tipo": "Geral",
        "EleicaoFederal": "6257",
        "EleicaoEstadual": "6259",
        "EleicaoDistrital": "6261",
        "AoVivo": true
      }
    ],
    "EleicoesHistoricas": [
      {
        "Ano": "2022",
        "Pleito": "406",
        "Descricao": "Eleições Gerais 2022",
        "Tipo": "Geral",
        "EleicaoFederal": "544",
        "EleicaoEstadual": "546",
        "AoVivo": false,
        "Historica": true
      },
      {
        "Ano": "2018",
        "Pleito": "295",
        "Descricao": "Eleições Gerais 2018",
        "Tipo": "Geral",
        "EleicaoFederal": "295",
        "EleicaoEstadual": "297",
        "AoVivo": false,
        "Historica": true
      }
    ]
  }
}
```

- **`EleicoesConfiguradas`**: Define quais eleições aparecem no seletor principal de apuração ao vivo.
- **`EleicoesHistoricas`**: Define as opções disponíveis no menu suspenso e modal de arquivo histórico.
- **`AoVivo`**: `true` ativa o cronômetro para o fechamento às 17:00 e o auto-refresh contínuo; `false` define o pleito como consolidado/histórico.

---

## 📂 Estrutura do Projeto

```text
apuracao.johnmarlon.com.br/
├── Controllers/
│   ├── ApiController.cs        # Endpoints JSON públicos/autenticados
│   ├── AuthController.cs       # Login, autenticação e logout de sessão
│   └── HomeController.cs       # View principal, consultas de apuração e dados do mapa
├── Data/
│   ├── ApuracaoRepository.cs   # Operações Dapper no SQL Server (Snapshots)
│   └── DatabaseInitializer.cs  # Criação automática de tabelas e índices
├── Models/
│   ├── ApuracaoViewModel.cs    # ViewModels da tela, candidatos e configurações
│   ├── MapaBrasilDto.cs        # Modelos de dados para UFs, exterior e placar de estados
│   └── Tse/
│       └── TseDto.cs           # Mapeamento do JSON oficial do TSE
├── Services/
│   ├── DadosHistoricosTse.cs   # Base consolidada homologada oficial (2002 a 2022)
│   └── TseService.cs           # Engine de consumo HTTP, cache, fallback histórico e mapa
├── Views/
│   ├── Auth/
│   │   └── Login.cshtml        # Tela de login estilizada com dark mode
│   ├── Home/
│   │   ├── Index.cshtml        # Dashboard principal de apuração, candidatos e histórico
│   │   └── _MapaBrasil.cshtml  # Partial view vetorial do Mapa do Brasil e SP
│   └── Shared/
│       └── _Layout.cshtml      # Master template com theme switcher e modais
├── wwwroot/
│   ├── favicon.ico             # Favicon multi-resolução
│   ├── favicon.png             # Ícone PNG 32x32
│   ├── favicon-192.png         # Ícone PNG 192x192
│   ├── favicon.svg             # Favicon vetorial oficial
│   └── img/
│       └── candidatos/         # Diretório para fotos locais de candidatos
├── appsettings.json            # Configurações de conexões, chaves e pleitos
├── database_setup.sql          # Script DDL do banco de dados MSSQL
└── web.config                  # Configuração IIS com OutOfProcess para SmartASP
```

---

## 💻 Instalação & Execução Local

### Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.
- Visual Studio 2022 ou VS Code.

### Passos
1. Abra a pasta do projeto no terminal:
   ```bash
   cd e:\99.Projetos\CSharp\VS2022\apuracao.johnmarlon.com.br
   ```

2. Restaure os pacotes NuGet:
   ```bash
   dotnet restore
   ```

3. Execute a aplicação em modo de desenvolvimento:
   ```bash
   dotnet run --launch-profile http
   ```

4. Acesse pelo navegador:
   ```text
   http://localhost:5161
   ```

---

## 🚀 Deploy e Hospedagem (SmartASP / IIS)

O projeto foi especificamente ajustado para garantir estabilidade e compatibilidade em ambientes de hospedagem compartilhada Windows IIS (como o **SmartASP**):

1. **Geração do Pacote:**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Hosting Model:**
   O arquivo `web.config` utiliza obrigatoriamente:
   ```xml
   <aspNetCore processPath="dotnet" 
               arguments=".\apuracao.johnmarlon.com.br.dll" 
               stdoutLogEnabled="true" 
               stdoutLogFile=".\logs\stdout" 
               hostingModel="OutOfProcess" />
   ```
   *Evita o erro HTTP 503 comum em Application Pools compartilhados no modo InProcess.*

3. **Satélites de Linguagem:**
   O arquivo `.csproj` restringe idiomas extras para manter o diretório de publicação limpo e rápido de subir via FTP:
   ```xml
   <SatelliteResourceLanguages>pt-BR</SatelliteResourceLanguages>
   ```

4. **Upload:**
   Envie todo o conteúdo gerado dentro da pasta `publish/` para o diretório raiz do seu site no FTP da SmartASP.

5. **Codificação dos Arquivos Razor & Estáticos:**
   As views Razor (.cshtml) contendo caracteres acentuados da língua portuguesa são mantidas com assinatura **UTF-8 com BOM**, prevenindo que o interpretador do IIS em servidores Windows interprete o arquivo como ANSI/Windows-1252.

---

## 📡 API RESTful

A aplicação expõe endpoints para consulta programática de dados de apuração:

### 1. Resumo da Apuração
- **Rota:** `GET /api/apuracao/resumo`
- **Autenticação:** Header `X-Api-Token` ou query param `token`
- **Parâmetros:**
  - `cargo`: `1` (Presidente), `3` (Governador), `5` (Senador), `6` (Dep. Federal), `7` (Dep. Estadual)
  - `uf`: `br` ou sigla do estado (`sp`)
  - `municipio`: código TSE do município (ex: `62910` para Campinas, apenas para eleições municipais/atuais)
  - `ano`: ano da eleição (`2026`, `2022`, `2018`, `2014`, `2010`, `2006`, `2002`)

### 2. Snapshots do Banco
- **Rota:** `GET /api/apuracao/snapshots`
- **Autenticação:** Header `X-Api-Token` ou query param `token`
- **Retorno:** Histórico dos últimos snapshots totalizados gravados no SQL Server.

### 3. Dados Agregados do Mapa do Brasil
- **Rota:** `GET /Home/DadosMapaBrasil?ano=2026`
- **Retorno:** JSON contendo o status de apuração de todas as 27 UFs + ZZ, líder e vice-líder por estado, total de estados totalizados e o placar geral de liderança.

---

## 🖼️ Gestão de Fotos de Candidatos

O TSE não disponibiliza fotos abertas na CDN de apuração em tempo real e bloqueia o portal de candidaturas (*DivulgaCandContas*) contra hotlinkings externos com código HTTP 403.

Por isso, o sistema possui **estratégia dual inteligente com resiliência a falhas**:
1. **Avatar Nativo Automático com Fallback Instantâneo:** Renderiza as iniciais do candidato sobre um gradiente elegante na **cor oficial do seu partido**, com carregamento instantâneo, zero dependência externa e sem poluir o console com erros 404.
2. **Fotos Próprias Locais (Opcional):** Para exibir a foto real de qualquer candidato, basta adicionar a imagem na pasta `wwwroot/img/candidatos/` nomeada com o **número de urna** (ex: `10.jpg`, `13.jpg`, `22.jpg`). O sistema detecta a imagem e faz a renderização automaticamente.

---

## 👤 Autor

Desenvolvido por **John Dias**  
- **Website / Portal:** [johnmarlon.com.br](https://johnmarlon.com.br)  
- **Projeto:** Apuração Eleitoral TSE & Inteligência de Dados

---

## 📄 Licença

**No License (Proprietário)**  
Copyright © 2024–2026 John Dias. Todos os direitos reservados.  
Nenhuma parte deste software, código-fonte, layout ou documentação pode ser reproduzida, distribuída ou transmitida sob qualquer forma sem a autorização prévia por escrito do autor.
