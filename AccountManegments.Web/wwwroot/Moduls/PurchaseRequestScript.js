AllPurchaseRequestListTable();

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
                $('#dspPrNo').val(response.prNo);
                $('#dspPId').val(PurchaseId);
                $('#dspItem').val(response.item);
                $('#dspUnitName').val(response.unitName);
                $('#dspQuantity').val(response.quantity);
                $('#dspSiteName').val(response.siteName);
                $('#dspIsApproved').prop('checked', response.isApproved);
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
    resetErrorsMessages();
    $('#txtItemName').val('');
    $('#txtUnitType').val('');
    $('#txtQuantity').val('');
    $('#txtSiteName').val('').prop('disabled', false);
    var button = document.getElementById("btnpurchaseRequest");
    if ($('#PurchaseRequestId').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreatePurchaseRequest'));
    offcanvas.show();
}

function validateAndCreatePurchaseRequest() {
  
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
        document.getElementById("spnItemName").innerText = "Item is required.";
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
    document.getElementById("spnUnitType").innerText = "";
    document.getElementById("spnItemName").innerText = "";
    document.getElementById("spnSiteName").innerText = "";
    document.getElementById("spnQuantity").innerText = ""; 
}

function EditPurchaseRequestDetails(PurchaseId) {
    
    $.ajax({
        url: '/PurchaseRequest/DisplayPurchaseRequestDetails?PurchaseId=' + PurchaseId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            $('#PurchaseRequestId').val(response.pid);
            $('#prNo').val(response.prNo);
            $('#txtItemName').val(response.item);
            $('#txtQuantity').val(response.quantity);
            $(document).ready(function () {
                if ($('#txtUserRole').val() == '9') {
                    $('#txtSiteName').val(response.siteId).prop('disabled', false);
                } else {
                    $('#txtSiteName').val(response.siteId).prop('disabled', true);
                }
            });
            $('#txtUnitType').val(response.unitTypeId);
            $('#txtSiteName').val(response.siteId);
            var button = document.getElementById("btnpurchaseRequest");
            if ($('#PurchaseRequestId').val() != '') {
                button.textContent = "Update";
            }
            var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreatePurchaseRequest'));
            resetErrorsMessages()
            offcanvas.show();
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

function UpdatePurchaseRequestDetails() {

    var objData = {
        Pid: $('#PurchaseRequestId').val(),
        UnitTypeId: $('#txtUnitType').val(),
        Item: $('#txtItemName').val(),
        SiteId: $('#txtSiteName').val(),
        Quantity: $('#txtQuantity').val(),
        PrNo: $('#prNo').val(),
    }

    $.ajax({
        url: '/PurchaseRequest/UpdatePurchaseRequestDetails',
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

function DeletePurchaseRequest(PurchaseId) {
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
                url: '/PurchaseRequest/DeletePurchaseRequest?PurchaseId=' + PurchaseId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {

                    Swal.fire({
                        title: Result.message,
                        icon: 'success',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK'
                    }).then(function () {
                        window.location = '/PurchaseRequest/PurchaseRequestListView';
                    })
                },
                error: function () {
                    Swal.fire({
                        title: "Can't Delete PurchaseRequest!",
                        icon: 'warning',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK',
                    }).then(function () {
                        window.location = '/PurchaseRequest/PurchaseRequestView';
                    })
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Purchase Request Have No Changes.!!😊',
                'error'
            );
        }
    });
}

function PurchaseRequestIsApproved(PurchaseId) {
   
    var isChecked = $('#flexSwitchCheckChecked_' + PurchaseId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to Approve this Purchase Request?" : "Are you sure want to UnApprove this Purchase Request?";

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
            formData.append("PurchaseId", PurchaseId);
      
            $.ajax({
                url: '/PurchaseRequest/PurchaseRequestIsApproved?PurchaseId=' + PurchaseId,
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
                        window.location = '/PurchaseRequest/PurchaseRequestListView';
                    });
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Purchase Request Have No Changes.!!😊',
                'error'
            ).then(function () {
                window.location = '/PurchaseRequest/PurchaseRequestListView';
            });;
        }
    });
}