"use strict";

// GUID DIVs
const inputGUID = $("#GUIDInput");
const inputUpload = $("#uploadInput");

// Passphrase DIVs
const containerPassphrase = $("#passphraseInputGroup");
const inputPassphrase = $("#passphraseInput");
const inputPassphraseUpload = $("#passphraseConfirmInput");

// Disable input buttons
inputGUID.disabled = true;
inputUpload.disabled = true;
inputPassphrase.disabled = true;

$(document).ready(function () {

    // Global-variables
    let encryptedImage = null;

    // Enable inputs
    inputGUID.disabled = false;
    inputUpload.disabled = false;

    // Event-listener
    inputUpload.click(function event() {
        fetchEncryptedImage();
    });
    inputPassphraseUpload.click(function event() {
        decryptImage();
    });

    // Fetches a decrypt image based on supplied GUID
    function fetchEncryptedImage() {

        if (!isGUIDValid()) {
            // Send notification that the GUID is not filled inn
            return;
        }

        $.ajax({
            url: "https://localhost:44373/api/get-encrypted-image",
            type: "get",
            crossDomain: true,
            data: { "guid": inputGUID.val() },
            success: function (data) {
                // Save the encrypted image data
                encryptedImage = data;

                // Show the passphrase inputs
                containerPassphrase.collapse("show");
                inputPassphrase.disabled = false;
                inputPassphraseUpload.disabled = false;
            },
            error: function (data) {
                alert("Finner ikke bilde med GUID: ".concat(inputGuid.val()));
            }
        });
    }

    // Decryptes the image fetched by GUID and displays it
    function decryptImage() {

        // make sure that the passphrase is of correct length
        if (!isPassPhraseValidLength()) {
            alert("Passphrase is invalid length")
            return;
        }

        let decryptedImage = "";
        // Try to decrypt the image, will throw expetion if supplied string is invalid
        try {
            // Decrypted the bytes of data and convert them back into string
            let bytes = CryptoJS.AES.decrypt(encryptedImage, inputPassphrase.val());
            decryptedImage = bytes.toString(CryptoJS.enc.Utf8);
        } catch (err) {
            
        }

        // Create a new image and try to display it
        let img = new Image();
        img.onload = function () {
            $("#imageContainer").empty();
            $("#imageContainer").append(img);
        };
        img.onerror = function () {
            alert("wrong passphrase");
        };
        img.src = "data:image/png;base64," + decryptedImage;
    }

    function isGUIDValid() {
        return inputGUID.val().length > 0;
    }

    function isPassPhraseValidLength() {
        return inputPassphrase.val().length > 3 && inputPassphrase.val().length < 13;
    }

});