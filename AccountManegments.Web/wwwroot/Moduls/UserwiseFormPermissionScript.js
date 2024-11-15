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



$(document).ready(function () {
    $("input[type='checkbox'][name='isApproved']").on('change', function () {
        const isChecked = $(this).is(':checked');
        const formId = $(this).attr('id').split('_')[1];
        const label = $('#labelIsApproved_' + formId);

        label.text(isChecked ? 'Auto Approved-On' : 'Auto Approved-Off');
    });
});

function EditUserWiseFormDetails(UserId, element) {
    debugger
    siteloadershow();
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');

    $.ajax({
        url: '/User/GetUserwiseFormListById?UserId=' + UserId,
        type: 'post',
        dataType: 'json',
        processData: false,
        contentType: false,
        complete: function (Result) {
            debugger
            siteloaderhide();
            $('#dvEditUserPermissionForm').html(Result.responseText);
        },
        Error: function () {
            siteloaderhide();
            toastr.error("Can't get data!");
        }
    });

}

function UpdateUserwiseFormPermission() {

    siteloadershow();
    var formPermissions = [];
    $(".forms").each(function () {
        var rolewiseformRow = $(this);
        var objData = {
            UserId: rolewiseformRow.find('#txtuserId').val(),
            FormId: rolewiseformRow.find('#txtuserformId').val(),
            IsAddAllow: rolewiseformRow.find('#isAdd_' + rolewiseformRow.data('product-id')).prop('checked'),
            IsViewAllow: rolewiseformRow.find('#isView_' + rolewiseformRow.data('product-id')).prop('checked'),
            IsEditAllow: rolewiseformRow.find('#isEdit_' + rolewiseformRow.data('product-id')).prop('checked'),
            IsDeleteAllow: rolewiseformRow.find('#isDelete_' + rolewiseformRow.data('product-id')).prop('checked'),
            IsApproved: rolewiseformRow.find('#isApproved_' + rolewiseformRow.data('product-id')).prop('checked'),
        };
        formPermissions.push(objData);
    });
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
            if (Result.code === 200) {
                toastr.success(Result.message);
                setTimeout(function () {
                    window.location = '/User/UserwisePermission';
                }, 2000);
            } else {
                toastr.error(Result.message);
            }
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            console.error(error);
        }
    });
}


$(document).ready(function () {

    $("input[type='checkbox'][name='isApproved']").on('change', function () {
        const isChecked = $(this).is(':checked');
        const formId = $(this).attr('id').split('_')[1];
        const label = $('#labelIsApproved_' + formId);
        label.text(isChecked ? 'Auto Approved-On' : 'Auto Approved-Off');
    });
});


function toggleUserCheckboxes(formId) {
    const isChecked = document.getElementById(`usercheckboxAll_${formId}`).checked;
    document.getElementById(`isAdd_${formId}`).checked = isChecked;
    document.getElementById(`isView_${formId}`).checked = isChecked;
    document.getElementById(`isEdit_${formId}`).checked = isChecked;
    document.getElementById(`isDelete_${formId}`).checked = isChecked;
    const isApprovedCheckbox = document.getElementById(`isApproved_${formId}`);
    isApprovedCheckbox.checked = isChecked;


    toggleIsApprovedLabel(formId);
}


function updateUserSelectAll(formId) {
    const isAdd = document.getElementById(`isAdd_${formId}`);
    const isView = document.getElementById(`isView_${formId}`);
    const isEdit = document.getElementById(`isEdit_${formId}`);
    const isDelete = document.getElementById(`isDelete_${formId}`);
    const isApproved = document.getElementById(`isApproved_${formId}`);
    const usercheckboxAll = document.getElementById(`usercheckboxAll_${formId}`);

    const allChecked = isAdd.checked && isView.checked && isEdit.checked && isDelete.checked && isApproved.checked;
    usercheckboxAll.checked = allChecked;
}


function userRoleAllCheckboxes(masterCheckbox) {
    const isChecked = masterCheckbox.checked;
    const allCheckboxes = document.querySelectorAll('.form-check-input');
    allCheckboxes.forEach((checkbox) => {
        checkbox.checked = isChecked;
    });


    const allApprovedCheckboxes = document.querySelectorAll("input[type='checkbox'][name='isApproved']");
    allApprovedCheckboxes.forEach((checkbox) => {
        const formId = checkbox.id.split('_')[1];
        toggleIsApprovedLabel(formId);
    });
}


function toggleIsApprovedLabel(formId) {
    const isApprovedCheckbox = document.getElementById(`isApproved_${formId}`);
    const label = document.getElementById(`labelIsApproved_${formId}`);
    label.textContent = isApprovedCheckbox.checked ? "Auto Approved-On" : "Auto Approved-Off";
}
