var TotalPending = '';
var TotalCreadit = '';
var TotalOutstanding = '';
var TotalPurchase = '';

//GetCompanyDetails();
//GetSupplierDetails();
//function GetCompanyDetails() {
//    $.ajax({
//        url: '/Company/GetCompanyNameList',
//        method: 'GET',
//        success: function (result) {
//            var $dropdown = $("#txtcompanyname");
//            $dropdown.empty();
//            $dropdown.append('<option value="">Select Company</option>');

//            result.forEach(function (data) {
//                $dropdown.append($('<option>', {
//                    value: data.companyId,
//                    text: data.companyName
//                }));
//            });

//            $dropdown.change(function () {
//                var selectedCompany = $(this).val();
//                $("#txtcompanynameHidden").val(selectedCompany);
//            });
//        },
//        error: function (err) {
//            console.error("Failed to fetch company names: ", err);
//        }
//    });
//}


//function GetSupplierDetails() {
//    $.ajax({
//        url: '/Supplier/GetSupplierNameList',
//        method: 'GET',
//        success: function (result) {
//            var supplierDetails = result.map(function (data) {
//                return {
//                    label: data.supplierName,
//                    value: data.supplierId
//                };
//            });

//            $("#txtSuppliername").autocomplete({
//                source: supplierDetails,
//                minLength: 0,
//                select: function (event, ui) {
//                    event.preventDefault();
//                    $("#txtSuppliername").val(ui.item.label);
//                    $("#txtSuppliernameHidden").val(ui.item.value);

//                    $("#txtSuppliernameHidden").trigger('change');
//                },
//                focus: function () {
//                    return false;
//                }
//            }).focus(function () {
//                $(this).autocomplete("search", "");
//            });
//        },
//        error: function (err) {
//            console.error("Failed to fetch unit types: ", err);
//        }
//    });
//}

//$(document).ready(function () {

//    $("#totalAmount").html('₹' + 00);
//    $("#pendingamount").html('₹' + 00);
//    $("#txttotalcreditamount").html('₹' + 00);
//    $("#txttotalpendingamount").html('₹' + 00);
//    $("#txttotalpurchase").html('₹' + 00);

//    $('#txtSuppliernameHidden').change(function () {
//        siteloadershow();
//        var CompanyId = $('#txtcompanynameHidden').val();
//        var SupplierId = $(this).val();
//        $.ajax({
//            url: '/InvoiceMaster/GetInvoiceDetails?CompanyId=' + CompanyId + '&SupplierId=' + SupplierId,
//            type: 'GET',
//            success: function (result) {
//                siteloaderhide();
//                $("#invoicedetails").html(result);
//                $("#txttotalpendingamount").html('₹' + result.totalPending);
//                $("#pendingamount").html('₹' + result.totalPending);
//                $("#txttotalcreditamount").html('₹' + result.totalCreadit);
//                $("#totalAmount").html('₹' + result.totalOutstanding);
//                $("#txttotalpurchase").html('₹' + result.totalPurchase);
//                var totalpendingAmount = result.totalPending;
//                $('#txtpayoutamount').on('input', function () {
//                    var enteredAmount = parseFloat($(this).val());

//                    if (!isNaN(enteredAmount)) {
//                        var pendingAmount = totalpendingAmount - enteredAmount;

//                        if (enteredAmount > totalpendingAmount) {
//                            $('#spnpayout').text('Entered amount cannot exceed pending amount.');
//                        } else {
//                            $('#txtpendingamount').val(pendingAmount.toFixed(2));
//                        }
//                    } else {
//                        $('#spnpayout').text('');
//                        $('#txtpendingamount').val('');
//                    }
//                });
//            },
//        });
//    });


//});
//function InsertPayOutDetails() {
//    siteloadershow();
//    if ($('.payoutinvoicerow').length >= 1) {
//        var PayoutDetails = [];
//        var isValidPayout = true;

//        $(".payoutinvoicerow").each(function () {
//            var orderRow = $(this);

//            var objData = {
//                InvoiceNo: "PayOut",
//                SiteId: $("#txtSiteId").val(),
//                SupplierId: $("#txtSuppliernameHidden").val(),
//                CompanyId: $("#txtcompanynameHidden").val(),
//                PaymentStatus: orderRow.find("input[name^='paymenttype']:checked").val(),
//                Description: orderRow.find("input[id^='txtdescription']").val(),
//                Date: orderRow.find("input[id^='txtdate']").val(),
//                CreatedBy: $("#txtUserId").val(),
//                TotalAmount: orderRow.find("input[id^='txtpayoutamount']").val()
//            };
//            orderRow.find("input[id^='txtdate']").on('input', function () {
//                $(this).css("border", "1px solid #ced4da");
//            });

//            orderRow.find("input[id^='txtpayoutamount']").on('input', function () {
//                $(this).css("border", "1px solid #ced4da");
//            });

//            if (objData.Date === "" || objData.TotalAmount === "") {
//                isValidPayout = false;

//                if (objData.Date === "") {
//                    orderRow.find("input[id^='txtdate']").css("border", "2px solid red");
//                }

//                if (objData.TotalAmount === "") {
//                    orderRow.find("input[id^='txtpayoutamount']").css("border", "2px solid red");
//                }
//            } else {
//                PayoutDetails.push(objData);
//            }
//        });

//        if (isValidPayout) {
//            var form_data = new FormData();
//            form_data.append("PAYOUTDETAILS", JSON.stringify(PayoutDetails));

//            $.ajax({
//                url: '/InvoiceMaster/InsertPayOutDetails',
//                type: 'POST',
//                data: form_data,
//                dataType: 'json',
//                contentType: false,
//                processData: false,
//                success: function (result) {
//                    siteloaderhide();
//                    if (result.code == 200) {
//                        toastr.success(result.message);
//                        setTimeout(function () {
//                            window.location = '/InvoiceMaster/PayOutInvoice';
//                        }, 2000);
//                    } else {
//                        toastr.error(result.message);
//                    }
//                },
//                error: function (xhr, status, error) {
//                    siteloaderhide();
//                    toastr.error('An error occurred while processing your request.');
//                }
//            });
//        } else {
//            siteloaderhide();
//            toastr.warning("Kindly fill all data fields");
//        }
//    } else {
//        siteloaderhide();
//        toastr.warning("Add payout details");
//    }
//}
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
var selectedstartDate = null;
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
            startDate: selectedstartDate,
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
        $('#PayoutstartDate').val('');
        $('#PayoutendDate').val('');
    }
    $('#timePeriodPayoutDropdown').change(function () {
        var selectedValue = $(this).val();

        if (selectedValue === 'This Month' || selectedValue === 'This Year') {
            $('#PayoutstartDate, #PayoutendDate, #PayoutyearDropdown').hide();
        } else if (selectedValue === 'Between Date') {
            $('#PayoutstartDate, #PayoutendDate, #searchPayoutReportButton').show();
            $('#PayoutyearDropdown').hide();
            clearDates();
        } else if (selectedValue === 'Between Year') {
            $('#PayoutyearDropdown, #searchPayoutReportButton').show();
            $('#PayoutstartDate, #PayoutendDate').hide();
            populateYearDropdown();
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
function SearchPayoutReportData() {
    var selectedValue = $('#timePeriodPayoutDropdown').val();
    var selectedSupplierId = $('#textPayoutReportSupplierNameHidden').val();
    var selectedCompanyId = $('#textPayoutReportCompanyName').val();
    var selectedGroupName = $('#textPayoutReportGroupList').val();
    var selectedReportSiteName = $('#txtSiteId').val();
    var selectedSortOrder = "AscendingDate";
    var selectedstartDate, selectedendDate, selectedYears;

    var PayOutReport = {
        SiteId: selectedReportSiteName || null,
        CompanyId: selectedCompanyId || null,
        SupplierId: selectedSupplierId || null,
        GroupName: selectedGroupName || null,
        sortBy: selectedSortOrder,
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
            selectedstartDate = $('#PayoutstartDate').val();
            selectedendDate = $('#PayoutendDate').val();
            if (!selectedstartDate || !selectedendDate) {
                toastr.warning("Select dates");
                return;
            }
            PayOutReport.filterType = "dateRange";
            PayOutReport.startDate = selectedstartDate;
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


    getnetamount(PayOutReport);
}
function getnetamount(PayOutReport) {
    $.ajax({
        url: '/InvoiceMaster/GetInvoiceDetails',
        type: 'POST',
        data: PayOutReport,
        datatype: 'json',
        success: function (result) {

            siteloaderhide();
            $("#reportInvoicenet").html(result);

            if ($("#reportInvoicenet").find(".text-center:contains('No data found for the selected criteria.')").length > 0) {
                $("#downloadnetreportfile").hide();
            } else {
                $("#downloadnetreportfile").show();
            }
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            console.error("An error occurred: " + error);
        }
    });
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
    var selectedstartDate, selectedendDate, selectedYears;

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
            selectedstartDate = $('#PayoutstartDate').val();
            selectedendDate = $('#PayoutendDate').val();
            if (!selectedstartDate || !selectedendDate) {
                toastr.warning("Select dates");
                return;
            }
            PayOutReport.filterType = "dateRange";
            PayOutReport.startDate = selectedstartDate;
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
    var selectedstartDate, selectedendDate, selectedYears;

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
            selectedstartDate = $('#PayoutstartDate').val();
            selectedendDate = $('#PayoutendDate').val();
            if (!selectedstartDate || !selectedendDate) {
                toastr.warning("Select dates");
                return;
            }
            PayOutReport.filterType = "dateRange";
            PayOutReport.startDate = selectedstartDate;
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
