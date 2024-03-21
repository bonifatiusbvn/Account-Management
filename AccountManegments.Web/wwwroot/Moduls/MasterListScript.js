
$(document).ready(function () {

    GetCountry();
    $('#ddlCountry').change(function () {
        var Text = $("#ddlCountry Option:Selected").text();
        var StateId = $(this).val();
        $("#txtcountry").val(Text);
        $('#ddlState').empty();
        $('#ddlState').append('<Option >--Select State--</Option>');
        $.ajax({
            url: '/Authentication/GetState?StateId=' + StateId,
            success: function (result) {

                $.each(result, function (i, data) {
                    $('#ddlState').append('<Option value=' + data.id + '>' + data.stateName + '</Option>')
                });
            }
        });
    });


    $('#ddlState').change(function () {

        var Text = $("#ddlState Option:Selected").text();
        var CityId = $(this).val();
        $("#txtstate").val(Text);
        $('#ddlCity').empty();
        $('#ddlCity').append('<Option >--Select City--</Option>');
        $.ajax({
            url: '/Authentication/GetCity?CityId=' + CityId,
            success: function (result) {
                $.each(result, function (i, data) {
                    $('#ddlCity').append('<Option value=' + data.id + '>' + data.cityName + '</Option>');

                });
            }
        });
    });

});


function GetCountry() {

    $.ajax({
        url: '/Authentication/GetCountrys',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#ddlCountry').append('<Option value=' + data.id + '>' + data.countryName + '</Option>')

            });
        }
    });
}

function Citytext(sel) {
    $("#txtcity").val((sel.options[sel.selectedIndex].text));
}


$(document).ready(function () {

    GetShippingCountry();
    $('#ddlShippingCountry').change(function () {
        var Text = $("#ddlShippingCountry Option:Selected").text();
        var StateId = $(this).val();
        $("#txtShippingcountry").val(Text);
        $('#ddlShippingState').empty();
        $('#ddlShippingState').append('<Option >--Select State--</Option>');
        $.ajax({
            url: '/Authentication/GetState?StateId=' + StateId,
            success: function (result) {

                $.each(result, function (i, data) {
                    $('#ddlShippingState').append('<Option value=' + data.id + '>' + data.stateName + '</Option>')
                });
            }
        });
    });


    $('#ddlShippingState').change(function () {

        var Text = $("#ddlShippingState Option:Selected").text();
        var CityId = $(this).val();
        $("#txtShippingstate").val(Text);
        $('#ddlShippingCity').empty();
        $('#ddlShippingCity').append('<Option >--Select City--</Option>');
        $.ajax({
            url: '/Authentication/GetCity?CityId=' + CityId,
            success: function (result) {
                $.each(result, function (i, data) {
                    $('#ddlShippingCity').append('<Option value=' + data.id + '>' + data.cityName + '</Option>');

                });
            }
        });
    });

});


function GetShippingCountry() {

    $.ajax({
        url: '/Authentication/GetCountrys',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#ddlShippingCountry').append('<Option value=' + data.id + '>' + data.countryName + '</Option>')

            });
        }
    });
}

function ShippingCitytext(sel) {
    $("#txtShippingcity").val((sel.options[sel.selectedIndex].text));
}
