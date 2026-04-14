// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function siteloadershow() {
    $('.cover-spin').show();

}

function siteloaderhide() {
    $('.cover-spin').hide();

}
$(window).on('beforeunload', function() {
    siteloadershow();
})

$(document).on('submit', 'form', function() {
    siteloadershow();
})
window.setTimeout(function() {
    siteloaderhide();
},2000)