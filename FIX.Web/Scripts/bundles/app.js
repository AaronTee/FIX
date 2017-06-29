$(function(){

    var $forms = $("form");
    $forms.each(function () {
        var validator = $(this).data('validator');
        if (validator) {
            $(validator).each(function () {
                this.settings.onkeyup = false;
            });
        }
    })
    

    //$("input,select").bind("keydown", function (e) {
    //    var keyCode = e.keyCode || e.which;
    //    if (keyCode === 13) {
    //        e.preventDefault();
    //        $('input, select, textarea')
    //        [$('input,select,textarea').index(this) + 1].focus();
    //    }
    //});
})

    

    //$("form").removeData("validator").removeData("unobtrusiveValidation");

    //$.validator.setDefaults({
    //    onkeyup: false,
    //    onfocusout: function (element) {
    //        this.element(element);
    //    },
    //    highlight: function (element) {
    //        $(element).closest('.form-control').removeClass('got-success').addClass('got-error');
    //    },
    //    unhighlight: function (element) {
    //        $(element).closest('.form-control').addClass('got-success').removeClass('got-error');
    //    }
    //});
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

$(function () {

var toggler = $('.navbar-toggle');
var sidebar = $('.' + toggler.attr("data-target"));
var dimTarget = $('.' + toggler.attr("data-dim-target"));
var bodyCanvas = $('.body-canvas');


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

$(function(){useClientSideValidation();$form=$("#user-form");$form.validate({rules:{Email:{required:!0,email:!0,remote:{url:GetPath("User/ValidateEmail"),type:"post",data:{input:function(){return $("#Email").val()}},complete:function(){$("#Email").valid()}}},Username:{required:!0,remote:{url:GetPath("User/ValidateUsername"),type:"post",data:{input:function(){return $("#Username").val()}},complete:function(){$("#Username").valid()}}},Password:{required:!0},ConfirmPassword:{required:!0,equalTo:"#Password"},RoleId:{required:!0},FirstName:{required:!0},Gender:{required:!0},Country:{required:!0},PhoneNo:{required:!0},BankId:{required:!0},BankAccountHolder:{required:!0},BankAccountNo:{required:!0},BankBranch:{required:!0}},messages:{required:"Thie field is required.",Email:{remote:"Email has been used in one of the account, please enter another."},Username:{remote:"Username has been taken, please enter another."},ConfirmPassword:"Password not match."}})});