AllItemInWordListTable();
GetItemDetails();
GetUnitType();
GetSiteList();
toggleSiteList();

function toggleSiteList() {
    var roleuserId = $('#userRoleId').val();
    if (roleuserId == 3) {
        document.getElementById("siteSection").style.display = "block";
    } else {
        document.getElementById("siteSection").style.display = "none";
    }
}

function AllItemInWordListTable() {

    var searchText = $('#txtItemInWordSearch').val();
    var searchBy = $('#ItemInWordSearchBy').val();

    $.get("/ItemInWord/ItemInWordListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {

            $("#itemInWordtbody").html(result);
        })
        .fail(function (error) {
            siteloaderhide();
            toastr.error(error);
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
            toastr.error(error);
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
            toastr.error(error);
        }
    });
}

function SelectItemInWordDetails(InwordId, element) {
    siteloadershow();
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

function GetItemDetails() {

    $.ajax({
        url: '/ItemMaster/GetItemNameList',
        success: function (result) {

            $('#txtItemId').empty();
            $('#txtItemId').append('<option value="">--Select Item --</option>');
            $.each(result, function (i, data) {
                $('#txtItemId').append('<option value="' + data.itemId + '">' + data.itemName + '</option>');
            });
        }
    });
}
$(document).ready(function () {
    $('#txtItemId').change(function () {
        var Text = $("#txtItemId Option:Selected").text();

        $("#txtItemName").val(Text);
    });
});
function GetUnitType() {

    $.ajax({
        url: '/ItemMaster/GetAllUnitType',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#txtUnitType').append('<Option value=' + data.unitId + '>' + data.unitName + '</Option>')
            });

        }
    });
}

function GetSiteList() {

    $.ajax({
        url: '/SiteMaster/GetSiteNameList',
        success: function (result) {
            $.each(result, function (i, data) {
                $('#siteNameList').append('<Option value=' + data.siteId + '>' + data.siteName + '</Option>')
            });
        }
    });
}

function ClearItemInWordTextBox() {
    if ($("#txtSiteid").val() == "") {
        Swal.fire({
            title: "Kindly select site on dashboard.",
            icon: 'warning',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'OK',
        });
    }
    else {
        resetErrorsMessages();
        $('#txtItemName').val('');
        $('#txtItemInWordid').val('');
        $('#txtItemId').val('');
        $('#txtUnitType').val('');
        $('#txtQuantity').val('');
        $('#txtDocument').val('');
        $('#txtVehicleNumber').val('');
        $('#txtReceiverName').val('');
        $('#siteNameList').val('');
        $('#addNewImage').empty();
        var button = document.getElementById("btnitemInWord");
        if ($('#txtItemInWordid').val() == '') {
            button.textContent = "Create";
        }
        var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreateItemInWord'));
        offcanvas.show();
        $('#txtItemId').select2({
            theme: 'bootstrap4',
            width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
            placeholder: $(this).data('placeholder'),
            allowClear: Boolean($(this).data('allow-clear')),
            dropdownParent: $("#CreateItemInWord")
        });
        $('#txtUnitType').select2({
            theme: 'bootstrap4',
            width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
            placeholder: $(this).data('placeholder'),
            allowClear: Boolean($(this).data('allow-clear')),
            dropdownParent: $("#CreateItemInWord")
        });
    }
}

var ItemInwordForm;
function validateAndCreateItemInWord() {
    ItemInwordForm = $("#itemInWordForm").validate({
        rules: {
            txtUnitType: "required",
            txtQuantity: "required",
            txtReceiverName: "required",
            txtVehicleNumber: "required",
            txtItemId: "required"
        },
        messages: {
            txtUnitType: "Enter UnitType",
            txtQuantity: "Enter Quantity",
            txtReceiverName: "Enter ReceiverName",
            txtVehicleNumber: "Enter VehicleNumber",
            txtItemId: "select item"
        }
    })
    var isValid = true;

    if (isValid) {
        if ($("#txtItemInWordid").val() == '') {
            InsertMultipleItemInWordDetails();
        }
        else {
            UpdateMultipleItemInWordDetails();
        }
    }

}

function resetErrorsMessages() {
    if (ItemInwordForm) {
        ItemInwordForm.resetForm();
    }
}
function EditItemInWordDetails(InwordId) {
    siteloadershow();
    $('#addNewImage').empty();
    $.ajax({
        url: '/ItemInWord/DisplayItemInWordDetails?InwordId=' + InwordId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            siteloaderhide();
            $('#txtItemInWordid').val(response.inwordId);
            $('#txtUnitType').val(response.unitTypeId);
            $('#txtItemId').val(response.itemId);
            $('#txtItemName').val(response.item);
            $('#txtQuantity').val(response.quantity);
            $("#txtVehicleNumber").val(response.vehicleNumber);
            $("#txtReceiverName").val(response.receiverName);
            $("#siteNameList").val(response.siteId);
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

            var button = document.getElementById("btnitemInWord");
            if ($('#txtItemInWordid').val() != '') {
                button.textContent = "Update";
            }

            var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreateItemInWord'));
            resetErrorsMessages();
            offcanvas.show();
            $('#txtItemId').select2({
                theme: 'bootstrap4',
                width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
                placeholder: $(this).data('placeholder'),
                allowClear: Boolean($(this).data('allow-clear')),
                dropdownParent: $("#CreateItemInWord")
            });
            $('#txtUnitType').select2({
                theme: 'bootstrap4',
                width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
                placeholder: $(this).data('placeholder'),
                allowClear: Boolean($(this).data('allow-clear')),
                dropdownParent: $("#CreateItemInWord")
            });
        },
        error: function (xhr, status, error) {
            siteloaderhide();
            toastr.error(xhr.responseText);
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
        return name !== documentName;
    });

    $("#txtDocumentName").val(updatedDocumentNames.join(';'));
}
function removenewaddImage() {

    $(document).on('click', '.img-remove', function () {
        var row = $(this).closest('.DocumentName');
        var documentName = row.find('img').data('document');
        row.remove();

        additionalFiles = additionalFiles.filter(function (item) {
            return item.name !== documentName;
        });
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
            reader.onload = (function (fileName) {
                return function (event) {
                    var documentName = fileName;
                    var newRow = "<div class='col-6 col-sm-6 DocumentName'><div><div id='showimages'><div onclick='removenewaddImage()' class='img-remove'><div class='font-22'><i class='lni lni-close'></i></div></div><img src='" + event.target.result + "' class='displayImage' data-document='" + documentName + "'></div></div></div>";
                    $("#addNewImage").append(newRow);
                };
            })(file.name);
            reader.readAsDataURL(file);
            additionalFiles.push(file);
        }
    }
}

function InsertMultipleItemInWordDetails() {
    siteloadershow();
    if ($("#itemInWordForm").valid()) {
        var siteId = null;
        var RoleUserId = $('#userRoleId').val();
        if (RoleUserId == 3) {
            siteId = $("#siteNameList").val();
        }
        else {
            siteId = $("#txtSiteid").val();
        }
        var ItemInWordRequest = {
            UnitTypeId: $("#txtUnitType").val(),
            ItemId: $("#txtItemId").val(),
            Item: $("#txtItemName").val(),
            Quantity: $("#txtQuantity").val(),
            SiteId: siteId,
            CreatedBy: $("#txtCreatedBy").val(),
            VehicleNumber: $("#txtVehicleNumber").val(),
            ReceiverName: $("#txtReceiverName").val(),
            Date: $("#txtIteminwordDate").val(),
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
                if (Result.code == 200) {
                    var offcanvasElement = document.getElementById('CreateItemInWord');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {

                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllItemInWordListTable();
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
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}

function UpdateMultipleItemInWordDetails() {
    siteloadershow();
    if ($("#itemInWordForm").valid()) {
        var siteId = null;
        var RoleUserId = $('#userRoleId').val();
        if (RoleUserId == 3) {
            siteId = $("#siteNameList").val();
        }
        else {
            siteId = $("#txtSiteid").val();
        }
        var documentName = $("#txtDocumentName").val();
        var UpdateItemInWord = {
            InwordId: $('#txtItemInWordid').val(),
            UnitTypeId: $("#txtUnitType").val(),
            ItemId: $("#txtItemId").val(),
            Item: $("#txtItemName").val(),
            Quantity: $("#txtQuantity").val(),
            VehicleNumber: $("#txtVehicleNumber").val(),
            ReceiverName: $("#txtReceiverName").val(),
            Date: $("#txtIteminwordDate").val(),
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
                    var offcanvasElement = document.getElementById('CreateItemInWord');
                    var offcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);

                    if (offcanvas) {
                        offcanvas.hide();
                    } else {

                        offcanvas = new bootstrap.Offcanvas(offcanvasElement);
                        offcanvas.hide();
                    }

                    AllItemInWordListTable();
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
    else {
        siteloaderhide();
        toastr.error("Kindly fill all details");
    }
}