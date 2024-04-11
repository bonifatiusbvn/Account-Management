AllItemInWordListTable();
GetItemDetails();
GetUnitType();



function AllItemInWordListTable() {
    var searchText = $('#txtItemInWordSearch').val();
    var searchBy = $('#ItemInWordSearchBy').val();

    $.get("/ItemInWord/ItemInWordListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {
            $("#itemInWordtbody").html(result);
        })
        .fail(function (error) {
            console.error(error);
        });
}

function filterItemInWordTable() {

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
            $("#itemInWordtbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}


function sortItemInWordTable() {
    var sortBy = $('#ItemInWordSortBy').val();
    $.ajax({
        url: '/ItemInWord/ItemInWordListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            $("#itemInWordtbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function SelectItemInWordDetails(InwordId, element) {
    $('tr').removeClass('active');
    $(element).closest('tr').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/ItemInWord/DisplayItemInWordDetails?InwordId=' + InwordId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
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
                    $('#dspDocumentName').empty();
                    $('.holder').empty();
                    response.documentLists.forEach(function (document) {
                        var imageURL = "/Content/InWordDocument/" + document.documentName;
                        var imageElement = $('<img>').attr('src', imageURL).addClass('document-image').css({
                            'height': '100px', 'width': '100px'
                        });
                        $('.holder').append(imageElement);
                    });
                }

            } else {
                console.log('Empty response received.');
            }
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}
//function GetItemDetails() {
//    $.ajax({
//        url: '/ItemMaster/GetItemNameList',
//        success: function (result) {
//            $.each(result, function (i, data) {
//                $('#txtItemId').append('<Option value=' + data.itemId + '>' + data.itemName + '</Option>')
//            });
//        }
//    });
//}
function GetItemDetails() {

    $.ajax({
        url: '/ItemMaster/GetItemNameList',
        success: function (result) {
            debugger

            $('#txtItemId').empty();

            $.each(result, function (i, data) {
                $('#txtItemId').append('<option value="' + data.itemId + '">' + data.itemName + '</option>');
            });
            debugger
            $('#txtItemId').select2({
                placeholder: "Select Product Name",
                allowClear: true
            });
        }
    });
}
$(document).ready(function () {
    $('#txtItemId').change(function () {
        var Text = $("#txtItemId Option:Selected").text();
        /*var txtItemName = $(this).val();*/
        $("#txtItemName").val(Text);
    });
});
function GetUnitType() {

    $.ajax({
        url: '/ItemMaster/GetAllUnitType',
        success: function (result) {debugger
            $.each(result, function (i, data) {
                $('#txtUnitType').append('<Option value=' + data.unitId + '>' + data.unitName + '</Option>')
            });
        }
    });
}
function AddItemInWordDetails() {

    var formData = new FormData();
    formData.append("UnitTypeId", $("#txtUnitType").val());
    formData.append("ItemId", $("#txtItemId").val());
    formData.append("Item", $("#txtItemName").val());
    formData.append("Quantity", $("#txtQuantity").val());
    formData.append("SiteId", $("#txtSiteid").val());
    formData.append("DocumentName", $("#txtDocument")[0].files[0]);
    formData.append("CreatedBy", $("#txtCreatedBy").val());
    formData.append("VehicleNumber", $("#txtVehicleNumber").val());
    formData.append("ReceiverName", $("#txtReceiverName").val());
    $.ajax({
        url: '/ItemInWord/AddItemInWordDetails',
        type: 'post',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (Result) {

            Swal.fire({
                title: Result.message,
                icon: 'success',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK'
            }).then(function () {
                window.location = '/ItemInWord/ItemInWord';
            });
        },
    })
}
function ClearItemInWordTextBox() {
    resetErrorsMessages();
    $('#txtItemName').val('');
    $('#txtItemInWordid').val('');
    $('#txtItemId').val('');
    $('#txtUnitType').val('');
    $('#txtQuantity').val('');
    $('#txtDocument').val('');
    $('#txtVehicleNumber').val('');
    $('#txtReceiverName').val('');
    $('#addNewImage').empty();
    var button = document.getElementById("btnitemInWord");
    if ($('#txtItemInWordid').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('CreateItemInWord'));
    offcanvas.show();
}
function validateAndCreateItemInWord() {

    resetErrorsMessages();
    var UnitTypeId = document.getElementById("txtUnitType").value.trim();
    var ItemName = document.getElementById("txtItemId").value.trim();
    var Quantity = document.getElementById("txtQuantity").value.trim();
    var VehicleNumber = document.getElementById("txtVehicleNumber").value.trim();
    var ReceiverName = document.getElementById("txtReceiverName").value.trim();

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


    if (Quantity === "") {
        document.getElementById("spnQuantity").innerText = "Quantity is required.";
        isValid = false;
    }


    if (VehicleNumber === "") {
        document.getElementById("spnVehicleNumber").innerText = "Vehicle Number is required.";
        isValid = false;
    }


    if (ReceiverName === "") {
        document.getElementById("spnReceiverName").innerText = "Receiver Name is required.";
        isValid = false;
    }

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
    document.getElementById("spnUnitType").innerText = "";
    document.getElementById("spnItemName").innerText = "";
    document.getElementById("spnQuantity").innerText = "";
    document.getElementById("spnVehicleNumber").innerText = "";
    document.getElementById("spnReceiverName").innerText = "";
}
function EditItemInWordDetails(InwordId) {
    debugger
    $.ajax({
        url: '/ItemInWord/DisplayItemInWordDetails?InwordId=' + InwordId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            $('#txtItemInWordid').val(response.inwordId);
            $('#txtUnitType').val(response.unitTypeId);
            $('#txtItemId').val(response.itemId);
            $('#txtItemName').val(response.item);
            $('#txtQuantity').val(response.quantity);
            $("#txtVehicleNumber").val(response.vehicleNumber);
            $("#txtReceiverName").val(response.receiverName);
            var date = response.date;
            var formattedDate = date.substr(0, 10);
            $('#txtIteminwordDate').val(formattedDate);

            if (response.documentLists && response.documentLists.length > 0) {
                var documentNames = "";
                $.each(response.documentLists, function (index, document) {
                    documentNames += document.documentName + ";";
                    var newRow = "<tr class='DocumentName' id='itemInWordId_" + document.id + "'><td><div id='showimages'><span onclick='CancelImage()' class='close btn-sm btn-outline-danger' style='margin-left:205px;'>&times;</span><img src='/Content/InWordDocument/" + document.documentName + "' class='displayImage' style='height: 230px; width: 230px; border: solid black;'></div></td></tr>";
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
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

function UpdateItemInWordDetails() {
    debugger
    var documentFile = $("#txtDocument")[0].files[0];
    var documentName = null;
    if (documentFile == undefined) {
        documentName = $("#txtDocumentName").val();
    }
    else {
        documentName = documentFile.name;
    }
    var objData = {
        InwordId: $('#txtItemInWordid').val(),
        UnitTypeId: $('#txtUnitType').val(),
        ItemId: $('#txtItemId').val(),
        Item: $('#txtItemName').val(),
        Quantity: $('#txtQuantity').val(),
        VehicleNumber: $("#txtVehicleNumber").val(),
        ReceiverName: $("#txtReceiverName").val(),
        DocumentName: documentName,
    };
    var form_data = new FormData();
    form_data.append("ITEMINWORD", JSON.stringify(objData));

    $.ajax({
        url: '/ItemInWord/UpdateItemInWordDetails',
        type: 'post',
        data: form_data,
        datatype: 'json',
        contentType: false,
        processData: false,
        success: function (Result) {

            Swal.fire({
                title: Result.message,
                icon: 'success',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK'
            }).then(function () {
                window.location = '/ItemInWord/ItemInWord';
            });
        },
    })
}
function DeleteItemInWord(InwordId) {
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
                url: '/ItemInWord/DeleteItemInWord?InwordId=' + InwordId,
                type: 'POST',
                dataType: 'json',
                success: function (Result) {

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
                    Swal.fire({
                        title: "Can't Delete ItemInWord!",
                        icon: 'warning',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK',
                    }).then(function () {
                        window.location = '/ItemInWord/ItemInWord';
                    })
                }
            })
        } else if (result.dismiss === Swal.DismissReason.cancel) {

            Swal.fire(
                'Cancelled',
                'Item In Word Have No Changes.!!😊',
                'error'
            );
        }
    });
}
function ItemInWordIsApproved(InwordId) {

    var isChecked = $('#flexSwitchCheckChecked_' + InwordId).is(':checked');
    var confirmationMessage = isChecked ? "Are you sure want to Approve this Item In Word?" : "Are you sure want to UnApprove this Item In Word?";

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
            formData.append("InwordId", InwordId);

            $.ajax({
                url: '/ItemInWord/ItemInWordIsApproved?InwordId=' + InwordId,
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
                        window.location = '/ItemInWord/ItemInWord';
                    });
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Item In Word Have No Changes.!!😊',
                'error'
            ).then(function () {
                window.location = '/ItemInWord/ItemInWord';
            });;
        }
    });
}

var additionalFiles = [];
function CancelImage() {
    $(document).on('click', '.close', function () {
        var row = $(this).closest('tr');
        var documentName = row.find('img').attr('src').split('/').pop();
        row.remove();

        // Remove the document name from the txtDocumentName input
        var currentDocumentNames = $("#txtDocumentName").val().split(';');
        var updatedDocumentNames = currentDocumentNames.filter(function (name) {
            return name !== documentName;
        });
        $("#txtDocumentName").val(updatedDocumentNames.join(';'));
    });
}
function showpictures() {
    debugger
    var files = $("#txtDocument")[0].files;
    if (files.length > 0) {
        for (var i = 0; i < files.length; i++) {
            const file = files[i];
            additionalFiles.push(file); 
            let reader = new FileReader();
            reader.onload = function (event) {
                var newRow = "<tr class='DocumentName'><td><div><span class='close btn-sm btn-outline-danger' style='margin-left:205px;'>&times;</span><img src='" + event.target.result + "' class='displayImage' style='height: 230px; width: 230px; border: solid black;'></div></td></tr>";
                $("#addNewImage").append(newRow);
            }
            reader.readAsDataURL(file);
        }
    }
}

function InsertMultipleItemInWordDetails() {
    var ItemInWordRequest = {
        UnitTypeId: $("#txtUnitType").val(),
        ItemId: $("#txtItemId").val(),
        Item: $("#txtItemName").val(),
        Quantity: $("#txtQuantity").val(),
        SiteId: $("#txtSiteid").val(),
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
                Swal.fire({
                    title: Result.message,
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(function () {
                    window.location = '/ItemInWord/ItemInWord';
                });
            }
            else {
                Swal.fire({
                    title: Result.message,
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(function () {
                    window.location = '/ItemInWord/ItemInWord';
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

function UpdateMultipleItemInWordDetails() {
    debugger
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
    };

    var form_data = new FormData();
    form_data.append("UpdateItemInWord", JSON.stringify(UpdateItemInWord));

    if (additionalFiles.length > 0) {
        for (var i = 0; i < additionalFiles.length; i++) {
            debugger
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
                Swal.fire({
                    title: Result.message,
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(function () {
                    window.location = '/ItemInWord/ItemInWord';
                });
            } else {
                Swal.fire({
                    title: Result.message,
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(function () {
                    window.location = '/ItemInWord/ItemInWord';
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






