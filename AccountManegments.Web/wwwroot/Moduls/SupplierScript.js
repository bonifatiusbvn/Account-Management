AllUserTable();
fn_getState('dropState', 1);
function CreateSupplier() {

    if ($("#SupplierForm").valid())
    {
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

                Swal.fire({
                    title: Result.message,
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(function () {
                    window.location = '/Supplier/SupplierList';
                });
            },
        })
    }
    else {
        Swal.fire({
            title: "Kindly Fill All Datafield",
            icon: 'warning',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'OK',
        })
    }
}


function ClearSupplierTextBox() {
    $('#dspSupplierId').val('');
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
    $('#ddlCountry').val('');


    var button = document.getElementById("btnsupplier");
    if ($('#dspSupplierId').val() == '') {
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
            offcanvas.show();
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
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
                console.log('Empty response received.');
            }
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
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
            console.error(error);
        });
}

function filterSupplierTable() {

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
            $("#Supplierbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function sortSupplierTable() {
    var sortBy = $('#SupplierSortBy').val();
    $.ajax({
        url: '/Supplier/SupplierListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            $("#Supplierbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}


function UpdateSupplierDetails() {

    var objData = {
        SupplierId: $('#txtSupplierid').val(),
        SupplierName: $('#txtSupplierName').val(),
        Email: $('#txtEmail').val(),
        Mobile: $('#txtPhoneNo').val(),
        Gstno: $('#txtGST').val(),
        BuildingName: $('#txtBuilding').val(),
        Area: $('#txtArea').val(),
        City: $('#txtCity').val(),
        State: $('#txtState').val(),
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

            Swal.fire({
                title: Result.message,
                icon: 'success',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK'
            }).then(function () {
                window.location = '/Supplier/SupplierList';
            });
        },
    })

}
function DeleteSupplierDetails(SupplierId) {
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
                url: '/Supplier/DeleteSupplierDetails?SupplierId=' + SupplierId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {
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
                    Swal.fire({
                        title: "Can't Delete Supplier!",
                        icon: 'warning',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK',
                    }).then(function () {
                        window.location = '/Supplier/SupplierList';
                    })
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Supplier Have No Changes.!!😊',
                'error'
            );
        }
    });
}
//function validateAndCreateSupplier() {


//    resetErrorMessages();

//    var supplierName = $('#txtSupplierName').val().trim();
//    var email = $('#txtEmail').val().trim();
//    var phoneNo = $('#txtPhoneNo').val().trim();
//    var GST = $('#txtGST').val().trim();
//    var building = $('#txtBuilding').val().trim();
//    var area = $('#txtArea').val().trim();
//    var pinCode = $('#txtPinCode').val().trim();
//    var bank = $('#txtBank').val().trim();
//    var account = $('#txtAccount').val().trim();
//    var IFFC = $('#txtIFFC').val().trim();


//    var isValid = true;

//    if (supplierName === "") {
//        document.getElementById("spnSupplier").innerText = "Supplier Name is required.";
//        isValid = false;
//    }

//    if (email === "") {
//        document.getElementById("spnEmail").innerText = "Email is required.";
//        isValid = false;
//    } else if (!isValidEmail(email)) {
//        document.getElementById("spnEmail").innerText = "Invalid Email format.";
//        isValid = false;
//    }

//    if (phoneNo === "") {
//        document.getElementById("spnPhoneNo").innerText = "Phone Number is required.";
//        isValid = false;
//    } else if (!isValidPhoneNo(phoneNo)) {
//        document.getElementById("spnPhoneNo").innerText = "Invalid Phone Number format.";
//        isValid = false;
//    }

//    if (GST === "") {
//        document.getElementById("spnGST").innerText = "GST Number is required.";
//        isValid = false;
//    }

//    if (building === "") {
//        document.getElementById("spnBuilding").innerText = "Building Name is required.";
//        isValid = false;
//    }

//    if (area === "") {
//        document.getElementById("spnArea").innerText = "Area is required.";
//        isValid = false;
//    }

//    if (pinCode === "") {
//        document.getElementById("spnPinCode").innerText = "PinCode is required.";
//        isValid = false;
//    }

//    if (bank === "") {
//        document.getElementById("spnBank").innerText = "Bank Name is required.";
//        isValid = false;
//    }

//    if (account === "") {
//        document.getElementById("spnAccount").innerText = "Account Number is required.";
//        isValid = false;
//    }

//    if (IFFC === "") {
//        document.getElementById("spnIFFC").innerText = "IFFC Code is required.";
//        isValid = false;
//    }


//    if (isValid) {
//        if ($("#txtSupplierid").val() == '') {
//            CreateSupplier();
//        }
//        else {
//            UpdateSupplierDetails()
//        }
//    }

//}
$(document).ready(function () {

    $("#SupplierForm").validate({
        rules: {
            txtSupplierName: "required",
            txtPhoneNo: "required",
            txtEmail: "required",
            txtGST: "required",
            txtBuilding: "required",
            txtArea: "required",
            txtPinCode: "required",
            ddlCity: "required",
            dropState: "required",
            ddlCountry: "required",
            txtBank: "required",
            txtAccount: "required",
            txtIFFC: "required",
        },
        messages: {
            txtSupplierName: "Please Enter SupplierName",
            txtPhoneNo: "Please Enter PhoneNo",
            txtEmail: "Please Enter Email",
            txtGST: "Please Enter GST",
            txtBuilding: "Please Enter Building",
            txtArea: "Please Enter Area",
            txtPinCode: "Please Enter PinCode",
            ddlCity: "Please Enter City",
            dropState: "Please Enter State",
            ddlCountry: "Please Enter Country",
            txtBank: "Please Enter Bank",
            txtAccount: "Please Enter Account",
            txtIFFC: "Please Enter IFFC",
        }
    })
});

function resetErrorMessages() {
    document.getElementById("spnSupplier").innerText = "";
    document.getElementById("spnEmail").innerText = "";
    document.getElementById("spnPhoneNo").innerText = "";
    document.getElementById("spnGST").innerText = "";
    document.getElementById("spnBuilding").innerText = "";
    document.getElementById("spnArea").innerText = "";
    document.getElementById("spnPinCode").innerText = "";
    document.getElementById("spnBank").innerText = "";
    document.getElementById("spnAccount").innerText = "";
    document.getElementById("spnIFFC").innerText = "";
}

function isValidEmail(email) {
    var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailPattern.test(email);
}

function isValidPhoneNo(phoneNo) {
    var phoneNoPattern = /^\d{10}$/;
    return phoneNoPattern.test(phoneNo);
}


function UserActiveDecative(UserId) {

    var isChecked = $('#flexSwitchCheckChecked_' + UserId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to Active this User?" : "Are you sure want to DeActive this User?";

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
            formData.append("UserId", UserId);

            $.ajax({
                url: '/User/UserActiveDecative?UserId=' + UserId,
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
                        window.location = '/User/UserListView';
                    });
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'User Have No Changes.!!😊',
                'error'
            ).then(function () {
                window.location = '/User/UserListView';
            });;
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