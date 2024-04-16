AllUserTable();
GetSiteDetails();
function CreateUser() {
    if ($("#userForm").valid()) {
        var objData = {
            FirstName: $('#txtFirstName').val(),
            LastName: $('#txtLastName').val(),
            UserName: $('#txtUserName').val(),
            Password: $('#txtPassword').val(),
            Role: $('#txtrole').val(),
            Email: $('#txtEmail').val(),
            PhoneNo: $('#txtPhoneNo').val(),
            SiteId: $('#txtuserSiteName').val(),
        }
        $.ajax({
            url: '/User/CreateUser',
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
                    window.location = '/User/UserListView';
                });
            },
        })
    }
    else {
        Swal.fire({
            title: "Kindly fill all details",
            icon: 'warning',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'OK',
        })
    }
}
function GetSiteDetails() {
    $.ajax({
        url: '/SiteMaster/GetSiteNameList',
        success: function (result) {
            $('#ddlSiteName').empty();

            $('#ddlSiteName').append('<option value="null" selected>All Site</option>');
            $.each(result, function (i, data) {

                $('#ddlSiteName').append('<option value=' + data.siteId + '>' + data.siteName + '</Option>')

            });
        }
    });
}


function ClearUserTextBox() {
    resetUserForm();
    $('#txtUserid').val('');
    $('#txtFirstName').val('');
    $('#txtLastName').val('');
    $('#txtUserName').val('');
    $('#txtPassword').val('');
    $('#txtEmail').val('');
    $('#txtrole').val('');
    $('#txtPhoneNo').val('');
    $('#ddlUserRole').val('');
    $('#txtuserSiteName').val('');
    var button = document.getElementById("btnuser");
    if ($('#txtUserid').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('createUser'));
    offcanvas.show();
}

function DisplayUserDetails(UserId) {
    $.ajax({
        url: '/User/DisplayUserDetails?UserId=' + UserId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            $('#txtUserid').val(response.id);
            $('#txtFirstName').val(response.firstName);
            $('#txtLastName').val(response.lastName);
            $('#txtPassword').val(response.password);
            $('#txtUserName').val(response.userName);
            $('#txtEmail').val(response.email);
            $('#txtPhoneNo').val(response.phoneNo);
            $('#txtuserSiteName').val(response.siteId);
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
            console.error(xhr.responseText);
        }
    });
}

function SelectUserDetails(UserId, element) {

    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/User/DisplayUserDetails?UserId=' + UserId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
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
                console.log('Empty response received.');
            }
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
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
            console.error(error);
        });
}

function filterTable() {

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
            $("#Usertbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function sortTable() {
    var sortBy = $('#ddlSortBy').val();
    $.ajax({
        url: '/User/UserListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            $("#Usertbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}


function UpdateUserDetails() {
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
            SiteId: $('#txtuserSiteName').val(),
        }
        $.ajax({
            url: '/User/UpdateUserDetails',
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
                    window.location = '/User/UserListView';
                });
            },
        })
    }
    else {
        Swal.fire({
            title: "Kindly fill all details",
            icon: 'warning',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'OK',
        })
    }
}
var UserForm;
function validateAndCreateUser() {

    UserForm=$("#userForm").validate({
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
    var isValid = true;

    if (isValid) {
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
            console.error('Error:', error);

        });
}
function deleteUserDetails(UserId) {

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
                url: '/User/DeleteUserDetails?UserId=' + UserId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {

                    Swal.fire({
                        title: Result.message,
                        icon: 'success',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK'
                    }).then(function () {
                        window.location = '/User/UserListView';
                    })
                },
                error: function () {
                    Swal.fire({
                        title: "Can't Delete User!",
                        icon: 'warning',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK',
                    }).then(function () {
                        window.location = '/User/UserListView';
                    })
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'User Have No Changes.!!😊',
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


$(document).ready(function () {

    bindEventListeners();


    $(document).on('input', '.product-quantity', function () {
        var row = $(this).closest('.product');
        updateProductTotalAmount(row);
        updateTotals();
    });


    $(document).on('keydown', '.product-quantity', function (event) {
        if (event.key === 'Enter') {
            $(this).blur();
        }
    });


    $(document).on('input', '#txtproductamount', function () {
        var row = $(this).closest('.product');
        updateProductTotalAmount(row);
        updateTotals();
    });


    $(document).on('keydown', '#txtproductamount', function (event) {
        if (event.key === 'Enter') {
            $(this).blur();
        }
    });

    $(document).on('input', '#txtgst', function () {
        var row = $(this).closest('.product');
        updateProductTotalAmount(row);
        updateTotals();
    });

    $(document).on('keydown', '#txtgst', function (event) {
        if (event.key === 'Enter') {
            $(this).blur();
        }
    });

    $(document).on('input', '#txtdiscountamount', function () {
        var row = $(this).closest('.product');
        updateProductTotalAmount(row);
        updateTotals();
    });

    $(document).on('keydown', '#txtdiscountamount', function (event) {
        if (event.key === 'Enter') {
            $(this).blur();
        }
    });

    $(document).on('focusout', '.product-quantity', function () {
        $(this).trigger('input');
    });
});






