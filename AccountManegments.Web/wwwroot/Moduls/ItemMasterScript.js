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
            $("#itemtbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function sortItemTable() {
    var sortBy = $('#ddlItemSortBy').val();
    $.ajax({
        url: '/ItemMaster/ItemListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            $("#itemtbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}
function DisplayItemDetails(ItemId, element) {

    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/ItemMaster/DisplayItemDetails?ItemId=' + ItemId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
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
    resetErrorMessages();
    $('#txtItemName').val('');
    $('#txtUnitType').val('');
    $('#txtPricePerUnit').val('');
    $('#txtPriceWithGst').val('');
    $('#txtGstAmount').val('');
    $('#txtGstPerUnit').val('');
    $('#txtHSNCode').val('');
    $('#txtIsApproved').val('');
    var button = document.getElementById("btnitem");
    if ($('#txtItemid').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreateItem'));
    offcanvas.show();
}
function CreateItem() {debugger

    var objData = {
        ItemName: $('#txtItemName').val(),
        UnitType: $('#txtUnitType').val(),
        PricePerUnit: $('#txtPricePerUnit').val(),
        IsWithGst: $('#txtIsWithGst').prop('checked'),
        Gstamount: $('#txtGstAmount').val(),
        Gstper: $('#txtGstPerUnit').val(),
        Hsncode: $('#txtHSNCode').val(),
    }
    debugger
    $.ajax({
        url: '/ItemMaster/CreateItem',
        type: 'post',
        data: objData,
        datatype: 'json',
        success: function (Result) {

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
function EditItemDetails(ItemId) {

    $.ajax({
        url: '/ItemMaster/DisplayItemDetails?ItemId=' + ItemId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

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
            resetErrorMessages()
            offcanvas.show();
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}
function UpdateItemDetails() {

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
function validateAndCreateItem() {debugger

    resetErrorMessages();

    var ItemName = document.getElementById("txtItemName").value.trim();
    var UnitType = document.getElementById("txtUnitType").value.trim();
    var PricePerUnit = document.getElementById("txtPricePerUnit").value.trim();
    var GstAmount = document.getElementById("txtGstAmount").value.trim();
    var GstPerUnit = document.getElementById("txtGstPerUnit").value.trim();

    var isValid = true;


    if (ItemName === "") {
        document.getElementById("spnItemName").innerText = "Item Name is required.";
        isValid = false;
    }


    if (UnitType === "") {
        document.getElementById("spnUnitType").innerText = "Unit Type is required.";
        isValid = false;
    }

    if (PricePerUnit === "") {
        document.getElementById("spnPricePerUnit").innerText = "PricePer Unit is required.";
        isValid = false;
    } else if (!isValidPriceInput(PricePerUnit)) {
        document.getElementById("spnPricePerUnit").innerText = "Please Enter Valid Price!";
        isValid = false;
    }

    if (GstAmount === "") {
        document.getElementById("spnGstAmount").innerText = "Gst Amount is required.";
        isValid = false;
    } else if (!isValidGSTInput(GstAmount)) {
        document.getElementById("spnGstAmount").innerText = "Please Enter Valid GST!";
        isValid = false;
    }

    if (GstPerUnit === "") {
        document.getElementById("spnGstPerUnit").innerText = "GstPer Unit is required.";
        isValid = false;
    }

    if (isValid) {debugger
        if ($("#txtItemid").val() == '') {
            CreateItem();
        }
        else {
            UpdateItemDetails()
        }
    }
}

function resetErrorMessages() {
    document.getElementById("spnItemName").innerText = "";
    document.getElementById("spnUnitType").innerText = "";
    document.getElementById("spnPricePerUnit").innerText = "";
    document.getElementById("spnGstAmount").innerText = "";
    document.getElementById("spnGstPerUnit").innerText = "";
    document.getElementById("spnHSNCode").innerText = "";
}
function isValidPriceInput(PricePerUnit) {

    var numberPattern = /^[0-9]+$/;
    return numberPattern.test(PricePerUnit);
}
function isValidGSTInput(GstAmount) {

    var numberPattern = /^\d+(\.\d+)?$/;
    return numberPattern.test(GstAmount);
}


function ItemIsApproved(ItemId) {
    debugger
    var isChecked = $('#flexSwitchCheckChecked_' + ItemId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to Approve this Item?" : "Are you sure want to UnApprove this Item?";

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
            debugger
            $.ajax({
                url: '/ItemMaster/ItemIsApproved?ItemId=' + ItemId,
                type: 'Post',
                contentType: 'application/json;charset=utf-8;',
                dataType: 'json',
                success: function (Result) {
                    Swal.fire({
                        title: isChecked ? "Active!" : "DeActive!",
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
            ).then(function () {
                window.location = '/ItemMaster/ItemListView';
            });;
        }
    });
}

function deleteItemDetails(ItemId) {

    Swal.fire({
        title: "Are you sure want to Delete This?",
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
                    }).then(function () {
                        window.location = '/ItemMaster/ItemListView';
                    })
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Item Have No Changes.!!😊',
                'error'
            );
        }
    });
}

// Define the function
function WithGSTSelected() {
    var isWithGstCheckbox = document.getElementById('txtIsWithGst');
    var gstAmountInput = document.getElementById('txtGstAmount');
    var gstPercentageInput = document.getElementById('txtGstPerUnit');
    var priceInput = document.getElementById('txtPricePerUnit');


    if (isWithGstCheckbox.checked) {
        var price = parseFloat(priceInput.value); 
        var gstPercentage = parseFloat(gstPercentageInput.value); 
        gstAmountInput.disabled = true;
        if (!isNaN(price) && !isNaN(gstPercentage)) {
            var gstAmount = (gstPercentage / 100) * price;
            gstAmountInput.value = gstAmount.toFixed(2);
            
        } else {
            gstAmountInput.value = ""; 
            gstAmountInput.disabled = true;
        }
    }
}

document.getElementById('txtGstPerUnit').addEventListener('input', function () {
    WithGSTSelected();
});

document.addEventListener("DOMContentLoaded", function () {
    debugger
    var closeButton = document.getElementById("errorCloseButton");
    if (closeButton) {
        closeButton.addEventListener("click", function () {
            window.location = '/ItemMaster/ItemListView';
        });
    }
});
