

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



function ClearUserTextBox() {
    var button = document.getElementById("btnUserRolewiseForm");
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('createFormGroup'));
    offcanvas.show();
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




