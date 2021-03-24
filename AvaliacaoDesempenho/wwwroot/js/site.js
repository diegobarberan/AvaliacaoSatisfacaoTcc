// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function validacaoEmail(field) {
    usuario = field.value.substring(0, field.value.indexOf("@"));
    dominio = field.value.substring(field.value.indexOf("@") + 1, field.value.length);
    if (dominio == "hotmail.com") {
        document.getElementById("msgEmail").innerHTML = "<font color='blue'>Email válido </font>";
        return true;
    }
    else {
        document.getElementById("msgEmail").innerHTML = "Email invalido. Obrigatório usar email institucional '@hotmail.com'. ";        
        document.getElementById("Email").onfocus;
        return false;

        
    }
}