﻿AllSupplierInvoiceListTable()
InvoiceListTable();
GetItemDetailsList()
GetSiteDetail();
GetCompanyDetail();
GetSupplierDetail();

function filterallItemTable() {
    siteloadershow();
    var searchText = $('#mdProductSearch').val();

    $.ajax({
        url: '/PurchaseMaster/GetAllItemDetailsList',
        type: 'GET',
        data: {
            searchText: searchText,
        },
        success: function (result) {
            siteloaderhide();
            $("#mdlistofItem").html(result);
        },

    });
}

function SerchItemDetailsById(Id, inputField) {
    clearItemErrorMessage();
    siteloadershow();
    var qty = $(inputField).closest('.ac-item').find('.product-quantity').val();
    var Item = {
        ItemId: Id,
        Quantity: qty,
    }

    var form_data = new FormData();
    form_data.append("ITEMID", JSON.stringify(Item));


    $.ajax({
        url: '/ItemMaster/DisplayItemListInInvoiceById',
        type: 'Post',
        datatype: 'json',
        data: form_data,
        processData: false,
        contentType: false,
        complete: function (Result) {
            siteloaderhide();
            if (Result.statusText === "success") {
                AddNewRow(Result.responseText);
            }
            else {
                siteloaderhide();
                var GetItemId = $('#searchItemname').val();
                if (GetItemId === "Select ProductName" || GetItemId === null) {
                    $('#searchvalidationMessage').text('Please select ProductName!!');
                }
                else {
                    siteloaderhide();
                    $('#searchvalidationMessage').text('');
                }
            }
        }
    });
}
function GetSiteDetail() {
    $.ajax({
        url: '/SiteMaster/GetSiteNameList',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#textInvoiceSiteName').append('<Option value=' + data.siteId + '>' + data.siteName + '</Option>')
            });
        }
    });
}

$(document).ready(function () {
    $('#textInvoiceSiteName').change(function () {
        var Site = $(this).val();
        $('#textInvoiceSiteName').val(Site);
        $.ajax({
            url: '/SiteMaster/DisplaySiteDetails/?SiteId=' + Site,
            type: 'GET',
            success: function (result) {

                $('#textmdAddress').val(result.shippingAddress + ' , ' + result.shippingArea + ', ' + result.shippingCityName + ', ' + result.shippingStateName + ', ' + result.shippingCountryName + ', ' + result.shippingPincode);
            },
            error: function (xhr, status, error) {
                toastr.error("Error fetching company details:", error);
            }
        });
    });
    $(document).on('click', '#removeAddress', function () {
        $(this).closest('tr').remove();
        $('.add-addresses').prop('disabled', false);
    });


});

function GetItemDetailsList() {

    var searchText = $('#mdProductSearch').val();

    $.get("/InvoiceMaster/GetAllItemDetailList", { searchText: searchText })
        .done(function (result) {
            $("#mdlistofItem").html(result);
        })
}

function GetCompanyDetail() {

    $.ajax({
        url: '/Company/GetCompanyNameList',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#textCompanyName').append('<Option value=' + data.companyId + '>' + data.companyName + '</Option>')
            });
        }
    });
}

$(document).ready(function () {
    $('#textCompanyName').change(function () {

        getCompanyDetail($(this).val());
        getInvoiceNumber($(this).val());
    });
});


function GetSupplierDetail() {

    $.ajax({
        url: '/Supplier/GetSupplierNameList',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#textSupplierName').append('<Option value=' + data.supplierId + '>' + data.supplierName + '</Option>')
            });
        }
    });
}

$(document).ready(function () {
    $('#textSupplierName').change(function () {
        getSupplierDetail($(this).val());
    });
});


$(document).ready(function () {

    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var yyyy = today.getFullYear();

    today = yyyy + '-' + mm + '-' + dd;
    $("#textOrderDate").val(today);




    $(document).ready(function () {
        function handleFocus(event, selector) {
            if (event.keyCode == 13 || event.keyCode == 9) {
                event.preventDefault();
                $(selector).focus();
            }
        }
        function showErrorMessage(selector, message) {
            $(selector).text(message).show();
        }
        $(document).on('input', '#txtproductquantity', function () {
            var productRow = $(this).closest(".product");
            updateProductTotalAmount(productRow);
            updateTotals();
        }).on('keydown', '#txtproductquantity', function (event) {
            var productRow = $(this).closest(".product");
            var productFocus = productRow.find('#txtproductamount');
            handleFocus(event, productFocus);
        });

        $(document).on('input', '#txtgst', function () {
            var productRow = $(this).closest(".product");
            var gstvalue = $('#txtgst').val();
            if (gstvalue > 100) {
                toastr.warning("GST% cannot be greater than 100%");
                $(this).val(100);
            }
            updateProductTotalAmount(productRow);
            updateTotals();
        })

        function debounce(func, delay) {
            let timer;
            return function (...args) {
                clearTimeout(timer);
                timer = setTimeout(() => func.apply(this, args), delay);
            };
        }

        $(document).on('input', '#txtdiscountpercentage', debounce(function () {
            var value = $(this).val();
            var productRow = $(this).closest(".product");
            if (value > 100) {
                toastr.warning("Discount cannot be greater than 100%");
                productRow.find("#txtdiscountpercentage").val(0);
                productRow.find("#txtdiscountamount").val(0);
            } else if (value <= 0 || value == "") {
                productRow.find("#txtdiscountamount").val(0);
                productRow.find("#txtdiscountpercentage").val(0);
                updateProductTotalAmount(productRow);
            } else {
                UpdateDiscountPercentage(productRow);
            }
        }, 300)).on('keydown', '#txtdiscountpercentage', function (event) {
            var productRow = $(this).closest(".product");
            var gstFocus = productRow.find('#txtgst');
            handleFocus(event, gstFocus);
        });

        $(document).on('input', '#txtdiscountamount', debounce(function () {
            var productRow = $(this).closest(".product");
            var discountAmount = parseFloat($(this).val());
            var productAmount = parseFloat($(productRow).find("#productamount").val());

            if (discountAmount > productAmount) {
                toastr.warning("Amount cannot be greater than Item price");
                productRow.find("#txtdiscountamount").val(0);
                productRow.find("#txtdiscountpercentage").val(0);
            } else if (discountAmount <= 0 || discountAmount == "") {
                productRow.find("#txtdiscountamount").val(0);
                productRow.find("#txtdiscountpercentage").val(0);
                updateProductTotalAmount(productRow);
            } else {
                updateDiscount(productRow);
            }
        }, 300)).on('keydown', '#txtdiscountamount', function (event) {
            var productRow = $(this).closest(".product");
            var discountPercentagefocus = productRow.find('#txtdiscountpercentage');
            handleFocus(event, discountPercentagefocus);
        });

        $(document).on('input', '#txtproductamount', function () {
            var productRow = $(this).closest(".product");
            var productAmount = parseFloat($(this).val());
            var discountAmountfocus = productRow.find('#txtdiscountamount');

            if (!isNaN(productAmount)) {
                productRow.find("#txtdiscountamount").val(0);
                productRow.find("#txtdiscountpercentage").val(0);
            }

            productRow.find("#productamount").val(productAmount.toFixed(2));
            updateProductTotalAmount(productRow);
            updateTotals();

        }).on('keydown', '#txtproductamount', function (event) {
            var productRow = $(this).closest(".product");
            var discountAmountfocus = productRow.find('#txtdiscountamount');
            handleFocus(event, discountAmountfocus);
        });


        $(document).on('input', '#cart-roundOff', debounce(function () {
            var roundoff = $('#cart-roundOff').val();
            if (isNaN(roundoff) || (roundoff < -0.99 || roundoff > 0.99)) {
                toastr.warning("Value must be between -0.99 and 0.99");
            }
            else {
                updateTotals();
            }
        }, 300));
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
            siteloaderhide();
            toastr.error(error);
        });
}

function filterSupplierInvoiceTable() {
    siteloadershow();
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
            siteloaderhide();
            $("#SupplierInvoicebody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function SupplierInvoicesortTable() {
    siteloadershow();
    var sortBy = $('#SortBySupplierInvoice').val();
    $.ajax({
        url: '/InvoiceMaster/SupplierInvoiceListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#SupplierInvoicebody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function DeleteSupplierInvoice(Id) {

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
                url: '/InvoiceMaster/DeleteSupplierInvoice?Id=' + Id,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {
                    siteloaderhide();
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
                    siteloaderhide();
                    toastr.error("Can't delete site!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Site have no changes.!!😊',
                'error'
            );
        }
    });
}

$(document).ready(function () {
    $("#shippingAddressForm").validate({
        rules: {
            textInvoiceSiteName: "required",
        },
        messages: {
            textInvoiceSiteName: "Select Site",
        }
    });
});


$(document).ready(function () {
    $("#CreateInvoiceForm").validate({

        rules: {
            textSupplierName: "required",
            textCompanyName: "required",
            paymentStatus: "required",
            textSupplierMobile: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 10
            },

            textSupplierAddress: "required",
        },
        messages: {
            textSupplierName: "Select Supplier Name",
            textCompanyName: "Select Company Name",
            paymentStatus: "Select Payment Status",
            textSupplierMobile: {
                required: "Please Enter Phone Number",
                digits: "Please enter a valid 10-digit phone number",
                minlength: "Phone number must be 10 digits long",
                maxlength: "Phone number must be 10 digits long"
            },
            textSupplierAddress: "Enter supplier address",
        }
    });

});

function clearItemErrorMessage() {
    $("#spnitembutton").text("");
}



function InsertMultipleSupplierItem() {
    siteloadershow();
    if ($("#CreateInvoiceForm").valid()) {
        if ($('#addnewproductlink tr').length >= 1) {

            var ItemDetails = [];
            $(".product").each(function () {
                var orderRow = $(this);
                var objData = {
                    ItemName: orderRow.find("#txtItemName").text(),
                    ItemId: orderRow.find("#txtItemId").val(),
                    UnitType: orderRow.find("#UnitTypeId").val(),
                    DiscountAmount: orderRow.find("#txtdiscountamount").val(),
                    DiscountPer: orderRow.find("#txtdiscountpercentage").val(),
                    Quantity: orderRow.find("#txtproductquantity").val(),
                    PricePerUnit: orderRow.find("#txtproductamount").val(),
                    GSTamount: orderRow.find("#txtgstAmount").val(),
                    GSTPercentage: orderRow.find("#txtgst").val(),
                    TotalAmount: orderRow.find("#txtproducttotalamount").val(),
                };
                ItemDetails.push(objData);
            });
            var sitevalue = $("#textInvoiceSiteName").val();
            var siteid = null;
            if (sitevalue != "") {
                siteid = sitevalue;
            } else {
                siteid = $("#siteid").val();
            }
            var InvoiceDetails = {
                SiteId: siteid,
                InvoiceNo: $("#textInvoicePrefix").val(),
                Date: $("#textOrderDate").val(),
                SupplierId: $("#textSupplierName").val(),
                CompanyId: $("#textCompanyName").val(),
                TotalAmountInvoice: $("#cart-total").val(),
                TotalGstamount: $("#totalgst").val(),
                PaymentStatus: $("input[name='paymentStatus']:checked").val(),
                Description: $("#textDescription").val(),
                CreatedBy: $("#createdbyid").val(),
                UnitTypeId: $("#UnitTypeId").val(),
                ShippingAddress: $("#textmdAddress").val(),
                SupplierInvoiceNo: $("#textSupplierInvoiceNo").val(),
                Roundoff: $('#cart-roundOff').val(),
                TotalDiscount: $('#cart-discount').val(),
                ItemList: ItemDetails,
            }

            var form_data = new FormData();
            form_data.append("SupplierItems", JSON.stringify(InvoiceDetails));
            $.ajax({
                url: '/InvoiceMaster/InsertMultipleSupplierItemDetails',
                type: 'POST',
                data: form_data,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (Result) {
                    siteloaderhide();
                    if (Result.code == 200) {
                        siteloaderhide();
                        toastr.success(Result.message);
                        setTimeout(function () {
                            window.location = '/InvoiceMaster/DisplayInvoiceDetails?Id=' + Result.data;
                        }, 2000);
                    }
                    else {
                        siteloaderhide();
                        toastr.error(Result.message);
                    }
                },
                error: function (xhr, status, error) {
                    siteloaderhide();
                    Swal.fire({
                        title: 'Error',
                        text: 'An error occurred while processing your request.',
                        icon: 'error',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK',
                    });
                }
            });
        } else {
            siteloaderhide();
            if ($('#addnewproductlink tr').length == 0) {
                $("#spnitembutton").text("Please Select Product!");
            } else {
                $("#spnitembutton").text("");
            }
        }
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}
function UpdateInvoiceDetails() {
    siteloadershow();
    if ($("#CreateInvoiceForm").valid()) {

        if ($('#addnewproductlink tr').length >= 1) {

            var ItemDetails = [];
            $(".product").each(function () {
                var orderRow = $(this);
                var objData = {
                    ItemName: orderRow.find("#txtItemName").text(),
                    ItemId: orderRow.find("#txtItemId").val(),
                    UnitType: orderRow.find("#UnitTypeId").val(),
                    DiscountAmount: orderRow.find("#txtdiscountamount").val(),
                    DiscountPer: orderRow.find("#txtdiscountpercentage").val(),
                    Quantity: orderRow.find("#txtproductquantity").val(),
                    PricePerUnit: orderRow.find("#txtproductamount").val(),
                    GSTamount: orderRow.find("#txtgstAmount").val(),
                    GSTPercentage: orderRow.find("#txtgst").val(),
                    TotalAmount: orderRow.find("#txtproducttotalamount").val(),
                };
                ItemDetails.push(objData);
            });
            var sitevalue = $("#textInvoiceSiteName").val();
            var siteid = null;
            if (sitevalue != "") {
                siteid = sitevalue;
            } else {
                siteid = document.getElementById("siteid").getAttribute("value");
            }
            var shippingAdd = $("#textmdAddress").val();
            var Address = null;
            if (shippingAdd != "") {
                Address = shippingAdd;
            } else {
                Address = $(".ShippingAddress").find("#shippingaddress").text().trim();
            }

            var InvoiceDetails = {
                Id: $('#textSupplierInvoiceId').val(),
                SiteId: siteid,
                InvoiceNo: $("#textInvoicePrefix").val(),
                Date: $("#textOrderDate").val(),
                SupplierId: $("#textSupplierName").val(),
                CompanyId: $("#textCompanyName").val(),
                TotalAmountInvoice: $("#cart-total").val(),
                TotalGstamount: $("#totalgst").val(),
                PaymentStatus: $("input[name='paymentStatus']:checked").val(),
                UpdatedBy: $("#createdbyid").val(),
                UnitTypeId: $("#UnitTypeId").val(),
                Description: $("#textDescription").val(),
                SupplierInvoiceNo: $("#textSupplierInvoiceNo").val(),
                Roundoff: $('#cart-roundOff').val(),
                TotalDiscount: $('#cart-discount').val(),
                CreatedOn: $('#textCreatedOn').val(),
                ItemList: ItemDetails,
                ShippingAddress: Address,
            }

            var form_data = new FormData();
            form_data.append("UpdateSupplierItems", JSON.stringify(InvoiceDetails));

            $.ajax({
                url: '/InvoiceMaster/UpdateSupplierInvoice',
                type: 'POST',
                data: form_data,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (Result) {
                    siteloaderhide();
                    if (Result.code == 200) {
                        siteloaderhide();
                        toastr.success(Result.message);
                        setTimeout(function () {
                            window.location = '/InvoiceMaster/SupplierInvoiceListView';
                        }, 2000);
                    }
                    else {
                        siteloaderhide();
                        toastr.error(Result.message);
                    }
                },
                error: function (xhr, status, error) {
                    siteloaderhide();
                    Swal.fire({
                        title: 'Error',
                        text: 'An error occurred while processing your request.',
                        icon: 'error',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK',
                    });
                }
            });
        } else {
            siteloaderhide();
            if ($('#addnewproductlink tr').length == 0) {
                $("#spnitembutton").text("Please Select Product!");
            } else {
                $("#spnitembutton").text("");
            }
        }
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }


}



function UnitTypeDropdown(itemId) {
    debugger

    $.ajax({
        url: '/ItemMaster/GetAllUnitType',
        success: function (result) {
            $('#txtPOUnitType_' + itemId).empty();

            $.each(result, function (i, data) {
                $('#txtPOUnitType_' + itemId).append('<option value=' + data.unitId + '>' + data.unitName + '</option>');
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
    debugger
    var newProductRow = $(Result);
    var itemId = newProductRow.data('product-id');
    var newProductId = newProductRow.attr('data-product-id');
    var isDuplicate = false;

    $('#addnewproductlink .product').each(function () {
        var existingProductRow = $(this);
        var existingProductId = existingProductRow.attr('data-product-id');
        if (existingProductId === newProductId) {
            isDuplicate = true;
            return false;
        }
    });

    if (!isDuplicate) {
        count++;
        $("#addnewproductlink").append(Result);
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
function preventEmptyValue(input) {
    if (input.value === "") {
        input.value = 1;
    }
}

function updateProductTotalAmount(that) {
    var row = $(that);
    var productPrice = parseFloat(row.find("#txtproductamount").val());
    var hiddenproductPrice = parseFloat(row.find("#productamount").val());
    var quantity = parseFloat(row.find("#txtproductquantity").val());
    var discountprice = parseFloat(row.find("#txtdiscountamount").val());
    var AmtWithDisc = hiddenproductPrice - discountprice;

    var gst = parseFloat(row.find("#txtgst").val());

    var totalGst = (AmtWithDisc * quantity * gst) / 100;

    var TotalAmountAfterDiscount = AmtWithDisc * quantity + totalGst;

    row.find("#txtgstAmount").val(totalGst.toFixed(2));
    row.find("#txtproducttotalamount").val(TotalAmountAfterDiscount.toFixed(2));
}

function updateDiscount(that) {
    var row = $(that);
    var productPrice = parseFloat(row.find("#productamount").val());
    var quantity = parseFloat(row.find("#txtproductquantity").val());
    var discountprice = parseFloat(row.find("#txtdiscountamount").val());
    var discountPercentage = parseFloat(row.find("#txtdiscountpercentage").val());

    if (isNaN(discountprice)) {
        row.find("#txtdiscountamount").val(0);
        row.find("#txtdiscountpercentage").val(0);
        updateProductTotalAmount(row);
        updateTotals();
        return;
    }

    if (discountPercentage == 0 && discountprice > 0) {
        var discountperbyamount = discountprice / productPrice * 100;
        row.find("#txtdiscountpercentage").val(discountperbyamount.toFixed(2));
    } else if (discountprice > 0 && discountPercentage > 0) {
        var discountperbyamount = discountprice / productPrice * 100;
        row.find("#txtdiscountpercentage").val(discountperbyamount.toFixed(2));
    }

    updateProductTotalAmount(row);
    updateTotals();
}

function UpdateDiscountPercentage(that) {
    var row = $(that);
    var productPrice = parseFloat(row.find("#productamount").val());
    var quantity = parseFloat(row.find("#txtproductquantity").val());
    var discountprice = parseFloat(row.find("#txtdiscountamount").val());
    var discountPercentage = parseFloat(row.find("#txtdiscountpercentage").val());

    if (isNaN(discountPercentage)) {
        row.find("#txtdiscountamount").val(0);
        row.find("#txtdiscountpercentage").val(0);
        updateProductTotalAmount(row);
        updateTotals();
        return;
    }

    if (discountprice == 0 && discountPercentage > 0) {
        discountprice = productPrice * discountPercentage / 100;
        row.find("#txtdiscountamount").val(discountprice.toFixed(2));
    } else if (discountprice > 0 && discountPercentage > 0) {
        discountprice = productPrice * discountPercentage / 100;
        row.find("#txtdiscountamount").val(discountprice.toFixed(2));
    }
    updateProductTotalAmount(row);
    updateTotals();
}

function updateProductQuantity(row, increment) {
    var quantityInput = parseInt(row.find(".product-quantity").val());
    var newQuantity = quantityInput + increment;
    if (newQuantity >= 0) {
        row.find(".product-quantity").val(newQuantity);
        updateProductTotalAmount(row);
        updateTotals();
    }
}

function updateTotals() {
    var totalSubtotal = 0;
    var totalGst = 0;
    var totalAmount = 0;
    var TotalItemQuantity = 0;
    var TotalDiscount = 0;

    var roundoffvalue = $('#cart-roundOff').val();

    $(".product").each(function () {

        var row = $(this);
        var subtotal = parseFloat(row.find("#txtproductamount").val());
        var gst = parseFloat(row.find("#txtgstAmount").val());
        var totalquantity = parseFloat(row.find("#txtproductquantity").val());
        var discountprice = parseFloat(row.find("#txtdiscountamount").val());

        totalSubtotal += subtotal * totalquantity;
        totalGst += gst;
        totalAmount = totalSubtotal + totalGst;
        TotalItemQuantity += totalquantity;
        TotalDiscount += discountprice;
    });

    $("#cart-subtotal").val(totalSubtotal.toFixed(2));
    $("#totalgst").val(totalGst.toFixed(2));
    $("#cart-discount").val(TotalDiscount.toFixed(2));
    $("#TotalDiscountPrice").html(TotalDiscount.toFixed(2));
    $("#TotalProductQuantity").text(TotalItemQuantity);
    $("#TotalProductPrice").html(totalSubtotal.toFixed(2));
    $("#TotalProductGST").html(totalGst.toFixed(2));
    $("#TotalProductAmount").html(totalAmount.toFixed(2));

    if (roundoffvalue != 0) {

        var roundtotal = parseFloat(totalAmount) + parseFloat(roundoffvalue);
        $("#cart-total").val(roundtotal.toFixed(2))
    } else {
        $("#cart-total").val(totalAmount.toFixed(2));
    }
}
function removeItem(btn) {
    $(btn).closest("tr").remove();
    updateRowNumbers();
    updateTotals();
}


var taxRate = .125,
    shippingRate = 65,
    discountRate = .15,
    gst = 18;

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

function InvoiceListTable() {

    var searchText = $('#txtInvoiceSearch').val();
    var searchBy = $('#InvoiceSearchBy').val();

    $.get("/InvoiceMaster/InvoiceListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {


            $("#Invoicetbody").html(result);
        })
        .fail(function (error) {
            siteloaderhide();
            toastr.error(error);
        });
}

function filterInvoiceTable() {
    siteloadershow();
    var searchText = $('#txtInvoiceSearch').val();
    var searchBy = $('#InvoiceSearchBy').val();

    $.ajax({
        url: '/InvoiceMaster/InvoiceListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#Invoicetbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(error);
        }
    });
}

function InvoiceSortTable() {
    siteloadershow();
    var sortBy = $('#SortByInvoice').val();
    $.ajax({
        url: '/InvoiceMaster/InvoiceListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#Invoicetbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(error);
        }
    });
}

function printInvoiceDiv() {

    var printContents = document.getElementById('displayInvoiceDetail').innerHTML;
    var originalContents = document.body.innerHTML;
    document.body.innerHTML = printContents;
    window.print();
    document.body.innerHTML = originalContents;
}

function getSupplierDetail(SupplierId) {
    siteloadershow();
    $.ajax({
        url: '/Supplier/DisplaySupplier?SupplierId=' + SupplierId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response) {
                $('#textSupplierMobile').val(response.mobile);
                $('#textSupplierGST').val(response.gstno);
                $('#textSupplierAddress').val(response.fullAddress);
            } else {
                siteloaderhide();
                toastr.error('Empty response received.');
            }
        },
    });
}

function getInvoiceNumber(CompanyId) {

    siteloadershow();
    $.ajax({
        url: '/InvoiceMaster/CheckSuppliersInvoiceNo?CompanyId=' + CompanyId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            siteloaderhide();
            if (response.code == 200) {

                siteloaderhide();
                $('#textInvoicePrefix').val(response.data);
            } else {
                siteloaderhide();
                toastr.error('Empty response received.');
            }
        },
    });
}
function getCompanyDetail(CompanyId) {
    siteloadershow();
    $.ajax({
        url: '/Company/GetCompnaytById?CompanyId=' + CompanyId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response) {
                $('#textCompanyGstNo').val(response.gstno);
                $('#textCompanyBillingAddress').val(response.fullAddress);
            } else {
                siteloaderhide();
                toastr.error('Empty response received.');
            }
        },
    });
}

function clearItemErrorMessages() {
    $("#spnitembutton").text("");
}

function addShippingAddress() {
    siteloadershow();
    if ($("#shippingAddressForm").valid()) {
        var address = $("#textmdAddress").val();
        var sitename = $("#textInvoiceSiteName").val();

        if ($('#dvShippingAddress .ac-invoice-shippingadd').length > 0) {
            siteloaderhide();
            Swal.fire({
                title: "Only one address allowed!",
                text: "You can only add one shipping address.",
                icon: "warning",
                confirmButtonColor: "#3085d6",
                confirmButtonText: "OK"
            });
            return;
        }

        var newRow = '<div class="row ac-invoice-shippingadd ShippingAddress">' +
            '<div class="col-2 col-sm-2">' +
            '<label id="lblshprownum1">1</label>' +
            '</div>' +
            '<div class="col-5 col-sm-5">' +
            '<p class="shippingaddress">' + address + '</p>' +
            '</div>' +
            '</div>';

        $('#dvShippingAddress').append(newRow);
        updateProductTotalAmount();
        updateTotals();
        updateRowNumbers();
        $('#mdShippingAdd').modal('toggle');
        siteloaderhide();
    } else {
        siteloaderhide();

        Swal.fire({
            title: "Kindly fill all data fields",
            icon: 'warning',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'OK',
        });
    }
}


