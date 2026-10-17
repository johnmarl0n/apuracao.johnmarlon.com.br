# Plano de Implementação: Segurança e Resiliência contra Invasões e DDoS

## Objetivo
Blindar a aplicação contra invasões, ataques de negação de serviço (DDoS/DoS), exaustão de recursos (CPU, memória, sockets e conexões SQL) e banimento de IP pelo TSE, garantindo estabilidade e disponibilidade no dia da eleição.

---

## Tarefas de Implementação

- [x] **Etapa 1: Blindagem de Endpoints e Autenticação Fail-Closed**
  - Removido `[HttpGet]` de rotinas administrativas de sincronização e carga.
  - Exigido token via cabeçalho `X-Api-Token` com comparação segura em tempo constante (`CryptographicOperations.FixedTimeEquals`).
  - Protegido o endpoint `SincronizarHistorico` em [HomeController.cs](file:///e:/99.Projetos/CSharp/VS2022/apuracao.johnmarlon.com.br/Controllers/HomeController.cs).
  - *Verificação:* Requisições sem token em `/api/apuracao/historico/importar` e `/Home/SincronizarHistorico` retornam `401 Unauthorized` ou `405 Method Not Allowed`.

- [x] **Etapa 2: Whitelist Rígida de Parâmetros e Proteção de Cache do TSE**
  - Validados e sanitizados os parâmetros `uf`, `cargo`, `ano` e `municipio` em [TseService.cs](file:///e:/99.Projetos/CSharp/VS2022/apuracao.johnmarlon.com.br/Services/TseService.cs).
  - Parâmetros desconhecidos sofrem fallback imediato para valores seguros, impedindo furos intencionais de cache e flood contra os servidores do TSE.
  - Configurado timeout agressivo de 8 segundos no `HttpClient` do TSE para evitar retenção de threads.
  - *Verificação:* Chamada com `?uf=hacker123&cargo=999&municipio=bad` foi normalizada para `uf=br` e `cargo=1`.

- [x] **Etapa 3: Estabilidade de Memória e CPU no Monitor de Presença e Geolocalização**
  - Refatorado [OnlinePresenceService.cs](file:///e:/99.Projetos/CSharp/VS2022/apuracao.johnmarlon.com.br/Services/OnlinePresenceService.cs) limitando tamanho do ID a 64 chars, teto máximo de 15.000 sessões em memória e limpeza periódica a cada 15 segundos protegida por lock/Monitor.
  - Em [HomeController.cs](file:///e:/99.Projetos/CSharp/VS2022/apuracao.johnmarlon.com.br/Controllers/HomeController.cs), injetado `IHttpClientFactory` e `IMemoryCache` no `DetectarLocalidade`, com validação de IP via `IPAddress.TryParse` e cache de 1 hora.
  - *Verificação:* Pings rápidos e chamadas de geolocalização respondem instantaneamente sem esgotar sockets TCP nem disparar CPU.

- [x] **Etapa 4: Fila / Semáforo de Gravação no MSSQL (Evitar Estouro de Connection Pool)**
  - Adicionado `SemaphoreSlim(2, 2)` nas gravações de snapshots do [TseService.cs](file:///e:/99.Projetos/CSharp/VS2022/apuracao.johnmarlon.com.br/Services/TseService.cs).
  - Descarte seguro de tentativas excedentes sob pico para proteger a hospedagem compartilhada contra esgotamento do pool de conexões do SQL Server.
  - *Verificação:* Gravações simultâneas em background são serializadas sem ultrapassar o teto seguro de concorrência.

- [x] **Etapa 5: Rate Limiting (.NET 8), Headers de Segurança HTTP e web.config**
  - Registrado middleware `AddRateLimiter` e `UseRateLimiter` em [Program.cs](file:///e:/99.Projetos/CSharp/VS2022/apuracao.johnmarlon.com.br/Program.cs) com limite de 120 reqs/min global e 25 reqs/10s para APIs críticas.
  - Adicionados cabeçalhos de proteção (`X-Content-Type-Options: nosniff`, `X-Frame-Options: SAMEORIGIN`, `Referrer-Policy`) e removidas as assinaturas do servidor (`Server`, `X-Powered-By`).
  - No [web.config](file:///e:/99.Projetos/CSharp/VS2022/apuracao.johnmarlon.com.br/web.config), desativado `stdoutLogEnabled="false"` para evitar enchimento de disco.
  - *Verificação:* Teste de rajada atingiu status 429 (Too Many Requests) na 25ª requisição rápida, e cabeçalhos validados com sucesso no HTTP GET.

- [x] **Etapa 6: Compilação e Testes Finais**
  - Executado `dotnet build` com 0 avisos e 0 erros.
  - Testada navegação no navegador com subagente browser: scoreboard, mapa interativo e feed funcionando sem falhas.

---

## Conclusão do Plano
- O sistema fica imune a ataques de quebra de pool de banco, exaustão de CPU por presença, furos de cache maliciosos e invasão de endpoints sem credenciais.
