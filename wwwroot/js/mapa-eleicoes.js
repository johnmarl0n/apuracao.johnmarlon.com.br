/**
 * mapa-eleicoes.js - Mapa Interativo Cartográfico via Leaflet.js
 * Inspirado na estética moderna e responsiva do seuimposto.com
 * Suporte a: Nível Brasil (27 UFs), Nível Municípios (São Paulo e Polos) e Exterior (Urnas no Mundo - ZZ)
 */

const MapaEleicoesLeaflet = {
    map: null,
    tileLayer: null,
    camadaEstados: null,
    camadaMunicipios: null,
    camadaExterior: null,
    dadosCache: null,
    visaoAtual: 'brasil', // 'brasil', 'sp', 'exterior'

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
        "SP": { lat: -23.5505, lng: -46.6333, zoom: 6.8, nome: "São Paulo" },
        "SE": { lat: -10.5741, lng: -37.3857, zoom: 8, nome: "Sergipe" },
        "TO": { lat: -10.1753, lng: -48.2982, zoom: 6, nome: "Tocantins" }
    },

    // Cidades e Polos do Estado de São Paulo
    PolosSp: [
        { nome: "São Paulo (Capital)", codigo: "71072", lat: -23.5505, lng: -46.6333, eleitorado: "9.322.444" },
        { nome: "Campinas", codigo: "62910", lat: -22.9056, lng: -47.0608, eleitorado: "884.726", destaque: true },
        { nome: "Guarulhos", codigo: "64777", lat: -23.4542, lng: -46.5333, eleitorado: "905.000" },
        { nome: "São Bernardo do Campo", codigo: "70750", lat: -23.6944, lng: -46.5653, eleitorado: "640.000" },
        { nome: "Santo André", codigo: "70734", lat: -23.6536, lng: -46.5336, eleitorado: "580.000" },
        { nome: "São José dos Campos", codigo: "70955", lat: -23.1896, lng: -45.8869, eleitorado: "540.000" },
        { nome: "Osasco", codigo: "67890", lat: -23.5325, lng: -46.7917, eleitorado: "570.000" },
        { nome: "Ribeirão Preto", codigo: "69698", lat: -21.1767, lng: -47.8103, eleitorado: "460.000" },
        { nome: "Sorocaba", codigo: "71455", lat: -23.5015, lng: -47.4581, eleitorado: "515.000" },
        { nome: "Santos", codigo: "70718", lat: -23.9608, lng: -46.3333, eleitorado: "350.000" },
        { nome: "Piracicaba", codigo: "68616", lat: -22.7253, lng: -47.6492, eleitorado: "310.000" },
        { nome: "Bauru", codigo: "62138", lat: -22.3147, lng: -49.0606, eleitorado: "280.000" },
        { nome: "Jundiaí", codigo: "66192", lat: -23.1856, lng: -46.8978, eleitorado: "330.000" },
        { nome: "São José do Rio Preto", codigo: "70971", lat: -20.8114, lng: -49.3758, eleitorado: "340.000" },
        { nome: "Franca", codigo: "64254", lat: -20.5386, lng: -47.4008, eleitorado: "250.000" },
        { nome: "Taubaté", codigo: "71838", lat: -23.0264, lng: -45.5553, eleitorado: "235.000" },
        { nome: "Praia Grande", codigo: "68918", lat: -24.0058, lng: -46.4028, eleitorado: "260.000" },
        { nome: "Limeira", codigo: "66532", lat: -22.5647, lng: -47.4017, eleitorado: "230.000" },
        { nome: "Indaiatuba", codigo: "65471", lat: -23.0903, lng: -47.2181, eleitorado: "190.000" },
        { nome: "São Carlos", codigo: "70874", lat: -22.0175, lng: -47.8908, eleitorado: "185.000" },
        { nome: "Americana", codigo: "61310", lat: -22.7394, lng: -47.3314, eleitorado: "180.000" },
        { nome: "Araraquara", codigo: "61697", lat: -21.7944, lng: -48.1758, eleitorado: "175.000" },
        { nome: "Marília", codigo: "66893", lat: -22.2139, lng: -49.9458, eleitorado: "175.000" },
        { nome: "Presidente Prudente", codigo: "69078", lat: -22.1256, lng: -51.3889, eleitorado: "175.000" }
    ],

    // Cidades Consulares no Exterior (ZZ - Urnas no Mundo)
    CidadesExterior: [
        { cidade: "Lisboa", pais: "Portugal", lat: 38.7223, lng: -9.1393, aptos: "45.210" },
        { cidade: "Porto", pais: "Portugal", lat: 41.1579, lng: -8.6291, aptos: "30.120" },
        { cidade: "Miami", pais: "Estados Unidos", lat: 25.7617, lng: -80.1918, aptos: "40.500" },
        { cidade: "Boston", pais: "Estados Unidos", lat: 42.3601, lng: -71.0589, aptos: "35.800" },
        { cidade: "Nova York", pais: "Estados Unidos", lat: 40.7128, lng: -74.0060, aptos: "28.400" },
        { cidade: "Orlando", pais: "Estados Unidos", lat: 28.5383, lng: -81.3792, aptos: "19.300" },
        { cidade: "Londres", pais: "Reino Unido", lat: 51.5074, lng: -0.1278, aptos: "34.000" },
        { cidade: "Paris", pais: "França", lat: 48.8566, lng: 2.3522, aptos: "18.200" },
        { cidade: "Tóquio", pais: "Japão", lat: 35.6762, lng: 139.6503, aptos: "21.000" },
        { cidade: "Nagoia", pais: "Japão", lat: 35.1815, lng: 136.9066, aptos: "32.000" },
        { cidade: "Roma", pais: "Itália", lat: 41.9028, lng: 12.4964, aptos: "14.500" },
        { cidade: "Milão", pais: "Itália", lat: 45.4642, lng: 9.1900, aptos: "19.800" },
        { cidade: "Berlim", pais: "Alemanha", lat: 52.5200, lng: 13.4050, aptos: "12.000" },
        { cidade: "Frankfurt", pais: "Alemanha", lat: 50.1109, lng: 8.6821, aptos: "15.300" },
        { cidade: "Zurique", pais: "Suíça", lat: 47.3769, lng: 8.5417, aptos: "16.100" },
        { cidade: "Buenos Aires", pais: "Argentina", lat: -34.6037, lng: -58.3816, aptos: "13.500" },
        { cidade: "Santiago", pais: "Chile", lat: -33.4489, lng: -70.6693, aptos: "8.200" },
        { cidade: "Sydney", pais: "Austrália", lat: -33.8688, lng: 151.2093, aptos: "9.500" },
        { cidade: "Dublin", pais: "Irlanda", lat: 53.3498, lng: -6.2603, aptos: "17.400" },
        { cidade: "Madrid", pais: "Espanha", lat: 40.4168, lng: -3.7038, aptos: "22.000" }
    ],

    inicializar(containerId = "mapaInterativoLeaflet") {
        const el = document.getElementById(containerId);
        if (!el) return;

        // Centro do Brasil
        this.map = L.map(containerId, {
            center: [-14.2350, -51.9253],
            zoom: 4,
            minZoom: 2,
            maxZoom: 14,
            zoomControl: false,
            attributionControl: false
        });

        // Controles de Zoom personalizados no canto inferior direito
        L.control.zoom({ position: 'bottomright' }).addTo(this.map);

        // Definir camada base (Dark Matter do CartoDB)
        this.atualizarTileLayer();

        // Grupos de camadas
        this.camadaEstados = L.layerGroup().addTo(this.map);
        this.camadaMunicipios = L.layerGroup().addTo(this.map);
        this.camadaExterior = L.layerGroup();

        // Observer de tema claro/escuro
        const observer = new MutationObserver(() => this.atualizarTileLayer());
        observer.observe(document.documentElement, { attributes: true, attributeFilter: ['data-theme'] });
    },

    atualizarTileLayer() {
        if (!this.map) return;
        const isLight = document.documentElement.getAttribute('data-theme') === 'light';
        const url = isLight 
            ? 'https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png'
            : 'https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png';

        if (this.tileLayer) {
            this.map.removeLayer(this.tileLayer);
        }
        this.tileLayer = L.tileLayer(url, { maxZoom: 19 }).addTo(this.map);
    },

    renderizarEstados(dados) {
        if (!this.map || !this.camadaEstados) return;
        this.dadosCache = dados;
        this.camadaEstados.clearLayers();

        const estados = dados?.estados || {};

        for (const [uf, info] of Object.entries(this.CoordenadasUfs)) {
            const estadoData = estados[uf] || estados[uf.toLowerCase()];
            const lider = estadoData?.lider;
            const cor = lider?.corPartido || '#3b82f6';
            const perc = estadoData?.percentualTotalizado || 0;
            const liderNome = lider ? lider.nomeUrna : "Aguardando";
            const liderPerc = lider ? `${lider.percentual.toFixed(2)}%` : "--";

            // Marcador circular vetorial estilizado
            const circulo = L.circleMarker([info.lat, info.lng], {
                radius: 14,
                fillColor: cor,
                color: '#ffffff',
                weight: 1.5,
                opacity: 0.9,
                fillOpacity: 0.75
            });

            // Tooltip moderno com dados da apuração
            const tooltipHtml = `
                <div style="font-family: 'Outfit', sans-serif; min-width: 150px;">
                    <div class="fw-bold text-white small">${info.nome} (${uf})</div>
                    <div style="font-size: 0.75rem; color: #94a3b8; margin-bottom: 4px;">${perc.toFixed(1)}% urnas apuradas</div>
                    <div style="display: flex; align-items: center; justify-content: space-between; gap: 8px;">
                        <span style="font-size: 0.8rem; font-weight: 700; color: ${cor};">${liderNome}</span>
                        <span style="font-family: 'JetBrains Mono', monospace; font-size: 0.85rem; font-weight: 700; color: #ffffff;">${liderPerc}</span>
                    </div>
                    <div style="font-size: 0.68rem; color: #38bdf8; margin-top: 4px;">Clique para abrir este estado</div>
                </div>
            `;
            circulo.bindTooltip(tooltipHtml, { className: 'leaflet-tooltip-dark', direction: 'top' });

            circulo.on('click', () => {
                if (uf === 'SP') {
                    this.alternarVisao('sp');
                } else {
                    const ano = window.anoAtual || '2026';
                    window.location.href = `?ano=${ano}&cargo=1&uf=${uf.toLowerCase()}`;
                }
            });

            // Ícone de texto com a sigla da UF
            const textIcon = L.divIcon({
                className: 'uf-label-pin',
                html: `<span style="font-family: 'Outfit', sans-serif; font-weight: 800; font-size: 10px; color: #ffffff; text-shadow: 0 1px 3px rgba(0,0,0,0.8);">${uf}</span>`,
                iconSize: [20, 20],
                iconAnchor: [10, 10]
            });
            const textMarker = L.marker([info.lat, info.lng], { icon: textIcon, interactive: false });

            this.camadaEstados.addLayer(circulo);
            this.camadaEstados.addLayer(textMarker);
        }
    },

    renderizarPolosSp() {
        if (!this.map || !this.camadaMunicipios) return;
        this.camadaMunicipios.clearLayers();

        for (const p of this.PolosSp) {
            const isCampinas = p.codigo === "62910";
            const corPin = isCampinas ? "#f59e0b" : "#38bdf8";

            const pinHtml = `
                <div style="position: relative; display: flex; align-items: center; justify-content: center; cursor: pointer;">
                    <div style="width: 14px; height: 14px; border-radius: 50%; background: ${corPin}; border: 2px solid #ffffff; box-shadow: 0 0 10px ${corPin};"></div>
                    <div style="position: absolute; top: -18px; white-space: nowrap; font-size: 11px; font-weight: 700; color: #ffffff; text-shadow: 0 1px 3px rgba(0,0,0,0.9); font-family: 'Outfit', sans-serif;">
                        ${p.nome}
                    </div>
                </div>
            `;

            const icon = L.divIcon({
                className: 'city-pin-custom',
                html: pinHtml,
                iconSize: [20, 20],
                iconAnchor: [10, 10]
            });

            const marker = L.marker([p.lat, p.lng], { icon: icon });

            const tooltipHtml = `
                <div style="font-family: 'Outfit', sans-serif; min-width: 140px;">
                    <div class="fw-bold text-white small">${p.nome} - SP</div>
                    <div style="font-size: 0.72rem; color: #94a3b8;">Eleitorado: <strong>${p.eleitorado}</strong></div>
                    <div style="font-size: 0.68rem; color: #f59e0b; margin-top: 4px;">Clique para abrir apuração da cidade</div>
                </div>
            `;
            marker.bindTooltip(tooltipHtml, { className: 'leaflet-tooltip-dark' });

            marker.on('click', () => {
                const ano = window.anoAtual || '2026';
                window.location.href = `?ano=${ano}&cargo=1&uf=sp&municipio=${p.codigo}`;
            });

            this.camadaMunicipios.addLayer(marker);
        }
    },

    renderizarExterior() {
        if (!this.map || !this.camadaExterior) return;
        this.camadaExterior.clearLayers();

        const dataZz = this.dadosCache?.estados?.["ZZ"] || this.dadosCache?.estados?.["zz"];
        const liderZz = dataZz?.lider;
        const liderNome = liderZz ? liderZz.nomeUrna : "Aguardando";
        const liderPerc = liderZz ? `${liderZz.percentual.toFixed(2)}%` : "--";

        for (const c of this.CidadesExterior) {
            const pinHtml = `
                <div style="position: relative; display: flex; align-items: center; justify-content: center; cursor: pointer;">
                    <div style="width: 12px; height: 12px; border-radius: 50%; background: #06b6d4; border: 2px solid #ffffff; box-shadow: 0 0 10px #06b6d4;"></div>
                    <div style="position: absolute; top: -16px; white-space: nowrap; font-size: 10px; font-weight: 700; color: #38bdf8; text-shadow: 0 1px 3px rgba(0,0,0,0.9); font-family: 'Outfit', sans-serif;">
                        ${c.cidade}
                    </div>
                </div>
            `;

            const icon = L.divIcon({
                className: 'exterior-pin-custom',
                html: pinHtml,
                iconSize: [20, 20],
                iconAnchor: [10, 10]
            });

            const marker = L.marker([c.lat, c.lng], { icon: icon });

            const tooltipHtml = `
                <div style="font-family: 'Outfit', sans-serif; min-width: 150px;">
                    <div class="fw-bold text-white small">${c.cidade} (${c.pais})</div>
                    <div style="font-size: 0.72rem; color: #94a3b8;">Consulado / Seção Eleitoral</div>
                    <div style="font-size: 0.75rem; color: #38bdf8; margin-top: 3px;">Eleitores: <strong>${c.aptos}</strong></div>
                    <div style="font-size: 0.7rem; color: #ffffff; border-top: 1px solid rgba(255,255,255,0.1); margin-top: 4px; padding-top: 3px;">
                        Líder no Exterior: <strong>${liderNome}</strong> (${liderPerc})
                    </div>
                </div>
            `;
            marker.bindTooltip(tooltipHtml, { className: 'leaflet-tooltip-dark' });

            marker.on('click', () => {
                const ano = window.anoAtual || '2026';
                window.location.href = `?ano=${ano}&cargo=1&uf=zz`;
            });

            this.camadaExterior.addLayer(marker);
        }
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
            if (lblModo) lblModo.innerText = "Brasil • 27 Unidades da Federação";
            this.map.removeLayer(this.camadaExterior);
            this.map.removeLayer(this.camadaMunicipios);
            this.map.addLayer(this.camadaEstados);
            this.map.flyTo([-14.2350, -51.9253], 4, { duration: 1.2 });
        } 
        else if (visao === 'sp') {
            btnSp?.classList.add('active');
            if (lblModo) lblModo.innerText = "Estado de SP • Cidades e Polos Regionais";
            this.map.removeLayer(this.camadaExterior);
            this.map.addLayer(this.camadaEstados);
            this.map.addLayer(this.camadaMunicipios);
            this.renderizarPolosSp();
            this.map.flyTo([-22.5, -48.5], 7, { duration: 1.2 });
        } 
        else if (visao === 'exterior') {
            btnExt?.classList.add('active');
            if (lblModo) lblModo.innerText = "Urnas no Mundo • Consulados e Embaixadas (ZZ)";
            this.map.removeLayer(this.camadaEstados);
            this.map.removeLayer(this.camadaMunicipios);
            this.map.addLayer(this.camadaExterior);
            this.renderizarExterior();
            this.map.flyTo([25.0, -10.0], 2.2, { duration: 1.5 });
        }
    }
};

window.MapaEleicoesLeaflet = MapaEleicoesLeaflet;
