AllSiteListTable();
function AllSiteListTable() {

    var searchText = $('#txtSearch').val();
    var searchBy = $('#ddlSearchBy').val();

    $.get("/SiteMaster/SiteListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {


            $("#Sitetbody").html(result);
        })
        .fail(function (error) {
            console.error(error);
        });
}

function filterTable() {

    var searchText = $('#txtSearch').val();
    var searchBy = $('#ddlSearchBy').val();

    $.ajax({
        url: '/SiteMaster/SiteListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            $("#Sitetbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function sortTable() {
    var sortBy = $('#ddlSortBy').val();
    $.ajax({
        url: '/SiteMaster/SiteListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            $("#Sitetbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function DisplaySiteDetails(SiteId) {
    debugger 
    $.ajax({
        url: '/SiteMaster/DisplaySiteDetails?SiteId=' + SiteId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
     debugger
            $('#txtSiteid').val(response.siteId);
            $('#txtSiteName').val(response.siteName);
            $('#txtContectPersonName').val(response.contectPersonName);
            $('#txtContectPersonPhoneNo').val(response.contectPersonPhoneNo);
            $('#txtAddress').val(response.address);
            $('#txtArea').val(response.area);
            $('#txtcity').val(response.cityId);
            $('#txtstate').val(response.stateId);
            $('#txtcountry').val(response.country);
            $('#txtPincode').val(response.pincode);
            $('#txtShippingAddress').val(response.shippingAddress);
            $('#txtShippingArea').val(response.shippingArea);
            $('#txtShippingcity').val(response.shippingCityName);
            $('#txtShippingstate').val(response.shippingState);
            $('#txtShippingcountry').val(response.shippingCountry);
            $('#txtShippingPincode').val(response.shippingPincode);
            var button = document.getElementById("btnSite");
            if ($('#txtSiteid').val() != '') {
                button.textContent = "Update";
            }
            var offcanvas = new bootstrap.Offcanvas(document.getElementById('createSite'));
            offcanvas.show();
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

function SelectSiteDetails(SiteId, element) {

    $('.row.ac-card').removeClass('active');
    $(element).closest('.row.ac-card').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/SiteMaster/DisplaySiteDetails?SiteId=' + SiteId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            if (response) {
                $('#dspSiteid').val(SiteId);
                $('#dspSiteName').val(response.siteName);
                $('#dspContactPersonName').val(response.contectPersonName);
                $('#dspContactPersonPhoneNo').val(response.contectPersonPhoneNo);
                $('#dspAddress').val(response.address);
                $('#dspCity').val(response.cityName);
                $('#dspPincode').val(response.pincode);
            } else {
                console.log('Empty response received.');
            }
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

function CreateSite() {
    var objData = {
        SiteName: $('#txtSiteName').val(),
        ContectPersonName :$('#txtContectPersonName').val(),
        ContectPersonPhoneNo:$('#txtContectPersonPhoneNo').val(),
        Address:$('#txtAddress').val(),
        Area:$('#txtArea').val(),
        CityId: $('#ddlCity').val(),
        StateId: $('#ddlState').val(),
        Country: $('#ddlCountry').val(),
        Pincode:$('#txtPincode').val(),
        ShippingAddress: $('#txtShippingAddress').val(),
        ShippingArea:$('#txtShippingArea').val(),
        ShippingPincode: $('#txtShippingPincode').val(),
        ShippingCityId: $('#ddlShippingCity').val(),
        ShippingStateId: $('#ddlShippingState').val(),
        ShippingCountry: $('#ddlShippingCountry').val(),
    }
    $.ajax({
        url: '/SiteMaster/CreateSite',
        type: 'post',
        data: objData,
        datatype: 'json',
        success: function (Result) {
     
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

function UpdateSiteDetails() {
   
    var objData = {
        SiteId: $('#txtSiteid').val(),
        SiteName: $('#txtSiteName').val(),
        ContectPersonName: $('#txtContectPersonName').val(),
        ContectPersonPhoneNo: $('#txtContectPersonPhoneNo').val(),
        Address: $('#txtAddress').val(),
        Area: $('#txtArea').val(),
        CityId: $('#ddlCity').val(),
        StateId: $('#ddlState').val(),
        Country: $('#ddlCountry').val(),
        Pincode: $('#txtPincode').val(),
        ShippingAddress: $('#txtShippingAddress').val(),
        ShippingArea: $('#txtShippingArea').val(),
        ShippingPincode: $('#txtShippingPincode').val(),
        ShippingCityId: $('#ddlShippingCity').val(),
        ShippingStateId: $('#ddlShippingState').val(),
        ShippingCountry: $('#ddlShippingCountry').val(),
    }
    $.ajax({
        url: '/SiteMaster/UpdateSiteDetails',
        type: 'post',
        data: objData,
        datatype: 'json',
        success: function (Result) {

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

function ClearTextBox() {
    resetErrorMessages();
    $('#txtSiteid').val('');
    $('#txtSiteName').val('');
    $('#txtContectPersonName').val('');
    $('#txtContectPersonPhoneNo').val('');
    $('#txtAddress').val('');
    $('#txtArea').val('');
    $('#txtcity').val('');
    $('#txtstate').val('');
    $('#txtcountry').val('');
    $('#txtPincode').val('');
    $('#txtShippingcity').val('');
    $('#txtShippingstate').val('');
    $('#txtShippingcountry').val('');
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

function validateAndCreateSite() {

    resetErrorMessages();
    var SiteName = document.getElementById("txtSiteName").value.trim();
    var ContectPersonName = document.getElementById("txtContectPersonName").value.trim();
    var ContectPersonPhoneNo = document.getElementById("txtContectPersonPhoneNo").value.trim();
    var Address = document.getElementById("txtAddress").value.trim();
    var Area = document.getElementById("txtArea").value.trim();
    var CityId = document.getElementById("ddlCity").value.trim();
    var StateId = document.getElementById("txtstate").value.trim();
    var Country = document.getElementById("txtcountry").value.trim();
    var Pincode = document.getElementById("txtPincode").value.trim();
    var ShippingCityId = document.getElementById("txtShippingcity").value.trim();
    var ShippingStateId = document.getElementById("txtShippingstate").value.trim();
    var ShippingCountry = document.getElementById("txtShippingcountry").value.trim();
    var ShippingAddress = document.getElementById("txtShippingAddress").value.trim();
    var ShippingArea = document.getElementById("txtShippingArea").value.trim();
    var ShippingPincode = document.getElementById("txtShippingPincode").value.trim();



    var isValid = true;


    if (SiteName === "") {
        document.getElementById("spnSiteName").innerText = "Site Name is required.";
        isValid = false;
    }


    if (ContectPersonName === "") {
        document.getElementById("spnContectPersonName").innerText = "Contact Person Name is required.";
        isValid = false;
    }


    if (Address === "") {
        document.getElementById("spnAddress").innerText = "Address is required.";
        isValid = false;
    }


    if (Area === "") {
        document.getElementById("spnArea").innerText = "Area is required.";
        isValid = false;
    }


    if (CityId === "") {
        document.getElementById("spnCity").innerText = "City is required.";
        isValid = false;
    }

    if (StateId === "") {
        document.getElementById("spnState").innerText = "State is required.";
        isValid = false;
    }

    if (Country === "") {
        document.getElementById("spnCountry").innerText = "Country is required.";
        isValid = false;
    }


    if (Pincode === "") {
        document.getElementById("spnPincode").innerText = "Pincode is required.";
        isValid = false;
    }


    if (ShippingCountry === "") {
        document.getElementById("spnShippingCountry").innerText = "Shipping Country is required.";
        isValid = false;
    }


    if (ShippingStateId === "") {
        document.getElementById("spnShippingState").innerText = "Shipping State is required.";
        isValid = false;
    }

    if (ShippingCityId === "") {
        document.getElementById("spnShippingCity").innerText = "Shipping City is required.";
        isValid = false;
    }


    if (ShippingAddress === "") {
        document.getElementById("spnShippingAddress").innerText = "Shipping Address is required.";
        isValid = false;
    }


    if (ShippingArea === "") {
        document.getElementById("spnShippingArea").innerText = "Shipping Area is required.";
        isValid = false;
    }


    if (ShippingPincode === "") {
        document.getElementById("spnShippingPincode").innerText = "Shipping Pincode is required.";
        isValid = false;
    }


    if (ContectPersonPhoneNo === "") {
        document.getElementById("spnContectPersonPhoneNo").innerText = "Contact Person Phone Number is required.";
        isValid = false;
    } else if (!isValidPhoneNo(ContectPersonPhoneNo)) {
        document.getElementById("spnContectPersonPhoneNo").innerText = "Invalid Phone Number format.";
        isValid = false;
    }


    if (isValid) {
        if ($("#txtSiteid").val() == '') {
            CreateSite();
        }
        else {
            UpdateSiteDetails();
        }
    }
}


function resetErrorMessages() {
    document.getElementById("spnSiteName").innerText = "";
    document.getElementById("spnContectPersonName").innerText = "";
    document.getElementById("spnContectPersonPhoneNo").innerText = "";
    document.getElementById("spnCountry").innerText = "";
    document.getElementById("spnState").innerText = "";
    document.getElementById("spnCity").innerText = "";
    document.getElementById("spnArea").innerText = "";
    document.getElementById("spnAddress").innerText = "";
    document.getElementById("spnPincode").innerText = "";
    document.getElementById("spnShippingCountry").innerText = "";
    document.getElementById("spnShippingState").innerText = "";
    document.getElementById("spnShippingCity").innerText = "";
    document.getElementById("spnShippingArea").innerText = "";
    document.getElementById("spnShippingAddress").innerText = "";
    document.getElementById("spnShippingPincode").innerText = "";
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
                    Swal.fire({
                        title: isChecked ? "Active!" : "DeActive!",
                        text: Result.message,
                        icon: "success",
                        confirmButtonClass: "btn btn-primary w-xs mt-2",
                        buttonsStyling: false
                    }).then(function () {
                        window.location = '/SiteMaster/SiteListView';
                    });
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Site Have No Changes.!!😊',
                'error'
            ).then(function () {
                window.location = '/SiteMaster/SiteListView';
            });;
        }
    });
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
    
        $("#txtShippingstate").val(CityId);
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

    $('#ddlShippingCity').change(function () {
    
        var Text = $("#ddlShippingCity Option:Selected").text();
        var txtShippingcity = $(this).val();
        $("#txtShippingcity").val(txtShippingcity);
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

