
var _editCompanyselectedValue = "";
var _editSupplierselectedValue = "";
var _editItemselectedValue = "";
var TotalPending = '';


AllPurchaseRequestListTable();
GetSiteDetail();
GetCompanyName();
GetSupplierDetails();
GetPurchaseOrderList();
GetPOList();
checkAndDisableAddButton();
GetAllItemDetailsList();
updateTotals();


function AllPurchaseRequestListTable() {

    var searchText = $('#txtPurchaseRequestSearch').val();
    var searchBy = $('#PurchaseRequestSearchBy').val();

    $.get("/PurchaseMaster/PurchaseRequestListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {

            $("#purchaseRequesttbody").html(result);
        })

}

function filterPurchaseRequestTable() {
    siteloadershow();
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
            siteloaderhide();
            $("#purchaseRequesttbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}
function GetSiteDetail() {
    $.ajax({
        url: '/SiteMaster/GetSiteNameList',
        success: function (result) {
            if (result.length > 0) {
                $.each(result, function (i, data) {
                    $('#txtPoSiteName').append('<Option value=' + data.siteId + '>' + data.siteName + '</Option>')
                });
            }
        }
    });
}

$(document).ready(function () {
    var RoleUserId = $('#txtPRRoleId').val();
    var prSiteId = $('#txtPRsiteid').val();

    if (RoleUserId == 8) {
        fn_getPRSiteDetail(prSiteId);
    }
    else {
        $('#txtPoSiteName').change(function () {
            fn_getPRSiteDetail($(this).val());
        });
    }
});


function fn_getPRSiteDetail(SiteId, callback) {
    $('#drpPRSiteAddress').empty();
    siteloadershow();
    $.ajax({
        url: '/SiteMaster/GetSiteAddressList?SiteId=' + SiteId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (result) {
            siteloaderhide();
            $('#drpPRSiteAddress').append('<option value="" selected disabled>--Select Site Address--</option>')
            if (result.length > 0) {
                $.each(result, function (i, data) {
                    $('#drpPRSiteAddress').append('<option value=' + data.aid + '>' + data.address + '</option>')
                });
            }
            if (callback && typeof callback === "function") {
                callback();
            }
        },
    });
}

function sortPurchaseRequestTable() {
    siteloadershow();
    var sortBy = $('#PurchaseRequestSortBy').val();
    $.ajax({
        url: '/PurchaseMaster/PurchaseRequestListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#purchaseRequesttbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function SelectPurchaseRequestDetails(PurchaseId, element) {
    siteloadershow();
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/PurchaseMaster/DisplayPurchaseRequestDetails?PurchaseId=' + PurchaseId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response) {
                $('#dspPrNo').val(response.prNo);
                $('#dspPId').val(PurchaseId);
                $('#dspItem').val(response.itemName);
                $('#dspUnitName').val(response.unitName);
                $('#dspQuantity').val(response.quantity);
                $('#dspSiteName').val(response.siteName);
                $('#dspPrSiteAddress').val(response.siteAddress);
                $('#dspIsApproved').prop('checked', response.isApproved);
            } else {
                siteloaderhide();
                toastr.error('Empty response received.');
            }
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });

}

function CreatePurchaseRequest() {
    siteloadershow();
    if ($("#purchaseRequestForm").valid()) {
        var siteName = null;
        var siteId = $("#SiteIdinPR").val();
        var PRsiteId = $("#txtPoSiteName").val();
        if (PRsiteId && PRsiteId !== "") {
            siteName = PRsiteId;
        } else {
            siteName = siteId;
        }
        var siteAddressId = $('#drpPRSiteAddress').val();
        var siteAddress = $('#drpPRSiteAddress option:selected').text();

        var objData = {
            SiteAddressId: siteAddressId,
            SiteAddress: siteAddress,
            UnitTypeId: $('#txtUnitTypeHidden').val(),
            ItemId: $('#txtItemName').val(),
            ItemName: $('#searchItemnameInput').val(),
            SiteId: siteName,
            Quantity: $('#txtQuantity').val(),
            PrNo: $('#prNo').val(),
        }

        $.ajax({
            url: '/PurchaseMaster/CreatePurchaseRequest',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                if (Result.code == 200) {
                    var offcanvasElement = document.getElementById('CreatePurchaseRequest');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {

                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllPurchaseRequestListTable();
                    toastr.success(Result.message);
                } else {
                    toastr.error(Result.message);
                }
                siteloaderhide();
            }
        })
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}

function ClearPurchaseRequestTextBox() {

    $('#prNo').val('');
    $.ajax({
        url: '/PurchaseMaster/CheckPRNo',
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            $('#prNo').val(response);
        }, error: function () {
            siteloaderhide();
            toastr.error("Error in getting PR No");
        }
    });
    resetPRForm();
    $('#changeName').html('Create PurchaseRequest');
    $('#searchItemnameInput').val('');
    $('#txtUnitType').val('');
    $('#searchItemname').val('');
    $('#txtQuantity').val('');
    $('#txtPoSiteName').val('');
    $('#drpPRSiteAddress').val('');
    $('#PurchaseRequestId').val('');

    var button = document.getElementById("btnpurchaseRequest");
    if ($('#PurchaseRequestId').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreatePurchaseRequest'));
    offcanvas.show();
    $('#searchItemname').select2({
        maximumSelectionLength: 1,
        theme: 'bootstrap4',

        closeOnSelect: true,
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        placeholder: $(this).data('placeholder'),
        allowClear: Boolean($(this).data('allow-clear')),
        dropdownParent: $("#CreatePurchaseRequest")
    });

    $('#drpPRSiteAddress').select2({

        theme: 'bootstrap4',
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        placeholder: $(this).data('placeholder'),
        allowClear: Boolean($(this).data('allow-clear')),
        dropdownParent: $("#CreatePurchaseRequest")
    });
}

var PRForm;
function validateAndCreatePurchaseRequest() {
    PRForm = $("#purchaseRequestForm").validate({
        rules: {
            searchItemnameInput: "required",
            txtUnitType: "required",
            txtQuantity: "required",
            txtPoSiteName: "required",
        },
        messages: {
            searchItemnameInput: "Select Item!",
            txtUnitType: "Select UnitType!",
            txtQuantity: "Enter Quantity",
            txtPoSiteName: "Select Site",
        }
    });
    var isValid = true;


    if (isValid) {
        if ($("#PurchaseRequestId").val() == '') {
            CreatePurchaseRequest();
        }
        else {
            UpdatePurchaseRequestDetails();
        }
    }
}

function resetPRForm() {
    if (PRForm) {
        PRForm.resetForm();
    }
}

function EditPurchaseRequestDetails(PurchaseId) {

    siteloadershow();
    $.ajax({
        url: '/PurchaseMaster/DisplayPurchaseRequestDetails?PurchaseId=' + PurchaseId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {

            siteloaderhide();
            $('#changeName').html('Update PurchaseRequest');
            $('#PurchaseRequestId').val(response.pid);
            $('#txtUnitTypeHidden').val(response.unitTypeId);
            $('#prNo').val(response.prNo);
            $('#txtItemName').val(response.itemId);
            $('#searchItemnameInput').val(response.itemName);
            $('#txtQuantity').val(response.quantity);
            $('#txtPoSiteName').val(response.siteId);
            $('#txtUnitType').val(response.unitName);

            fn_getPRSiteDetail(response.siteId, function () {
                $('#drpPRSiteAddress').val(response.siteAddressId);
            });

            var button = document.getElementById("btnpurchaseRequest");
            if ($('#PurchaseRequestId').val() != '') {
                button.textContent = "Update";
            }
            var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreatePurchaseRequest'));
            resetPRForm()
            offcanvas.show();
            $('#searchItemname').select2({
                maximumSelectionLength: 1,
                theme: 'bootstrap4',
                width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
                placeholder: $(this).data('placeholder'),
                allowClear: Boolean($(this).data('allow-clear')),
                dropdownParent: $("#CreatePurchaseRequest")
            });


            $('#drpPRSiteAddress').select2({

                theme: 'bootstrap4',
                width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
                placeholder: $(this).data('placeholder'),
                allowClear: Boolean($(this).data('allow-clear')),
                dropdownParent: $("#CreatePurchaseRequest")
            });
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function UpdatePurchaseRequestDetails() {

    siteloadershow();
    if ($("#purchaseRequestForm").valid()) {

        var siteName = null;
        siteId = $("#SiteIdinPR").val();
        PRsiteId = $("#txtPoSiteName").val();
        if (PRsiteId != undefined) {
            siteName = PRsiteId;
        }
        else {
            siteName = siteId
        }
        var siteAddressId = $('#drpPRSiteAddress').val();
        var siteAddress = $('#drpPRSiteAddress option:selected').text();

        var objData = {
            SiteAddressId: siteAddressId,
            SiteAddress: siteAddress,
            Pid: $('#PurchaseRequestId').val(),
            CreatedBy: $('#txtcreatedby').val(),
            UnitTypeId: $('#txtUnitTypeHidden').val(),
            ItemId: $('#txtItemName').val(),
            ItemName: $('#searchItemnameInput').val(),
            SiteId: siteName,
            Quantity: $('#txtQuantity').val(),
            PrNo: $('#prNo').val(),
        }
        $.ajax({
            url: '/PurchaseMaster/UpdatePurchaseRequestDetails',
            type: 'post',
            data: objData,
            datatype: 'json',
            success: function (Result) {
                siteloaderhide();
                if (Result.code == 200) {
                    var offcanvasElement = document.getElementById('CreatePurchaseRequest');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {

                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllPurchaseRequestListTable();
                    toastr.success(Result.message);
                    siteloaderhide();
                } else {
                    toastr.error(Result.message);
                }
                siteloaderhide();
            },
        })
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}

function DeletePurchaseRequest(PurchaseId) {
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
                url: '/PurchaseMaster/DeletePurchaseRequest?PurchaseId=' + PurchaseId,
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
                        window.location = '/PurchaseMaster/PurchaseRequestListView';
                    })
                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete Purchaserequest!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Purchaserequest have no changes.!!😊',
                'error'
            );
        }
    });
}

function PurchaseRequestIsApproved(PurchaseId) {

    var isChecked = $('#flexSwitchCheckChecked_' + PurchaseId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to approve this purchaserequest?" : "Are you sure want to unapprove this purchaserequest?";

    Swal.fire({
        title: confirmationMessage,
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, enter it!",
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
                        title: isChecked ? "Approved!" : "Unapproved!",
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
                'Purchaserequest have no changes.!!😊',
                'error'
            );
        }
    });
}

function clearPOtextbox() {
    window.location.href = '/PurchaseMaster/CreatePurchaseOrder';
}



$(document).ready(function () {
    fn_autoselect('#txtUnitType', '/ItemMaster/GetAllUnitType', '#txtUnitTypeHidden');
    fn_autoselect('#searchItemnameInput', '/ItemMaster/GetItemNameList', '#txtItemName');
});

function GetPurchaseOrderList() {
    siteloadershow();
    var searchText = $('#txtPurchaseOrderSearch').val();
    var searchBy = $('#ddlPurchaseOrderSearchBy').val();

    $.get("/PurchaseMaster/PurchaseOrderListView", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {
            siteloaderhide();
            $("#PurchaseOrdertbody").html(result);
        })
        .fail(function (error) {
            siteloaderhide();
            toastr.error(error);
        });
}
function filterPurchaseOrderTable() {
    siteloadershow();
    var searchText = $('#txtPurchaseOrderSearch').val();
    var searchBy = $('#ddlPurchaseOrderSearchBy').val();

    $.ajax({
        url: '/PurchaseMaster/PurchaseOrderListView',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#PurchaseOrdertbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function sortPurchaseOrderTable() {
    siteloadershow();
    var sortBy = $('#ddlPurchaseOrderSortBy').val();
    $.ajax({
        url: '/PurchaseMaster/PurchaseOrderListView',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#PurchaseOrdertbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function EditPurchaseOrderDetails(Id) {
    siteloadershow();
    $.ajax({
        url: '/PurchaseMaster/DisplayPurchaseOrderDetails?Id=' + Id,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        success: function (response) {

            siteloaderhide();
            $('#purchaseorderid').val(response.id);
            $('#txtcompanyname').val(response.toCompanyId);
            $('#txtbillingAddress').val(response.billingAddress);
            $('#textPOPrefix').val(response.poid);
            $('#orderdate').val(response.date);
            $('#txtSuppliername').val(response.fromSupplierId);
            $('#searchItemname').val(response.itemId);
            $('#totalgst').val(response.totalGstamount);
            $('#cart-total').val(response.totalAmount);
            $('#txtdelivryschedule').val(response.deliveryShedule);
            $('#txtshippingAddress').val(response.shippingAddress);
            window.location.href = '/PurchaseMaster/CreatePurchaseOrder';
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function GetCompanyName() {
    $.ajax({
        url: '/Company/GetCompanyNameList',
        success: function (result) {
            if (result.length > 0) {

                var selectedValue = $('#txtcompanyname').find('option:first').val();
                $.each(result, function (i, data) {
                    if (data.companyId !== selectedValue) {
                        $('#txtcompanyname').append('<option value="' + data.companyId + '">' + data.companyName + '</option>');
                    }
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching company details:', error);
        }
    });
}


function getPONumber(CompanyId) {

    siteloadershow();
    $.ajax({
        url: '/PurchaseMaster/CheckPurchaseOrderNo?CompanyId=' + CompanyId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response.code == 200) {
                siteloaderhide();
                $('#textPOPrefix').val(response.data);
            } else {
                siteloaderhide();
                toastr.error('Empty response received.');
            }
        },
    });
}


$(document).ready(function () {
    $('#txtcompanyname').change(function () {
        getPONumber($(this).val());
    });
});

$(document).ready(function () {
    $('#txtcompanyname').change(function () {
        siteloadershow();
        var Company = $(this).val();
        $('#txtcompany').val(Company);
        $.ajax({
            url: '/Company/GetCompnaytById/?CompanyId=' + Company,
            type: 'GET',
            success: function (result) {
                siteloaderhide();
                $('#companybillingaddressDetails').empty().append(
                    '<div class="mb-2"><input type="text" class="form-control bg-light border-0" name="data[#].ShippingName" id="txtbillingcompanyname" value="' + result.companyName + '" readonly /></div>' +
                    '<div class="mb-2"><textarea class="form-control bg-light border-0" id="txtbillingAddress" name="data[#].ShippingAddress" rows="3" readonly style="height: 90px;">' + result.address + ', ' + result.area + ', ' + result.cityName + ', ' + result.stateName + ', ' + result.countryName + ', ' + result.pincode + '</textarea></div>'
                );
            },

        });
    });


    $('#sameAsBillingAddress').change(function () {
        var billingAddress = $('#companybillingaddressDetails textarea').val();
        var shippingNameInput = $('#shippingName');
        var shippingAddressInput = $('#txtshippingAddress');
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

function GetItemDetails() {

    $.ajax({
        url: '/ItemMaster/GetItemNameList',
        success: function (result) {


            $.each(result, function (i, data) {
                $('#searchItemname').append('<option value="' + data.itemId + '">' + data.itemName + '</option>');
            });
            $.each(result, function (i, data) {
                $('#Itemnamesearch').append('<option value="' + data.itemId + '">' + data.itemName + '</option>');
            });

        }
    });
}


$(document).ready(function () {
    $('#searchItemname').change(function () {
        var Text = $("#searchItemname Option:Selected").text();
        $("#txtItemName").val(Text);
    });
});
function GetSupplierDetails() {

    $.ajax({
        url: '/Supplier/GetSupplierNameList',
        success: function (result) {
            if (result.length > 0) {
                var selectedValue = $('#txtSuppliername').find('option:first').val();
                $.each(result, function (i, data) {
                    if (data.supplierId !== selectedValue) {
                        $('#txtSuppliername').append('<Option value=' + data.supplierId + '>' + data.supplierName + '</Option>')
                    }
                });
            }

        }
    });
}

function SerchItemDetailsById(Id, inputField) {
    siteloadershow();
    var qty = $(inputField).closest('.ac-item').find('.product-quantity').val();
    var Item = {
        ItemId: Id,
        Quantity: qty,
    }

    var form_data = new FormData();
    form_data.append("ITEMID", JSON.stringify(Item));


    $.ajax({
        url: '/ItemMaster/DisplayItemDetailsListById',
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


    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var yyyy = today.getFullYear();

    today = yyyy + '-' + mm + '-' + dd;
    $("#orderdate").val(today);
    $("#txtDeliverySchedule").val(today);

    function handleFocus(event, selector) {
        if (event.keyCode == 13 || event.keyCode == 9) {
            event.preventDefault();
            $(selector).focus();
        }
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

    $(document).on('input', '#txtproductamount', function () {
        var productRow = $(this).closest(".product");
        var productAmount = parseFloat($(this).val());

        productRow.find("#productamount").val(productAmount.toFixed(2));
        updateProductTotalAmount(productRow);
        updateTotals();

    }).on('keydown', '#txtproductamount', function (event) {
        var productRow = $(this).closest(".product");
        var gstFocus = productRow.find('#txtgst');
        handleFocus(event, gstFocus);
    });
});
function clearShippingAddressErrorMssage() {
    $("#spnshipping").text("");
}
function clearItemErrorMessage() {
    $("#spnitembutton").text("");
}

$("#AddShippingBtn").on("click", function () {
    clearShippingAddressErrorMssage();
});
$(document).on("click", "#addItemButton", function () {
    clearItemErrorMessage();
});
$(document).ready(function () {

    $("#CreatePOForm").validate({

        rules: {
            txtSuppliername: "required",
            companybillingaddressDetails: "required",
            txtcompanyname: "required",
            txtDeliverySchedule: {
                required: function (element) {
                    return !($("#txtDeliverySchedule").val() || $("input[name='txtDeliverySchedule']:checked").length > 0);
                }
            },
            txtContectPerson: "required",
            txtMobileNo: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 10
            },
            txtSuppliermobile: {
                required: true,
                digits: true,
                minlength: 10,
                maxlength: 10
            },
            txtSupplierAddress: "required",
        },
        messages: {
            txtSuppliername: "Select Supplier",
            companybillingaddressDetails: "Enter Billing Address",
            txtcompanyname: "Select Company Name",
            txtDeliverySchedule: "Please Enter PO Delivery Schedule",
            txtContectPerson: "Enter Contact Person Name",
            txtMobileNo: {
                required: "Please Enter Phone Number",
                digits: "Please enter a valid 10-digit phone number",
                minlength: "Phone number must be 10 digits long",
                maxlength: "Phone number must be 10 digits long"
            },
            txtSuppliermobile: {
                required: "Please Enter Phone Number",
                digits: "Please enter a valid 10-digit phone number",
                minlength: "Phone number must be 10 digits long",
                maxlength: "Phone number must be 10 digits long"
            },
            txtSupplierAddress: "Enter supplier address",
        }
    });
});

let editorPOInstance;
const poTermsElement = document.querySelector('#dvPOTerms');
const initialPOTerms = poTermsElement.getAttribute('data-terms');
var uploadUrl = '/PurchaseMaster/UploadTermsImage';
ClassicEditor
    .create(poTermsElement, {
        ckfinder: {
            uploadUrl: uploadUrl
        }
    })
    .then(editor => {
        editorPOInstance = editor;
        if (initialPOTerms) {
            editor.setData(initialPOTerms);
        }
    })
    .catch(error => {
        toastr.error(error);
    });

function InsertMultiplePurchaseOrderDetails() {
    siteloadershow();
    if ($("#CreatePOForm").valid()) {

        if ($('#addNewlink tr').length >= 1 && $('#dvshippingAdd .row.ac-invoice-shippingadd').length >= 1) {
            var orderDetails = [];
            var AddressDetails = [];
            $(".product").each(function () {
                var orderRow = $(this);
                var objData = {
                    ItemName: orderRow.find("#txtItemName").text(),
                    ItemId: orderRow.find("#txtItemId").val(),
                    UnitTypeId: orderRow.find("#txtPOUnitType_" + orderRow.find("#txtItemId").val()).val(),
                    Quantity: orderRow.find("#txtproductquantity").val(),
                    TotalPrice: orderRow.find("#txtproductamount").val(),
                    Price: orderRow.find("#txtproductamount").val(),
                    Gst: orderRow.find("#txtgstAmount").val(),
                    ItemTotal: orderRow.find("#txtproducttotalamount").val(),
                    Hsncode: orderRow.find("#txtHSNcode").val(),
                };
                orderDetails.push(objData);
            });

            $(".ShippingAddress").each(function () {
                var shippingAddress = $(this);
                var addressData = {
                    ShippingQuantity: shippingAddress.find("#shippingquantity").text(),
                    ShippingAddress: shippingAddress.find("#shippingaddress").text(),
                };
                AddressDetails.push(addressData);
            });
            var PORequest = {
                SiteId: $("#drpSiteNameList").val(),
                Poid: $("#textPOPrefix").val(),
                Date: $("#orderdate").val(),
                FromSupplierId: $("#txtSuppliername").val(),
                ToCompanyId: $("#txtcompanyname").val(),
                TotalAmount: $("#cart-total").val(),
                TotalGstamount: $("#totalgst").val(),
                BillingAddress: $("#companybillingaddressDetails").val(),
                DeliveryShedule: $("input[name='txtDeliverySchedule']:checked").length > 0 && $("input[name='txtDeliverySchedule']:checked").val() === "Immediate" ? "Immediate" : $("#txtDeliverySchedule").val(),
                ContactName: $("#txtContectPerson").val(),
                ContactNumber: $("#txtMobileNo").val(),
                CreatedBy: $("#createdbyid").val(),
                UnitTypeId: $("#UnitTypeId").val(),
                Terms: editorPOInstance.getData(),
                ItemOrderlist: orderDetails,
                ShippingAddressList: AddressDetails,
            }

            var form_data = new FormData();
            form_data.append("PODETAILS", JSON.stringify(PORequest));

            $.ajax({
                url: '/PurchaseMaster/InsertMultiplePurchaseOrderDetails',
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
                            window.location = '/PurchaseMaster/DisplayPODetails?POId=' + Result.data;
                        }, 2000);
                    }
                    else {
                        siteloaderhide();
                        toastr.error("There Is Some Problem in Your Request!");
                    }
                },
                error: function (xhr, status, error) {
                    siteloaderhide();
                    toastr.error('An error occurred while processing your request.');
                }
            });
        } else {
            siteloaderhide();
            if ($('#addNewlink tr').length == 0) {
                $("#spnitembutton").text("Please Select Product!");
            } else {
                $("#spnitembutton").text("");
            }
            if ($('#dvshippingAdd .row.ac-invoice-shippingadd').length == 0) {
                siteloaderhide();
                $("#spnshipping").text("Please Select Shipping Address!");
            } else {
                $("#spnshipping").text("");
            }
        }
    } else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}

function checkAndDisableAddButton() {
    if ($('#addNewlink tr').length > 1) {
        $('.add-address').prop('disabled', true);
    } else {
        $('.add-address').prop('disabled', false);
    }
}

$(document).ready(function () {
    var poRoleUser = $('#UserRoleinPO').val();
    var poSiteId = $('#txtPoSiteName').val();

    if (poRoleUser == 8) {
        fn_GetPOSiteAddressList(poSiteId);
    }
    else {
        $('#txtPoSiteName').change(function () {
            var Site = $(this).val();
            $('#txtPoSiteName').val(Site);
            fn_GetPOSiteAddressList(Site);
        });
    }
    $('#drpPOSiteAddress').select2({
        theme: 'bootstrap4',
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        placeholder: $(this).data('placeholder'),
        allowClear: Boolean($(this).data('allow-clear')),
        dropdownParent: $("#mdShippingAdd")
    });
    $(document).on('click', '#removeAddress', function () {
        $(this).closest('tr').remove();
        $('.add-address').prop('disabled', false);
    });
});

function fn_GetPOSiteAddressList(SiteId) {
    $.ajax({
        url: '/SiteMaster/DisplaySiteAddressList?SiteId=' + SiteId,
        success: function (result) {
            $('#drpPOSiteAddress').empty();
            $('#drpPOSiteAddress').append('<option value="">-- Select site address --</option>');
            $('#txtmdAddress').val('');

            if (Array.isArray(result)) {
                $.each(result, function (i, data) {
                    $('#drpPOSiteAddress').append('<option value="' + data.address + '">' + data.address + '</option>');
                });
            } else {
                $('#txtmdAddress').val(result.shippingAddress + ' , ' + result.shippingArea + ', ' + result.shippingCityName + ', ' + result.shippingStateName + ', ' + result.shippingCountryName);
            }

        }
    });
}
$('#drpPOSiteAddress').on('change', function () {
    var selectedPOAddress = $(this).val();
    $('#txtmdAddress').val(selectedPOAddress);
});
function AddShippingAddress() {
    siteloadershow();
    if ($("#ShippingAddressForm").valid()) {
        var quantity = $("#txtmdqty").val();
        var address = $("#txtmdAddress").val();
        var ItemQuantity = $("#TotalProductQuantity").text();
        var rowcount = $('#dvshippingAdd .row.ac-invoice-shippingadd').length + 1

        var totalQuantity = 0;

        $('#dvshippingAdd .row.ac-invoice-shippingadd').each(function () {
            totalQuantity += parseInt($(this).find('#shippingquantity').text().trim());
        });

        if ((totalQuantity + parseInt(quantity)) > ItemQuantity) {
            siteloaderhide();
            document.getElementById("spnShippingQuantity").innerText = "Enter Quantity is more than Item Total Quantity!";
            return;
        }
        checkAndDisableAddButton();
        totalQuantity += quantity;

        var isDuplicate = false;


        $('#dvshippingAdd .ac-invoice-shippingadd').each(function () {
            var existingAddress = $(this).find('#shippingaddress').text().trim();
            if (existingAddress === address) {
                isDuplicate = true;
                return false;
            }
        });
        siteloaderhide();
        if (!isDuplicate) {

            var newRow = '';
            if (rowcount == 1) {
                newRow = '<div class="row ac-invoice-shippingadd ShippingAddress">' +
                    '<div class="col-2 col-sm-2">' +
                    '<label id="lblshprownum1">' + rowcount + '</label>' +
                    '</div>' +
                    '<div class="col-5 col-sm-5">' +
                    '<p id="shippingaddress">' + address + '</p>' +
                    '</div>' +
                    '<div class="col-3 col-sm-3">' +
                    '<p id="shippingquantity">' + quantity + '</p>' +
                    '</div>' +
                    '</div>';
            } else {
                newRow = '<div class="row ac-invoice-shippingadd ShippingAddress">' +
                    '<div class="col-2 col-sm-2">' +
                    '<label id="lblshprownum1">' + rowcount + '</label>' +
                    '</div>' +
                    '<div class="col-5 col-sm-5">' +
                    '<p id="shippingaddress">' + address + '</p>' +
                    '</div>' +
                    '<div class="col-3 col-sm-3">' +
                    '<p id="shippingquantity">' + quantity + '</p>' +
                    '</div>' +
                    '<div class="col-2 col-sm-2">' +
                    '<a id="remove" class="btn text-primary" onclick="fn_removeShippingAdd(this)"><i class="lni lni-trash"></i></a>' +
                    '</div>' +
                    '</div>';
            }

            $('#dvshippingAdd').append(newRow);
            updateProductTotalAmount();
            updateTotals();
            updateRowNumbers();
        } else {
            siteloaderhide();
            toastr.warning("The selected address is already added.");
        }
    } else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}
$(document).ready(function () {
    $("#ShippingAddressForm").validate({
        rules: {
            txtmdAddress: "required",
        },
        messages: {
            txtmdAddress: "Select Site",
        }
    });
});

$(document).ready(function () {

    $('#txtSuppliername').change(function () {

        getSupplierDetails($(this).val());
    });
});

function getSupplierDetails(SupplierId) {

    if (SupplierId == "") {

        toastr.warning('Kindly select valid supplier');
        siteloaderhide();
    }
    else {


        siteloadershow();
        $.ajax({
            url: '/Supplier/DisplaySupplier?SupplierId=' + SupplierId,
            type: 'GET',
            contentType: 'application/json;charset=utf-8',
            dataType: 'json',
            success: function (response) {
                siteloaderhide();
                if (response) {
                    $('#txtSuppliermobile').val(response.mobile);
                    $('#txtSupplierGST').val(response.gstno);
                    $('#txtSupplierAddress').val(response.fullAddress);
                } else {
                    siteloaderhide();
                    toastr.error('Empty response received.');
                }
            },
        });
    }
}

$(document).ready(function () {

    $('#txtcompanyname').change(function () {

        getCompanyDetails($(this).val());
    });


});

function getCompanyDetails(CompanyId) {
    siteloadershow();
    $.ajax({
        url: '/Company/GetCompnaytById?CompanyId=' + CompanyId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            if (response) {
                siteloaderhide();
                $('#txtCompanyGstNo').val(response.gstno);
                $('#companybillingaddressDetails').val(response.fullAddress);
            } else {
                siteloaderhide();
                toastr.error('Empty response received.');
            }
        },
    });
}

function UpdateMultiplePurchaseOrderDetails() {
    siteloadershow();
    if ($("#CreatePOForm").valid()) {
        if ($('#dvshippingAdd .row.ac-invoice-shippingadd').length >= 1) {
            var AddressDetails = [];

            $(".ShippingAddress").each(function () {
                var shippingAddress = $(this);
                var addressData = {
                    ShippingQuantity: shippingAddress.find("#shippingquantity").text(),
                    ShippingAddress: shippingAddress.find("#shippingaddress").text(),
                };
                AddressDetails.push(addressData);
            });
            var PORequest = {
                Id: $("#RefPOid").val(),
                SiteId: $("#drpSiteNameList").val(),
                Poid: $("#textPOPrefix").val(),
                Date: $("#orderdate2").val(),
                FromSupplierId: $("#txtSuppliername").val(),
                ToCompanyId: $("#txtcompanyname").val(),
                TotalAmount: $("#cart-total").val(),
                TotalGstamount: $("#totalgst").val(),
                BillingAddress: $("#companybillingaddressDetails").val(),
                DeliveryShedule: $("input[name='txtDeliverySchedule']:checked").length > 0 && $("input[name='txtDeliverySchedule']:checked").val() === "Immediate" ? "Immediate" : $("#txtDeliverySchedule").val(),
                ContactName: $("#txtContectPerson").val(),
                ContactNumber: $("#txtMobileNo").val(),
                UnitTypeId: $("#UnitTypeId").val(),
                Terms: editorPOInstance.getData(),
                ShippingAddressList: AddressDetails,
            }
            var form_data = new FormData();
            form_data.append("PODETAILS", JSON.stringify(PORequest));
            $.ajax({
                url: '/PurchaseMaster/UpdateMultiplePurchaseOrderDetails',
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
                            window.location = '/PurchaseMaster/DisplayPODetails?POId=' + Result.data;
                        }, 2000);
                    }
                    else {
                        siteloaderhide();
                        toastr.error("There is some prolem in your request!");
                    }
                },
                error: function (xhr, status, error) {
                    siteloaderhide();
                    toastr.error('An error occurred while processing your request.');
                }
            });
        } else {
            siteloaderhide();
            if ($('#dvshippingAdd .row.ac-invoice-shippingadd').length == 0) {
                siteloaderhide();
                $("#spnshipping").text("Please Select Shipping Address!");
            } else {
                siteloaderhide();
                $("#spnshipping").text("");
            }
        }
    } else {
        siteloaderhide();
        Swal.fire({
            title: "Kindly fill all data fields",
            icon: 'warning',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'OK',
        })
    }
}

function DeletePODetails(POId) {

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
                url: '/PurchaseMaster/DeletePurchaseOrderDetails?POId=' + POId,
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
                        window.location = '/PurchaseMaster/POListView';
                    })
                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete purchaseorder!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'PO have no changes.!!😊',
                'error'
            );
        }
    });
}

function PopulateUnitTypeDropdown(itemId) {

    $.ajax({
        url: '/ItemMaster/GetAllUnitType',
        success: function (result) {
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


var count = 0;
function AddNewRow(Result) {

    var newProductRow = $(Result);
    var itemId = newProductRow.data('product-id');
    PopulateUnitTypeDropdown(itemId);
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
        updateTotals();
        updateRowNumbers();
    } else {
        siteloaderhide();
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


function updateProductTotalAmount(that) {

    var row = $(that);
    var productPrice = parseFloat(row.find("#txtproductamount").val());
    var quantity = parseInt(row.find("#txtproductquantity").val());
    var gst = parseFloat(row.find("#txtgst").val());
    var totalGst = (productPrice * quantity * gst) / 100;
    var totalAmount = productPrice * quantity + totalGst;

    row.find("#txtgstAmount").val(totalGst.toFixed(2));
    row.find("#txtproducttotalamount").val(totalAmount.toFixed(2));

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

    $(".product").each(function () {
        var row = $(this);
        var subtotal = parseFloat(row.find("#txtproductamount").val());
        var gst = parseFloat(row.find("#txtgstAmount").val());
        var totalquantity = parseFloat(row.find("#txtproductquantity").val());

        totalSubtotal += subtotal * totalquantity;
        totalGst += gst;
        totalAmount = totalSubtotal + totalGst;
        TotalItemQuantity += totalquantity;
    });

    $("#cart-subtotal").val(totalSubtotal.toFixed(2));
    $("#totalgst").val(totalGst.toFixed(2));
    $("#cart-total").val(totalAmount.toFixed(2));
    $("#TotalProductQuantity").text(TotalItemQuantity);
    $("#TotalProductPrice").html(totalSubtotal.toFixed(2));
    $("#TotalProductGST").html(totalGst.toFixed(2));
    $("#TotalProductAmount").html(totalAmount.toFixed(2));
}
function removeItem(btn) {
    $(btn).closest("tr").remove();
    updateRowNumbers();
    updateTotals();
}


let viewobj;
var value, invoices_list = localStorage.getItem("invoices-list"),
    options = localStorage.getItem("option"),
    invoice_no = localStorage.getItem("invoice_no"),
    invoices = JSON.parse(invoices_list);


function GetPOList() {
    siteloadershow();
    var searchText = $('#txtPOSearch').val();
    var searchBy = $('#ddlPOSearchBy').val();

    $.get("/PurchaseMaster/POListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {
            siteloaderhide();

            $("#PurchaseOrderListbody").html(result);
        })
        .fail(function (error) {
            siteloaderhide();
            toastr.error(error);
        });
}

function GetAllItemDetailsList() {
    siteloadershow();
    var searchText = $('#mdProductSearch').val();

    $.get("/PurchaseMaster/GetAllItemDetailsList", { searchText: searchText })
        .done(function (result) {

            siteloaderhide();
            $("#mdlistofItem").html(result);
        })
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

function filterPOTable() {
    siteloadershow();
    var searchText = $('#txtPOSearch').val();
    var searchBy = $('#ddlPOSearchBy').val();

    $.ajax({
        url: '/PurchaseMaster/POListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#PurchaseOrderListbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}
function sortPOTable() {
    siteloadershow();
    var sortBy = $('#ddlPOSortBy').val();
    $.ajax({
        url: '/PurchaseMaster/POListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#PurchaseOrderListbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
        }
    });
}

function printDiv() {
    var printContents = document.getElementById('displayPODetail').innerHTML;
    var originalContents = document.body.innerHTML;
    document.body.innerHTML = printContents;
    window.print();
    document.body.innerHTML = originalContents;
}

function toggleRadio() {
    const dateInput = document.getElementById('txtDeliverySchedule');
    const radioImmediate = document.getElementById('txtImmediate');
    const radioDate = document.getElementById('txtradiodate');

    if (radioImmediate.checked) {
        dateInput.disabled = true;
    } else if (radioDate.checked) {
        dateInput.disabled = false;
    }
}


function fn_OpenAddproductmodal() {
    if ($("#drpSiteNameList").val() == "") {
        $("#frmdrpdashbord").css("border", "2px solid red");
        $("#siteErrorMesssage").html("Select Site").css("color", "red");
    } else {
        $('#mdProductSearch').val('');
        $('#mdPoproductModal').modal('show');
    }
}
$("#drpSiteNameList").change(function () {
    if ($(this).val() != "") {
        $("#frmdrpdashbord").css("border", "");
        $("#siteErrorMesssage").html("").css("color", "");
    }
});