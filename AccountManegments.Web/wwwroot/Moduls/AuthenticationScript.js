
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
                        '<a href="javascript:;" class="text-warning" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Edit" aria-label="Edit"><i class="fadeIn animated bx bx-edit"></i></a>' +
                        '<a href="javascript:;" class="text-danger" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Delete" aria-label="Delete"><i class="lni lni-trash"></i></a>' +
                        '</div>';
                }
            }
        ]
    });
}



