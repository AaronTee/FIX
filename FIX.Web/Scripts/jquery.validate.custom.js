$.validator.newMethod = function () {
};
$.validator.addMethod('validateUsername', function (value, element) {
    var isValid = false;
    if (value) {
        $.ajax({
            url: GetPath('User/ValidateUsername'),
            type: 'GET',
            async: false,
            data: { input: value },
            success: function (_isValid) {
                isValid = _isValid;
                return isValid;
            }
        });
    }
    return isValid;
}, 'Username has been taken, please choose another one.');
