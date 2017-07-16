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