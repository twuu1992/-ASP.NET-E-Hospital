
const TOKEN = "pk.eyJ1IjoidHd1dTAwMTIiLCJhIjoiY2psM2R1NmxmMHR0NDNrbjNqejgzbTQwcCJ9.lDLXUSOFO5TNY53MBsf60w";
var locations = [];
// The first step is obtain all the latitude and longitude from the HTML
// The below is a simple jQuery selector
$(".coordinates").each(function () {
    var longitude = $(".longitude", this).text().trim();
    var latitude = $(".latitude", this).text().trim();
    var description = $(".description", this).text().trim();
    var title = $(".title", this).text().trim();
    // Create a point data structure to hold the values.
    var point = {
        "latitude": latitude,
        "longitude": longitude,
        "description": description,
        "title": title
    };
    // Push them all into an array.
    locations.push(point);
});
var data = [];
for (i = 0; i < locations.length; i++) {
    var feature = {
        "type": "Feature",
        "properties": {
            "title": locations[i].title,
            "description": locations[i].description,
            "icon": "circle-15"
        },
        "geometry": {
            "type": "Point",
            "coordinates": [locations[i].longitude, locations[i].latitude]
        }
    };
    data.push(feature);
}
mapboxgl.accessToken = TOKEN;
var map = new mapboxgl.Map({
    container: 'map',
    style: 'mapbox://styles/twuu0012/cjlxtorel4sdu2rp4wrmh3rsh',
    zoom: 11,
    center: [144.963058, -37.813629]
});
map.on('load', function () {
    map.loadImage('https://upload.wikimedia.org/wikipedia/commons/a/ad/Map_marker_icon_%E2%80%93_Nicolas_Mollet_%E2%80%93_Hospital_%E2%80%93_Health_%26_Education_%E2%80%93_Classic.png',
        function(error, image) {
            if (error) throw error;
            map.addImage('marker_icon', image);
            map.addLayer({
                "id": "places",
                "type": "symbol",
                "source": {
                    "type": "geojson",
                    "data": {
                        "type": "FeatureCollection",
                        "features": data
                    }
                },
                "layout": {
                    "icon-image": "marker_icon",
                    "icon-allow-overlap": true
                }
            });
            map.addControl(new MapboxGeocoder({ accessToken: mapboxgl.accessToken }));
            map.addControl(new mapboxgl.NavigationControl());
        });
    // Add a layer showing the places.
    
    // When a click event occurs on a feature in the places layer, open a popup at the
    // location of the feature, with description HTML from its properties.
    map.on('click', 'places', function (e) {
        var coordinates = e.features[0].geometry.coordinates.slice();
        var description = e.features[0].properties.description;
        // Ensure that if the map is zoomed out such that multiple
        // copies of the feature are visible, the popup appears
        // over the copy being pointed to.
        while (Math.abs(e.lngLat.lng - coordinates[0]) > 180) {
            coordinates[0] += e.lngLat.lng > coordinates[0] ? 360 : -360;
        }
        new mapboxgl.Popup()
            .setLngLat(coordinates)
            .setHTML(description)
            .addTo(map);
    });
    // Change the cursor to a pointer when the mouse is over the places layer.
    map.on('mouseenter', 'places', function () {
        map.getCanvas().style.cursor = 'pointer';
    });
    // Change it back to a pointer when it leaves.
    map.on('mouseleave', 'places', function () {
        map.getCanvas().style.cursor = '';
    });
});
