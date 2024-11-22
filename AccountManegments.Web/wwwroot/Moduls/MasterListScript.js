
$(document).ready(function () {

    GetCountry();

    $('#dropState').change(function () {

        var Text = $("#dropState Option:Selected").text();
        var txtstateid = $(this).val();
        $("#txtstate").val(txtstateid);
    });

    $('#ddlCity').change(function () {

        var Text = $("#ddlCity Option:Selected").text();
        var txtcityid = $(this).val();
        $("#txtcity").val(txtcityid);
    });

    $('#ddlUserRole').change(function () {
        var Text = $("#ddlUserRole Option:Selected").text();
        var txtroleid = $(this).val();
        $("#txtrole").val(txtroleid);
    });

});

function fn_getState(drpstate, countryId, that) {
    var cid = countryId;
    if (cid == undefined || cid == null) {
        var cid = $(that).val();
    }

    $('#' + drpstate).empty();
    $('#' + drpstate).append('<Option >--Select State--</Option>');
    $.ajax({
        url: '/Authentication/GetState?StateId=' + cid,
        success: function (result) {

            $.each(result, function (i, data) {
                $('#' + drpstate).append('<Option value=' + data.id + '>' + data.stateName + '</Option>')
            });
        }
    });
}

function fn_getcitiesbystateId(drpcity, stateid, that) {

    var sid = stateid;
    if (sid == undefined || sid == null) {
        var sid = $(that).val();
    }
    $('#' + drpcity).empty();
    $('#' + drpcity).append('<Option >--Select City--</Option>');
    $.ajax({
        url: '/Authentication/GetCity?CityId=' + sid,
        success: function (result) {

            $.each(result, function (i, data) {
                $('#' + drpcity).append('<Option value=' + data.id + '>' + data.cityName + '</Option>');

            });
        }
    });
}

function GetCountry() {

    $.ajax({
        url: '/Authentication/GetCountrys',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#ddlCountry').append('<Option value=' + data.id + ' Selected>' + data.countryName + '</Option>')

            });
        }
    });
}

function GetCompanyDetail(that) {
    debugger
    $.ajax({
        url: '/Company/GetCompanyNameList',
        success: function (result) {
            debugger
            if (result.length > 0) {
                var $dropdown = $('#' + that);
                $dropdown.empty();

                $dropdown.append('<option selected value="" disabled>Select Company</option>');


                $.each(result, function (i, data) {
                    $dropdown.append('<option value="' + data.companyId + '">' + data.companyName + '</option>');
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching company details:', error);
        }
    });
}
