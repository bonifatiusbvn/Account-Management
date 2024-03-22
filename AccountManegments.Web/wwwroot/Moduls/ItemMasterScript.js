AllItemTable();
GetAllUnitType();
function AllItemTable() {

    var searchText = $('#txtSearch').val();
    var searchBy = $('#ddlSearchBy').val();

    $.get("/ItemMaster/ItemListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {
            $("#itemtbody").html(result);
        })
        .fail(function (error) {
            console.error(error);
        });
}
function filterTable() {

    var searchText = $('#txtSearch').val();
    var searchBy = $('#ddlSearchBy').val();

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

function sortTable() {
    var sortBy = $('#ddlSortBy').val();
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
function SelectItemDetails(ItemId, element) {

    $('.row.ac-card').removeClass('active');
    $(element).closest('.row.ac-card').addClass('active');
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
                $('#dspWithGst').val(response.isWithGst);
                $('#dspGstAmount').val(response.gstamount);
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
function CreateItem() {

    var objData = {
        ItemName: $('#txtItemName').val(),
        UnitType: $('#txtUnitType').val(),
        PricePerUnit: $('#txtPricePerUnit').val(),
        IsWithGst: $('#txtPriceWithGst').val(),
        Gstamount: $('#txtGstAmount').val(),
        Gstper: $('#txtGstPerUnit').val(),
        Hsncode: $('#txtHSNCode').val(),
    }
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
function DisplayItemDetails(ItemId) {

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
function validateAndCreateItem() {

    resetErrorMessages();

    var ItemName = document.getElementById("txtItemName").value.trim();
    var UnitType = document.getElementById("txtUnitType").value.trim();
    var PricePerUnit = document.getElementById("txtPricePerUnit").value.trim();
    var GstAmount = document.getElementById("txtGstAmount").value.trim();
    var GstPerUnit = document.getElementById("txtGstPerUnit").value.trim();
    var HSNCode = document.getElementById("txtHSNCode").value.trim();


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
    }

    if (GstAmount === "") {
        document.getElementById("spnGstAmount").innerText = "Gst Amount is required.";
        isValid = false;
    }

    if (GstPerUnit === "") {
        document.getElementById("spnGstPerUnit").innerText = "GstPer Unit is required.";
        isValid = false;
    }

    if (HSNCode === "") {
        document.getElementById("spnHSNCode").innerText = "Hsn Code is required.";
        isValid = false;
    }

    if (isValid) {
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