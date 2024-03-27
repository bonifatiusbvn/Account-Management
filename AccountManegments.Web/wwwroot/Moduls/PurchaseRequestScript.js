AllPurchaseRequestListTable();
GetSiteDetails()
GetAllUnitType()

function AllPurchaseRequestListTable() {
    var searchText = $('#txtPurchaseRequestSearch').val();
    var searchBy = $('#PurchaseRequestSearchBy').val();

    $.get("/PurchaseRequest/PurchaseRequestListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {


            $("#purchaseRequesttbody").html(result);
        })
        .fail(function (error) {
            console.error(error);
        });
}

function filterPurchaseRequestTable() {

    var searchText = $('#txtPurchaseRequestSearch').val();
    var searchBy = $('#PurchaseRequestSearchBy').val();

    $.ajax({
        url: '/PurchaseRequest/PurchaseRequestListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            $("#purchaseRequesttbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function sortPurchaseRequestTable() {
    var sortBy = $('#PurchaseRequestSortBy').val();
    $.ajax({
        url: '/PurchaseRequest/PurchaseRequestListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            $("#purchaseRequesttbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function SelectPurchaseRequestDetails(PurchaseId, element) {

    $('.row.ac-card').removeClass('active');
    $(element).closest('.row.ac-card').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/PurchaseRequest/DisplayPurchaseRequestDetails?PurchaseId=' + PurchaseId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            if (response) {
                $('#dspPId').val(PurchaseId);
                $('#dspItem').val(response.item);
                $('#dspUnitName').val(response.unitName);
                $('#dspQuantity').val(response.quantity);
                $('#dspSiteName').val(response.siteName);
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

function CreatePurchaseRequest() {
    debugger
    var objData = {
        UnitTypeId: $('#txtUnitType').val(),
        Item: $('#txtItemName').val(),
        SiteId: $('#txtSiteName').val(),
        Quantity:$('#txtQuantity').val(),
        PrNo: $('#prNo').val(),
    }
    $.ajax({
        url: '/PurchaseRequest/CreatePurchaseRequest',
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
                window.location = '/PurchaseRequest/PurchaseRequestListView';
            });
        },
    })
}

function ClearPurchaseRequestTextBox() {
    debugger
    resetErrorsMessages();
    $('#txtItemName').val('');
    $('#txtQuantity').val('');

    var button = document.getElementById("btnpurchaseRequest");
    if ($('#PurchaseRequestId').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreatePurchaseRequest'));
    offcanvas.show();
}

function validateAndCreatePurchaseRequest() {
    debugger
    resetErrorsMessages();
    var UnitTypeId = document.getElementById("txtUnitType").value.trim();
    var ItemName = document.getElementById("txtItemName").value.trim();
    var SiteId = document.getElementById("txtSiteName").value.trim();
    var Quantity = document.getElementById("txtQuantity").value.trim();

    var isValid = true;

    if (UnitTypeId === "") {
        document.getElementById("spnUnitType").innerText = "Unit Type is required.";
        isValid = false;
    } else if (UnitTypeId === "--Select Unit Type--") {
        document.getElementById("spnUnitType").innerText = "Unit Type is required.";
        isValid = false;
    }


    if (ItemName === "") {
        document.getElementById("spnAddress").innerText = "Item is required.";
        isValid = false;
    } 


    if (SiteId === "") {
        document.getElementById("spnSiteName").innerText = "Site is required.";
        isValid = false;
    } else if (SiteId === "--Select SiteName--") {
        document.getElementById("spnSiteName").innerText = "Site is required.";
        isValid = false;
    } 


    if (Quantity === "") {
        document.getElementById("spnQuantity").innerText = "Quantity is required.";
        isValid = false;
    } 
   

    if (isValid) {
        if ($("#PurchaseRequestId").val() == '') {
            CreatePurchaseRequest();
        }
        else {
            UpdatePurchaseRequestDetails();
        }
    }
}

function resetErrorsMessages() {
    document.getElementById("txtUnitType").innerText = "";
    document.getElementById("txtItemName").innerText = "";
    document.getElementById("txtSiteName").innerText = "";
    document.getElementById("txtQuantity").innerText = ""; 
}

function EditPurchaseRequestDetails(PurchaseId) {

    $.ajax({
        url: '/PurchaseRequest/DisplayPurchaseRequestDetails?PurchaseId=' + PurchaseId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            $('#PurchaseRequestId').val(response.itemId);
            $('#prNo').val(response.itemName);
            $('#txtUnitType').val(response.unitType);
            $('#txtItemName').val(response.pricePerUnit);
            $('#txtSiteName').prop('checked', response.isWithGst);
            $('#txtQuantity').val(response.gstamount);

            var button = document.getElementById("btnpurchaseRequest");
            if ($('#PurchaseRequestId').val() != '') {
                button.textContent = "Update";
            }
            var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreatePurchaseRequest'));
            resetErrorMessages()
            offcanvas.show();
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}
