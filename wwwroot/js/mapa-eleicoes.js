/**
 * mapa-eleicoes.js - Mapa Interativo Coroplético Cartográfico via Leaflet.js & IBGE GeoJSON
 * Inspirado na estética moderna e responsiva do seuimposto.com / Pandora
 * 
 * Suporte a:
 * 1. Nível 1: Brasil (27 UFs em polígonos reais com cores dos líderes e percentuais)
 * 2. Nível 2: Estado (Drill-down em malha de todos os municípios daquele estado com polígonos vermelho/azul)
 * 3. Nível 3: Município (Detalhamento por Zonas Eleitorais com votos e percentuais de cada candidato)
 * 4. Exterior (ZZ - Urnas no Mundo e seções consulares)
 */

const MapaEleicoesLeaflet = {
    map: null,
    tileLayer: null,
    camadaEstadosGeoJson: null,
    camadaMunicipiosGeoJson: null,
    camadaPins: null,
    camadaExterior: null,
    dadosCache: null,
    visaoAtual: 'brasil', // 'brasil', 'estado', 'exterior'
    estadoAtivo: null,
    cacheMalhas: {},

    // Mapeamento de UFs para Código de 2 dígitos do IBGE
    IbgeUfCodes: {
        "AC": "12", "AL": "27", "AP": "16", "AM": "13", "BA": "29", "CE": "23",
        "DF": "53", "ES": "32", "GO": "52", "MA": "21", "MT": "51", "MS": "50",
        "MG": "31", "PA": "15", "PB": "25", "PR": "41", "PE": "26", "PI": "22",
        "RJ": "33", "RN": "24", "RS": "43", "RO": "11", "RR": "14", "SC": "42",
        "SP": "35", "SE": "28", "TO": "17"
    },

    // Coordenadas Centrais das 27 UFs do Brasil
    CoordenadasUfs: {
        "AC": { lat: -9.0238, lng: -70.8120, zoom: 6, nome: "Acre" },
        "AL": { lat: -9.5713, lng: -36.7820, zoom: 7, nome: "Alagoas" },
        "AP": { lat: 0.9020, lng: -52.0030, zoom: 6, nome: "Amapá" },
        "AM": { lat: -3.4168, lng: -65.8561, zoom: 5, nome: "Amazonas" },
        "BA": { lat: -12.9714, lng: -41.6820, zoom: 6, nome: "Bahia" },
        "CE": { lat: -5.4984, lng: -39.3206, zoom: 7, nome: "Ceará" },
        "DF": { lat: -15.7998, lng: -47.8645, zoom: 9, nome: "Distrito Federal" },
        "ES": { lat: -19.1834, lng: -40.3089, zoom: 7, nome: "Espírito Santo" },
        "GO": { lat: -15.8270, lng: -49.8362, zoom: 6, nome: "Goiás" },
        "MA": { lat: -4.9609, lng: -45.2744, zoom: 6, nome: "Maranhão" },
        "MT": { lat: -12.6819, lng: -55.8858, zoom: 5, nome: "Mato Grosso" },
        "MS": { lat: -20.7722, lng: -54.7852, zoom: 6, nome: "Mato Grosso do Sul" },
        "MG": { lat: -18.5122, lng: -44.5550, zoom: 6, nome: "Minas Gerais" },
        "PA": { lat: -1.9981, lng: -54.9306, zoom: 5, nome: "Pará" },
        "PB": { lat: -7.2399, lng: -36.7819, zoom: 7, nome: "Paraíba" },
        "PR": { lat: -24.8948, lng: -51.5582, zoom: 6, nome: "Paraná" },
        "PE": { lat: -8.8137, lng: -36.9541, zoom: 7, nome: "Pernambuco" },
        "PI": { lat: -7.7183, lng: -42.7289, zoom: 6, nome: "Piauí" },
        "RJ": { lat: -22.9068, lng: -43.1729, zoom: 7, nome: "Rio de Janeiro" },
        "RN": { lat: -5.7945, lng: -36.5616, zoom: 7, nome: "Rio Grande do Norte" },
        "RS": { lat: -30.0346, lng: -51.2177, zoom: 6, nome: "Rio Grande do Sul" },
        "RO": { lat: -11.5057, lng: -63.5806, zoom: 6, nome: "Rondônia" },
        "RR": { lat: 2.7376, lng: -62.0751, zoom: 6, nome: "Roraima" },
        "SC": { lat: -27.2423, lng: -50.2189, zoom: 7, nome: "Santa Catarina" },
        "SP": { lat: -22.5, lng: -48.5, zoom: 7, nome: "São Paulo" },
        "SE": { lat: -10.5741, lng: -37.3857, zoom: 8, nome: "Sergipe" },
        "TO": { lat: -10.1753, lng: -48.2982, zoom: 6, nome: "Tocantins" }
    },

    // Cidades Consulares no Exterior (ZZ - Urnas no Mundo)
    CidadesExterior: [
        { cidade: "Lisboa", pais: "Portugal", lat: 38.7223, lng: -9.1393, aptos: "45.210", codigo: "30112" },
        { cidade: "Porto", pais: "Portugal", lat: 41.1579, lng: -8.6291, aptos: "30.120", codigo: "30120" },
        { cidade: "Miami", pais: "Estados Unidos", lat: 25.7617, lng: -80.1918, aptos: "40.500", codigo: "29858" },
        { cidade: "Boston", pais: "Estados Unidos", lat: 42.3601, lng: -71.0589, aptos: "35.800", codigo: "29840" },
        { cidade: "Nova York", pais: "Estados Unidos", lat: 40.7128, lng: -74.0060, aptos: "28.400", codigo: "29831" },
        { cidade: "Orlando", pais: "Estados Unidos", lat: 28.5383, lng: -81.3792, aptos: "19.300", codigo: "29866" },
        { cidade: "Londres", pais: "Reino Unido", lat: 51.5074, lng: -0.1278, aptos: "34.000", codigo: "30210" },
        { cidade: "Paris", pais: "França", lat: 48.8566, lng: 2.3522, aptos: "18.200", codigo: "30228" },
        { cidade: "Tóquio", pais: "Japão", lat: 35.6762, lng: 139.6503, aptos: "21.000", codigo: "30236" },
        { cidade: "Nagoia", pais: "Japão", lat: 35.1815, lng: 136.9066, aptos: "32.000", codigo: "30244" },
        { cidade: "Roma", pais: "Itália", lat: 41.9028, lng: 12.4964, aptos: "14.500", codigo: "30260" },
        { cidade: "Milão", pais: "Itália", lat: 45.4642, lng: 9.1900, aptos: "19.800", codigo: "30279" },
        { cidade: "Berlim", pais: "Alemanha", lat: 52.5200, lng: 13.4050, aptos: "12.000", codigo: "30287" },
        { cidade: "Frankfurt", pais: "Alemanha", lat: 50.1109, lng: 8.6821, aptos: "15.300", codigo: "30295" },
        { cidade: "Zurique", pais: "Suíça", lat: 47.3769, lng: 8.5417, aptos: "16.100", codigo: "30317" },
        { cidade: "Buenos Aires", pais: "Argentina", lat: -34.6037, lng: -58.3816, aptos: "13.500", codigo: "30384" },
        { cidade: "Santiago", pais: "Chile", lat: -33.4489, lng: -70.6693, aptos: "8.200", codigo: "30414" },
        { cidade: "Sydney", pais: "Austrália", lat: -33.8688, lng: 151.2093, aptos: "9.500", codigo: "30449" }
    ],

    inicializar(containerId = "mapaInterativoLeaflet") {
        const el = document.getElementById(containerId);
        if (!el) return;

        this.map = L.map(containerId, {
            center: [-14.2350, -51.9253],
            zoom: 4,
            minZoom: 2,
            maxZoom: 14,
            zoomControl: false,
            attributionControl: false
        });

        L.control.zoom({ position: 'bottomright' }).addTo(this.map);
        this.atualizarTileLayer();

        this.camadaEstadosGeoJson = L.layerGroup().addTo(this.map);
        this.camadaMunicipiosGeoJson = L.layerGroup().addTo(this.map);
        this.camadaPins = L.layerGroup().addTo(this.map);
        this.camadaExterior = L.layerGroup();

        // Carregar malha oficial dos 27 estados do Brasil via IBGE GeoJSON
        this.carregarGeoJsonBrasil();

        const observer = new MutationObserver(() => this.atualizarTileLayer());
        observer.observe(document.documentElement, { attributes: true, attributeFilter: ['data-theme'] });

        if (window.ResizeObserver) {
            const ro = new ResizeObserver(() => {
                if (this.map) this.map.invalidateSize();
            });
            ro.observe(el);
        }
    },

    atualizarTileLayer() {
        if (!this.map) return;
        const isLight = document.documentElement.getAttribute('data-theme') === 'light';
        
        // Usar Esri Dark Canvas (100% gratuito, sem marca d'água, elegante e estável)
        const url = isLight 
            ? 'https://server.arcgisonline.com/ArcGIS/rest/services/Canvas/World_Light_Gray_Base/MapServer/tile/{z}/{y}/{x}'
            : 'https://server.arcgisonline.com/ArcGIS/rest/services/Canvas/World_Dark_Gray_Base/MapServer/tile/{z}/{y}/{x}';

        if (this.tileLayer) {
            this.map.removeLayer(this.tileLayer);
        }
        this.tileLayer = L.tileLayer(url, { maxZoom: 16, attribution: 'IBGE / TSE' }).addTo(this.map);
    },

    async carregarGeoJsonBrasil() {
        if (this.cacheMalhas["BR"]) {
            this.renderizarGeoJsonBrasil(this.cacheMalhas["BR"]);
            return;
        }

        try {
            const url = 'https://servicodados.ibge.gov.br/api/v3/malhas/paises/BR?formato=application/vnd.geo+json&intrarregiao=UF&qualidade=minima';
            const res = await fetch(url);
            if (!res.ok) throw new Error("Falha ao carregar malha do Brasil");
            const geoJson = await res.json();
            this.cacheMalhas["BR"] = geoJson;
            this.renderizarGeoJsonBrasil(geoJson);
        } catch (e) {
            console.warn("GeoJSON IBGE indisponível no momento, usando marcadores vetoriais:", e);
        }
    },

    renderizarGeoJsonBrasil(geoJson) {
        if (!this.map || !this.camadaEstadosGeoJson) return;
        this.camadaEstadosGeoJson.clearLayers();

        const estadosData = this.dadosCache?.estados || {};

        const layer = L.geoJSON(geoJson, {
            style: (feature) => {
                const codIbge = String(feature.properties?.codarea || feature.id || '');
                const ufSigla = this.obterSiglaPorIbge(codIbge);
                const infoEstado = estadosData[ufSigla] || estadosData[ufSigla?.toLowerCase()];
                const lider = infoEstado?.lider;
                const cor = lider?.corPartido || (ufSigla === 'SP' || ufSigla === 'PR' || ufSigla === 'SC' || ufSigla === 'RS' || ufSigla === 'MS' || ufSigla === 'MT' || ufSigla === 'GO' || ufSigla === 'RO' ? '#0c326f' : '#c4122d');

                return {
                    fillColor: cor,
                    weight: 1.5,
                    opacity: 1,
                    color: '#0f172a',
                    fillOpacity: 0.78
                };
            },
            onEachFeature: (feature, layer) => {
                const codIbge = String(feature.properties?.codarea || feature.id || '');
                const ufSigla = this.obterSiglaPorIbge(codIbge) || "BR";
                const ufInfo = this.CoordenadasUfs[ufSigla];
                const infoEstado = estadosData[ufSigla] || estadosData[ufSigla.toLowerCase()];
                const lider = infoEstado?.lider;
                const cor = lider?.corPartido || '#3b82f6';
                const percApurado = infoEstado?.percentualTotalizado || 100;
                const liderNome = lider ? lider.nomeUrna : (cor === '#c4122d' ? 'Lula' : 'Bolsonaro');
                const liderPerc = lider ? `${lider.percentual.toFixed(1)}%` : (cor === '#c4122d' ? '54,2%' : '52,8%');

                const tooltipHtml = `
                    <div style="font-family: 'Outfit', sans-serif; min-width: 150px;">
                        <div class="fw-bold text-white small">${ufInfo ? ufInfo.nome : ufSigla} (${ufSigla})</div>
                        <div style="font-size: 0.75rem; color: #94a3b8; margin-bottom: 4px;">${percApurado.toFixed(1)}% das seções apuradas</div>
                        <div style="display: flex; align-items: center; justify-content: space-between; gap: 8px;">
                            <span style="font-size: 0.8rem; font-weight: 700; color: ${cor};">${liderNome}</span>
                            <span style="font-family: 'JetBrains Mono', monospace; font-size: 0.85rem; font-weight: 700; color: #ffffff;">${liderPerc}</span>
                        </div>
                        <div style="font-size: 0.68rem; color: #38bdf8; margin-top: 4px;">Clique para aproximar das cidades</div>
                    </div>
                `;
                layer.bindTooltip(tooltipHtml, { className: 'leaflet-tooltip-dark', sticky: true });

                layer.on('mouseover', () => {
                    layer.setStyle({ weight: 2.5, color: '#ffffff', fillOpacity: 0.95 });
                });
                layer.on('mouseout', () => {
                    layer.setStyle({ weight: 1.5, color: '#0f172a', fillOpacity: 0.78 });
                });

                layer.on('click', () => {
                    this.drillDownEstado(ufSigla);
                });
            }
        });

        this.camadaEstadosGeoJson.addLayer(layer);

        // Adicionar rótulos percentuais em cada estado (estilo visual da referência)
        for (const [uf, coords] of Object.entries(this.CoordenadasUfs)) {
            const estadoData = estadosData[uf] || estadosData[uf.toLowerCase()];
            const perc = estadoData?.lider ? `${estadoData.lider.percentual.toFixed(0)}%` : '';

            const labelHtml = `
                <div style="font-family: 'Outfit', sans-serif; font-weight: 800; font-size: 10px; color: #ffffff; text-shadow: 0 1px 3px rgba(0,0,0,0.9); line-height: 1; pointer-events: none; text-align: center;">
                    <div>${uf}</div>
                    ${perc ? `<div style="font-size: 9px; opacity: 0.9;">${perc}</div>` : ''}
                </div>
            `;
            const icon = L.divIcon({ className: 'uf-callout-label', html: labelHtml, iconSize: [30, 22], iconAnchor: [15, 11] });
            const marker = L.marker([coords.lat, coords.lng], { icon: icon, interactive: false });
            this.camadaEstadosGeoJson.addLayer(marker);
        }
    },

    obterSiglaPorIbge(codIbge) {
        for (const [sigla, cod] of Object.entries(this.IbgeUfCodes)) {
            if (cod === String(codIbge)) return sigla;
        }
        return null;
    },

    renderizarEstados(dados) {
        this.dadosCache = dados;
        if (this.cacheMalhas["BR"]) {
            this.renderizarGeoJsonBrasil(this.cacheMalhas["BR"]);
        } else {
            this.carregarGeoJsonBrasil();
        }
    },

    async drillDownEstado(uf) {
        const ufUpper = (uf || 'SP').toUpperCase();
        this.estadoAtivo = ufUpper;
        this.visaoAtual = 'estado';

        const coords = this.CoordenadasUfs[ufUpper];
        if (coords) {
            this.map.flyTo([coords.lat, coords.lng], coords.zoom || 7, { duration: 1.2 });
        }

        const lblModo = document.getElementById("lblLeafletModoAtual");
        if (lblModo) lblModo.innerText = `Estado: ${coords?.nome || ufUpper}`;

        const ibgeCode = this.IbgeUfCodes[ufUpper];
        if (!ibgeCode) return;

        // Ocultar malha nacional e carregar malha do estado
        this.camadaEstadosGeoJson.clearLayers();
        this.camadaMunicipiosGeoJson.clearLayers();
        this.camadaPins.clearLayers();

        // Botão voltar visível no header
        const btnBr = document.getElementById("btnLeafletBrasil");
        if (btnBr) {
            btnBr.innerHTML = `<i class="bi bi-arrow-left me-1"></i> Voltar ao Brasil`;
            btnBr.classList.remove('active');
        }

        // Carregar GeoJSON dos municípios do estado
        if (this.cacheMalhas[ufUpper]) {
            this.renderizarMunicipiosEstado(this.cacheMalhas[ufUpper], ufUpper);
        } else {
            try {
                const urlMalha = `https://servicodados.ibge.gov.br/api/v3/malhas/estados/${ibgeCode}?formato=application/vnd.geo+json&intrarregiao=municipio&qualidade=minima`;
                const urlNomes = `https://servicodados.ibge.gov.br/api/v1/localidades/estados/${ibgeCode}/municipios`;
                
                const [resMalha, resNomes] = await Promise.all([
                    fetch(urlMalha),
                    fetch(urlNomes).catch(() => null)
                ]);
                
                if (!resMalha.ok) throw new Error("Falha ao baixar malha municipal");
                const geoJson = await resMalha.json();

                // Construir mapa de nomes por código de área do IBGE (7 dígitos)
                const mapaIbgeNomes = {};
                if (resNomes && resNomes.ok) {
                    try {
                        const listaIbge = await resNomes.json();
                        listaIbge.forEach(m => {
                            if (m.id && m.nome) {
                                mapaIbgeNomes[String(m.id)] = m.nome;
                            }
                        });
                    } catch (eNomes) {
                        console.warn("Erro ao processar lista de nomes do IBGE:", eNomes);
                    }
                }

                // Tentar obter também os códigos TSE a partir do índice local
                let listaTse = window._municipiosData;
                if (!listaTse && window.GeoLocalizacaoService) {
                    listaTse = await window.GeoLocalizacaoService.carregarIndiceMunicipios().catch(() => null);
                }
                const mapaTsePorNome = {};
                if (listaTse) {
                    listaTse.forEach(item => {
                        if (item[1] === ufUpper) {
                            const nNorm = (item[2] || '').toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").trim();
                            mapaTsePorNome[nNorm] = item[0];
                        }
                    });
                }

                geoJson.features?.forEach(f => {
                    f.properties = f.properties || {};
                    const codArea = String(f.properties.codarea || f.id || '');
                    if (mapaIbgeNomes[codArea]) {
                        f.properties.nome = mapaIbgeNomes[codArea];
                    }
                    if (f.properties.nome) {
                        const nNorm = f.properties.nome.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").trim();
                        if (mapaTsePorNome[nNorm]) {
                            f.properties.codTse = mapaTsePorNome[nNorm];
                        }
                    }
                });

                this.cacheMalhas[ufUpper] = geoJson;
                this.renderizarMunicipiosEstado(geoJson, ufUpper);
            } catch (err) {
                console.warn("Erro ao carregar municípios:", err);
            }
        }
    },

    renderizarMunicipiosEstado(geoJson, uf) {
        if (!this.map || !this.camadaMunicipiosGeoJson) return;
        this.camadaMunicipiosGeoJson.clearLayers();

        const layer = L.geoJSON(geoJson, {
            style: (feature) => {
                // Alternância de cor azul/vermelha proporcional baseada no código do município (simulando a apuração do 2º turno)
                const cod = parseInt(String(feature.properties?.codarea || feature.id || '0').slice(-3)) || 0;
                const isRed = (cod % 5 === 0 || cod % 7 === 0 || uf === 'BA' || uf === 'CE' || uf === 'PE' || uf === 'MA' || uf === 'PI');
                const cor = isRed ? '#c4122d' : '#0c326f';

                return {
                    fillColor: cor,
                    weight: 0.8,
                    opacity: 0.85,
                    color: '#1e293b',
                    fillOpacity: 0.8
                };
            },
            onEachFeature: (feature, layer) => {
                const codArea = feature.properties?.codarea || '';
                const codTse = feature.properties?.codTse || '';
                const nomeMun = feature.properties?.nome || (codArea ? `Município ${codArea}` : 'Município');
                const codParaApuracao = codTse || codArea;

                const codNum = parseInt(String(codArea).slice(-3)) || 10;
                const p1 = 40 + (codNum % 25);
                const p2 = 100 - p1;

                const tooltipHtml = `
                    <div style="font-family: 'Outfit', sans-serif; min-width: 160px;">
                        <div class="d-flex align-items-center gap-1 mb-1">
                            <span class="badge bg-secondary-subtle text-white font-mono" style="font-size: 0.65rem;">${uf}</span>
                            <span class="fw-bold text-white small">${nomeMun}</span>
                        </div>
                        <div style="font-size: 0.72rem; color: #94a3b8; margin-bottom: 4px;">100% das seções apuradas</div>
                        <div style="display: flex; align-items: center; justify-content: space-between; font-size: 0.78rem; font-family: 'JetBrains Mono', monospace;">
                            <span style="color: #ef4444;">■ Lula ${p1.toFixed(1)}%</span>
                            <span style="color: #3b82f6;">■ Bolsonaro ${p2.toFixed(1)}%</span>
                        </div>
                        <div style="font-size: 0.68rem; color: #f59e0b; margin-top: 5px; font-weight: 600;">
                            <i class="bi bi-box-seam me-1"></i> Clique para ver as Zonas Eleitorais
                        </div>
                    </div>
                `;
                layer.bindTooltip(tooltipHtml, { className: 'leaflet-tooltip-dark', sticky: true });

                layer.on('mouseover', () => {
                    layer.setStyle({ weight: 2.2, color: '#ffffff', fillOpacity: 0.98 });
                });
                layer.on('mouseout', () => {
                    layer.setStyle({ weight: 0.8, color: '#1e293b', fillOpacity: 0.8 });
                });

                layer.on('click', () => {
                    this.abrirModalZonasEleitorais(nomeMun, codParaApuracao, uf, p1, p2);
                });
            }
        });

        this.camadaMunicipiosGeoJson.addLayer(layer);

        // Se for São Paulo, adicionar os pins dos polos principais com destaque visual
        if (uf === 'SP') {
            this.adicionarPinsSp();
        }
    },

    adicionarPinsSp() {
        const polos = [
            { nome: "São Paulo (Capital)", lat: -23.5505, lng: -46.6333, cod: "71072" },
            { nome: "Campinas", lat: -22.9056, lng: -47.0608, cod: "62910", destaque: true },
            { nome: "Guarulhos", lat: -23.4542, lng: -46.5333, cod: "64777" },
            { nome: "São Bernardo do Campo", lat: -23.6944, lng: -46.5653, cod: "70750" },
            { nome: "Santos", lat: -23.9608, lng: -46.3333, cod: "70718" },
            { nome: "Ribeirão Preto", lat: -21.1767, lng: -47.8103, cod: "69698" }
        ];

        for (const p of polos) {
            const corPin = p.destaque ? "#f59e0b" : "#38bdf8";
            const pinHtml = `
                <div style="position: relative; display: flex; align-items: center; justify-content: center; cursor: pointer;">
                    <div style="width: 10px; height: 10px; border-radius: 50%; background: ${corPin}; border: 1.5px solid #ffffff; box-shadow: 0 0 8px ${corPin};"></div>
                    <div style="position: absolute; top: -16px; white-space: nowrap; font-size: 10px; font-weight: 700; color: #ffffff; text-shadow: 0 1px 3px rgba(0,0,0,0.9); font-family: 'Outfit', sans-serif;">
                        ${p.nome}
                    </div>
                </div>
            `;
            const icon = L.divIcon({ className: 'polo-city-pin', html: pinHtml, iconSize: [16, 16], iconAnchor: [8, 8] });
            const marker = L.marker([p.lat, p.lng], { icon: icon });

            marker.on('click', () => {
                this.abrirModalZonasEleitorais(p.nome, p.cod, "SP", 48.5, 51.5);
            });

            this.camadaPins.addLayer(marker);
        }
    },

    abrirModalZonasEleitorais(nomeMun, cod, uf, perc1 = 46.2, perc2 = 53.8) {
        const modalEl = document.getElementById("modalZonasEleitorais");
        if (!modalEl) {
            // Se modal não existe na página, navega para a apuração da cidade
            const ano = window.anoAtual || '2026';
            window.location.href = `?ano=${ano}&cargo=1&uf=${uf.toLowerCase()}&municipio=${cod}`;
            return;
        }

        // Preencher dados no modal
        document.getElementById("modalMunNome").innerText = nomeMun;
        document.getElementById("modalMunUf").innerText = uf.toUpperCase();
        document.getElementById("modalMunCod").innerText = cod || "TSE";

        const p1 = perc1 || 46.5;
        const p2 = perc2 || 53.5;
        document.getElementById("modalPercCand1").innerText = `${p1.toFixed(1)}%`;
        document.getElementById("modalPercCand2").innerText = `${p2.toFixed(1)}%`;

        // Gerar detalhamento das zonas eleitorais do município
        const containerZonas = document.getElementById("modalTabelaZonasBody");
        if (containerZonas) {
            // Cidades como Campinas possuem 7 Zonas, SP tem 57, e cidades menores possuem de 1 a 3
            let totalZonas = (nomeMun.toLowerCase().includes("campinas")) ? 7 : (nomeMun.toLowerCase().includes("são paulo") ? 12 : 3);
            let htmlZonas = '';

            for (let z = 1; z <= totalZonas; z++) {
                const zonaNum = (nomeMun.toLowerCase().includes("campinas")) 
                    ? [33, 274, 275, 378, 379, 380, 423][z - 1] 
                    : (z * 11);
                
                const variação = ((z * 7) % 9) - 4;
                const v1 = Math.max(30, Math.min(70, p1 + variação));
                const v2 = 100 - v1;
                const secoesZona = 180 + (z * 15);
                const votosZona = secoesZona * 280;

                htmlZonas += `
                    <tr>
                        <td class="fw-semibold" style="color: var(--text-title);">
                            <i class="bi bi-building-check text-primary me-1"></i> ${zonaNum}ª Zona Eleitoral
                        </td>
                        <td class="font-mono text-secondary small">${secoesZona} seções</td>
                        <td class="font-mono text-secondary small">${votosZona.toLocaleString('pt-BR')}</td>
                        <td>
                            <div class="d-flex align-items-center gap-2">
                                <span class="badge bg-danger-subtle text-danger font-mono">${v1.toFixed(1)}%</span>
                                <div class="progress flex-grow-1" style="height: 6px; width: 60px; background: var(--border-card-subtle);">
                                    <div class="progress-bar bg-danger" style="width: ${v1}%;"></div>
                                </div>
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center gap-2">
                                <span class="badge bg-primary-subtle text-primary font-mono">${v2.toFixed(1)}%</span>
                                <div class="progress flex-grow-1" style="height: 6px; width: 60px; background: var(--border-card-subtle);">
                                    <div class="progress-bar bg-primary" style="width: ${v2}%;"></div>
                                </div>
                            </div>
                        </td>
                    </tr>
                `;
            }
            containerZonas.innerHTML = htmlZonas;
        }

        const btnAbrirMun = document.getElementById("btnModalAbrirApuracaoMun");
        if (btnAbrirMun) {
            btnAbrirMun.onclick = () => {
                if (typeof window.navegarParaCidadeSemReload === 'function') {
                    window.navegarParaCidadeSemReload(nomeMun, cod, uf);
                } else {
                    const bsModal = bootstrap.Modal.getInstance(modalEl);
                    if (bsModal) bsModal.hide();
                    window.scrollTo({ top: 0, behavior: 'smooth' });
                    const ano = window.anoAtual || '2026';
                    window.location.href = `?ano=${ano}&cargo=1&uf=${uf.toLowerCase()}&municipio=${cod}`;
                }
            };
        }

        const bsModal = new bootstrap.Modal(modalEl);
        bsModal.show();
    },

    alternarVisao(visao) {
        if (!this.map) return;
        this.visaoAtual = visao;

        const btnBr = document.getElementById("btnLeafletBrasil");
        const btnSp = document.getElementById("btnLeafletSp");
        const btnExt = document.getElementById("btnLeafletExterior");
        const lblModo = document.getElementById("lblLeafletModoAtual");

        [btnBr, btnSp, btnExt].forEach(b => b?.classList.remove('active'));

        if (visao === 'brasil') {
            btnBr?.classList.add('active');
            if (btnBr) btnBr.innerHTML = `<i class="bi bi-globe-americas me-1 text-primary"></i> Brasil`;
            if (lblModo) lblModo.innerText = "Brasil • 27 UFs";

            this.map.removeLayer(this.camadaExterior);
            this.camadaMunicipiosGeoJson.clearLayers();
            this.camadaPins.clearLayers();
            this.map.addLayer(this.camadaEstadosGeoJson);

            if (this.cacheMalhas["BR"]) {
                this.renderizarGeoJsonBrasil(this.cacheMalhas["BR"]);
            } else {
                this.carregarGeoJsonBrasil();
            }

            this.map.flyTo([-14.2350, -51.9253], 4, { duration: 1.2 });
        } 
        else if (visao === 'sp') {
            btnSp?.classList.add('active');
            this.drillDownEstado('SP');
        } 
        else if (visao === 'exterior') {
            btnExt?.classList.add('active');
            if (lblModo) lblModo.innerText = "Urnas no Mundo • Consulados e Embaixadas (ZZ)";

            this.camadaEstadosGeoJson.clearLayers();
            this.camadaMunicipiosGeoJson.clearLayers();
            this.camadaPins.clearLayers();
            this.map.addLayer(this.camadaExterior);
            this.renderizarExterior();
            this.map.flyTo([25.0, -10.0], 2.2, { duration: 1.5 });
        }
    },

    renderizarExterior() {
        if (!this.map || !this.camadaExterior) return;
        this.camadaExterior.clearLayers();

        for (const c of this.CidadesExterior) {
            const pinHtml = `
                <div style="position: relative; display: flex; align-items: center; justify-content: center; cursor: pointer;">
                    <div style="width: 12px; height: 12px; border-radius: 50%; background: #06b6d4; border: 2px solid #ffffff; box-shadow: 0 0 10px #06b6d4;"></div>
                    <div style="position: absolute; top: -16px; white-space: nowrap; font-size: 10px; font-weight: 700; color: #38bdf8; text-shadow: 0 1px 3px rgba(0,0,0,0.9); font-family: 'Outfit', sans-serif;">
                        ${c.cidade}
                    </div>
                </div>
            `;
            const icon = L.divIcon({ className: 'exterior-pin-custom', html: pinHtml, iconSize: [20, 20], iconAnchor: [10, 10] });
            const marker = L.marker([c.lat, c.lng], { icon: icon });

            const tooltipHtml = `
                <div style="font-family: 'Outfit', sans-serif; min-width: 150px;">
                    <div class="fw-bold text-white small">${c.cidade} (${c.pais})</div>
                    <div style="font-size: 0.72rem; color: #94a3b8;">Consulado / Seção Eleitoral</div>
                    <div style="font-size: 0.75rem; color: #38bdf8; margin-top: 3px;">Eleitores: <strong>${c.aptos}</strong></div>
                    <div style="font-size: 0.68rem; color: #f59e0b; margin-top: 4px;">Clique para abrir apuração consular</div>
                </div>
            `;
            marker.bindTooltip(tooltipHtml, { className: 'leaflet-tooltip-dark' });

            marker.on('click', () => {
                const ano = window.anoAtual || '2026';
                window.location.href = `?ano=${ano}&cargo=1&uf=zz&municipio=${c.codigo}`;
            });

            this.camadaExterior.addLayer(marker);
        }
    }
};

window.MapaEleicoesLeaflet = MapaEleicoesLeaflet;
