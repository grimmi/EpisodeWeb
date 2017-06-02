var loadedFiles = [];

function getAllCheckedIds() {
    var checkboxes = [];
    var allInputs = document.getElementsByTagName("input");
    for (var i = 0; i < allInputs.length; i++) {
        if (allInputs[i].id.startsWith("cb-") && allInputs[i].checked) {
            checkboxes.push(allInputs[i].id.replace("cb-", ""));
        }
    }
    return checkboxes;
}

function markAll() {
    var allInputs = document.getElementsByTagName("input");
    for (var i = 0; i < allInputs.length; i++) {
        if (allInputs[i].id.startsWith("cb-")) {
            allInputs[i].checked = true;
        }
    }
}

function getStatus() {
    var req = new XMLHttpRequest();
    req.open("GET", "./api/DecodeJob", true);
    req.setRequestHeader("Content-Type", "application/json");
    req.onload = function (e) {
        var status = JSON.parse(req.responseText);
        if (!status["done"]) {
            setTimeout(getStatus, 2000);
            if (status["progress"] !== undefined) {
                Materialize.toast(status["progress"] + "% done", 1800);
            }
        }
        else {
            Materialize.toast("all done!", 4000);
        }
    };
    req.send(null);
}

function sendDecode() {
    var ids = getAllCheckedIds();
    var checkedFiles = [];
    for (var i = 0; i < loadedFiles.length; i++) {
        var f = loadedFiles[i];
        if (ids.indexOf(f["Index"].toString()) > -1) {
            checkedFiles.push(f["Path"]);
        }
    }

    if (checkedFiles.length === 0) {
        return;
    }

    var req = new XMLHttpRequest();
    req.open("POST", "./api/DecodeJob", true);
    req.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    req.onload = function (e) {
        console.log(req.responseText);
    };
    req.send("files=" + checkedFiles);

    getStatus();
}

function addToTable(encodedfile) {
    var tableList = document.getElementById("encodedlist");

    var newRow = tableList.insertRow(tableList.rows.length);
    var cbCell = newRow.insertCell(0);

    var cb = document.createElement("input");
    cb.setAttribute("type", "checkbox");
    cb.setAttribute("id", "cb-" + encodedfile["Index"]);
    cbCell.appendChild(cb);

    var label = document.createElement("label");
    label.setAttribute("for", "cb-" + encodedfile["Index"]);
    cbCell.appendChild(label);

    var textCell = newRow.insertCell(1);
    var pathElement = document.createTextNode(file["Path"]);
    textCell.appendChild(pathElement);
}

function loadfiles() {
    var xhttp = new XMLHttpRequest();
    xhttp.open("GET", "./api/keyfiles", true);
    xhttp.setRequestHeader("Content-Type", "application/json");
    xhttp.onload = function (e) {
        if (xhttp.readyState === 4) {
            if (xhttp.status === 200) {
                var response = JSON.parse(xhttp.responseText);
                loadedFiles = response["files"];
                for (file of response["files"]) {
                    addToTable(file);
                }
                if (response["files"].length > 0) {
                    document.getElementById("decodeselected").classList.toggle("disabled");
                    document.getElementById("selectall").classList.toggle("disabled");
                    document.getElementById("decodeall").classList.toggle("disabled");
                }
            }
        }
    };
    xhttp.send(null);
}