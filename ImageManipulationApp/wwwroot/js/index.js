"use strict";

// Image input
let inputImage = document.querySelector("#imageInput");

// Postal code inputs
const postCodeElem = document.querySelector("#postalCodeAreaName");
let inputPostalCode = document.querySelector("#postalCodeInput");
let inputUpload = document.querySelector("#uploadInput");

// Passphrase inputs
let passphraseTextInput = document.querySelector("#passphraseInput");
let passphraseCommit = document.querySelector("#passphraseInputUpload");

// Canvas
let canvas = document.querySelector("#imageCanvas");

// Disable inputs until DOM and signalr is ready
inputImage.disabled = true;
inputPostalCode.disabled = true;
inputUpload.disabled = true;
passphraseTextInput.disabled = true;
passphraseCommit.disabled = true;

$(document).ready(function () {

    // Event-listeners
    $("#imageInput").change(function func(e) {
        setPreviewImage(e);
    });
    $("#postalCodeInput").keyup(function func(e) {
        getPostCodeInformation();
    });
    $("#uploadInput").click(function func(e) {
        uploadImage(e);
    });
    $("#passphraseInput").keyup(() => {
        passphraseCommit.disabled = !passphraseIsValidLength();
    });
    $("#passphraseInputUpload").click(() => postEncryptedImage());

    // Global variables
    let postCodeAreaName = "";

    // Show the image that the user has selected
    function setPreviewImage(e) {

        let img = document.querySelector("#imagePreview");
        let fileReader = new FileReader();

        fileReader.readAsDataURL(e.target.files[0]);
        fileReader.onload = function () {

            img.onload = function () {
                img.height = 400;
                img.width = 400;
            }
            img.onerror = function () {
                alert("Make sure image is of png");
            }
            img.src = fileReader.result.toString();
        };

        // When image is loaded we show the next step and unlock the button
        $("#postalCodeInputGroup").collapse("show");
        inputPostalCode.disabled = false;
    }

    // Try and fetch the name of the given postal code
    function getPostCodeInformation() {

        if (!postcodeIsValidLength()) {
            postCodeElem.textContent = "Ugyldig postnummer";
            postCodeAreaName = "Invalid";

            $("#uploadImageInputGroup").collapse("hide");
            inputUpload.disabled = true;
            return;
        }

        $.ajax({
            async: true,
            type: "get",
            crossDomain: true,
            data: { "postalCode": inputPostalCode.value.toString(), "country": "NO" },
            url: "https://localhost:44373/api/get-post-area-info",
            success: function (data, response, jqXHR) {

                if (response != null && response === "success") {

                    data = $.parseJSON(data);
                    postCodeElem.textContent = data.result;
                    postCodeAreaName = data.result;

                    // If successful show the next step and allow the user to submit the image
                    if (data.valid) {
                        $("#uploadImageInputGroup").collapse("show");
                        inputUpload.disabled = false;
                    } else {
                        $("#uploadImageInputGroup").collapse("hide");
                        inputUpload.disabled = true;
                    }
                }
                
            },
            error: function (jqXHR, response, errorThrown) {
                postCodeAreaName = "Invalid";
                $("#uploadImageInputGroup").collapse("hide");
                inputUpload.disabled = true;
            }
        });
    }

    // Upload the user image together with the data from the postal code
    function uploadImage(e) {
        e.preventDefault();

        let formData = new FormData();
        formData.append("PostalCodeAreaName", postCodeAreaName);
        formData.append("UserFile", inputImage.files[0]);

        let xhr = new XMLHttpRequest();
        xhr.open('POST', 'https://localhost:44373/api/post/manipulate-image', true);
        xhr.responseType = 'blob';
        xhr.onload = function (e) {
            let url = URL.createObjectURL(this.response);
            let img = new Image();

            img.onload = function () {

                const height = 800;
                const width = 1200;

                URL.revokeObjectURL(this.src);
                let ctx = canvas.getContext('2d');
                ctx.clearRect(0, 0, width, height);
                ctx.drawImage(this, 0, 0, width, height)

                ctx.font = "3em Arial";
                ctx.fillText(inputPostalCode.value, 50, 100);
            };
            img.src = url;
        };
        xhr.send(formData);

        // Show and enable passphrase text input
        $("#hiddenImageRow").collapse("show");
        passphraseTextInput.disabled = false;
        passphraseCommit.disabled = false;
    }

    // Post encrypted image to the server to save
    function postEncryptedImage() {

        if (!passphraseIsValidLength()) {
            alert("Passphrase is not valid length!");
            return;
        }

        // Disable the encrypt button to stop people from sending duplicate encrypted images to db
        passphraseCommit.disabled = true;

        // Get the canvas image as dataUrl
        let imgData = ($('#imageCanvas')[0]).toDataURL("image/png");
        imgData = imgData.replace('data:image/png;base64,', '');

        // encrypt the dataURL
        let encryptedImage = CryptoJS.AES.encrypt(imgData, $("#passphraseInput").val()).toString();

        $.ajax({
            async: true,
            type: "post",
            crossDomain: true,
            contentType: "application/json",
            data: JSON.stringify(encryptedImage),
            dataType: "text",
            url: "https://localhost:44373/api/post/store-encrypted-image",
            success: function (data, response, jqXHR) {
                if (response != null && response === "success") {
                    let paragraph = document.getElementById("guidNumber");
                    paragraph.innerHTML = "Your GUID is: ".concat(data);
                }
            },
            error: function (jqXHR, response, errorThrown) {
                console.log("err");
            }
        });
    }

    // Check that post code is correct length
    function postcodeIsValidLength() {
        return inputPostalCode.value.length == 4;
    }

    // Check that passphrase length is valid range
    function passphraseIsValidLength() {
        return passphraseTextInput.value.length > 3 && passphraseTextInput.value.length < 23;
    }

});
