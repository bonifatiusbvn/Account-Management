AllSupplierInvoiceListTable()
InvoiceListTable();
GetItemDetailsList()
GetSiteDetail();
GetCompanyDetail();
GetSupplierDetail();
GetGroupList();
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

            var selectedValue = $('#textCompanyName').find('option:first').val();

            $.each(result, function (i, data) {
                if (data.companyId !== selectedValue) {
                    $('#textCompanyName').append('<option value="' + data.companyId + '">' + data.companyName + '</option>');
                }
            });

            $.each(result, function (i, data) {
                $('#ddlInvoiceCompanyName').append('<option value="' + data.companyName + '">' + data.companyName + '</option>');
            });
        },
        error: function (xhr, status, error) {
            console.error('Error fetching company details:', error);
        }
    });
}


$(document).ready(function () {
    $('#textCompanyName').change(function () {

        getCompanyDetail($(this).val());
        getInvoiceNumber($(this).val());
    });
});


function companyfilterSupplierInvoice() {
    siteloadershow();
    var searchText = $('#ddlInvoiceCompanyName').val();
    var searchBy = "AscendingCompanyName";

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

    });
}

function GetSupplierDetail() {

    $.ajax({
        url: '/Supplier/GetSupplierNameList',
        success: function (result) {
            var selectedValue = $('#textSupplierName').find('option:first').val();

            $.each(result, function (i, data) {

                if (data.supplierId !== selectedValue) {
                    $('#textSupplierName').append('<Option value=' + data.supplierId + '>' + data.supplierName + '</Option>')
                }
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
        $(document).on('input', '#IDiscountRoundOff', debounce(function () {
            var Discountroundoff = $('#IDiscountRoundOff').val();
            if (isNaN(Discountroundoff) || (Discountroundoff < -0.99 || Discountroundoff > 0.99)) {
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
        .fail(function (xhr, status, error) {

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
                    toastr.error("Can't delete Invoice!");
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
            textmdAddress: "required",
        },
        messages: {
            textmdAddress: "Select shipping address",
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
                    ItemDescription: orderRow.find("#txtInvoiceProductDes").val(),
                    UnitType: orderRow.find("#txtPOUnitType_" + orderRow.find("#txtItemId").val()).val(),
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

            var shippingAddress = $("#shippingaddress").text() != "" ? $("#shippingaddress").text() : $("#textmdAddress").val();

            var InvoiceDetails = {
                SiteId: $("#txtsessionSiteName").val(),
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
                ShippingAddress: shippingAddress,
                ChallanNo: $("#txtchalanNo").val(),
                Lrno: $("#txtlrNo").val(),
                VehicleNo: $("#txtvehicleno").val(),
                DispatchBy: $("#txtdispatch").val(),
                PaymentTerms: $("#txtpayment").val(),
                SupplierInvoiceNo: $("#textSupplierInvoiceNo").val(),
                Roundoff: $('#cart-roundOff').val(),
                TotalDiscount: $('#cart-discount').val(),
                DiscountRoundoff: $("#IDiscountRoundOff").val(),
                SiteGroup: $("#InvoiceGroupList").val(),
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
                    ItemDescription: orderRow.find("#txtInvoiceProductDes").val(),
                    UnitType: orderRow.find("#txtPOUnitType_" + orderRow.find("#txtItemId").val()).val(),
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
            var shippingAdd = $("#textmdAddress").val();
            var Address = null;
            if (shippingAdd != "") {
                Address = shippingAdd;
            } else {
                Address = $(".ShippingAddress").find("#shippingaddress").text().trim();
            }

            var InvoiceDetails = {
                Id: $('#textSupplierInvoiceId').val(),
                SiteId: $("#txtModelSiteId").val(),
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
                Terms: $("#textInvoiceTerms").val(),
                SupplierInvoiceNo: $("#textSupplierInvoiceNo").val(),
                Roundoff: $('#cart-roundOff').val(),
                TotalDiscount: $('#cart-discount').val(),
                CreatedOn: $('#textCreatedOn').val(),
                ItemList: ItemDetails,
                ShippingAddress: Address,
                ChallanNo: $("#txtchalanNo").val(),
                Lrno: $("#txtlrNo").val(),
                VehicleNo: $("#txtvehicleno").val(),
                DispatchBy: $("#txtdispatch").val(),
                PaymentTerms: $("#txtpayment").val(),
                SiteGroup: $("#InvoiceGroupList").val(),
                DiscountRoundoff: $("#IDiscountRoundOff").val(),
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

    if ($('#txtPOUnitType_' + itemId + ' option').length > 1) {
        return
    }

    $.ajax({
        url: '/ItemMaster/GetAllUnitType',
        success: function (result) {
            $('#txtPOUnitType_' + itemId).empty();

            $.each(result, function (i, data) {
                $('#txtPOUnitType_' + itemId).append('<option value=' + data.unitId + '>' + data.unitName + '</option>');

            });

            $('#txtPOUnitType_' + itemId).val($("#txtunittype_" + itemId).val())


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

//document.querySelector("#profile-img-file-input").addEventListener("change", function () {
//    var e = document.querySelector(".user-profile-image"),
//        t = document.querySelector(".profile-img-file-input").files[0],
//        n = new FileReader;
//    n.addEventListener("load", function () {
//        e.src = n.result
//    }, !1), t && n.readAsDataURL(t)
//}), isData();


var count = 0;
function AddNewRow(Result) {
    var newProductRow = $(Result);
    var itemId = newProductRow.data('product-id');
    UnitTypeDropdown(itemId);


    count++;
    $("#addnewproductlink").append(Result);
    updateTotals();
    updateRowNumbers();
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
        row.find("#txtproductamount").val(productPrice.toFixed(2));
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
    var AmountAfterDisc = productPrice - discountprice;
    row.find("#txtproductamount").val(AmountAfterDisc.toFixed(2));
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
        row.find("#txtproductamount").val(productPrice.toFixed(2));
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

    var AmountAfterDisc = productPrice - discountprice;
    row.find("#txtproductamount").val(AmountAfterDisc.toFixed(2));
    updateProductTotalAmount(row);
    updateTotals();
}

function updateProductQuantity(row, increment) {
    var quantityInput = parseFloat(row.find(".product-quantity").val());
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

    $(".product").each(function () {
        var row = $(this);
        var subtotal = parseFloat(row.find("#txtproductamount").val());
        var gst = parseFloat(row.find("#txtgstAmount").val());
        var totalquantity = parseFloat(row.find("#txtproductquantity").val());
        var discountprice = parseFloat(row.find("#txtdiscountamount").val());

        totalSubtotal += subtotal * totalquantity;
        totalGst += gst;
        TotalItemQuantity += totalquantity;
        TotalDiscount += discountprice * totalquantity;
    });

    totalAmount = totalSubtotal + totalGst;

    $("#cart-subtotal").val(totalSubtotal.toFixed(2));
    $("#totalgst").val(totalGst.toFixed(2));
    $("#cart-discount").val(TotalDiscount.toFixed(2));
    $("#TotalDiscountPrice").html(TotalDiscount.toFixed(2));
    $("#TotalProductQuantity").text(TotalItemQuantity);
    $("#TotalProductPrice").html(totalSubtotal.toFixed(2));
    $("#TotalProductGST").html(totalGst.toFixed(2));


    var decimalPart = totalAmount - Math.floor(totalAmount);

    if (decimalPart <= 0.50) {
        totalAmount = Math.floor(totalAmount);
    } else {
        totalAmount = Math.ceil(totalAmount);
    }

    $("#TotalProductAmount").html(totalAmount.toFixed(2));
    $("#cart-total").val(totalAmount.toFixed(2));
}
function removeItem(btn) {
    $(btn).closest("tr").remove();
    updateRowNumbers();
    updateTotals();
}

function updateQuantity(e, t, n) {
    e = (e = e * t).toFixed(2);
    n.value = paymentSign + e, recalculateCart()
}


let viewobj;
var value, invoices_list = localStorage.getItem("invoices-list"),
    options = localStorage.getItem("option"),
    invoice_no = localStorage.getItem("invoice_no"),
    invoices = JSON.parse(invoices_list);


function InvoiceListTable() {

    var searchText = $('#txtInvoiceSearch').val();
    var searchBy = $('#InvoiceSearchBy').val();

    $.get("/InvoiceMaster/InvoiceListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {
            $("#Invoicetbody").html(result);
        })

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

$(document).ready(function () {
    var invoiceRoleUser = $('#UserRoleinInvoice').val();
    var invoiceSiteId = $('#textInvoiceSiteName').val();

    if (invoiceRoleUser == 8) {
        fn_GetInvoiceSiteAddressList(invoiceSiteId);
    }
    else {
        $('#textInvoiceSiteName').change(function () {
            var Site = $(this).val();
            fn_GetInvoiceSiteAddressList(Site);
        });
    }
    $('#drpInvoiceSiteAddress').select2({
        theme: 'bootstrap4',
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        placeholder: $(this).data('placeholder'),
        allowClear: Boolean($(this).data('allow-clear')),
        dropdownParent: $("#mdShippingAdd")
    });

    $(document).on('click', '#removeAddress', function () {
        $(this).closest('tr').remove();
        $('.add-addresses').prop('disabled', false);
    });
});

function fn_GetInvoiceSiteAddressList(SiteId) {
    $.ajax({
        url: '/SiteMaster/DisplaySiteAddressList?SiteId=' + SiteId,
        success: function (result) {

            $('#drpInvoiceSiteAddress').empty();
            $('#textmdAddress').val('');
            $('#drpInvoiceSiteAddress').append('<option value="">-- Select site address --</option>');
            if (Array.isArray(result)) {
                $.each(result, function (i, data) {

                    $('#drpInvoiceSiteAddress').append('<option value="' + data.address + ', Code : ' + data.stateCode + '">' + data.address + ', Code :' + data.stateCode + '</option>');
                });
            } else {

                $('#textmdAddress').val(result.shippingAddress + ' , ' + result.shippingArea + ', ' + result.shippingCityName + ', ' + result.shippingStateName + ', ' + result.shippingCountryName + ',Code : ' + result.stateCode);
            }
        }
    });
}
$('#drpInvoiceSiteAddress').on('change', function () {
    var selectedInvoiceAddress = $(this).val();
    $('#textmdAddress').val(selectedInvoiceAddress);
});
function addShippingAddress() {
    siteloadershow();
    if ($("#shippingAddressForm").valid()) {
        var address = $("#textmdAddress").val();

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
            '<div class="col-1 col-sm-1">' +
            '<label id="lblshprownum1">1</label>' +
            '</div>' +
            '<div class="col-5 col-sm-5 text-center">' +
            '<p id="addShippingAddress" class="shippingaddress">' + address + '</p>' +
            '</div>' +
            '<div class="col-2 col-sm-2">' +
            '<a id="remove" class="btn text-primary text-center" onclick="fn_removeShippingAdd(this)"><i class="lni lni-trash"></i></a>' +
            '</div>' +
            '</div>';

        $('#dvShippingAddress').append(newRow);
        siteloaderhide();
    } else {
        siteloaderhide();
        toastr.warning('please select address!');
    }
}

function fn_OpenAddproductmodal() {
    if ($("#drpSiteName").val() == "") {
        $("#frmdrpdashbord").css("border", "2px solid red");
        $("#siteErrorMesssage").html("Select Site").css("color", "red");
    }
    else {
        $('#mdProductSearch').val('');
        $('#mdPoproductModal').modal('show');
    }
}

function printinvoice() {

    var divToPrint = document.getElementById('displayInvoiceDetail');

    var newWin = window.open('', 'Print-Window');

    newWin.document.open();

    newWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');
}

function ClearAddProductTextBox() {
    ClearProductTextBox();
    $('#addnewItemModal').modal('show');
}

$(document).ready(function () {
    function GetUnitType() {
        $.ajax({
            url: '/ItemMaster/GetAllUnitType',
            method: 'GET',
            success: function (result) {
                var unitTypes = result.map(function (data) {
                    return {
                        label: data.unitName,
                        value: data.unitId
                    };
                });


                $("#textUnitType").autocomplete({
                    source: unitTypes,
                    minLength: 0,
                    select: function (event, ui) {

                        event.preventDefault();
                        $("#textUnitType").val(ui.item.label);
                        $("#textUnitTypeHidden").val(ui.item.value);

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

    GetUnitType();
    function GetAllItemName() {
        $.ajax({
            url: '/ItemMaster/GetItemNameList',
            method: 'GET',
            success: function (result) {
                var ItemName = result.map(function (data) {
                    return {
                        label: data.itemName,
                    };
                });

                $("#textItemName").autocomplete({
                    source: ItemName,
                    minLength: 0,
                    select: function (event, ui) {
                        event.preventDefault();
                        $("#textItemName").val(ui.item.label);
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
    GetAllItemName();
});


function CreateNewItem() {
    siteloadershow();
    if ($("#addnewItemForm").valid()) {
        var objData = {
            ItemName: $('#textItemName').val(),
            UnitType: $('#textUnitTypeHidden').val(),
            PricePerUnit: $('#textPricePerUnit').val(),
            IsWithGst: $('#textIsWithGst').prop('checked'),
            Gstamount: $('#textGstAmount').val(),
            Gstper: $('#textGstPerUnit').val(),
            Hsncode: $('#textHSNCode').val(),
        };

        $.ajax({
            url: '/ItemMaster/CreateItem',
            type: 'POST',
            data: objData,
            dataType: 'json',
            success: function (result) {
                if (result.code == 200) {
                    toastr.success(result.message);
                    filterallItemTable();
                    fn_OpenAddproductmodal();
                    $('#addnewItemModal').modal('hide');
                } else {
                    toastr.warning(result.message);
                }
                siteloaderhide();
            },
            error: function (xhr, status, error) {

                siteloaderhide();
                toastr.error('An error occurred while processing your request.');
            }
        });
    } else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}

function ClearProductTextBox() {
    resetNewItemForm();
    $('#textItemName').val('');
    $('#textUnitType').val('');
    $('#textPricePerUnit').val('');
    $('#textPriceWithGst').val('');
    $('#textGstAmount').val('');
    $('#textGstPerUnit').val('');
    $('#textHSNCode').val('');
    $('#textItemid').val('');
    $("#textIsWithGst").prop("checked", false);
}


var AddNewItemForm;
$(document).ready(function () {

    AddNewItemForm = $("#addnewItemForm").validate({
        rules: {
            textItemName: "required",
            textUnitType: "required",
            textPricePerUnit: "required",
            textGstAmount: "required",
            textGstPerUnit: "required",
        },
        messages: {
            textItemName: "Please Enter ItemName",
            textUnitType: "Please Enter UnitType",
            textPricePerUnit: "Please Enter PricePerUnit",
            textGstAmount: "Please Enter GstAmount",
            textGstPerUnit: "Please Enter GstPerUnit",
        }
    })
});

function resetNewItemForm() {
    if (AddNewItemForm) {
        AddNewItemForm.resetForm();
    }
}

function clearItemtListSearchText() {
    $('#mdProductSearch').val('');
    filterallItemTable();
}

function WithGSTSelected() {
    var isWithGstCheckbox = document.getElementById('textIsWithGst');
    var gstAmountInput = document.getElementById('textGstAmount');
    var gstPercentageInput = document.getElementById('textGstPerUnit');
    var priceInput = document.getElementById('textPricePerUnit');

    var price = parseFloat(priceInput.value);
    var gstPercentage = parseFloat(gstPercentageInput.value);

    if (isWithGstCheckbox.checked) {

        if (!isNaN(price) && !isNaN(gstPercentage)) {
            var totalAmount = 100 + gstPercentage;
            var baseAmount = price - (price * gstPercentage / totalAmount);
            var gstAmount = price - baseAmount;
            gstAmountInput.value = gstAmount.toFixed(2);
            priceInput.value = baseAmount.toFixed(2);
        } else {
            gstAmountInput.value = "";
        }
    } else {

        if (!isNaN(price) && !isNaN(gstPercentage)) {
            var Amount = (gstPercentage / 100) * price;
            gstAmountInput.value = Amount.toFixed(2);
        } else {
            gstAmountInput.value = "";
        }
    }
}

document.getElementById('textGstPerUnit').addEventListener('input', function () {
    WithGSTSelected();
});
document.getElementById('textPricePerUnit').addEventListener('input', function () {
    WithGSTSelected();
});
document.getElementById('textIsWithGst').addEventListener('change', function () {
    WithGSTSelected();
});

function fn_AddInvoiceProductDescription(element) {
    var itemId = $(element).data('item-id');

    var $productDesBtn = $(`div[data-item-id='${itemId}']#ProductDesBtn`);
    var $productDesText = $(`div[data-item-id='${itemId}']#ProductDesText`);

    if ($productDesText.is(':visible')) {
        $productDesText.find('input').val('');
    }

    $productDesBtn.toggle();
    $productDesText.toggle();
}
function GetGroupList() {

    $.ajax({
        url: '/SiteMaster/GetGroupNameListBySiteId',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#InvoiceGroupList').append('<Option value=' + data.groupName + '>' + data.groupName + '</Option>')
            });
        }
    });
}
