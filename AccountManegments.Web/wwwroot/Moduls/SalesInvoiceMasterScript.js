SalesInvoicesortTable();
function companyfilterSalesInvoice() {
    siteloadershow();
    var searchText = $('#ddlInvoiceCompanyName').val();
    var searchBy = "AscendingCompanyName";

    $.ajax({
        url: '/Sales/SalesInvoiceListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#SalesInvoicebody").html(result);
        },

    });
}


let currentSortOrder = 'AscendingDate';

function sortTable(field) {
    if (currentSortOrder === 'Ascending' + field) {
        currentSortOrder = 'Descending' + field;
    } else {
        currentSortOrder = 'Ascending' + field;
    }

    siteloadershow();

    $.ajax({
        url: '/Sales/SalesInvoiceListAction',
        type: 'GET',
        data: {
            sortBy: currentSortOrder,
        },
        success: function (result) {
            siteloaderhide();
            $("#SalesInvoicebody").html(result);
        },
        error: function (xhr, status, error) {
            console.error("Error fetching sorted data:", error);
            siteloaderhide();
        }
    });
}

function filterSalesInvoiceTable() {
    debugger
    siteloadershow();
    var searchText = $('#txtSupplierInvoiceSearch').val();
    var searchBy = $('#SupplierInvoiceSearchBy').val();

    $.ajax({
        url: '/Sales/SalesInvoiceListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#SalesInvoicebody").html(result);
        },

    });
}

function SalesInvoicesortTable() {
    debugger
    siteloadershow();
    var sortBy = $('#SortBySupplierInvoice').val();
    $.ajax({
        url: '/Sales/SalesInvoiceListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#SalesInvoicebody").html(result);
        },

    });
}