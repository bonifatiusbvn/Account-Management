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
            toastr.error(error);
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
            toastr.error(error);
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
            toastr.error(error);
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
            $('#changeName').html('Update Site');
            $('#txtSiteid').val(response.siteId);
            $('#txtSiteName').val(response.siteName);
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

            $('#shippingAddressTable').empty();

            if (response.siteShippingAddresses == null) {

            } else {
                $.each(response.siteShippingAddresses, function (i, data) {

                    var shippingAddressNumber = i + 1;

                    $('#shippingAddressTable').append(

                        '<div class="col-12 mb-2" id="shippingAddressContainer-' + shippingAddressNumber + '" style="padding: 20px;">' +
                        '<label class="form-label">Shipping Address</label>' +
                        '<div class="row">' +
                        '<div class="col-11">' +
                        '<textarea class="form-control mb-2" rows="3" placeholder="Shipping Address" id="shippingAddressContainer-' + shippingAddressNumber + '" name="txtShippingAddress-' + shippingAddressNumber + '">' + data.address + '</textarea>' +
                        '</div>' +
                        '<div class="col-1" style="position:relative; right:16px; top:22px;">' +
                        '<a id="remove" class="btn text-primary" onclick="removeItem(this)"><i class="lni lni-trash"></i></a>' +
                        '</div>' +
                        '</div>' +

                        '</div>'
                    );
                });
            }



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
            toastr.error(xhr.responseText);
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
                toastr.error('Empty response received.');
            }
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function CreateSite() {
    siteloadershow();

    if ($("#siteForm").valid()) {
        var shippingAddressDetails = [];

        var isValidProduct = true;
        $('#shippingAddressTable textarea').each(function () {
            var address = $(this);
            var addressData = {
                Address: address.val()
            };
            address.on('input', function () {
                address.css("border", "1px solid #ced4da");
            });

            if (addressData.Address === "") {
                isValidProduct = false;
                address.css("border", "2px solid red");
                siteloaderhide();
                toastr.error("Kindly fill Multiple Site Address");
                return false;
            }

            shippingAddressDetails.push(addressData);
        });

        if (!isValidProduct) {
            return;
        }

        var objData = {
            SiteName: $('#txtSiteName').val(),
            ContectPersonName: $('#txtContectPersonName').val(),
            ContectPersonPhoneNo: $('#txtContectPersonPhoneNo').val(),
            Address: $('#txtAddress').val(),
            Area: $('#txtArea').val(),
            CityId: $('#ddlCity').val(),
            StateId: $('#stateDropdown').val(),
            Country: $('#ddlCountry').val(),
            Pincode: $('#txtPincode').val(),
            CreatedBy: $("#txtSiteUserId").val(),
            ShippingAddress: $('#hideShippingAddress').is(':checked') ? $('#txtAddress').val() : $('#txtShippingAddress').val(),
            ShippingArea: $('#hideShippingAddress').is(':checked') ? $('#txtArea').val() : $('#txtShippingArea').val(),
            ShippingPincode: $('#hideShippingAddress').is(':checked') ? $('#txtPincode').val() : $('#txtShippingPincode').val(),
            ShippingCityId: $('#hideShippingAddress').is(':checked') ? $('#ddlCity').val() : $('#ShippingCity').val(),
            ShippingStateId: $('#hideShippingAddress').is(':checked') ? $('#stateDropdown').val() : $('#ShippingState').val(),
            ShippingCountry: $('#hideShippingAddress').is(':checked') ? $('#ddlCountry').val() : $('#shippingCountry').val(),
            SiteShippingAddresses: shippingAddressDetails
        };

        if (objData.CityId === "--Select City--" || objData.StateId === "--Select State--") {
            siteloaderhide();
            toastr.error("Kindly fill all details");
            return;
        }

        $.ajax({
            url: '/SiteMaster/CreateSite',
            type: 'post',
            data: objData,
            dataType: 'json',
            success: function (Result) {
                if (Result.code === 200) {
                    var offcanvasElement = document.getElementById('createSite');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {
                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllSiteListTable();
                    toastr.success(Result.message);
                } else {
                    toastr.error(Result.message);
                }
            },
            complete: function () {
                siteloaderhide();
            }
        });

    } else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}


function UpdateSiteDetails() {
    siteloadershow();
    if ($("#siteForm").valid()) {
        var shippingAddressDetails = [];

        var isValidProduct = true;
        $('#shippingAddressTable textarea').each(function () {
            var address = $(this);
            var addressData = {
                Address: address.val()
            };
            address.on('input', function () {
                address.css("border", "1px solid #ced4da");
            });

            if (addressData.Address === "") {
                isValidProduct = false;
                address.css("border", "2px solid red");
                siteloaderhide();
                toastr.error("Kindly fill Multiple Site Address");
                return false;
            }

            shippingAddressDetails.push(addressData);
        });

        if (!isValidProduct) {
            return;
        }
        var objData = {
            SiteId: $('#txtSiteid').val(),
            SiteName: $('#txtSiteName').val(),
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
            SiteShippingAddresses: shippingAddressDetails,
        };
        if (objData.CityId == "--Select City--" || objData.StateId == "--Select State--") {
            siteloaderhide();
            toastr.error("Kindly fill all details");
        }
        else {
            $.ajax({
                url: '/SiteMaster/UpdateSiteDetails',
                type: 'post',
                data: objData,
                datatype: 'json',
                success: function (Result) {
                    if (Result.code == 200) {
                        var offcanvasElement = document.getElementById('createSite');
                        var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                        if (offcanvas) {
                            offcanvas.hide();
                        } else {

                            offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                            offcanvas.hide();
                        }

                        AllSiteListTable();
                        toastr.success(Result.message);
                    } else {
                        toastr.error(Result.message);
                    }
                    siteloaderhide();
                }
            })
        }
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}

function ClearSiteTextBox() {

    resetSiteForm();

    $('#changeName').html('Create Site');
    $('#txtSiteid').val('');
    $('#txtSiteName').val('');
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
    $('#shippingAddressTable').empty();


    var button = document.getElementById("btnSite");
    if ($('#txtSiteid').val() == '') {
        button.textContent = "Create";
    }

    var offcanvas = new bootstrap.Offcanvas(document.getElementById('createSite'));
    offcanvas.show();
}

var sForm;
function validateAndCreateSite() {

    $.validator.addMethod("notZero", function (value, element) {
        return value !== "0";
    }, "Please select a valid option");


    sForm = $("#siteForm").validate({

        rules: {

            txtSiteName: "required",
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
                digits: true,
                minlength: 6,
                maxlength: 6
            },
            ddlCountry: "required",
            stateDropdown: {
                required: true,
                notZero: true
            },
            ddlCity: {
                required: true,
                notZero: true
            },
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
            txtSiteName: "Please Enter siteName",
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
                digits: "Pin code must contain only digits",
                minlength: "Pin code must be 6 digits long",
                maxlength: "Pin code must be 6 digits long"
            },
            ddlCountry: "Please Enter Country",
            stateDropdown: {
                required: "Please Enter state",
                notZero: "Please select a valid state"
            },
            ddlCity: {
                required: "Please Enter City",
                notZero: "Please select a valid city"
            },
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
    var confirmationMessage = isChecked ? "Are you sure want to active this site?" : "Are you sure want to deactive this site?";

    Swal.fire({
        title: confirmationMessage,
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, enter it!",
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
                        siteloaderhide();
                        Swal.fire({
                            title: isChecked ? "Active!" : "Deactive!",
                            text: Result.message,
                            icon: "success",
                            confirmButtonClass: "btn btn-primary w-xs mt-2",
                            buttonsStyling: false
                        }).then(function () {
                            window.location = '/SiteMaster/SiteListView';
                        });
                    } else {
                        siteloaderhide();
                        toastr.error(Result.message);
                    }

                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Site have no changes.!!😊',
                'error'
            );
            AllSiteListTable();
        }
    });
}

function DeleteSite(SiteId) {
    Swal.fire({
        title: "If you want to delete this site,delete all data related this site!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
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
                        siteloaderhide();
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
                        siteloaderhide();
                        toastr.error(Result.message);
                    }
                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete site!");
                    window.location = '/SiteMaster/SiteListView';
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Site have no changes.!!😊',
                'error'
            );
            AllSiteListTable();
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
$(document).ready(function () {
    let shippingAddressCount = 1;
    const maxShippingAddresses = 11;

    $('#btnaddmoresite').click(function (e) {
        e.preventDefault();

        const lastTextarea = $(`#txtShippingAddress-${shippingAddressCount}`);
        if (lastTextarea.length && lastTextarea.val().trim() === '') {
            toastr.error("Kindly fill the current Shipping Address before adding a new one.");
            return;
        }

        if (shippingAddressCount < maxShippingAddresses) {
            shippingAddressCount++;
            const newShippingAddress = `
                <div class="col-12 mb-2" id="shippingAddressContainer-${shippingAddressCount}" style="padding: 20px;">
                    <label class="form-label">Shipping Address</label>
                    <div class="row">
                        <div class="col-11">
                            <textarea class="form-control mb-2" rows="3" placeholder="Shipping Address" id="txtShippingAddress-${shippingAddressCount}" name="txtShippingAddress-${shippingAddressCount}"></textarea>
                        </div>
                        <div class="col-1" style="position:relative;right:16px;top:22px;">
                            <a id="remove" class="btn text-primary" onclick="removeItem(this)"><i class="lni lni-trash"></i></a>
                        </div>
                    </div>
                </div>`;
            $('#shippingAddressTable').append(newShippingAddress);
        } else {
            toastr.warning("You can add up to 10 shipping addresses only.");
        }
    });

    window.removeItem = function (btn) {
        $(btn).closest('.col-12').remove();
        updateShippingAddressNumbers();
    };

    function updateShippingAddressNumbers() {
        $('#shippingAddressTable .col-12').each(function (index) {
            const newIndex = index + 1;
            $(this).attr('id', `shippingAddressContainer-${newIndex}`);
            $(this).find('.form-label').text(`Shipping Address ${newIndex}`);
            $(this).find('textarea').attr('id', `txtShippingAddress-${newIndex}`).attr('name', `txtShippingAddress-${newIndex}`);
        });
        shippingAddressCount = $('#shippingAddressTable .col-12').length;
    }
});





function toggleShippingAddress() {
    var checkbox = document.getElementById("hideShippingAddress");
    var shippingFields = document.getElementById("shippingAddressFields");

    if (checkbox.checked) {
        shippingFields.style.display = "none";
    } else {
        shippingFields.style.display = "block";
    }
}
