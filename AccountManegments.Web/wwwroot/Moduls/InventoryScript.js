InventoryTable();
function InventoryTable() {

    var searchText = $('#txtInventoryRequestSearch').val();
    var searchBy = $('#InventoryRequestSearchBy').val();

    $.get("/Sales/InventoryListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {

            $("#Inventorybody").html(result);
        })

}

function filterInventoryRequestTable() {
    siteloadershow();
    var searchText = $('#txtInventoryRequestSearch').val();
    var searchBy = $('#InventoryRequestSearchBy').val();

    $.ajax({
        url: '/Sales/InventoryListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#Inventorybody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function sortInventoryRequestTable() {
    siteloadershow();
    var sortBy = $('#InventoryRequestSortBy').val();
    $.ajax({
        url: '/Sales/InventoryListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#Inventorybody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}
$(document).ready(function () {
    fn_autoselect('#txtInventoryUnitType', '/ItemMaster/GetAllUnitType', '#txtInventoryUnitTypeHidden');
    fn_autoselect('#searchInventoryItemnameInput', '/ItemMaster/GetItemNameList', '#txtInventoryItemName');
});
clearCreatetxtInventoryText();
function clearCreatetxtInventoryText() {
    $('#searchInventoryItemnameInput').val('');
    $('#txtInventoryUnitType').val('');
    $('#searchInventoryItemname').val('');
    $('#txtInventoryQuantity').val('');
    $('#PRInventoryItemDescription').val('');
    const today = new Date().toISOString().split('T')[0];
    $('#txtInventoryPrdate').val(today);
    resetInventoryForm();
    $('#Inventoryheadingtext').html('Create Inventory');
    $('#addInventoryInfo').removeClass('d-none');
    $('#InventoryInfo').addClass('d-none');
    $('#addbtnInventory').show();
    $('#updatebtnInventory').hide();
}

var InventoryForm;
function resetInventoryForm() {
    if (InventoryForm) {
        InventoryForm.resetForm();
    }
}
$(document).ready(function () {
    $("#InventoryForm").validate({
        rules: {
            searchInventoryItemnameInput: "required",
            txtInventoryUnitType: "required",
            txtInventoryQuantity: "required",
        },
        highlight: function (element) {
            $(element).addClass('error-border');
        },
        unhighlight: function (element) {
            $(element).removeClass('error-border');
        },
        errorPlacement: function (error, element) {
            return true;
        }
    });
});


function SelectInventoryDetails(InventoryId, element) {
    siteloadershow();
    $('#Inventoryheadingtext').text('Inventory Details');
    $('#InventoryInfo').removeClass('d-none');
    $('#addInventoryInfo').addClass('d-none');
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/Sales/EditInventoryDetails?InventoryId=' + InventoryId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response) {
                $('#dspInventoryItem').val(response.itemName);
                $('#dspInventoryItemDescription').val(response.details);
                $('#dspInventoryUnitName').val(response.unitName);
                $('#dspInventoryQuantity').val(response.quantity);
                $('#dspInventoryIsApproved').prop('checked', response.isApproved);
            } else {
                siteloaderhide();
                toastr.error('Empty response received.');
            }
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });

}

function CreateInventory() {
    siteloadershow();
    if ($("#InventoryForm").valid()) {

        var objData = {
            UnitTypeId: $('#txtInventoryUnitTypeHidden').val(),
            Date: $('#txtInventoryPrdate').val(),
            ItemId: $('#txtInventoryItemName').val(),
            ItemName: $('#searchInventoryItemnameInput').val(),
            Details: $('#PRInventoryItemDescription').val(),
            Quantity: $('#txtInventoryQuantity').val(),
            CreatedBy: $('#txtInventoryCreatedBy').val(),
        }

        $.ajax({
            url: '/Sales/InsertInventoryDetails',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                if (Result.code == 200) {

                    InventoryTable();
                    toastr.success(Result.message);
                    clearCreatetxtInventoryText();
                } else {
                    toastr.error(Result.message);
                }
                siteloaderhide();
            }
        })
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}


function EditInventoryDetails(InventoryId) {

    siteloadershow();
    $.ajax({
        url: '/Sales/EditInventoryDetails?InventoryId=' + InventoryId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();         

            $('#txtInventoryId').val(response.id);
            $('#txtInventoryUnitTypeHidden').val(response.unitTypeId);
            $('#txtInventoryItemName').val(response.itemId);
            $('#searchInventoryItemnameInput').val(response.itemName);
            $('#PRInventoryItemDescription').val(response.details);
            $('#txtInventoryQuantity').val(response.quantity);
            $('#txtInventoryUnitType').val(response.unitName);
            const date = new Date(response.date);
            const formattedDate = [
                date.getFullYear(),
                String(date.getMonth() + 1).padStart(2, '0'),
                String(date.getDate()).padStart(2, '0')
            ].join('-');
            $('#txtInventoryPrdate').val(formattedDate);

            $('#Inventoryheadingtext').html('Edit Inventory');
            $('#addInventoryInfo').removeClass('d-none');
            $('#InventoryInfo').addClass('d-none');
            $('#addbtnInventory').hide();
            $('#updatebtnInventory').show();
            resetInventoryForm()
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function UpdateInventoryDetails() {

    siteloadershow();
    if ($("#InventoryForm").valid()) {

        var objData = {
            Id: $('#txtInventoryId').val(),
            UnitTypeId: $('#txtInventoryUnitTypeHidden').val(),
            Date: $('#txtInventoryPrdate').val(),
            ItemId: $('#txtInventoryItemName').val(),
            ItemName: $('#searchInventoryItemnameInput').val(),
            Details: $('#PRInventoryItemDescription').val(),
            Quantity: $('#txtInventoryQuantity').val(),
            CreatedBy: $('#txtInventoryCreatedBy').val(),
        }

        $.ajax({
            url: '/Sales/UpdateInventoryDetails',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                if (Result.code == 200) {

                    InventoryTable();
                    toastr.success(Result.message);
                    clearCreatetxtInventoryText();
                } else {
                    toastr.error(Result.message);
                }
                siteloaderhide();
            }
        })
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}

function DeleteInventory(InventoryId, ItemName, element) {
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    Swal.fire({
        title: "Are you sure want to delete this Inventory?",
        text: "To confirm, type the InventoryItem name below",
        input: 'text',
        inputPlaceholder: 'Enter the InventoryItem name to confirm',
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        confirmButtonClass: "btn btn-primary w-xs me-2 mt-2",
        cancelButtonClass: "btn btn-danger w-xs mt-2",
        buttonsStyling: false,
        showCloseButton: true, inputValidator: (value) => {

            if (!value) {
                return 'Please enter the InventoryItem name!';
            } else if (value !== ItemName) {
                return 'InventoryItem name mismatch! Please enter valid InventoryItem Name';
            }
        }
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Sales/DeleteInventoryDetails?InventoryId=' + InventoryId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {
                    siteloaderhide();
                    if (Result.code == 200) {
                        Swal.fire({
                            title: Result.message,
                            icon: 'success',
                            confirmButtonColor: '#3085d6',
                            confirmButtonText: 'OK'
                        }).then(function () {
                            window.location = '/Sales/CreateInventory';
                        })
                    }
                    else {
                        toastr.error(Result.message);
                    }
                   
                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete Inventory!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Inventory have no changes.!!😊',
                'error'
            );
        }
    });
}
function InventoryIsApproved(InventoryId) {

    var isChecked = $('#flexSwitchCheckChecked_' + InventoryId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to approve this Inventory?" : "Are you sure want to unapprove this Inventory?";

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
            $.ajax({
                url: '/Sales/ApproveInventoryDetails?InventoryId=' + InventoryId,
                type: 'Post',
                contentType: 'application/json;charset=utf-8;',
                dataType: 'json',
                success: function (Result) {
                    if (Result.code == 200) {
                        Swal.fire({
                            title: Result.message,
                            icon: 'success',
                            confirmButtonColor: '#3085d6',
                            confirmButtonText: 'OK'
                        }).then(function () {
                            window.location = '/Sales/CreateInventory';
                        })
                    }
                    else {
                        toastr.error(Result.message);
                    }
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Inventory have no changes.!!😊',
                'error'
            );
        }
    });
}


