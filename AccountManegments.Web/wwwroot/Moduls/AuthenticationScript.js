
function CreateUser() {

    var objData = {
        FirstName: $('#txtFirstName').val(),
        LastName: $('#txtLastName').val(),
        UserName: $('#txtUserName').val(),
        Password: $('#txtPassword').val(),
        Email: $('#txtEmail').val(),
        PhoneNo: $('#txtPhoneNo').val(),

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
//function GetAllUserData() {
//    $('#UserTableData').DataTable({
//        processing: false,
//        serverSide: true,
//        filter: false,
//        searching: false,
//        lengthChange: false,
//        "bDestroy": true,
//        sorting: false,

//        ajax: {
//            type: "Post",
//            url: '/User/GetUserList',
//            dataType: 'json'
//        },
//        columns: [
//            { "data": "firstName", "name": "FirstName", "className": "text-center" },
//            { "data": "lastName", "name": "LastName", "className": "text-center" },
//            { "data": "userName", "name": "UserName", "className": "text-center" },
//            { "data": "email", "name": "Email", "className": "text-center" },
//            { "data": "phoneNo", "name": "PhoneNo", "className": "text-center" },
//            {
//                "data": "isActive", "name": "IsActive", "className": "text-center",
//                "render": function (data, type, full) {
//                    if (full.isActive == true) {
//                        return '<span class="badge bg-success text-uppercase">Active</span>';
//                    } else {
//                        return '<span class="badge bg-danger text-uppercase">Deactive</span>';
//                    }
//                }
//            },
//            { "data": "roleName", "name": "RoleName", "className": "text-center" },
//            {

//                "data": null,
//                "orderable": false,
//                "className": "text-center",
//                "render": function (data, type, full, meta) {
//                    return '<div class="table-actions d-flex align-items-center gap-3 fs-6">' +
//                        '<a class="text-warning" onclick="DisplayUserDetails(\'' + data.id + '\')" title="Edit" aria-label="Edit"><i class="fadeIn animated bx bx-edit"></i></a>' +
//                        '<a href="javascript:;" class="text-danger" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Delete" aria-label="Delete"><i class="lni lni-trash"></i></a>' +
//                        '</div>';
//                }

//            }
//        ]
//    });
//}


function DisplayUserDetails(UserId) {
    $.ajax({
        url: '/User/DisplayUserDetails?UserId=' + UserId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            $('#Userid').val(response.id);
            $('#FirstName').val(response.firstName);
            $('#LastName').val(response.lastName);
            $('#Password').val(response.password);
            $('#UserName').val(response.userName);
            $('#Email').val(response.email);
            $('#PhoneNo').val(response.phoneNo);

            var offcanvas = new bootstrap.Offcanvas(document.getElementById('editUserDetails'));
            offcanvas.show();
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

document.addEventListener("DOMContentLoaded", function () {
    // JavaScript code for filtering the table
    function filterTable() {
        var searchText = document.getElementById("txtSearch").value.toLowerCase();
        var searchBy = document.getElementById("ddlSearchBy").value.toLowerCase();
        var rows = document.getElementById("userTable").getElementsByTagName("tbody")[0].rows;
        for (var i = 0; i < rows.length; i++) {
            var text = rows[i].cells[searchBy].textContent.toLowerCase();
            if (!text.includes(searchText)) {
                rows[i].style.display = "none";
            } else {
                rows[i].style.display = "";
            }
        }
    }

    // JavaScript code for sorting the table
    function sortTable() {
        var sortBy = document.getElementById("ddlSortBy").value;
        var tbody = document.getElementById("userTable").getElementsByTagName("tbody")[0];
        var rows = Array.from(tbody.rows);
        rows.sort((a, b) => {
            var cellA = a.cells[sortBy].textContent.toUpperCase();
            var cellB = b.cells[sortBy].textContent.toUpperCase();
            return cellA.localeCompare(cellB);
        });
        tbody.innerHTML = "";
        rows.forEach(row => tbody.appendChild(row));
    }

    // Check if necessary elements exist before adding event listeners
    var txtSearch = document.getElementById("txtSearch");
    var ddlSearchBy = document.getElementById("ddlSearchBy");
    var ddlSortBy = document.getElementById("ddlSortBy");
    var userTable = document.getElementById("userTable");

    if (txtSearch && ddlSearchBy && ddlSortBy && userTable) {
        txtSearch.addEventListener("input", filterTable);
        ddlSearchBy.addEventListener("change", filterTable);
        ddlSortBy.addEventListener("change", sortTable);
    } else {
        console.error("One or more elements not found.");
    }
});

function UpdateUserDetails() {

    var objData = {
        Id: $('#Userid').val(),
        FirstName: $('#FirstName').val(),
        LastName: $('#LastName').val(),
        UserName: $('#UserName').val(),
        Password: $('#Password').val(),
        Email: $('#Email').val(),
        PhoneNo: $('#PhoneNo').val(),
    }
    $.ajax({
        url: '/Authentication/UpdateUserDetails',
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
                window.location = '/Authentication/UserListView';
            });
        },
    })

}
var FirstName, LastName, UserName, Password, Email, PhoneNo;
var isValid = true;

$('#btnUpdate').click(function () {
    if (CheckValidation() == false) {
        return false;
    }
});
function CheckValidation() {
    FirstName = $('#txtFirstName').val();
    LastName = $('#txtLastName').val();
    UserName = $('#txtUserName').val();
    Password = $('#txtPassword').val();
    Email = $('#txtEmail').val();
    PhoneNo = $('#txtPhoneNo').val();

    //fname
    if (FirstName == "") {
        $('#spnFirstName').text('FirstName can not be blank.');
        $('#txtFirstName').css('border-color', 'red');
        $('#txtFirstName').focus();
        isValid = false;
    }
    else {
        $('#spnFirstName').text('');
        $('#txtFirstName').css('border-color', 'green');
    }
    //lname
    if (LastName == "") {

        $('#spnLastName').text('LastName can not be blank.');
        $('#txtLastName').css('border-color', 'red');
        $('#txtLastName').focus();
        isValid = false;
    }
    else {

        $('#spnLastName').text('');
        $('#txtLastName').css('border-color', 'green');
    }
    //Username
    if (UserName == "") {

        $('#spnUserName').text('UserName can not be blank.');
        $('#txtUserName').css('border-color', 'red');
        $('#txtUserName').focus();
        isValid = false;
    }
    else {

        $('#spnUserName').text('');
        $('#txtUserName').css('border-color', 'green');
    }

    //email
    if (Email == "") {

        $('#spnEmail').text('EmailId can not be blank.');
        $('#txtEmail').css('border-color', 'red');
        $('#txtEmail').focus();
        isValid = false;
    }
    else {

        $('#spnEmail').text('');
        $('#txtEmail').css('border-color', 'green');
    }

    //password
    if (Password == "") {

        $('#spnPassword').text('Password can not be blank.');
        $('#txtPassword').css('border-color', 'red');
        $('#txtPassword').focus();
        isValid = false;
    }
    else {

        $('#spnPassword').text('');
        $('#txtPassword').css('border-color', 'green');
    }

    //phone
    if (PhoneNo == "") {

        $('#spnPhoneNo').text('PhoneNumber can not be blank.');
        $('#txtPhoneNo').css('border-color', 'red');
        $('#txtPhoneNo').focus();
        isValid = false;
    }
    else {

        $('#spnPhoneNo').text('');
        $('#txtPhoneNo').css('border-color', 'green');
    }

    return isValid;
}






