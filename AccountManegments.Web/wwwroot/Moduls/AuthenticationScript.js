AllUserTable();
GetSiteDetails();
GetDashboardItemList();
GetDashboardPurchaseOrderList();
GetDashboardInvoiceList();
GetDashboardSupplierList();
GetDashboardInwardList();
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

function GetDashboardSupplierList() {

    $.get("/Home/SupplierListView")
        .done(function (result) {
            $("#dashboardSupplierList").html(result);
        })
        .fail(function (error) {

        });
}
function GetDashboardInwardList() {

    $.get("/Home/ItemInWordListView")
        .done(function (result) {
            $("#dashboardInwardList").html(result);
        })
        .fail(function (error) {
        });
}

function MutipleSupplierIsApproved() {

    Swal.fire({
        title: "Are you sure you want to approve this Supplier?",
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
            $('input[name="chk_Supplierchild"]').each(function () {
                var SupplierId = $(this).attr('id').split('_')[1];
                var isChecked = $(this).is(':checked');
                approvalStatuses.push({ SupplierId: SupplierId, IsApproved: isChecked });
            });

            var SupplierDetails = {
                SupplierList: approvalStatuses
            };

            var form_data = new FormData();
            form_data.append("SupplierIsApproved", JSON.stringify(SupplierDetails));

            $.ajax({
                url: '/Home/MultipleSupplierIsApproved',
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
                'Supplier have no changes.!!😊',
                'error'
            ).then(function () {
                window.location = '/Home/Index';
            });
        }
    });
}

function MutipleInwardIsApproved() {

    Swal.fire({
        title: "Are you sure you want to approve this inward?",
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
            $('input[name="chk_Inwardchild"]').each(function () {
                var InwardId = $(this).attr('id').split('_')[1];
                var isChecked = $(this).is(':checked');
                approvalStatuses.push({ InwardId: InwardId, IsApproved: isChecked });
            });

            var InwardDetails = {
                InwardList: approvalStatuses
            };

            var form_data = new FormData();
            form_data.append("InwardIsApproved", JSON.stringify(InwardDetails));

            $.ajax({
                url: '/Home/MultipleInwardIsApproved',
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
                'Inward have no changes.!!😊',
                'error'
            ).then(function () {
                window.location = '/Home/Index';
            });
        }
    });
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

function DeleteItemInWord(InwordId) {

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
                url: '/ItemInWord/DeleteItemInWord?InwordId=' + InwordId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {
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
                    toastr.error("Can't delete iteminward!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Iteminward have no changes.!!😊',
                'error'
            );
        }
    });
}
$(document).ready(function () {
    fn_autoselect('#txtUnitType', '/ItemMaster/GetAllUnitType', '#txtUnitTypeHidden');
    fn_autoselect('#txtPRUnitType', '/ItemMaster/GetAllUnitType', '#txtPRUnitTypeHidden');
    fn_autoselect('#txtInwardUnitType', '/ItemMaster/GetAllUnitType', '#txtInwardUnitTypeHidden');
    fn_autoselect('#searchItemnameInput', '/ItemMaster/GetItemNameList', '#txtItemName');
    fn_autoselect('#searchPRItemnameInput', '/ItemMaster/GetItemNameList', '#txtPRItemName');
});
var additionalFiles = [];
function CancelImage(documentName) {
    $("#addNewImage").find("img[src$='" + documentName + "']").closest('.DocumentName').remove();

    var currentDocumentNames = $("#txtDocumentName").val().split(';');
    var updatedDocumentNames = currentDocumentNames.filter(function (name) {
        return name.trim() !== documentName.trim();
    });

    $("#txtDocumentName").val(updatedDocumentNames.join(';'));
}
function removenewaddImage() {

    $(document).on('click', '.img-remove', function () {
        var row = $(this).closest('.DocumentName');
        var documentName = row.find('img').data('document');
        row.remove();

        additionalFiles = additionalFiles.filter(function (item) {
            return item.name !== documentName;
        });
    });
}
function showpictures() {

    var files = $("#txtDocument")[0].files;
    if (files.length > 0) {
        if ($("#addNewImage .DocumentName").length + files.length > 5) {
            toastr.error("You can only add a maximum of 5 images.");
            return;
        }
        for (var i = 0; i < files.length; i++) {
            const file = files[i];
            let reader = new FileReader();
            reader.onload = (function (fileName) {
                return function (event) {
                    var documentName = fileName;
                    var newRow = "<div class='col-6 col-sm-6 DocumentName'><div><div id='showimages'><div onclick='removenewaddImage()' class='img-remove'><div class='font-22'><i class='lni lni-close'></i></div></div><img src='" + event.target.result + "' class='displayImage' data-document='" + documentName + "'></div></div></div>";
                    $("#addNewImage").append(newRow);
                };
            })(file.name);
            reader.readAsDataURL(file);
            additionalFiles.push(file);
        }
    }
}
GetSiteDetail()
function GetSiteDetail() {
    $.ajax({
        url: '/SiteMaster/GetSiteNameList',
        success: function (result) {
            if (result.length > 0) {
                $.each(result, function (i, data) {
                    $('#txtPRSiteName').append('<Option value=' + data.siteId + '>' + data.siteName + '</Option>')
                });
            }
        }
    });
}
function EditDashboardItemInWordDetails(InwordId) {
    $('#addNewImage').empty();


    $('#EditDashboardItemInwardModal').modal('show');

    $.ajax({
        url: '/ItemInWord/DisplayItemInWordDetails?InwordId=' + InwordId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            $('#txtItemInWordid').val(response.inwordId);
            $('#txtInwardUnitType').val(response.unitName);
            $('#txtInwardUnitTypeHidden').val(response.unitTypeId);
            $('#txtItemName').val(response.itemId);
            $('#searchItemnameInput').val(response.item);
            $('#txtQuantity').val(response.quantity);
            $("#txtVehicleNumber").val(response.vehicleNumber);
            $("#txtReceiverName").val(response.receiverName);
            $("#siteNameList").val(response.siteName);
            $("#siteNameListHidden").val(response.siteId);

            var date = response.date;
            var formattedDate = date.substr(0, 10);
            $('#txtIteminwordDate').val(formattedDate);

            if (response.documentLists && response.documentLists.length > 0) {
                var documentNames = "";
                $.each(response.documentLists, function (index, document) {
                    documentNames += document.documentName + ";";
                    var newRow = "<div class='col-6 col-sm-6 DocumentName' id='itemInWordId_" + document.id + "'>" +
                        "<div>" +
                        "<div id='showimages'>" +
                        "<div onclick='CancelImage(\"" + document.documentName + "\")' class='img-remove'>" +
                        "<div class='font-22'><i class='lni lni-close'></i></div></div>" +
                        "<img src='/Content/InWordDocument/" + document.documentName + "' class='displayImage'></div>" +
                        "</div></div>";
                    $("#addNewImage").append(newRow);
                });
                $("#txtDocumentName").val(documentNames);
            }
        },
        error: function (xhr, status, error) {
        }
    });
}
$(document).ready(function () {

    $("#DashboarditemInWordForm").validate({
        rules: {
            txtInwardUnitType: "required",
            searchItemnameInput: "required",
            txtQuantity: "required",
            txtReceiverName: "required",
            txtVehicleNumber: "required",
            txtItemId: "required"
        },
        messages: {
            txtInwardUnitType: "Enter UnitType",
            searchItemnameInput: "Enter Product",
            txtQuantity: "Enter Quantity",
            txtReceiverName: "Enter ReceiverName",
            txtVehicleNumber: "Enter VehicleNumber",
            txtItemId: "select item"
        }
    })
});
function UpdateMultipleItemInWordDetails() {

    if ($("#DashboarditemInWordForm").valid()) {
        var siteId = null;
        if ($("#siteNameList").val()) {
            siteId = $("#siteNameListHidden").val();
        }
        else {
            siteId = $("#txtInwardSiteid").val();
        }
        var documentName = $("#txtDocumentName").val();


        var UpdateItemInWord = {
            InwordId: $('#txtItemInWordid').val(),
            UnitTypeId: $("#txtInwardUnitTypeHidden").val(),
            ItemId: $("#txtItemName").val(),
            Item: $("#searchItemnameInput").val(),
            Quantity: $("#txtQuantity").val(),
            VehicleNumber: $("#txtVehicleNumber").val(),
            ReceiverName: $("#txtReceiverName").val(),
            Date: $("#txtIteminwordDate").val(),
            DocumentName: documentName,
            SiteId: siteId,
        };

        var form_data = new FormData();
        form_data.append("UpdateItemInWord", JSON.stringify(UpdateItemInWord));

        if (additionalFiles.length > 0) {
            for (var i = 0; i < additionalFiles.length; i++) {
                form_data.append("DocDetails", additionalFiles[i]);
            }
        }

        $.ajax({
            url: '/ItemInWord/UpdatetMultipleItemInWordDetails',
            type: 'POST',
            data: form_data,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (Result) {
                if (Result.code == 200) {
                    $('#EditDashboardItemInwardModal').modal('hide');
                    toastr.success(Result.message);
                    GetDashboardItemList();
                    GetDashboardPurchaseOrderList();
                    GetDashboardInvoiceList();
                    GetDashboardSupplierList();
                    GetDashboardInwardList();
                } else {
                    toastr.error(Result.message);
                }
            },
            error: function (xhr, status, error) {
                siteloaderhide();
                toastr.error('An error occurred while processing your request.');
            }
        });
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}

function EditItemDetails(ItemId) {

    $('#EditDashboardItemModal').modal('show');

    $.ajax({
        url: '/ItemMaster/DisplayItemDetails?ItemId=' + ItemId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            $('#txtItemid').val(response.itemId);
            $('#txtItemName').val(response.itemName);
            $('#txtUnitTypeHidden').val(response.unitType);
            $('#txtUnitType').val(response.unitTypeName);
            $('#txtPricePerUnit').val(response.pricePerUnit);
            $('#txtIsWithGst').prop('checked', response.isWithGst);
            $('#txtGstAmount').val(response.gstamount);
            $('#txtGstPerUnit').val(response.gstper);
            $('#txtHSNCode').val(response.hsncode);
            $('#txtIsApproved').val(response.isApproved);

        },
        error: function (xhr, status, error) {

        }
    });
}
function UpdateItemDetails() {

    if ($("#DashboardItemMsterForm").valid()) {
        var objData = {
            ItemId: $('#txtItemid').val(),
            ItemName: $('#txtItemName').val(),
            UnitType: $('#txtUnitTypeHidden').val(),
            PricePerUnit: $('#txtPricePerUnit').val(),
            Gstamount: $('#txtGstAmount').val(),
            IsWithGst: $('#txtIsWithGst').prop('checked'),
            Gstper: $('#txtGstPerUnit').val(),
            Hsncode: $('#txtHSNCode').val(),
            IsApproved: $('#txtIsApproved').val(),
        };

        $.ajax({
            url: '/ItemMaster/UpdateItemDetails',
            type: 'post',
            data: objData,
            dataType: 'json',
            success: function (result) {
                if (result.code == 200) {
                    $('#EditDashboardItemModal').modal('hide');
                    toastr.success(result.message);
                    GetDashboardItemList();
                    GetDashboardPurchaseOrderList();
                    GetDashboardInvoiceList();
                    GetDashboardSupplierList();
                    GetDashboardInwardList();
                } else {
                    toastr.error(result.message);
                }
            },
            error: function (xhr, status, error) {

                toastr.error('An error occurred while processing your request.');
            }
        });
    } else {
        toastr.error("Kindly fill all details");
    }
}
function fn_getDashboardPRSiteDetail(SiteId, callback) {
    $('#drpPRSiteAddress').empty();
    $.ajax({
        url: '/SiteMaster/GetSiteAddressList?SiteId=' + SiteId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (result) {
            $('#drpPRSiteAddress').append('<option value="" selected disabled>--Select Site Address--</option>')
            if (result.length > 0) {
                $.each(result, function (i, data) {
                    $('#drpPRSiteAddress').append('<option value=' + data.aid + '>' + data.address + '</option>')
                });
            }
            if (callback && typeof callback === "function") {
                callback();
            }
        },
    });
}
function EditPurchaseRequestDetails(PurchaseId) {
    $('#EditPurchaseRequestModal').modal('show');
    $.ajax({
        url: '/PurchaseMaster/DisplayPurchaseRequestDetails?PurchaseId=' + PurchaseId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            $('#PurchaseRequestId').val(response.pid);
            $('#txtPRUnitTypeHidden').val(response.unitTypeId);
            $('#prNo').val(response.prNo);
            $('#txtPRItemName').val(response.itemId);
            $('#searchPRItemnameInput').val(response.itemName);
            $('#txtPRQuantity').val(response.quantity);
            $('#txtPRSiteName').val(response.siteId);
            $('#txtPRUnitType').val(response.unitName);
            fn_getDashboardPRSiteDetail(response.siteId, function () {
                $('#drpPRSiteAddress').val(response.siteAddressId);
            });
        },
        error: function (xhr, status, error) {
            toastr.error(xhr.responseText);
        }
    });
}
function UpdatePurchaseRequestDetails() {

    var siteName = null;
    siteId = $("#SiteIdinPR").val();
    PRsiteId = $("#txtPRSiteName").val();
    if (PRsiteId != undefined) {
        siteName = PRsiteId;
    }
    else {
        siteName = siteId
    }
    var siteAddressId = $('#drpPRSiteAddress').val();
    var siteAddress = $('#drpPRSiteAddress option:selected').text();

    var objData = {
        SiteAddressId: siteAddressId,
        SiteAddress: siteAddress,
        Pid: $('#PurchaseRequestId').val(),
        CreatedBy: $('#txtcreatedby').val(),
        UnitTypeId: $('#txtPRUnitTypeHidden').val(),
        ItemId: $('#txtPRItemName').val(),
        ItemName: $('#searchPRItemnameInput').val(),
        SiteId: siteName,
        Quantity: $('#txtPRQuantity').val(),
        PrNo: $('#prNo').val(),
    }

    $.ajax({
        url: '/PurchaseMaster/UpdatePurchaseRequestDetails',
        type: 'post',
        data: objData,
        datatype: 'json',
        success: function (Result) {
            siteloaderhide();
            if (Result.code == 200) {
                $('#EditPurchaseRequestModal').modal('hide');
                toastr.success(Result.message);
                GetDashboardItemList();
                GetDashboardPurchaseOrderList();
                GetDashboardInvoiceList();
                GetDashboardSupplierList();
                GetDashboardInwardList();
            } else {
                toastr.error(Result.message);
            }
        },
    })
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
fn_getState('dropState', 1);
GetCountry();
function EditSupplierDetails(SupplierId) {

    $('#EditSupplierModal').modal('show');

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
            $('#dropState').val(response.state);
            $('#txtPinCode').val(response.pinCode);
            $('#txtBank').val(response.bankName);
            $('#txtBranch').val(response.branchName);
            $('#txtAccount').val(response.accountNo);
            $('#txtIFFC').val(response.iffccode);

            fn_getcitiesbystateId('ddlCity', response.state);

            setTimeout(function () { $('#ddlCity').val(response.city); }, 100);

            var button = document.getElementById("btnitem");
            if ($('#txtSupplierid').val() !== '') {
                button.textContent = "Update";
            }
        },
        error: function (xhr, status, error) {
            toastr.error(xhr.responseText);
        }
    });
}

$(document).ready(function () {
    $("#EditSupplierForm").validate({
        rules: {
            txtSupplierName: "required",
            txtGST: "required",
            txtBuilding: "required",
            txtArea: "required",
            ddlCity: "required",
            dropState: "required",
            ddlCountry: "required"
        },
        messages: {
            txtSupplierName: "Please Enter SupplierName",
            txtGST: "Please Enter GST",
            txtBuilding: "Please Enter Building",
            txtArea: "Please Enter Area",
            ddlCity: "Please Enter City",
            dropState: "Please Enter State",
            ddlCountry: "Please Enter Country"
        }
    });
})

function UpdateSupplierDetails() {

    if ($("#EditSupplierForm").valid()) {
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
            BranchName: $('#txtBranch').val(),
            AccountNo: $('#txtAccount').val(),
            Iffccode: $('#txtIFFC').val(),
            UpdatedBy: $('#txtUserid').val(),

        }

        if (objData.City == "--Select City--" || objData.State == "--Select State--") {
            toastr.error("Kindly fill all details");
        }

        else {
            $.ajax({
                url: '/Supplier/UpdateSupplierDetails',
                type: 'post',
                data: objData,
                datatype: 'json',
                success: function (Result) {
                    if (Result.code == 200) {
                        $('#EditSupplierModal').modal('hide');
                        toastr.success(Result.message);
                        GetDashboardItemList();
                        GetDashboardPurchaseOrderList();
                        GetDashboardInvoiceList();
                        GetDashboardSupplierList();
                        GetDashboardInwardList();
                    } else {
                        toastr.error(Result.message);
                    }
                },
            })
        }

    }
    else {
        toastr.error("Kindly fill all details");
    }
}

$(document).ready(function () {
    function PRApproveButton() {
        var hasChecked = $('input[name="chk_PRchild"]:checked').length > 0;
        if (hasChecked) {
            $('#PRApproveBtn').show(); 
        } else {
            $('#PRApproveBtn').hide();
        }
    } 
    $('input[name="chk_PRchild"], #checkedAllPR').on('change', function () {
        PRApproveButton();
    });
    PRApproveButton();

    function ItemApproveButton() {
        var hasChecked = $('input[name="chk_child"]:checked').length > 0;
        if (hasChecked) {
            $('#ItemApproveBtn').show();
        } else {
            $('#ItemApproveBtn').hide(); 
        }
    }
    $('input[name="chk_child"], #checkedAllItem').on('change', function () {
        ItemApproveButton();
    });
    ItemApproveButton();

    function POApproveButton() {
        var hasChecked = $('input[name="chk_POchild"]:checked').length > 0;
        if (hasChecked) {
            $('#POApproveBtn').show();
        } else {
            $('#POApproveBtn').hide();
        }
    }
    $('input[name="chk_POchild"], #checkedAllPO').on('change', function () {
        POApproveButton();
    });
    POApproveButton();

    function InvoiceApproveButton() {
        var hasChecked = $('input[name="chk_invoicechild"]:checked').length > 0;
        if (hasChecked) {
            $('#InvoiceApproveBtn').show();
        } else {
            $('#InvoiceApproveBtn').hide();
        }
    }
    $('input[name="chk_invoicechild"], #checkedAllInvoice').on('change', function () {
        InvoiceApproveButton();
    });
    InvoiceApproveButton();

    function SupplierApproveButton() {
        var hasChecked = $('input[name="chk_Supplierchild"]:checked').length > 0;
        if (hasChecked) {
            $('#SupplierApproveBtn').show(); 
        } else {
            $('#SupplierApproveBtn').hide(); 
        }
    }
    $('input[name="chk_Supplierchild"], #checkedAllSupplier').on('change', function () {
        SupplierApproveButton();
    });
    SupplierApproveButton();

    function InwardApproveButton() {
        var hasChecked = $('input[name="chk_Inwardchild"]:checked').length > 0;
        if (hasChecked) {
            $('#InwardApproveBtn').show();
        } else {
            $('#InwardApproveBtn').hide();
        }
    }
    $('input[name="chk_Inwardchild"], #checkedAllInward').on('change', function () {
        InwardApproveButton();
    });
    InwardApproveButton();
});