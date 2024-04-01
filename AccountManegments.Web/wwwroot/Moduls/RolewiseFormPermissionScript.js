AllRolewiseFormUserTable();

function showFormGroupList() {

    $.ajax({
        url: '/User/GetFormGroupList',
        type: 'Post',
        dataType: 'json',
        processData: false,
        contentType: false,
        complete: function (Result) {

            $('#dvrolePermissionForm').html(Result.responseText);
        },
        Error: function () {
            Swal.fire({
                title: "Can't get data!",
                icon: 'warning',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK',
            })
        }
    })
}

function AllRolewiseFormUserTable() {

    $.get("/User/RolewisePermissionListAction")
        .done(function (result) {

            $("#UserRoletbody").html(result);
        })
        .fail(function (error) {
            console.error(error);
        });
}

function ClearUserTextBox() {
    var button = document.getElementById("btnUserRolewiseForm");
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('createFormGroup'));
    offcanvas.show();
}


function EditRoleWiseFormDetails(RoleId) {
    var button = document.getElementById("btnUserRolewiseForm");
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('createFormGroup'));
    offcanvas.show();
    $.ajax({
        url: '/User/GetRolewiseFormListById?RoleId=' + RoleId,
        type: 'post',
        dataType: 'json',
        processData: false,
        contentType: false,
        complete: function (Result) {

            $('#dveditRolePermissionForm').html(Result.responseText);         
        },
        Error: function () {
    
            Swal.fire({
                title: "Can't get data!",
                icon: 'warning',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK',
            })
        }
    });
}

function UpdateRolewiseFormPermission() {
    var formPermissions = [];
    $(".forms").each(function () {

        var rolewiseformRow = $(this);
        var objData = {
            RoleId: $('#txtRoleId').val(),
            FormId: rolewiseformRow.find('#formId').val(),
            IsViewAllow: rolewiseformRow.find('#isView').prop('checked'),
            IsEditAllow: rolewiseformRow.find('#isEdit').prop('checked'),
            IsDeleteAllow: rolewiseformRow.find('#isDelete').prop('checked'),
        };

        formPermissions.push(objData);
    });

    var form_data = new FormData();
    form_data.append("RolewisePermissionDetails", JSON.stringify(formPermissions));

    $.ajax({
        url: '/User/UpdateMultipleRolewiseFormPermission',
        type: 'post',
        data: form_data,
        processData: false,
        contentType: false, 
        dataType: 'json',
        success: function (Result) {
            if (Result.code == 200) {
                Swal.fire({
                    title: Result.message,
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(function () {
                    window.location = '/User/RolewisePermission';
                });
            } else {
                Swal.fire({
                    title: Result.message,
                    icon: 'warning',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(function () {
                    window.location = '/User/RolewisePermission';
                });
            }
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

function CreateRolewiseFormPermission() {
    var formPermissions = [];
    $(".forms").each(function () {
        var rolewiseformRow = $(this);
        var objData = {
            RoleId: $('#roleId').val(),
            CreatedBy: $('#createdbyid').val(),
            FormId: rolewiseformRow.find('#formId').val(),
            IsViewAllow: rolewiseformRow.find('#isView').prop('checked'),
            IsEditAllow: rolewiseformRow.find('#isEdit').prop('checked'),
            IsDeleteAllow: rolewiseformRow.find('#isDelete').prop('checked'),
        };

        formPermissions.push(objData);
    });

    var form_data = new FormData();
    form_data.append("RolewisePermissionDetails", JSON.stringify(formPermissions));

    $.ajax({
        url: '/User/InsertMultipleRolewiseFormPermission',
        type: 'post',
        data: form_data,
        processData: false,
        contentType: false, 
        dataType: 'json',
        success: function (Result) {
            Swal.fire({
                title: Result.message,
                icon: 'success',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK'
            }).then(function () {
                window.location = '/User/RolewisePermission';
            });
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}





