AllSupplierInvoiceListTable()
$(document).ready(function () {
    var itemCounter = 0;

    $("#addItemBtn").click(function () {
        var newItem = $(".invoice-item:first").clone().removeAttr("style");
        newItem.find("input").val(""); // Clear input values for the new item
        $("#invoiceItems").append(newItem);

        itemCounter++;
    });


    $("#SubmitInvoiceBtn").click(function (e) {

        e.preventDefault();

        var formData =
        {
            CompanyAddress: $("#idStatusCompany").val(),
            InvoiceNo: $("#InvoiceNo").val(),
            Date: $("#date-field").val(),
            PaymentStatus: $("#choices-payment-status").val(),
            TotalAmount: $("#totalamountInput").val(),
            BillingName: $("#billingName").val(),
            BillingAddress: $("#billingAddress").val(),
            BillingNumber: $("#billingPhoneno").val(),
            BillingTaxNumber: $("#billingTaxno").val(),
            ShippingName: $("#shippingName").val(),
            ShippingAddress: $("#shippingAddress").val(),
            ShippingNumber: $("#shippingPhoneno").val(),
            ShippingTaxNumber: $("#shippingTaxno").val(),
            ProductName: $("#productName-1").val(),
            ProductDetails: $("#productDetails-1").val(),
            HSN: $("#productHsn-1").val(),
            Price: $("#productRate-1").val(),
            Quantity: $("#product-qty-1").val(),
            PaymentMethod: $("#choices-payment-type").val(),
            CardHolderName: $("#cardholderName").val(),
            CardNumber: $("#cardNumber").val(),
        };
        $.ajax({
            url: '/Invoice/GenerateInvoice',
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response) {
                    generatePdf(response);
                } else {
                    alert("Error generating invoice. Please try again.");
                }
            },
            error: function () {
                alert("An error occurred. Please try again.");
            }
        });
    });

});

function AllSupplierInvoiceListTable() {
   
    var searchText = $('#txtSupplierInvoiceSearch').val();
    var searchBy = $('#SupplierInvoiceSearchBy').val();

    $.get("/InvoiceMaster/SupplierInvoiceListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {


            $("#SupplierInvoicebody").html(result);
        })
        .fail(function (error) {
            console.error(error);
        });
}

function filterSupplierInvoiceTable() {

    var searchText = $('#txtSupplierInvoiceSearch').val();
    var searchBy = $('#SupplierInvoiceSearchBy').val();

    $.ajax({
        url: '/InvoiceMaster/SupplierInvoiceListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            $("#SupplierInvoicebody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function SupplierInvoicesortTable() {
    var sortBy = $('#SortBySupplierInvoice').val();
    $.ajax({
        url: '/InvoiceMaster/SupplierInvoiceListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            $("#SupplierInvoicebody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function DeleteSupplierInvoiceDetails(InvoiceId)
{
    debugger
    Swal.fire({
        title: "Are you sure want to Delete This?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, Delete it!",
        cancelButtonText: "No, cancel!",
        confirmButtonClass: "btn btn-primary w-xs me-2 mt-2",
        cancelButtonClass: "btn btn-danger w-xs mt-2",
        buttonsStyling: false,
        showCloseButton: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/InvoiceMaster/DeleteSupplierInvoiceDetails?InvoiceId=' + InvoiceId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {

                    Swal.fire({
                        title: Result.message,
                        icon: 'success',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK'
                    }).then(function () {
                        window.location = '/InvoiceMaster/SupplierInvoiceListView';
                    })
                },
                error: function () {
                    Swal.fire({
                        title: "Can't Delete Site!",
                        icon: 'warning',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK',
                    }).then(function () {
                        window.location = '/InvoiceMaster/SupplierInvoiceListView';
                    })
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Site Have No Changes.!!😊',
                'error'
            );
        }
    });   
}