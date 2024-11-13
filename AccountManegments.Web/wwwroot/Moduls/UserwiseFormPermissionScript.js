$(document).ready(function () {
    AllUserwiseFormUserTable();
});

function AllUserwiseFormUserTable() {
    $.get("/User/UserwisePermissionListAction")
        .done(function (result) {
            $("#UserwisePermissiontbody").html(result);
        })
        .fail(function (error) {
            siteloaderhide();
        });
}
function UserActiveDecative(UserId) {

    var isChecked = $('#flexSwitchCheckChecked_' + UserId).is(':checked');
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
            formData.append("UserId", UserId);

            $.ajax({
                url: '/User/UserActiveDecative?UserId=' + UserId,
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
                            window.location = '/User/UserwisePermission';
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
                window.location = '/User/UserwisePermission';
            });;
        }
    });
}

function EditUserWiseFormDetails(UserId, element) {
    siteloadershow();
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    var button = document.getElementById("btnUserRolewiseForm");
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('createFormGroup'));
    offcanvas.show();
    $.ajax({
        url: '/User/GetUserwiseFormListById?UserId=' + UserId,
        type: 'post',
        dataType: 'json',
        processData: false,
        contentType: false,
        complete: function (Result) {
            siteloaderhide();
            $('#dvEditUserPermissionForm').html(Result.responseText);
        },
        Error: function () {
            siteloaderhide();
            toastr.error("Can't get data!");
        }
    });
}

function UpdateUserwiseFormPermission() {debugger
    siteloadershow();
    var formPermissions = [];
    $(".forms").each(function () {

        var rolewiseformRow = $(this);
        var objData = {
           UserId: $('#txtUserId').val(),
            FormId: rolewiseformRow.find('#formId').val(),
            IsAddAllow: rolewiseformRow.find('#isAdd').prop('checked'),
            IsViewAllow: rolewiseformRow.find('#isView').prop('checked'),
            IsEditAllow: rolewiseformRow.find('#isEdit').prop('checked'),
            IsDeleteAllow: rolewiseformRow.find('#isDelete').prop('checked'),
            IsApproved: rolewiseformRow.find('#isApproved').prop('checked'),
        };

        formPermissions.push(objData);
    });
    debugger
    var form_data = new FormData();
    form_data.append("UserwisePermissionDetails", JSON.stringify(formPermissions));

    $.ajax({
        url: '/User/UpdateMultipleUserwiseFormPermission',
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
                    window.location = '/User/UserwisePermission';
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