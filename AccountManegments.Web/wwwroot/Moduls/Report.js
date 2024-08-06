GetAllSiteList();
GetAllCompanyList();
GetAllSupplierList();
loadReportData();
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

    $('.nav-radio').click(function () {
        var targetTab = $(this).attr('href');
        if (targetTab === '#GetCurrentYearInvoicelist') {
            GetCurrentYearInvoiceList();
        }
    });

    $('.nav-btn').click(function () {
        var targetTab = $(this).attr('href');
        if (targetTab === '#GetBetweenDatesList') {
            GetBetweenDateInvoiceList();
        }
    });
});

function loadReportData(objData) {
    $.ajax({
        type: "post",
        url: '/Report/GetSupplierInvoiceDetailsReport',
        data: objData,
        datatype: 'json',
        success: function (result) {
            $("#reportInvoiceListbody").html(result);

            if ($("#reportInvoiceListbody").find(".text-center:contains('No data found for the selected criteria.')").length > 0) {
                $("#downloadreportfile").hide();
            } else {
                $("#downloadreportfile").show();
            }
        },
        error: function (xhr, status, error) {
            console.error('AJAX Error: ', status, error);
        }
    });
}

function GetInvoiceReportData() {
    debugger
    if (selectedCompanyId || selectedSupplierId) {
        var objData = {
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            startDate: selectedstartDate,
            endDate: selectedendDate
        };
        loadReportData(objData);
    }
}

function GetCurrentMonthInvoiceList() {
    debugger
    selectedfilterType = "currentMonth";
    var objData = {
        CompanyId: selectedCompanyId,
        SupplierId: selectedSupplierId,
        filterType: selectedfilterType
    };
    loadReportData(objData);
}

function GetCurrentYearInvoiceList() {
    debugger
    selectedfilterType = "currentYear";
    var objData = {
        CompanyId: selectedCompanyId,
        SupplierId: selectedSupplierId,
        filterType: selectedfilterType
    };
    loadReportData(objData);
}

function GetBetweenDateInvoiceList() {
    selectedstartDate = $('#startDate').val();
    selectedendDate = $('#endDate').val();
    selectedfilterType = "dateRange";

    if (!selectedstartDate || !selectedendDate) {
        toastr.warning("Select dates");
    } else {
        var objData = {
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            startDate: selectedstartDate,
            endDate: selectedendDate
        };
        loadReportData(objData);
    }
}


function ExportToPDF() {

    siteloadershow();
    var objData = {
        SiteId: selectedSiteId,
        CompanyId: selectedCompanyId,
        SupplierId: selectedSupplierId,
        filterType: selectedfilterType,
        startDate: selectedstartDate,
        endDate: selectedendDate
    };
    $.ajax({
        url: '/Report/ExportToPdf',
        type: 'POST',
        data: objData,
        datatype: 'json',
        success: function (data, status, xhr) {
            siteloaderhide();
            var filename = "";
            var disposition = xhr.getResponseHeader('Content-Disposition');
            if (disposition && disposition.indexOf('attachment') !== -1) {
                var matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(disposition);
                if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
            }

            var type = xhr.getResponseHeader('Content-Type');
            var blob = new Blob([data], { type: type });

            if (typeof window.navigator.msSaveBlob !== 'undefined') {
                window.navigator.msSaveBlob(blob, filename);
            } else {
                var URL = window.URL || window.webkitURL;
                var downloadUrl = URL.createObjectURL(blob);

                if (filename) {
                    var a = document.createElement("a");
                    if (typeof a.download === 'undefined') {
                        window.location = downloadUrl;
                    } else {
                        a.href = downloadUrl;
                        a.download = filename;
                        document.body.appendChild(a);
                        a.click();
                    }
                } else {
                    window.location = downloadUrl;
                }

                setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100);
            }
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.warning("No data found for the selected criteria.");
        },
        xhrFields: {
            responseType: 'blob'
        }
    });
}

function ExportToExcel() {
    siteloadershow();
    var objData = {
        SiteId: selectedSiteId,
        CompanyId: selectedCompanyId,
        SupplierId: selectedSupplierId,
        filterType: selectedfilterType,
        startDate: selectedstartDate,
        endDate: selectedendDate
    };
    $.ajax({
        url: '/Report/ExportToExcel',
        type: 'GET',
        data: objData,
        datatype: 'json',
        success: function (data, status, xhr) {
            siteloaderhide();
            var filename = "";
            var disposition = xhr.getResponseHeader('Content-Disposition');
            if (disposition && disposition.indexOf('attachment') !== -1) {
                var matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(disposition);
                if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
            }

            var type = xhr.getResponseHeader('Content-Type');
            var blob = new Blob([data], { type: type });

            if (typeof window.navigator.msSaveBlob !== 'undefined') {
                window.navigator.msSaveBlob(blob, filename);
            } else {
                var URL = window.URL || window.webkitURL;
                var downloadUrl = URL.createObjectURL(blob);

                if (filename) {
                    var a = document.createElement("a");
                    if (typeof a.download === 'undefined') {
                        window.location = downloadUrl;
                    } else {
                        a.href = downloadUrl;
                        a.download = filename;
                        document.body.appendChild(a);
                        a.click();
                    }
                } else {
                    window.location = downloadUrl;
                }

                setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100);
            }
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.warning("No data found for the selected criteria.");
        },
        xhrFields: {
            responseType: 'blob'
        }
    });
}