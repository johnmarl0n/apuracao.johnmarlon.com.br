/**
 * geolocalizacao.js - Detecção silenciosa e não-invasiva da localidade do visitante
 * Hierarquia de exibição: Brasil (padrão) -> Estado do visitante -> Cidade do visitante
 */

const GeoLocalizacaoService = {
    CHAVE_CACHE: 'apuracao_user_geo_loc',
    CACHE_EXPIRACAO_MS: 1000 * 60 * 60 * 24, // 24 horas

    // Mapeamento básico de cidades do Estado de SP com código TSE
    CidadesSpTse: {
        "campinas": { codigo: "62910", nome: "Campinas" },
        "sao paulo": { codigo: "71072", nome: "São Paulo" },
        "santos": { codigo: "70718", nome: "Santos" },
        "ribeirao preto": { codigo: "69698", nome: "Ribeirão Preto" },
        "sorocaba": { codigo: "71455", nome: "Sorocaba" },
        "sao jose dos campos": { codigo: "70955", nome: "São José dos Campos" },
        "guarulhos": { codigo: "64777", nome: "Guarulhos" },
        "sao bernardo do campo": { codigo: "70750", nome: "São Bernardo do Campo" },
        "santo andre": { codigo: "70734", nome: "Santo André" },
        "osasco": { codigo: "67890", nome: "Osasco" },
        "piracicaba": { codigo: "68616", nome: "Piracicaba" },
        "bauru": { codigo: "62138", nome: "Bauru" },
        "jundiai": { codigo: "66192", nome: "Jundiaí" },
        "sao jose do rio preto": { codigo: "70971", nome: "São José do Rio Preto" },
        "franca": { codigo: "64254", nome: "Franca" },
        "taubate": { codigo: "71838", nome: "Taubaté" },
        "praia grande": { codigo: "68918", nome: "Praia Grande" },
        "limeira": { codigo: "66532", nome: "Limeira" },
        "suzano": { codigo: "71676", nome: "Suzano" },
        "taboao da serra": { codigo: "71773", nome: "Taboão da Serra" },
        "sumare": { codigo: "71633", nome: "Sumaré" },
        "barueri": { codigo: "62073", nome: "Barueri" },
        "embu das artes": { codigo: "63975", nome: "Embu das Artes" },
        "indaiatuba": { codigo: "65471", nome: "Indaiatuba" },
        "sao carlos": { codigo: "70874", nome: "São Carlos" },
        "americana": { codigo: "61310", nome: "Americana" },
        "araraquara": { codigo: "61697", nome: "Araraquara" },
        "marilia": { codigo: "66893", nome: "Marília" },
        "presidente prudente": { codigo: "69078", nome: "Presidente Prudente" }
    },

    normalizarTexto(str) {
        if (!str) return '';
        return str.toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "")
            .trim();
    },

    obterCache() {
        try {
            const raw = localStorage.getItem(this.CHAVE_CACHE);
            if (!raw) return null;
            const data = JSON.parse(raw);
            if (Date.now() - (data.timestamp || 0) < this.CACHE_EXPIRACAO_MS) {
                return data.loc;
            }
        } catch (e) {
            console.warn("Erro ao ler cache de geolocalização:", e);
        }
        return null;
    },

    salvarCache(loc) {
        try {
            localStorage.setItem(this.CHAVE_CACHE, JSON.stringify({
                timestamp: Date.now(),
                loc: loc
            }));
        } catch (e) {
            console.warn("Erro ao salvar cache de geolocalização:", e);
        }
    },

    async detectarLocalidade() {
        // 1. Tentar ler do cache rápido
        const cached = this.obterCache();
        if (cached) return cached;

        // 2. Consulta de IP gratuita e não bloqueante
        try {
            const res = await fetch('https://ipapi.co/json/', { cache: 'no-store' });
            if (!res.ok) throw new Error("Falha na consulta ipapi");
            const data = await res.json();

            const pais = data.country_code || 'BR';
            const estado = (data.region_code || '').toLowerCase();
            const cidade = data.city || '';
            const cidadeNorm = this.normalizarTexto(cidade);

            let codigoMunTse = null;
            let nomeMunTse = cidade;

            if (estado === 'sp' && this.CidadesSpTse[cidadeNorm]) {
                codigoMunTse = this.CidadesSpTse[cidadeNorm].codigo;
                nomeMunTse = this.CidadesSpTse[cidadeNorm].nome;
            }

            const loc = {
                pais: pais,
                estado: estado,
                estadoNome: data.region || estado.toUpperCase(),
                cidade: nomeMunTse,
                codigoMunicipio: codigoMunTse,
                isExterior: pais !== 'BR'
            };

            this.salvarCache(loc);
            return loc;
        } catch (err) {
            // Fallback silencioso (ex: Campinas como polo de demonstração ou Capital)
            const fallbackLoc = {
                pais: 'BR',
                estado: 'sp',
                estadoNome: 'São Paulo',
                cidade: 'Campinas',
                codigoMunicipio: '62910',
                isExterior: false
            };
            return fallbackLoc;
        }
    },

    /**
     * Aplica os chips na barra de localidade:
     * Brasil (Nacional) -> Estado do visitante -> Cidade do visitante
     */
    async aplicarTrilhaLocalidade(anoAtual, cargoAtual, ufAtual, munAtual) {
        const container = document.getElementById("barraLocalidadesContainer");
        if (!container) return;

        const loc = await this.detectarLocalidade();
        if (!loc) return;

        // Se for exterior, o estado equivalente no TSE é 'zz'
        const estadoUf = loc.isExterior ? 'zz' : (loc.estado || 'sp');
        const estadoLabel = loc.isExterior ? 'Exterior (ZZ)' : (loc.estadoNome || loc.estado.toUpperCase());
        const cidadeNome = loc.cidade || 'Sua Cidade';
        const munParam = loc.codigoMunicipio ? `&municipio=${loc.codigoMunicipio}` : '';

        // Montar chip de Estado
        const isEstadoAtivo = (ufAtual === estadoUf && !munAtual);
        const isCidadeAtiva = (munAtual && loc.codigoMunicipio === munAtual);

        let htmlChips = `
            <span class="text-secondary small fw-bold text-uppercase me-1">
                <i class="bi bi-geo-alt-fill text-primary"></i> Localidade:
            </span>

            <!-- 1º Nível: Brasil (Nacional) -->
            <a href="?ano=${anoAtual}&cargo=${cargoAtual}&uf=br" 
               class="filter-chip ${(ufAtual === 'br' && !munAtual) ? 'active' : ''}">
                <i class="bi bi-globe-americas"></i> Brasil (Total)
            </a>

            <!-- 2º Nível: Estado do Visitante -->
            <a href="?ano=${anoAtual}&cargo=${cargoAtual}&uf=${estadoUf}" 
               class="filter-chip ${isEstadoAtivo ? 'active' : ''}" 
               title="Ver votos de todo o ${estadoLabel}">
                <i class="bi bi-map-fill"></i> ${estadoLabel}
            </a>
        `;

        // 3º Nível: Cidade do Visitante (se detectada e mapeada)
        if (loc.codigoMunicipio) {
            htmlChips += `
                <a href="?ano=${anoAtual}&cargo=${cargoAtual}&uf=${estadoUf}${munParam}" 
                   class="filter-chip chip-sua-cidade ${isCidadeAtiva ? 'active' : ''}" 
                   style="${isCidadeAtiva ? '' : 'border-color: rgba(245, 158, 11, 0.45); background: rgba(245, 158, 11, 0.08);'}"
                   title="Clique para ver a apuração em ${cidadeNome}">
                    <i class="bi bi-pin-map-fill text-warning"></i> ${cidadeNome} 
                    <span class="badge bg-warning text-dark font-mono px-1 py-0 ms-1" style="font-size:0.65rem;">Sua Cidade</span>
                </a>
            `;
        }

        // Se o usuário selecionou outra cidade diferente da detectada
        if (munAtual && munAtual !== loc.codigoMunicipio) {
            const nomeCidadeUrl = document.getElementById("lblNomeCidadeAtual")?.innerText || "Cidade Selecionada";
            htmlChips += `
                <div class="filter-chip active d-flex align-items-center gap-2">
                    <i class="bi bi-building"></i>
                    <span>${nomeCidadeUrl}</span>
                    <a href="?ano=${anoAtual}&cargo=${cargoAtual}&uf=${ufAtual}" class="text-white text-decoration-none ms-1" title="Remover filtro de cidade">
                        <i class="bi bi-x-circle-fill"></i>
                    </a>
                </div>
            `;
        }

        container.innerHTML = htmlChips;
    }
};

window.GeoLocalizacaoService = GeoLocalizacaoService;
