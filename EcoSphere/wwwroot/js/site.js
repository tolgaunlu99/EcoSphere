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

        // Gözlem verilerini al ve haritaya ekle
        fetch('/ObservationView/GetObservationsAsJson')
            .then(response => response.json())
            .then(data => {
                const features = data.map(obs => {
                    return new ol.Feature({
                        geometry: new ol.geom.Point(
                            ol.proj.fromLonLat([
                                parseFloat(obs.long),
                                parseFloat(obs.lat)
                            ])
                        ),
                        name: obs.name
                    });
                });

                const vectorSource = new ol.source.Vector({
                    features: features
                });

                const vectorLayer = new ol.layer.Vector({
                    source: vectorSource,
                    style: new ol.style.Style({
                        image: new ol.style.Circle({
                            radius: 6,
                            fill: new ol.style.Fill({ color: 'red' }),
                            stroke: new ol.style.Stroke({ color: 'white', width: 2 })
                        })
                    })
                });

                map.addLayer(vectorLayer);

                // Popup HTML öğesi
                const popup = document.getElementById('popup');
                const popupContent = document.getElementById('popup-content');

                const overlay = new ol.Overlay({
                    element: popup,
                    positioning: 'bottom-center',
                    stopEvent: false,
                    offset: [0, -10]
                });
                map.addOverlay(overlay);

                map.on('pointermove', function (evt) {
                    const feature = map.forEachFeatureAtPixel(evt.pixel, function (f) {
                        return f;
                    });

                    if (feature) {
                        const coord = feature.getGeometry().getCoordinates();
                        popupContent.innerHTML = feature.get('name');
                        overlay.setPosition(coord);
                        popup.style.display = 'block';
                    } else {
                        popup.style.display = 'none';
                    }
                });
            })
            .catch(error => {
                console.error('Gözlem verileri alınamadı:', error);
            });
    }
});
