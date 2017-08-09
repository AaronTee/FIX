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
        $this.find(".sidebar-subnav").toggleClass('show');
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

    $('input:not([type="checkbox"])[data-val="true"], select[data-val="true"]')
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