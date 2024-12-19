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
        }

        $.ajax({
            url: '/PurchaseMaster/CreatePurchaseRequest',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                if (Result.code == 200) {

                    AllPurchaseRequestListTable();
                    toastr.success(Result.message);
                    ClearPurchaseRequestTextBox();
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
ClearPurchaseRequestTextBox();
function ClearPurchaseRequestTextBox() {
    clearCreatePRText();
    $('#prNo').val('');
    $.ajax({
        url: '/PurchaseMaster/CheckPRNo',
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            $('#prNo').val(response);
        }, error: function () {
            siteloaderhide();
            toastr.error("Error in getting PR No");
        }
    });
    resetPRForm();
    $('#Inventoryheadingtext').html('Create Inventory');
    $('#addInventoryInfo').removeClass('d-none');
    $('#InventoryInfo').addClass('d-none');
    $('#addbtnInventory').show();
    $('#updatebtnInventory').hide();

    $('#searchItemname').select2({
        maximumSelectionLength: 1,
        theme: 'bootstrap4',

        closeOnSelect: true,
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        placeholder: $(this).data('placeholder'),
        allowClear: Boolean($(this).data('allow-clear')),
        dropdownParent: $("#CreatePurchaseRequest")
    });

    //$('#drpPRSiteAddress').select2({

    //    theme: 'bootstrap4',
    //    width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
    //    placeholder: $(this).data('placeholder'),
    //    allowClear: Boolean($(this).data('allow-clear')),
    //    dropdownParent: $("#CreatePurchaseRequest")
    //});
}
function clearCreatePRText() {
    $('#searchItemnameInput').val('');
    $('#txtUnitType').val('');
    $('#searchItemname').val('');
    $('#txtQuantity').val('');
    $('#txtPoSiteName').val('');
    $('#drpPRSiteAddress').empty();
    $('#PurchaseRequestId').val('');
    $('#PRItemDescription').val('');
    $('#txtPrdate').val('');
}

var PRForm;
$(document).ready(function () {
    $("#purchaseRequestForm").validate({
        rules: {
            searchItemnameInput: "required",
            txtUnitType: "required",
            txtQuantity: "required",
            txtPoSiteName: "required",
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



function resetPRForm() {
    if (PRForm) {
        PRForm.resetForm();
    }
}

function EditPurchaseRequestDetails(PurchaseId) {

    siteloadershow();
    $.ajax({
        url: '/PurchaseMaster/DisplayPurchaseRequestDetails?PurchaseId=' + PurchaseId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            siteloaderhide();
            $('#PRheadingtext').html('Edit PurchaseRequest');
            $('#addPRInfo').removeClass('d-none');
            $('#PRInfo').addClass('d-none');

            var dbDate = response.date.split('T')[0];
            $('#txtPrdate').val(dbDate);

            $('#addbtnpurchaseRequest').hide();
            $('#updatebtnpurchaseRequest').show();
            $('#PurchaseRequestId').val(response.pid);
            $('#txtUnitTypeHidden').val(response.unitTypeId);
            $('#prNo').val(response.prNo);
            $('#txtItemName').val(response.itemId);
            $('#searchItemnameInput').val(response.itemName);
            $('#PRItemDescription').val(response.itemDescription);
            $('#txtQuantity').val(response.quantity);
            $('#txtPoSiteName').val(response.siteId);
            $('#txtUnitType').val(response.unitName);

            fn_getPRSiteDetail(response.siteId, function () {
                $('#drpPRSiteAddress').val(response.siteAddressId);
            });

            resetPRForm()
            $('#searchItemname').select2({
                maximumSelectionLength: 1,
                theme: 'bootstrap4',
                width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
                placeholder: $(this).data('placeholder'),
                allowClear: Boolean($(this).data('allow-clear')),
                dropdownParent: $("#CreatePurchaseRequest")
            });


            //$('#drpPRSiteAddress').select2({
            //    theme: 'bootstrap4',
            //    width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
            //    placeholder: $(this).data('placeholder'),
            //    allowClear: Boolean($(this).data('allow-clear')),
            //    dropdownParent: $("#CreatePurchaseRequest")
            //});
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function UpdatePurchaseRequestDetails() {

    siteloadershow();
    if ($("#purchaseRequestForm").valid()) {

        var siteName = null;
        siteId = $("#SiteIdinPR").val();
        PRsiteId = $("#txtPoSiteName").val();
        if (PRsiteId != undefined) {
            siteName = PRsiteId;
        }
        else {
            siteName = siteId
        }
        var siteAddressId = $('#drpPRSiteAddress').val();
        var siteAddress = $('#drpPRSiteAddress option:selected').text();

        var objData = {
            SiteAddressId: siteAddressId,
            SiteAddress: siteAddress,
            Pid: $('#PurchaseRequestId').val(),
            CreatedBy: $('#txtcreatedby').val(),
            UnitTypeId: $('#txtUnitTypeHidden').val(),
            ItemId: $('#txtItemName').val(),
            ItemName: $('#searchItemnameInput').val(),
            ItemDescription: $('#PRItemDescription').val(),
            SiteId: siteName,
            Quantity: $('#txtQuantity').val(),
            PrNo: $('#prNo').val(),
            Date: $('#txtPrdate').val(),
        }

        $.ajax({
            url: '/PurchaseMaster/UpdatePurchaseRequestDetails',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                siteloaderhide();
                if (Result.code == 200) {
                    AllPurchaseRequestListTable();

                    toastr.success(Result.message);
                    ClearPurchaseRequestTextBox();
                    siteloaderhide();
                } else {
                    toastr.error(Result.message);
                }
                siteloaderhide();
            },
        })
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}

function DeletePurchaseRequest(PurchaseId, ItemName, element) {
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    Swal.fire({
        title: "Are you sure want to delete this Item?",
        text: "To confirm, type the Item name below",
        input: 'text',
        inputPlaceholder: 'Enter the Item name to confirm',
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        confirmButtonClass: "btn btn-primary w-xs me-2 mt-2",
        cancelButtonClass: "btn btn-danger w-xs mt-2",
        buttonsStyling: false,
        showCloseButton: true, inputValidator: (value) => {

            if (!value) {
                return 'Please enter the Item name!';
            } else if (value !== ItemName) {
                return 'Item name mismatch! Please enter valid Item Name';
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
                    toastr.error("Can't delete Purchaserequest!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Purchaserequest have no changes.!!😊',
                'error'
            );
        }
    });
}

function PurchaseRequestIsApproved(PurchaseId) {

    var isChecked = $('#flexSwitchCheckChecked_' + PurchaseId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to approve this purchaserequest?" : "Are you sure want to unapprove this purchaserequest?";

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
            formData.append("PurchaseId", PurchaseId);

            $.ajax({
                url: '/PurchaseMaster/PurchaseRequestIsApproved?PurchaseId=' + PurchaseId,
                type: 'Post',
                contentType: 'application/json;charset=utf-8;',
                dataType: 'json',
                success: function (Result) {


                    Swal.fire({
                        title: isChecked ? "Approved!" : "Unapproved!",
                        text: Result.message,
                        icon: "success",
                        confirmButtonClass: "btn btn-primary w-xs mt-2",
                        buttonsStyling: false
                    }).then(function () {
                        window.location = '/PurchaseMaster/PurchaseRequestListView';
                    });
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Purchaserequest have no changes.!!😊',
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
    debugger
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