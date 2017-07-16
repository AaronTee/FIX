$(document).ready(function () {
    $("table[data-search-placeholder]").each(function () {
        $(this).parents("div.bootstrap-table")
            .find("input[placeholder='Search']")
            .attr("placeholder", $(this).data("search-placeholder"));
    });
});

/* Note: Dependent on ActionLink class @ ListViewModel.cs */
function LinkFormatter(value, row, index) {

    var html = '';

    value.forEach(function (data) {
        html += "<a class='btn btn-actionlink' href='" + data.Url + "'>" + data.Name + "</a>";
    });

    return html;
}

/* Note: Dependent on ActionTag class @ ListViewModel.cs */
function actionFormatter(value, row, index) {

    var html = '';

    value.forEach(function (data) {
        html += "<a class='btn btn-actionlink action' data-action='" + data.Action + "'>" + data.Name + "</a>"
    });

    return html;
}

/* Note: Dependent on ActionTag class @ ListViewModel.cs */
function percentageFormatter(value, row, index) {

    return (value * 100).format() + '%';
}


function currencyFormatter(value, row, index) {

    return value.format();
}