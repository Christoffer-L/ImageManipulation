"use strict";

//import { singalrReady } from "index.js";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:44373/imageManipulationHub")
    .withAutomaticReconnect()
    .build();

const MAX_LIST_CHILDS = 10;

const userImageInput = document.querySelector("#imageInput");

// connection start
connection.start().then(function () {
    if (userImageInput)
        userImageInput.disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

// RecevieNotification
connection.on("ReceivedNotification", function (message) {

    let list = document.getElementById("notificationList");

    if (list.childElementCount > MAX_LIST_CHILDS)
        list.removeChild(list.firstChild);

    let li = document.createElement("li");
    li.textContent = message;

    list.appendChild(li);
});