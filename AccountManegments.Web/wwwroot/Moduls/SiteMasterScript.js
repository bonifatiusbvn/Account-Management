AllSiteListTable();
function AllSiteListTable() {

    var searchText = $('#txtSearch').val();
    var searchBy = $('#ddlSearchBy').val();

    $.get("/SiteMaster/SiteListAction", { searchBy: searchBy, searchText: searchText })
        .done(function (result) {


            $("#Sitetbody").html(result);
        })
        .fail(function (error) {
            console.error(error);
        });
}

function filterTable() {

    var searchText = $('#txtSearch').val();
    var searchBy = $('#ddlSearchBy').val();

    $.ajax({
        url: '/SiteMaster/SiteListAction',
        type: 'GET',
        data: {
            searchText: searchText,
            searchBy: searchBy
        },
        success: function (result) {
            $("#Sitetbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}

function sortTable() {
    var sortBy = $('#ddlSortBy').val();
    $.ajax({
        url: '/SiteMaster/SiteListAction',
        type: 'GET',
        data: {
            sortBy: sortBy
        },
        success: function (result) {
            $("#Sitetbody").html(result);
        },
        error: function (xhr, status, error) {

        }
    });
}