function AllPurchaseRequestListTable() {

    var searchText = $('#txtPurchaseRequestSearch').val();
    var searchBy = $('#PurchaseRequestSearchBy').val();

    $.get("/PurchaseMaster/PurchaseRequestListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {

            $("#purchaseRequesttbody").html(result);
        })

}

function filterPurchaseRequestTable() {
    siteloadershow();
    var searchText = $('#txtPurchaseRequestSearch').val();
    var searchBy = $('#PurchaseRequestSearchBy').val();

    $.ajax({
        url: '/PurchaseMaster/PurchaseRequestListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#purchaseRequesttbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}
function sortPurchaseRequestTable() {
    siteloadershow();
    var sortBy = $('#PurchaseRequestSortBy').val();
    $.ajax({
        url: '/PurchaseMaster/PurchaseRequestListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#purchaseRequesttbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}

function SelectPurchaseRequestDetails(PurchaseId, element) {
    siteloadershow();
    $('#PRheadingtext').text('PurchaseRequest Details');
    $('#PRInfo').removeClass('d-none');
    $('#addPRInfo').addClass('d-none');
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/PurchaseMaster/DisplayPurchaseRequestDetails?PurchaseId=' + PurchaseId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response) {
                $('#dspPrNo').val(response.prNo);
                $('#dspPId').val(PurchaseId);
                $('#dspItem').val(response.itemName);
                $('#dspItemDescription').val(response.itemDescription);
                $('#dspUnitName').val(response.unitName);
                $('#dspQuantity').val(response.quantity);
                $('#dspSiteName').val(response.siteName);
                $('#dspPrSiteAddress').val(response.siteAddress);
                $('#dspIsApproved').prop('checked', response.isApproved);
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
            ItemDescription: $('#PRInventoryItemDescription').val(),
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

                    AllPurchaseRequestListTable();
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
clearCreatetxtInventoryText();
function clearCreatetxtInventoryText() {
    $('#searchInventoryItemnameInput').val('');
    $('#txtInventoryUnitType').val('');
    $('#searchInventoryItemname').val('');
    $('#txtInventoryQuantity').val('');
    $('#txtInventoryPrdate').val('');

    resetInventoryForm();
    $('#Inventoryheadingtext').html('Create Inventory');
    $('#addInventoryInfo').removeClass('d-none');
    $('#InventoryInfo').addClass('d-none');
    $('#addbtnInventory').show();
    $('#updatebtnInventory').hide();
}

var InventoryForm;
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



function resetInventoryForm() {
    if (InventoryForm) {
        InventoryForm.resetForm();
    }
}

function EditInventoryDetails(InventoryId) {

    siteloadershow();
    $.ajax({
        url: '/PurchaseMaster/DisplayPurchaseRequestDetails?InventoryId=' + InventoryId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            siteloaderhide();
            $('#Inventoryheadingtext').html('Edit PurchaseRequest');
            $('#addInventoryInfo').removeClass('d-none');
            $('#InventoryInfo').addClass('d-none');

            var dbDate = response.date.split('T')[0];
            $('#txtInventoryPrdate').val(dbDate);

            $('#addbtnInventory').hide();
            $('#updatebtnInventory').show();
            $('#InventoryId').val(response.pid);
            $('#txtInventoryUnitTypeHidden').val(response.unitTypeId);
            $('#txtInventoryItemName').val(response.itemId);
            $('#searchInventoryItemnameInput').val(response.itemName);
            $('#PRInventoryItemDescription').val(response.itemDescription);
            $('#txtInventoryQuantity').val(response.quantity);
            $('#txtInventoryUnitType').val(response.unitName);
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
            UnitTypeId: $('#txtInventoryUnitTypeHidden').val(),
            Date: $('#txtInventoryPrdate').val(),
            ItemId: $('#txtInventoryItemName').val(),
            ItemName: $('#searchInventoryItemnameInput').val(),
            ItemDescription: $('#PRInventoryItemDescription').val(),
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

                    AllPurchaseRequestListTable();
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
                url: '/PurchaseMaster/DeletePurchaseRequest?PurchaseId=' + PurchaseId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {
                    siteloaderhide();
                    Swal.fire({
                        title: Result.message,
                        icon: 'success',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK'
                    }).then(function () {
                        window.location = '/PurchaseMaster/PurchaseRequestListView';
                    })
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
$(document).ready(function () {
    fn_autoselect('#txtInventoryUnitType', '/ItemMaster/GetAllUnitType', '#txtInventoryUnitTypeHidden');
    fn_autoselect('#searchInventoryItemnameInput', '/ItemMaster/GetItemNameList', '#txtInventoryItemName');
});

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

function EditInventoryRequestDetails() {

}

function DeleteInventoryRequest() {

}

function SelectInventoryRequestDetails() {

}