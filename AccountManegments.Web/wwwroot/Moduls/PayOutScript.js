var TotalPending = '';
var TotalCreadit = '';
var TotalOutstanding = '';
var TotalPurchase = '';

GetCompanyDetails();
GetSupplierDetails();
function GetCompanyDetails() {
    $.ajax({
        url: '/Company/GetCompanyNameList',
        success: function (result) {
            if (result.length > 0) {
                $.each(result, function (i, data) {
                    $('#txtcompanyname').append('<Option value=' + data.companyId + '>' + data.companyName + '</Option>')
                });
            }
        }
    });
}

function GetSupplierDetails() {
    $.ajax({
        url: '/Supplier/GetSupplierNameList',
        success: function (result) {
            if (result.length > 0) {
                $.each(result, function (i, data) {
                    $('#txtSuppliername').append('<Option value=' + data.supplierId + '>' + data.supplierName + '</Option>')
                });
            }
        }
    });
}

$(document).ready(function () {

    $("#totalAmount").html('₹' + 00);
    $("#pendingamount").html('₹' + 00);
    $("#txttotalcreditamount").html('₹' + 00);
    $("#txttotalpendingamount").html('₹' + 00);
    $("#txttotalpurchase").html('₹' + 00);


    $('#txtSuppliername').change(function () {
        siteloadershow();
        var CompanyId = $('#txtcompanyname').val();
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
    var objData = {
        InvoiceNo: "PayOut",
        SiteId: $("#txtSiteId").val(),
        SupplierId: $("#txtSuppliername").val(),
        CompanyId: $("#txtcompanyname").val(),
        TotalAmount: $("#txtpayoutamount").val(),
        TotalGstamount: $("#txtpayoutamount").val(),
        PaymentStatus: $("input[name='paymenttype']:checked").val(),
        Description: $("#txtdescription").val(),
        CreatedBy: $("#txtUserId").val(),
    };

    var form_data = new FormData();
    form_data.append("PAYOUTDETAILS", JSON.stringify(objData));
    $.ajax({
        url: '/InvoiceMaster/InsertPayOutDetails',
        type: 'POST',
        data: form_data,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (result) {
            siteloaderhide();
            Swal.fire({
                title: result.message,
                icon: 'success',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK',
            }).then(function () {
                window.location = '/InvoiceMaster/PayOutInvoice';
            });
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'Error',
                text: 'An error occurred while processing your request.',
                icon: 'error',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK',
            });
        }
    });
}

function validateAndInsertPayOutDetails() {

    var payout = document.getElementById('txtpayoutamount').value.trim();
    var company = document.getElementById('txtcompanyname').value.trim();
    var supplier = document.getElementById('txtSuppliername').value.trim();

    var isValid = true;

    if (payout === "") {
        document.getElementById("spnpayout").innerText = "Please Enter value for PayOut amount!!";
        isValid = false;
    }


    if (company === "") {
        document.getElementById("spncompany").innerText = "Please Select Company!";
        isValid = false;
    }

    if (supplier === "") {
        document.getElementById("spnsupplier").innerText = "Please Select Supplier!";
        isValid = false;
    }

    if (isValid) {
        InsertPayOutDetails();
    }
}