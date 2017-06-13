$.fn.dtTable = function(config) {

    var $table = $(this) || $('#dtTable');
    var _dtTable;

    var checkBoxEnabled = config.enableCheckbox;

    if ($.fn.DataTable.isDataTable($table)) _dtTable = $table.DataTable();
    else
    _dtTable = $table.DataTable({
        'dom': 'frtip',
        'columnDefs': function () {
            var colDef =
            [
                { 'orderable': false, 'targets': ['nosort'] }
            ];
            if (checkBoxEnabled) {
                colDef.push({
                    targets: [0],
                    className: 'checkbox',
                    title: '<input type="checkbox" id="checkall">',
                    orderable: false,
                    render: function (data, type, row) {
                        if (checkBoxEnabled) return '<input type="checkbox" class="listing-checkbox">';
                    }
                });
            }
        },
        'order': [],
        language: {
            search: '_INPUT_',
            searchPlaceholder: 'Search',
            'paginate': {
                'previous': '<',
                'next': '>'
            },
            select: {
                rows: ''
            }
        },
        fnDrawCallback: function (settings) {
            redrawCheckAll();
        }
    });

    function redrawCheckAll() {
        //managing the 'Select all' checkbox
        // everytime the _dtTable is drawn, it checks if all the 
        //checkboxes are checked and if they are, then the select all
        // checkbox in the _dtTable header is selected
        var allChecked = true;
        $table.find('tbody tr').each(function () {
            $(this).find(':checkbox[class=listing-checkbox]').each(function () {
                if (!$(this).is(':checked')) {
                    allChecked &= false;
                }
            });
        });
        $('#checkall').prop('checked', allChecked);
    }


    _dtTable.on('click', '.listing-checkbox', function () {
        var trNode = $(this).closest('tr');
        if (this.checked) {
            _dtTable.row(trNode).select();
        } else {
            _dtTable.row(trNode).deselect();
        }
        onCheckBoxClicked();
    });

    function onCheckBoxClicked() {
        if (_dtTable.rows('.selected').any()) {
            $('.mode-delete').addClass('show');
            $('.mode-create').removeClass('show');

        } else {
            $('.mode-create').addClass('show');
            $('.mode-delete').removeClass('show');
        }
        var count = _dtTable.rows('.selected').count();
        $('#item-delete-count').html(count);

        redrawCheckAll();
    }

    this.getSelectedData = function() {
        return _dtTable.rows({ selected: true }).data();
    }

    _dtTable.on('click', '#checkall', function () {
        var checked;
        checked = this.checked;

        /* Toggle Checkbox */
        _dtTable.rows({ page: 'current' }).every(function (rowIdx, tableLoop, rowLoop) {
            _dtTable.row(rowIdx).nodes().to$().find('input').prop('checked', checked);

            if (checked) {
                _dtTable.row(rowIdx).select();
            } else {
                _dtTable.row(rowIdx).deselect();
            }
        });

        onCheckBoxClicked();
    });

    return this;
}

$.extend($.fn.dataTableExt.oStdClasses, {
    'sFilterInput': 'search-filter',
});