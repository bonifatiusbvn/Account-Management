AllUserTable();
GetSiteDetails();
function CreateUser() {


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
function GetSiteDetails() {

    $.ajax({
        url: '/SiteMaster/GetSiteNameList',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#txtuserSiteName').append('<Option value=' + data.siteId + '>' + data.siteName + '</Option>')
                $('#txtSiteName').append('<Option value=' + data.siteId + '>' + data.siteName + '</Option>')
            });
        }
    });
}

function ClearUserTextBox() {

    resetErrorMessages();
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
    debugger
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
function validateAndCreateUser() {

    resetErrorMessages();

    var firstName = document.getElementById("txtFirstName").value.trim();
    var lastName = document.getElementById("txtLastName").value.trim();
    var userName = document.getElementById("txtUserName").value.trim();
    var password = document.getElementById("txtPassword").value.trim();
    var email = document.getElementById("txtEmail").value.trim();
    var phoneNo = document.getElementById("txtPhoneNo").value.trim();
    var SiteName = document.getElementById("txtuserSiteName").value.trim();


    var isValid = true;


    if (firstName === "") {
        document.getElementById("spnFirstName").innerText = "First Name is required.";
        isValid = false;
    }


    if (lastName === "") {
        document.getElementById("spnLastName").innerText = "Last Name is required.";
        isValid = false;
    }


    if (userName === "") {
        document.getElementById("spnUserName").innerText = "User Name is required.";
        isValid = false;
    }


    if (password === "") {
        document.getElementById("spnPassword").innerText = "Password is required.";
        isValid = false;
    }


    if (email === "") {
        document.getElementById("spnEmail").innerText = "Email is required.";
        isValid = false;
    } else if (!isValidEmail(email)) {
        document.getElementById("spnEmail").innerText = "Invalid Email format.";
        isValid = false;
    }


    if (phoneNo === "") {
        document.getElementById("spnPhoneNo").innerText = "Phone Number is required.";
        isValid = false;
    } else if (!isValidPhoneNo(phoneNo)) {
        document.getElementById("spnPhoneNo").innerText = "Invalid Phone Number format.";
        isValid = false;
    }

    if (SiteName === "") {
        document.getElementById("spnSiteName").innerText = "Site Name is required.";
        isValid = false;
    }
    debugger
    if (isValid) {
        if ($("#txtUserid").val() == '') {
            CreateUser();
        }
        else {
            UpdateUserDetails()
        }
    }
}


function resetErrorMessages() {
    document.getElementById("spnFirstName").innerText = "";
    document.getElementById("spnLastName").innerText = "";
    document.getElementById("spnUserName").innerText = "";
    document.getElementById("spnPassword").innerText = "";
    document.getElementById("spnEmail").innerText = "";
    document.getElementById("spnPhoneNo").innerText = "";
    document.getElementById("spnSiteName").innerText = "";
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
            debugger
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


    $(document).on('focusout', '.product-quantity', function () {
        $(this).trigger('input');
    });
});



