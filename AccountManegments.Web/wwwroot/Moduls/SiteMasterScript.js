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
            console.error(error);
        });
}

function SitefilterTable() {

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
            $("#Sitetbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function sortSiteTable() {
    var sortBy = $('#SiteDataSortBy').val();
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
  
    $.ajax({
        url: '/SiteMaster/DisplaySiteDetails?SiteId=' + SiteId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
 
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

            var button = document.getElementById("btnSite");
            if ($('#txtSiteid').val() != '') {
                button.textContent = "Update";
            }
            var offcanvas = new bootstrap.Offcanvas(document.getElementById('createSite'));
            offcanvas.show();
            resetErrorMessages();
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
        SiteName: $('#txtsiteName').val(),
        ContectPersonName :$('#txtContectPersonName').val(),
        ContectPersonPhoneNo:$('#txtContectPersonPhoneNo').val(),
        Address:$('#txtAddress').val(),
        Area:$('#txtArea').val(),
        CityId: $('#ddlCity').val(),
        StateId: $('#stateDropdown').val(),
        Country: $('#ddlCountry').val(),
        Pincode:$('#txtPincode').val(),
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

function ClearSiteTextBox() {

    resetErrorMessages();
    $('#txtSiteid').val('');
    $('#txtsiteName').val('');
    $('#txtContectPersonName').val('');
    $('#txtContectPersonPhoneNo').val('');
    $('#txtAddress').val('');
    $('#txtArea').val('');
    $('#ddlCity').val('');
    $('#stateDropdown').val('');
    $("#ddlCountry").val('');
    $('#txtPincode').val('');
    $('#ShippingCity').val('');
    $('#ShippingState').val('');
    $('#shippingCountry').val('');
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
    var SiteName = document.getElementById("txtsiteName").value.trim();
    var ContectPersonName = document.getElementById("txtContectPersonName").value.trim();
    var ContectPersonPhoneNo = document.getElementById("txtContectPersonPhoneNo").value.trim();
    var Address = document.getElementById("txtAddress").value.trim();
    var Area = document.getElementById("txtArea").value.trim();
    var CityId = document.getElementById("ddlCity").value.trim();
    var StateId = document.getElementById("stateDropdown").value.trim();
    var Country = document.getElementById("ddlCountry").value.trim();
    var Pincode = document.getElementById("txtPincode").value.trim();
    var ShippingCityId = document.getElementById("ShippingCity").value.trim();
    var ShippingStateId = document.getElementById("ShippingState").value.trim();
    var ShippingCountry = document.getElementById("shippingCountry").value.trim();
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
    } else if (Address.length > 100) {
        document.getElementById("spnAddress").innerText = "Address should not exceed 100 characters.";
        isValid = false;
    }


    if (Area === "") {
        document.getElementById("spnArea").innerText = "Area is required.";
        isValid = false;
    }


    if (CityId === "") {
        document.getElementById("spnCity").innerText = "City is required.";
        isValid = false;
    } else if(CityId === "--Select City--") {
        document.getElementById("spnCity").innerText = "City is required.";
        isValid = false;
    }


    if (StateId === "") {
        document.getElementById("spnState").innerText = "State is required.";
        isValid = false;
    }
    else if(StateId === "--Select State--") {
        document.getElementById("spnState").innerText = "State is required.";
        isValid = false;
    }


    if (Country === "") {
        document.getElementById("spnCountry").innerText = "Country is required.";
        isValid = false;
    }
    else if(Country === "--Select Country--") {
        document.getElementById("spnCountry").innerText = "Country is required.";
        isValid = false;
    }


    if (Pincode === "") {
        document.getElementById("spnPincode").innerText = "Pincode is required.";
        isValid = false;
    }


    if (!document.getElementById("hideShippingAddress").checked) {
        if (ShippingCountry === "") {
            document.getElementById("spnShippingCountry").innerText = "Shipping Country is required.";
            isValid = false;
        } else if (ShippingCountry === "--Select Country--") {
            document.getElementById("spnShippingCountry").innerText = "Shipping Country is required.";
            isValid = false;
        }
    }


    if (!document.getElementById("hideShippingAddress").checked) {
        if (ShippingStateId === "") {
            document.getElementById("spnShippingState").innerText = "Shipping State is required.";
            isValid = false;
        }
        else if (ShippingStateId === "--Select State--") {
            document.getElementById("spnShippingState").innerText = "Shipping State is required.";
            isValid = false;
        }
    }


    if (!document.getElementById("hideShippingAddress").checked) {
        if (ShippingCityId === "") {
            document.getElementById("spnShippingCity").innerText = "Shipping City is required.";
            isValid = false;
        }
        else if (ShippingCityId === "--Select City--") {
            document.getElementById("spnShippingCity").innerText = "Shipping City is required.";
            isValid = false;
        }
    }


    if (!document.getElementById("hideShippingAddress").checked) {
        if (ShippingAddress === "") {
            document.getElementById("spnShippingAddress").innerText = "Shipping Address is required.";
            isValid = false;    
        }
        else if (ShippingAddress.length > 100) {
            document.getElementById("spnShippingAddress").innerText = "Address should not exceed 100 characters.";
            isValid = false;
        }
    }


    if (!document.getElementById("hideShippingAddress").checked) {
        if (ShippingArea === "") {

            document.getElementById("spnShippingArea").innerText = "Shipping Area is required.";
            isValid = false;
        }
    }


    if (!document.getElementById("hideShippingAddress").checked) {
        if (ShippingPincode === "") {
            document.getElementById("spnShippingPincode").innerText = "Shipping Pincode is required.";
            isValid = false;
        }
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
            });
        }
    });
}

function DeleteSite(SiteId)
{
    Swal.fire({
        title: "Are you sure want to Delete This?",
        text: "You won't be able to revert this!",
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

                    Swal.fire({
                        title: Result.message,
                        icon: 'success',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK'
                    }).then(function () {
                        window.location = '/SiteMaster/SiteListView';
                    })
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
