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
window.isMobile = function () {
    var check = false;
    (function (a) { if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true; })(navigator.userAgent || navigator.vendor || window.opera);
    return check;
};

//Toplevel
if ($.validator) {
    $.validator.setDefaults({ ignore: [] });
}

$(function () {

    /*======================= Default ========================*/
    /* Assign a class for select2 plugin */
    if ($.fn.select2) {
        $('.searchable-dropdown').select2();
    }

    //Datepicker
    var d = $('.datepicker');
    if (!isMobile()) {
        if ($.fn.datepicker) {
            $.fn.datepicker.defaults.format = "dd-M-yyyy";
            $.fn.datepicker.defaults.autoclose = true;
            $.fn.datepicker.defaults.startDate = "-5y";
            $.fn.datepicker.defaults.endDate = "+5y";
            $.fn.datepicker.defaults.maxViewMode = 2;
            $.fn.datepicker.defaults.orientation = 'bottom';
            d.datepicker();
            var t;
            $(document).on(
                'DOMMouseScroll mousewheel scroll touchmove',
                'body',
                function () {
                    window.clearTimeout(t);
                    t = window.setTimeout(function () {
                        d.datepicker('place')
                    }, 250);
                    d.datepicker('place');
                }
            );
        }
    } else {

        if (Pikaday && pikadayResponsive) {
            d.each(function (index, element) {
                pikadayResponsive(this, {
                    format: "DD-MMM-YYYY",
                    outputFormat: "DD-MMM-YYYY",
                    checkIfNativeDate: function () {
                        return Modernizr.inputtypes.date && (Modernizr.touch && navigator.appVersion.indexOf("Win") === -1);
                    },
                    placeholder: "",
                    classes: "form-control",
                    dayOffset: 0,
                    pikadayOptions: {
                        onSelect: function () {
                            console.log($(this).focus());
                        }
                    }
                });
            });
        }

        $('.pikaday__invisible').click(function () {
            $(this).focus();
        });
    }

    //wenzhixin
    if($.fn.bootstrapTable){
        $.fn.bootstrapTable.defaults.undefinedText = "";
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

    /* Prevent user click submit form twice or more */
    $forms.submit(function () {
        if ($(this).valid()) {
            $forms.submit(function () {
                return false;
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

!function(root,factory){"function"==typeof define?define("pikaday-responsive",["exports"],function(exports){return exports["default"]=factory()}):"object"==typeof module?module.exports=factory():root.pikadayResponsive=factory()}(this,function(){if(!moment)return void console.error("You need to load moment.js in order to use pikaday-responsive.");if(!jQuery)return void console.error("You need to load jQuery in order to use pikaday-responsive.");if(!Pikaday)return void console.error("You need to load pikaday in order to use pikaday-responsive.");var defaultOptions={format:"YYYY-MM-DD",outputFormat:"YYYY-MM-DD",checkIfNativeDate:function(){return Modernizr.inputtypes.date&&Modernizr.touchevents&&-1===navigator.appVersion.indexOf("Win")},classes:"",placeholder:"Select a date",pikadayOptions:{},dayOffset:0};return function(el,options){var $container,$input,$display,$el=$(el),settings=$.extend({},defaultOptions,options),obj={pikaday:null,value:null,date:null,element:$el[0]};if(!$el.length||"INPUT"!==$el[0].tagName)return console.error("pikadayResponsive expects an input-field as its first element.",$el[0]),!1;$el.attr("type","hidden"),$el.wrap("<span class='pikaday__container'></span>"),$container=$el.parent(".pikaday__container");var originalId=$el.attr("id");if(settings.checkIfNativeDate())$input=$("<input type='date' class='pikaday__invisible' placeholder='"+settings.placeholder+"'/>"),originalId&&$input.attr("id",originalId+"-input"),$container.append($input),$display=$("<input type='text' readonly='readonly' class='pikaday__display pikaday__display--native "+settings.classes+"' placeholder='"+settings.placeholder+"' />"),$container.append($display),$input.on("change",function(){var val=$(this).val();$display.removeClass("is-empty"),val?(obj.date=moment(val,"YYYY-MM-DD"),obj.value=obj.date.format(settings.outputFormat)):(obj.date=null,obj.value=null,$display.addClass("is-empty")),1*obj.value===parseInt(obj.value,10)&&(obj.value*=1),$el.val(obj.value),obj.date?$display.val(obj.date.format(settings.format)):$display.val(null),$el.trigger("change"),$el.trigger("change-date",[obj])});else{$input=$("<input type='text' class='pikaday__display pikaday__display--pikaday "+settings.classes+"' placeholder='"+settings.placeholder+"' />"),originalId&&$input.attr("id",originalId+"-input"),$container.append($input);var hasSelected=!1,selectTimer=null;obj.pikaday=new Pikaday($.extend({},settings.pikadayOptions,{field:$input[0],format:settings.format})),$input.on("change",function(){if(!hasSelected){hasSelected=!0,selectTimer=window.setTimeout(function(){hasSelected=!1},10);var val=$(this).val();$input.removeClass("is-empty"),val?(obj.date=moment(val,settings.format),obj.date.add(settings.dayOffset,"day"),obj.value=obj.date.format(settings.outputFormat),$(this).val(obj.date.format(settings.format))):(obj.date=null,obj.value=null,$input.addClass("is-empty")),1*obj.value===parseInt(obj.value,10)&&(obj.value*=1),$el.val(obj.value),setTimeout(function(){$el.trigger("change"),$el.trigger("change-date",[obj])},1)}})}var setDate=function(date,format){return date?("object"==typeof date&&"function"!=typeof date.format&&(date=moment(date)),"string"==typeof date&&("undefined"!=typeof format&&format||(format=settings.outputFormat),date=moment(date,format)),"number"==typeof date&&(date=moment(date)),obj.pikaday?obj.pikaday.setMoment(date):($input.val(date.format("YYYY-MM-DD")),$input.trigger("change")),date):(obj.pikaday?obj.pikaday.setDate(null):($input.val(null),$input.trigger("change")),null)};return $el.val()&&setDate($el.val()),obj.setDate=setDate,obj}});