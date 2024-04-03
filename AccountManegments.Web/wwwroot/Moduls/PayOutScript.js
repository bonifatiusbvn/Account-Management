var TotalAmount = '';

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
    $('#txtSuppliername').change(function () {
        var CompanyId = $('#txtcompanyname').val();
        var SupplierId = $(this).val();
        $.ajax({
            url: '/InvoiceMaster/GetInvoiceDetails?CompanyId=' + CompanyId + '&SupplierId=' + SupplierId,
            type: 'GET',
            success: function (result) {
                if (result == "There is no data for selected Supplier!") {
                    Swal.fire({
                        title: 'Warning',
                        text: result,
                        icon: 'warning',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK',
                    });
                } else {
                    $("#invoicedetails").html(result);
                    $("#totalAmount").html('₹' + TotalAmount);
                    GetPayOutTotalAmount();
                }
            },
        });
    });
});

function GetPayOutTotalAmount() {debugger

    var details = {
        CompanyId: $('#txtcompanyname').val(),
        SupplierId: $('#txtSuppliername').val(),
    }
    var form_data = new FormData();
    form_data.append("TOTALAMOUNT", JSON.stringify(details));
    $.ajax({
        url: '/InvoiceMaster/GetPayOutDetailsForTotalAmount',
        type: 'Post',
        data: form_data,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (result) {
            var total = 0;
            result.forEach(function (obj) {
                if (obj.totalAmount) {
                    total += obj.totalAmount;
                }
            });
            var totalamount = $("#totalAmount").html().replace('₹', '');
            $("#txttotalcreditamount").text('₹' + total);
            var totalpendingAmount = totalamount - total;
            var formattedAmount = '₹' + totalpendingAmount.toFixed(2);
            $("#txttotalpendingamount").text(formattedAmount);
            $("#pendingamount").text(formattedAmount);
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
};

function InsertPayOutDetails() {

        var objData = {
            InvoiceNo: "PayOut",
            SiteId: $("#txtSiteId").val(),
            SupplierId: $("#txtSuppliername").val(),
            CompanyId: $("#txtcompanyname").val(),
            TotalAmount: $("#txtpayoutamount").val(),
            TotalGstamount: $("#txtpayoutamount").val(),
            PaymentStatus: $("#paymenttype").val(),
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

$(document).ready(function () {
    function anyCheckboxChecked() {
        return $("input[name='chk_child']:checked").length > 0;
    }

    $('#UserallUnApprovedExpenseTable').on('change', 'input[name="chk_child"]', function () {
        if (anyCheckboxChecked()) {
            $('#remove-actions').show();
        } else {
            $('#remove-actions').hide();
        }
        var allChecked = $('input[name="chk_child"]:checked').length === $('input[name="chk_child"]').length;
        $('#checkedAll').prop('checked', allChecked);
    });

    $('#checkedAll').on('change', function () {
        $('input[name="chk_child"]').prop('checked', $(this).prop('checked'));
        if ($(this).prop('checked')) {
            $('#remove-actions').show();
        } else {
            if (!anyCheckboxChecked()) {
                $('#remove-actions').hide();
            }
        }
    });
});