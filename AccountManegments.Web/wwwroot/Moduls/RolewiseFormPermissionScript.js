AllRolewiseFormUserTable();

function showFormGroupList() {
    siteloadershow();
    $.ajax({
        url: '/User/GetFormGroupList',
        type: 'Post',
        dataType: 'json',
        processData: false,
        contentType: false,
        complete: function (Result) {
            siteloaderhide();
            $('#dvrolePermissionForm').html(Result.responseText);
        },
        Error: function () {
            siteloaderhide();
            toastr.error("Can't get data!");
        }
    })
}

function AllRolewiseFormUserTable() {

    $.get("/User/RolewisePermissionListAction")
        .done(function (result) {

            $("#UserRoletbody").html(result);
        })
        .fail(function (error) {
            siteloaderhide();
            toastr.error(error);
        });
}

function ClearUserTextBox() {
    var button = document.getElementById("btnUserRolewiseForm");
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('createFormGroup'));
    offcanvas.show();
}


function EditRoleWiseFormDetails(RoleId, element) {
    siteloadershow();
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
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
            siteloaderhide();
            $('#dveditRolePermissionForm').html(Result.responseText);
        },
        Error: function () {
            siteloaderhide();
            toastr.error("Can't get data!");
        }
    });
}

function UpdateRolewiseFormPermission() {
    siteloadershow();
    var formPermissions = [];
    $(".forms").each(function () {

        var rolewiseformRow = $(this);
        var objData = {
            RoleId: $('#txtRoleId').val(),
            FormId: rolewiseformRow.find('#formId').val(),
            IsAddAllow: rolewiseformRow.find('#isAdd').prop('checked'),
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
            siteloaderhide();
            if (Result.code == 200) {
                siteloaderhide();
                Swal.fire({
                    title: Result.message,
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(function () {
                    window.location = '/User/RolewisePermission';
                });
            } else {
                siteloaderhide();
                toastr.error(Result.message);    
            }
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(error);
        }
    });
}

function CreateRolewiseFormPermission() {
    siteloadershow();
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
            siteloaderhide();
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
            siteloaderhide();
            toastr.error(error);
        }
    });
}





