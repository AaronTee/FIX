function useClientSideValidation() {

    $("form").removeData("validator").removeData("unobtrusiveValidation");

    $.validator.setDefaults({
        onkeyup: false,
        highlight: function (element) {
            $(element).closest('.form-control').removeClass('got-success').addClass('got-error');
        },
        unhighlight: function (element) {
            $(element).closest('.form-control').addClass('got-success').removeClass('got-error');
        },
    })

}