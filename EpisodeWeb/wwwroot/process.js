var decodedFiles = [];

function loadDecodedFiles() {
    var req = new XMLHttpRequest();
    req.open("GET", "./api/decodedfiles", true);
    req.setRequestHeader("Content-Type", "application/json");
    req.onload = function (e) {
        if (req.readyState === 4 && req.status === 200) {
            var response = JSON.parse(req.responseText);
            decodedFiles = response["files"];
            addFilesToDecodedTable();
        }
    };
    req.send(null);
}

function addFilesToDecodedTable() {
    var fileTable = document.getElementById("decodedTable");
    for (file of decodedFiles) {
        var newRow = fileTable.insertRow(fileTable.rows.length);
        var cbCell = newRow.insertCell(0);

        var cb = document.createElement("input");
        cb.setAttribute("type", "checkbox");
        cb.setAttribute("id", "dec-" + file["Index"]);
        cbCell.appendChild(cb);

        var label = document.createElement("label");
        label.setAttribute("for", "dec-" + file["Index"]);
        cbCell.appendChild(label);

        var showCell = newRow.insertCell(1);
        var showElement = document.createTextNode("<SHOW????>");
        showCell.appendChild(showElement);

        var epCell = newRow.insertCell(2);
        var epElement = document.createTextNode("<EPISODE????>");
        epCell.appendChild(epElement);

        var pathCell = newRow.insertCell(3);
        var pathElement = document.createTextNode(file["Path"]);
        pathCell.appendChild(pathElement);
    }
}