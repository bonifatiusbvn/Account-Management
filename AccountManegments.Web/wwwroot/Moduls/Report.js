GetAllSiteList();
GetAllCompanyList();
GetAllSupplierList();
GetGroupList();
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
        method: 'GET',
        success: function (result) {
            var $dropdown = $("#textReportCompanyName");
            $dropdown.empty();
            $dropdown.append('<option value="">Select Company</option>');
            result.forEach(function (data) {
                $dropdown.append(
                    '<option value="' + data.companyId + '" data-company-name="' + data.companyName + '">' + data.companyName + '</option>'
                );
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
                $(this).autocomplete("search", "");
            });
        },
        error: function (err) {
            console.error("Failed to fetch supplier list: ", err);
        }
    });
}

let currentReportSortOrder = 'DescendingDate';
function sortReportTable(field) {
    if (currentReportSortOrder === 'Descending' + field) {
        currentReportSortOrder = 'Ascending' + field;
    } else {
        currentReportSortOrder = 'Descending' + field;
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

    function clearDates() {
        $('#startDate').val('');
        $('#endDate').val('');
    }

    $('#timePeriodDropdown').change(function () {
        var selectedValue = $(this).val();

        if (selectedValue === 'This Month' || selectedValue === 'This Year') {
            $('#startDate, #endDate, #yearDropdown').hide();
        } else if (selectedValue === 'Between Date') {
            $('#startDate, #endDate, #searchReportButton').show();
            $('#yearDropdown').hide();
            clearDates();
        } else if (selectedValue === 'Between Year') {
            $('#yearDropdown, #searchReportButton').show();
            $('#startDate, #endDate').hide();
            populateYearDropdown();
        }
    });
});
function SearchReportData() {
    var selectedValue = $('#timePeriodDropdown').val();
    var selectedSupplierId = $('#textReportSupplierNameHidden').val();
    var selectedCompanyId = $('#textReportCompanyName').val();
    var selectedGroupName = $('#textReportGroupList').val();
    var selectedReportSiteName = $('#txtReportSiteId').val();
    var selectedstartDate, selectedendDate, selectedYears;

    var objData = {
        SiteId: selectedReportSiteName || null,
        CompanyId: selectedCompanyId || null,
        SupplierId: selectedSupplierId || null,
        GroupName: selectedGroupName || null,
        filterType: null,
        startDate: null,
        endDate: null,
        SelectedYear: null,
    };

    switch (selectedValue) {
        case 'This Month':
            objData.filterType = "currentMonth";
            break;
        case 'This Year':
            objData.filterType = "currentYear";
            break;
        case 'Between Date':
            selectedstartDate = $('#startDate').val();
            selectedendDate = $('#endDate').val();
            if (!selectedstartDate || !selectedendDate) {
                toastr.warning("Select dates");
                return;
            }
            objData.filterType = "dateRange";
            objData.startDate = selectedstartDate;
            objData.endDate = selectedendDate;
            break;
        case 'Between Year':
            selectedYears = $('#yearDropdown').val();
            if (!selectedYears) {
                alert('Please select a year.');
                return;
            }
            objData.filterType = "betweenYear";
            objData.SelectedYear = selectedYears;
            break;
        default:
            selectedValue = null;
            break;
    }

    loadReportData(objData);
}
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

$(document).ready(function () {
    $("#textReportCompanyName").on('change', function () {
        var selectedOption = $(this).find('option:selected');
        selectedCompanyName = selectedOption.data('company-name');
    });
});

function ExportToPDF() {

    siteloadershow();
    var selectedValue = $('#timePeriodDropdown').val();
    var selectedSupplierId = $('#textReportSupplierNameHidden').val();
    var selectedSupplierName = $('#textReportSupplierName').val();
    var selectedCompanyId = $('#textReportCompanyName').val();
    var selectedGroupName = $('#textReportGroupList').val();
    var selectedReportSiteName = $('#txtReportSiteId').val();
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
    var selectedValue = $('#timePeriodDropdown').val();
    var selectedSupplierId = $('#textReportSupplierNameHidden').val();
    var selectedSupplierName = $('#textReportSupplierName').val();
    var selectedCompanyId = $('#textReportCompanyName').val();
    var selectedGroupName = $('#textReportGroupList').val();
    var selectedReportSiteName = $('#txtReportSiteId').val();
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

function GetPayoutReportData() {

    if (selectedCompanyId || selectedSupplierId) {
        var PayOutReport = {
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            startDate: selectedstartDate,
            endDate: selectedendDate,
            GroupName: null,
        };
        getnetamount(PayOutReport);
    }
}
function GetPayoutGroupData() {

    if (selectedGroupName) {
        var PayOutReport = {
            GroupName: selectedGroupName
        };
        getnetamount(PayOutReport);
    }
}
function GetCurrentMonthPayoutInvoiceList() {

    selectedfilterType = "currentMonth";
    selectedGroupName = null;
    var PayOutReport = {
        CompanyId: selectedCompanyId,
        SupplierId: selectedSupplierId,
        filterType: selectedfilterType,
        GroupName: null,
    };
    getnetamount(PayOutReport);
}
function GetCurrentYearPayoutInvoiceList() {

    selectedfilterType = "currentYear";
    selectedGroupName = null;
    var PayOutReport = {
        CompanyId: selectedCompanyId,
        SupplierId: selectedSupplierId,
        filterType: selectedfilterType,
        GroupName: null,
    };
    getnetamount(PayOutReport);
}
function GetBetweenDatePayoutInvoiceList() {
    selectedstartDate = $('#startDate').val();
    selectedendDate = $('#endDate').val();
    selectedfilterType = "dateRange";
    selectedGroupName = null;

    if (!selectedstartDate || !selectedendDate) {
        toastr.warning("Select dates");
    } else {
        var PayOutReport = {
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            startDate: selectedstartDate,
            endDate: selectedendDate,
            GroupName: null,
        };
        getnetamount(PayOutReport);
    }
}
function GetBetweenYearPayoutInvoiceList() {
    var selectedYears = $('#yearDropdown').val();
    var selectedFilterType = "betweenYear";
    selectedGroupName = null;

    if (selectedYears) {
        var PayOutReport = {
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedFilterType,
            SelectedYear: selectedYears,
            GroupName: null,
        };
        getnetamount(PayOutReport);
    } else {
        alert('Please select a year.');
    }
}

let rowCounter = 0;
function AddNewRowforPayOutInvoicebtn() {
    selectedSupplierId = $('#textReportSupplierNameHidden').val();
    selectedCompanyId = $('#textReportCompanyName').val();
    selectedReportSiteName = $('#txtReportSiteId').val();
    var CompanyId = selectedCompanyId;
    var SupplierId = selectedSupplierId;
    var SiteId = selectedReportSiteName;
    if (SiteId != "" && SupplierId != "" && CompanyId != "") {
        $.ajax({
            url: '/InvoiceMaster/DisplayPayOutInvoicePayOutInvoice',
            type: 'Post',
            datatype: 'json',
            processData: false,
            contentType: false,
            complete: function (Result) {
                if (Result.statusText === "success" || Result.statusText === "OK") {
                    rowCounter++;
                    AddNewRow(Result.responseText, rowCounter);
                } else {
                    toastr.error("Error in display product");
                }
            }
        });
    } else {
        toastr.warning("select site, company and supplier");
    }
}
function AddNewRow(resultHtml, rowNumber) {
    const rowHtml = resultHtml
        .replace(/ROWID/g, rowNumber)
        .replace(/ROWNUMBER/g, rowNumber);

    $('#payoutpartialView').show();
    $('#payoutpartialView').append(rowHtml);
    $('#payoutsubmitbutton').show();
}
function removePayout(buttonElement) {
    $(buttonElement).closest('tr').remove();
    updatePayoutRowNumbers();

    if ($('.payoutinvoicerow').length >= 1) {
        $('#payoutsubmitbutton').show();
    }
    else {
        $('#payoutsubmitbutton').hide();
    }
}
function updatePayoutRowNumbers() {
    $('#payoutpartialView .payoutinvoicerow').each(function (index) {
        $(this).find('.row-number').text(index + 1 + '.');
    });
}
function InsertPayOutDetailsReport() {
    siteloadershow();
    if ($('.payoutinvoicerow').length >= 1) {
        var PayoutDetails = [];
        var isValidPayout = true;

        $(".payoutinvoicerow").each(function () {
            var orderRow = $(this);

            var objData = {
                InvoiceNo: "PayOut",
                SiteId: $("#txtReportSiteId").val(),
                SupplierId: $('#textReportSupplierNameHidden').val(),
                CompanyId: $('#textReportCompanyName').val(),
                PaymentStatus: orderRow.find("input[name^='paymenttype']:checked").val(),
                Description: orderRow.find("input[id^='txtdescription']").val(),
                Date: orderRow.find("input[id^='txtdate']").val(),
                CreatedBy: $("#txtReportUserId").val(),
                TotalAmount: orderRow.find("input[id^='txtpayoutamount']").val()
            };

            orderRow.find("input[id^='txtdate']").on('input', function () {
                $(this).css("border", "1px solid #ced4da");
            });

            orderRow.find("input[id^='txtpayoutamount']").on('input', function () {
                $(this).css("border", "1px solid #ced4da");
            });

            if (objData.Date === "" || objData.TotalAmount === "") {
                isValidPayout = false;

                if (objData.Date === "") {
                    orderRow.find("input[id^='txtdate']").css("border", "2px solid red");
                }

                if (objData.TotalAmount === "") {
                    orderRow.find("input[id^='txtpayoutamount']").css("border", "2px solid red");
                }
            } else {
                PayoutDetails.push(objData);
            }
        });

        if (isValidPayout) {
            var form_data = new FormData();
            form_data.append("PAYOUTDETAILS", JSON.stringify(PayoutDetails));

            $.ajax({
                url: '/InvoiceMaster/InsertPayOutDetails',
                type: 'POST',
                data: form_data,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (result) {
                    siteloaderhide();
                    if (result.code == 200) {
                        toastr.success(result.message);
                        clearPayoutPartialView();
                        $('#payoutpartialView').hide();
                        SearchReportData();
                    } else {
                        toastr.error(result.message);
                    }
                },
                error: function (xhr, status, error) {
                    siteloaderhide();
                    toastr.error('An error occurred while processing your request.');
                }
            });
        } else {
            siteloaderhide();
            toastr.warning("Kindly fill all data fields");
        }
    } else {
        siteloaderhide();
        toastr.warning("Add payout details");
    }
}

function fn_ResetAllDropdown() {
    window.location = '/Report/ReportDetails';
}
function clearPayoutPartialView() {
    $('.payoutinvoicerow').remove();
    rowCounter = 0;
}

function saveOpeningBalanceInvoice() {
    siteloadershow();
    var PayoutDetails = [];
    var isValidPayout = true;

    var objData = {
        InvoiceNo: "Opening Balance",
        SupplierId: $('#textReportSupplierNameHidden').val(),
        CompanyId: $('#textReportCompanyName').val(),
        Description: $('#txtOBdescription').val(),
        Date: $('#txtOBdate').val(),
        CreatedBy: $("#txtReportUserId").val(),
        TotalAmount: $('#txtOBamount').val(),
    };
    $('#txtOBamount').on('input', function () {
        $('#txtOBamount').css("border", "1px solid #ced4da");
    });

    if (objData.TotalAmount === "") {
        isValidPayout = false;
        $('#txtOBamount').css("border", "2px solid red");
    } else {
        PayoutDetails.push(objData);
        $('#txtOBamount').css("border", "");
    }

    if (isValidPayout) {
        var form_data = new FormData();
        form_data.append("PAYOUTDETAILS", JSON.stringify(PayoutDetails));

        $.ajax({
            url: '/InvoiceMaster/InsertPayOutDetails',
            type: 'POST',
            data: form_data,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (result) {
                siteloaderhide();
                if (result.code === 200) {
                    $("#OBPayoutModal").modal('hide');
                    toastr.success(result.message);
                } else {
                    toastr.error(result.message);
                }
            },
            error: function (xhr, status, error) {
                siteloaderhide();
                toastr.error('An error occurred while processing your request.');
            }
        });
    } else {
        siteloaderhide();
        toastr.warning("Kindly fill all data fields");
    }
}
function ClearOBTextBox() {
    $('#txtOBdescription').val('');
    $('#txtOBdate').val('');
    $('#txtOBamount').val('');
    $('#txtOBamount').css("border", "1px solid #ced4da");
}
var datas = userPermissions
$(document).ready(function () {
    var table;

    $('#searchReportButton').click(function () {

        var userPermissionArray = JSON.parse(datas);
        var canEdit = false;
        var canDelete = false;

        for (var i = 0; i < userPermissionArray.length; i++) {
            var permission = userPermissionArray[i];
            if (permission.formName == "Details Report & Payout") {
                canEdit = permission.edit;
                canDelete = permission.delete;
                break;
            }
        }
        var columns = [
            { "data": "invoiceNo", "name": "InvoiceNo" },
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
                    if (row.invoiceNo !== 'PayOut') {
                        return '<span style="color:green">' + '₹' + data + '</span>';
                    } else {
                        return '';
                    }
                }
            },
            {
                "data": "totalAmount",
                "name": "Debit",
                "render": function (data, type, row) {
                    if (row.invoiceNo === 'PayOut') {
                        return '<span style="color:red">' + '₹' + data + '</span>';
                    } else {
                        return '';
                    }
                }
            },
        ];
        if (canEdit || canDelete) {
            columns.push({
                "data": null,
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
        if ($.fn.DataTable.isDataTable('#tblinvoice')) {
            table.destroy();
        }

        table = $('#tblinvoice').DataTable({
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
                url: '/Report/GetSupplierInvoiceDetailsReport',
                type: 'POST',
                data: function (d) {
                    d.draw = d.draw;
                    d.start = d.start;
                    d.length = d.length;
                    d.order = d.order;
                    d.columns = d.columns;
                    d.SiteId = $('#txtReportSiteId').val() || null;
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
            columns: columns,
            scrollX: true,
            scrollY: '550px',
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
                var netAmount = totalCredit - totalDebit;

                $(api.table().footer()).find('#totalCredit').html('<span>' + '₹' + totalCredit.toFixed(2) + '</span>');
                $(api.table().footer()).find('#totalDebit').html('<span>' + '₹' + totalDebit.toFixed(2) + '</span>');
                $(api.table().footer()).find('#NetAmount').html('<span>' + '₹' + netAmount.toFixed(2) + '</span>');

            },
            columnDefs: [{
                defaultContent: "",
                targets: "_all",
                width: 'auto'
            }]
        });
    });
});

function openOB() {
    siteloadershow();
    ClearOBTextBox();
    var supplierId = $('#textReportSupplierNameHidden').val();
    var companyId = $('#textReportCompanyName').val();

    if (supplierId !== "" && companyId !== "") {
        var objData = {
            SupplierId: supplierId,
            CompanyId: companyId,
        };

        $.ajax({
            url: '/InvoiceMaster/CheckOpeningBalance',
            type: 'GET',
            data: objData,
            datatype: 'json',
            success: function (result) {
                siteloaderhide();
                if (result.code == 200) {
                    var modal = new bootstrap.Modal(document.getElementById('OBPayoutModal'));
                    modal.show();
                } else {
                    toastr.error(result.message);
                }
            },
            error: function (xhr, status, error) {
                siteloaderhide();
                toastr.error('An error occurred while processing your request.');
            }
        });
    } else {
        siteloaderhide();
        toastr.warning("Select company and supplier");
    }
}
