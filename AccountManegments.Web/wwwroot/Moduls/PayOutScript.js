var TotalPending = '';
var TotalCreadit = '';
var TotalOutstanding = '';
var TotalPurchase = '';

GetCompanyDetails();
GetSupplierDetails();
function GetCompanyDetails() {
    $.ajax({
        url: '/Company/GetCompanyNameList',
        method: 'GET',
        success: function (result) {
            var $dropdown = $("#txtcompanyname");
            $dropdown.empty();
            $dropdown.append('<option value="">Select Company</option>');

            result.forEach(function (data) {
                $dropdown.append($('<option>', {
                    value: data.companyId,
                    text: data.companyName
                }));
            });

            $dropdown.change(function () {
                var selectedCompany = $(this).val();
                $("#txtcompanynameHidden").val(selectedCompany);
            });
        },
        error: function (err) {
            console.error("Failed to fetch company names: ", err);
        }
    });
}


function GetSupplierDetails() {
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

            $("#txtSuppliername").autocomplete({
                source: supplierDetails,
                minLength: 0,
                select: function (event, ui) {
                    event.preventDefault();
                    $("#txtSuppliername").val(ui.item.label);
                    $("#txtSuppliernameHidden").val(ui.item.value);

                    $("#txtSuppliernameHidden").trigger('change');
                },
                focus: function () {
                    return false;
                }
            }).focus(function () {
                $(this).autocomplete("search", "");
            });
        },
        error: function (err) {
            console.error("Failed to fetch unit types: ", err);
        }
    });
}

$(document).ready(function () {

    $("#totalAmount").html('₹' + 00);
    $("#pendingamount").html('₹' + 00);
    $("#txttotalcreditamount").html('₹' + 00);
    $("#txttotalpendingamount").html('₹' + 00);
    $("#txttotalpurchase").html('₹' + 00);

    $('#txtSuppliernameHidden').change(function () {
        siteloadershow();
        var CompanyId = $('#txtcompanynameHidden').val();
        var SupplierId = $(this).val();
        $.ajax({
            url: '/InvoiceMaster/GetInvoiceDetails?CompanyId=' + CompanyId + '&SupplierId=' + SupplierId,
            type: 'GET',
            success: function (result) {
                siteloaderhide();
                $("#invoicedetails").html(result);
                $("#txttotalpendingamount").html('₹' + result.totalPending);
                $("#pendingamount").html('₹' + result.totalPending);
                $("#txttotalcreditamount").html('₹' + result.totalCreadit);
                $("#totalAmount").html('₹' + result.totalOutstanding);
                $("#txttotalpurchase").html('₹' + result.totalPurchase);
                var totalpendingAmount = result.totalPending;
                $('#txtpayoutamount').on('input', function () {
                    var enteredAmount = parseFloat($(this).val());

                    if (!isNaN(enteredAmount)) {
                        var pendingAmount = totalpendingAmount - enteredAmount;

                        if (enteredAmount > totalpendingAmount) {
                            $('#spnpayout').text('Entered amount cannot exceed pending amount.');
                        } else {
                            $('#txtpendingamount').val(pendingAmount.toFixed(2));
                        }
                    } else {
                        $('#spnpayout').text('');
                        $('#txtpendingamount').val('');
                    }
                });
            },
        });
    });


});
function InsertPayOutDetails() {
    siteloadershow();
    if ($('.payoutinvoicerow').length >= 1) {
        var PayoutDetails = [];
        var isValidPayout = true;

        $(".payoutinvoicerow").each(function () {
            var orderRow = $(this);

            var objData = {
                InvoiceNo: "PayOut",
                SiteId: $("#txtSiteId").val(),
                SupplierId: $("#txtSuppliernameHidden").val(),
                CompanyId: $("#txtcompanynameHidden").val(),
                PaymentStatus: orderRow.find("input[name^='paymenttype']:checked").val(),
                Description: orderRow.find("input[id^='txtdescription']").val(),
                Date: orderRow.find("input[id^='txtdate']").val(),
                CreatedBy: $("#txtUserId").val(),
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
                        setTimeout(function () {
                            window.location = '/InvoiceMaster/PayOutInvoice';
                        }, 2000);
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