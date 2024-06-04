AllCompanyTable();
fn_getState('dropState', 1);
function AllCompanyTable() {

    var searchText = $('#txtSearch').val();
    var searchBy = $('#ddlSearchBy').val();

    $.get("/Company/GetAllCompanyDetails", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {

            $("#Companytbody").html(result);
        })
        .fail(function (error) {
            siteloaderhide();
            toastr.error(error);
        });
}

function filterCompanyTable() {
    siteloadershow();
    var searchText = $('#txtSearch').val();
    var searchBy = $('#ddlSearchBy').val();

    $.ajax({
        url: '/Company/GetAllCompanyDetails',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#Companytbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function sortCompanyTable() {
    siteloadershow();
    var sortBy = $('#ddlSortBy').val();
    $.ajax({
        url: '/Company/GetAllCompanyDetails',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#Companytbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}
function AddCompany() {
    siteloadershow();
    if ($("#companyForm").valid()) {
        var objData = {
            CompanyName: $('#txtCompanyName').val(),
            Gstno: $('#txtGstNo').val(),
            PanNo: $('#txtPanNo').val(),
            Address: $('#txtAddress').val(),
            Area: $('#txtArea').val(),
            CityId: $('#ddlCity').val(),
            StateId: $('#dropState').val(),
            Country: $('#ddlCountry').val(),
            Pincode: $('#txtPincode').val(),
            InvoicePef : $('#txtInvoicePrefix').val(),
        }
        $.ajax({
            url: '/Company/AddCompany',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                if (Result.code == 200) {
                    var offcanvasElement = document.getElementById('createCompany');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {

                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllCompanyTable();
                    toastr.success(Result.message);
                } else {
                    toastr.error(Result.message);
                }
                siteloaderhide();

            },

        })
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}
function ClearTextBox() {
    resetCompanyForm();
    $('#txtCompanyid').val('');
    $('#txtCompanyName').val('');
    $('#txtGstNo').val('');
    $('#txtPanNo').val('');
    $('#txtAddress').val('');
    $('#txtArea').val('');
    $('#ddlCity').val('');
    $('#dropState').val('');
    $('#txtPincode').val('');
    $('#txtInvoicePrefix').val('');
    var button = document.getElementById("btncompany");
    if ($('#txtCompanyid').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('createCompany'));
    offcanvas.show();
}
function GetCompnaytById(CompanyId) {
    siteloadershow();
    $.ajax({
        url: '/Company/GetCompnaytById?CompanyId=' + CompanyId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            $('#txtCompanyid').val(response.companyId);
            $('#txtCompanyName').val(response.companyName);
            $('#txtGstNo').val(response.gstno);
            $('#txtPanNo').val(response.panNo);
            $('#txtAddress').val(response.address);
            $('#txtArea').val(response.area);
            fn_getcitiesbystateId('ddlCity', response.stateId)
            $('#dropState').val(response.stateId);
            $('#txtCountry').val(response.countryId);
            $('#txtPincode').val(response.pincode);
            $("#txtInvoicePrefix").val(response.invoicePef),

            setTimeout(function () { $('#ddlCity').val(response.cityId); }, 100)

            var button = document.getElementById("btncompany");
            if ($('#txtCompanyid').val() != '') {
                button.textContent = "Update";
            }
            var offcanvas = new bootstrap.Offcanvas(document.getElementById('createCompany'));
            resetCompanyForm();
            offcanvas.show();
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}
function SelectCompanyDetails(CompanyId, element) {
    siteloadershow();
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/Company/GetCompnaytById?CompanyId=' + CompanyId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response) {
                $('#dspCompanyid').val(response.companyId);
                $('#dspCompanyName').val(response.companyName);
                $('#dspGstNo').val(response.gstno);
                $('#dspPanNo').val(response.panNo);
                $('#dspAddress').val(response.address);
                $('#dspArea').val(response.area);
                $('#dspCity').val(response.cityName);
                $('#dspState').val(response.stateName);
                $('#dspCountry').val(response.countryName);
                $('#dspPincode').val(response.pincode);
                $('#dspInvoicePrefix').val(response.invoicePef);
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
function UpdateCompany() {
    siteloadershow();
    if ($("#companyForm").valid()) {
        var objData = {
            CompanyId: $('#txtCompanyid').val(),
            CompanyName: $('#txtCompanyName').val(),
            Gstno: $('#txtGstNo').val(),
            PanNo: $('#txtPanNo').val(),
            Address: $('#txtAddress').val(),
            Area: $('#txtArea').val(),
            CityId: $('#ddlCity').val(),
            StateId: $('#dropState').val(),
            Country: $('#ddlCountry').val(),
            Pincode: $('#txtPincode').val(),
            InvoicePef: $("#txtInvoicePrefix").val(),
        }
        $.ajax({
            url: '/Company/UpdateCompany',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                if (Result.code == 200) {
                    var offcanvasElement = document.getElementById('createCompany');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {

                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllCompanyTable();
                    toastr.success(Result.message);
                } else {
                    toastr.error(Result.message);
                }
                siteloaderhide();
            },
        })
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}
function DeleteCompanyDetails(CompanyId) {
    Swal.fire({
        title: "Are you sure want to delete this?",
        text: "You won't be able to revert this!",
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
                url: '/Company/DeleteCompanyDetails?CompanyId=' + CompanyId,
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
                            window.location = '/Company/CreateCompany';
                        })
                    }
                    else {
                        siteloaderhide();
                        toastr.error(Result.message);
                    }

                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete company!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Company have no changes.!!😊',
                'error'
            );
        }
    });
}

var CompanyForm;
function validateAndCreateCompany() {
    CompanyForm = $("#companyForm").validate({
        rules: {
            txtCompanyName: "required",
            txtGstNo: "required",
            txtPanNo: "required",
            txtAddress: "required",
            txtArea: "required",
            txtPincode: {
                required: true,
                digits: true,
                minlength: 6,
                maxlength: 6
            },
            ddlCity: "required",
            dropState: "required",
            ddlCountry: "required",
            txtInvoicePrefix: "required",
        },
        messages: {
            txtCompanyName: "Please Enter CompanyName",
            txtGstNo: "Please Enter GstNo",
            txtPanNo: "Please Enter PanNo",
            txtAddress: "Please Enter Address",
            txtArea: "Please Enter Area",
            txtPincode: {
                required: "Please Enter Pin Code",
                digits: "Pin code must contain only digits",
                minlength: "Pin code must be 6 digits long",
                maxlength: "Pin code must be 6 digits long"
            },
            ddlCity: "Please Enter City",
            dropState: "Please Enter State",
            ddlCountry: "Please Enter Country",
            txtInvoicePrefix: "Please Enter Invoice Prefix",
        }
    })
    var isValid = true;

    if (isValid) {
        if ($("#txtCompanyid").val() == '') {
            AddCompany();
        }
        else {
            UpdateCompany()
        }
    }
}
function resetCompanyForm() {
    if (CompanyForm) {
        CompanyForm.resetForm();
    }
}






