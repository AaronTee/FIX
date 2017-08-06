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




//Show Alert on top
function showBoostrapAlert(message, style, dismissable) {
    $("#AlertPartial").empty();

    var html = "";
    var dismissableClass = dismissable ? "alert-dismissable" : null;

    html = "<div class='alert alert-" + style + " " + dismissableClass + "' >";
    if (dismissable) {
        html = html + "<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>";
    }
    html = html + message + "</div>";
    $("#AlertPartial").append(html);
    $("#AlertPartial").show();
    $("#AlertPartial").delay(8000).fadeOut(300);
};






























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