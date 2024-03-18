
GetAllUserData();
function GetAllUserData() {
    $('#UserTableData').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        "bDestroy": true,
        ajax: {
            type: "Post",
            url: '/User/GetUserList',
            dataType: 'json'
        },
        columns: [
            { "data": "firstName", "name": "FirstName", "className": "text-center" },
            { "data": "lastName", "name": "LastName", "className": "text-center" },
            { "data": "userName", "name": "UserName", "className": "text-center" },
            { "data": "email", "name": "Email", "className": "text-center" },
            { "data": "phoneNo", "name": "PhoneNo", "className": "text-center" },
            { "data": "isActive", "name": "IsActive", "className": "text-center" },
            { "data": "roleName", "name": "RoleName", "className": "text-center" },
            {
                // Action column
                "data": null,
                "orderable": false,
                "className": "text-center",
                "render": function (data, type, full, meta) {
                    return '<div class="table-actions d-flex align-items-center gap-3 fs-6">' +
                        '<a href="javascript:;" class="text-primary" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Views" aria-label="Views"><i class="lni lni-eye"></i></a>' +
                        '<a class="text-warning" onclick="DisplayUserDetails(\'' + data.id + '\')" title="Edit" aria-label="Edit"><i class="fadeIn animated bx bx-edit"></i></a>' +
                        '<a href="javascript:;" class="text-danger" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Delete" aria-label="Delete"><i class="lni lni-trash"></i></a>' +
                        '</div>';
                }

            }
        ]
    });
}


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


function CreateUser() {
    debugger
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



function UpdateUserDetails() {
    debugger
    var objData = {
        Id: $('#Userid').val(),
        FirstName: $('#FirstName').val(),
        LastName: $('#LastName').val(),
        UserName: $('#UserName').val(),
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


