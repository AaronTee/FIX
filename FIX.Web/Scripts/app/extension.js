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

    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };

    ////Add validator method.
    //$.validator.addMethod("moreEqualZero", function (value, element) {
    //    if (value >= 0) return true;
    //    else false;
    //});

})(jQuery);