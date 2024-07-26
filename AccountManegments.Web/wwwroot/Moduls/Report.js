GetAllSiteList();
GetAllCompanyList();
GetAllSupplierList();

function GetAllSiteList() {
    $.ajax({
        url: '/SiteMaster/GetSiteNameList',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#textReportSiteName').append('<Option value=' + data.siteId + '>' + data.siteName + '</Option>')
            });
        }
    });
}

function GetAllCompanyList() {
    $.ajax({
        url: '/Company/GetCompanyNameList',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#textReportCompanyName').append('<option value="' + data.companyId + '">' + data.companyName + '</option>');
            });
        }
    });
}

function GetAllSupplierList() {
    $.ajax({
        url: '/Supplier/GetSupplierNameList',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#textReportSupplierName').append('<Option value=' + data.supplierId + '>' + data.supplierName + '</Option>')
            });
        }
    });
}

var selectedSiteId = null;
var selectedCompanyId = null;
var selectedSupplierId = null;
var selectedstartDate = null;
var selectedendDate = null;
var selectedfilterType = null;

$(document).ready(function () {
    $('#textReportSupplierName').change(function () {
        selectedSupplierId = $(this).val();
        GetInvoiceReportData(); 
    });

    $('#textReportSiteName').change(function () {
        selectedSiteId = $(this).val();
        GetInvoiceReportData(); 
    });

    $('#textReportCompanyName').change(function () {
        selectedCompanyId = $(this).val();
        GetInvoiceReportData();
    });

    $('.nav-radio').click(function () {
        var targetTab = $(this).attr('href');
        if (targetTab === '#GetCurrentMonthInvoicelist') {
            GetCurrentMonthInvoiceList();
        }
    });

    $('.nav-btn').click(function () {
        var targetTab = $(this).attr('href');
        if (targetTab === '#GetBetweenDatesList') {
            GetBetweenDateInvoiceList();
        }
    });
});

function GetInvoiceReportData()
{
    if (selectedSiteId || selectedCompanyId || selectedSupplierId) {
        var objData = {
            SiteId: selectedSiteId,
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            startDate: selectedstartDate,
            endDate: selectedendDate
        }
        $.ajax({
            type: "post",
            url: '/InvoiceMaster/GetSupplierInvoiceDetailsReport',
            data: objData,
            datatype: 'json',
            success: function (result) {
                $("#reportInvoiceListbody").html(result);
            },
            error: function (xhr, status, error) {
                console.error('AJAX Error: ', status, error);
            }
        });
    }
}

function GetCurrentMonthInvoiceList() {

    selectedfilterType = "currentMonth";

    var objData = {
        SiteId: selectedSiteId,
        CompanyId: selectedCompanyId,
        SupplierId: selectedSupplierId,
        filterType: selectedfilterType
    }

    $.ajax({
        type: "post",
        url: '/InvoiceMaster/GetSupplierInvoiceDetailsReport',
        data: objData,
        datatype: 'json',
        success: function (result) {
            $("#reportInvoiceListbody").html(result);
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error: " + status + error);
        }
    });
}

function GetBetweenDateInvoiceList() {

    selectedstartDate = $('#startDate').val();
    selectedendDate = $('#endDate').val();
    selectedfilterType = "dateRange";

    if (!selectedstartDate && !selectedendDate) {
        toastr.warning("Select dates");
        return;
    } else if (!selectedstartDate) {
        toastr.warning("Select Start date");
        return;
    } else if (!selectedendDate) {
        toastr.warning("Select End date");
        return;
    }
    else {
        var objData = {
            SiteId: selectedSiteId,
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            startDate: selectedstartDate,
            endDate: selectedendDate
        };
        $.ajax({
            type: "post",
            url: '/InvoiceMaster/GetSupplierInvoiceDetailsReport',
            data: objData,
            datatype: 'json',
            success: function (result) {
                $("#reportInvoiceListbody").html(result);
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error: " + status + " - " + error);
            }
        });
    }
}