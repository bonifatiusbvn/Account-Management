AllItemTable();

function AllItemTable() {

    var searchText = $('#txtItemSearch').val();
    var searchBy = $('#ddlItemSearchBy').val();

    $.get("/ItemMaster/ItemListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {

            $("#itemtbody").html(result);
        })
        .fail(function (error) {
            siteloaderhide();

        });
}
function filterItemTable() {
    siteloadershow();
    var searchText = $('#txtItemSearch').val();
    var searchBy = $('#ddlItemSearchBy').val();

    $.ajax({
        url: '/ItemMaster/ItemListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#itemtbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}

function sortItemTable() {
    siteloadershow();
    var sortBy = $('#ddlItemSortBy').val();
    $.ajax({
        url: '/ItemMaster/ItemListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#itemtbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}

function ItemHistory(ItemId, element) {
    siteloadershow();
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/ItemMaster/GetItemHistory',
        type: 'Post',
        data: {
            ItemId: ItemId
        },
        success: function (result) {
            siteloaderhide();
            $("#ItemDetailsView").hide();
            $("#ItemDetailsView").hide();
            $("#ItemHistoryView").html(result).removeClass('d-none').show();
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}

function DisplayItemDetails(ItemId, element) {
    siteloadershow();
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/ItemMaster/DisplayItemDetails?ItemId=' + ItemId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response) {
                $("#ItemDetailsView").removeClass('d-none').show();
                $("#ItemHistoryView").hide();
                $('#dspItemId').val(response.itemId);
                $('#dspItemName').val(response.itemName);
                $('#dspUnitName').val(response.unitTypeName);
                $('#dspPricePerUnit').val(response.pricePerUnit);
                $('#dspWithGst').prop('checked', response.isWithGst);
                $('#dspGstAmount').val(response.gstamount);
                $('#dspGstPercentage').val(response.gstper);
                $('#dspHsnCode').val(response.hsncode);
                $('#dspIsApproved').val(response.isApproved);
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
$(document).ready(function () {
    function GetAllUnitType() {
        $.ajax({
            url: '/ItemMaster/GetAllUnitType',
            method: 'GET',
            success: function (result) {
                var unitTypes = result.map(function (data) {
                    return {
                        label: data.unitName,
                        value: data.unitId
                    };
                });


                $("#txtUnitType").autocomplete({
                    source: unitTypes,
                    minLength: 0,
                    select: function (event, ui) {

                        event.preventDefault();
                        $("#txtUnitType").val(ui.item.label);
                        $("#txtUnitTypeHidden").val(ui.item.value);

                    },
                    focus: function () {
                        return false;
                    }
                }).focus(function () {
                    $(this).autocomplete("search", "");
                });
            },
            error: function (err) {
                console.error("Failed to fetch unit types: ", err);
            }
        });
    }

    GetAllUnitType();
    function GetAllItemName() {
        $.ajax({
            url: '/ItemMaster/GetItemNameList',
            method: 'GET',
            success: function (result) {
                var ItemName = result.map(function (data) {
                    return {
                        label: data.itemName,
                    };
                });

                $("#txtItemName").autocomplete({
                    source: ItemName,
                    minLength: 0,
                    select: function (event, ui) {
                        event.preventDefault();
                        $("#txtItemName").val(ui.item.label);
                    },
                    focus: function () {
                        return false;
                    }
                }).focus(function () {
                    $(this).autocomplete("search", "");
                });
            },
            error: function (err) {
                console.error("Failed to fetch unit types: ", err);
            }
        });
    }
    GetAllItemName();
});

function ClearTextBox() {
    resetItemForm();
    $('#changeName').html('Create Item');
    $('#txtItemName').val('');
    $('#txtUnitType').val('');
    $('#txtPricePerUnit').val('');
    $('#txtPriceWithGst').val('');
    $('#txtGstAmount').val('');
    $('#txtGstPerUnit').val('');
    $('#txtHSNCode').val('');
    $('#txtItemid').val('');
    var button = document.getElementById("btnitem");
    if ($('#txtItemid').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreateItem'));
    offcanvas.show();

}
function CreateItem() {
    siteloadershow();

    if ($("#ItemMsterForm").valid()) {
        var objData = {
            ItemName: $('#txtItemName').val(),
            UnitType: $('#txtUnitTypeHidden').val(),
            PricePerUnit: $('#txtPricePerUnit').val(),
            IsWithGst: $('#txtIsWithGst').prop('checked'),
            Gstamount: $('#txtGstAmount').val(),
            Gstper: $('#txtGstPerUnit').val(),
            Hsncode: $('#txtHSNCode').val(),
        };

        $.ajax({
            url: '/ItemMaster/CreateItem',
            type: 'POST',
            data: objData,
            dataType: 'json',
            success: function (result) {
                if (result.code == 200) {
                    var offcanvasElement = document.getElementById('CreateItem');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {

                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllItemTable();
                    toastr.success(result.message);
                } else {
                    toastr.error(result.message);
                }
                siteloaderhide();
            },
            error: function (xhr, status, error) {

                siteloaderhide();
                toastr.error('An error occurred while processing your request.');
            }
        });
    } else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}


function EditItemDetails(ItemId, element) {

    siteloadershow();
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/ItemMaster/DisplayItemDetails?ItemId=' + ItemId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            siteloaderhide();
            $('#changeName').html('Update Item');
            $('#txtItemid').val(response.itemId);
            $('#txtItemName').val(response.itemName);
            $('#txtUnitTypeHidden').val(response.unitType);
            $('#txtUnitType').val(response.unitTypeName);
            $('#txtPricePerUnit').val(response.pricePerUnit);
            $('#txtIsWithGst').prop('checked', response.isWithGst);
            $('#txtGstAmount').val(response.gstamount);
            $('#txtGstPerUnit').val(response.gstper);
            $('#txtHSNCode').val(response.hsncode);
            $('#txtIsApproved').val(response.isApproved);
            var button = document.getElementById("btnitem");
            if ($('#txtItemid').val() != '') {
                button.textContent = "Update";
            }
            var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreateItem'));
            resetItemForm()
            offcanvas.show();

        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}
function UpdateItemDetails() {

    siteloadershow();
    if ($("#ItemMsterForm").valid()) {
        var objData = {
            ItemId: $('#txtItemid').val(),
            ItemName: $('#txtItemName').val(),
            UnitType: $('#txtUnitTypeHidden').val(),
            PricePerUnit: $('#txtPricePerUnit').val(),
            Gstamount: $('#txtGstAmount').val(),
            IsWithGst: $('#txtIsWithGst').prop('checked'),
            Gstper: $('#txtGstPerUnit').val(),
            Hsncode: $('#txtHSNCode').val(),
            IsApproved: $('#txtIsApproved').val(),
        };

        $.ajax({
            url: '/ItemMaster/UpdateItemDetails',
            type: 'post',
            data: objData,
            dataType: 'json',
            success: function (result) {
                if (result.code == 200) {
                    var offcanvasElement = document.getElementById('CreateItem');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {
                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllItemTable();
                    toastr.success(result.message);
                } else {
                    toastr.error(result.message);
                }
                siteloaderhide();
            },
            error: function (xhr, status, error) {
                siteloaderhide();
                toastr.error('An error occurred while processing your request.');
            }
        });
    } else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}


var ItemForm;
function validateAndCreateItem() {

    ItemForm = $("#ItemMsterForm").validate({
        rules: {
            txtItemName: "required",
            txtUnitType: "required",
            txtPricePerUnit: "required",
            txtGstAmount: "required",
            txtGstPerUnit: "required",
        },
        messages: {
            txtItemName: "Please Enter ItemName",
            txtUnitType: "Please Enter UnitType",
            txtPricePerUnit: "Please Enter PricePerUnit",
            txtGstAmount: "Please Enter GstAmount",
            txtGstPerUnit: "Please Enter GstPerUnit",
        }
    })
    var isValid = true;

    if (isValid) {
        if ($("#txtItemid").val() == '') {
            CreateItem();
        }
        else {
            UpdateItemDetails()
        }
    }
}
function resetItemForm() {
    if (ItemForm) {
        ItemForm.resetForm();
    }
}


function ItemIsApproved(ItemId, element) {
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');

    var isChecked = $('#flexSwitchCheckChecked_' + ItemId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to approve this item?" : "Are you sure want to unapprove this item?";

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
            formData.append("ItemId", ItemId);
            $.ajax({
                url: '/ItemMaster/ItemIsApproved?ItemId=' + ItemId,
                type: 'Post',
                contentType: 'application/json;charset=utf-8;',
                dataType: 'json',
                success: function (Result) {
                    if (Result.code == 200) {
                        siteloaderhide();
                        Swal.fire({
                            title: isChecked ? "Approved!" : "Unapproved!",
                            text: Result.message,
                            icon: "success",
                            confirmButtonClass: "btn btn-primary w-xs mt-2",
                            buttonsStyling: false
                        }).then(function () {
                            window.location = '/ItemMaster/ItemListView';
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
                'Item have no changes.!!😊',
                'error'
            )
        }
    });
}

function deleteItemDetails(ItemId) {
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    Swal.fire({
        title: "Are you sure you want to delete this item?",
        text: "To confirm, type the item name below ",
        input: 'text',
        inputPlaceholder: 'Enter the item name to confirm',
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        confirmButtonClass: "btn btn-primary w-xs me-2 mt-2",
        cancelButtonClass: "btn btn-danger w-xs mt-2",
        buttonsStyling: false,
        showCloseButton: true,
        inputValidator: (value) => {

            if (!value) {
                return 'Please enter the item name!';
            } else if (value !== ItemName) {
                return 'Item name mismatch! Please enter valid Item Name';
            }
        }
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: '/ItemMaster/DeleteItemDetails?ItemId=' + ItemId,
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
                            window.location = '/ItemMaster/ItemListView';
                        });
                    } else {
                        toastr.error(Result.message);
                    }
                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete item!");
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Item has not been deleted.😊',
                'error'
            );
        }
    });
}



function WithGSTSelected() {
    var isWithGstCheckbox = document.getElementById('txtIsWithGst');
    var gstAmountInput = document.getElementById('txtGstAmount');
    var gstPercentageInput = document.getElementById('txtGstPerUnit');
    var priceInput = document.getElementById('txtPricePerUnit');

    var price = parseFloat(priceInput.value);
    var gstPercentage = parseFloat(gstPercentageInput.value);

    if (isWithGstCheckbox.checked) {

        if (!isNaN(price) && !isNaN(gstPercentage)) {
            var totalAmount = 100 + gstPercentage;
            var baseAmount = price - (price * gstPercentage / totalAmount);
            var gstAmount = price - baseAmount;
            gstAmountInput.value = gstAmount.toFixed(2);
            priceInput.value = baseAmount.toFixed(2);
        } else {
            gstAmountInput.value = "";
        }
    } else {

        if (!isNaN(price) && !isNaN(gstPercentage)) {
            var Amount = (gstPercentage / 100) * price;
            gstAmountInput.value = Amount.toFixed(2);
        } else {
            gstAmountInput.value = "";
        }
    }
}

document.getElementById('txtGstPerUnit').addEventListener('input', function () {
    WithGSTSelected();
});
document.getElementById('txtPricePerUnit').addEventListener('input', function () {
    WithGSTSelected();
});
document.getElementById('txtIsWithGst').addEventListener('change', function () {
    WithGSTSelected();
});



document.addEventListener("DOMContentLoaded", function () {

    var closeButton = document.getElementById("errorCloseButton");
    if (closeButton) {
        closeButton.addEventListener("click", function () {
            window.location = '/ItemMaster/ItemListView';
        });
    }
});
function downloadFile() {
    siteloadershow();
    var fileUrl = '/uploadexcelfile/ItemMastersDetails.xlsx';

    var link = document.createElement('a');

    link.href = fileUrl;

    link.setAttribute('download', '');

    document.body.appendChild(link);

    link.click();

    document.body.removeChild(link);
    siteloaderhide();
}

var UploadExcelFile;
$(document).ready(function () {
    UploadExcelFile = $("#uploadItemFile").validate({
        rules: {
            itemExcelFile: "required"
        },
        messages: {
            itemExcelFile: "Upload File"
        }
    })
});

function UploadItemFile() {

    if ($("#uploadItemFile").valid()) {
        var formData = new FormData();
        formData.append("FormFile", $("#itemExcelFile")[0].files[0]);

        $.ajax({
            url: '/ItemMaster/ImportExcelFile',
            type: 'POST',
            data: formData,
            dataType: 'json',
            processData: false,
            contentType: false,
            success: function (Result) {
                siteloaderhide();
                if (Result.code == 200) {
                    toastr.success(Result.message);
                    setTimeout(function () {
                        window.location = '/ItemMaster/ItemListView';
                    }, 2000);
                } else {
                    toastr.error(Result.message);
                }
            },
            error: function (xhr, status, error) {
                siteloaderhide();
                toastr.error("An error occurred while uploading the file: " + error);
            }
        });
    } else {
        siteloaderhide();
        toastr.error("Kindly upload a file.");
    }
}

function ResetButton() {
    if (UploadExcelFile) {
        UploadExcelFile.resetForm();
    }
}

function clearExcelFileModel() {
    ResetButton();
    $("#itemExcelFile").val('');
}
function OpenFileUploadModel() {
    clearExcelFileModel();
    $("#uploadExcelFileModal").modal('show');
}