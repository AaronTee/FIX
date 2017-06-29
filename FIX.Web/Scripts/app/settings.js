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