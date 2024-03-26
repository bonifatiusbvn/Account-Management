AllPurchaseRequestListTable();
function AllPurchaseRequestListTable() {
    debugger
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
    debugger
    $('.row.ac-card').removeClass('active');
    $(element).closest('.row.ac-card').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/PurchaseRequest/DisplayPurchaseRequestDetails?PurchaseId=' + PurchaseId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            debugger
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