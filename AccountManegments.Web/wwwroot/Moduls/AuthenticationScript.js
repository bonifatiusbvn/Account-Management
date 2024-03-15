
GetAllUserData();
function GetAllUserData() {
    $('#UserTableData').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        "bDestroy": true,
        ajax: {
            type: "Post",
            url: '/Authentication/GetUserList',
            dataType: 'json'
        },
        columns: [
            { "data": "firstName", "name": "FirstName" },
            { "data": "lastName", "name": "LastName" },
            { "data": "userName", "name": "UserName" },
            { "data": "email", "name": "Email" },
            { "data": "phoneNo", "name": "PhoneNo" },
            { "data": "isActive", "name": "IsActive" },
            { "data": "roleName", "name": "RoleName" },
            {
                // Action column
                "data": null,
                "orderable": false,
                "render": function (data, type, full, meta) {
                    return '<div class="table-actions d-flex align-items-center gap-3 fs-6">' +
                        '<a href="javascript:;" class="text-primary" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Views" aria-label="Views"><i class="bi bi-eye-fill"></i></a>' +
                        '<a href="javascript:;" class="text-warning" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Edit" aria-label="Edit"><i class="bi bi-pencil-fill"></i></a>' +
                        '<a href="javascript:;" class="text-danger" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Delete" aria-label="Delete"><i class="bi bi-trash-fill"></i></a>' +
                        '</div>';
                }
            }
        ]
    });
}



