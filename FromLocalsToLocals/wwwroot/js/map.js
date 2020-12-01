var vendorIcon = L.Icon.extend({options: {iconSize: [30, 30],iconAnchor: [15, 30],popupAnchor: [-3, -76]}});
var userMapLoc = new vendorIcon({ iconUrl: '/../Assets/userLoc.png' });

var userLocation;
var distanceCirle = L.featureGroup();
var useDistanceFilter = document.getElementById('useDistanceFilter');
var locError = document.getElementById('locError');
var distanceSlider = document.getElementById('distanceSlider');

$(document).ready(function () {
    addMarkers();
});

var markers = L.markerClusterGroup({
    spiderfyShapePositions: function (count, centerPt) {
        var distanceFromCenter = 35,
            markerDistance = 45,
            lineLength = markerDistance * (count - 1),
            lineStart = centerPt.y - lineLength / 2,
            res = [],
            i;

        res.length = count;

        for (i = count - 1; i >= 0; i--) {
            res[i] = new Point(centerPt.x + distanceFromCenter, lineStart + markerDistance * i);
        }

        return res;
    },

    showCoverageOnHover: false,
    zoomToBoundsOnClick: true,
    removeOutsideVisibleBounds: true
});

var baseMaps = {
    "<span style='color: gray'>Grayscale</span>": grayscale,
    "Streets": streets
};

L.control.layers(baseMaps).addTo(map);

//Popup stuff
var info = L.control({ position: 'bottomleft'});

info.onAdd = function (map) {
    this._div = L.DomUtil.create('div', 'info'); // create a div with a class "info"
    this.update();
    return this._div;
};

// method that we will use to update the control based on feature properties passed
info.update = function (props) {
    this._div.innerHTML = props ? (props.picture ? `<div ><button type = 'button' id='mygtukas' class='boxclose' onClick='closeInfoTab()'></button><img src='data:image;base64,${props.picture}' height='100px' width='100%'/><div><hr>
            <h4>${props.title}</h4><br><a href='/Vendors/Details/${props.id}'>Read More</a>` : `<div ><button type = 'button' id='mygtukas' class='boxclose' onClick='closeInfoTab()'></button><img src='/../Assets/appLogo.png' width='100%'/><div><hr>
                     <h4>${props.title}</h4><br><a href='/Vendors/Details/${props.id}'>Read More</a>`) : '<span style="color:#916814; margin-bottom: 40px;">Select vendor</span>';

};

info.addTo(map);

function closeInfoTab() {
    info.update();
}

//Filters
function vendorTypeOnChange(e) {
    if (e.checked) {
        vendorTypes.push(e.id);
    } else {
        vendorTypes = vendorTypes.filter(x => x != e.id);
    }
    map.removeLayer(markers);
    addMarkers();
}


//Get live location
function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showPosition,showError);
    } else {
        locError.innerHTML = "Geolocation is not supported by this browser.";
        locError.style.display = "block";
        distanceSlider.style.display = "none";
    }
}

function showPosition(position) {
    userLocation = L.latLng(position.coords.latitude, position.coords.longitude);
    var userMarker = L.marker(userLocation, {
        icon: userMapLoc
    });
    userMarker.addTo(map);
    locError.style.display = "none";
    distanceSlider.style.display = "block";
    drawCircle(distanceSlider.value);
}

function showError(error) {
    switch (error.code) {
        case error.PERMISSION_DENIED:
            locError.innerHTML = "User denied the request for Geolocation."
            break;
        case error.POSITION_UNAVAILABLE:
            locError.innerHTML = "Location information is unavailable."
            break;
        case error.TIMEOUT:
            locError.innerHTML = "The request to get user location timed out."
            break;
        case error.UNKNOWN_ERROR:
            locError.innerHTML = "An unknown error occurred."
            break;
    }
    locError.style.display = "block";
    distanceSlider.style.display = "none";
}

function useDistanceFilterChange(e) {
    if (e.checked) {
        getLocation();
    } else {
        map.removeLayer(distanceCirle);
        distanceCirle.clearLayers();
        locError.style.display = "none";
        distanceSlider.style.display = "none";
        map.removeLayer(markers);
        addMarkers();

    }
}

function drawCircle(e) {
    map.removeLayer(distanceCirle);
    distanceCirle.clearLayers();
    map.removeLayer(markers);
    addMarkers();

    distanceCirle.addLayer(L.circle(userLocation, {
        color: 'black',
        fillColor: '#F0F8FF',
        fillOpacity: 0.4,
        radius: (1000 * e)
    })).addTo(map);

}

//Updating markers
function addMarkers() {
    markers.clearLayers();

    for (var i = 0; i < allVendors.length; i++) {

        if (vendorTypes.includes(allVendors[i].VendorTypeDb) && (!useDistanceFilter.checked
            || (useDistanceFilter.checked && userLocation.distanceTo(L.latLng(allVendors[i].Latitude, allVendors[i].Longitude)) <= (distanceSlider.value*1000) ))) {
            var newMarker = L.marker([allVendors[i].Latitude, allVendors[i].Longitude], {
                title: allVendors[i].Title,
                id: allVendors[i].ID,
                icon: new vendorIcon({ iconUrl: `/../Assets/${allVendors[i].VendorTypeDb}.png` }),
                picture: allVendors[i].Image 
            }).on('click', function onClick(e) {
                info.update(e.target.options);
            });

            markers.addLayer(newMarker);
        }
    };
    markers.addTo(map);
}


