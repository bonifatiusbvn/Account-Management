var _userCompanyCount = window._userCompanyCount || 0;

AllSupplierInvoiceListTable()
//InvoiceListTable();
GetItemDetailsList()
GetSiteDetail();
GetSupplierDetail();
updateTotals();



if (_userCompanyCount === 0) {
    $(document).ready(function () {
        var SelectedCompanyId = $('#txtInvoiceCompanyId').val();
        GetCompanyDetail('textCompanyName', SelectedCompanyId);
    });
}
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


$(document).ready(function () {
    $('#textCompanyName').change(function () {

        getCompanyDetail($(this).val());
        getInvoiceNumber($(this).val());
    });

    var InvoiceCompanyId = $('#textCompanyName').val();
    var InvoiceGUID = $('#textSupplierInvoiceId').val();
    if (InvoiceCompanyId != null && InvoiceGUID == "00000000-0000-0000-0000-000000000000") {
        var CompanyId = $('#textCompanyName').val();
        getInvoiceNumber(CompanyId);
    }
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
let currentSortOrder = 'AscendingDate';

function sortTable(field) {
    if (currentSortOrder === 'Ascending' + field) {
        currentSortOrder = 'Descending' + field;
    } else {
        currentSortOrder = 'Ascending' + field;
    }

    siteloadershow();

    $.ajax({
        url: '/InvoiceMaster/SupplierInvoiceListAction',
        type: 'GET',
        data: {
            sortBy: currentSortOrder,
        },
        success: function (result) {
            siteloaderhide();
            $("#SupplierInvoicebody").html(result);
        },
        error: function (xhr, status, error) {
            console.error("Error fetching sorted data:", error);
            siteloaderhide();
        }
    });
}


$(document).ready(function () {
    $('#textSupplierName').change(function () {
        getSupplierDetail($(this).val());
    });
});

$(document).ready(function () {
    function handleFocus(event, $selector) {
        if (event.keyCode == 13 || event.keyCode == 9) {
            event.preventDefault();
            $selector.focus();
        }
    }

    function showErrorMessage(selector, message) {
        $(selector).text(message).show();
    }

    function debounce(func, delay) {
        let timer;
        return function (...args) {
            clearTimeout(timer);
            timer = setTimeout(() => func.apply(this, args), delay);
        };
    }

    // Handle quantity input
    $(document).on('input', '.txtproductquantity', function () {
        var productRow = $(this).closest(".productRow");
        updateProductTotalAmount(productRow);
        updateTotals();
    }).on('keydown', '.txtproductquantity', function (event) {
        var productRow = $(this).closest(".productRow");
        if (event.shiftKey && event.key === 'Tab') {
            event.preventDefault();
            productRow.find('.txtHSNcode').focus();
        } else if (event.key === 'Tab') {
            handleFocus(event, productRow.find('.txtproductamount'));
        }
    });

    // Handle GST input
    $(document).on('input', '.txtgst', function () {
        var productRow = $(this).closest(".productRow");
        var gstvalue = parseFloat($(this).val()) || 0;
        if (gstvalue > 100) {
            toastr.warning("GST% cannot be greater than 100%");
            $(this).val(100);
        }
        updateProductTotalAmount(productRow);
        updateTotals();
    });

    // Handle discount percentage input
    $(document).on('input', '.txtdiscountpercentage', debounce(function () {
        var productRow = $(this).closest(".productRow");
        var discountPercentage = parseFloat($(this).val()) || 0;
        if (discountPercentage > 100) {
            toastr.warning("Discount % cannot be greater than 100%");
            productRow.find(".txtdiscountpercentage").val(0);
            productRow.find(".txtdiscountamount").val(0);
        }
        UpdateDiscountPercentage(productRow);
    }, 300)).on('keydown', '.txtdiscountpercentage', function (event) {
        var productRow = $(this).closest(".productRow");
        if (event.shiftKey && event.key === 'Tab') {
            event.preventDefault();
            productRow.find('.txtdiscountamount').focus();
        } else if (event.key === 'Tab') {
            handleFocus(event, productRow.find('.txtgst'));
        }
    });

    // Handle discount amount input
    $(document).on('input', '.txtdiscountamount', debounce(function () {
        var productRow = $(this).closest(".productRow");
        var discountAmount = parseFloat($(this).val()) || 0;
        var productAmount = parseFloat(productRow.find(".productamount").val()) || 0;

        if (discountAmount > productAmount) {
            toastr.warning("Discount amount cannot exceed product price!");
            productRow.find(".txtdiscountamount").val(0);
            productRow.find(".txtdiscountpercentage").val(0);
        }
        updateDiscount(productRow);
    }, 300)).on('keydown', '.txtdiscountamount', function (event) {
        var productRow = $(this).closest(".productRow");
        if (event.shiftKey && event.key === 'Tab') {
            event.preventDefault();
            productRow.find('.txtproductamount').focus();
        } else if (event.key === 'Tab') {
            handleFocus(event, productRow.find('.txtdiscountpercentage'));
        }
    });

    // Handle product amount input
    $(document).on('input', '.txtproductamount', function () {
        var productRow = $(this).closest(".productRow");
        var productAmount = parseFloat($(this).val()) || 0;
        productRow.find(".productamount").val(productAmount.toFixed(2));
        productRow.find(".txtdiscountamount").val(0);
        productRow.find(".txtdiscountpercentage").val(0);
        updateProductTotalAmount(productRow);
        updateTotals();
    }).on('keydown', '.txtproductamount', function (event) {
        var productRow = $(this).closest(".productRow");
        if (event.shiftKey && event.key === 'Tab') {
            event.preventDefault();
            productRow.find('.txtproductquantity').focus();
        } else if (event.key === 'Tab') {
            handleFocus(event, productRow.find('.txtdiscountamount'));
        }
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
        updateTotals();
    }, 300));

    $(document).on('input', '#cart-tds', debounce(function () {
        var tds = parseFloat($('#cart-tds').val());
        var TotalAmount = parseFloat($('#cart-total').val());

        if (tds > TotalAmount) {
            toastr.warning("Value cannot be greater than subtotal.");
        } else {
            updateTotals();
        }
    }, 300));

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

var _FilterUserCompanyCount = window._FilterUserCompanyCount || 0;
if (_FilterUserCompanyCount === 0) {
    $(document).ready(function () {
        GetFilterInvoiceCompanyList();
    });
}
GetFilterInvoiceSupplierList();

function GetFilterInvoiceCompanyList() {
    $.ajax({
        url: '/Company/GetCompanyNameList',
        success: function (result) {

            if (result.length > 0) {
                var $dropdown = $('#ddlFilterInvoiceCompany');
                $dropdown.empty();

                $dropdown.append('<option selected value="" disabled>Select Company</option>');

                $.each(result, function (i, data) {
                    $dropdown.append(
                        '<option value="' + data.companyId + '" data-company-name="' + data.companyName + '">' + data.companyName + '</option>'
                    );
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching company details:', error);
        }
    });
}
function GetFilterInvoiceSupplierList() {
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

            $("#ddlFilterInvoiceSupplier").autocomplete({
                source: supplierDetails,
                minLength: 0,
                select: function (event, ui) {
                    event.preventDefault();
                    $("#ddlFilterInvoiceSupplier").val(ui.item.label);
                    $("#ddlFilterInvoiceSupplierHidden").val(ui.item.value);
                    $("#ddlFilterInvoiceSupplierHidden").trigger('change');
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
$("#ddlFilterInvoiceCompany").on('change', function () {
    filterSupplierInvoiceTable();
});
$("#ddlFilterInvoiceSupplierHidden").on('change', function () {
    filterSupplierInvoiceTable();
});
function filterSupplierInvoiceTable() {
    siteloadershow();
    var CompanyId = $('#ddlFilterInvoiceCompany').val();
    var SupplierId = $('#ddlFilterInvoiceSupplierHidden').val();

    $.ajax({
        url: '/InvoiceMaster/SupplierInvoiceListAction',
        type: 'GET',
        data: {
            CompanyId: CompanyId,
            SupplierId: SupplierId
        },
        success: function (result) {
            siteloaderhide();
            $("#SupplierInvoicebody").html(result);
        },

    });
}

function SupplierInvoiceSearchTable() {
    siteloadershow();
    var Search = $('#txtSupplierInvoiceSearch').val();
    $.ajax({
        url: '/InvoiceMaster/SupplierInvoiceListAction',
        type: 'GET',
        data: {
            searchText: Search
        },
        success: function (result) {
            siteloaderhide();
            $("#SupplierInvoicebody").html(result);
        },

    });
}
function fn_ResetAllInvoiceDropdown() {
    window.location = '/InvoiceMaster/SupplierInvoiceListView';
}
function DeleteSupplierInvoice(Id, SupplierInvoiceNo, InvoiceNo, element) {
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    Swal.fire({
        title: "Are you sure you want to delete this invoice?",
        text: "To confirm, type the invoice no below ",
        input: 'text',
        inputPlaceholder: 'Enter the invoice no to confirm',
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        confirmButtonClass: "btn btn-primary w-xs me-2 mt-2",
        cancelButtonClass: "btn btn-danger w-xs mt-2",
        buttonsStyling: false,
        showCloseButton: true,
        inputValidator: (value) => {
            if (SupplierInvoiceNo == "") {
                if (!value) {
                    return 'Please enter the Invoice No!';
                } else if (value !== InvoiceNo) {
                    return 'Invoice No mismatch! Please enter valid Invoice No';
                }
            } else {
                if (!value) {
                    return 'Please enter the Invoice No!';
                } else if (value !== SupplierInvoiceNo) {
                    return 'Invoice No mismatch! Please enter valid Invoice No';
                }
            }
        }
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
                'Invoice have no changes.!!😊',
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
                maxlength: 15
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
        var selectedGroupAddress = $('input[name="selectedGroupAddress"]:checked').val();
        if ($('#addnewproductlink tr').length >= 1) {

            var ItemDetails = [];
            $(".productRow").each(function () {
                var orderRow = $(this);
                var objData = {
                    ItemName: orderRow.find(".txtInvoiceItemName").val(),
                    ItemId: orderRow.find(".txtInvoiceItemNameHidden").val(),
                    ItemDescription: orderRow.find(".txtInvoiceProductDes").val(),
                    UnitType: orderRow.find(".txtPOUnitType").val(),
                    DiscountAmount: orderRow.find(".txtdiscountamount").val(),
                    DiscountPer: orderRow.find(".txtdiscountpercentage").val(),
                    Quantity: orderRow.find(".txtproductquantity").val(),
                    PricePerUnit: orderRow.find(".txtproductamount").val(),
                    GSTamount: orderRow.find(".txtgstAmount").val(),
                    GSTPercentage: orderRow.find(".txtgst").val(),
                    TotalAmount: orderRow.find(".txtproducttotalamount").val(),
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
function UpdateInvoiceDetails() {
    siteloadershow();
    if ($("#CreateInvoiceForm").valid()) {

        if ($('#addnewproductlink tr').length >= 1) {

            var ItemDetails = [];
            $(".productRow").each(function () {

                var orderRow = $(this);
                var objData = {
                    ItemName: orderRow.find(".txtInvoiceItemName").val(),
                    ItemId: orderRow.find(".txtInvoiceItemNameHidden").val(),
                    ItemDescription: orderRow.find(".txtInvoiceProductDes").val(),
                    UnitType: orderRow.find(".txtPOUnitType").val(),
                    DiscountAmount: orderRow.find(".txtdiscountamount").val(),
                    DiscountPer: orderRow.find(".txtdiscountpercentage").val(),
                    Quantity: orderRow.find(".txtproductquantity").val(),
                    PricePerUnit: orderRow.find(".txtproductamount").val(),
                    GSTamount: orderRow.find(".txtgstAmount").val(),
                    GSTPercentage: orderRow.find(".txtgst").val(),
                    TotalAmount: orderRow.find(".txtproducttotalamount").val(),
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
                Id: $('#textSupplierInvoiceId').val(),
                SiteId: $("#txtModelSiteId").val(),
                InvoiceNo: $("#textInvoicePrefix").val(),
                InvoiceType: $('#ddlinvoicetype').val(),
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
                Tds: $('#cart-tds').val(),
                TotalDiscount: $('#cart-discount').val(),
                CreatedOn: $('#textCreatedOn').val(),
                ItemList: ItemDetails,
                ShippingAddress: $("input[name='selectedAddress']:checked").data('address'),
                ChallanNo: $("#txtchalanNo").val(),
                Lrno: $("#textPONoListHidden").val() ? $("#textPONoListHidden").val() : $("#txtlrNo").val(),
                VehicleNo: $("#txtvehicleno").val(),
                DispatchBy: $("#txtdispatch").val(),
                PaymentTerms: $("#txtpayment").val(),
                SiteGroup: $("#InvoiceGroupList").val(),
                DiscountRoundoff: $("#IDiscountRoundOff").val(),
                IsApproved: $("#txtInvoiceIsApproved").val(),
                GroupAddress: $('input[name="selectedGroupAddress"]:checked').data('address'),
                Poid: $("#txtInvoicePOID").val(),
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
$(document).ready(function () {
    $('input[name="selectedAddress"]').change(function () {
        var selectedAddress = $(this).data('address');
        $('#selectedShippingAddress').val(selectedAddress);
    });
});
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

function updateProductTotalAmount(row) {
    var $row = $(row); 
    var productPrice = parseFloat($row.find(".txtproductamount").val()) || 0;
    var quantity = parseFloat($row.find(".txtproductquantity").val()) || 0;
    var discount = parseFloat($row.find(".txtdiscountamount").val()) || 0;
    var gstPercent = parseFloat($row.find(".txtgst").val()) || 0;

    var priceAfterDiscount = productPrice;
    var gstAmount = (priceAfterDiscount * quantity * gstPercent) / 100;
    var totalAmount = (priceAfterDiscount * quantity) + gstAmount;

    $row.find(".txtgstAmount").val(gstAmount.toFixed(2));
    $row.find(".txtproducttotalamount").val(totalAmount.toFixed(2));
}


function updateDiscount(row) {
    var originalPrice = parseFloat(row.find(".productamount").val()) || 0;
    var discountAmount = parseFloat(row.find(".txtdiscountamount").val()) || 0;

    if (originalPrice > 0) {
        var discountedPrice = originalPrice - discountAmount;
        var discountPercent = (discountAmount / originalPrice) * 100;
        row.find(".txtdiscountpercentage").val(discountPercent.toFixed(2));
        row.find(".txtproductamount").val(discountedPrice.toFixed(2));
    }
    updateProductTotalAmount(row);
    updateTotals();
}

function UpdateDiscountPercentage(row) {
    var originalPrice = parseFloat(row.find(".productamount").val()) || 0;
    var discountPercent = parseFloat(row.find(".txtdiscountpercentage").val()) || 0;

    if (originalPrice > 0) {
        var discountAmount = (originalPrice * discountPercent) / 100;
        var discountedPrice = originalPrice - discountAmount;
        row.find(".txtdiscountamount").val(discountAmount.toFixed(2));
        row.find(".txtproductamount").val(discountedPrice.toFixed(2));
    }
    updateProductTotalAmount(row);
    updateTotals();
}

function updateTotals() {
    var totalSubtotal = 0, totalGst = 0, totalQuantity = 0, totalDiscount = 0;

    $(".productRow").each(function () {
        var row = $(this);
        var price = parseFloat(row.find(".txtproductamount").val()) || 0;
        var quantity = parseFloat(row.find(".txtproductquantity").val()) || 0;
        var gstAmount = parseFloat(row.find(".txtgstAmount").val()) || 0;
        var discountAmount = parseFloat(row.find(".txtdiscountamount").val()) || 0;

        totalSubtotal += price * quantity;
        totalGst += gstAmount;
        totalQuantity += quantity;
        totalDiscount += discountAmount * quantity;
    });

    var Tds = parseFloat($('#cart-tds').val()) || 0;
    var discountRoundOff = parseFloat($('#IDiscountRoundOff').val()) || 0;

    var grandTotal = totalSubtotal + totalGst - Tds + discountRoundOff;

    // Round off
    var decimal = grandTotal - Math.floor(grandTotal);
    grandTotal = (decimal <= 0.5) ? Math.floor(grandTotal) : Math.ceil(grandTotal);

    $("#cart-subtotal").val(totalSubtotal.toFixed(2));
    $("#totalgst").val(totalGst.toFixed(2));
    $("#cart-discount").val(totalDiscount.toFixed(2));
    $("#TotalDiscountPrice").html(totalDiscount.toFixed(2));
    $("#TotalProductQuantity").text(totalQuantity);
    $("#TotalProductPrice").html(totalSubtotal.toFixed(2));
    $("#TotalProductGST").html(totalGst.toFixed(2));
    $("#cart-total").val(grandTotal.toFixed(2));
    $("#TotalProductAmount").html(grandTotal.toFixed(2));
}

function removeItem(btn) {
    $(btn).closest("tr").remove();
    updateRowNumbers();
    updateTotals();

    const tableBody = document.querySelector("#InvoiceProductDetailsTable tbody");
    if (!tableBody) return;

    const allAddBtns = tableBody.querySelectorAll(".AddNewInvoiceProductDetailBtn");
    allAddBtns.forEach(btn => btn.remove());

    const lastRow = tableBody.querySelector("tr:last-child");
    if (lastRow) {
        const actionTd = lastRow.querySelector(".product-removal");
        if (actionTd) {
            const addBtn = document.createElement("a");
            addBtn.className = "btn text-primary AddNewInvoiceProductDetailBtn";
            addBtn.style.marginLeft = "-33px";
            addBtn.onclick = function () { AddNewInvoiceProductDetailRow(); };
            addBtn.innerHTML = '<i class="lni lni-circle-plus"></i>';
            actionTd.appendChild(addBtn);
        }
    }
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


//function InvoiceListTable() {

//    var searchText = $('#txtInvoiceSearch').val();
//    var searchBy = $('#InvoiceSearchBy').val();

//    $.get("/InvoiceMaster/InvoiceListAction", { searchBy: searchBy, searchText: searchText })
//        .done(function (result) {
//            $("#Invoicetbody").html(result);
//        })

//}

//function filterInvoiceTable() {
//    siteloadershow();
//    var searchText = $('#txtInvoiceSearch').val();
//    var searchBy = $('#InvoiceSearchBy').val();

//    $.ajax({
//        url: '/InvoiceMaster/InvoiceListAction',
//        type: 'GET',
//        data: {
//            searchText: searchText,
//            searchBy: searchBy
//        },
//        success: function (result) {
//            siteloaderhide();
//            $("#Invoicetbody").html(result);
//        },
//        error: function (xhr, status, error) {
//            siteloaderhide();

//        }
//    });
//}

//function InvoiceSortTable() {
//    siteloadershow();
//    var sortBy = $('#SortByInvoice').val();
//    $.ajax({
//        url: '/InvoiceMaster/InvoiceListAction',
//        type: 'GET',
//        data: {
//            sortBy: sortBy
//        },
//        success: function (result) {
//            siteloaderhide();
//            $("#Invoicetbody").html(result);
//        },
//        error: function (xhr, status, error) {
//            siteloaderhide();

//        }
//    });
//}

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

//$(document).ready(function () {
//    var invoiceRoleUser = $('#UserRoleinInvoice').val();
//    var invoiceSiteId = $('#textInvoiceSiteName').val();

//    if (invoiceRoleUser == 8) {
//        fn_GetInvoiceSiteAddressList(invoiceSiteId);
//    }
//    else {
//        $('#textInvoiceSiteName').change(function () {
//            var Site = $(this).val();
//            fn_GetInvoiceSiteAddressList(Site);
//        });
//    }
//    $('#drpInvoiceSiteAddress').select2({
//        theme: 'bootstrap4',
//        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
//        placeholder: $(this).data('placeholder'),
//        allowClear: Boolean($(this).data('allow-clear')),
//        dropdownParent: $("#mdShippingAdd")
//    });

//    $(document).on('click', '#removeAddress', function () {
//        $(this).closest('tr').remove();
//        $('.add-addresses').prop('disabled', false);
//    });
//});

//function fn_GetInvoiceSiteAddressList(SiteId) {
//    $.ajax({
//        url: '/SiteMaster/DisplaySiteAddressList?SiteId=' + SiteId,
//        success: function (result) {

//            $('#drpInvoiceSiteAddress').empty();
//            $('#textmdAddress').val('');
//            $('#drpInvoiceSiteAddress').append('<option value="">-- Select site address --</option>');
//            if (Array.isArray(result)) {
//                $.each(result, function (i, data) {

//                    $('#drpInvoiceSiteAddress').append('<option value="' + data.address + ', Code : ' + data.stateCode + '">' + data.address + ', Code :' + data.stateCode + '</option>');
//                });
//            } else {

//                $('#textmdAddress').val(result.shippingAddress + ' , ' + result.shippingArea + ', ' + result.shippingCityName + ', ' + result.shippingStateName + ', ' + result.shippingCountryName + ',Code : ' + result.stateCode);
//            }
//        }
//    });
//}
//$('#drpInvoiceSiteAddress').on('change', function () {
//    var selectedInvoiceAddress = $(this).val();
//    $('#textmdAddress').val(selectedInvoiceAddress);
//});
//function addShippingAddress() {
//    siteloadershow();
//    if ($("#shippingAddressForm").valid()) {
//        var address = $("#textmdAddress").val();

//        if ($('#dvShippingAddress .ac-invoice-shippingadd').length > 0) {
//            siteloaderhide();
//            Swal.fire({
//                title: "Only one address allowed!",
//                text: "You can only add one shipping address.",
//                icon: "warning",
//                confirmButtonColor: "#3085d6",
//                confirmButtonText: "OK"
//            });
//            return;
//        }

//        var newRow = '<div class="row ac-invoice-shippingadd ShippingAddress">' +
//            '<div class="col-1 col-sm-1">' +
//            '<label id="lblshprownum1">1</label>' +
//            '</div>' +
//            '<div class="col-5 col-sm-5 text-center">' +
//            '<p id="addShippingAddress" class="shippingaddress">' + address + '</p>' +
//            '</div>' +
//            '<div class="col-2 col-sm-2">' +
//            '<a id="remove" class="btn text-primary text-center" onclick="fn_removeShippingAdd(this)"><i class="lni lni-trash"></i></a>' +
//            '</div>' +
//            '</div>';

//        $('#dvShippingAddress').append(newRow);
//        siteloaderhide();
//    } else {
//        siteloaderhide();
//        toastr.warning('please select address!');
//    }
//

function fn_OpenAddproductmodal() {
    const supplierName = $("#textSupplierName").val();
    const companyName = $("#textCompanyName").val();
    const siteName = $("#drpSiteName").val();
    const invoiceNo = $("#textSupplierInvoiceNo").val();

    let hasError = false;

    // Clear previous error states
    $("#frmdrpdashbord").css("border", "none");
    $("#siteErrorMesssage").html("");
    $("#textSupplierName-error").html("");
    $("#textCompanyName-error").html("");

    // Validate Site Name
    if (siteName === "") {
        $("#frmdrpdashbord").css("border", "2px solid red");
        $("#siteErrorMesssage").html("Select Site").css("color", "red");
        hasError = true;
    }

    // Validate Supplier and Company Names
    if (!supplierName) {
        $("#textSupplierName-error").html("Select Supplier Name");
        hasError = true;
    }
    if (!companyName) {
        $("#textCompanyName-error").html("Select Company Name");
        hasError = true;
    }

    // Handle errors and display warnings
    if (hasError) {
        toastr.warning("Fill in all required fields (Site, Supplier, Company) to proceed.");
    } else {
        if (invoiceNoExists != true) {
            // If no errors, show the modal
            $('#mdProductSearch').val('');
            $('#mdPoproductModal').modal('show');
        }
    }
}


var invoiceNoExists = null;
$(document).ready(function () {
    $('#textSupplierInvoiceNo').on('input', function () {
        checkSupplierInoiveNoExits();
    });
});

function checkSupplierInoiveNoExits() {
    if ($("#textSupplierName").val() != null && $("#textCompanyName").val() != null && $("#textSupplierInvoiceNo").val() != "") {
        var compnayid = $("#textCompanyName").val();
        var supplierid = $("#textSupplierName").val();
        var SupplierInvoiceNo = $("#textSupplierInvoiceNo").val();
        var formData = new FormData();

        formData.append('CompanyId', $("#textCompanyName").val());
        formData.append('SupplierId', $("#textSupplierName").val());
        formData.append('SupplierInvoiceNo', $("#textSupplierInvoiceNo").val());
        $.ajax({
            url: '/InvoiceMaster/CheckSupplierInvoiceNo',
            type: 'Post',
            datatype: 'json',
            data: formData,
            processData: false,
            contentType: false,
            success: function (Result) {
                siteloaderhide();
                if (Result.data == true) {
                    invoiceNoExists = true;
                    $("#textSupplierInvoiceNo").css("border", "2px solid red");
                    $("#InvoicenoErrorMesssage").show();
                    $("#InvoicenoErrorMesssage").html("invoice no already exist").css("color", "red");
                } else {
                    invoiceNoExists = false;
                    $("#textSupplierInvoiceNo").css("border", "2px solid lightgrey");
                    $("#InvoicenoErrorMesssage").hide();
                }
            }
        });
    } else {
        toastr.warning("Enter supplier, company and invoice no.")
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
    var $currentTd = $(element).closest('td');
    var $productDesBtn = $currentTd.find('.ProductDesBtn');
    var $productDesText = $currentTd.find('.ProductDesText');

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
            $('#InvoiceGroupList').empty();
            $('#InvoiceGroupList').append('<option value="" selected>' + "Select Group" + '</option>');
            $.each(result, function (i, data) {
                $('#InvoiceGroupList').append('<option value="' + data.groupName + '" data-groupids="' + data.groupId + '">' + data.groupName + '</option>');
            });
        }
    });
}
$(document).ready(function () {
    $('#InvoiceGroupList').change(function () {
        var selectedOption = $(this).find('option:selected');
        var groupId = selectedOption.data('groupids');
        GetGroupAddress(groupId);
    });
    var EditSiteId = $('#txtModelSiteId').val();
    var SessionSiteId = $('#txtsessionSiteName').val();
    var Company = $('#textCompanyName').val();
    if (EditSiteId != "00000000-0000-0000-0000-000000000000" && Company != null && SessionSiteId == "") {
        EditGroupList(EditSiteId)
    }
    else if (EditSiteId != "00000000-0000-0000-0000-000000000000" && SessionSiteId != "" && Company != null) {
        EditGroupList(EditSiteId)
    }
    else {
        GetGroupList();
    }
});

function EditGroupList(EditSiteId) {
    $.ajax({
        url: '/InvoiceMaster/EditGroupNameListBySiteId?SiteId=' + EditSiteId,
        success: function (result) {
            var existingGroup = $('#InvoiceGroupList option:selected').text();
            $.each(result, function (i, data) {
                if (existingGroup !== data.groupName) {
                    $('#InvoiceGroupList').append('<option value="' + data.groupName + '" data-groupids="' + data.groupId + '">' + data.groupName + '</option>');
                }
            });
        }
    });
}


function GetGroupAddress(GroupId) {
    if (GroupId == undefined) {
        $('#dvGroupAddress').empty();
    }
    else {
        siteloadershow();
        $.ajax({
            url: '/SiteMaster/GetGroupDetailsByGroupId?GroupId=' + GroupId,
            type: 'GET',
            contentType: 'application/json;charset=utf-8',
            dataType: 'json',
            success: function (response) {
                siteloaderhide();
                $('#dvGroupAddress').empty();
                if (response.groupAddressList == null) {

                }
                else {
                    $.each(response.groupAddressList, function (i, data) {
                        var groupAddressNumber = i + 1;

                        $('#dvGroupAddress').append(
                            '<div class="row ac-invoice-groupadd GroupAddress" style="display: flex; align-items: flex-start;">' +
                            '<div class="col-2 col-sm-2" style="flex: 1; display: flex; align-items: center; justify-content: flex-start;">' +
                            '<label id="lblgprownum' + groupAddressNumber + '" style="margin-right: 10px;">' + groupAddressNumber + '</label>' +
                            '</div>' +
                            '<div class="col-5 col-sm-5" style="flex: 1; display: flex; align-items: center;">' +
                            '<p id="groupaddress_' + groupAddressNumber + '">' + data.groupAddress + '</p>' +
                            '<input type="hidden" id="selectedGroupAddress_' + groupAddressNumber + '" name="selectedGroupAddress" value="' + data.groupAddress + '" />' +
                            '</div>' +
                            '<div class="col-2 col-sm-2" style="flex: 1; display: flex; align-items: center; justify-content: center;">' +
                            '<input class="nav-radio form-check-input" ' +
                            'name="selectedGroupAddress" ' +
                            'data-bs-toggle="tab" ' +
                            'role="tab" ' +
                            'type="radio" ' +
                            'id="GroupAddressRadio_' + groupAddressNumber + '" ' +
                            'data-address="' + data.groupAddress + '"' +
                            'value="' + data.groupAddress + '"' +
                            (i === 0 ? ' checked' : '') + ' />' +
                            '</div>' +
                            '</div>' +
                            '<hr>'
                        );
                    });
                }
            },
        });
    }
}

var InvoicePONo = null;
function getallPONoList() {
    var SupplierId = $('#textSupplierName').val();
    $.ajax({
        url: '/PurchaseMaster/PurchaseOrderNoListInvoice?SupplierId=' + SupplierId,
        method: 'GET',
        success: function (result) {
            var PODetails = result.map(function (data) {
                return {
                    label: data.buyersPurchaseNo ? data.poid + '-' + data.buyersPurchaseNo : data.poid,
                    value: data.buyersPurchaseNo ? data.poid + '-' + data.buyersPurchaseNo : data.poid,
                    poid: data.poid
                };
            });

            $("#textPONoList").autocomplete({
                source: PODetails,
                minLength: 0,
                select: function (event, ui) {
                    event.preventDefault();
                    $("#textPONoList").val(ui.item.label);
                    $("#textPONoListHidden").val(ui.item.value);
                    InvoicePONo = ui.item.poid;
                },
                focus: function () {
                    return false;
                }
            }).focus(function () {
                $(this).autocomplete("search", "");
            });

            $("#textPONoList").on('input', function () {
                if ($(this).val() === '') {
                    $("#textPONoListHidden").val('');
                }
            });
        },
        error: function (err) {
            console.error("Failed to fetch purchase order details list: ", err);
        }
    });
}

$(document).ready(function () {
    $("#txtlrNo").on('click', function () {
        $("#txtlrNo").prop("readonly", false);
        $("#textPONoList").prop("readonly", true);
        $("#textPONoList").val('');
    });

    $("#textPONoList").on('click', function () {
        $("#txtlrNo").prop("readonly", true);
        $("#txtlrNo").val('');
        $("#textPONoList").prop("readonly", false);
    });
    $('#textSupplierName').change(function () {
        getallPONoList();
    });
});
function ExportToPDFInvoiceDetails() {
    var CompanyId = $('#ddlFilterInvoiceCompany').val();
    var SupplierId = $('#ddlFilterInvoiceSupplierHidden').val();
    siteloadershow();
    $.ajax({
        url: '/InvoiceMaster/InvoiceDetailsExportToPdf',
        type: 'Get',
        data: {
            CompanyId: CompanyId,
            SupplierId: SupplierId
        },
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
            if (xhr.status === 400) {
                toastr.warning("Please select a Site.");
            } else {
                toastr.warning("An error occurred while processing the request.");
            }
        },
        xhrFields: {
            responseType: 'blob'
        }
    });
}
function ExportToExcelInvoiceDetails() {
    var CompanyId = $('#ddlFilterInvoiceCompany').val();
    var SupplierId = $('#ddlFilterInvoiceSupplierHidden').val();
    siteloadershow();
    $.ajax({
        url: '/InvoiceMaster/InvoiceDetailsExportToExcel',
        type: 'Get',
        data: {
            CompanyId: CompanyId,
            SupplierId: SupplierId
        },
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
            if (xhr.status === 400) {
                toastr.warning("Please select a Site.");
            } else {
                toastr.warning("An error occurred while processing the request.");
            }
        },
        xhrFields: {
            responseType: 'blob'
        }
    });
}


//----------------------------------------------------------- Add New Row ----------------------------------------------------//

function GetAllProductList(row) {
    $.ajax({
        url: '/ItemMaster/GetItemNameList',
        method: 'GET',
        success: function (result) {
            var supplierDetails = result.map(function (data) {
                return {
                    label: data.itemName,
                    value: data.itemId
                };
            });

            var $itemInput = $(row).find('.txtInvoiceItemName');

            $itemInput.autocomplete({
                source: supplierDetails,
                minLength: 0,
                select: function (event, ui) {
                    event.preventDefault();
                    $(this).val(ui.item.label);

                    var $row = $(this).closest('.productRow');

                    $row.find('.txtInvoiceItemNameHidden').val(ui.item.value);
                    $row.find('.txtInvoiceItemNameHidden').trigger('change');

                    GetItemDetailsByItemId(ui.item.value, $row);
                },
                focus: function () {
                    return false;
                }
            }).focus(function () {
                if ($(this).val() === "") {
                    $(this).closest('.productRow').find('.txtInvoiceItemNameHidden').val("");
                }
                $(this).autocomplete("search", "");
            });
        },
        error: function (err) {
            console.error("Failed to fetch supplier list: ", err);
        }
    });
}

function GetProductUnittypeList(row) {
    $.ajax({
        url: '/ItemMaster/GetAllUnitType',
        type: 'GET',
        success: function (result) {
            const $unitTypeDropdown = $(row).find('.txtPOUnitType');
            $unitTypeDropdown.empty();
            $unitTypeDropdown.append('<option value="">Select</option>');

            $.each(result, function (i, data) {
                $unitTypeDropdown.append('<option value="' + data.unitId + '">' + data.unitName + '</option>');
            });

            const unitTypeId = $(row).find('.txtunittype').val();

            if (unitTypeId) {

                $unitTypeDropdown.val(unitTypeId).change(); 
            }
        },
        error: function (err) {
            console.error('Failed to fetch unit type list: ', err);
        }
    });
}


function GetItemDetailsByItemId(ItemId, row) {
    $.ajax({
        url: '/InvoiceMaster/GetItemDetailsByProductId?ProductId=' + ItemId,
        type: 'POST',
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                $(row).find('.txtHSNcode').val(data.hsncode);
                $(row).find('.txtproductquantity').val(1);
                $(row).find('.txtunittype').val(data.unitType);
                $(row).find('.txtPOUnitType').val(data.unitType); 
                $(row).find('.txtproductamount').val(data.pricePerUnit);
                $(row).find('.productamount').val(data.pricePerUnit);
                $(row).find('.txtgstAmount').val(data.gstamount);
                $(row).find('.txtgst').val(data.gstPercentage);
                $(row).find('.txtproducttotalamount').val(data.totalAmount);
            }
        },
        error: function (err) {
            console.error("Failed to fetch product details: ", err);
        }
    });
}



function AddNewInvoiceProductDetailRow() {
    const tableBody = document.querySelector("#InvoiceProductDetailsTable tbody");
    if (tableBody != null) {
        const rowCount = tableBody.rows.length + 1;  

        const lastRow = tableBody.lastElementChild;
        var lastProductNameInput = lastRow ? lastRow.querySelector(".txtInvoiceItemName") : null;

        if (lastProductNameInput && lastProductNameInput.value.trim() === "") {
            toastr.error("Please select a product before adding a new row.");
            lastProductNameInput.focus();
            return;
        }

        const allAddRowButtons = tableBody.querySelectorAll(".AddNewInvoiceProductDetailBtn");
        allAddRowButtons.forEach(btn => btn.remove());

        const newRow = document.createElement("tr");
        newRow.classList.add("productRow");
        newRow.innerHTML = `
            <td scope="row" class="product-id text-start">${rowCount}</td>
            <td class="text-start" style="width:230px">
                <input type="text" class="form-control txtInvoiceItemName" id="txtInvoiceItemName${rowCount}" placeholder="Item Name" style="margin-top:25px;"/>
                <input type="hidden" class="txtInvoiceItemNameHidden" id="txtInvoiceItemNameHidden${rowCount}" />
                <div class="row align-items-center ProductDesBtn" id="ProductDesBtn" onclick="fn_AddInvoiceProductDescription(this)">
                    <div class="col-sm-1" style="margin-top : -5px;">
                        <i class="lni lni-plus text-success" style="font-size: 10px; text-shadow: 1px 1px 1px #000;"></i>
                    </div>
                    <div class="col-sm-10" style="margin-left:-5px;">
                        <p class="text-primary mb-0"><b>Item Description</b></p>
                    </div>
                </div>
                <div class="row align-items-center ProductDesText" id="ProductDesText" style="display:none;">
                    <div class="col-sm-10" style="margin-right:-7px;">
                        <input type="text" class="form-control txtInvoiceProductDes" placeholder="Description" id="txtInvoiceProductDes${rowCount}" />
                    </div>
                    <div class="col-sm-1">
                        <div class="text-danger" style="cursor: pointer; font-size:20px;" onclick="fn_AddInvoiceProductDescription(this)">x</div>
                    </div>
                </div>
            </td>
            <td class="text-center">
                <input type="text" class="form-control bg-light txtHSNcode" id="txtHSNcode${rowCount}" placeholder="Hsn" readonly style="width: 100px;" />
            </td>
            <td class="text-start">
                <div class="">
                    <input type="number" class="product-quantity form-control txtproductquantity" id="txtproductquantity${rowCount}" placeholder="Quantity" value="" style="width:110px;">
                </div>
            </td>
            <td class="text-start">
                <div class="">
                    <input type="hidden" class="txtunittype" id="txtunittype${rowCount}" />
                    <select class="form-control txtPOUnitType" id="txtPOUnitType${rowCount}" style="width: 112px;">
                        <option value=""></option>
                    </select>
                </div>
            </td>
            <td class="text-start">
                <input type="text" class="form-control txtproductamount" id="txtproductamount${rowCount}" oninput="preventEmptyValue(this)" placeholder="Product amount" style="width:125px;" />
                <input type="hidden" class="form-control productamount" id="productamount${rowCount}" />
            </td>
            <td class="text-start">
                <input type="text" class="form-control txtdiscountamount" id="txtdiscountamount${rowCount}" value="0" placeholder="Dis(₹)" style="width:100px;" />
            </td>
            <td class="text-start">
                <input type="text" class="form-control txtdiscountpercentage" id="txtdiscountpercentage${rowCount}" value="0" placeholder="Dis(%)" style="width:100px;" />
            </td>
            <td class="text-start">
                <input type="text" class="product-Gstper form-control txtgst" id="txtgst${rowCount}" oninput="preventEmptyValue(this)" placeholder="Gst(%)" style="width:100px;" />
            </td>
            <td>
                <input type="number" class="product-Gstamount form-control bg-light txtgstAmount" id="txtgstAmount${rowCount}" readonly placeholder="Gst(₹)" style="width:140px;" />
            </td>
            <td class="text-start">
                <input type="text" class="product-producttotalamount form-control bg-light txtproducttotalamount" id="txtproducttotalamount${rowCount}" placeholder="Total Amount" readonly style="width:140px;" />
            </td>
           <td class="product-removal">
                <a class="btn text-primary remove-btn" onclick="removeItem(this)"><i class="lni lni-trash"></i></a>
                <a class="btn text-primary AddNewInvoiceProductDetailBtn" onclick="AddNewInvoiceProductDetailRow()" style="margin-left: -33px;">
                   <i class="lni lni-circle-plus"></i> 
                </a>
            </td>
        `;

        tableBody.appendChild(newRow);

        GetAllProductList(newRow);
        GetProductUnittypeList(newRow);
    }
}
