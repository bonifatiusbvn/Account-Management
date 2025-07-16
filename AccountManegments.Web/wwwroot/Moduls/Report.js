
var _userCompanyCount = window._userCompanyCount || 0;


GetAllSiteList();

GetAllSupplierList();
GetGroupList();


if (_userCompanyCount === 0) {

    $(document).ready(function () {
        GetAllCompanyList();
    });
}
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
            if (result.length > 0) {
                var $dropdown = $('#textReportCompanyName');
                $dropdown.empty();

                $dropdown.append('<option selected value="" disabled>Select Company</option>');

                $.each(result, function (i, data) {
                    $dropdown.append(
                        '<option value="' + data.companyId + '" data-company-name="' + data.companyName + '">' + data.companyName + '</option>'
                    );
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching company details:', error);
        }
    });
}
function GetGroupList() {

    $.ajax({
        url: '/SiteMaster/GetGroupNameListBySiteId',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#textReportGroupList').append('<Option value=' + data.groupName + '>' + data.groupName + '</Option>')
            });
        }
    });
}
function GetAllSupplierList() {
    $.ajax({
        url: '/Supplier/GetSupplierNameList',
        method: 'GET',
        success: function (result) {
            var supplierDetails = result.map(function (data) {
                return {
                    label: data.supplierName,
                    value: data.supplierId
                };
            });
           
            $("#textReportSupplierName").autocomplete({
                source: supplierDetails,
                minLength: 0,
                select: function (event, ui) {
                    event.preventDefault();
                    $("#textReportSupplierName").val(ui.item.label);
                    $("#textReportSupplierNameHidden").val(ui.item.value);
                    selectedSupplierName = ui.item.label;
                    $("#textReportSupplierNameHidden").trigger('change');
                },
                focus: function () {
                    return false;
                }
            }).focus(function () {
                if ($("#textReportSupplierName").val() === "") {
                    $("#textReportSupplierNameHidden").val("");
                }
                $(this).autocomplete("search", "");
            });
        },
        error: function (err) {
            console.error("Failed to fetch supplier list: ", err);
        }
    });
}

var selectedSiteId = null;
var selectedCompanyId = null;
var selectedSupplierId = null;
var selectedstartDate = null;
var selectedendDate = null;
var selectedfilterType = null;
var selectedGroupName = null;
var selectedYears = null;
var selectedCompanyName = null;
var selectedSupplierName = null;
var selectedSortOrder = "DescendingDate";
var parsedSiteId = null;
var selectedInvoiceTypeName = null;

function populateYearDropdown() {
    var currentYear = new Date().getFullYear();
    var startYear = 2023;
    var yearDropdown = $('#yearDropdown');

    yearDropdown.empty().append('<option value="">Select Year</option>');

    for (var year = startYear; year <= currentYear; year++) {
        var nextYear = (year + 1).toString().slice(-2);
        var yearRange = year + '-' + nextYear;
        yearDropdown.append('<option value="' + yearRange + '">' + yearRange + '</option>');
    }
}

$(document).ready(function () {

    // Clear date fields
    function clearDates() {
        $('#startDate').val('');
        $('#endDate').val('');
    }


    function setTodaysDate() {
        var today = new Date();
        var formattedDate = today.toISOString().substr(0, 10);
        $('#startDate').val(formattedDate);
        $('#endDate').val(formattedDate);
    }

    $('#timePeriodDropdown').change(function () {
        var selectedValue = $(this).val();

        if (selectedValue === 'This Month' || selectedValue === 'This Year') {
            $('#startDate, #endDate, #yearDropdown').addClass('d-none');
        } else if (selectedValue === 'Between Date') {
            $('#startDate, #endDate, #searchReportButton').removeClass('d-none');
            $('#yearDropdown').addClass('d-none');
            setTodaysDate(); 
        } else if (selectedValue === 'Between Year') {
            $('#yearDropdown, #searchReportButton').removeClass('d-none');
            $('#startDate, #endDate').addClass('d-none');
            populateYearDropdown(); 
        }
    });
});



$("#textReportCompanyName").on('change', function () {
    var selectedOption = $(this).find('option:selected');
    selectedCompanyName = selectedOption.data('company-name');
});

$("#textReportTypeList").on('change', function () {
    selectedInvoiceTypeName = $('#textReportTypeList').val();
});
function ExportToPDF() {

    siteloadershow();
    var selectedValue = $('#timePeriodDropdown').val();
    var selectedSupplierId = $('#textReportSupplierNameHidden').val();
    var selectedSupplierName = $('#textReportSupplierName').val();
    var selectedCompanyId = $('#textReportCompanyName').val();
    var selectedGroupName = $('#textReportGroupList').val();
    var selectedReportSiteName = $('#textReportSiteName').val();
    var selectedSortOrder = "AscendingDate";

    var objData = {
        SiteId: selectedReportSiteName || null,
        CompanyId: selectedCompanyId || null,
        SupplierId: selectedSupplierId || null,
        GroupName: selectedGroupName || null,
        sortBy: selectedSortOrder,
        CompanyName: selectedCompanyName || null,
        SupplierName: selectedSupplierName || null,
        filterType: null,
        startDate: $('#startDate').val(),
        endDate: $('#endDate').val(),
        SelectedYear: null,
    };
    var selectedValue = $('#timePeriodDropdown').val();
    switch (selectedValue) {
        case 'This Month':
            objData.filterType = "currentMonth";
            break;
        case 'This Year':
            objData.filterType = "currentYear";
            break;
        case 'Between Date':
            objData.filterType = "dateRange";
            objData.filterType = $('#startDate').val();
            objData.filterType = $('#endDate').val();
            break;
        case 'Between Year':
            objData.filterType = "betweenYear";
            objData.filterType = $('#yearDropdown').val();
            break;
        default:
            objData.filterType = null;
            break;
    }
    var ajaxUrl = selectedInvoiceTypeName === 'Sales'
        ? '/Sales/ExportSalesPaymentInvoiceToPdf'
        : '/Report/ExportToPdf';
    $.ajax({
        url: ajaxUrl,
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
    var selectedValue = $('#timePeriodDropdown').val();
    var selectedSupplierId = $('#textReportSupplierNameHidden').val();
    var selectedSupplierName = $('#textReportSupplierName').val();
    var selectedCompanyId = $('#textReportCompanyName').val();
    var selectedGroupName = $('#textReportGroupList').val();
    var selectedReportSiteName = $('#textReportSiteName').val();
    var selectedSortOrder = "AscendingDate";
    var objData = {
        SiteId: selectedReportSiteName || null,
        CompanyId: selectedCompanyId || null,
        SupplierId: selectedSupplierId || null,
        GroupName: selectedGroupName || null,
        sortBy: selectedSortOrder,
        CompanyName: selectedCompanyName || null,
        SupplierName: selectedSupplierName || null,
        filterType: null,
        startDate: $('#startDate').val(),
        endDate: $('#endDate').val(),
        SelectedYear: null,
    };
    var selectedValue = $('#timePeriodDropdown').val();
    switch (selectedValue) {
        case 'This Month':
            objData.filterType = "currentMonth";
            break;
        case 'This Year':
            objData.filterType = "currentYear";
            break;
        case 'Between Date':
            objData.filterType = "betweenDate";
            break;
        case 'Between Year':
            objData.filterType = "betweenYear";
            objData.filterType = $('#yearDropdown').val();
            break;
        default:
            objData.filterType = null;
            break;
    }
    var ajaxUrl = selectedInvoiceTypeName === 'Sales'
        ? '/Sales/ExportSalesPaymentInvoiceToExcel'
        : '/Report/ExportToExcel';
    $.ajax({
        url: ajaxUrl,
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
function GetInvoiceDetailsBySupplierExcelReport() {
    siteloadershow();
    var selectedValue = $('#timePeriodDropdown').val();
    var selectedSupplierId = $('#textReportSupplierNameHidden').val();
    var selectedSupplierName = $('#textReportSupplierName').val();
    var selectedCompanyId = $('#textReportCompanyName').val();
    var selectedGroupName = $('#textReportGroupList').val();
    var selectedReportSiteName = $('#textReportSiteName').val();
    var selectedSortOrder = "AscendingDate";
    var objData = {
        SiteId: selectedReportSiteName || null,
        CompanyId: selectedCompanyId || null,
        SupplierId: selectedSupplierId || null,
        GroupName: selectedGroupName || null,
        sortBy: selectedSortOrder,
        CompanyName: selectedCompanyName || null,
        SupplierName: selectedSupplierName || null,
        filterType: null,
        startDate: null,
        endDate: null,
        SelectedYear: null,
    };
    var selectedValue = $('#timePeriodDropdown').val();
    switch (selectedValue) {
        case 'This Month':
            objData.filterType = "currentMonth";
            break;
        case 'This Year':
            objData.filterType = "currentYear";
            break;
        case 'Between Date':
            objData.filterType = "dateRange";
            objData.filterType = $('#startDate').val();
            objData.filterType = $('#endDate').val();
            break;
        case 'Between Year':
            objData.filterType = "betweenYear";
            objData.filterType = $('#yearDropdown').val();
            break;
        default:
            objData.filterType = null;
            break;
    }
    var ajaxUrl ='/Report/GetInvoiceDetailsBySupplierExcelReport';
    $.ajax({
        url: ajaxUrl,
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
var datas = userPermissions;

var supplierBalances = {};
var processedInvoices = {};
let dtSupplierCoulms = [];
let dtSalesCoulms = [];

dtSalesCoulms = [
    {
        "data": "salesInvoiceNo",
        "name": "SalesInvoiceNo",
        "render": function (data, type, row) {
            if (row.salesInvoiceNo === 'PayIn') {
                var description = row.description ? ' (' + row.description + ')' : '';
                return row.salesInvoiceNo + description;
            } else {
                var invoiceType = row.invoiceType ? ' (' + row.invoiceType + ')' : '';
                return row.salesInvoiceNo + invoiceType;
            }
        }
    },
    {
        "data": "date",
        "name": "Date",
        "render": function (data, type, row) {
            if (data) {
                var date = new Date(data);
                return date.toLocaleDateString('en-GB');
            }
            return data;
        }
    },
    { "data": "siteName", "name": "SiteName" },
    { "data": "groupName", "name": "GroupName" },
    { "data": "supplierName", "name": "SupplierName" },
    {
        "data": "totalAmount",
        "name": "Credit",
        "render": function (data, type, row) {
            if (row.salesInvoiceNo !== 'PayIn' && row.invoiceType != 'Sales Return' && row.invoiceType != 'Credit Note') {
                return '<span style="color:green">' + '₹' + formatReportNumberWithCommas(data.toFixed(2)) + '</span>';
            } else {
                return '';
            }
        }
    },
    {
        "data": "totalAmount",
        "name": "Debit",
        "render": function (data, type, row) {
            if (row.salesInvoiceNo === 'PayIn' || row.invoiceType === 'Sales Return' || row.invoiceType === 'Credit Note') {
                return '<span style="color:red">' + '₹' + formatReportNumberWithCommas(data.toFixed(2)) + '</span>';
            } else {
                return '';
            }
        }
    },
    {
        "data": "totalAmount",
        "name": "Balance",
        "render": function (data, type, row) {
            var supplierName = row.supplierName;
            var totalAmount = parseFloat(row.totalAmount) || 0;
            var SalesInvoiceNo = row.salesInvoiceNo;
            var invoiceType = row.invoiceType;

            if (!supplierBalances[supplierName]) {
                supplierBalances[supplierName] = 0;
                processedInvoices[supplierName] = new Set();
            }
            var uniqueKey = (SalesInvoiceNo === "PayIn" || invoiceType === 'Sales Return' || invoiceType === 'Credit Note') ? SalesInvoiceNo + "_" + totalAmount : SalesInvoiceNo;
            
            if (!processedInvoices[supplierName].has(uniqueKey)) {

                if (SalesInvoiceNo === "PayIn" || invoiceType === 'Sales Return' || invoiceType === 'Credit Note') {
                    supplierBalances[supplierName] -= totalAmount;
                }
                else {
                    supplierBalances[supplierName] += totalAmount;
                }
                
                processedInvoices[supplierName].add(uniqueKey);
            }

            return '<span style="color:blue">' + '₹' + formatReportNumberWithCommas(supplierBalances[supplierName].toFixed(2)) + '</span>';
        }
    }
];

dtSupplierCoulms = [
    {
        "data": "supplierInvoiceNo",
        "name": "supplierInvoiceNo",
        "render": function (data, type, row) {
            if (row.invoiceNo === 'Opening Balance' || row.invoiceNo === 'PayOut') {
                var description = row.description ? ' (' + row.description + ')' : '';
                if (row.supplierInvoiceNo != null) {
                    return row.supplierInvoiceNo + description;
                } else {
                    return row.invoiceNo + description;
                }
            } else {
                var invoiceType = row.invoiceType ? ' (' + row.invoiceType + ')' : '';
                if (row.supplierInvoiceNo != null) {
                    return row.supplierInvoiceNo + invoiceType;
                } else {
                    return row.invoiceNo + invoiceType;
                }
            }
            return data;
        }
    },
    {
        "data": "date",
        "name": "Date",
        "render": function (data, type, row) {
            if (data) {
                var date = new Date(data);
                return date.toLocaleDateString('en-GB');
            }
            return data;
        }
    },
    { "data": "siteName", "name": "SiteName" },
    { "data": "groupName", "name": "GroupName" },
    { "data": "supplierName", "name": "SupplierName" },
    {
        "data": "totalAmount",
        "name": "Credit",
        "render": function (data, type, row) {
            if (row.invoiceNo !== 'PayOut' && row.invoiceType != 'Purchase Return' && row.invoiceType != 'Credit Note') {
                return '<span style="color:green">' + '₹' + formatReportNumberWithCommas(data.toFixed(2)) + '</span>';
            } else {
                return '';
            }
        }
    },
    {
        "data": "totalAmount",
        "name": "Debit",
        "render": function (data, type, row) {
            if (row.invoiceNo === 'PayOut' || row.invoiceType === 'Purchase Return' || row.invoiceType === 'Credit Note') {
                return '<span style="color:red">' + '₹' + formatReportNumberWithCommas(data.toFixed(2)) + '</span>';
            } else {
                return '';
            }
        }
    },
    {
        "data": "totalAmount",
        "name": "Balance",
        "render": function (data, type, row) {
            var supplierName = row.supplierName;
            var totalAmount = parseFloat(row.totalAmount) || 0;
            var invoiceNo = row.invoiceNo;
            var invoiceType = row.invoiceType;

            if (!supplierBalances[supplierName]) {
                supplierBalances[supplierName] = 0;
                processedInvoices[supplierName] = new Set();
            }
            var uniqueKey = (invoiceNo === "PayOut" || invoiceType === 'Purchase Return' || invoiceType === 'Credit Note') ? invoiceNo + "_" + totalAmount : invoiceNo;

            if (!processedInvoices[supplierName].has(uniqueKey)) {

                if (invoiceNo === "PayOut" || invoiceType === 'Purchase Return' || invoiceType === 'Credit Note') {
                    supplierBalances[supplierName] -= totalAmount;
                }
                else {
                    supplierBalances[supplierName] += totalAmount;
                }

                processedInvoices[supplierName].add(uniqueKey);
            }

            return '<span style="color:blue">' + '₹' + formatReportNumberWithCommas(supplierBalances[supplierName].toFixed(2)) + '</span>';
        }
    }
];




$(document).ready(function () {
    var table;
    var userPermissionArray = JSON.parse(datas);
    var canEdit = false;
    var canDelete = false;

    for (var i = 0; i < userPermissionArray.length; i++) {
        var permission = userPermissionArray[i];
        if (permission.formName == "Reports & Payments") {
            canEdit = permission.edit;
            canDelete = permission.delete;
            break;
        }
    } 
    if (canEdit || canDelete) {
        dtSalesCoulms.push({
            "data": null,
            "name": "Action",
            "render": function (data, type, row) {
                if (row.salesInvoiceNo === 'PayIn') {
                    var buttons = '';
                    if (canEdit) {
                        buttons +=
                            '<a class="btn text-primary p-0 m-0" style="display: inline-block;" onclick="EditPayInDetails(\'' + row.id + '\')" title="Edit" aria-label="Edit">' +
                            '<i class="fadeIn animated bx bx-edit"></i>' +
                            '</a>';
                    }

                    if (canDelete) {
                        buttons += '<a href="javascript:;" class="btn text-primary p-0 m-0" style="display: inline-block;" onclick="deletePayInDetails(\'' + row.id + '\')" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Delete" aria-label="Delete">' +
                            '<i class="lni lni-trash"></i>' +
                            '</a>';
                    }
                    return buttons;
                }
            },
        });

        dtSupplierCoulms.push({
            "data": null,
            "name": "Action",
            "render": function (data, type, row) {

                if (row.invoiceNo === 'PayOut' || row.invoiceNo === 'Opening Balance') {
                    var buttons = '';
                    if (canEdit) {
                        buttons +=
                            '<a class="btn text-primary p-0 m-0" style="display: inline-block;" onclick="EditPayoutDetails(\'' + row.id + '\')" title="Edit" aria-label="Edit">' +
                            '<i class="fadeIn animated bx bx-edit"></i>' +
                            '</a>';
                    }

                    if (canDelete) {
                        buttons += '<a href="javascript:;" class="btn text-primary p-0 m-0" style="display: inline-block;" onclick="deletePayoutDetails(\'' + row.id + '\')" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Delete" aria-label="Delete">' +
                            '<i class="lni lni-trash"></i>' +
                            '</a>';
                    }
                    return buttons;
                }
            },
        });
    }

    $('#searchReportButton').click(function () {

        if ($.fn.DataTable.isDataTable('#tblinvoice')) {
            table.destroy();
        }

        var ajaxUrl = selectedInvoiceTypeName === 'Sales'
            ? '/Sales/SalesInvoicePaymentReport'
            : '/Report/GetSupplierInvoiceDetailsReport';

        var dtColumns = selectedInvoiceTypeName === 'Sales'
            ? dtSalesCoulms : dtSupplierCoulms;

        table = $('#tblinvoice').DataTable({
            processing: false,
            serverSide: true,
            filter: false,
            paging: false,
            pageLength: 15,
            lengthChange: true,
            lengthMenu: [[15, 25, 50, 100], [15, 25, 50, 100]],
            destroy: true,
            order: [],
            ajax: {
                url: ajaxUrl,
                type: 'POST',
                data: function (d) {
                    d.draw = d.draw;
                    d.start = d.start;
                    d.length = d.length;
                    d.order = d.order;
                    d.columns = d.columns;
                    d.SiteId = $('#textReportSiteName').val() || null;
                    d.CompanyId = $('#textReportCompanyName').val() || null;
                    d.SupplierId = $('#textReportSupplierNameHidden').val() || null;
                    d.GroupName = $('#textReportGroupList').val() || null;

                    var selectedValue = $('#timePeriodDropdown').val();
                    switch (selectedValue) {
                        case 'This Month':
                            d.filterType = "currentMonth";
                            break;
                        case 'This Year':
                            d.filterType = "currentYear";
                            break;
                        case 'Between Date':
                            d.filterType = "dateRange";
                            d.startDate = $('#startDate').val();
                            d.endDate = $('#endDate').val();
                            break;
                        case 'Between Year':
                            d.filterType = "betweenYear";
                            d.SelectedYear = $('#yearDropdown').val();
                            break;
                        default:
                            d.filterType = null;
                            break;
                    }
                }
            },
            columns: dtColumns,
            scrollX: true,
            scrollY: '350px',
            scrollCollapse: true,
            fixedHeader: {
                header: true,
                footer: false
            },
            autoWidth: false,
            drawCallback: function (settings) {
                var api = this.api();

                var totalCredit = settings.json.totalCredit || 0;
                var totalDebit = settings.json.totalDebit || 0;
                var NetAmount = totalCredit - totalDebit;

                var formattedTotalCredit = formatReportNumberWithCommas(totalCredit.toFixed(2));
                var formattedTotalDebit = formatReportNumberWithCommas(totalDebit.toFixed(2));
                var formattedNetAmount = formatReportNumberWithCommas(NetAmount.toFixed(2));

                $(api.table().footer()).find('#totalCredit').html('<span>' + '₹' + formattedTotalCredit + '</span>');
                $(api.table().footer()).find('#totalDebit').html('<span>' + '₹' + formattedTotalDebit + '</span>');
                $(api.table().footer()).find('#NetAmount').html('<span>' + '₹' + formattedNetAmount + '</span>');

                $(this.api().table().container()).find('.current paginate button').removeClass('paginate_button').addClass('btn btn-outline-primary');
                $(this.api().table().container()).find('.paginate_button current').removeClass('btn-outline-primary').addClass('btn btn-primary');

                var scrollContainer = $('.dataTables_scrollBody');
                var lastRow = $(this.api().table().body()).find('tr:last');

                if (lastRow.length && scrollContainer.length) {
                    var offsetTop = lastRow.offset().top - scrollContainer.offset().top + scrollContainer.scrollTop();
                    scrollContainer.animate({ scrollTop: offsetTop }, 10);
                }
            },
            columnDefs: [{
                defaultContent: "",
                targets: "_all",
                width: 'auto'
            }]
        });
    });
});

function formatReportNumberWithCommas(x) {
    var parts = x.toString().split(".");
    var integerPart = parts[0];
    var decimalPart = parts.length > 1 ? "." + parts[1] : "";

    var lastThree = integerPart.substring(integerPart.length - 3);
    var otherNumbers = integerPart.substring(0, integerPart.length - 3);

    if (otherNumbers != '')
        lastThree = ',' + lastThree;

    return otherNumbers.replace(/\B(?=(\d{2})+(?!\d))/g, ",") + lastThree + decimalPart;
}

function deletePayoutDetails(InvoiceId) {
    Swal.fire({
        title: "Are you sure want to delete this?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        confirmButtonClass: "btn btn-primary w-xs me-2 mt-2",
        cancelButtonClass: "btn btn-danger w-xs mt-2",
        buttonsStyling: false,
        showCloseButton: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Report/DeletePayoutDetails?InvoiceId=' + InvoiceId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {
                    if (Result.code == 200) {
                        siteloaderhide();
                        Swal.fire({
                            title: Result.message,
                            icon: 'success',
                            confirmButtonColor: '#3085d6',
                            confirmButtonText: 'OK'
                        }).then(function () {
                            $("#searchReportButton").click();
                        })
                    }
                    else {
                        siteloaderhide();
                        toastr.error(Result.message);
                    }

                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete payout invoice!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'payout invoice have no changes.!!😊',
                'error'
            );
        }
    });
}
function EditPayoutDetails(InvoiceId) {
    ClearPayoutTextBox();
    siteloadershow();
    $.ajax({
        url: '/Report/GetPayoutDetailsbyId?InvoiceId=' + InvoiceId,
        type: "Get",
        contentType: 'application/json;charset=utf-8;',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            $("#updatePayoutModal").modal('show');
            $('#EdittxtpayoutId').val(response.id);
            $('#EdittxtpayoutSupplierNameHidden').val(response.supplierId);
            $('#EdittxtpayoutSiteId').val(response.siteId);
            $('#EdittxtpayoutCompanyNameHidden').val(response.companyId);
            $('#Edittxtpayoutdescription').val(response.description);
            const formattedDate = formatDate(response.date);
            $('#Edittxtpayoutdate').val(formattedDate);
            $('#Edittxtpayoutamount').val(response.totalAmount);
            $('input[name="Editpayoutpaymenttype"][value="' + response.paymentStatus + '"]').prop('checked', true);

        },
        error: function () {
            siteloaderhide();
            toastr.error("Data not found");
        }
    });
}
function formatDate(dateString) {
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');

    const day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
}
function UpdatePayoutInvoice() {
    if ($("#updatePayoutForm").valid()) {
        siteloadershow();
        var objData = {
            Id: $('#EdittxtpayoutId').val(),
            SiteId: $('#EdittxtpayoutSiteId').val(),
            SupplierId: $('#EdittxtpayoutSupplierNameHidden').val(),
            CompanyId: $('#EdittxtpayoutCompanyNameHidden').val(),
            Description: $('#Edittxtpayoutdescription').val(),
            Date: $('#Edittxtpayoutdate').val(),
            TotalAmount: $('#Edittxtpayoutamount').val(),
            PaymentStatus: $('input[name="Editpayoutpaymenttype"]:checked').val(),
            UpdatedBy: $('#EdittxtpayoutUpdatedBy').val(),
        }
        $.ajax({
            url: '/Report/UpdatePayoutDetails',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                siteloaderhide();
                if (Result.code == 200) {
                    $("#updatePayoutModal").modal('hide');
                    toastr.success(Result.message);
                    $("#searchReportButton").click();
                } else {
                    toastr.error(Result.message);
                }
            }
        })
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}

var UpdatePayoutForm;
$(document).ready(function () {

    UpdatePayoutForm = $("#updatePayoutForm").validate({
        rules: {
            Edittxtpayoutdate: "required",
            Edittxtpayoutamount: {
                required: true,
                number: true,
                min: 0
            }
        },
        messages: {
            Edittxtpayoutdate: "Enter Date",
            Edittxtpayoutamount: {
                required: "Enter Total Amount",
                number: "Enter a valid number",
                min: "Total Amount must be greater than zero"
            }
        }
    })
});
function resetPayoutForm() {
    if (UpdatePayoutForm) {
        UpdatePayoutForm.resetForm();
    }
}
function ClearPayoutTextBox() {
    resetPayoutForm();
    $('#EdittxtpayoutSupplierName').val('');
    $('#EdittxtpayoutCompanyName').val('');
    $('#Edittxtpayoutamount').val('');
    $('#Edittxtpayoutdate').val('');
    $('#Edittxtpayoutdescription').val('');
    $("#Editpayoutpaymenttype").prop("checked", false);
}
function updatePayoutRowNumbers() {
    $('#payoutpartialView .payoutinvoicerow').each(function (index) {
        $(this).find('.row-number').text(index + 1 + '.');
    });
}

function EditPayInDetails(SalesId) {
    ClearPayinTextBox();
    siteloadershow();
    $.ajax({
        url: '/Sales/EditSalesPayInInvoice?SalesId=' + SalesId,
        type: "Get",
        contentType: 'application/json;charset=utf-8;',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            $("#updatePayInModal").modal('show');
            $('#EdittxtpayinId').val(response.id);
            $('#Edittxtpayindescription').val(response.description);
            const formattedDate = formatDate(response.date);
            $('#Edittxtpayindate').val(formattedDate);
            $('#Edittxtpayinamount').val(response.totalAmount);
            $('input[name="Editpayinpaymenttype"][value="' + response.paymentStatus + '"]').prop('checked', true);

        },
        error: function () {
            siteloaderhide();
            toastr.error("Data not found");
        }
    });
}
function UpdatePayInInvoice() {
    if ($("#updatePayInForm").valid()) {
        siteloadershow();
        var objData = {
            Id: $('#EdittxtpayinId').val(),
            Description: $('#Edittxtpayindescription').val(),
            Date: $('#Edittxtpayindate').val(),
            TotalAmount: $('#Edittxtpayinamount').val(),
            PaymentStatus: $('input[name="Editpayinpaymenttype"]:checked').val(),
            UpdatedBy: $('#EdittxtpayinUpdatedBy').val(),
        }
        $.ajax({
            url: '/Sales/UpdateSalesPayInInvoice',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                siteloaderhide();
                if (Result.code == 200) {
                    $("#updatePayInModal").modal('hide');
                    toastr.success(Result.message);
                    $("#searchReportButton").click();
                } else {
                    toastr.error(Result.message);
                }
            }
        })
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}

var UpdatePayinForm;
$(document).ready(function () {

    UpdatePayinForm = $("#updatePayInForm").validate({
        rules: {
            Edittxtpayindate: "required",
            Edittxtpayinamount: {
                required: true,
                number: true,
                min: 0
            }
        },
        messages: {
            Edittxtpayindate: "Enter Date",
            Edittxtpayinamount: {
                required: "Enter Total Amount",
                number: "Enter a valid number",
                min: "Total Amount must be greater than zero"
            }
        }
    })
});
function resetPayinForm() {
    if (UpdatePayinForm) {
        UpdatePayinForm.resetForm();
    }
}
function ClearPayinTextBox() {
    resetPayinForm();
    $('#EdittxtpayinSupplierName').val('');
    $('#EdittxtpayinCompanyName').val('');
    $('#Edittxtpayinamount').val('');
    $('#Edittxtpayindate').val('');
    $('#Edittxtpayindescription').val('');
    $("#Editpayinpaymenttype").prop("checked", false);
}
function deletePayInDetails(SalesId) {
    Swal.fire({
        title: "Are you sure want to delete this?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        confirmButtonClass: "btn btn-primary w-xs me-2 mt-2",
        cancelButtonClass: "btn btn-danger w-xs mt-2",
        buttonsStyling: false,
        showCloseButton: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Sales/DeleteSalesPayInInvoice?SalesId=' + SalesId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {
                    if (Result.code == 200) {
                        siteloaderhide();
                        Swal.fire({
                            title: Result.message,
                            icon: 'success',
                            confirmButtonColor: '#3085d6',
                            confirmButtonText: 'OK'
                        }).then(function () {
                            $("#searchReportButton").click();
                        })
                    }
                    else {
                        siteloaderhide();
                        toastr.error(Result.message);
                    }

                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete payin invoice!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'payin invoice have no changes.!!😊',
                'error'
            );
        }
    });
}