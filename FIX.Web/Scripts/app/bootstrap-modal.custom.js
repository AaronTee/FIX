//template
(function ($) {

    $.modal = function (rowElem) {

        var tr = $(rowElem.closest('tr'));
        var _backStepTracker;

        var init = function () {
            _backStepTracker = 0;
        }

        var repositioning = function (pos) {
            while (pos != 0) {
                if (pos < 0) { //Move down
                    if (tr.prev()[0]) _backStepTracker++;
                    tr.insertBefore(tr.prev());

                    pos++;
                }
                else { //Move up
                    if (tr.next()[0]) _backStepTracker--;
                    tr.insertAfter(tr.next());
                    pos--;
                }
            }
        }

        this.moveRow = function (pos) {
            repositioning(pos);
        }

        this.reset = function () {
            repositioning(_backStepTracker);
        }

        init();
        return this;
    };

    $.fn.modal = function () {
        return this.each(function () {
            // if plugin has not already been attached to the element
            if ($(this).data('RowMover') == undefined) {
                var _inst = new $.RowMover(this);
                $(this).data('RowMover', _inst);
            }
        });
    }
    //$.fn.RowMover.defaults = {};

})(jQuery);