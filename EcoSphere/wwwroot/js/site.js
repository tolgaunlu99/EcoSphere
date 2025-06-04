document.addEventListener("DOMContentLoaded", function () {
    if (typeof ol === "undefined") {
        console.error("OpenLayers yüklenemedi!");
        return;
    }

    // ── Form dropdown bağlılıkları ─────────────────────────────
    const regionDropdown = document.getElementById('RegionDropdown');
    const provinceDropdown = document.getElementById('ProvinceDropdown');
    const districtDropdown = document.getElementById('DistrictDropdown');
    const localityDropdown = document.getElementById('LocalityDropdown');
    const neighborhoodDropdown = document.getElementById('NeighborhoodDropdown');

    if (regionDropdown && provinceDropdown) {
        regionDropdown.addEventListener('change', function () {
            const regionID = this.value;
            fetch(`/ObservationView/GetCitysByRegion?RegionID=${regionID}`)
                .then(res => res.json())
                .then(data => {
                    provinceDropdown.innerHTML = '<option value="">Select City</option>';
                    data.forEach(p => {
                        const opt = document.createElement('option');
                        opt.value = p.value;
                        opt.textContent = p.text;
                        provinceDropdown.appendChild(opt);
                    });
                    if (districtDropdown) districtDropdown.innerHTML = '<option value="">Select District</option>';
                    if (localityDropdown) localityDropdown.innerHTML = '<option value="">Select Locality</option>';
                    if (neighborhoodDropdown) neighborhoodDropdown.innerHTML = '<option value="">Select Neighborhood</option>';
                });
        });
    }

    if (provinceDropdown && districtDropdown) {
        provinceDropdown.addEventListener('change', function () {
            const cityID = this.value;
            fetch(`/ObservationView/GetDistrictsByCity?CityID=${cityID}`)
                .then(res => res.json())
                .then(data => {
                    districtDropdown.innerHTML = '<option value="">Select District</option>';
                    data.forEach(d => {
                        const opt = document.createElement('option');
                        opt.value = d.value;
                        opt.textContent = d.text;
                        districtDropdown.appendChild(opt);
                    });
                    if (localityDropdown) localityDropdown.innerHTML = '<option value="">Select Locality</option>';
                    if (neighborhoodDropdown) neighborhoodDropdown.innerHTML = '<option value="">Select Neighborhood</option>';
                });
        });
    }

    if (districtDropdown && localityDropdown) {
        districtDropdown.addEventListener('change', function () {
            const districtID = this.value;
            fetch(`/ObservationView/GetLocalitiesByDistrict?DistrictID=${districtID}`)
                .then(res => res.json())
                .then(data => {
                    localityDropdown.innerHTML = '<option value="">Select Locality</option>';
                    data.forEach(l => {
                        const opt = document.createElement('option');
                        opt.value = l.value;
                        opt.textContent = l.text;
                        localityDropdown.appendChild(opt);
                    });
                    if (neighborhoodDropdown) neighborhoodDropdown.innerHTML = '<option value="">Select Neighborhood</option>';
                });
        });
    }

    if (localityDropdown && neighborhoodDropdown) {
        localityDropdown.addEventListener('change', function () {
            const localityID = this.value;
            fetch(`/ObservationView/GetNeighbourhoodsByLocality?LocalityID=${localityID}`)
                .then(res => res.json())
                .then(data => {
                    neighborhoodDropdown.innerHTML = '<option value="">Select Neighborhood</option>';
                    data.forEach(n => {
                        const opt = document.createElement('option');
                        opt.value = n.value;
                        opt.textContent = n.text;
                        neighborhoodDropdown.appendChild(opt);
                    });
                });
        });
    }

    if (!document.getElementById("map")) return;

    // ── Loader kontrol fonksiyonları ──────────────────────────
    function showLoader() {
        const l = document.getElementById("map-loader");
        if (l) l.style.display = "block";
    }
    function hideLoader() {
        const l = document.getElementById("map-loader");
        if (l) l.style.display = "none";
    }

    // ── Harita ve overlay ─────────────────────────────────────
    const map = new ol.Map({
        target: "map",
        layers: [new ol.layer.Tile({ source: new ol.source.OSM() })],
        view: new ol.View({
            center: ol.proj.fromLonLat([35.2433, 38.9637]),
            zoom: 6
        })
    });

    // ── Uydu ve OSM katmanları ──────────────────────────────
    const osmLayer = new ol.layer.Tile({ source: new ol.source.OSM() });
    const satelliteLayer = new ol.layer.Tile({
        source: new ol.source.XYZ({
            url: 'https://mt1.google.com/vt/lyrs=s&x={x}&y={y}&z={z}',
            attributions: '© Google'
        })
    });

    // İlk katmanı OSM olarak ayarla
    map.getLayers().setAt(0, osmLayer);

    // ── Toggle butonu oluştur ───────────────────────────────
    const toggleBtn = document.createElement("button");
    toggleBtn.innerHTML = `<i class="fas fa-globe"></i> Uydu Görünümü`;
    toggleBtn.className = "satellite-toggle";

    Object.assign(toggleBtn.style, {
        position: "absolute",
        top: "10px",
        right: "10px",
        zIndex: "1000",
        backgroundColor: "white",
        border: "none",
        padding: "6px 10px",
        borderRadius: "6px",
        boxShadow: "0px 2px 5px rgba(0,0,0,0.3)",
        fontSize: "0.9rem",
        cursor: "pointer"
    });

    document.getElementById("map").appendChild(toggleBtn);

    // ── Butona tıklayınca katman değiştir ───────────────────
    let isSatellite = false;
    toggleBtn.addEventListener("click", function () {
        if (isSatellite) {
            map.getLayers().setAt(0, osmLayer);
            toggleBtn.innerHTML = `<i class="fas fa-globe"></i> Uydu Görünümü`;
        } else {
            map.getLayers().setAt(0, satelliteLayer);
            toggleBtn.innerHTML = `<i class="fas fa-map"></i> Harita Görünümü`;
        }
        isSatellite = !isSatellite;
    });

    const popup = document.getElementById("popup"),
        popupContent = document.getElementById("popup-content"),
        overlay = new ol.Overlay({
            element: popup,
            positioning: "bottom-center",
            stopEvent: false,
            offset: [0, -10]
        });
    map.addOverlay(overlay);

    // ── Stil tanımları ───────────────────────────────────────
    const defaultProvinceStyle = new ol.style.Style({
        stroke: new ol.style.Stroke({ color: "green", width: 2 }),
        fill: new ol.style.Fill({ color: "rgba(0,255,0,0)" })
    });
    const hoverProvinceStyle = new ol.style.Style({
        stroke: new ol.style.Stroke({ color: "green", width: 2 }),
        fill: new ol.style.Fill({ color: "rgba(0,255,0,0.3)" })
    });
    const defaultDistrictStyle = new ol.style.Style({
        stroke: new ol.style.Stroke({ color: "green", width: 2 }),
        fill: new ol.style.Fill({ color: "rgba(0,255,0,0)" })
    });
    const hoverDistrictStyle = new ol.style.Style({
        stroke: new ol.style.Stroke({ color: "green", width: 2 }),
        fill: new ol.style.Fill({ color: "rgba(0,255,0,0.3)" })
    });

    // ── İlçe katmanı (başlangıçta gizli) ──────────────────────
    const ilceLayer = new ol.layer.Vector({
        source: new ol.source.Vector(),
        style: defaultDistrictStyle,
        visible: false
    });
    ilceLayer.setZIndex(10);
    map.addLayer(ilceLayer);

    // ── Marker katmanı ─────────────────────────────────────────
    const markerLayer = new ol.layer.Vector({
        source: new ol.source.Vector(),
        style: new ol.style.Style({
            image: new ol.style.Circle({
                radius: 6,
                fill: new ol.style.Fill({ color: "red" }),
                stroke: new ol.style.Stroke({ color: "#fff", width: 2 })
            })
        })
    });
    markerLayer.setZIndex(15);
    map.addLayer(markerLayer);

    // ── İl katmanı ve etkileşim ───────────────────────────────
    fetch("/data/TR_iller.geojson")
        .then(res => res.json())
        .then(data => {
            const format = new ol.format.GeoJSON();
            const provinceFeatures = format.readFeatures(data, {
                featureProjection: map.getView().getProjection()
            });

            const provinceLayer = new ol.layer.Vector({
                source: new ol.source.Vector({ features: provinceFeatures }),
                style: defaultProvinceStyle
            });
            provinceLayer.setZIndex(5);
            map.addLayer(provinceLayer);

            // Hover mantığı
            let prevHover = { feature: null, layer: null };
            map.on("pointermove", function (evt) {
                let hit = false;
                map.forEachFeatureAtPixel(evt.pixel, (feature, layer) => {
                    if (layer === provinceLayer || layer === ilceLayer) {
                        hit = true;
                        if (prevHover.feature && prevHover.feature !== feature) {
                            prevHover.feature.setStyle(
                                prevHover.layer === provinceLayer
                                    ? defaultProvinceStyle
                                    : defaultDistrictStyle
                            );
                        }
                        feature.setStyle(
                            layer === provinceLayer
                                ? hoverProvinceStyle
                                : hoverDistrictStyle
                        );
                        prevHover = { feature, layer };
                        return true;
                    }
                });
                if (!hit && prevHover.feature) {
                    prevHover.feature.setStyle(
                        prevHover.layer === provinceLayer
                            ? defaultProvinceStyle
                            : defaultDistrictStyle
                    );
                    prevHover = { feature: null, layer: null };
                }

                // Popup göster/gizle
                let found = false;
                map.forEachFeatureAtPixel(evt.pixel, feature => {
                    const p = feature.getProperties();
                    if (p.name || p.Ilce_Adi || p.Il_Adi) {
                        popupContent.innerHTML = p.name || p.Ilce_Adi || p.Il_Adi;
                        overlay.setPosition(
                            p.name
                                ? feature.getGeometry().getCoordinates()
                                : evt.coordinate
                        );
                        popup.style.display = "block";
                        found = true;
                        return true;
                    }
                });
                if (!found) popup.style.display = "none";
            });

            // Tıklama mantığı
            map.on("singleclick", function (evt) {
                map.forEachFeatureAtPixel(evt.pixel, (feature, layer) => {
                    const provinceName = feature.get("Il_Adi"),
                        districtName = feature.get("Ilce_Adi"),
                        basePath = window.location.pathname;

                    // ── İl seçildi
                    if (provinceName && layer === provinceLayer) {
                        history.pushState(null, "", `${basePath}?province=${encodeURIComponent(provinceName)}`);
                        showLoader();
                        markerLayer.getSource().clear();
                        map.getView().fit(feature.getGeometry().getExtent(), { duration: 1000 });

                        // İlçeleri yükle
                        fetch("/data/ilceler.geojson")
                            .then(r => r.json())
                            .then(ilceData => {
                                const ilceFeats = format.readFeatures(ilceData, {
                                    featureProjection: map.getView().getProjection()
                                });
                                const filtered = ilceFeats.filter(f => f.get("Il_Adi") === provinceName);
                                ilceLayer.getSource().clear();
                                ilceLayer.getSource().addFeatures(filtered);
                                ilceLayer.setVisible(true);
                            });

                        // Marker’ları getir ve renk ata
                        fetch(`/ObservationView/GetObservationsByProvince?province=${encodeURIComponent(provinceName)}`)
                            .then(r => r.json())
                            .then(obs => {
                                const feats = obs.map(o => {
                                    const ft = new ol.Feature({
                                        geometry: new ol.geom.Point(
                                            ol.proj.fromLonLat([+o.long, +o.lat])
                                        )
                                    });

                                    ft.set("name", o.name);
                                    ft.set("id", o.id);

                                    // ► Kingdom’a göre renk atama
                                    const markerColor =
                                        o.kingdom === "Animal" ? "red" :
                                            o.kingdom === "Plant" ? "purple" :
                                                "gray";

                                    ft.setStyle(new ol.style.Style({
                                        image: new ol.style.Circle({
                                            radius: 6,
                                            fill: new ol.style.Fill({ color: markerColor }),
                                            stroke: new ol.style.Stroke({ color: "#fff", width: 2 })
                                        })
                                    }));

                                    return ft;
                                });
                                markerLayer.getSource().addFeatures(feats);
                            })

                            .finally(() => hideLoader());

                        return true;
                    }

                    // ── İlçe seçildi
                    if (districtName && layer === ilceLayer) {
                        history.pushState(null, "", `${basePath}?district=${encodeURIComponent(districtName)}`);
                        showLoader();
                        markerLayer.getSource().clear();
                        map.getView().fit(feature.getGeometry().getExtent(), { duration: 1000 });

                        fetch(`/ObservationView/GetObservationsByDistrict?district=${encodeURIComponent(districtName)}`)
                            .then(r => r.json())
                            .then(obs => {
                                const feats = obs.map(o => {
                                    const ft = new ol.Feature({
                                        geometry: new ol.geom.Point(
                                            ol.proj.fromLonLat([+o.long, +o.lat])
                                        )
                                    });

                                    ft.set("name", o.name);
                                    ft.set("id", o.id);

                                    // ► Burada da aynı renk atama
                                    const markerColor =
                                        o.kingdom === "Animal" ? "red" :
                                            o.kingdom === "Plant" ? "purple" :
                                                "gray";

                                    ft.setStyle(new ol.style.Style({
                                        image: new ol.style.Circle({
                                            radius: 6,
                                            fill: new ol.style.Fill({ color: markerColor }),
                                            stroke: new ol.style.Stroke({ color: "#fff", width: 2 })
                                        })
                                    }));

                                    return ft;
                                });
                                markerLayer.getSource().addFeatures(feats);
                            })
                            .finally(() => hideLoader());

                        return true;
                    }
                });
            });


            // Marker’a tıklanınca detay sayfası
            map.on("click", function (evt) {
                const feat = map.forEachFeatureAtPixel(evt.pixel, f => f);
                if (feat && feat.get("id")) {
                    window.location.href = `/ObservationView/Details/${feat.get("id")}`;
                }
            });
        });
});