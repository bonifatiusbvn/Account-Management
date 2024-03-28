AllPurchaseRequestListTable();
GetSiteDetails();
GetCompanyDetails();
GetItemDetails();
GetSupplierDetails();
function AllPurchaseRequestListTable() {
    var searchText = $('#txtPurchaseRequestSearch').val();
    var searchBy = $('#PurchaseRequestSearchBy').val();

    $.get("/PurchaseMaster/PurchaseRequestListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {


            $("#purchaseRequesttbody").html(result);
        })
        .fail(function (error) {
            console.error(error);
        });
}

function filterPurchaseRequestTable() {

    var searchText = $('#txtPurchaseRequestSearch').val();
    var searchBy = $('#PurchaseRequestSearchBy').val();

    $.ajax({
        url: '/PurchaseMaster/PurchaseRequestListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            $("#purchaseRequesttbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}
function GetSiteDetails() {

    $.ajax({
        url: '/SiteMaster/GetSiteNameList',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#txtPoSiteName').append('<Option value=' + data.siteId + '>' + data.siteName + '</Option>')
            });
        }
    });
}
function sortPurchaseRequestTable() {
    var sortBy = $('#PurchaseRequestSortBy').val();
    $.ajax({
        url: '/PurchaseMaster/PurchaseRequestListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            $("#purchaseRequesttbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function SelectPurchaseRequestDetails(PurchaseId, element) {

    $('.row.ac-card').removeClass('active');
    $(element).closest('.row.ac-card').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/PurchaseMaster/DisplayPurchaseRequestDetails?PurchaseId=' + PurchaseId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            if (response) {
                $('#dspPrNo').val(response.prNo);
                $('#dspPId').val(PurchaseId);
                $('#dspItem').val(response.item);
                $('#dspUnitName').val(response.unitName);
                $('#dspQuantity').val(response.quantity);
                $('#dspSiteName').val(response.siteName);
                $('#dspIsApproved').prop('checked', response.isApproved);
            } else {
                console.log('Empty response received.');
            }
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

function CreatePurchaseRequest() {

    var objData = {
        UnitTypeId: $('#txtUnitType').val(),
        Item: $('#txtItemName').val(),
        SiteId: $('#txtPoSiteName').val(),
        Quantity: $('#txtQuantity').val(),
        PrNo: $('#prNo').val(),
    }
    $.ajax({
        url: '/PurchaseMaster/CreatePurchaseRequest',
        type: 'post',
        data: objData,
        datatype: 'json',
        success: function (Result) {

            Swal.fire({
                title: Result.message,
                icon: 'success',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK'
            }).then(function () {
                window.location = '/PurchaseMaster/PurchaseRequestListView';
            });
        },
    })
}

function ClearPurchaseRequestTextBox() {
    resetErrorsMessages();
    $('#txtItemName').val('');
    $('#txtUnitType').val('');
    $('#txtQuantity').val('');
    $('#txtPoSiteName').val('');
    var button = document.getElementById("btnpurchaseRequest");
    if ($('#PurchaseRequestId').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreatePurchaseRequest'));
    offcanvas.show();
}

function validateAndCreatePurchaseRequest() {

    resetErrorsMessages();
    var UnitTypeId = document.getElementById("txtUnitType").value.trim();
    var ItemName = document.getElementById("txtItemName").value.trim();
    var SiteId = document.getElementById("txtPoSiteName").value.trim();
    var Quantity = document.getElementById("txtQuantity").value.trim();

    var isValid = true;

    if (UnitTypeId === "") {
        document.getElementById("spnUnitType").innerText = "Unit Type is required.";
        isValid = false;
    } else if (UnitTypeId === "--Select Unit Type--") {
        document.getElementById("spnUnitType").innerText = "Unit Type is required.";
        isValid = false;
    }


    if (ItemName === "") {
        document.getElementById("spnItemName").innerText = "Item is required.";
        isValid = false;
    }


    if (SiteId === "") {
        document.getElementById("spnSiteName").innerText = "Site is required.";
        isValid = false;
    } else if (SiteId === "--Select SiteName--") {
        document.getElementById("spnSiteName").innerText = "Site is required.";
        isValid = false;
    }


    if (Quantity === "") {
        document.getElementById("spnQuantity").innerText = "Quantity is required.";
        isValid = false;
    }


    if (isValid) {
        if ($("#PurchaseRequestId").val() == '') {
            CreatePurchaseRequest();
        }
        else {
            UpdatePurchaseRequestDetails();
        }
    }
}

function resetErrorsMessages() {
    document.getElementById("spnUnitType").innerText = "";
    document.getElementById("spnItemName").innerText = "";
    document.getElementById("spnSiteName").innerText = "";
    document.getElementById("spnQuantity").innerText = "";
}

function EditPurchaseRequestDetails(PurchaseId) {

    $.ajax({
        url: '/PurchaseMaster/DisplayPurchaseRequestDetails?PurchaseId=' + PurchaseId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            $('#PurchaseRequestId').val(response.pid);
            $('#prNo').val(response.prNo);
            $('#txtItemName').val(response.item);
            $('#txtQuantity').val(response.quantity);
            $('#txtPoSiteName').val(response.siteId);
            $('#txtUnitType').val(response.unitTypeId);
            $('#txtPoSiteName').val(response.siteId);
            var button = document.getElementById("btnpurchaseRequest");
            if ($('#PurchaseRequestId').val() != '') {
                button.textContent = "Update";
            }
            var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreatePurchaseRequest'));
            resetErrorsMessages()
            offcanvas.show();
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

function UpdatePurchaseRequestDetails() {

    var objData = {
        Pid: $('#PurchaseRequestId').val(),
        UnitTypeId: $('#txtUnitType').val(),
        Item: $('#txtItemName').val(),
        SiteId: $('#txtPoSiteName').val(),
        Quantity: $('#txtQuantity').val(),
        PrNo: $('#prNo').val(),
    }

    $.ajax({
        url: '/PurchaseMaster/UpdatePurchaseRequestDetails',
        type: 'post',
        data: objData,
        datatype: 'json',
        success: function (Result) {

            Swal.fire({
                title: Result.message,
                icon: 'success',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK'
            }).then(function () {
                window.location = '/PurchaseMaster/PurchaseRequestListView';
            });
        },
    })

}

function DeletePurchaseRequest(PurchaseId) {
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
                url: '/PurchaseMaster/DeletePurchaseRequest?PurchaseId=' + PurchaseId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {

                    Swal.fire({
                        title: Result.message,
                        icon: 'success',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK'
                    }).then(function () {
                        window.location = '/PurchaseMaster/PurchaseRequestListView';
                    })
                },
                error: function () {
                    Swal.fire({
                        title: "Can't Delete PurchaseRequest!",
                        icon: 'warning',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK',
                    }).then(function () {
                        window.location = '/PurchaseMaster/PurchaseRequestView';
                    })
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Purchase Request Have No Changes.!!😊',
                'error'
            );
        }
    });
}

function PurchaseRequestIsApproved(PurchaseId) {

    var isChecked = $('#flexSwitchCheckChecked_' + PurchaseId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to Approve this Purchase Request?" : "Are you sure want to UnApprove this Purchase Request?";

    Swal.fire({
        title: confirmationMessage,
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, Enter it!",
        cancelButtonText: "No, cancel!",
        confirmButtonClass: "btn btn-primary w-xs me-2 mt-2",
        cancelButtonClass: "btn btn-danger w-xs mt-2",
        buttonsStyling: false,
        showCloseButton: true
    }).then((result) => {
        if (result.isConfirmed) {
            var formData = new FormData();
            formData.append("PurchaseId", PurchaseId);

            $.ajax({
                url: '/PurchaseMaster/PurchaseRequestIsApproved?PurchaseId=' + PurchaseId,
                type: 'Post',
                contentType: 'application/json;charset=utf-8;',
                dataType: 'json',
                success: function (Result) {

                    Swal.fire({
                        title: isChecked ? "Approved!" : "UnApproved!",
                        text: Result.message,
                        icon: "success",
                        confirmButtonClass: "btn btn-primary w-xs mt-2",
                        buttonsStyling: false
                    }).then(function () {
                        window.location = '/PurchaseMaster/PurchaseRequestListView';
                    });
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Purchase Request Have No Changes.!!😊',
                'error'
            ).then(function () {
                window.location = '/PurchaseMaster/PurchaseRequestListView';
            });;
        }
    });
}
function GetCompanyDetails() {

    $.ajax({
        url: '/Company/GetCompanyNameList',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#txtcompanyname').append('<Option value=' + data.companyId + '>' + data.companyName + '</Option>')
            });
        }
    });
}
function GetItemDetails() {

    $.ajax({
        url: '/ItemMaster/GetItemNameList',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#searchItemname').append('<Option value=' + data.itemId + '>' + data.itemName + '</Option>')
            });
        }
    });
}
function GetSupplierDetails() {

    $.ajax({
        url: '/Supplier/GetSupplierNameList',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#txtSuppliername').append('<Option value=' + data.supplierId + '>' + data.supplierName + '</Option>')
            });
        }
    });
}
$(document).ready(function () {
    $('#txtcompanyname').change(function () {
        var CompanyId = $(this).val();
        $.ajax({
            url: '/Company/GetCompnaytById/?CompanyId=' + CompanyId,
            type: 'GET',
            success: function (result) {
                $('#companybillingaddressDetails').empty().append(
                    '<div class="mb-2"><input type="text" class="form-control bg-light border-0" value="' + result.companyName + '" readonly /></div>' +
                    '<div class="mb-2"><textarea class="form-control bg-light border-0" readonly style="height: 90px;">' + result.address + ' , ' + result.area + ' , ' + result.cityName + ' , ' + result.stateName + ' , ' + result.countryName + ' , ' + result.pincode + '</textarea></div>'
                );
            },
            error: function (xhr, status, error) {
                console.error("Error fetching company details:", error);
            }
        });
    });

    $('#sameAsBillingAddress').change(function () {
        var billingAddress = $('#companybillingaddressDetails textarea').val();
        var shippingNameInput = $('#shippingName');
        var shippingAddressInput = $('#shippingAddress');
        if (this.checked) {
            var companyName = $('#companybillingaddressDetails input').val();
            shippingNameInput.val(companyName);
            shippingAddressInput.val(billingAddress);
        } else {
            shippingNameInput.val("");
            shippingAddressInput.val("");
        }
    });
});




function SerchItemDetailsById() {

    var GetItemId = {
        ItemId: $('#searchItemname').val(),

    }
    var form_data = new FormData();
    form_data.append("ITEMID", JSON.stringify(GetItemId));


    $.ajax({
        url: '/ItemMaster/DisplayItemDetailsById',
        type: 'Post',
        datatype: 'json',
        data: form_data,
        processData: false,
        contentType: false,
        complete: function (Result) {
            AddNewRow(Result.responseText);
        }
    });
}

function InsertMultiplePurchaseOrderDetails() {

    var orderDetails = [];
    var numOrders = $(".product").length;
    $(".product").each(function () {

        var orderRow = $(this);
        var objData = {
            POId: $("#txtProjectId").val(),
            ProductId: orderRow.find("#Product_Id").val(),
            ProductType: orderRow.find("#Product_TypeId").val(),
            Quantity: orderRow.find("#txtproductquantity").val(),
            ProductName: orderRow.find("#txtproductName").val(),
            GstPerUnit: orderRow.find("#txtgst").val(),
            SubTotal: orderRow.find("#txtproducttotalamount").val(),
            TotalGst: $("#totalgst").val(),
            TotalAmount: $("#cart-total").val(),
        };
        orderDetails.push(objData);
        debugger
    });
    var form_data = new FormData();
    form_data.append("PODETAILS", JSON.stringify(orderDetails));
    $.ajax({
        url: '/PurchaseMaster/InsertMultiplePurchaseOrderDetails',
        type: 'POST',
        data: form_data,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (result) {
            if (result.message != null) {
                Swal.fire({
                    title: result.message,
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK',
                }).then(function () {
                    window.location = '/PurchaseOrderMaster/CreatePurchaseOrderView';
                });
            }
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

var paymentSign = "$";

function otherPayment() {
    var e = document.getElementById("choices-payment-currency").value;
    paymentSign = e, Array.from(document.getElementsByClassName("product-line-price")).forEach(function (e) {
        isUpdate = e.value.slice(1), e.value = paymentSign + isUpdate
    }), recalculateCart()
}
Array.from(document.getElementsByClassName("product-line-price")).forEach(function (e) {
    e.value = paymentSign + "0.00"
});
//var isPaymentEl = document.getElementById("choices-payment-currency"),
//    choices = new Choices(isPaymentEl, {
//        searchEnabled: !1
//    });

function isData() {
    var e = document.getElementsByClassName("plus"),
        t = document.getElementsByClassName("minus");
    e && Array.from(e).forEach(function (n) {
        n.onclick = function (e) {
            var t;
            parseInt(n.previousElementSibling.value) < 10 && (e.target.previousElementSibling.value++, e = n.parentElement.parentElement.previousElementSibling.querySelector(".product-price").value, t = n.parentElement.parentElement.nextElementSibling.querySelector(".product-line-price"), updateQuantity(n.parentElement.querySelector(".product-quantity").value, e, t))
        }
    }), t && Array.from(t).forEach(function (n) {
        n.onclick = function (e) {
            var t;
            1 < parseInt(n.nextElementSibling.value) && (e.target.nextElementSibling.value--, e = n.parentElement.parentElement.previousElementSibling.querySelector(".product-price").value, t = n.parentElement.parentElement.nextElementSibling.querySelector(".product-line-price"), updateQuantity(n.parentElement.querySelector(".product-quantity").value, e, t))
        }
    })
}

document.querySelector("#profile-img-file-input").addEventListener("change", function () {
    var e = document.querySelector(".user-profile-image"),
        t = document.querySelector(".profile-img-file-input").files[0],
        n = new FileReader;
    n.addEventListener("load", function () {
        e.src = n.result
    }, !1), t && n.readAsDataURL(t)
}), isData();


var count = 0;
function AddNewRow(Result) {
    var newProductRow = $(Result);
    var newProductId = newProductRow.attr('data-product-id');
    var isDuplicate = false;

    $('#addNewlink .product').each(function () {
        var existingProductRow = $(this);
        var existingProductId = existingProductRow.attr('data-product-id');
        if (existingProductId === newProductId) {
            isDuplicate = true;
            return false;
        }
    });

    if (!isDuplicate) {
        count++;
        $("#addNewlink").append(Result);
        updateProductTotalAmount();
        updateTotals();
        updateRowNumbers();
    } else {
        Swal.fire({
            title: "Product already added!",
            text: "The selected product is already added.",
            icon: "warning",
            confirmButtonColor: "#3085d6",
            confirmButtonText: "OK"
        });
    }
}


function updateRowNumbers() {
    $(".product-id").each(function (index) {
        $(this).text(index + 1);
    });
}
function bindEventListeners() {

    document.querySelectorAll(".product-removal a").forEach(function (e) {
        e.addEventListener("click", function (event) {
            removeItem(event.target.closest("tr"));
            updateTotals();
        });
    });


    document.querySelectorAll(".plus").forEach(function (btn) {
        btn.addEventListener("click", function (event) {
            updateProductQuantity(event.target.closest("tr"), 1);
            updateTotals();
        });
    });


    document.querySelectorAll(".minus").forEach(function (btn) {
        btn.addEventListener("click", function (event) {
            updateProductQuantity(event.target.closest("tr"), -1);
            updateTotals();
        });
    });
}

function updateProductTotalAmount() {

    $(".product").each(function () {
        var row = $(this);
        var productPrice = parseFloat(row.find("#txtproductamount").val());
        var quantity = parseInt(row.find("#txtproductquantity").val());
        var gst = parseFloat(row.find("#txtgst").val());
        var totalGst = (productPrice * quantity * gst) / 100;
        var totalAmount = productPrice * quantity;

        row.find("#txtproductamountwithGST").val(totalGst.toFixed(2));
        row.find("#txtproducttotalamount").val(totalAmount.toFixed(2));
    });
}

function updateProductQuantity(row, increment) {

    var quantityInput = row.find(".product-quantity").val();
    var currentQuantity = parseInt(quantityInput);
    var newQuantity = currentQuantity + increment;
    if (newQuantity >= 0) {
        row.find(".product-quantity").val(newQuantity.toFixed(2));;
        updateProductTotalAmount();
        updateTotals();

    }
}

function updateTotals() {

    var totalSubtotal = 0;
    var totalGst = 0;
    var totalAmount = 0;

    $(".product").each(function () {
        var row = $(this);
        var subtotal = parseFloat(row.find("#txtproducttotalamount").val());
        var gst = parseFloat(row.find("#txtproductamountwithGST").val());

        totalSubtotal += subtotal;
        totalGst += gst;
        totalAmount += subtotal + gst;
    });

    $("#cart-subtotal").val(totalSubtotal.toFixed(2));
    $("#totalgst").val(totalGst.toFixed(2));
    $("#cart-total").val(totalAmount.toFixed(2));
}



var taxRate = .125,
    shippingRate = 65,
    discountRate = .15,
    gst = 18;

function remove() {
    Array.from(document.querySelectorAll(".product-removal a")).forEach(function (e) {
        e.addEventListener("click", function (e) {
            removeItem(e), resetRow()
        })
    })
}

function resetRow() {
    Array.from(document.getElementById("addNewlink").querySelectorAll("tr")).forEach(function (e, t) {
        t += 1;
        e.querySelector("#product-id").innerHTML = t
    })
}

function recalculateCart() {
    var t = 0,
        e = (Array.from(document.getElementsByClassName("product")).forEach(function (e) {
            Array.from(e.getElementsByClassName("product-line-price")).forEach(function (e) {
                e.value && (t += parseFloat(e.value.slice(1)))
            })
        }), t * taxRate),
        n = t * discountRate,
        o = 0 < t ? shippingRate : 0,
        a = t + e + o - n,
        b = t * 18 / 100;
    p = t
    document.getElementById("cart-subtotal").value = t.toFixed(2), document.getElementById("cart-tax").value = paymentSign + e.toFixed(2), document.getElementById("totalgst").value = b.toFixed(2), document.getElementById("cart-shipping").value = paymentSign + o.toFixed(2), document.getElementById("cart-total").value = paymentSign + a.toFixed(2), document.getElementById("cart-discount").value = paymentSign + n.toFixed(2), document.getElementById("totalamountInput").value = paymentSign + a.toFixed(2), document.getElementById("amountTotalPay").value = paymentSign + a.toFixed(2)
}

function amountKeyup() {
    Array.from(document.getElementsByClassName("product-price")).forEach(function (n) {
        n.addEventListener("keyup", function (e) {
            var t = n.parentElement.nextElementSibling.nextElementSibling.querySelector(".product-line-price");
            updateQuantity(e.target.value, n.parentElement.nextElementSibling.querySelector(".product-quantity").value, t)
        })
    })
}

function updateQuantity(e, t, n) {
    e = (e = e * t).toFixed(2);
    n.value = paymentSign + e, recalculateCart()
}

function removeItem(e) {
    e.target.closest("tr").remove(), recalculateCart()
}
amountKeyup();
var genericExamples = document.querySelectorAll("[data-trigger]");

function billingFunction() {
    document.getElementById("same").checked ? (document.getElementById("shippingName").value = document.getElementById("billingName").value, document.getElementById("shippingAddress").value = document.getElementById("billingAddress").value, document.getElementById("shippingPhoneno").value = document.getElementById("billingPhoneno").value, document.getElementById("shippingTaxno").value = document.getElementById("billingTaxno").value) : (document.getElementById("shippingName").value = "", document.getElementById("shippingAddress").value = "", document.getElementById("shippingPhoneno").value = "", document.getElementById("shippingTaxno").value = "")
}
Array.from(genericExamples).forEach(function (e) {
    new Choices(e, {
        placeholderValue: "This is a placeholder set in the config",
        searchPlaceholderValue: "This is a search placeholder"
    })
});
//var cleaveBlocks = new Cleave("#cardNumber", {
//    blocks: [4, 4, 4, 4],
//    uppercase: !0
//}),
//    genericExamples = document.querySelectorAll('[data-plugin="cleave-phone"]');
Array.from(genericExamples).forEach(function (e) {
    new Cleave(e, {
        delimiters: ["(", ")", "-"],
        blocks: [0, 3, 3, 4]
    })
});
let viewobj;
var value, invoices_list = localStorage.getItem("invoices-list"),
    options = localStorage.getItem("option"),
    invoice_no = localStorage.getItem("invoice_no"),
    invoices = JSON.parse(invoices_list);
if (null === localStorage.getItem("invoice_no") && null === localStorage.getItem("option") ? (viewobj = "", value = "#VL" + Math.floor(11111111 + 99999999 * Math.random()), document.getElementById("invoicenoInput").value = value) : viewobj = invoices.find(e => e.invoice_no === invoice_no), "" != viewobj && "edit-invoice" == options) {
    document.getElementById("registrationNumber").value = viewobj.company_details.legal_registration_no, document.getElementById("companyEmail").value = viewobj.company_details.email, document.getElementById("companyWebsite").value = viewobj.company_details.website, new Cleave("#compnayContactno", {
        prefix: viewobj.company_details.contact_no,
        delimiters: ["(", ")", "-"],
        blocks: [0, 3, 3, 4]
    }), document.getElementById("companyAddress").value = viewobj.company_details.address, document.getElementById("companyaddpostalcode").value = viewobj.company_details.zip_code;
    for (var preview = document.querySelectorAll(".user-profile-image"), paroducts_list = ("" !== viewobj.img && (preview.src = viewobj.img), document.getElementById("invoicenoInput").value = "#VAL" + viewobj.invoice_no, document.getElementById("invoicenoInput").setAttribute("readonly", !0), document.getElementById("date-field").value = viewobj.date, document.getElementById("choices-payment-status").value = viewobj.status, document.getElementById("totalamountInput").value = "$" + viewobj.order_summary.total_amount, document.getElementById("billingName").value = viewobj.billing_address.full_name, document.getElementById("billingAddress").value = viewobj.billing_address.address, new Cleave("#billingPhoneno", {
        prefix: viewobj.company_details.contact_no,
        delimiters: ["(", ")", "-"],
        blocks: [0, 3, 3, 4]
    }), document.getElementById("billingTaxno").value = viewobj.billing_address.tax, document.getElementById("shippingName").value = viewobj.shipping_address.full_name, document.getElementById("shippingAddress").value = viewobj.shipping_address.address, new Cleave("#shippingPhoneno", {
        prefix: viewobj.company_details.contact_no,
        delimiters: ["(", ")", "-"],
        blocks: [0, 3, 3, 4]
    }), document.getElementById("shippingTaxno").value = viewobj.billing_address.tax, viewobj.prducts), counter = 1; counter++, 1 < paroducts_list.length && document.getElementById("add-item").click(), paroducts_list.length - 1 >= counter;);
    var counter_1 = 1,
        cleave = (setTimeout(() => {
            Array.from(paroducts_list).forEach(function (e) {
                document.getElementById("productName-" + counter_1).value = e.product_name, document.getElementById("productDetails-" + counter_1).value = e.product_details, document.getElementById("productRate-" + counter_1).value = e.rates, document.getElementById("product-qty-" + counter_1).value = e.quantity, document.getElementById("productPrice-" + counter_1).value = "$" + e.rates * e.quantity, counter_1++
            })
        }, 300), document.getElementById("cart-subtotal").value = viewobj.order_summary.sub_total, document.getElementById("cart-tax").value = viewobj.order_summary.estimated_tex, document.getElementById("cart-discount").value = "$" + viewobj.order_summary.discount, document.getElementById("cart-shipping").value = "$" + viewobj.order_summary.shipping_charge, document.getElementById("cart-total").value = "$" + viewobj.order_summary.total_amount, document.getElementById("choices-payment-type").value = viewobj.payment_details.payment_method, document.getElementById("cardholderName").value = viewobj.payment_details.card_holder_name, new Cleave("#cardNumber", {
            prefix: viewobj.payment_details.card_number,
            delimiter: " ",
            blocks: [4, 4, 4, 4],
            uppercase: !0
        }));
    document.getElementById("amountTotalPay").value = "$" + viewobj.order_summary.total_amount, document.getElementById("exampleFormControlTextarea1").value = viewobj.notes
}
document.addEventListener("DOMContentLoaded", function () {
    var T = document.getElementById("invoice_form");
    document.getElementsByClassName("needs-validation");
    T.addEventListener("submit", function (e) {
        e.preventDefault();
        var t = document.getElementById("invoicenoInput").value.slice(4),
            e = document.getElementById("companyEmail").value,
            n = document.getElementById("date-field").value,
            o = document.getElementById("totalamountInput").value.slice(1),
            a = document.getElementById("choices-payment-status").value,
            l = document.getElementById("billingName").value,
            i = document.getElementById("billingAddress").value,
            c = document.getElementById("billingPhoneno").value.replace(/[^0-9]/g, ""),
            d = document.getElementById("billingTaxno").value,
            r = document.getElementById("shippingName").value,
            u = document.getElementById("shippingAddress").value,
            m = document.getElementById("shippingPhoneno").value.replace(/[^0-9]/g, ""),
            s = document.getElementById("shippingTaxno").value,
            p = document.getElementById("choices-payment-type").value,
            v = document.getElementById("cardholderName").value,
            g = document.getElementById("cardNumber").value.replace(/[^0-9]/g, ""),
            y = document.getElementById("amountTotalPay").value.slice(1),
            E = document.getElementById("registrationNumber").value.replace(/[^0-9]/g, ""),
            b = document.getElementById("companyEmail").value,
            I = document.getElementById("companyWebsite").value,
            h = document.getElementById("compnayContactno").value.replace(/[^0-9]/g, ""),
            _ = document.getElementById("companyAddress").value,
            B = document.getElementById("companyaddpostalcode").value,
            f = document.getElementById("cart-subtotal").value.slice(1),
            x = document.getElementById("cart-tax").value.slice(1),
            w = document.getElementById("cart-discount").value.slice(1),
            S = document.getElementById("cart-shipping").value.slice(1),
            j = document.getElementById("cart-total").value.slice(1),
            q = document.getElementById("exampleFormControlTextarea1").value,
            A = document.getElementsByClassName("product"),
            N = 1,
            C = [];
        Array.from(A).forEach(e => {
            var t = e.querySelector("#txtproductName-" + N).value,
                n = e.querySelector("#txtproductDescription-" + N).value,
                o = parseInt(e.querySelector("#txtproductamount-" + N).value),
                p = parseInt(e.querySelector("#txtgst-" + N).value),
                q = parseInt(e.querySelector("#txtproductamountwithGST-" + N).value),
                a = parseInt(e.querySelector("#product-qty-" + N).value),
                e = e.querySelector("#productPrice-" + N).value.split("$"),
                t = {
                    productName: t,
                    productShortDescription: n,
                    perUnitPrice: o,
                    gst: p,
                    perUnitWithGstprice: q,
                    quantity: a,
                    totalAmount: parseInt(e[1])
                };
            C.push(t), N++
        }), !1 === T.checkValidity() ? T.classList.add("was-validated") : ("edit-invoice" == options && invoice_no == t ? (objIndex = invoices.findIndex(e => e.invoice_no == t), invoices[objIndex].invoice_no = t, invoices[objIndex].customer = l, invoices[objIndex].img = "", invoices[objIndex].email = e, invoices[objIndex].date = n, invoices[objIndex].invoice_amount = o, invoices[objIndex].status = a, invoices[objIndex].billing_address = {
            full_name: l,
            address: i,
            phone: c,
            tax: d
        }, invoices[objIndex].shipping_address = {
            full_name: r,
            address: u,
            phone: m,
            tax: s
        }, invoices[objIndex].payment_details = {
            payment_method: p,
            card_holder_name: v,
            card_number: g,
            total_amount: y
        }, invoices[objIndex].company_details = {
            legal_registration_no: E,
            email: b,
            website: I,
            contact_no: h,
            address: _,
            zip_code: B
        }, invoices[objIndex].order_summary = {
            sub_total: f,
            estimated_tex: x,
            discount: w,
            shipping_charge: S,
            total_amount: j
        }, invoices[objIndex].prducts = C, invoices[objIndex].notes = q, localStorage.removeItem("invoices-list"), localStorage.removeItem("option"), localStorage.removeItem("invoice_no"), localStorage.setItem("invoices-list", JSON.stringify(invoices))) : localStorage.setItem("new_data_object", JSON.stringify({
            invoice_no: t,
            customer: l,
            img: "",
            email: e,
            date: n,
            invoice_amount: o,
            status: a,
            billing_address: {
                full_name: l,
                address: i,
                phone: c,
                tax: d
            },
            shipping_address: {
                full_name: r,
                address: u,
                phone: m,
                tax: s
            },
            payment_details: {
                payment_method: p,
                card_holder_name: v,
                card_number: g,
                total_amount: y
            },
            company_details: {
                legal_registration_no: E,
                email: b,
                website: I,
                contact_no: h,
                address: _,
                zip_code: B
            },
            order_summary: {
                sub_total: f,
                estimated_tex: x,
                discount: w,
                shipping_charge: S,
                total_amount: j
            },
            prducts: C,
            notes: q
        })), window.location.href = "apps-invoices-list.html")
    })
});