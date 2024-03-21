AllCompanyTable();
function AllCompanyTable() {

    var searchText = $('#txtSearch').val();
    var searchBy = $('#ddlSearchBy').val();

    $.get("/Company/GetAllCompanyDetails", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {debugger
            $("#Companytbody").html(result);
        })
        .fail(function (error) {
            console.error(error);
        });
}

function filterCompanyTable() {

    var searchText = $('#txtSearch').val();
    var searchBy = $('#ddlSearchBy').val();

    $.ajax({
        url: '/Company/GetAllCompany',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            $("#Companytbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function sortCompanyTable() {
    var sortBy = $('#ddlSortBy').val();
    $.ajax({
        url: '/Company/GetAllCompany',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            $("#Companytbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}
function AddCompany() {debugger

    var objData = {
        CompanyName: $('#txtCompanyName').val(),
        Gstno: $('#txtGstNo').val(),
        PanNo: $('#txtPanNo').val(),
        Address: $('#txtAddress').val(),
        Area: $('#txtArea').val(),
        CityId: $('#ddlCity').val(),
        StateId: $('#ddlState').val(),
        Country: $('#ddlCountry').val(),
        Pincode: $('#txtPincode').val(),

    }
    $.ajax({
        url: '/Company/AddCompany',
        type: 'post',
        data: objData,
        datatype: 'json',
        success: function (Result) {debugger

            Swal.fire({
                title: Result.message,
                icon: 'success',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK'
            }).then(function () {
                window.location = '/Company/CreateCompany';
            });
        },
    })
}
function ClearTextBox() {
    resetErrorMessages();
    $('#txtCompanyid').val('');
    $('#txtCompanyName').val('');
    $('#txtGstNo').val('');
    $('#txtPanNo').val('');
    $('#txtAddress').val('');
    $('#txtArea').val('');
    $('#txtCityId').val('');
    $('#txtStateId').val('');
    $('#txtCountry').val('');
    $('#txtPincode').val('');
    var button = document.getElementById("btncompany");
    if ($('#txtCompanyid').val() == '') {
        button.textContent = "Create";
    }
    var offcanvas = new bootstrap.Offcanvas(document.getElementById('createCompany'));
    offcanvas.show();
}
function GetCompnaytById(CompanyId) {debugger

    $.ajax({
        url: '/Company/GetCompnaytById?CompanyId=' + CompanyId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {debugger

            $('#txtCompanyid').val(response.companyId);
            $('#txtCompanyName').val(response.companyName);
            $('#txtGstNo').val(response.gstno);
            $('#txtPanNo').val(response.panNo);
            $('#txtAddress').val(response.address);
            $('#txtArea').val(response.area);
            $('#txtCityId').val(response.cityId);
            $('#txtStateId').val(response.stateId);
            $('#ddlCountry').val(response.country);
            $('#txtPincode').val(response.pincode);
            var button = document.getElementById("btncompany");
            resetErrorMessages();
            if ($('#txtCompanyid').val() != '') {
                button.textContent = "Update";
            }
            var offcanvas = new bootstrap.Offcanvas(document.getElementById('createCompany'));
            offcanvas.show();
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}
function SelectCompanyDetails(CompanyId, element) {debugger

    $('.row.ac-card').removeClass('active');
    $(element).closest('.row.ac-card').addClass('active');
    $('.ac-detail').removeClass('d-none');
    $.ajax({
        url: '/Company/GetCompnaytById?CompanyId=' + CompanyId,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {debugger
            if (response) {
                $('#dspCompanyid').val(response.companyId);
                $('#dspCompanyName').val(response.companyName);
                $('#dspGstNo').val(response.gstno);
                $('#dspPanNo').val(response.panNo);
                $('#dspAddress').val(response.address);
                $('#dspArea').val(response.area);
                $('#dspCity').val(response.cityId);
                $('#dspState').val(response.stateId);
                $('#dspCountry').val(response.country);
                $('#dspPincode').val(response.pincode);
            } else {
                console.log('Empty response received.');
            }
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}
function UpdateCompany() {debugger

    var objData = {
        CompanyId: $('#txtCompanyid').val(),
        CompanyName: $('#txtCompanyName').val(),
        Gstno: $('#txtGstNo').val(),
        PanNo: $('#txtPanNo').val(),
        Address: $('#txtAddress').val(),
        Area: $('#txtArea').val(),
        CityId: $('#txtCityId').val(),
        StateId: $('#txtStateId').val(),
        Country: $('#txtCountry').val(),
        Pincode: $('#txtPincode').val(),
    }
    $.ajax({
        url: '/Company/UpdateCompany',
        type: 'post',
        data: objData,
        datatype: 'json',
        success: function (Result) {debugger

            Swal.fire({
                title: Result.message,
                icon: 'success',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'OK'
            }).then(function () {
                window.location = '/Company/CreateCompany';
            });
        },
    })

}
function validateAndCreateCompany() {

    resetErrorMessages();

    var companyName = document.getElementById("txtCompanyName").value.trim();
    var gstno = document.getElementById("txtGstNo").value.trim();
    var panNo = document.getElementById("txtPanNo").value.trim();
    var address = document.getElementById("txtAddress").value.trim();
    var area = document.getElementById("txtArea").value.trim();
    var cityId = document.getElementById("ddlCity").value.trim();
    var stateId = document.getElementById("ddlState").value.trim();
    var country = document.getElementById("ddlCountry").value.trim();
    var pincode = document.getElementById("txtPincode").value.trim();


    var isValid = true;


    if (companyName === "") {
        document.getElementById("spnCompanyName").innerText = "Company Name is required.";
        isValid = false;
    }


    if (gstno === "") {
        document.getElementById("spnGstNo").innerText = "Gst No is required.";
        isValid = false;
    }


    if (panNo === "") {
        document.getElementById("spnPanNo").innerText = "Pan No is required.";
        isValid = false;
    }


    if (address === "") {
        document.getElementById("spnAddress").innerText = "Address is required.";
        isValid = false;
    }

    if (area === "") {
        document.getElementById("spnArea").innerText = "Area is required.";
        isValid = false;
    }

    if (cityId === "") {
        document.getElementById("spnCityId").innerText = "CityId is required.";
        isValid = false;
    }

    if (stateId === "") {
        document.getElementById("spnStateId").innerText = "StateId is required.";
        isValid = false;
    }

    if (country === "") {
        document.getElementById("spnCountry").innerText = "Country is required.";
        isValid = false;
    }

    if (pincode === "") {
        document.getElementById("spnPincode").innerText = "Pincode is required.";
        isValid = false;
    }

    

    if (isValid) {
        if ($("#txtCompanyid").val() == '') {
            AddCompany();
        }
        else {
            UpdateCompany()
        }
    }
}

function resetErrorMessages() {
    document.getElementById("spnCompanyName").innerText = "";
    document.getElementById("spnGstNo").innerText = "";
    document.getElementById("spnPanNo").innerText = "";
    document.getElementById("spnAddress").innerText = "";
    document.getElementById("spnArea").innerText = "";
    document.getElementById("spnCityId").innerText = "";
    document.getElementById("spnStateId").innerText = "";
    document.getElementById("spnCountry").innerText = "";
    document.getElementById("spnPincode").innerText = "";
}






