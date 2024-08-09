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
        method: 'GET',
        success: function (result) {
            var $dropdown = $("#textReportCompanyName");
            $dropdown.empty();
            $dropdown.append('<option value="">Select Company</option>');
            result.forEach(function (data) {
                $dropdown.append('<option value="' + data.companyId + '">' + data.companyName + '</option>');
            });
        },
        error: function (err) {
            console.error("Failed to fetch company list: ", err);
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

var selectedSiteId = null;
var selectedCompanyId = null;
var selectedSupplierId = null;
var selectedstartDate = null;
var selectedendDate = null;
var selectedfilterType = null;

function populateYearDropdown() {
    var currentYear = new Date().getFullYear();
    var yearDropdown = $('#yearDropdown');
    yearDropdown.empty().append('<option value="">Select Year</option>');

    for (var year = currentYear - 20; year <= currentYear + 20; year++) {
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
        GetInvoiceReportData();
    });

    $('#textReportCompanyName').change(function () {
        selectedCompanyId = $(this).val();
        GetInvoiceReportData();
    });

    $('.nav-radio').click(function () {
        var radioId = $(this).attr('id');

        if (radioId === 'currentMonthRadio') {
            GetCurrentMonthInvoiceList();
            $('#startDate, #endDate, #yearDropdown, #searchBetweenDate').hide();
        } else if (radioId === 'currentYearRadio') {
            GetCurrentYearInvoiceList();
            $('#startDate, #endDate, #yearDropdown, #searchBetweenDate').hide();
        } else if (radioId === 'betweenMonthRadio') {
            $('#startDate, #endDate, #searchBetweenDate').show();
            $('#yearDropdown').hide();
            clearDates();
        } else if (radioId === 'betweenYearRadio') {
            $('#yearDropdown, #searchBetweenDate').show();
            $('#startDate, #endDate').hide();
            populateYearDropdown();
        }
    });

    $('.nav-btn').click(function () {

        var buttonId = $(this).attr('id');

        if (buttonId === 'searchBetweenDate') {
            if ($('#betweenMonthRadio').is(':checked')) {
                GetBetweenDateInvoiceList();
            } else if ($('#betweenYearRadio').is(':checked')) {
                GetBetweenYearInvoiceList();
            }
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
            endDate: selectedendDate
        };
        loadReportData(objData);
    }
}

function GetCurrentMonthInvoiceList() {

    selectedfilterType = "currentMonth";
    var objData = {
        CompanyId: selectedCompanyId,
        SupplierId: selectedSupplierId,
        filterType: selectedfilterType
    };
    loadReportData(objData);
}

function GetCurrentYearInvoiceList() {

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


function GetBetweenYearInvoiceList() {
    debugger
    var selectedYears = $('#yearDropdown').val();
    var selectedFilterType = "betweenYear";

    if (selectedYears) {
        var objData = {
            CompanyId: selectedCompanyId,
            SupplierId: selectedSupplierId,
            filterType: selectedFilterType,
            SelectedYear: selectedYears,
        };
        loadReportData(objData);
    } else {
        alert('Please select a year.');
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