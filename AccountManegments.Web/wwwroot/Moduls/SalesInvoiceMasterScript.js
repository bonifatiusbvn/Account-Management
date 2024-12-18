GetSalesSupplierDetail();
GetSalesCompanyDetail();
GetSalesItemDetailsList();
function GetSalesSupplierDetail() {

    $.ajax({
        url: '/Supplier/GetSupplierNameList',
        success: function (result) {
            var selectedValue = $('#textSalesSupplierName').find('option:first').val();

            $.each(result, function (i, data) {

                if (data.supplierId !== selectedValue) {
                    $('#textSalesSupplierName').append('<Option value=' + data.supplierId + '>' + data.supplierName + '</Option>')
                }
            });
        }
    });
}
function GetSalesCompanyDetail() {

    $.ajax({
        url: '/Company/GetCompanyNameList',
        success: function (result) {
            if (result.length > 0) {

                $.each(result, function (i, data) {
                    $('#textSalesCompanyName').append('<option value="' + data.companyId + '">' + data.companyName + '</option>');
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching company details:', error);
        }
    });
}
$(document).ready(function () {
    $('#textSalesCompanyName').change(function () {

        getSalesCompanyDetail($(this).val());
        getSalesInvoiceNumber($(this).val());
    });

    var InvoiceCompanyId = $('#textSalesCompanyName').val();
    var InvoiceGUID = $('#textSalesSupplierInvoiceId').val();
    if (InvoiceCompanyId != null && InvoiceGUID == "00000000-0000-0000-0000-000000000000") {
        var CompanyId = $('#textSalesCompanyName').val();
        getSalesInvoiceNumber(CompanyId);
    }
    $('#textSalesSupplierName').change(function () {
        getSalesSupplierDetail($(this).val());
    });
});
function getSalesCompanyDetail(CompanyId) {
    siteloadershow();
    $.ajax({
        url: '/Company/GetCompnaytById?CompanyId=' + CompanyId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response) {
                $('#textSalesCompanyGstNo').val(response.gstno);
                $('#textSalesCompanyBillingAddress').val(response.fullAddress);
            } else {
                siteloaderhide();
                toastr.error('Empty response received.');
            }
        },
    });
}
function getSalesInvoiceNumber(CompanyId) {

    siteloadershow();
    $.ajax({
        url: '/Sales/CheckSalesInvoiceNo?CompanyId=' + CompanyId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            siteloaderhide();
            if (response.code == 200) {

                siteloaderhide();
                $('#textSalesInvoicePrefix').val(response.data);
            } else {
                siteloaderhide();
                toastr.error('Empty response received.');
            }
        },
    });
}
function getSalesSupplierDetail(SupplierId) {
    siteloadershow();
    $.ajax({
        url: '/Supplier/DisplaySupplier?SupplierId=' + SupplierId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response) {
                $('#textSalesSupplierMobile').val(response.mobile);
                $('#textSalesSupplierGST').val(response.gstno);
                $('#textSalesSupplierAddress').val(response.fullAddress);
            } else {
                siteloaderhide();
                toastr.error('Empty response received.');
            }
        },
    });
}
function SalesUnitTypeDropdown(itemId) {

    if ($('#txtSalesPOUnitType_' + itemId + ' option').length > 1) {
        return
    }

    $.ajax({
        url: '/ItemMaster/GetAllUnitType',
        success: function (result) {
            $('#txtSalesPOUnitType_' + itemId).empty();

            $.each(result, function (i, data) {
                $('#txtSalesPOUnitType_' + itemId).append('<option value=' + data.unitId + '>' + data.unitName + '</option>');

            });

            $('#txtSalesPOUnitType_' + itemId).val($("#txtSalesunittype_" + itemId).val())


        }
    });

}
var count = 0;
function AddNewSalesRow(Result) {
    var newProductRow = $(Result);
    var itemId = newProductRow.data('product-id');
    SalesUnitTypeDropdown(itemId);

    count++;
    $("#addnewSalesproductlink").append(Result);
    updateSalesTotals();
    updateSalesRowNumbers();
}
function updateSalesRowNumbers() {
    $(".product-id").each(function (index) {
        $(this).text(index + 1);
    });
}
function preventEmptyValue(input) {
    if (input.value === "") {
        input.value = 1;
    }
}

function updateSalesProductTotalAmount(that) {
    var row = $(that);
    var productPrice = parseFloat(row.find("#txtSalesproductamount").val());
    var hiddenproductPrice = parseFloat(row.find("#Salesproductamount").val());
    var quantity = parseFloat(row.find("#txtSalesproductquantity").val());
    var discountprice = parseFloat(row.find("#txtSalesdiscountamount").val());
    var AmtWithDisc = hiddenproductPrice - discountprice;

    var gst = parseFloat(row.find("#txtSalesgst").val());

    var totalGst = (AmtWithDisc * quantity * gst) / 100;

    var TotalAmountAfterDiscount = AmtWithDisc * quantity + totalGst;

    row.find("#txtSalesgstAmount").val(totalGst.toFixed(2));
    row.find("#txtSalesproducttotalamount").val(TotalAmountAfterDiscount.toFixed(2));
}

function updateSalesDiscount(that) {
    var row = $(that);
    var productPrice = parseFloat(row.find("#Salesproductamount").val());
    var quantity = parseFloat(row.find("#txtSalesproductquantity").val());
    var discountprice = parseFloat(row.find("#txtSalesdiscountamount").val());
    var discountPercentage = parseFloat(row.find("#txtSalesdiscountpercentage").val());

    if (isNaN(discountprice)) {
        row.find("#txtSalesdiscountamount").val(0);
        row.find("#txtSalesdiscountpercentage").val(0);
        row.find("#txtSalesproductamount").val(productPrice.toFixed(2));
        updateSalesProductTotalAmount(row);
        updateSalesTotals();
        return;
    }

    if (discountPercentage == 0 && discountprice > 0) {
        var discountperbyamount = discountprice / productPrice * 100;
        row.find("#txtSalesdiscountpercentage").val(discountperbyamount.toFixed(2));
    } else if (discountprice > 0 && discountPercentage > 0) {
        var discountperbyamount = discountprice / productPrice * 100;
        row.find("#txtSalesdiscountpercentage").val(discountperbyamount.toFixed(2));
    }
    var AmountAfterDisc = productPrice - discountprice;
    row.find("#txtSalesproductamount").val(AmountAfterDisc.toFixed(2));
    updateSalesProductTotalAmount(row);
    updateSalesTotals();
}

function UpdateSalesDiscountPercentage(that) {
    var row = $(that);
    var productPrice = parseFloat(row.find("#Salesproductamount").val());
    var quantity = parseFloat(row.find("#txtSalesproductquantity").val());
    var discountprice = parseFloat(row.find("#txtSalesdiscountamount").val());
    var discountPercentage = parseFloat(row.find("#txtSalesdiscountpercentage").val());

    if (isNaN(discountPercentage)) {
        row.find("#txtSalesdiscountamount").val(0);
        row.find("#txtSalesdiscountpercentage").val(0);
        row.find("#txtSalesproductamount").val(productPrice.toFixed(2));
        updateSalesProductTotalAmount(row);
        updateSalesTotals();
        return;
    }

    if (discountprice == 0 && discountPercentage > 0) {
        discountprice = productPrice * discountPercentage / 100;
        row.find("#txtSalesdiscountamount").val(discountprice.toFixed(2));
    } else if (discountprice > 0 && discountPercentage > 0) {
        discountprice = productPrice * discountPercentage / 100;
        row.find("#txtSalesdiscountamount").val(discountprice.toFixed(2));
    }

    var AmountAfterDisc = productPrice - discountprice;
    row.find("#txtSalesproductamount").val(AmountAfterDisc.toFixed(2));
    updateSalesProductTotalAmount(row);
    updateSalesTotals();
}

function updateSalesProductQuantity(row, increment) {
    var quantityInput = parseFloat(row.find(".product-quantity").val());
    var newQuantity = quantityInput + increment;
    if (newQuantity >= 0) {
        row.find(".product-quantity").val(newQuantity);
        updateSalesProductTotalAmount(row);
        updateSalesTotals();
    }
}
function updateSalesTotals() {
    var totalSubtotal = 0;
    var totalGst = 0;
    var totalAmount = 0;
    var TotalItemQuantity = 0;
    var TotalDiscount = 0;
    $(".product").each(function () {
        var row = $(this);
        var subtotal = parseFloat(row.find("#txtSalesproductamount").val()) || 0;
        var gst = parseFloat(row.find("#txtSalesgstAmount").val()) || 0;
        var totalquantity = parseFloat(row.find("#txtSalesproductquantity").val()) || 0;
        var discountprice = parseFloat(row.find("#txtSalesdiscountamount").val()) || 0;

        totalSubtotal += subtotal * totalquantity;
        totalGst += gst;
        TotalItemQuantity += totalquantity;
        TotalDiscount += discountprice * totalquantity;
    });
    var Tds = $('#Sales-cart-tds').val();
    totalAmount = totalSubtotal + totalGst - Tds;
    var dicountRoundOff = parseFloat($('#SalesIDiscountRoundOff').val()) || 0;
    totalAmount += dicountRoundOff;

    $("#Sales-cart-subtotal").val(totalSubtotal.toFixed(2));
    $("#Salestotalgst").val(totalGst.toFixed(2));
    $("#Sales-cart-discount").val(TotalDiscount.toFixed(2));
    $("#SalesTotalDiscountPrice").html(TotalDiscount.toFixed(2));
    $("#SalesTotalProductQuantity").text(TotalItemQuantity);
    $("#SalesTotalProductPrice").html(totalSubtotal.toFixed(2));
    $("#SalesTotalProductGST").html(totalGst.toFixed(2));

    var decimalPart = totalAmount - Math.floor(totalAmount);

    if (decimalPart <= 0.50) {
        totalAmount = Math.floor(totalAmount);
    } else {
        totalAmount = Math.ceil(totalAmount);
    }

    $("#Sales-cart-total").val(totalAmount.toFixed(2));
    $("#SalesTotalProductAmount").html(totalAmount.toFixed(2));
}
function removeSalesItem(btn) {
    $(btn).closest("tr").remove();
    updateSalesRowNumbers();
    updateSalesTotals();
}
function fn_OpenAddSalesproductmodal() {

    $('#mdSalesProductSearch').val('');
    $('#mdSalesproductModal').modal('show');
}
function GetSalesItemDetailsList() {

    var searchText = $('#mdSalesProductSearch').val();

    $.get("/Sales/GetAllSalesItemDetailList", { searchText: searchText })
        .done(function (result) {
            $("#mdSaleslistofItem").html(result);
        })
}
function clearSalesItemErrorMessage() {
    $("#spnSalesitembutton").text("");
}
function SerchSalesItemDetailsById(Id, inputField) {
    clearSalesItemErrorMessage();
    siteloadershow();
    var qty = $(inputField).closest('.ac-item').find('.Sales-product-quantity').val();
    var Item = {
        ItemId: Id,
        Quantity: qty,
    }

    var form_data = new FormData();
    form_data.append("ITEMID", JSON.stringify(Item));


    $.ajax({
        url: '/Sales/DisplayItemListInSalesInvoice',
        type: 'Post',
        datatype: 'json',
        data: form_data,
        processData: false,
        contentType: false,
        complete: function (Result) {

            siteloaderhide();
            if (Result.statusText === "success") {
                AddNewSalesRow(Result.responseText);
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

$(document).ready(function () {

    $("#CreateSalesInvoiceForm").validate({
        rules: {
            textSalesSupplierName: "required",
            textSalesCompanyName: "required",
            SalespaymentStatus: "required",
            textSalesSupplierMobile: {
                digits: true,
                minlength: 10,
                maxlength: 15
            },

            textSalesSupplierAddress: "required",
        },
        messages: {
            textSalesSupplierName: "Select Supplier Name",
            textSalesCompanyName: "Select Company Name",
            SalespaymentStatus: "Select Payment Status",
            textSalesSupplierMobile: {
                digits: "Please enter a valid 10-digit phone number",
                minlength: "Phone number must be 10 digits long",
                maxlength: "Phone number must be 10 digits long"
            },
            textSalesSupplierAddress: "Enter supplier address",
        }
    });
    function handleFocus(event, selector) {
        if (event.keyCode == 13 || event.keyCode == 9) {
            event.preventDefault();
            $(selector).focus();
        }
    }

    $(document).on('input', '#txtSalesproductquantity', function () {
        var productRow = $(this).closest(".product");
        updateSalesProductTotalAmount(productRow);
        updateSalesTotals();
    }).on('keydown', '#txtSalesproductquantity', function (event) {
        var productRow = $(this).closest(".product");
        if (event.key === 'Tab' && event.shiftKey) {
            event.preventDefault();
            productRow.find('#txtSalesHSNcode').focus();
        } else if (event.key === 'Tab') {
            var productFocus = productRow.find('#txtSalesproductamount');
            handleFocus(event, productFocus);
        }
    });

    $(document).on('input', '#txtSalesgst', function () {
        var productRow = $(this).closest(".product");
        var gstvalue = $('#txtSalesgst').val();
        if (gstvalue > 100) {
            toastr.warning("GST% cannot be greater than 100%");
            $(this).val(100);
        }
        updateSalesProductTotalAmount(productRow);
        updateSalesTotals();
    })

    function debounce(func, delay) {
        let timer;
        return function (...args) {
            clearTimeout(timer);
            timer = setTimeout(() => func.apply(this, args), delay);
        };
    }

    $(document).on('input', '#txtSalesdiscountpercentage', debounce(function () {
        var value = $(this).val();
        var productRow = $(this).closest(".product");

        if (value > 100) {
            toastr.warning("Discount cannot be greater than 100%");
            productRow.find("#txtSalesdiscountpercentage").val(0);
            productRow.find("#txtSalesdiscountamount").val(0);
        } else if (value <= 0 || value == "") {
            productRow.find("#txtSalesdiscountamount").val(0);
            productRow.find("#txtSalesdiscountpercentage").val(0);
            updateSalesProductTotalAmount(productRow);
        } else {
            UpdateSalesDiscountPercentage(productRow);
        }
    }, 300)).on('keydown', '#txtSalesdiscountpercentage', function (event) {
        var productRow = $(this).closest(".product");
        if (event.key === 'Tab' && event.shiftKey) {
            event.preventDefault();
            productRow.find('#txtSalesdiscountamount').focus();
        } else if (event.key === 'Tab') {
            var gstFocus = productRow.find('#txtSalesgst');
            handleFocus(event, gstFocus);
        }
    });


    $(document).on('input', '#txtSalesdiscountamount', debounce(function () {
        var productRow = $(this).closest(".product");
        var discountAmount = parseFloat($(this).val());
        var productAmount = parseFloat($(productRow).find("#Salesproductamount").val());

        if (discountAmount > productAmount) {
            toastr.warning("Amount cannot be greater than Item price");
            productRow.find("#txtSalesdiscountamount").val(0);
            productRow.find("#txtSalesdiscountpercentage").val(0);
        } else if (discountAmount <= 0 || discountAmount == "") {
            productRow.find("#txtSalesdiscountamount").val(0);
            productRow.find("#txtSalesdiscountpercentage").val(0);
            updateSalesProductTotalAmount(productRow);
        } else {
            updateSalesDiscount(productRow);
        }
    }, 300)).on('keydown', '#txtSalesdiscountamount', function (event) {
        var productRow = $(this).closest(".product");
        if (event.key === 'Tab' && event.shiftKey) {
            event.preventDefault();
            productRow.find('#txtSalesproductamount').focus();
        } else if (event.key === 'Tab') {
            var discountPercentagefocus = productRow.find('#txtSalesdiscountpercentage');
            handleFocus(event, discountPercentagefocus);
        }
    });

    $(document).on('input', '#txtSalesproductamount', function () {
        var productRow = $(this).closest(".product");
        var productAmount = parseFloat($(this).val());
        var discountAmountfocus = productRow.find('#txtSalesdiscountamount');

        if (!isNaN(productAmount)) {
            productRow.find("#txtSalesdiscountamount").val(0);
            productRow.find("#txtSalesdiscountpercentage").val(0);
        }

        productRow.find("#Salesproductamount").val(productAmount.toFixed(2));
        updateSalesProductTotalAmount(productRow);
        updateSalesTotals();

    }).on('keydown', '#txtSalesproductamount', function (event) {
        var productRow = $(this).closest(".product");
        if (event.key === 'Tab' && event.shiftKey) {
            event.preventDefault();
            productRow.find('#txtSalesproductquantity').focus();
        } else if (event.key === 'Tab') {
            var discountAmountfocus = productRow.find('#txtSalesdiscountamount');
            handleFocus(event, discountAmountfocus);
        }
    });


    $(document).on('input', '#Sales-cart-roundOff', debounce(function () {
        var roundoff = $('#Sales-cart-roundOff').val();
        if (isNaN(roundoff) || (roundoff < -0.99 || roundoff > 0.99)) {
            toastr.warning("Value must be between -0.99 and 0.99");
        }
        else {
            updateSalesTotals();
        }
    }, 300));
    $(document).on('input', '#SalesIDiscountRoundOff', debounce(function () {

        var Discountroundoff = $('#SalesIDiscountRoundOff').val();
        updateSalesTotals();
    }, 300));

    $(document).on('input', '#Sales-cart-tds', debounce(function () {
        var tds = parseFloat($('#Sales-cart-tds').val());
        var TotalAmount = parseFloat($('#Sales-cart-total').val());

        if (tds > TotalAmount) {
            toastr.warning("Value cannot be greater than subtotal.");
        } else {
            updateSalesTotals();
        }
    }, 300));

    $('input[name="selectedAddress"]').change(function () {
        var selectedAddress = $(this).data('address');
        $('#selectedSalesShippingAddress').val(selectedAddress);
    });
});

function InsertMultipleSupplierItem() {
    siteloadershow();
    if ($("#CreateInvoiceForm").valid()) {
        var selectedGroupAddress = $('input[name="selectedGroupAddress"]:checked').val();
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

            var modelSiteId = $("#txtModelSiteId").val();
            var sessionSiteId = $("#txtsessionSiteName").val();

            if (!modelSiteId || modelSiteId.trim() === "") {
                $("#txtsessionSiteName").val(sessionSiteId);
            } else {
                $("#txtsessionSiteName").val(modelSiteId);
            }

            var InvoiceDetails = {
                SiteId: $("#txtsessionSiteName").val(),
                InvoiceNo: $("#textInvoicePrefix").val(),
                InvoiceType: $('#ddlinvoicetype').val(),
                Date: $("#textOrderDate").val(),
                SupplierId: $("#textSupplierName").val(),
                CompanyId: $("#textCompanyName").val(),
                TotalAmountInvoice: $("#cart-total").val(),
                TotalGstamount: $("#totalgst").val(),
                PaymentStatus: $("input[name='paymentStatus']:checked").val(),
                Description: $("#textDescription").val(),
                CreatedBy: $("#createdbyid").val(),
                UnitTypeId: $("#UnitTypeId").val(),
                ShippingAddress: $("input[name='selectedAddress']:checked").data('address'),
                ChallanNo: $("#txtchalanNo").val(),
                Lrno: $("#textPONoListHidden").val() ? $("#textPONoListHidden").val() : $("#txtlrNo").val(),
                VehicleNo: $("#txtvehicleno").val(),
                DispatchBy: $("#txtdispatch").val(),
                PaymentTerms: $("#txtpayment").val(),
                SupplierInvoiceNo: $("#textSupplierInvoiceNo").val(),
                Tds: $('#cart-tds').val(),
                TotalDiscount: $('#cart-discount').val(),
                DiscountRoundoff: $("#IDiscountRoundOff").val(),
                SiteGroup: $("#InvoiceGroupList").val(),
                GroupAddress: $('input[name="selectedGroupAddress"]:checked').val(),
                Poid: $("#txtInvoicePOID").val() ? $("#txtInvoicePOID").val() : InvoicePONo,
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

SalesInvoicesortTable();
function companyfilterSalesInvoice() {
    siteloadershow();
    var searchText = $('#ddlInvoiceCompanyName').val();
    var searchBy = "AscendingCompanyName";

    $.ajax({
        url: '/Sales/SalesInvoiceListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#SalesInvoicebody").html(result);
        },

    });
}


let currentSortOrder = '';

function sortTable(field) {
    if (currentSortOrder === 'Ascending' + field) {
        currentSortOrder = 'Descending' + field;
    } else {
        currentSortOrder = 'Ascending' + field;
    }

    siteloadershow();

    $.ajax({
        url: '/Sales/SalesInvoiceListAction',
        type: 'GET',
        data: {
            sortBy: currentSortOrder,
        },
        success: function (result) {
            siteloaderhide();
            $("#SalesInvoicebody").html(result);
        },
        error: function (xhr, status, error) {
            console.error("Error fetching sorted data:", error);
            siteloaderhide();
        }
    });
}

function filterSalesInvoiceTable() {

    siteloadershow();
    var searchText = $('#txtSupplierInvoiceSearch').val();
    var searchBy = $('#SupplierInvoiceSearchBy').val();

    $.ajax({
        url: '/Sales/SalesInvoiceListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#SalesInvoicebody").html(result);
        },

    });
}

function SalesInvoicesortTable() {

    siteloadershow();
    var sortBy = $('#SortBySupplierInvoice').val();

    $.ajax({
        url: '/Sales/SalesInvoiceListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#SalesInvoicebody").html(result);
        },

    });
}