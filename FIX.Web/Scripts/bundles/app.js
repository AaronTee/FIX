(function ($) {

    //Number format
    var decimalPlaces = 2;
    Number.prototype.format = function (c, d, t) {
        var n = isNaN(this) ? 0 : this,
            c = isNaN(c = Math.abs(c)) ? decimalPlaces : c,
            d = d == undefined ? "." : d,
            t = t == undefined ? "," : t,
            s = n < 0 ? "-" : "",
            i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
            j = (j = i.length) > 3 ? j % 3 : 0;
        return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
    };

    String.prototype.format = function (c, d, t) {
        var on = Number(this),
            _c = (isNaN(this)) ? 0 : on;
        return on.format(c, d, t);
    }

    ////Add validator method.
    //$.validator.addMethod("moreEqualZero", function (value, element) {
    //    if (value >= 0) return true;
    //    else false;
    //});

})(jQuery);
$(function() {

    /*======================= Default ========================*/
    /* Assign a class for select2 plugin */
    if ($.fn.select2) {
        $('.searchable-dropdown').select2();
    }

    if ($.fn.datepicker) {
        $.fn.datepicker.defaults.format = "dd-M-yyyy";
        $.fn.datepicker.defaults.autoclose = true;
        $.fn.datepicker.defaults.startDate = "-5y";
        $.fn.datepicker.defaults.endDate = "+5y";
        $.fn.datepicker.defaults.maxViewMode = 2;
        $.fn.datepicker.defaults.orientation = 'bottom';
        $('.datepicker').datepicker();
    }

    /*======================= Events ========================*/
    /* Format number automatically with class */
    var numericInputElem = $("input.num, input.dec, input.int");

    numericInputElem.on('focus', function (e) {
        $(this).select();
    });
    numericInputElem.on('change', function (e) {
        var $this = $(this);
        var v = 0;
        var checker = Number($this.val());
        if (!isNaN(checker)) v = checker;
        $this.val(v.format());
        $this.valid();
    });

    /*======================= Preload execution ========================*/
    /* Set each validator onkeyup to false. */
    var $forms = $("form");
    $forms.each(function () {
        var validator = $(this).data('validator');
        if (validator) {
            $(validator).each(function () {
                this.settings.onkeyup = false;
            });
        }
    });

    /* Format (especially for number field) on load */
    numericInputElem.each(function () {
        $(this).trigger('change');
    })

})
//template
(function ($) {

    var $_this;
    var _options;
    var $_okBtn;
    var $_title;
    var $_body;

    $.fn.modalCreator = function (options) {
        var $_this = $(this);
        var pre_id = $_this.attr('id');

        _options = $.extend({}, $.fn.modalCreator.defaults, options);

        this.make = function (material) {
            _options = $.extend({}, _options, material)
            buildModal();
        }

        this.show = function () {
            $_this.modal('show');
        }

        
        this.hide = function () {
            reset();
            $_this.modal('hide');
        }

        function reset() {
            $_okBtn.removeClass(_options.confirmButtonStyle);
            $_okBtn.prop("disabled", false);
        }

        function buildModal() {

            //Assigning text to modal
            $_okBtn.html(_options.confirmText);
            $_title.html(_options.header);
            $_body.html(_options.body);

            //Assigning style
            $_okBtn.addClass(_options.confirmButtonStyle);

            //Registering events
            $_this.off().on('click', '.btn-ok', disableButton)
                        .on("click", ".btn-ok", _options.onConfirm);
        }

        function disableButton() {
            $_okBtn.prop("disabled", true);
        }

        function init() {
            $.ajax({
                type: "GET",
                url: GetPath('htmlloader/modal'),
                success: function (result) {
                    $_this.append(result);
                    $_this.appendTo('body');
                    var modalElem = $_this.find('.modal');
                    $_this = $(modalElem);
                    $_this.attr('id', pre_id);
                    $_okBtn = $_this.find('.btn-ok') || {};
                    $_title = $_this.find('.modal-title') || {};
                    $_body = $_this.find('.modal-body') || {};
                }
            });
        }

        //Registering event
        $_this.on('hidden.bs.modal', function () {
            reset();
        });

        init();
        return this;
    }

    $.fn.modalCreator.defaults = {
        header: 'Confirmation',
        body: 'Are you sure you want to proceed?',
        confirmText: 'Confirm',
        confirmButtonStyle: 'btn-success',
        closeText: 'Close',
        onConfirm: function () { }
    }

})(jQuery);



































////template
//(function ($) {

//    var methods = {
//        init: function (options) {
//            return this.each(function () {
//                var _options = $.extend({}, $.fn.modalCreator.defaults, options)
//                $.get(GetPath('htmlloader/modal'), function (result) {

//                    var _id = $this.id;

//                    $this.append(result);

//                    //Set all settings before we replace user defined div.
//                    var modalElem = $this.find('.modal');

//                    $(this) = $(modalElem);

//                    //Assigning options
//                    $(this).attr('id', _id);
//                    $(this).find('.btn-ok').html(_options.confirmText);
//                    $(this).find('.modal-title').html(_options.header);
//                    $(this).find('.modal-body').html(_options.body);

//                    //Registering events
//                    $(this).on('click', '.btn-ok', _options.onConfirm);
//                });
//                $(this).data('modalCreator', _options);
//            });
            
//        },
//        show: function () {
//            this.modal('show');
//        },// IS
//        hide: function () { },// GOOD
//        update: function (content) { }// !!!
//    };

//    $.fn.modalCreator = function (method) {
//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on jQuery.modalCreator.');
//        }
//    };

//    $.fn.modalCreator.defaults = {
//        header: 'Confirmation',
//        body: 'Are you sure you want to proceed?',
//        confirmText: 'Confirm',
//        closeText: 'Close',
//        onConfirm: function () { },

//    };

















    //$.modalCreator = function (elem, options) {

    //    var $_modal;

    //    var _id = elem.id;
    //    var $this = $(this);
    //    var $elem = $(elem);
    //    var content = $elem.html();
        
    //    $elem.empty();

    //    $.get(GetPath('htmlloader/modal'), function (result) {
    //        $elem.append(result);

    //        //Set all settings before we replace user defined div.
    //        var modalElem = $elem.find('.modal');

    //        $_modal = $(modalElem);

    //        //Assigning options
    //        $_modal.attr('id', _id);
    //        $_modal.data($elem.data());
    //        $_modal.find('.btn-ok').html(options.confirmText);
    //        $_modal.find('.modal-title').html(options.header);
    //        $_modal.find('.modal-body').html(options.body);

    //        //Registering events
    //        $_modal.on('click', '.btn-ok', options.onConfirm);

    //        $elem.replaceWith($_modal);
    //    });

    //    this.show = function () {
    //        this.modal('toggle');
    //    }

    //    return this;
    //};

    //$.fn.modalCreator = function (options) {

    //    if ($.modalCreator[options]) {
    //        return $.modalCreator[method].apply(this, Array.prototype.slice.call(arguments, 1));
    //    } else if (typeof options === 'object' || !options) {
    //        return this.each(function () {
    //            // if plugin has not already been attached to the element
    //            if ($(this).data('modalCreator') == undefined) {
    //                var opts = $.extend({}, $.fn.modalCreator.defaults, options);
    //                var _inst = new $.modalCreator(this, opts);
    //                $(this).data('modalCreator', _inst);
    //            }
    //        });
    //    } else {
    //        $.error('Method ' + options + ' does not exist on MyPlugin');
    //    }
    //}

    //$.fn.modalCreator.defaults = {
    //    header: 'Confirmation',
    //    body: 'Are you sure you want to proceed?',
    //    confirmText: 'Confirm',
    //    closeText: 'Close',
    //    onConfirm: function () { },

    //};

//})(jQuery);
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
$(function () {

var toggler = $('.navbar-toggle');
var sidebar = $('.' + toggler.attr("data-target"));
var dimTarget = $('.' + toggler.attr("data-dim-target"));
var bodyCanvas = $('.body-canvas');

$('form').addClass('form-horizontal');

/* Handler window onResize */
$(window).resize(function () {

    var width = $(window).width();

    /* ENTER MOBILE VIEW */
    //if (width < 768) {
    //    slideInSideBar();
    //}

    /* EXIT MOBILE VIEW */
    if (width > 767) {
        slideOutSideBar();
    }
});

bodyCanvas.on("click", function () {
    slideOutSideBar();
});

function slideInSideBar() {
    sidebar.addClass('responsive-sidebar');
    sidebar.addClass('responsive');
    dimTarget.addClass('dim');

}

function slideOutSideBar() {
    sidebar.removeClass('responsive');
    sidebar.removeClass('responsive-sidebar');
    dimTarget.removeClass('dim');
}

toggler.click(function () {

    if (sidebar.hasClass('responsive')) {
        slideOutSideBar();
    } else {
        slideInSideBar();
    }

});

//end navbar

//start submenu
var parentMenu = $(".nav-menu.has-child");
var childMenu = $(".nav-menu.has-child .sidebar-subnav");

parentMenu.click(function () {
    var $this = $(this);

    $this.toggleClass('expanding');
    childMenu.toggleClass('show');
});

    //--------------disable overlay scroll body------------
$(function () {
    var _overlay = document.getElementsByClassName('scrollable')[0];
    var _clientY = null; // remember Y position on touch start

    if (!_overlay) return;

    _overlay.addEventListener('touchstart', function (event) {
        if (event.targetTouches.length === 1) {
            // detect single touch
            _clientY = event.targetTouches[0].clientY;
        }
    }, false);

    _overlay.addEventListener('touchmove', function (event) {
        if (event.targetTouches.length === 1) {
            // detect single touch
            disableRubberBand(event);
        }
    }, false);

    function disableRubberBand(event) {
        var clientY = event.targetTouches[0].clientY - _clientY;

        if (_overlay.scrollTop === 0 && clientY > 0) {
            // element is at the top of its scroll
            event.preventDefault();
        }

        if (isOverlayTotallyScrolled() && clientY < 0) {
            //element is at the top of its scroll
            event.preventDefault();
        }
    }

    function isOverlayTotallyScrolled() {
        // https://developer.mozilla.org/en-US/docs/Web/API/Element/scrollHeight#Problems_and_solutions
        return _overlay.scrollHeight - _overlay.scrollTop <= _overlay.clientHeight;
    }

    $('input[data-val="true"], select[data-val="true"]')
        .closest(".form-group")
        .children("label")
        .addClass("required-field");
})

    


//--------- disable touch hover------------
var touch = 'ontouchstart' in document.documentElement
            || navigator.maxTouchPoints > 0
            || navigator.msMaxTouchPoints > 0;

if (touch) { // remove all :hover stylesheets
    try { // prevent exception on browsers not supporting DOM styleSheets properly
        for (var si in document.styleSheets) {
            var styleSheet = document.styleSheets[si];
            if (!styleSheet.rules) continue;

            for (var ri = styleSheet.rules.length - 1; ri >= 0; ri--) {
                if (!styleSheet.rules[ri].selectorText) continue;

                if (styleSheet.rules[ri].selectorText.match(':hover')) {
                    styleSheet.deleteRule(ri);
                }
            }
        }
    } catch (ex) { }
}
});
