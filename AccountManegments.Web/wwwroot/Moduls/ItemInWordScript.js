AllItemInWordListTable();
GetSiteList();
ClearItemInWordTextBox();
GetSupplierList();

function AllItemInWordListTable() {
    var searchText = $('#txtItemInWordSearch').val();
    var searchBy = $('#ItemInWordSearchBy').val();

    $.get("/ItemInWord/ItemInWordListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {
            clearCreateInwardtext();
            $("#itemInWordtbody").html(result);
        })
        .fail(function (error) {
            siteloaderhide();

        });
}

function filterItemInWordTable() {
    siteloadershow();
    var searchText = $('#txtItemInWordSearch').val();
    var searchBy = $('#ItemInWordSearchBy').val();

    $.ajax({
        url: '/ItemInWord/ItemInWordListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            siteloaderhide();
            $("#itemInWordtbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}


function sortItemInWordTable() {
    siteloadershow();
    var sortBy = $('#ItemInWordSortBy').val();
    $.ajax({
        url: '/ItemInWord/ItemInWordListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            siteloaderhide();
            $("#itemInWordtbody").html(result);
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}

function SelectItemInWordDetails(InwordId, element) {
    siteloadershow();
    $('#inwardheadingtxt').text('Item Inward Details');
    $('#inwardInfo').removeClass('d-none');
    $('#addInWordInfo').addClass('d-none');
    $('#btnitemInWord').hide();
    $('#updateitemInWord').hide();
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/ItemInWord/DisplayItemInWordDetails?InwordId=' + InwordId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            if (response) {
                $('#dspInwordId').val(response.inwordId);
                $('#dspSiteId').val(response.siteName);
                $('#dspItemId').val(response.itemId);
                $('#dspItem').val(response.item);
                $('#dspUnitName').val(response.unitName);
                $('#dspQuantity').val(response.quantity);
                $('#dspIsApproved').prop('checked', response.isApproved);
                $('#dspVehicleNumber').val(response.vehicleNumber);
                $('#dspReceiverName').val(response.receiverName);
                var date = new Date(response.date);
                var formattedDate = date.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' });
                $('#dspDate').val(formattedDate);
                if (response.documentName != null) {
                    $('#dspDocumentName').attr("src", "/Content/InWordDocument/" + response.documentName).css({
                        'height': '250px',
                        'width': '250px'
                    });;
                } else {
                    siteloaderhide();
                    $('#dspDocumentName').empty();
                    $('.holder').empty();
                    response.documentLists.forEach(function (document) {
                        var imageURL = "/Content/InWordDocument/" + document.documentName;
                        var imageElement = $('<img>').attr('src', imageURL).addClass('document-image').css({
                            'height': '250px', 'width': '250px'
                        });
                        $('.holder').append(imageElement);
                    });
                }

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

$(document).ready(function () {
    fn_autoselect('#txtUnitType', '/ItemMaster/GetAllUnitType', '#txtUnitTypeHidden');
    fn_autoselect('#searchItemnameInput', '/ItemMaster/GetItemNameList', '#txtItemName');
});
function GetSiteList() {
    $.ajax({
        url: '/SiteMaster/GetSiteNameList',
        method: 'GET',
        success: function (result) {
            var unitTypes = result.map(function (data) {
                return {
                    label: data.siteName,
                    value: data.siteId
                };
            });


            $("#siteNameList").autocomplete({
                source: unitTypes,
                minLength: 0,
                select: function (event, ui) {

                    event.preventDefault();
                    $("#siteNameList").val(ui.item.label);
                    $("#siteNameListHidden").val(ui.item.value);

                },
                focus: function () {
                    return false;
                }
            }).focus(function () {
                $(this).autocomplete("search", "");
            });
            $("#siteNameList").on('input', function () {
                if ($(this).val().trim() === "") {
                    $("#siteNameListHidden").val("");
                }
            });
        },
        error: function (err) {
            console.error("Failed to fetch site list: ", err);
        }
    });
}

function GetSupplierList() {
    $.ajax({
        url: '/Supplier/GetSupplierNameList',
        method: 'GET',
        success: function (result) {
            var supplier = result.map(function (data) {
                return {
                    label: data.supplierName,
                    value: data.supplierId
                };
            });
            $("#inwardSupplierList").autocomplete({
                source: supplier,
                minLength: 0,
                select: function (event, ui) {

                    event.preventDefault();
                    $("#inwardSupplierList").val(ui.item.label);
                    $("#inwardSupplierListHidden").val(ui.item.value);

                },
                focus: function () {
                    return false;
                }
            }).focus(function () {
                $(this).autocomplete("search", "");
            });
            $("#inwardSupplierList").on('input', function () {
                if ($(this).val().trim() === "") {
                    $("#inwardSupplierListHidden").val("");
                }
            });
        },
        error: function (err) {
            console.error("Failed to fetch supplier list: ", err);
        }
    });
}

function ClearItemInWordTextBox() {
    clearCreateInwardtext();
    $('#inwardheadingtxt').text('Create Item Inword');
    $('#addInWordInfo').removeClass('d-none');
    $('#inwardInfo').addClass('d-none');
    $('#btnitemInWord').show();
    $('#updateitemInWord').hide();
    var siteId = $("#txtInwardSiteid").val();
    var siteName = $("#txtInWardSiteName").val();
    $("#siteNameList").val(siteName);
    $("#siteNameListHidden").val(siteId);
    $("#inwardSupplierListHidden").val('');
    $("#inwardSupplierList").val('');
    $("#spnInWardSiteName").hide();
}

function clearCreateInwardtext() {
    resetErrorsMessages();
    $('#searchItemnameInput').val('');
    $('#txtItemInWordid').val('');
    $('#txtItemName').val('');
    $('#txtUnitType').val('');
    $('#txtQuantity').val('');
    $('#txtDocument').val('');
    $('#txtVehicleNumber').val('');
    $('#txtReceiverName').val('');
    $('#siteNameList').val('');
    $('#siteNameListHidden').val('');
    $('#addNewImage').empty();
}

var ItemInwordForm;
$(document).ready(function () {

    ItemInwordForm = $("#itemInWordForm").validate({
        rules: {
            txtUnitType: "required",
            searchItemnameInput: "required",
            inwardSupplierList: "required",
            txtQuantity: "required",
            txtReceiverName: "required",
            txtVehicleNumber: "required",
            txtItemId: "required",
        },
        messages: {
            txtUnitType: "Enter UnitType",
            searchItemnameInput: "Enter Product",
            inwardSupplierList: "Enter Supplier",
            txtQuantity: "Enter Quantity",
            txtReceiverName: "Enter ReceiverName",
            txtVehicleNumber: "Enter VehicleNumber",
            txtItemId: "select item",
        }
    })
});

function resetErrorsMessages() {
    if (ItemInwordForm) {
        ItemInwordForm.resetForm();
    }
}
function EditItemInWordDetails(InwordId) {
    siteloadershow();
    resetErrorsMessages();
    $('#addNewImage').empty();
    $.ajax({
        url: '/ItemInWord/DisplayItemInWordDetails?InwordId=' + InwordId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            $('#inwardheadingtxt').text('Edit Item Inword');
            $('#addInWordInfo').removeClass('d-none');
            $('#inwardInfo').addClass('d-none');
            $('#btnitemInWord').hide();
            $('#updateitemInWord').show();
            $('#txtItemInWordid').val(response.inwordId);
            $('#txtUnitType').val(response.unitName);
            $('#txtUnitTypeHidden').val(response.unitTypeId);
            $('#txtItemName').val(response.itemId);
            $('#searchItemnameInput').val(response.item);
            $('#txtQuantity').val(response.quantity);
            $("#txtVehicleNumber").val(response.vehicleNumber);
            $("#txtReceiverName").val(response.receiverName);
            $("#siteNameList").val(response.siteName);
            $("#siteNameListHidden").val(response.siteId);
            $("#inwardSupplierListHidden").val(response.supplierId);
            $("#inwardSupplierList").val(response.supplierName);
            additionalFiles.length = 0;
            var date = response.date;
            var formattedDate = date.substr(0, 10);
            $('#txtIteminwordDate').val(formattedDate);

            if (response.documentLists && response.documentLists.length > 0) {
                var documentNames = "";
                $.each(response.documentLists, function (index, document) {
                    documentNames += document.documentName + ";";
                    var newRow = "<div class='col-6 col-sm-6 DocumentName' id='itemInWordId_" + document.id + "'><div><div id='showimages'><div onclick='CancelImage(\"" + document.documentName + "\")' class='img-remove'><div class='font-22'><i class='lni lni-close'></i></div></div><img src='/Content/InWordDocument/" + document.documentName + "' class='displayImage'></div></div></div>";
                    $("#addNewImage").append(newRow);
                });
                $("#txtDocumentName").val(documentNames);
            }
        },
        error: function (xhr, status, error) {
            siteloaderhide();

        }
    });
}

function DeleteItemInWord(InwordId) {
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
                url: '/ItemInWord/DeleteItemInWord?InwordId=' + InwordId,
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
                        window.location = '/ItemInWord/ItemInWord';
                    })
                },
                error: function () {
                    siteloaderhide();
                    toastr.error("Can't delete iteminward!");
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Iteminward have no changes.!!😊',
                'error'
            );
        }
    });
}
function ItemInWordIsApproved(InwordId) {

    var isChecked = $('#flexSwitchCheckChecked_' + InwordId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to approve this iteminward?" : "Are you sure want to unapprove this iteminward?";

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
            formData.append("InwordId", InwordId);

            $.ajax({
                url: '/ItemInWord/ItemInWordIsApproved?InwordId=' + InwordId,
                type: 'Post',
                contentType: 'application/json;charset=utf-8;',
                dataType: 'json',
                success: function (Result) {
                    siteloaderhide();
                    Swal.fire({
                        title: isChecked ? "Approved!" : "Unapproved!",
                        text: Result.message,
                        icon: "success",
                        confirmButtonClass: "btn btn-primary w-xs mt-2",
                        buttonsStyling: false
                    }).then(function () {
                        window.location = '/ItemInWord/ItemInWord';
                    });
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Iteminward have no changes.!!😊',
                'error'
            ).then(function () {
                window.location = '/ItemInWord/ItemInWord';
            });;
        }
    });
}

var additionalFiles = [];
function CancelImage(documentName) {
    $("#addNewImage").find("img[src$='" + documentName + "']").closest('.DocumentName').remove();

    var currentDocumentNames = $("#txtDocumentName").val().split(';');
    var updatedDocumentNames = currentDocumentNames.filter(function (name) {
        return name.trim() !== documentName.trim();
    });
    $("#txtDocumentName").val(updatedDocumentNames.join(';'));
}
function removenewaddImage(element) {
    var row = $(element).closest('.DocumentName');
    var documentName = row.find('img').data('document');
    var fileIndex = row.find('img').data('file-index');
    row.remove();

    if (fileIndex !== undefined) {
        additionalFiles.splice(fileIndex, 1); 
    }

    additionalFiles.forEach((file, index) => {
        $('#addNewImage .DocumentName img[data-file-index="' + (index + 1) + '"]').data('file-index', index);
    });
}
function showpictures() {
    var files = $("#txtDocument")[0].files;
    if (files.length > 0) {
        if ($("#addNewImage .DocumentName").length + files.length > 5) {
            toastr.error("You can only add a maximum of 5 images.");
            return;
        }
        for (var i = 0; i < files.length; i++) {
            const file = files[i];
            let reader = new FileReader();
            reader.onload = (function (file) {
                return function (event) {
                    var newRow = "<div class='col-6 col-sm-6 DocumentName'><div><div id='showimages'><div onclick='removenewaddImage(this)' class='img-remove'><div class='font-22'><i class='lni lni-close'></i></div></div><img src='" + event.target.result + "' class='displayImage' data-document='" + file.name + "' data-file-index='" + additionalFiles.length + "'></div></div></div>";
                    $("#addNewImage").append(newRow);
                    additionalFiles.push(file); 
                };
            })(file);
            reader.readAsDataURL(file);
        }
    }
}

function InsertMultipleItemInWordDetails() {
    siteloadershow();
    if ($("#itemInWordForm").valid()) {
        var siteId = $("#siteNameListHidden").val();
        if (siteId == undefined)
        {
            siteId = $("#siteNameIdHidden").val();
        }
        if (siteId != "") {
            var ItemInWordRequest = {
                UnitTypeId: $("#txtUnitTypeHidden").val(),
                ItemId: $("#txtItemName").val(),
                Item: $("#searchItemnameInput").val(),
                Quantity: $("#txtQuantity").val(),
                SiteId: siteId,
                CreatedBy: $("#txtCreatedBy").val(),
                VehicleNumber: $("#txtVehicleNumber").val(),
                ReceiverName: $("#txtReceiverName").val(),
                Date: $("#txtIteminwordDate").val(),
                SupplierId: $("#inwardSupplierListHidden").val(),
            };

            var form_data = new FormData();
            form_data.append("InWordsDetails", JSON.stringify(ItemInWordRequest));

            for (var i = 0; i < additionalFiles.length; i++) {
                form_data.append("DocDetails", additionalFiles[i]);
            }

            $.ajax({
                url: '/ItemInWord/InsertMultipleItemInWordDetail',
                type: 'POST',
                data: form_data,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (Result) {
                    siteloaderhide();
                    if (Result.code == 200) {
                        AllItemInWordListTable();
                        ClearItemInWordTextBox();
                        toastr.success(Result.message);
                    } else {
                        toastr.error(Result.message);
                    }

                },
                error: function (xhr, status, error) {
                    siteloaderhide();
                    toastr.error('An error occurred while processing your request.');
                }
            });
        }
        else
        {
            siteloaderhide();
            $("#spnInWardSiteName").show().text('select the site');
            toastr.error("Kindly select the site");
        }
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}

function UpdateMultipleItemInWordDetails() {
    siteloadershow();
    if ($("#itemInWordForm").valid()) {
        var siteId = $("#siteNameListHidden").val();
        if (siteId == undefined) {
            siteId = $("#siteNameIdHidden").val();
        }
        if (siteId != "") {
            var documentName = $("#txtDocumentName").val();

            var UpdateItemInWord = {
                InwordId: $('#txtItemInWordid').val(),
                UnitTypeId: $("#txtUnitTypeHidden").val(),
                ItemId: $("#txtItemName").val(),
                Item: $("#searchItemnameInput").val(),
                Quantity: $("#txtQuantity").val(),
                VehicleNumber: $("#txtVehicleNumber").val(),
                ReceiverName: $("#txtReceiverName").val(),
                Date: $("#txtIteminwordDate").val(),
                SupplierId: $("#inwardSupplierListHidden").val(),
                DocumentName: documentName,
                SiteId: siteId,
            };

            var form_data = new FormData();
            form_data.append("UpdateItemInWord", JSON.stringify(UpdateItemInWord));

            if (additionalFiles.length > 0) {
                for (var i = 0; i < additionalFiles.length; i++) {
                    form_data.append("DocDetails", additionalFiles[i]);
                }
            }

            $.ajax({
                url: '/ItemInWord/UpdatetMultipleItemInWordDetails',
                type: 'POST',
                data: form_data,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (Result) {
                    if (Result.code == 200) {
                        AllItemInWordListTable();
                        ClearItemInWordTextBox();
                        toastr.success(Result.message);
                    } else {
                        toastr.error(Result.message);
                    }
                    siteloaderhide();
                },
                error: function (xhr, status, error) {
                    siteloaderhide();
                    toastr.error('An error occurred while processing your request.');
                }
            });
        }
        else
        {
            siteloaderhide();
            $("#spnInWardSiteName").show().text('select the site');
            toastr.error("Kindly select the site");
        }
    }
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}