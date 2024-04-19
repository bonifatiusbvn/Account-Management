AllItemTable();
GetAllUnitType();
function AllItemTable() {

    var searchText = $('#txtItemSearch').val();
    var searchBy = $('#ddlItemSearchBy').val();

    $.get("/ItemMaster/ItemListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {

            $("#itemtbody").html(result);
        })
        .fail(function (error) {
            console.error(error);
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
                console.log('Empty response received.');
            }
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}
function GetAllUnitType() {

    $.ajax({
        url: '/ItemMaster/GetAllUnitType',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#txtUnitType').append('<Option value=' + data.unitId + '>' + data.unitName + '</Option>')
            });
        }
    });
}
function ClearTextBox() {
    resetItemForm();
    $('#txtItemName').val('');
    $('#txtUnitType').val('');
    $('#txtPricePerUnit').val('');
    $('#txtPriceWithGst').val('');
    $('#txtGstAmount').val('');
    $('#txtGstPerUnit').val('');
    $('#txtHSNCode').val('');
    var button = document.getElementById("btnitem");
    if ($('#txtItemid').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreateItem'));
    offcanvas.show();
    $('#txtUnitType').select2({
        theme: 'bootstrap4',
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        placeholder: $(this).data('placeholder'),
        allowClear: Boolean($(this).data('allow-clear')),
        dropdownParent: $("#CreateItem")
    });
}
function CreateItem() {
    siteloadershow();
    if ($("#ItemMsterForm").valid()) {
        var objData = {
            ItemName: $('#txtItemName').val(),
            UnitType: $('#txtUnitType').val(),
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
                siteloaderhide();
                if (result.code == 200) {
                    Swal.fire({
                        title: result.message,
                        icon: 'success',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK'
                    }).then(function () {
                        window.location = '/ItemMaster/ItemListView';
                    });
                } else {
                    Swal.fire({
                        title: result.message,
                        icon: 'warning',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK'
                    });
                }
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
                Swal.fire({
                    title: 'Error',
                    text: 'An error occurred while processing your request.',
                    icon: 'error',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                });
            }
        });
    }
    else {
        Swal.fire({
            title: "Kindly fill all details",
            icon: 'warning',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'OK',
        })
    }
}

function EditItemDetails(ItemId) {
    siteloadershow();
    $.ajax({
        url: '/ItemMaster/DisplayItemDetails?ItemId=' + ItemId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            $('#txtItemid').val(response.itemId);
            $('#txtItemName').val(response.itemName);
            $('#txtUnitType').val(response.unitType);
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
            $('#txtUnitType').select2({
                theme: 'bootstrap4',
                width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
                placeholder: $(this).data('placeholder'),
                allowClear: Boolean($(this).data('allow-clear')),
                dropdownParent: $("#CreateItem")
            });
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}
function UpdateItemDetails() {
    siteloadershow();
    if ($("#ItemMsterForm").valid()) {
        var objData = {
            ItemId: $('#txtItemid').val(),
            ItemName: $('#txtItemName').val(),
            UnitType: $('#txtUnitType').val(),
            PricePerUnit: $('#txtPricePerUnit').val(),
            Gstamount: $('#txtGstAmount').val(),
            IsWithGst: $('#txtIsWithGst').prop('checked'),
            Gstper: $('#txtGstPerUnit').val(),
            Hsncode: $('#txtHSNCode').val(),
            IsApproved: $('#txtIsApproved').val(),
        }
        $.ajax({
            url: '/ItemMaster/UpdateItemDetails',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                siteloaderhide();
                Swal.fire({
                    title: Result.message,
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(function () {
                    window.location = '/ItemMaster/ItemListView';
                });
            },
        })
    }
    else {
        Swal.fire({
            title: "Kindly fill all details",
            icon: 'warning',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'OK',
        })
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


function ItemIsApproved(ItemId) {

    var isChecked = $('#flexSwitchCheckChecked_' + ItemId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to approve this Item?" : "Are you sure want to unapprove this Item?";

    Swal.fire({
        title: confirmationMessage,
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, Enter it!",
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
                    Swal.fire({
                        title: isChecked ? "Approved!" : "UnApproved!",
                        text: Result.message,
                        icon: "success",
                        confirmButtonClass: "btn btn-primary w-xs mt-2",
                        buttonsStyling: false
                    }).then(function () {
                        window.location = '/ItemMaster/ItemListView';
                    });
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Item Have No Changes.!!😊',
                'error'
            )
        }
    });
}

function deleteItemDetails(ItemId) {

    Swal.fire({
        title: "Are you sure want to Delete this?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, Delete it!",
        cancelButtonText: "No, cancel!",
        confirmButtonClass: "btn btn-primary w-xs me-2 mt-2",
        cancelButtonClass: "btn btn-danger w-xs mt-2",
        buttonsStyling: false,
        showCloseButton: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/ItemMaster/DeleteItemDetails?ItemId=' + ItemId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {

                    Swal.fire({
                        title: Result.message,
                        icon: 'success',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK'
                    }).then(function () {
                        window.location = '/ItemMaster/ItemListView';
                    })
                },
                error: function () {
                    Swal.fire({
                        title: "Can't Delete Item!",
                        icon: 'warning',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK',
                    })
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Item have no changes.!!😊',
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
    var fileUrl = '/uploadexcelfile/itemmasterdetails.xlsx';

    var link = document.createElement('a');

    link.href = fileUrl;

    link.setAttribute('download', '');

    document.body.appendChild(link);

    link.click();

    document.body.removeChild(link);
    siteloaderhide();
}
