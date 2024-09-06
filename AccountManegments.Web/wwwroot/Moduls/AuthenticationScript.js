AllUserTable();
GetSiteDetails();
GetDashboardItemList();
GetDashboardPurchaseOrderList();
GetDashboardInvoiceList();
function CreateUser() {
    siteloadershow();
    if ($("#userForm").valid()) {
        var objData = {
            FirstName: $('#txtFirstName').val(),
            LastName: $('#txtLastName').val(),
            UserName: $('#txtUserName').val(),
            Password: $('#txtPassword').val(),
            Role: $('#txtrole').val(),
            Email: $('#txtEmail').val(),
            PhoneNo: $('#txtPhoneNo').val(),
            SiteId: $('#ddlSiteName').val(),
        };
        $.ajax({
            url: '/User/CreateUser',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                if (Result.code == 200) {
                    var offcanvasElement = document.getElementById('createUser');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {

                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllUserTable();
                    toastr.success(Result.message);
                } else {
                    toastr.error(Result.message);
                }
                siteloaderhide();
            }
        });
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}
function GetSiteDetails() {
    $.ajax({
        url: '/SiteMaster/GetSiteNameList',
        success: function (result) {
            $('#ddlSiteName').empty();

            $('#ddlSiteName').append('<option value="" selected>All Site</option>');
            $.each(result, function (i, data) {

                $('#ddlSiteName').append('<option value=' + data.siteId + '>' + data.siteName + '</Option>')

            });
        }
    });
}


function ClearUserTextBox() {
    resetUserForm();
    $('#changeName').html('Create User');
    $('#txtUserid').val('');
    $('#txtFirstName').val('');
    $('#txtLastName').val('');
    $('#txtUserName').val('');
    $('#txtPassword').val('');
    $('#txtEmail').val('');
    $('#txtrole').val('');
    $('#txtPhoneNo').val('');
    $('#ddlUserRole').val('');
    $('#ddlSiteName').val('');
    var button = document.getElementById("btnuser");
    if ($('#txtUserid').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('createUser'));
    offcanvas.show();
}

function DisplayUserDetails(UserId) {
    siteloadershow();
    $.ajax({
        url: '/User/DisplayUserDetails?UserId=' + UserId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            $('#changeName').html('Update User');
            $('#txtUserid').val(response.id);
            $('#txtFirstName').val(response.firstName);
            $('#txtLastName').val(response.lastName);
            $('#txtPassword').val(response.password);
            $('#txtUserName').val(response.userName);
            $('#txtEmail').val(response.email);
            $('#txtPhoneNo').val(response.phoneNo);
            $('#ddlSiteName').val(response.siteId);
            $('#ddlUserRole').val(response.roleId);
            var button = document.getElementById("btnuser");
            if ($('#txtUserid').val() != '') {
                button.textContent = "Update";
            }
            var offcanvas = new bootstrap.Offcanvas(document.getElementById('createUser'));
            resetUserForm()
            offcanvas.show();
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}

function SelectUserDetails(UserId, element) {
    siteloadershow();
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/User/DisplayUserDetails?UserId=' + UserId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response) {
                $('#dspUserid').val(UserId);
                $('#dspFirstName').val(response.firstName);
                $('#dspLastName').val(response.lastName);
                $('#dspUserName').val(response.userName);
                $('#dspPassword').val(response.password);
                $('#dspEmail').val(response.email);
                $('#dspPhoneNo').val(response.phoneNo);
                $('#dspRole').val(response.roleName);
                $('#dspSiteName').val(response.siteName);
            } else {
                siteloaderhide();
                toastr.error('Empty response received.');
            }
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}

function AllUserTable() {

    var searchText = $('#txtSearch').val();
    var searchBy = $('#ddlSearchBy').val();

    $.get("/User/UserListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {
            $("#Usertbody").html(result);
        })
        .fail(function (error) {

        });
}

function filterTable() {
    siteloadershow();
    var searchText = $('#txtSearch').val();
    var searchBy = $('#ddlSearchBy').val();

    $.ajax({
        url: '/User/UserListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#Usertbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}

function sortTable() {
    siteloadershow();
    var sortBy = $('#ddlSortBy').val();
    $.ajax({
        url: '/User/UserListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#Usertbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}


function UpdateUserDetails() {
    siteloadershow();
    if ($("#userForm").valid()) {
        var objData = {
            Id: $('#txtUserid').val(),
            FirstName: $('#txtFirstName').val(),
            LastName: $('#txtLastName').val(),
            UserName: $('#txtUserName').val(),
            Password: $('#txtPassword').val(),
            Role: $('#ddlUserRole').val(),
            Email: $('#txtEmail').val(),
            PhoneNo: $('#txtPhoneNo').val(),
            SiteId: $('#ddlSiteName').val(),
        }
        $.ajax({
            url: '/User/UpdateUserDetails',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                if (Result.code == 200) {
                    var offcanvasElement = document.getElementById('createUser');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {

                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllUserTable();
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
var UserForm;
function validateAndCreateUser() {

    if ($('#ddlUserRole').val() == 3) {
        UserForm = $("#userForm").validate({
            rules: {
                txtFirstName: "required",
                txtLastName: "required",
                txtUserName: "required",
                txtPassword: "required",
                txtEmail: {
                    required: true,
                    email: true
                },
                txtPhoneNo: {
                    required: true,
                    digits: true,
                    minlength: 10,
                    maxlength: 10
                },
                ddlUserRole: "required",
            },
            messages: {
                txtFirstName: "FirstName is Required!",
                txtLastName: "Lastname is Required!",
                txtUserName: "Username is Required!",
                txtPassword: "Password is Required!",
                txtEmail: {
                    required: "Please Enter Email",
                    email: "Please enter a valid email address"
                },
                txtPhoneNo: {
                    required: "Please Enter Phone Number",
                    digits: "Please enter a valid 10-digit phone number",
                    minlength: "Phone number must be 10 digits long",
                    maxlength: "Phone number must be 10 digits long"
                },
                ddlUserRole: "Select User Role!",
            }
        })
    }
    else {
        UserForm = $("#userForm").validate({
            rules: {
                txtFirstName: "required",
                txtLastName: "required",
                txtUserName: "required",
                txtPassword: "required",
                txtEmail: {
                    required: true,
                    email: true
                },
                txtPhoneNo: {
                    required: true,
                    digits: true,
                    minlength: 10,
                    maxlength: 10
                },
                ddlUserRole: "required",

            },
            messages: {
                txtFirstName: "FirstName is Required!",
                txtLastName: "Lastname is Required!",
                txtUserName: "Username is Required!",
                txtPassword: "Password is Required!",
                txtEmail: {
                    required: "Please Enter Email",
                    email: "Please enter a valid email address"
                },
                txtPhoneNo: {
                    required: "Please Enter Phone Number",
                    digits: "Please enter a valid 10-digit phone number",
                    minlength: "Phone number must be 10 digits long",
                    maxlength: "Phone number must be 10 digits long"
                },
                ddlUserRole: "Select User Role!",

            }
        })
    }

    if ($("#userForm").valid()) {
        if ($("#txtUserid").val() == '') {
            CreateUser();
        }
        else {
            UpdateUserDetails()
        }
    }
}
function resetUserForm() {
    if (UserForm) {
        UserForm.resetForm();
    }
}

function UserActiveDecative(UserId) {

    var isChecked = $('#flexSwitchCheckChecked_' + UserId).is(':checked');
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
            formData.append("UserId", UserId);

            $.ajax({
                url: '/User/UserActiveDecative?UserId=' + UserId,
                type: 'Post',
                contentType: 'application/json;charset=utf-8;',
                dataType: 'json',
                success: function (Result) {
                    siteloaderhide();
                    if (Result.code == 200) {
                        siteloaderhide();
                        Swal.fire({
                            title: isChecked ? "Active!" : "Deactive!",
                            text: Result.message,
                            icon: "success",
                            confirmButtonClass: "btn btn-primary w-xs mt-2",
                            buttonsStyling: false
                        }).then(function () {
                            window.location = '/User/UserListView';
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
                'User have no changes.!!😊',
                'error'
            ).then(function () {
                window.location = '/User/UserListView';
            });;
        }
    });
}

function UserLogout() {
    Swal.fire({
        title: 'Logout Confirmation',
        text: 'Are you sure you want to logout?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, logout'
    }).then((result) => {
        if (result.isConfirmed) {

            logout();
        }
    });
}

function logout() {
    sessionStorage.removeItem('SelectedSiteId');
    fetch('/Authentication/Logout', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': '@Token.Get(Request.HttpContext)'

        },
        body: ''
    })
        .then(response => {

            window.location.href = '/Authentication/UserLogin';
        })
        .catch(error => {
            siteloaderhide();
            toastr.error('Error:', error);

        });
}
function deleteUserDetails(UserId) {
    siteloadershow();
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
                url: '/User/DeleteUserDetails?UserId=' + UserId,
                type: 'GET',
                dataType: 'json',
                success: function (Result) {
                    siteloaderhide();
                    if (Result.code == 200) {
                        siteloaderhide();
                        Swal.fire({
                            title: Result.message,
                            icon: 'success',
                            confirmButtonColor: '#3085d6',
                            confirmButtonText: 'OK'
                        }).then(function () {
                            window.location = '/User/UserListView';
                        })
                    }
                    else {
                        siteloaderhide();
                        toastr.error(Result.message);
                    }
                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete user!");
                }
            })
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

$(document).on("click", ".plus", function () {

    updateProductQuantity($(this).closest(".product"), 1);
    return
});

$(document).on("click", ".minus", function () {

    updateProductQuantity($(this).closest(".product"), -1);
    return
});
function preventEmptyValue(input) {

    if (input.value === "") {

        input.value = 1;
    }
}

//$(document).ready(function () {

//    bindEventListeners();

//    $(document).on('input', '.product-quantity', function () {
//        var row = $(this).closest('.product');
//        updateProductTotalAmount(row);
//        updateTotals();
//    });


//    $(document).on('keydown', '.product-quantity', function (event) {
//        if (event.key === 'Enter') {
//            $(this).blur();
//        }
//    });


//    $(document).on('input', '#txtproductamount', function () {
//        var row = $(this).closest('.product');
//        updateProductTotalAmount(row);
//        updateTotals();
//    });


//    $(document).on('keydown', '#txtproductamount', function (event) {
//        if (event.key === 'Enter') {
//            $(this).blur();
//        }
//    });

//    $(document).on('input', '#txtgst', function () {
//        var row = $(this).closest('.product');
//        updateProductTotalAmount(row);
//        updateTotals();
//    });

//    $(document).on('keydown', '#txtgst', function (event) {
//        if (event.key === 'Enter') {
//            $(this).blur();
//        }
//    });

//    $(document).on('keydown', '#txtdiscountamount', function (event) {
//        if (event.key === 'Enter') {
//            $(this).blur();
//        }
//    });

//    $(document).on('focusout', '.product-quantity', function () {
//        $(this).trigger('input');
//    });


//});

function GetDashboardItemList() {
    $.get("/Home/ItemListAction")
        .done(function (result) {

            $("#tbItemsList").html(result);
        })
        .fail(function (error) {
            siteloaderhide();

        });
}
function dashboardItemIsApproved() {
    Swal.fire({
        title: "Are you sure you want to approve this item?",
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
            var approvalStatuses = [];
            $('input[name="chk_child"]').each(function () {
                var itemId = $(this).attr('id').split('_')[1];
                var isChecked = $(this).is(':checked');
                approvalStatuses.push({ ItemId: itemId, IsApproved: isChecked });
            });


            var ItemDetails = {
                ItemList: approvalStatuses
            };

            var form_data = new FormData();
            form_data.append("ItemIsApproved", JSON.stringify(ItemDetails));

            $.ajax({
                url: '/Home/MutipleItemsIsApproved',
                type: 'Post',
                processData: false,
                contentType: false,
                data: form_data,
                success: function (Result) {
                    if (Result.code == 200) {
                        siteloaderhide();
                        Swal.fire({
                            text: Result.message,
                            icon: "success",
                            confirmButtonClass: "btn btn-primary w-xs mt-2",
                            buttonsStyling: false
                        }).then(function () {
                            window.location = '/Home/Index';
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
                'Item have no changes.!!😊',
                'error'
            )
        }
    });
}

function dashboarddeleteItemDetails(ItemId) {

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
                url: '/ItemMaster/DeleteItemDetails?ItemId=' + ItemId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {
                    siteloaderhide();
                    if (Result.code == 200) {
                        siteloaderhide();
                        Swal.fire({
                            title: Result.message,
                            icon: 'success',
                            confirmButtonColor: '#3085d6',
                            confirmButtonText: 'OK'
                        }).then(function () {
                            window.location = '/Home/Index';
                        });
                    } else {
                        siteloaderhide();
                        toastr.error(Result.message);
                    }
                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete item!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Item have no changes.!!😊',
                'error'
            );
        }
    });
}
function GetDashboardPurchaseOrderList() {

    $.get("/Home/PurchaseOrderListView")
        .done(function (result) {
            $("#dashboardPOList").html(result);
        })
}
function dashboardPOIsApproved() {
    Swal.fire({
        title: "Are you sure you want to approve this purchase order?",
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
            var approvalStatuses = [];
            $('input[name="chk_POchild"]').each(function () {
                var Id = $(this).attr('id').split('_')[1];
                var isChecked = $(this).is(':checked');
                approvalStatuses.push({ Id: Id, IsApproved: isChecked });
            });

            var PODetails = {
                POList: approvalStatuses
            };

            var form_data = new FormData();
            form_data.append("POIsApproved", JSON.stringify(PODetails));

            $.ajax({
                url: '/Home/PurchaseOrderIsApproved',
                type: 'Post',
                processData: false,
                contentType: false,
                data: form_data,
                success: function (Result) {
                    if (Result.code == 200) {
                        siteloaderhide();
                        Swal.fire({
                            text: Result.message,
                            icon: "success",
                            confirmButtonClass: "btn btn-primary w-xs mt-2",
                            buttonsStyling: false
                        }).then(function () {
                            window.location = '/Home/Index';
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
                'Purchase Order have no changes.!!😊',
                'error'
            )
        }
    });
}
function dashboardDeletePODetails(POId) {

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
                url: '/PurchaseMaster/DeletePurchaseOrderDetails?POId=' + POId,
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
                        window.location = '/Home/Index';
                    })
                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete purchaseorder!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Purchase Order have no changes.!!😊',
                'error'
            );
        }
    });
}
function GetDashboardInvoiceList() {

    $.get("/Home/SupplierInvoiceListAction")
        .done(function (result) {
            $("#dashboardInvoiceList").html(result);
        })
        .fail(function (xhr, status, error) {

        });
}

function dashboardInvoiceIsApproved() {
    Swal.fire({
        title: "Are you sure you want to approve this invoice?",
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
            var approvalStatuses = [];
            $('input[name="chk_invoicechild"]').each(function () {
                var Id = $(this).attr('id').split('_')[1];
                var isChecked = $(this).is(':checked');
                approvalStatuses.push({ Id: Id, IsApproved: isChecked });
            });


            var InvoiceDetails = {
                InvoiceList: approvalStatuses
            };

            var form_data = new FormData();
            form_data.append("InvoiceIsApproved", JSON.stringify(InvoiceDetails));

            $.ajax({
                url: '/Home/InvoiceIsApproved',
                type: 'Post',
                processData: false,
                contentType: false,
                data: form_data,
                success: function (Result) {
                    if (Result.code == 200) {
                        siteloaderhide();
                        Swal.fire({
                            text: Result.message,
                            icon: "success",
                            confirmButtonClass: "btn btn-primary w-xs mt-2",
                            buttonsStyling: false
                        }).then(function () {
                            window.location = '/Home/Index';
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
                'Invoice have no changes.!!😊',
                'error'
            ).then(function () {
                window.location = '/Home/Index';
            });
        }
    });
}

function dashboardDeleteSupplierInvoice(Id) {

    Swal.fire({
        title: "Are you sure want to delete this invoice?",
        text: "You won't be able to revert this invoice!",
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
                url: '/InvoiceMaster/DeleteSupplierInvoice?Id=' + Id,
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
                        window.location = '/Home/Index';
                    })
                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete Invoice!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Invoice have no changes.!!😊',
                'error'
            );
        }
    });
}

function MutiplePurchaseRequestIsApproved() {

    Swal.fire({
        title: "Are you sure you want to approve this purchase order?",
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
            var approvalStatuses = [];
            $('input[name="chk_PRchild"]').each(function () {
                var PId = $(this).attr('id').split('_')[1];
                var isChecked = $(this).is(':checked');
                approvalStatuses.push({ Pid: PId, IsApproved: isChecked });
            });

            var PRDetails = {
                PRList: approvalStatuses
            };

            var form_data = new FormData();
            form_data.append("PRIsApproved", JSON.stringify(PRDetails));

            $.ajax({
                url: '/Home/MultiplePurchaseRequestIsApproved',
                type: 'Post',
                processData: false,
                contentType: false,
                data: form_data,
                success: function (Result) {
                    Swal.fire({
                        text: Result.message,
                        icon: "success",
                        confirmButtonClass: "btn btn-primary w-xs mt-2",
                        buttonsStyling: false
                    }).then(function () {
                        window.location = '/Home/Index';
                    });
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Purchase request have no changes.!!😊',
                'error'
            ).then(function () {
                window.location = '/Home/Index';
            });
        }
    });
}