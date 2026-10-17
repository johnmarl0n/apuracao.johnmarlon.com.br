# 🗳️ Monitor de Apuração TSE & Base Histórica Eleitoral

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C# 12](https://img.shields.io/badge/C%23-12-239120?logo=csharp)](https://docs.microsoft.com/dotnet/csharp/)
[![SmartASP MSSQL](https://img.shields.io/badge/Database-SQL%20Server-CC292B?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![Leaflet.js](https://img.shields.io/badge/Maps-Leaflet.js-199900?logo=leaflet)](https://leafletjs.com/)
[![CartoDB](https://img.shields.io/badge/Tiles-CartoDB%20Positron%20%2F%20Dark-0096D6)](https://carto.com/)
[![License: Proprietary](https://img.shields.io/badge/License-Proprietary-red.svg)](#-licença)
[![Author](https://img.shields.io/badge/Author-John%20Dias-blue.svg)](#-autor)

Plataforma corporativa de alta performance para auditoria, visualização analítica e acompanhamento em tempo real de apurações eleitorais oficiais do **Tribunal Superior Eleitoral (TSE)**. 

Construída em **ASP.NET Core 8.0 MVC**, com arquitetura em cache volátil de múltiplos níveis, persistência em **SQL Server (SmartASP)**, mapa cartográfico interativo via **Leaflet.js** com malhas geográficas vetoriais (GeoJSON) dos 645 municípios de São Paulo e das 27 UFs do Brasil, painel de monitoramento de **usuários online em tempo real**, **timeline interativa (scrubber temporal)**, e uma interface adaptativa moderna desenhada para **preencher 100% da altura da tela (100vh)** sem barras de rolagem vertical no desktop.

---

## 📋 Sumário

- [Visão Geral & Interface](#-visão-geral--interface)
- [Arquitetura do Dashboard (3 Colunas em 100vh)](#-arquitetura-do-dashboard-3-colunas-em-100vh)
- [Mapa Interativo Cartográfico (Leaflet + GeoJSON)](#-mapa-interativo-cartográfico-leaflet--geojson)
- [Monitoramento de Usuários Online (Ao Vivo)](#-monitoramento-de-usuários-online-ao-vivo)
- [Linha do Tempo Interativa (Timeline Scrubber)](#-linha-do-tempo-interativa-timeline-scrubber)
- [Base Histórica de Eleições Gerais (2002 a 2022)](#-base-histórica-de-eleições-gerais-2002-a-2022)
- [Arquitetura & Tecnologias](#-arquitetura--tecnologias)
- [Banco de Dados MSSQL (Tabelas & Snapshots)](#-banco-de-dados-mssql-tabelas--snapshots)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Instalação & Execução Local](#-instalação--execução-local)
- [Deploy e Hospedagem (SmartASP / IIS)](#-deploy-e-hospedagem-smartasp--iis)
- [API RESTful Documentada](#-api-restful-documentada)
- [Gestão de Fotos de Candidatos](#-gestão-de-fotos-de-candidatos)
- [Autor](#-autor)
- [Licença](#-licença)

---

## 🎯 Visão Geral & Interface

O sistema conecta-se de forma assíncrona aos servidores de dados abertos do TSE (`resultados.tse.jus.br`) para processar e totalizar os boletins de urna à medida que são divulgados, garantindo confiabilidade estatística e fluidez na navegação.

### Destaques da Interface:
- **Layout Profissional sem Rolagem (100vh):** Projetado sob métricas estritas de layout fluido. No desktop (`>= 992px`), todas as 3 colunas e seus componentes alinham-se perfeitamente na linha de base inferior da janela, eliminando qualquer scroll vertical indesejado.
- **Suporte Total a Dark / Light Mode:** Design System de alto contraste semântico (WCAG AA), com alternância instantânea de temas para todos os componentes (incluindo mapas de tiles CartoDB Dark Matter e Positron).
- **Menu Lateral Offcanvas de Alta Legibilidade:** Gaveta de opções com acesso à base histórica (2002 a 2022) e endpoints de API, estilizada com tipografia legível e contrastes específicos para temas claro e escuro.
- **Drill-down Instantâneo de Localidades:** Navegação em SPA sem recarregar a página (Brasil ⇄ São Paulo ⇄ Cidades) atualizando gráficos, placares e mapa de forma sincronizada.

---

## 🏛️ Arquitetura do Dashboard (3 Colunas em 100vh)

O dashboard é organizado em 3 colunas independentes e balanceadas:

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│ NAVBAR: Apuração 2026 | Pleito | Localidade | Exterior | Compartilhar | Tela Cheia | Atualizar │
├──────────────────────────┬─────────────────────────────┬───────────────────────────────┤
│ [COLUNA ESQUERDA]        │ [COLUNA CENTRAL]            │ [COLUNA DIREITA]              │
│                          │                             │                               │
│ 1. Card Duelo 2º Turno:  │ 1. Filtros de Visão:        │ 1. Card Por Região:           │
│    • Avatares Squircle   │    • Municípios (SP)        │    • Norte, Nordeste,         │
│    • Headline Serifada   │    • Estados (Brasil)       │      Centro-Oeste, Sudeste,    │
│    • Percentuais Grandes │    • Exterior (ZZ)          │      Sul e Exterior           │
│    • Barra Duelo (50%)   │                             │    • Mini-barras e fotos      │
│    • Diferença de Votos  │ 2. Mapa Interativo Leaflet: │                               │
│    • Comparecimento      │    • Camada CartoDB         │ 2. Card Últimas Atualizações: │
│    • Regressiva 25/10    │    • GeoJSON 645 municípios │    • Feed ao vivo com fotos   │
│                          │    • Tooltip com placar     │    • Máscara de gradiente     │
│ 2. Ao Longo da Apuração: │                             │      inferior (sem rollover)  │
│    • Gráfico SVG temporal│ 3. Timeline Scrubber:       │                               │
│      esticado até a base │    • Relógio de Brasília    │                               │
│                          │    • Range Slider (0-100%)  │                               │
│                          │    • Contador Online Ao Vivo│                               │
└──────────────────────────┴─────────────────────────────┴───────────────────────────────┘
```

1. **Coluna Esquerda (Card Duelo & Evolução Temporal):**
   - **Identidade Visual Refinada:** Avatares em formato *squircle* com fotos reais dos candidatos ou iniciais sobre o gradiente partidário.
   - **Headline Serifada:** Destaque editorial moderno (*"Flávio Bolsonaro e Lula vão ao 2º turno"*).
   - **Métricas Chave:** Votos válidos, comparecimento, brancos, nulos e contagem regressiva para abertura das urnas do 2º turno (25/10).
   - **Gráfico Vetorial ("Ao longo da apuração"):** Curvas em SVG responsivo que acompanham a evolução de cada candidato de 0% a 100% das seções apuradas.

2. **Coluna Central (Mapa Interativo & Controle Temporal):**
   - **Mapa Cartográfico Leaflet:** Renderização ultraveloz com tiles CartoDB e malhas GeoJSON de alta fidelidade.
   - **Barra de Linha do Tempo (Scrubber):** Permite rebobinar o tempo e ver a configuração da disputa hora a hora durante a totalização.
   - **Contador de Presença Online:** Exibe a quantidade de usuários ativos acompanhando a página em tempo real.

3. **Coluna Direita (Votação por Região & Feed de Notícias):**
   - **Liderança Regional:** Desempenho nas 5 regiões do Brasil (Norte, Nordeste, Centro-Oeste, Sudeste, Sul) e no Exterior com mini-barras duel e percentuais de liderança.
   - **Feed com Fade Gradiente:** Linha do tempo de acontecimentos e eleições estaduais com efeito de esmaecimento suave no rodapé (`mask-image`), eliminando barras de rolagem e sensação de rollover.

---

## 🗺️ Mapa Interativo Cartográfico (Leaflet + GeoJSON)

O módulo de mapas substitui imagens estáticas por uma solução dinâmica com a biblioteca **Leaflet 1.9.4**:

- **Malha dos 645 Municípios de São Paulo:** Renderização vetorial baseada em coordenadas oficiais do IBGE, pintando cada cidade de acordo com o candidato vitorioso na localidade (azul para PL, vermelho para PT).
- **Visão do Brasil (27 UFs):** Visão nacional com os estados coloridos pelo partido líder, permitindo clicar em qualquer estado para detalhar a apuração local.
- **Votação no Exterior (ZZ):** Acompanhamento exclusivo das urnas de brasileiros espalhadas em mais de 140 cidades pelo mundo (Lisboa, Tóquio, Paris, Miami, etc.).
- **Modal de Zonas Eleitorais:** Ao clicar em Campinas ou em qualquer município polo, abre-se uma janela modal com a distribuição detalhada de votos por zona eleitoral (Zonas 33ª, 274ª, 275ª, 378ª, 379ª, 380ª e 423ª).

---

## 👥 Monitoramento de Usuários Online (Ao Vivo)

A plataforma incorpora rastreamento de audiência concorrente em tempo real:

- **Arquitetura Thread-Safe:** Implementado via [OnlinePresenceService.cs](file:///e:/99.Projetos/CSharp/VS2022/apuracao.johnmarlon.com.br/Services/OnlinePresenceService.cs), utilizando um `ConcurrentDictionary<string, DateTime>` em memória com limpeza automática de conexões inativas (timeout de 35 segundos).
- **Sessão por Navegador:** Cada visitante gera um identificador único anônimo salvo em `sessionStorage`.
- **Heartbeat Assíncrono:** O frontend envia um ping a cada 15 segundos para o endpoint `GET /api/apuracao/presenca?id={cid}`.
- **Exibição Dinâmica:** A barra do mapa exibe o contador atualizado instantaneamente:
  ```html
  <i class="bi bi-eye text-primary"></i> 1 pessoa agora • Ao vivo
  ```

---

## ⏳ Linha do Tempo Interativa (Timeline Scrubber)

O controle deslizante na base do mapa atua como uma máquina do tempo da eleição:

- **Endpoint Dedicado:** `GET /api/apuracao/timeline` retorna os marcos de totalização minuto a minuto (das 17h00 às 22h00).
- **Interação em Tempo Real:** Ao arrastar o slider de 0% a 100%:
  1. O relógio oficial de Brasília (`#lblLiveClockScrubber`) muda para o horário exato daquele marco.
  2. O placar de votos e percentuais dos candidatos no card de duelo retrocede para refletir o momento histórico da apuração.
  3. A barra central de 50% recalcula a liderança provisória.
  4. Ao soltar ou posicionar em 100%, o dashboard restaura instantaneamente os dados consolidados ao vivo.

---

## 🏛️ Base Histórica de Eleições Gerais (2002 a 2022)

Para contornar o comportamento do TSE — que remove pleitos anteriores de suas CDNs de tempo real após a homologação oficial gerando erros HTTP 404 —, a plataforma incorpora um repositório nativo de dados históricos oficiais ([DadosHistoricosTse.cs](file:///e:/99.Projetos/CSharp/VS2022/apuracao.johnmarlon.com.br/Services/DadosHistoricosTse.cs)) com persistência automática no SQL Server.

| Ano | Pleito Principal | Vencedor / Disputa |
|---|---|---|
| **2022** | Eleições Gerais 2022 | Lula (48,43%) vs Jair Bolsonaro (43,20%) - 1º Turno |
| **2018** | Eleições Gerais 2018 | Jair Bolsonaro (46,03%) vs Fernando Haddad (29,28%) |
| **2014** | Eleições Gerais 2014 | Dilma Rousseff (41,59%) vs Aécio Neves (33,55%) |
| **2010** | Eleições Gerais 2010 | Dilma Rousseff (46,91%) vs José Serra (32,61%) |
| **2006** | Eleições Gerais 2006 | Lula (48,61%) vs Geraldo Alckmin (41,64%) |
| **2002** | Eleições Gerais 2002 | Lula (46,44%) vs José Serra (23,20%) |

---

## 🛠️ Arquitetura & Tecnologias

| Camada | Tecnologia | Descrição |
|---|---|---|
| **Backend** | .NET 8.0 (C# 12) | ASP.NET Core MVC & Web API com controllers tipados |
| **Acesso a Dados** | Dapper 2.1 | Micro-ORM de altíssima performance para queries SQL brutas |
| **Banco de Dados** | Microsoft SQL Server | Hospedado na nuvem (SQL Server / Azure SQL / SmartASP) |
| **Cache em Memória** | `IMemoryCache` | Cache volátil de 20s para prevenir throttling e absorver picos de tráfego |
| **Presença Online** | `OnlinePresenceService` | Serviço singleton concorrente para contagem de clientes ativos |
| **Cartografia** | Leaflet.js 1.9.4 | Renderização vetorial interativa de mapas de municípios e estados |
| **Tiles de Mapa** | CartoDB Positron / Dark Matter | Camadas de mapa sob demanda adaptadas ao tema ativo |
| **Frontend** | HTML5 / CSS3 / Vanilla JS | Estilização moderna via Glassmorphism, CSS Custom Properties e Bootstrap 5.3 |
| **Tipografia & Ícones** | Google Fonts & Bootstrap Icons | Fontes *Inter*, *Playfair Display* (serif) e *JetBrains Mono* |
| **Codificação** | UTF-8 com BOM | Arquivos Razor (.cshtml) gravados com BOM para blindagem contra mojibake no IIS |
| **Servidor Web** | IIS / Kestrel | Configurado em modo `OutOfProcess` no `web.config` |

---

## 🗄️ Banco de Dados MSSQL (Tabelas & Snapshots)

A classe [DatabaseInitializer.cs](file:///e:/99.Projetos/CSharp/VS2022/apuracao.johnmarlon.com.br/Data/DatabaseInitializer.cs) cria e atualiza automaticamente as tabelas e índices necessários no SQL Server:

1. **`ApuracaoSnapshots`**: Registra cada leitura e totalização de votos por cargo, UF e município.
2. **`ApuracaoCandidatos`**: Votos nominais, situação (Eleito, 2º Turno) e percentuais de cada candidato.
3. **`ApuracaoTimeline`**: Armazena a curva de evolução da apuração ao longo do tempo (pontos de amostragem por hora/minuto).
4. **`CandidatosFotos`**: Cadastro de fotos e avatares oficiais associados ao número de urna e ano do pleito.

---

## 📂 Estrutura do Projeto

```text
apuracao.johnmarlon.com.br/
├── Controllers/
│   ├── ApuracaoApiController.cs # Endpoints REST de resumo, presença, timeline e municípios
│   ├── AuthController.cs        # Login, autenticação e logout de sessão
│   └── HomeController.cs        # View principal, renderização de dados e mapa Brasil
├── Data/
│   ├── ApuracaoRepository.cs    # Operações Dapper no SQL Server (Snapshots e histórico)
│   └── DatabaseInitializer.cs   # DDL automática: tabelas, índices e constraints
├── Models/
│   ├── ApuracaoViewModel.cs     # ViewModels da tela, candidatos e métricas
│   ├── MapaBrasilDto.cs         # DTOs para UFs, exterior e placar de estados
│   └── Tse/
│       └── TseDto.cs            # Mapeamento do JSON oficial da CDN do TSE
├── Services/
│   ├── DadosHistoricosTse.cs    # Base homologada consolidada (2002 a 2022)
│   ├── OnlinePresenceService.cs # Monitor de usuários online concorrentes em memória
│   └── TseService.cs            # Engine de consumo HTTP, cache volátil e normalização
├── Views/
│   ├── Auth/
│   │   └── Login.cshtml         # Tela de autenticação administrativa
│   ├── Home/
│   │   ├── Index.cshtml         # Dashboard 3 colunas em 100vh com duelo e feed
│   │   └── _MapaBrasil.cshtml   # Partial view Leaflet com scrubber e presença
│   └── Shared/
│       └── _Layout.cshtml       # Master template (Navbar, tema claro/escuro, offcanvas)
├── wwwroot/
│   ├── img/
│   │   └── candidatos/          # Fotos oficiais dos candidatos (formato squircle/webp)
│   ├── js/
│   │   ├── geolocalizacao.js    # Serviço de detecção e trilha de localidade
│   │   └── mapa-eleicoes.js     # Engine Leaflet de renderização de cidades e estados
│   ├── favicon.ico              # Favicon multi-resolução
│   └── favicon.svg              # Ícone vetorial oficial
├── appsettings.json             # Parâmetros de eleição, connection string e chaves
└── web.config                   # Configuração OutOfProcess para IIS/SmartASP
```

---

## 💻 Instalação & Execução Local

### Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.
- Visual Studio 2022 ou VS Code.

### Passos
1. Clone o repositório e navegue até a pasta:
   ```bash
   git clone https://github.com/johnmarl0n/apuracao.johnmarlon.com.br.git
   cd apuracao.johnmarlon.com.br
   ```

2. Restaure os pacotes e dependências:
   ```bash
   dotnet restore
   ```

3. Execute a aplicação em modo de desenvolvimento:
   ```bash
   dotnet run --launch-profile http
   ```

4. Acesse pelo navegador:
   ```text
   http://localhost:5161/?ano=2026&cargo=1&uf=sp
   ```

---

## 🚀 Deploy e Hospedagem (SmartASP / IIS)

O projeto está otimizado para ambientes compartilhados Windows IIS (como o **SmartASP**):

1. **Compilação e Publicação:**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Modelo de Processo no `web.config`:**
   ```xml
   <aspNetCore processPath="dotnet" 
               arguments=".\apuracao.johnmarlon.com.br.dll" 
               stdoutLogEnabled="true" 
               stdoutLogFile=".\logs\stdout" 
               hostingModel="OutOfProcess" />
   ```
   *O modo `OutOfProcess` evita erros HTTP 503 causados por reciclagem de Application Pools.*

3. **Otimização de Pacotes de Idioma:**
   O `.csproj` restringe idiomas extras para manter a pasta de deploy leve e rápida de enviar via FTP:
   ```xml
   <SatelliteResourceLanguages>pt-BR</SatelliteResourceLanguages>
   ```

---

## 📡 API RESTful Documentada

A plataforma expõe endpoints abertos e protegidos para consumo externo em JSON:

### 1. Resumo da Apuração
- **Rota:** `GET /api/apuracao/resumo`
- **Parâmetros:**
  - `cargo`: `1` (Presidente), `3` (Governador), `5` (Senador), `6` (Dep. Federal), `7` (Dep. Estadual)
  - `uf`: `br`, `sp` ou qualquer sigla de estado
  - `municipio`: código TSE do município (ex: `62910` para Campinas)
  - `ano`: ano da eleição (`2026`, `2022`, `2018`, `2014`, `2010`, `2006`, `2002`)

### 2. Presença Online (Ao Vivo)
- **Rota:** `GET /api/apuracao/presenca?id={cid}`
- **Retorno:**
  ```json
  {
    "sucesso": true,
    "online": 42,
    "texto": "42 pessoas agora",
    "dataHora": "20:30:15"
  }
  ```

### 3. Linha do Tempo (Scrubber)
- **Rota:** `GET /api/apuracao/timeline`
- **Retorno:** Array com os marcos da totalização (hora, % de seções, percentuais de cada candidato e status descritivo).

### 4. Busca de Municípios
- **Rota:** `GET /api/apuracao/municipios?uf=sp&q=Campinas`
- **Retorno:** Lista de municípios com código oficial do TSE para drill-down dinâmico.

### 5. Dados Agregados do Mapa
- **Rota:** `GET /Home/DadosMapaBrasil?ano=2026`
- **Retorno:** Status de todas as 27 UFs + ZZ, líder e vice-líder por estado e o placar geral de estados liderados.

---

## 🖼️ Gestão de Fotos de Candidatos

O sistema emprega estratégia dual para fotos oficiais:
1. **Fotos Oficiais Locais:** Arquivos `.webp` ou `.jpg` no diretório `wwwroot/img/candidatos/` nomeados pelo número do candidato (`13.jpg`, `22.jpg`, `lula.webp`, `bolsonaro.webp`, `douglas_ruas.webp`, `tarcisio.webp`, etc.).
2. **Fallback Inteligente:** Caso a imagem não esteja disponível, o componente renderiza automaticamente um avatar em formato *squircle* com as iniciais do candidato sobre a cor oficial da sua legenda, sem gerar erros de rede no console.

---

## 👤 Autor

Desenvolvido por **John Dias**  
- **Website / Portal:** [johnmarlon.com.br](https://johnmarlon.com.br)  
- **LinkedIn:** [linkedin.com/in/johnmarlondias](https://linkedin.com/in/johnmarlondias)  
- **Projeto:** Monitor de Apuração TSE & Inteligência de Dados Eleitorais

---

## 📄 Licença

**Proprietário — No License**  
Copyright © 2024–2026 John Dias. Todos os direitos reservados.  
Nenhuma parte deste software, layout ou código-fonte pode ser reproduzida ou distribuída sem autorização prévia por escrito do autor.
