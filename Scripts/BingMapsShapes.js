var map = null;
var polyPoints = new Array();   //Arreglo que guarda los puntos del poligono
var down = 0;
var X = 0, Y = 0;
var origin;
var options;
var shapeCoordinatesJSON = ""; //Variable global que almacena el JSON de las coordenadas


function GetMap() {

    map = new Microsoft.Maps.Map(document.getElementById("map_shapes"), {
        credentials: "ApOTYKFhJOwRBzZjezJNQfmXOYGZipN16UiNVMAqterclvoLldC89u7l9AsHrHOP"
        //credentials: "Ar68-tKrKTTwzHDleJ9cdkPbn7VaTWWNQZXJmcwAjU_s33ARtdghuEnrvSh3QzFY"
    });

    map.setView({ center: new Microsoft.Maps.Location(-30, -70), zoom: 6 });

    //Creamos un layer que solo guarde los poligonos
    var polygonLayer = new Microsoft.Maps.EntityCollection();
    map.entities.push(polygonLayer);

    //Cursos en estilo de cruz
    map.getRootElement().style.cursor = 'crosshair';

    //Eliminamos el menú que aparece al apretar con el boton derecho
    document.oncontextmenu = function (e) {
        return false;
    };

    //Opciones del color del borde y el relleno del pólígono que se dibuja 
    options = { fillColor: new Microsoft.Maps.Color(140, 13, 76, 167), strokeColor: new Microsoft.Maps.Color(140, 2, 46, 108) };
    //Eventos del mouse. Cada uno direcciona a un metodo que se ocupa de ese evento
    Microsoft.Maps.Events.addHandler(map, 'mouseup', MouseUpHandler);
    Microsoft.Maps.Events.addHandler(map, 'mousemove', MouseMoveHandler);
    Microsoft.Maps.Events.addHandler(map, 'mouseover', MouseOverHandler);
    Microsoft.Maps.Events.addHandler(map, 'rightclick', RightClickHandler);
}

//Método que llama la accion GetCoordinatesFromShapeFile del controlador ShapeController para recuperar 
//un JSON que contiene las coordenadas del poligono almacenado en el shapefile que se 
//subio anteriormente con la accion SaveUploadedFile del mismo controlador
function displayPolygon() {

    //Llamada asincrona de los datos de los archivo temporal en Content/Shapefiles
    jQuery.extend({
        getValues: function (url) {
            var result = null;
            $.ajax({
                url: url,
                type: 'get',
                dataType: 'json',
                async: false,
                success: function (data) {
                    result = data;
                }
            });
            return result;
        }
    });

    shapeCoordinatesJSON = $.getValues(window.location.origin + "/Cuenca/GetCoordinatesFromShapeFile");
    //Pasamos las coordenadas al formulario de la vista
    document.getElementById("Coordinates").setAttribute("value", JSON.stringify(shapeCoordinatesJSON));
    //Arreglo que va almacenando las coordenadas
    polyPoints = new Array(0);

    var i = 0;
    //Si el JSON contiene la palabra error, entonces el archivo no se cargo adecuadamente
    if (shapeCoordinatesJSON[0] == "Error") {
        alert("Error en el archivo, asegurese de que esta subiendo los archivos .shp, .dbf y .shx");
        window.location.reload();
    }
        //El archivo se cargo adecuadamente
    else {
        //...hasta vaciar el JSON...
        while (shapeCoordinatesJSON[i] != null) {
            var split = shapeCoordinatesJSON[i].split(";");
            var point = new Microsoft.Maps.Location(parseFloat(split[1].replace(",", ".")), parseFloat(split[0].replace(",", ".")));
            polyPoints.push(point);
            i++;
        }

        //Dibujamos el poligono y despues buscamos algun pin que se encuentre dentro de él
        drawPolygon();
        //polygonSearch();
        //Zoom al poligono
        map.setView({ center: new Microsoft.Maps.Location(polyPoints[0].latitude, polyPoints[0].longitude), zoom: 5 });

    }
}

//Recibe los puntos desde la vista y despliega el poligono
function setPolypointsFromView(coordinates) {
    polyPoints = new Array(0);

    var i = 0;
    while (coordinates[i] != null) {
        var split = coordinates[i].split(";");
        var point = new Microsoft.Maps.Location(parseFloat(split[1].replace(",", ".")), parseFloat(split[0].replace(",", ".")));
        polyPoints.push(point);
        i++;
    }
    drawPolygon();

    map.setView({ center: new Microsoft.Maps.Location(polyPoints[0].latitude, polyPoints[0].longitude), zoom: 5 });
}

//Agarra los puntos que estén dentro de la variable polypoints y los dibuja
function drawPolygon() {
    map.getRootElement().style.cursor = 'crosshair';
    //remove the polygon that is on the map
    var polygonLayer = map.entities.pop();
    polygonLayer.clear();

    //draw polyline if there is only two points in the array
    if (polyPoints.length < 3) {
        var polygon = new Microsoft.Maps.Polyline(polyPoints, options);
    }
    else {
        //create a new polygon
        var polygon = new Microsoft.Maps.Polygon(polyPoints, options);
    }

    //draw the new polygon on the map;
    polygonLayer.push(polygon);
    map.entities.push(polygonLayer);
}

//Busca algun shape que esté dentro del poligono
function polygonSearch() {

    var polygonLayer = map.entities.pop();
    var pinLayer = map.entities.pop();
    var pinsFounded = new Array();

    //loop through shapes on the shape layer and see if they are within the polygon
    for (var i = 0; i < pinLayer.getLength() ; i++) {
        var shape = pinLayer.get(i);
        var latlong = shape.getLocation();
        var lat = latlong.latitude;
        var lon = latlong.longitude;
        if (pointInPolygon(polyPoints, lat, lon)) {
            pinsFounded.push(shape);
        }
    }

    var x = pinsFounded.length;
    if (pinsFounded.length != 0) {
        var message = "";
        if (pinsFounded.length == 1) {
            message += "Se encontró el punto: " + pinsFounded[0].getLocation().latitude + "," + pinsFounded[0].getLocation().longitude;
            pinsFounded.pop();
        }
        else {
            message += "Se encontraron los puntos: ";
            while (pinsFounded.length != 0) {
                var punto = pinsFounded.pop().getLocation();
                message += "\n" + punto.latitude + "," + punto.longitude;
            }
        }
        alert(message);
    }
    else {
        alert("No se encontró ningún pin dentro del polígono");
    }

    map.entities.push(pinLayer);
    map.entities.push(polygonLayer);
}

function pointInPolygon(points, lat, lon) {
    var i;
    var j = points.length - 1;
    var inPoly = false;

    for (i = 0; i < points.length; i++) {
        if (points[i].longitude < lon && points[j].longitude >= lon || points[j].longitude < lon && points[i].longitude >= lon) {
            if (points[i].latitude + (lon - points[i].longitude) / (points[j].longitude - points[i].longitude) * (points[j].latitude - points[i].latitude) < lat) {
                inPoly = !inPoly;
            }
        }
        j = i;
    }

    return inPoly;
}

function MouseUpHandler(e) {
    map.getRootElement().style.cursor = 'crosshair';
    //...si se trata del boton izquierdo
    if (e.isPrimary) {
        if (down == 1) {
            down = 0;
            polyPoints.push(origin);
            //Se busca lo que hay dentro del poligonos
            //polygonSearch();
            X = 0;
            Y = 0;
            var polygonLayer = map.entities.pop();
            polygonLayer.clear();

            //draw polyline if there is only two points in the array
            if (polyPoints.length < 3) {
                var polygon = new Microsoft.Maps.Polyline(polyPoints, options);
            }
            else {
                //create a new polygon
                var polygon = new Microsoft.Maps.Polygon(polyPoints, options);
            }

            //draw the new polygon on the map;
            polygonLayer.push(polygon);
            map.entities.push(polygonLayer);

            document.getElementById("Coordinates").setAttribute("value", JSON.stringify(PolypointsToJSON()));
        }
    }
}

//Si apreta con el boton derecho debemos dibujar...
function RightClickHandler(e) {
    map.getRootElement().style.cursor = 'crosshair';
    var X = e.getX();
    var Y = e.getY();
    var pixel = new Microsoft.Maps.Point(X, Y);
    var point = map.tryPixelToLocation(pixel);

    if (down == 0) {
        //intialize the map and variables.
        var polygonLayer = map.entities.pop();
        polygonLayer.clear();
        map.entities.push(polygonLayer);

        polyPoints = new Array(0);

        origin = point;
        polyPoints.push(origin);
        down = 1;
    }

    //add the new point to the array
    polyPoints.push(point);

    return true;
}

function MouseMoveHandler(e) {
    map.getRootElement().style.cursor = 'crosshair';
    if (down == 1) {
        var x = e.getX();
        var y = e.getY();

        var dx = Math.abs(X) - Math.abs(x);
        var dy = Math.abs(Y) - Math.abs(y);

        //do not want to redraw polygon unless we move a significant amount
        if (Math.abs(dx) > 5 || Math.abs(dy) > 5) {
            var latlong = map.tryPixelToLocation(new Microsoft.Maps.Point(x, y));
            polyPoints.pop();
            polyPoints.push(latlong);

            drawPolygon();
        }
    }
}

function MouseOverHandler(e) {
    map.getRootElement().style.cursor = 'crosshair';
    if (down == 1) {
        //disable popups when drawing polygon to make drawing the polygon easier
        return true;
    }
}

function PolypointsToJSON() {
    var json = {};

    for (var i = 0; i < polyPoints.length; i++) {
        json[i] = polyPoints[i].longitude + ";" + polyPoints[i].latitude;
    }
    return json;
}