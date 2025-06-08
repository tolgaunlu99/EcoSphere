
let coordinateMap, coordMarkerLayer, provinceFeatures = [], districtFeatures = [], countryBorderGeometry = null;

document.addEventListener("DOMContentLoaded", function () {
    if (!document.getElementById("coordinateMap")) return;

    const format = new ol.format.GeoJSON();

    const osmLayer = new ol.layer.Tile({ source: new ol.source.OSM() });
    const satelliteLayer = new ol.layer.Tile({
        source: new ol.source.XYZ({
            url: 'https://mt1.google.com/vt/lyrs=s&x={x}&y={y}&z={z}',
            attributions: '© Google'
        })
    });

    coordinateMap = new ol.Map({
        target: 'coordinateMap',
        layers: [osmLayer],
        view: new ol.View({ center: ol.proj.fromLonLat([35.2433, 38.9637]), zoom: 6 })
    });

    coordMarkerLayer = new ol.layer.Vector({ source: new ol.source.Vector() });
    coordinateMap.addLayer(coordMarkerLayer);

    // Toggle uydu görünümü butonu
    const layerToggleBtn = document.createElement("button");
    layerToggleBtn.innerHTML = `<i class="fas fa-globe" style="color: #2E590E;"></i> Uydu Görünümü`;
    layerToggleBtn.className = "satellite-toggle";
    layerToggleBtn.style.position = "absolute";
    layerToggleBtn.style.top = "10px";
    layerToggleBtn.style.right = "10px";
    layerToggleBtn.style.zIndex = "1";
    layerToggleBtn.style.backgroundColor = "white";
    layerToggleBtn.style.boxShadow = "0px 2px 5px rgba(0,0,0,0.3)";
    layerToggleBtn.style.fontSize = "0.9rem";
    layerToggleBtn.style.border = "none";
    layerToggleBtn.style.padding = "6px 10px";
    layerToggleBtn.style.borderRadius = "6px";
    document.getElementById("coordinateMap").appendChild(layerToggleBtn);

    let isSatellite = false;
    layerToggleBtn.addEventListener("click", function () {
        if (isSatellite) {
            coordinateMap.removeLayer(satelliteLayer);
            coordinateMap.addLayer(osmLayer);
            layerToggleBtn.innerHTML = `<i class="fas fa-globe"></i> Uydu Görünümü`;
        } else {
            coordinateMap.removeLayer(osmLayer);
            coordinateMap.addLayer(satelliteLayer);
            layerToggleBtn.innerHTML = `<i class="fas fa-map"></i> Harita Görünümü`;
        }
        isSatellite = !isSatellite;
    });

    // Türkiye sınırı yükle
    fetch("/data/TR_ulke_siniri.geojson")
        .then(res => res.json())
        .then(countryData => {
            const features = format.readFeatures(countryData, {
                featureProjection: coordinateMap.getView().getProjection()
            });

            const geometries = features.map(f => f.getGeometry());
            if (geometries.length > 1) {
                countryBorderGeometry = new ol.geom.MultiPolygon(
                    geometries.map(g => g.getCoordinates())
                );
            } else {
                countryBorderGeometry = geometries[0];
            }
        });

    // İl ve ilçe sınırları yükle
    Promise.all([
        fetch("/data/TR_iller.geojson").then(res => res.json()),
        fetch("/data/ilceler.geojson").then(res => res.json())
    ]).then(([provinceData, districtData]) => {
        provinceFeatures = format.readFeatures(provinceData, {
            featureProjection: coordinateMap.getView().getProjection()
        });
        districtFeatures = format.readFeatures(districtData, {
            featureProjection: coordinateMap.getView().getProjection()
        });
    });

    coordinateMap.on("click", function (evt) {
        // Türkiye sınırı kontrolü
        if (!countryBorderGeometry) {
            alert("Türkiye sınırları henüz yüklenmedi.");
            return;
        }

        if (!countryBorderGeometry.intersectsCoordinate(evt.coordinate)) {
            alert("Lütfen Türkiye sınırları içinde bir konum seçin.");
            return;
        }

        const coord = ol.proj.toLonLat(evt.coordinate);
        const lon = coord[0].toFixed(6);
        const lat = coord[1].toFixed(6);

        coordMarkerLayer.getSource().clear();
        const point = new ol.Feature(new ol.geom.Point(evt.coordinate));
        coordMarkerLayer.getSource().addFeature(point);

        let foundProvince = "", foundDistrict = "";

        for (let p of provinceFeatures) {
            if (p.getGeometry().intersectsCoordinate(evt.coordinate)) {
                foundProvince = p.get("Il_Adi") || p.get("name");
                break;
            }
        }

        for (let d of districtFeatures) {
            if (d.getGeometry().intersectsCoordinate(evt.coordinate)) {
                foundDistrict = d.get("Ilce_Adi") || d.get("name");
                break;
            }
        }

        // Update hidden inputs
        document.getElementById("LongInput").value = lon;
        document.getElementById("LatInput").value = lat;
        document.getElementById("HiddenProvinceName").value = foundProvince;
        document.getElementById("HiddenDistrictName").value = foundDistrict;

        // Update display
        document.getElementById("displayCoords").textContent = `${lat} / ${lon}`;
        document.getElementById("displayLocation").textContent = `${foundProvince || "-"} / ${foundDistrict || "-"}`;
    });
});

// Gelişmiş Görsel Önizleme Fonksiyonları
function previewImage(event) {
    const input = event.target;
    const preview = document.getElementById("imagePreview");
    const wrapper = document.getElementById("imageWrapper");
    const label = document.getElementById("fileLabel");

    if (input.files && input.files[0]) {
        const file = input.files[0];

        // Dosya boyutu kontrolü (10MB)
        if (file.size > 10 * 1024 * 1024) {
            alert("Dosya boyutu 10MB'dan küçük olmalıdır!");
            input.value = "";
            return;
        }

        // Dosya türü kontrolü
        if (!file.type.match(/^image\/(jpeg|jpg|png|gif)$/)) {
            alert("Lütfen geçerli bir görsel dosyası seçin (JPEG, JPG, PNG, GIF)!");
            input.value = "";
            return;
        }

        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;
            wrapper.style.display = "block";
            label.style.display = "none";
        }

        reader.readAsDataURL(file);
    }
}

function removeImage() {
    const input = document.getElementById("ImageFile");
    const preview = document.getElementById("imagePreview");
    const wrapper = document.getElementById("imageWrapper");
    const label = document.getElementById("fileLabel");

    // Dosya inputunu temizle
    input.value = "";

    // Önizlemeyi gizle
    preview.src = "#";
    wrapper.style.display = "none";
    label.style.display = "block";
}

function openImageModal() {
    const preview = document.getElementById("imagePreview");
    const modal = document.getElementById("imageModal");
    const modalImage = document.getElementById("modalImage");

    if (preview.src && preview.src !== window.location.href + "#") {
        modalImage.src = preview.src;
        modal.style.display = "block";

        // Body scroll'unu engelle
        document.body.style.overflow = "hidden";
    }
}

function closeImageModal() {
    const modal = document.getElementById("imageModal");
    modal.style.display = "none";

    // Body scroll'unu geri aç
    document.body.style.overflow = "auto";
}

function removeImageFromModal() {
    removeImage();
    closeImageModal();
}

// Modal dışına tıklayınca kapanması için
document.getElementById("imageModal").addEventListener("click", function (event) {
    if (event.target === this) {
        closeImageModal();
    }
});

// ESC tuşu ile modal kapanması
document.addEventListener("keydown", function (event) {
    if (event.key === "Escape") {
        closeImageModal();
    }
});

// Dosya sürükle-bırak özelliği
const fileLabel = document.getElementById("fileLabel");
const fileInput = document.getElementById("ImageFile");

if (fileLabel && fileInput) {
    fileLabel.addEventListener("dragover", function (e) {
        e.preventDefault();
        this.style.borderColor = "#764ba2";
        this.style.background = "#f8f9ff";
    });

    fileLabel.addEventListener("dragleave", function (e) {
        e.preventDefault();
        this.style.borderColor = "#667eea";
        this.style.background = "white";
    });

    fileLabel.addEventListener("drop", function (e) {
        e.preventDefault();
        this.style.borderColor = "#667eea";
        this.style.background = "white";

        const files = e.dataTransfer.files;
        if (files.length > 0) {
            fileInput.files = files;
            previewImage({ target: fileInput });
        }
    });
}

$(document).ready(function () {
    // Tom Select başlatma - CDN kullanıldığında import'ları kaldırın
    if (typeof TomSelect !== 'undefined') {

        // Creature Select - Class selector kullanarak
        const creatureElement = document.querySelector('.tom-select-creature');
        if (creatureElement) {
            new TomSelect(creatureElement, {
                placeholder: 'Tür Seçiniz',
                allowEmptyOption: true,
                create: false,
                maxItems: 1,
                searchField: ['text'],
                sortField: [
                    { field: 'text', direction: 'asc' }
                ],
                plugins: ['clear_button'],
                dropdownParent: 'body',
                render: {
                    no_results: function (data, escape) {
                        return '<div class="no-results">Sonuç bulunamadı</div>';
                    }
                },
                onInitialize: function () {
                    this.control.setAttribute('data-placeholder', 'Tür Seçiniz');
                },
                onFocus: function () {
                    this.wrapper.classList.add('focus');
                },
                onBlur: function () {
                    this.wrapper.classList.remove('focus');
                },
                onItemAdd: function (value, item) {
                    console.log('Creature selected:', value);
                }
            });
        }

        // Project Select - Class selector kullanarak
        const projectElement = document.querySelector('.tom-select-project');
        if (projectElement) {
            new TomSelect(projectElement, {
                placeholder: 'Proje Seçiniz',
                allowEmptyOption: true,
                create: false,
                maxItems: 1,
                searchField: ['text'],
                sortField: [
                    { field: 'text', direction: 'asc' }
                ],
                plugins: ['clear_button'],
                dropdownParent: 'body',
                render: {
                    no_results: function (data, escape) {
                        return '<div class="no-results">Sonuç bulunamadı</div>';
                    }
                },
                onInitialize: function () {
                    this.control.setAttribute('data-placeholder', 'Proje Seçiniz');
                },
                onFocus: function () {
                    this.wrapper.classList.add('focus');
                },
                onBlur: function () {
                    this.wrapper.classList.remove('focus');
                },
                onItemAdd: function (value, item) {
                    console.log('Project selected:', value);
                }
            });
        }

        // Reference Select - Class selector kullanarak
        const referenceElement = document.querySelector('.tom-select-reference');
        if (referenceElement) {
            new TomSelect(referenceElement, {
                placeholder: 'Referans Seçiniz',
                allowEmptyOption: true,
                create: false,
                maxItems: 1,
                searchField: ['text'],
                sortField: [
                    { field: 'text', direction: 'asc' }
                ],
                plugins: ['clear_button'],
                dropdownParent: 'body',
                render: {
                    no_results: function (data, escape) {
                        return '<div class="no-results">Sonuç bulunamadı</div>';
                    }
                },
                onInitialize: function () {
                    this.control.setAttribute('data-placeholder', 'Referans Seçiniz');
                },
                onFocus: function () {
                    this.wrapper.classList.add('focus');
                },
                onBlur: function () {
                    this.wrapper.classList.remove('focus');
                },
                onItemAdd: function (value, item) {
                    console.log('Reference selected:', value);
                }
            });
        }
    } else {
        console.warn('TomSelect is not loaded, falling back to regular select elements');
    }

    // Form submit işlemi
    $('form').on('submit', function (e) {
        console.log('Form submitted');
    });
});

// Başarı mesajı gösterme
function showSuccessMessage() {
    const successAlert = `
                <div class="alert alert-success alert-dismissible fade show" role="alert">
                    <i class="fas fa-check-circle me-2"></i>
                    Gözlem başarıyla kaydedildi!
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                </div>
            `;
    $('.form-section').append(successAlert);

    // 3 saniye sonra otomatik gizle
    setTimeout(function () {
        $('.alert').fadeOut();
    }, 3000);
}

// Proje ekleme modal fonksiyonları
function openAddProjectModal() {
    $('#addProjectModal').fadeIn(300);
    $('#newProjectName').focus();
}

function closeAddProjectModal() {
    $('#addProjectModal').fadeOut(300);
    $('#newProjectName, #newProjectDescription').val('');
}

function saveNewProject() {
    const projectName = $('#newProjectName').val().trim();
    const projectDescription = $('#newProjectDescription').val().trim();

    if (projectName) {
        // AJAX ile controller'a yeni proje ekle
        $.ajax({
            url: '/Observation/AddProject',
            type: 'POST',
            data: {
                name: projectName,
                description: projectDescription
            },
            success: function (response) {
                if (response.success) {
                    // Tom Select kullanıyorsak
                    const projectElement = document.querySelector('[asp-for="ProjectId"]');
                    if (projectElement && projectElement.tomselect) {
                        projectElement.tomselect.addOption({ value: response.id, text: response.name });
                        projectElement.tomselect.setValue(response.id);
                    } else {
                        // Fallback için normal select
                        const newOption = new Option(response.name, response.id, true, true);
                        $('[asp-for="ProjectId"]').append(newOption).trigger('change');
                    }

                    closeAddProjectModal();
                    showTempMessage('Yeni proje başarıyla eklendi!', 'success');
                } else {
                    showTempMessage('Proje eklenirken hata oluştu!', 'danger');
                }
            },
            error: function () {
                showTempMessage('Proje eklenirken hata oluştu!', 'danger');
            }
        });
    } else {
        showTempMessage('Proje adı boş olamaz!', 'danger');
    }
}

// Referans ekleme modal fonksiyonları
function openAddReferenceModal() {
    $('#addReferenceModal').fadeIn(300);
    $('#newReferenceName').focus();
}

function closeAddReferenceModal() {
    $('#addReferenceModal').fadeOut(300);
    $('#newReferenceName, #newReferenceAuthor').val('');
}

function saveNewReference() {
    const referenceName = $('#newReferenceName').val().trim();
    const referenceAuthor = $('#newReferenceAuthor').val().trim();

    if (referenceName) {
        // AJAX ile controller'a yeni referans ekle
        $.ajax({
            url: '/Observation/AddReference',
            type: 'POST',
            data: {
                name: referenceName,
                author: referenceAuthor
            },
            success: function (response) {
                if (response.success) {
                    // Tom Select kullanıyorsak
                    const referenceElement = document.querySelector('[asp-for="ReferenceId"]');
                    if (referenceElement && referenceElement.tomselect) {
                        referenceElement.tomselect.addOption({ value: response.id, text: response.name });
                        referenceElement.tomselect.setValue(response.id);
                    } else {
                        // Fallback için normal select
                        const newOption = new Option(response.name, response.id, true, true);
                        $('[asp-for="ReferenceId"]').append(newOption).trigger('change');
                    }

                    closeAddReferenceModal();
                    showTempMessage('Yeni referans başarıyla eklendi!', 'success');
                } else {
                    showTempMessage('Referans eklenirken hata oluştu!', 'danger');
                }
            },
            error: function () {
                showTempMessage('Referans eklenirken hata oluştu!', 'danger');
            }
        });
    } else {
        showTempMessage('Referans adı boş olamaz!', 'danger');
    }
}

// Geçici mesaj gösterme
function showTempMessage(message, type) {
    const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    const icon = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';

    const tempAlert = `
                <div class="alert ${alertClass} alert-dismissible fade show position-fixed"
                     style="top: 20px; right: 20px; z-index: 10000; min-width: 300px;" role="alert">
                    <i class="fas ${icon} me-2"></i>
                    ${message}
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                </div>
            `;
    $('body').append(tempAlert);

    // 3 saniye sonra otomatik gizle
    setTimeout(function () {
        $('.alert.position-fixed').fadeOut(300, function () {
            $(this).remove();
        });
    }, 3000);
}

// Modal dışına tıklandığında kapatma
$(document).on('click', '.popup-modal', function (e) {
    if (e.target === this) {
        $(this).fadeOut(300);
        // Input'ları temizle
        $(this).find('input').val('');
    }
});

// ESC tuşu ile modal kapatma
$(document).keydown(function (e) {
    if (e.key === 'Escape') {
        $('.popup-modal:visible').fadeOut(300);
        $('.popup-modal input').val('');
    }
});

// Yeni eklenen item'ı select'e eklemek için yardımcı fonksiyon
function addNewItemToTomSelect(selectSelector, value, text) {
    const selectElement = document.querySelector(selectSelector);
    if (selectElement && selectElement.tomselect) {
        // Yeni option ekle
        selectElement.tomselect.addOption({ value: value, text: text });
        // Yeni eklenen item'ı seç
        selectElement.tomselect.setValue(value);
    }
}

// Kullanım örneği:
// Modal'dan yeni proje eklendikten sonra:
// addNewItemToTomSelect('[asp-for="ProjectId"]', 'new-project-id', 'Yeni Proje Adı');