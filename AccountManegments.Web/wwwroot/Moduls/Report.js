GetAllSiteList();
GetAllCompanyList();
GetAllSupplierList();
loadReportData();
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
                $dropdown.append('<option value="' + data.companyId + '">' + data.companyName + '</option>');
            });

            if (result.length > 0) {
                $dropdown.val(result[0].companyId).trigger('change');
            }
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
            $('#textReportGroupList').empty();
            $('#textReportGroupList').append('<option value="">Select Group</option>');
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

var selectedSiteId = null;
var selectedCompanyId = null;
var selectedSupplierId = null;
var selectedstartDate = null;
var selectedendDate = null;
var selectedfilterType = null;
var selectedGroupName = null;
var selectedYears = null;
var selectedSortOrder = "DescendingDate";

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
    $('#textReportSupplierNameHidden').change(function () {
        selectedSupplierId = $(this).val();
        GetPayoutReportData();
        GetGroupList();
        GetInvoiceReportData();
    });

    $('#textReportCompanyName').change(function () {
        selectedCompanyId = $(this).val();
        GetGroupList();
        GetInvoiceReportData();
        GetPayoutReportData();
    });
    $('#textReportGroupList').change(function () {
        selectedGroupName = $(this).val();
        GetInvoiceGroupData();
        GetPayoutGroupData();
    });

    $('#timePeriodDropdown').change(function () {
        var selectedValue = $(this).val();

        if (selectedValue === 'This Month') {
            GetCurrentMonthInvoiceList();
            GetCurrentMonthPayoutInvoiceList();
            $('#startDate, #endDate, #yearDropdown, #searchBetweenDate').hide();
        } else if (selectedValue === 'This Year') {
            GetCurrentYearInvoiceList();
            GetCurrentYearPayoutInvoiceList();
            $('#startDate, #endDate, #yearDropdown, #searchBetweenDate').hide();
        } else if (selectedValue === 'Between Date') {
            $('#startDate, #endDate, #searchBetweenDate').show();
            $('#yearDropdown').hide();
            clearDates();
        } else if (selectedValue === 'Between Year') {
            $('#yearDropdown, #searchBetweenDate').show();
            $('#startDate, #endDate').hide();
            populateYearDropdown();
        }
    });

    $('.nav-btn').click(function () {
        var selectedValue = $('#timePeriodDropdown').val();

        if (selectedValue === 'Between Date') {
            GetBetweenDateInvoiceList();
            GetBetweenDatePayoutInvoiceList();
        } else if (selectedValue === 'Between Year') {
            GetBetweenYearInvoiceList();
            GetBetweenYearPayoutInvoiceList();
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

    if (selectedCompanyId || selectedSupplierId) {
        var objData = {
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            startDate: selectedstartDate,
            endDate: selectedendDate,
            sortBy: selectedSortOrder,
            GroupName: null,
        };
        loadReportData(objData);
    }
}
function GetInvoiceGroupData() {

    if (selectedGroupName) {
        var objData = {
            GroupName: selectedGroupName,
            sortBy: selectedSortOrder,
        };
        loadReportData(objData);
    }
}
function GetCurrentMonthInvoiceList() {

    selectedfilterType = "currentMonth";
    selectedGroupName = null
    var objData = {
        CompanyId: selectedCompanyId,
        SupplierId: selectedSupplierId,
        filterType: selectedfilterType,
        sortBy: selectedSortOrder,
        GroupName: null,
    };
    loadReportData(objData);
}

function GetCurrentYearInvoiceList() {

    selectedfilterType = "currentYear";
    selectedGroupName = null
    var objData = {
        CompanyId: selectedCompanyId,
        SupplierId: selectedSupplierId,
        filterType: selectedfilterType,
        sortBy: selectedSortOrder,
        GroupName: null,
    };
    loadReportData(objData);
}

function GetBetweenDateInvoiceList() {
    selectedstartDate = $('#startDate').val();
    selectedendDate = $('#endDate').val();
    selectedfilterType = "dateRange";
    selectedGroupName = null

    if (!selectedstartDate || !selectedendDate) {
        toastr.warning("Select dates");
    } else {
        var objData = {
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            startDate: selectedstartDate,
            endDate: selectedendDate,
            sortBy: selectedSortOrder,
            selectedGroupName: null,
        };
        loadReportData(objData);
    }
}


function GetBetweenYearInvoiceList() {
    selectedYears = $('#yearDropdown').val();
    var selectedFilterType = "betweenYear";
    selectedGroupName = null

    if (selectedYears) {
        var objData = {
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedFilterType,
            SelectedYear: selectedYears,
            selectedGroupName: null,
        };
        loadReportData(objData);
    } else {
        alert('Please select a year.');
    }
}

function ExportToPDF() {
    siteloadershow();
    if (selectedGroupName) {
        var objData = {
            GroupName: selectedGroupName,
            sortBy: "AscendingDate",
        };
    }
    else {
        var objData = {
            SiteId: selectedSiteId,
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            startDate: selectedstartDate,
            endDate: selectedendDate,
            GroupName: selectedGroupName,
            SelectedYear: selectedYears,
            sortBy: "AscendingDate",
        };
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
    if (selectedGroupName) {
        var objData = {
            GroupName: selectedGroupName,
            sortBy: "AscendingDate",
        };
    }
    else {
        var objData = {
            SiteId: selectedSiteId,
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            startDate: selectedstartDate,
            endDate: selectedendDate,
            GroupName: selectedGroupName,
            SelectedYear: selectedYears,
            sortBy: "AscendingDate",
        };
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
                            window.location = '/Report/ReportDetails';
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
                    loadReportData();
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
function getnetamount(PayOutReport) {
    $.ajax({
        url: '/InvoiceMaster/GetInvoiceDetails',
        type: 'POST',
        data: PayOutReport,
        datatype: 'json',
        success: function (result) {

            siteloaderhide();
            $("#dispalybody").addClass('d-none');
            $("#dispalynetamount").html(result);

            $('#txtpayoutamount').on('input', function () {
                var enteredAmount = parseFloat($(this).val());

                if (!isNaN(enteredAmount)) {
                    var pendingAmount = totalpendingAmount - enteredAmount;

                    if (enteredAmount > totalpendingAmount) {
                        $('#spnpayout').text('Entered amount cannot exceed pending amount.');
                    } else {
                        $('#txtpendingamount').val(pendingAmount.toFixed(2));
                        $('#spnpayout').text('');
                    }
                } else {
                    $('#spnpayout').text('');
                    $('#txtpendingamount').val('');
                }
            });

            if ($("#dispalynetamount").find(".text-center:contains('No data found for the selected criteria.')").length > 0) {
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


let rowCounter = 0;

function AddNewRowforPayOutInvoicebtn() {
    var siteId = $("#txtSiteId").val();
    var CompanyId = selectedCompanyId;
    var SupplierId = selectedSupplierId;
    if (siteId != "" && SupplierId != null && CompanyId != null) {
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

function ExportNetReportToPDF() {
    siteloadershow();
    if (selectedGroupName) {
        var PayOutReport = {
            GroupName: selectedGroupName
        };
    }
    else {
        var PayOutReport = {
            SiteId: selectedSiteId,
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            startDate: selectedstartDate,
            endDate: selectedendDate,
            SelectedYear: selectedYears,
        };
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
    if (selectedGroupName) {
        var PayOutReport = {
            GroupName: selectedGroupName
        };
    }
    else {
        var PayOutReport = {
            SiteId: selectedSiteId,
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedfilterType,
            startDate: selectedstartDate,
            endDate: selectedendDate,
            SelectedYear: selectedYears,
        };
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
                SupplierId: selectedSupplierId,
                CompanyId: selectedCompanyId,
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
                        GetInvoiceReportData();
                        GetPayoutReportData();
                        $('#payoutpartialView').hide();
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