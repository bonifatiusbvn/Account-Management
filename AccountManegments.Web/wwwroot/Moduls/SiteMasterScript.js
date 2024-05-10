AllSiteListTable();
fn_getState('stateDropdown', 1);
fn_getShippingState('ShippingState', 1);

function AllSiteListTable() {

    var searchText = $('#txtSiteSearch').val();
    var searchBy = $('#SiteSearchBy').val();

    $.get("/SiteMaster/SiteListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {


            $("#Sitetbody").html(result);
        })
        .fail(function (error) {
            siteloaderhide();
            console.error(error);
        });
}

function SitefilterTable() {
    siteloadershow();
    var searchText = $('#txtSiteSearch').val();
    var searchBy = $('#SiteSearchBy').val();

    $.ajax({
        url: '/SiteMaster/SiteListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#Sitetbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
        }
    });
}

function sortSiteTable() {
    siteloadershow();
    var sortBy = $('#SiteDataSortBy').val();
    $.ajax({
        url: '/SiteMaster/SiteListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#Sitetbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
        }
    });
}

function DisplaySiteDetails(SiteId) {
    siteloadershow();
    $.ajax({
        url: '/SiteMaster/DisplaySiteDetails?SiteId=' + SiteId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            $('#txtSiteid').val(response.siteId);
            $('#txtsiteName').val(response.siteName);
            $('#txtContectPersonName').val(response.contectPersonName);
            $('#txtContectPersonPhoneNo').val(response.contectPersonPhoneNo);
            $('#txtAddress').val(response.address);
            $('#txtArea').val(response.area);
            $('#stateDropdown').val(response.stateId);
            fn_getcitiesbystateId('ddlCity', response.stateId)
            $('#txtSiteCountry').val(response.country);
            $('#txtPincode').val(response.pincode);
            $('#txtShippingAddress').val(response.shippingAddress);
            $('#txtShippingArea').val(response.shippingArea);
            fn_getShippingcitiesbystateId('ShippingCity', response.shippingStateId)
            $('#ShippingState').val(response.shippingStateId);
            $('#txtShippingCountry').val(response.shippingCountry);
            $('#txtShippingPincode').val(response.shippingPincode);


            setTimeout(function () { $('#ddlCity').val(response.cityId); $('#ShippingCity').val(response.shippingCityId); }, 100)

            if (response.address == response.shippingAddress) {
                $('#hideShippingAddress').prop('checked', true);
                $('#shippingAddressFields').hide();
            } else {
                $('#hideShippingAddress').prop('checked', false);
                $('#shippingAddressFields').show();
            }

            var button = document.getElementById("btnSite");
            if ($('#txtSiteid').val() != '') {
                button.textContent = "Update";
            }
            var offcanvas = new bootstrap.Offcanvas(document.getElementById('createSite'));
            resetSiteForm();
            offcanvas.show();
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            console.error(xhr.responseText);
        }
    });
}

function SelectSiteDetails(SiteId, element) {
    siteloadershow();
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/SiteMaster/DisplaySiteDetails?SiteId=' + SiteId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response) {
                $('#dspSiteid').val(SiteId);
                $('#dspSiteName').val(response.siteName);
                $('#dspContactPersonName').val(response.contectPersonName);
                $('#dspContactPersonPhoneNo').val(response.contectPersonPhoneNo);
                $('#dspAddress').val(response.address);
                $('#dspCity').val(response.cityName);
                $('#dspPincode').val(response.pincode);
            } else {
                siteloaderhide();
                console.log('Empty response received.');
            }
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

function CreateSite() {
    siteloadershow();
    if ($("#siteForm").valid()) {
        var objData = {
            SiteName: $('#txtsiteName').val(),
            ContectPersonName: $('#txtContectPersonName').val(),
            ContectPersonPhoneNo: $('#txtContectPersonPhoneNo').val(),
            Address: $('#txtAddress').val(),
            Area: $('#txtArea').val(),
            CityId: $('#ddlCity').val(),
            StateId: $('#stateDropdown').val(),
            Country: $('#ddlCountry').val(),
            Pincode: $('#txtPincode').val(),
            ShippingAddress: $('#hideShippingAddress').is(':checked') ? $('#txtAddress').val() : $('#txtShippingAddress').val(),
            ShippingArea: $('#hideShippingAddress').is(':checked') ? $('#txtArea').val() : $('#txtShippingArea').val(),
            ShippingPincode: $('#hideShippingAddress').is(':checked') ? $('#txtPincode').val() : $('#txtShippingPincode').val(),
            ShippingCityId: $('#hideShippingAddress').is(':checked') ? $('#ddlCity').val() : $('#ShippingCity').val(),
            ShippingStateId: $('#hideShippingAddress').is(':checked') ? $('#stateDropdown').val() : $('#ShippingState').val(),
            ShippingCountry: $('#hideShippingAddress').is(':checked') ? $('#ddlCountry').val() : $('#shippingCountry').val(),
        }
        $.ajax({
            url: '/SiteMaster/CreateSite',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                siteloaderhide();
                Swal.fire({
                    title: Result.message,
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(function () {
                    window.location = '/SiteMaster/SiteListView';
                });
            },
        })
    }
    else {
        siteloaderhide();
        Swal.fire({
            title: "Kindly fill all details",
            icon: 'warning',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'OK',
        })
    }
}

function UpdateSiteDetails() {
    siteloadershow();
    if ($("#siteForm").valid()) {

        var objData = {
            SiteId: $('#txtSiteid').val(),
            SiteName: $('#txtsiteName').val(),
            ContectPersonName: $('#txtContectPersonName').val(),
            ContectPersonPhoneNo: $('#txtContectPersonPhoneNo').val(),
            Address: $('#txtAddress').val(),
            Area: $('#txtArea').val(),
            CityId: $('#ddlCity').val(),
            StateId: $('#stateDropdown').val(),
            Country: $('#ddlCountry').val(),
            Pincode: $('#txtPincode').val(),
            ShippingAddress: $('#hideShippingAddress').is(':checked') ? $('#txtAddress').val() : $('#txtShippingAddress').val(),
            ShippingArea: $('#hideShippingAddress').is(':checked') ? $('#txtArea').val() : $('#txtShippingArea').val(),
            ShippingPincode: $('#hideShippingAddress').is(':checked') ? $('#txtPincode').val() : $('#txtShippingPincode').val(),
            ShippingCityId: $('#hideShippingAddress').is(':checked') ? $('#ddlCity').val() : $('#ShippingCity').val(),
            ShippingStateId: $('#hideShippingAddress').is(':checked') ? $('#stateDropdown').val() : $('#ShippingState').val(),
            ShippingCountry: $('#hideShippingAddress').is(':checked') ? $('#ddlCountry').val() : $('#shippingCountry').val(),
        }
        $.ajax({
            url: '/SiteMaster/UpdateSiteDetails',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                siteloaderhide();
                Swal.fire({
                    title: Result.message,
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(function () {
                    window.location = '/SiteMaster/SiteListView';
                });
            },
        })
    }
    else {
        siteloaderhide();
        Swal.fire({
            title: "Kindly fill all details",
            icon: 'warning',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'OK',
        })
    }
}

function ClearSiteTextBox() {
    resetSiteForm();
    $('#txtSiteid').val('');
    $('#txtsiteName').val('');
    $('#txtContectPersonName').val('');
    $('#txtContectPersonPhoneNo').val('');
    $('#txtAddress').val('');
    $('#txtArea').val('');
    $('#ddlCity').val('');
    $('#stateDropdown').val('');
    $('#txtPincode').val('');
    $('#ShippingCity').val('');
    $('#ShippingState').val('');
    $('#txtShippingAddress').val('');
    $('#txtShippingArea').val('');
    $('#txtShippingPincode').val('');
    var button = document.getElementById("btnSite");
    if ($('#txtSiteid').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('createSite'));
    offcanvas.show();
}
var sForm;
function validateAndCreateSite() {

    sForm = $("#siteForm").validate({

        rules: {

            txtsiteName: "required",
            txtContectPersonName: "required",
            txtContectPersonPhoneNo: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 10
            },
            txtAddress: "required",
            txtArea: "required",
            txtPincode: {
                required: true,
                digits: true,
                minlength: 6,
                maxlength: 6
            },
            ddlCountry: "required",
            stateDropdown: "required",
            ddlCity: "required",
            txtShippingAddress: "required",
            txtShippingArea: "required",
            txtShippingPincode: {
                required: true,
                digits: true,
                minlength: 6,
                maxlength: 6
            },
            ShippingCity: "required",
            ShippingState: "required",
            shippingCountry: "required",
        },
        messages: {
            txtsiteName: "Please Enter siteName",
            txtContectPersonName: "Please Enter ContectPersonName",
            txtContectPersonPhoneNo: {
                required: "Please Enter Phone Number",
                digits: "Please enter a valid 10-digit phone number",
                minlength: "Phone number must be 10 digits long",
                maxlength: "Phone number must be 10 digits long"
            },
            txtAddress: "Please Enter Address",
            txtArea: "Please Enter Area",
            txtPincode: {
                required: "Please Enter Pin Code",
                digits: "Pin code must contain only digits",
                minlength: "Pin code must be 6 digits long",
                maxlength: "Pin code must be 6 digits long"
            },
            ddlCountry: "Please Enter Country",
            stateDropdown: "Please Enter state",
            ddlCity: "Please Enter City",
            txtShippingAddress: "Please Enter ShippingAddress",
            txtShippingArea: "Please Enter ShippingArea",
            txtShippingPincode: {
                required: "Please Enter Pin Code",
                digits: "Pin code must contain only digits",
                minlength: "Pin code must be 6 digits long",
                maxlength: "Pin code must be 6 digits long"
            },
            ShippingCity: "Please Enter ShippingCity",
            ShippingState: "Please Enter ShippingState",
            shippingCountry: "Please Enter shippingCountry",
        }
    })
    var isValid = true;
    if (isValid) {
        if ($("#txtSiteid").val() == '') {
            CreateSite();
        }
        else {
            UpdateSiteDetails();
        }
    }
}
function resetSiteForm() {
    if (sForm) {
        sForm.resetForm();
    }
}
function isValidPhoneNo(ContectPersonPhoneNo) {

    var phoneNoPattern = /^\d{10}$/;
    return phoneNoPattern.test(ContectPersonPhoneNo);
}

function ActiveDecativeSite(SiteId) {

    var isChecked = $('#flexSwitchCheckChecked_' + SiteId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to Active this Site?" : "Are you sure want to DeActive this Site?";

    Swal.fire({
        title: confirmationMessage,
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, Enter it!",
        cancelButtonText: "No, cancel!",
        confirmButtonClass: "btn btn-primary w-xs me-2 mt-2",
        cancelButtonClass: "btn btn-danger w-xs mt-2",
        buttonsStyling: false,
        showCloseButton: true
    }).then((result) => {
        if (result.isConfirmed) {
            var formData = new FormData();
            formData.append("SiteId", SiteId);

            $.ajax({
                url: '/SiteMaster/ActiveDeactiveSite?SiteId=' + SiteId,
                type: 'Post',
                contentType: 'application/json;charset=utf-8;',
                dataType: 'json',
                success: function (Result) {
                    if (Result.code == 200) {
                        Swal.fire({
                            title: isChecked ? "Active!" : "DeActive!",
                            text: Result.message,
                            icon: "success",
                            confirmButtonClass: "btn btn-primary w-xs mt-2",
                            buttonsStyling: false
                        }).then(function () {
                            window.location = '/SiteMaster/SiteListView';
                        });
                    } else {
                        Swal.fire({
                            title: Result.message,
                            icon: "warning",
                            confirmButtonClass: "btn btn-primary w-xs mt-2",
                            buttonsStyling: false
                        });
                    }

                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Site Have No Changes.!!😊',
                'error'
            );
        }
    });
}

function DeleteSite(SiteId) {
    Swal.fire({
        title: "If you want to delete this site,delete all data related this site!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, Delete it!",
        cancelButtonText: "No, cancel!",
        confirmButtonClass: "btn btn-primary w-xs me-2 mt-2",
        cancelButtonClass: "btn btn-danger w-xs mt-2",
        buttonsStyling: false,
        showCloseButton: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/SiteMaster/DeleteSite?SiteId=' + SiteId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {
                    if (Result.code == 200) {
                        Swal.fire({
                            title: Result.message,
                            icon: 'success',
                            confirmButtonColor: '#3085d6',
                            confirmButtonText: 'OK'
                        }).then(function () {
                            window.location = '/SiteMaster/SiteListView';
                        })
                    }
                    else {
                        Swal.fire({
                            title: Result.message,
                            icon: 'warning',
                            confirmButtonColor: '#3085d6',
                            confirmButtonText: 'OK'
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        title: "Can't Delete Site!",
                        icon: 'warning',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK',
                    }).then(function () {
                        window.location = '/SiteMaster/SiteListView';
                    })
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Site Have No Changes.!!😊',
                'error'
            );
        }
    });
}

$(document).ready(function () {

    GetShippingCountry();

    $('#dropShippingState').change(function () {

        var Text = $("#dropShippingState Option:Selected").text();
        var txtstateid = $(this).val();
        $("#txtShippingstate").val(txtstateid);
    });

    $('#ShippingCity').change(function () {

        var Text = $("#shippingCity Option:Selected").text();
        var txtShippingcity = $(this).val();
        $("#txtShippingCity").val(txtShippingcity);
    });

});

function fn_getShippingState(drpShippingstate, countryId, that) {
    var cid = countryId;
    if (cid == undefined || cid == null) {
        var cid = $(that).val();
    }


    $('#' + drpShippingstate).empty();
    $('#' + drpShippingstate).append('<Option >--Select State--</Option>');
    $.ajax({
        url: '/Authentication/GetState?StateId=' + cid,
        success: function (result) {

            $.each(result, function (i, data) {
                $('#' + drpShippingstate).append('<Option value=' + data.id + '>' + data.stateName + '</Option>')
            });
        }
    });
}

function fn_getShippingcitiesbystateId(drpShippingcity, stateid, that) {

    var sid = stateid;
    if (sid == undefined || sid == null) {
        var sid = $(that).val();
    }


    $('#' + drpShippingcity).empty();
    $('#' + drpShippingcity).append('<Option >--Select City--</Option>');
    $.ajax({
        url: '/Authentication/GetCity?CityId=' + sid,
        success: function (result) {

            $.each(result, function (i, data) {
                $('#' + drpShippingcity).append('<Option value=' + data.id + '>' + data.cityName + '</Option>');

            });
        }
    });
}

function GetShippingCountry() {

    $.ajax({
        url: '/Authentication/GetCountrys',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#shippingCountry').append('<Option value=' + data.id + ' Selected>' + data.countryName + '</Option>')

            });
        }
    });
}

function toggleShippingAddress() {
    var checkbox = document.getElementById("hideShippingAddress");
    var shippingFields = document.getElementById("shippingAddressFields");

    if (checkbox.checked) {
        shippingFields.style.display = "none";
    } else {
        shippingFields.style.display = "block";
    }
}
