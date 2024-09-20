var TotalPending = '';
var TotalCreadit = '';
var TotalOutstanding = '';
var TotalPurchase = '';


GetAllCompanyList();
GetGroupList();
GetAllSupplierList();
function GetAllCompanyList() {
    $.ajax({
        url: '/Company/GetCompanyNameList',
        method: 'GET',
        success: function (result) {
            var $dropdown = $("#textPayoutReportCompanyName");
            $dropdown.empty();
            $dropdown.append('<option value="">Select Company</option>');
            result.forEach(function (data) {
                $dropdown.append('<option value="' + data.companyId + '" data-payoutcompany-name="' + data.companyName + '">' + data.companyName + '</option>');
            });

        },
        error: function (err) {
            console.error("Failed to fetch company list: ", err);
        }
    });
}

function GetGroupList() {

    $.ajax({
        url: '/SiteMaster/GetGroupNameListBySiteId',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#textPayoutReportGroupList').append('<Option value=' + data.groupName + '>' + data.groupName + '</Option>')
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

            $("#textPayoutReportSupplierName").autocomplete({
                source: supplierDetails,
                minLength: 0,
                select: function (event, ui) {
                    event.preventDefault();
                    $("#textPayoutReportSupplierName").val(ui.item.label);
                    $("#textPayoutReportSupplierNameHidden").val(ui.item.value);
                    $("#textPayoutReportSupplierNameHidden").trigger('change');
                },
                focus: function () {
                    return false;
                }
            }).focus(function () {
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
var selectedendDate = null;
var selectedfilterType = null;
var selectedGroupName = null;
var selectedYears = null;
var selectedSortOrder = "DescendingDate";
var parsedSiteId = null;
var selectedCompanyName = null;
var selectedSupplierName = null;

let currentReportSortOrder = 'AscendingDate';
function sortReportTable(field) {
    if (currentReportSortOrder === 'Ascending' + field) {
        currentReportSortOrder = 'Descending' + field;
    } else {
        currentReportSortOrder = 'Ascending' + field;
    }

    selectedSortOrder = currentReportSortOrder;

    if (selectedGroupName) {
        var objData = {
            GroupName: selectedGroupName,
            sortBy: selectedSortOrder,
        };
    }
    else {
        var objData = {
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            endDate: selectedendDate,
            GroupName: selectedGroupName,
            SelectedYear: selectedYears,
            sortBy: selectedSortOrder,
        };
    }

    siteloadershow();

    $.ajax({
        type: "post",
        url: '/Report/GetSupplierInvoiceDetailsReport',
        data: objData,
        datatype: 'json',
        success: function (result) {
            siteloaderhide();
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


$(document).ready(function () {

    function clearDates() {

        $('#PayoutendDate').val('');
    }

    // Function to set today's date in the Payout start and end date fields
    function setTodaysPayoutDate() {
        var today = new Date();
        var formattedDate = today.toISOString().substr(0, 10); // Format date as yyyy-mm-dd
        $('#PayoutendDate').val(formattedDate);
    }

    $('#timePeriodPayoutDropdown').change(function () {
        var selectedValue = $(this).val();

        if (selectedValue === 'This Month' || selectedValue === 'This Year') {
            $('#PayoutendDate, #PayoutyearDropdown').hide();
        } else if (selectedValue === 'Between Date') {
            $('#PayoutendDate, #searchPayoutReportButton').show();
            $('#PayoutyearDropdown').hide();
            clearDates(); // Clear previous values
            setTodaysPayoutDate(); // Set today's date
        } else if (selectedValue === 'Between Year') {
            $('#PayoutyearDropdown, #searchPayoutReportButton').show();
            $('#PayoutendDate').hide();
            populateYearDropdown(); // Assuming you have a function to populate the year dropdown
        }
    });
});


function populateYearDropdown() {
    var currentYear = new Date().getFullYear();
    var startYear = 2023;
    var yearDropdown = $('#PayoutyearDropdown');

    yearDropdown.empty().append('<option value="">Select Year</option>');

    for (var year = startYear; year <= currentYear; year++) {
        var nextYear = (year + 1).toString().slice(-2);
        var yearRange = year + '-' + nextYear;
        yearDropdown.append('<option value="' + yearRange + '">' + yearRange + '</option>');
    }
}
function fn_ResetAllPayoutDropdown() {
    window.location = '/InvoiceMaster/PayOutInvoice';
}

$(document).ready(function () {
    $("#textPayoutReportCompanyName").on('change', function () {
        var selectedOption = $(this).find('option:selected');
        selectedCompanyName = selectedOption.data('payoutcompany-name');
    });
});

function ExportNetReportToPDF() {
    siteloadershow();
    var selectedValue = $('#timePeriodPayoutDropdown').val();
    var selectedSupplierId = $('#textPayoutReportSupplierNameHidden').val();
    var selectedSupplierName = $('#textPayoutReportSupplierName').val();
    var selectedCompanyId = $('#textPayoutReportCompanyName').val();
    var selectedGroupName = $('#textPayoutReportGroupList').val();
    var selectedReportSiteName = $('#txtSiteId').val();
    var selectedSortOrder = "AscendingDate";
    var selectedendDate, selectedYears;

    var PayOutReport = {
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

    switch (selectedValue) {
        case 'This Month':
            PayOutReport.filterType = "currentMonth";
            break;
        case 'This Year':
            PayOutReport.filterType = "currentYear";
            break;
        case 'Between Date':
            selectedendDate = $('#PayoutendDate').val();
            if (!selectedendDate) {
                toastr.warning("Select dates");
                return;
            }
            PayOutReport.filterType = "dateRange";
            PayOutReport.endDate = selectedendDate;
            break;
        case 'Between Year':
            selectedYears = $('#PayoutyearDropdown').val();
            if (!selectedYears) {
                alert('Please select a year.');
                return;
            }
            PayOutReport.filterType = "betweenYear";
            PayOutReport.SelectedYear = selectedYears;
            break;
        default:
            selectedValue = null;
            break;
    }
    $.ajax({
        url: '/Report/ExportNetReportToPDF',
        type: 'POST',
        data: PayOutReport,
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

function ExportNetReportToExcel() {
    siteloadershow();
    var selectedValue = $('#timePeriodPayoutDropdown').val();
    var selectedSupplierId = $('#textPayoutReportSupplierNameHidden').val();
    var selectedSupplierName = $('#textPayoutReportSupplierName').val();
    var selectedCompanyId = $('#textPayoutReportCompanyName').val();
    var selectedGroupName = $('#textPayoutReportGroupList').val();
    var selectedReportSiteName = $('#txtSiteId').val();
    var selectedSortOrder = "AscendingDate";
    var selectedendDate, selectedYears;

    var PayOutReport = {
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

    switch (selectedValue) {
        case 'This Month':
            PayOutReport.filterType = "currentMonth";
            break;
        case 'This Year':
            PayOutReport.filterType = "currentYear";
            break;
        case 'Between Date':

            selectedendDate = $('#PayoutendDate').val();
            if (!selectedendDate) {
                toastr.warning("Select dates");
                return;
            }
            PayOutReport.filterType = "dateRange";
            PayOutReport.endDate = selectedendDate;
            break;
        case 'Between Year':
            selectedYears = $('#PayoutyearDropdown').val();
            if (!selectedYears) {
                alert('Please select a year.');
                return;
            }
            PayOutReport.filterType = "betweenYear";
            PayOutReport.SelectedYear = selectedYears;
            break;
        default:
            selectedValue = null;
            break;
    }
    $.ajax({
        url: '/Report/ExportNetReportToExcel',
        type: 'GET',
        data: PayOutReport,
        datatype: 'json',
        success: function (data, status, xhr) {
            debugger
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

var dtcoulms = [
    {
        "data": "supplierName",
        "name": "SupplierName",
        "orderable": true,
        "render": function (data, type, row) {
            return row.supplierName;
        }
    },
    {
        "data": "nonPayOutTotalAmount",
        "name": "NonPayOutTotalAmount",
        "orderable": true,
        "render": function (data, type, row) {
            return '<span style="color:green">' + '₹' + row.nonPayOutTotalAmount + '</span>';
        }
    },
    {
        "data": "payOutTotalAmount",
        "name": "PayOutTotalAmount",
        "orderable": true,
        "render": function (data, type, row) {
            return '<span style="color:red">' + '₹' + row.payOutTotalAmount + '</span>';
        }
    },
    {
        "data": "netAmount",
        "name": "NetAmount",
        "orderable": true,
        "render": function (data, type, row) {
            var netAmount = row.nonPayOutTotalAmount - row.payOutTotalAmount;
            return '<span>' + '₹' + netAmount.toFixed(2) + '</span>';
        }
    }
];
$(document).ready(function () {
    var table;
    $('#searchPayoutReportButton').click(function () {

        if ($.fn.DataTable.isDataTable('#tblPayoutReport')) {
            table.destroy();
        }
        table = $('#tblPayoutReport').DataTable({
            processing: false,
            serverSide: true,
            filter: false,
            paging: true,
            pageLength: 15,
            lengthChange: true,
            lengthMenu: [[15, 25, 50, 100], [15, 25, 50, 100]],
            destroy: true,
            order: [],
            ajax: {
                url: '/InvoiceMaster/GetInvoiceDetails',
                type: 'POST',
                data: function (d) {
                    d.draw = d.draw;
                    d.start = d.start;
                    d.length = d.length;
                    d.order = d.order;
                    d.columns = d.columns;
                    d.SiteId = $('#txtSiteId').val() || null;
                    d.CompanyId = $('#textPayoutReportCompanyName').val() || null;
                    d.SupplierId = $('#textPayoutReportSupplierNameHidden').val() || null;
                    d.GroupName = $('#textPayoutReportGroupList').val() || null;

                    var selectedValue = $('#timePeriodPayoutDropdown').val();
                    switch (selectedValue) {
                        case 'This Month':
                            d.filterType = "currentMonth";
                            break;
                        case 'This Year':
                            d.filterType = "currentYear";
                            break;
                        case 'Between Date':
                            d.filterType = "dateRange";

                            d.endDate = $('#PayoutendDate').val();
                            break;
                        case 'Between Year':
                            d.filterType = "betweenYear";
                            d.SelectedYear = $('#PayoutyearDropdown').val();
                            break;
                        default:
                            d.filterType = null;
                            break;
                    }
                }
            },
            columns: dtcoulms,
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

                $(api.table().footer()).find('#totalCredit').html('<span>' + '₹' + totalCredit.toFixed(2) + '</span>');
                $(api.table().footer()).find('#totalDebit').html('<span>' + '₹' + totalDebit.toFixed(2) + '</span>');
                $(api.table().footer()).find('#NetAmount').html('<span>' + '₹' + NetAmount.toFixed(2) + '</span>');

                // Replace classes for pagination buttons
                $(this.api().table().container()).find('.current paginate button').removeClass('paginate_button').addClass('btn btn-outline-primary');
                $(this.api().table().container()).find('.paginate_button current').removeClass('btn-outline-primary').addClass('btn btn-primary');
            },
            columnDefs: [{
                defaultContent: "",
                targets: "_all",
                width: 'auto'
            }]
        });
    });
});

//let rowCounter = 0;

//function AddNewRowforPayOutInvoicebtn() {
//    var siteId = $("#txtSiteId").val();
//    var supplierName = $("#txtSuppliername").val();
//    var companyName = $("#txtcompanyname").val();
//    if (siteId != "" && supplierName != null && companyName != null) {
//        $.ajax({
//            url: '/InvoiceMaster/DisplayPayOutInvoicePayOutInvoice',
//            type: 'Post',
//            datatype: 'json',
//            processData: false,
//            contentType: false,
//            complete: function (Result) {
//                if (Result.statusText === "success" || Result.statusText === "OK") {
//                    rowCounter++;
//                    AddNewRow(Result.responseText, rowCounter);
//                } else {
//                    toastr.error("Error in display product");
//                }
//            }
//        });
//    } else {
//        toastr.warning("select site, company and supplier");
//    }
//}

//function AddNewRow(resultHtml, rowNumber) {

//    const rowHtml = resultHtml
//        .replace(/ROWID/g, rowNumber)
//        .replace(/ROWNUMBER/g, rowNumber);

//    $('#payoutpartialView').append(rowHtml);
//    $('#payoutsubmitbutton').show();
//}

//function removePayout(buttonElement) {
//    $(buttonElement).closest('tr').remove();
//    updatePayoutRowNumbers();

//    if ($('.payoutinvoicerow').length >= 1) {
//        $('#payoutsubmitbutton').show();
//    }
//    else {
//        $('#payoutsubmitbutton').hide();
//    }
//}

//function updatePayoutRowNumbers() {
//    $('#payoutpartialView .payoutinvoicerow').each(function (index) {
//        $(this).find('.row-number').text(index + 1 + '.');
//    });
//}
