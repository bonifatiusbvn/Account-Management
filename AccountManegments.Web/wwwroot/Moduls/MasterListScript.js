
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
                debugger
                $.each(result, function (i, data) {
                    $('#ddlState').append('<Option value=' + data.id + '>' + data.stateName + '</Option>')
                });
            }
        });
    });


    $('#ddlState').change(function () {

        var Text = $("#ddlState Option:Selected").text();
        var CityId = $(this).val();
        debugger
        $("#txtstate").val(CityId);
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

    $('#ddlCity').change(function () {
        debugger
        var Text = $("#ddlCity Option:Selected").text();
        var txtcity = $(this).val();
        $("#txtcity").val(txtcity);
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

