document.addEventListener("DOMContentLoaded", function () {
    if (typeof ol === "undefined") {
        console.error("OpenLayers yüklenemedi!");
        return;
    }

    if (document.getElementById('map')) {
        const map = new ol.Map({
            target: 'map',
            layers: [
                new ol.layer.Tile({ source: new ol.source.OSM() })
            ],
            view: new ol.View({
                center: ol.proj.fromLonLat([35.2433, 38.9637]),
                zoom: 6
            })
        });

        const popup = document.getElementById('popup');
        const popupContent = document.getElementById('popup-content');

        const overlay = new ol.Overlay({
            element: popup,
            positioning: 'bottom-center',
            stopEvent: false,
            offset: [0, -10]
        });
        map.addOverlay(overlay);

        // 🔵 İlçe katmanı
        const ilceLayer = new ol.layer.Vector({
            source: new ol.source.Vector(),
            style: new ol.style.Style({
                stroke: new ol.style.Stroke({ color: 'blue', width: 1 }),
                fill: new ol.style.Fill({ color: 'rgba(0, 0, 255, 0.1)' })
            }),
            visible: false
        });
        ilceLayer.setZIndex(10);
        map.addLayer(ilceLayer);

        // 🔴 Marker katmanı
        let markerLayer = new ol.layer.Vector({
            source: new ol.source.Vector(),
            style: new ol.style.Style({
                image: new ol.style.Circle({
                    radius: 6,
                    fill: new ol.style.Fill({ color: 'red' }),
                    stroke: new ol.style.Stroke({ color: '#fff', width: 2 })
                })
            })
        });
        markerLayer.setZIndex(15);
        map.addLayer(markerLayer);

        // 🟢 İl katmanı
        fetch('/data/TR_iller.geojson')
            .then(res => res.json())
            .then(data => {
                const format = new ol.format.GeoJSON();
                const features = format.readFeatures(data, {
                    featureProjection: map.getView().getProjection()
                });

                const provinceLayer = new ol.layer.Vector({
                    source: new ol.source.Vector({ features }),
                    style: new ol.style.Style({
                        stroke: new ol.style.Stroke({ color: 'green', width: 2 }),
                        fill: new ol.style.Fill({ color: 'rgba(0, 255, 0, 0.1)' })
                    })
                });
                provinceLayer.setZIndex(5);
                map.addLayer(provinceLayer);

                // 🟩 İl tıklanınca: ilçe + marker filtrele
                map.on('singleclick', function (evt) {
                    map.forEachFeatureAtPixel(evt.pixel, function (feature, layer) {
                        const ilAdi = feature.get('Il_Adi');
                        if (ilAdi && layer === provinceLayer) {
                            const extent = feature.getGeometry().getExtent();
                            map.getView().fit(extent, { duration: 1000 });

                            // İlçeleri getir
                            fetch('/data/ilceler.geojson')
                                .then(res => res.json())
                                .then(ilceData => {
                                    const ilceFeatures = format.readFeatures(ilceData, {
                                        featureProjection: map.getView().getProjection()
                                    });

                                    const filtered = ilceFeatures.filter(f => f.get('Il_Adi') === ilAdi);
                                    ilceLayer.getSource().clear();
                                    ilceLayer.getSource().addFeatures(filtered);
                                    ilceLayer.setVisible(true);
                                });

                            // Marker'ları filtrele
                            fetch('/ObservationView/GetObservationsByProvince?province=' + encodeURIComponent(ilAdi))
                                .then(res => res.json())
                                .then(data => {
                                    const features = data.map(obs => {
                                        const feature = new ol.Feature({
                                            geometry: new ol.geom.Point(ol.proj.fromLonLat([parseFloat(obs.long), parseFloat(obs.lat)]))
                                        });
                                        feature.set('name', obs.name);
                                        feature.set('id', obs.id);
                                        return feature;
                                    });

                                    markerLayer.getSource().clear();
                                    markerLayer.getSource().addFeatures(features);
                                });
                        }
                    });
                });

                // 🧭 Hover: marker / il / ilçe adı
                map.on('pointermove', function (evt) {
                    let found = false;
                    map.forEachFeatureAtPixel(evt.pixel, function (feature) {
                        const props = feature.getProperties();

                        if (props['name']) {
                            popupContent.innerHTML = props['name'];
                            overlay.setPosition(feature.getGeometry().getCoordinates());
                            popup.style.display = 'block';
                            found = true;
                            return true;
                        }

                        if (props['Ilce_Adi']) {
                            popupContent.innerHTML = props['Ilce_Adi'];
                            overlay.setPosition(evt.coordinate);
                            popup.style.display = 'block';
                            found = true;
                            return true;
                        }

                        if (props['Il_Adi']) {
                            popupContent.innerHTML = props['Il_Adi'];
                            overlay.setPosition(evt.coordinate);
                            popup.style.display = 'block';
                            found = true;
                            return true;
                        }
                    });
                    if (!found) popup.style.display = 'none';
                });

                // Marker tıklama → detay
                map.on('click', function (evt) {
                    const feature = map.forEachFeatureAtPixel(evt.pixel, f => f);
                    if (!feature) return;

                    const props = feature.getProperties();
                    if (props['id']) {
                        window.location.href = '/ObservationView/Details/' + props['id'];
                        return;
                    }
                });
            });
    }
});
