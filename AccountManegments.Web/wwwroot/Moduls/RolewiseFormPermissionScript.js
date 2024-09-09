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
            IsApproved: rolewiseformRow.find('#isApproved').prop('checked'),
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
                toastr.success(Result.message);
                setTimeout(function () {
                    window.location = '/User/RolewisePermission';
                }, 2000);
            } else {
                siteloaderhide();
                toastr.error(Result.message);
            }
        },
        error: function (xhr, status, error) {
            siteloaderhide();

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
            if (Result.code == 200) {
                siteloaderhide();
                toastr.success(Result.message);
                setTimeout(function () {
                    window.location = '/User/RolewisePermission';
                }, 2000);
            }
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}
var UserRoleForm;
function validateAndCreateRole() {
    UserRoleForm = $("#addUserRole").validate({
        rules: {
            textRoleName: "required",
        },
        messages: {
            textRoleName: "Please enter role",
        }
    })
    var isValid = true;

    if (isValid) {
        createRole();
    }
}

function createRole() {
    if ($("#addUserRole").valid()) {
        var formData = new FormData();
        formData.append("Role", $("#textRoleName").val());
        formData.append("CreatedBy", $("#txtUserId").val());

        $.ajax({
            url: '/User/CreateUserRole',
            type: 'Post',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
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
                }
                else {
                    toastr.error(Result.message);
                }
            }
        });
    }
    else {
        toastr.warning("Kindly fill role");
    }
}


function clearTextRoleName() {
    ResetUserRoleForm();
    document.getElementById("textRoleName").value = "";
    $('#createRoleModal').modal('show');
}


function RoleActiveDecative(roleId) {

    var isChecked = $('#flexSwitchCheckChecked_' + roleId).is(':checked');
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
            formData.append("RoleId", roleId);

            $.ajax({
                url: '/User/RoleActiveDecative?RoleId=' + roleId,
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

