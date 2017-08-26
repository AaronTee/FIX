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
        html += "<a class='btn btn-actionlink " + data.ClassName + "' href='" + data.Url + "'>" + data.Name + "</a>";
    });

    return html;
}

/* Note: Dependent on ActionTag class @ ListViewModel.cs */
function actionFormatter(value, row, index) {

    if (!(value instanceof Array) || !value || value.length < 1) return;
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

function runningFormatter(value, row, index) {
    var tableOptions = $(row).find('table').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}

function hasFooterRowStyle(row, index) {
    var tableOptions = $('table[data-row-style="hasFooterRowStyle"]').bootstrapTable('getOptions');
    var totalRow = tableOptions.totalRows;
    var totalPage = tableOptions.totalPages || 1;
    var currentPage = tableOptions.pageNumber || 1;
    var currentRowCount = tableOptions.data.length - 1 || 0;
    
    var isFirstPage = totalRow == 0;
    var isLastPage = totalPage == currentPage;
    if ((index == currentRowCount && isLastPage) || isFirstPage) {
        return {
            classes: 'footer success'
        };
    }

    return {};
}