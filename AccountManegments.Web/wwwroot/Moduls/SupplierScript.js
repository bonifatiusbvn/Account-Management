AllUserTable();
fn_getState('dropState', 1);
function CreateSupplier() {
    siteloadershow();
    if ($("#SupplierForm").valid()) {
        var objData = {
            SupplierName: $('#txtSupplierName').val(),
            Email: $('#txtEmail').val(),
            Mobile: $('#txtPhoneNo').val(),
            Gstno: $('#txtGST').val(),
            BuildingName: $('#txtBuilding').val(),
            Area: $('#txtArea').val(),
            City: $('#txtcity').val(),
            State: $('#txtstate').val(),
            PinCode: $('#txtPinCode').val(),
            BankName: $('#txtBank').val(),
            AccountNo: $('#txtAccount').val(),
            Iffccode: $('#txtIFFC').val(),
            CreatedBy: $('#txtUserid').val(),

        }
        $.ajax({
            url: '/Supplier/CreateSupplier',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                if (result.code == 200) {
                    var offcanvasElement = document.getElementById('createSupplier');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {

                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllUserTable();
                    toastr.success(result.message);
                } else {
                    toastr.error(result.message);
                }
                siteloaderhide();
            },
            error: function (xhr, status, error) {
                debugger
                siteloaderhide();
                toastr.error("An error occurred while creating Supplier");
            }
        })
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}


function ClearSupplierTextBox() {
    resetSupplierForm();
    $('#txtSupplierid').val('');
    $('#txtSupplierName').val('');
    $('#txtEmail').val('');
    $('#txtPhoneNo').val('');
    $('#txtGST').val('');
    $('#txtBuilding').val('');
    $('#txtArea').val('');
    $('#ddlCity').val('');
    $('#dropState').val('');
    $('#txtPinCode').val('');
    $('#txtBank').val('');
    $('#txtAccount').val('');
    $('#txtIFFC').val('');

    var button = document.getElementById("btnsupplier");
    if ($('#txtSupplierid').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('createSupplier'));
    offcanvas.show();
}
function DisplaySupplierDetails(SupplierId) {

    $.ajax({
        url: '/Supplier/DisplaySupplier?SupplierId=' + SupplierId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            $('#txtSupplierid').val(response.supplierId);
            $('#txtSupplierName').val(response.supplierName);
            $('#txtEmail').val(response.email);
            $('#txtPhoneNo').val(response.mobile);
            $('#txtGST').val(response.gstno);
            $('#txtArea').val(response.area);
            $('#txtBuilding').val(response.buildingName);
            fn_getcitiesbystateId('ddlCity', response.state)
            $('#dropState').val(response.state);
            $('#txtPinCode').val(response.pinCode);
            $('#txtBank').val(response.bankName);
            $('#txtAccount').val(response.accountNo);
            $('#txtIFFC').val(response.iffccode);

            setTimeout(function () { $('#ddlCity').val(response.city); }, 100)
            var button = document.getElementById("btnsupplier");
            if ($('#txtUserid').val() != '') {
                button.textContent = "Update";
            }
            var offcanvas = new bootstrap.Offcanvas(document.getElementById('createSupplier'));
            resetSupplierForm();
            offcanvas.show();
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function SelectSupplierDetails(SupplierId, element) {

    $('.row.ac-card').removeClass('active');
    $(element).closest('.row.ac-card').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/Supplier/DisplaySupplier?SupplierId=' + SupplierId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            if (response) {
                $('#dspSupplierId').val(response.supplierId);
                $('#dspSupplierName').val(response.supplierName);
                $('#dspEmail').val(response.email);
                $('#dspPhoneNo').val(response.mobile);
                $('#dspArea').val(response.area);
                $('#dspArea').val(response.area);
                $('#dspBuildingName').val(response.buildingName);
                $('#dspStateName').val(response.stateName);
                $('#dspCityName').val(response.cityName);
                $('#dspPinCode').val(response.pinCode);
                $('#dspBankName').val(response.bankName);
                $('#dspAccountNo').val(response.accountNo);
                $('#dspIffccode').val(response.iffccode);
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

function AllUserTable() {

    var searchText = $('#txtSupplierSearch').val();
    var searchBy = $('#SupplierSearchBy').val();

    $.get("/Supplier/SupplierListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {


            $("#Supplierbody").html(result);
        })
        .fail(function (error) {
            siteloaderhide();
            toastr.error(error);
        });
}

function filterSupplierTable() {
    siteloadershow();
    var searchText = $('#txtSupplierSearch').val();
    var searchBy = $('#SupplierSearchBy').val();

    $.ajax({
        url: '/Supplier/SupplierListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#Supplierbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(error);
        }
    });
}

function sortSupplierTable() {
    siteloadershow();
    var sortBy = $('#SupplierSortBy').val();
    $.ajax({
        url: '/Supplier/SupplierListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#Supplierbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(error);
        }
    });
}


function UpdateSupplierDetails() {
    siteloadershow();
    if ($("#SupplierForm").valid()) {
        var objData = {
            SupplierId: $('#txtSupplierid').val(),
            SupplierName: $('#txtSupplierName').val(),
            Email: $('#txtEmail').val(),
            Mobile: $('#txtPhoneNo').val(),
            Gstno: $('#txtGST').val(),
            BuildingName: $('#txtBuilding').val(),
            Area: $('#txtArea').val(),
            City: $('#ddlCity').val(),
            State: $('#dropState').val(),
            PinCode: $('#txtPinCode').val(),
            BankName: $('#txtBank').val(),
            AccountNo: $('#txtAccount').val(),
            Iffccode: $('#txtIFFC').val(),
            UpdatedBy: $('#txtUserid').val(),
        }
        $.ajax({
            url: '/Supplier/UpdateSupplierDetails',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                if (result.code == 200) {
                    var offcanvasElement = document.getElementById('createSupplier');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {

                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllUserTable();
                    toastr.success(result.message);
                } else {
                    toastr.error(result.message);
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
function DeleteSupplierDetails(SupplierId) {
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
                url: '/Supplier/DeleteSupplierDetails?SupplierId=' + SupplierId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {
                    siteloaderhide();
                    Swal.fire({
                        title: Result.message,
                        icon: 'success',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK'
                    }).then(function () {
                        window.location = '/Supplier/SupplierList';
                    })
                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete supplier!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Supplier have no changes.!!😊',
                'error'
            );
        }
    });
}
var SupplierForm;
function validateAndCreateSupplier() {
    SupplierForm = $("#SupplierForm").validate({
        rules: {
            txtSupplierName: "required",
            txtPhoneNo: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 10
            },
            txtEmail: {
                required: true,
                email: true
            },
            txtGST: "required",
            txtBuilding: "required",
            txtArea: "required",
            txtPinCode: {
                required: true,
                digits: true,
                minlength: 6,
                maxlength: 6
            },
            ddlCity: "required",
            dropState: "required",
            ddlCountry: "required",
            txtBank: "required",
            txtAccount: "required",
            txtIFFC: "required",
        },
        messages: {
            txtSupplierName: "Please Enter SupplierName",
            txtPhoneNo: {
                required: "Please Enter Phone Number",
                digits: "Please enter a valid 10-digit phone number",
                minlength: "Phone number must be 10 digits long",
                maxlength: "Phone number must be 10 digits long"
            },
            txtEmail: {
                required: "Please Enter Email",
                email: "Please enter a valid email address"
            },
            txtGST: "Please Enter GST",
            txtBuilding: "Please Enter Building",
            txtArea: "Please Enter Area",
            txtPinCode: {
                required: "Please Enter Pin Code",
                digits: "Pin code must contain only digits",
                minlength: "Pin code must be 6 digits long",
                maxlength: "Pin code must be 6 digits long"
            },
            ddlCity: "Please Enter City",
            dropState: "Please Enter State",
            ddlCountry: "Please Enter Country",
            txtBank: "Please Enter Bank",
            txtAccount: "Please Enter Account",
            txtIFFC: "Please Enter IFFC",
        }
    });
    var isValid = true;


    if (isValid) {
        if ($("#txtSupplierid").val() == '') {
            CreateSupplier();
        }
        else {
            UpdateSupplierDetails()
        }
    }
}

function resetSupplierForm() {
    if (SupplierForm) {
        SupplierForm.resetForm();
    }
}

function SupplierActiveDecative(SupplierId) {

    var isChecked = $('#flexSwitchCheckChecked_' + SupplierId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to active this user?" : "Are you sure want to deactive this user?";

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
            formData.append("SupplierId", SupplierId);

            $.ajax({
                url: '/Supplier/ActiveDeactiveSupplier?SupplierId=' + SupplierId,
                type: 'Post',
                contentType: 'application/json;charset=utf-8;',
                dataType: 'json',
                success: function (Result) {
                    siteloaderhide();
                    Swal.fire({
                        title: isChecked ? "Active!" : "Deactive!",
                        text: Result.message,
                        icon: "success",
                        confirmButtonClass: "btn btn-primary w-xs mt-2",
                        buttonsStyling: false
                    }).then(function () {
                        window.location = '/Supplier/SupplierList';
                    });
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            siteloaderhide();
            Swal.fire(
                'Cancelled',
                'User have no changes.!!😊',
                'error'
            );
        }
    });
}
function GetSupplierInvoiceDetailsById(SupplierId, element) {

    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');

    $.get("/InvoiceMaster/GetSupplierInvoiceDetailsById", { SupplierId: SupplierId })
        .done(function (result) {

            $("#supplierinvoicedetails").html(result);
        })
}

document.addEventListener("DOMContentLoaded", function () {

    var closeButton = document.getElementById("ErrorCloseButton");
    if (closeButton) {
        closeButton.addEventListener("click", function () {
            window.location = '/Supplier/SupplierList';
        });
    }
});

function downloadSampleFile() {
    siteloadershow();
    var fileUrl = '/uploadexcelfile/SuplierMasterDetails.xlsx';

    var link = document.createElement('a');

    link.href = fileUrl;

    link.setAttribute('download', '');

    document.body.appendChild(link);

    link.click();

    document.body.removeChild(link);
    siteloaderhide();
}

var UploadExcelFile;
$(document).ready(function () {
    UploadExcelFile = $("#uploadSupplierFile").validate({
        rules: {
            supplierExcelFile: "required"
        },
        messages: {
            supplierExcelFile: "Upload File"
        }
    })
});

function UploadSupplierFile() {
    if ($("#uploadSupplierFile").valid()) {
        var formData = new FormData();
        formData.append("FormFile", $("#supplierExcelFile")[0].files[0]);

        $.ajax({
            url: '/Supplier/ImportSupplierListFromExcel',
            type: 'POST',
            data: formData,
            dataType: 'json',
            processData: false,
            contentType: false,
            success: function (Result) {
                siteloaderhide();
                if (Result.code == 200) {
                    toastr.success(Result.message);
                    setTimeout(function () {
                        window.location = '/Supplier/SupplierList';
                    }, 2000);
                } else {
                    toastr.error(Result.message);
                }
            },
            error: function (xhr, status, error) {
                siteloaderhide();
                toastr.error("An error occurred while uploading the file: " + error);
            }
        });
    } else {
        siteloaderhide();
        toastr.error("Kindly upload a file.");
    }
}


function ResetButton() {
    if (UploadExcelFile) {
        UploadExcelFile.resetForm();
    }
}

function clearExcelFileModel() {
    ResetButton();
    $("#supplierExcelFile").val('');
}
function SupplierFileUploadModel() {
    clearExcelFileModel();
    $("#uploadFileModal").modal('show');
}