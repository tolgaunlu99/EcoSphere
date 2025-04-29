document.addEventListener("DOMContentLoaded", function () {
    if (typeof ol === "undefined") {
        console.error("OpenLayers yüklenemedi!");
        return;
    }

    if (document.getElementById('map')) {
        const map = new ol.Map({
            target: 'map',
            layers: [
                new ol.layer.Tile({
                    source: new ol.source.OSM()
                })
            ],
            view: new ol.View({
                center: ol.proj.fromLonLat([35.2433, 38.9637]),
                zoom: 6
            })
        });

        fetch('/ObservationView/GetObservationsAsJson')
            .then(response => response.json())
            .then(data => {
                const features = data.map(obs => {
                    const feature = new ol.Feature({
                        geometry: new ol.geom.Point(
                            ol.proj.fromLonLat([
                                parseFloat(obs.long),
                                parseFloat(obs.lat)
                            ])
                        ),
                        name: obs.name,
                        id: obs.id   // 🔥 Id burada feature'a eklendi!
                    });
                    return feature;
                });

                const vectorSource = new ol.source.Vector({
                    features: features
                });

                const clusterSource = new ol.source.Cluster({
                    distance: 40,
                    source: vectorSource
                });

                const styleCache = {};
                const clusters = new ol.layer.Vector({
                    source: clusterSource,
                    style: function (feature) {
                        const size = feature.get('features').length;
                        let style = styleCache[size];
                        if (!style) {
                            style = new ol.style.Style({
                                image: new ol.style.Circle({
                                    radius: size > 1 ? 12 : 6,
                                    fill: new ol.style.Fill({
                                        color: size > 1 ? 'rgba(255, 153, 0, 0.6)' : 'red'
                                    }),
                                    stroke: new ol.style.Stroke({
                                        color: '#fff',
                                        width: 2
                                    })
                                }),
                                text: size > 1 ? new ol.style.Text({
                                    text: size.toString(),
                                    fill: new ol.style.Fill({ color: '#fff' })
                                }) : undefined
                            });
                            styleCache[size] = style;
                        }
                        return style;
                    }
                });

                map.addLayer(clusters);

                const popup = document.getElementById('popup');
                const popupContent = document.getElementById('popup-content');

                const overlay = new ol.Overlay({
                    element: popup,
                    positioning: 'bottom-center',
                    stopEvent: false,
                    offset: [0, -10]
                });
                map.addOverlay(overlay);

                // Sadece üstüne gelince popup göster
                map.on('pointermove', function (evt) {
                    const feature = map.forEachFeatureAtPixel(evt.pixel, function (f) {
                        return f;
                    });

                    if (feature && feature.get('features')) {
                        const features = feature.get('features');
                        if (features.length === 1) {
                            const singleFeature = features[0];
                            const coord = singleFeature.getGeometry().getCoordinates();
                            popupContent.innerHTML = singleFeature.get('name');
                            overlay.setPosition(coord);
                            popup.style.display = 'block';
                        } else {
                            popup.style.display = 'none';
                        }
                    } else {
                        popup.style.display = 'none';
                    }
                });

                // Tıklayınca: Eğer cluster ise yakınlaştır, değilse detay sayfasına yönlendir
                map.on('click', function (evt) {
                    const feature = map.forEachFeatureAtPixel(evt.pixel, function (f) {
                        return f;
                    });

                    if (feature && feature.get('features')) {
                        const features = feature.get('features');
                        if (features.length > 1) {
                            // Birden fazla feature varsa zoom in yap
                            const extent = ol.extent.createEmpty();
                            features.forEach(function (f) {
                                ol.extent.extend(extent, f.getGeometry().getExtent());
                            });
                            map.getView().fit(extent, { duration: 500, maxZoom: 16 });
                        } else if (features.length === 1) {
                            const singleFeature = features[0];
                            const id = singleFeature.get('id'); // Buradan id alınıyor
                            if (id) {
                                window.location.href = '/ObservationView/Details/' + id;
                            }
                        }
                    }
                });

            })
            .catch(error => {
                console.error('Gözlem verileri alınamadı:', error);
            });
    }
});
