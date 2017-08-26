function UrlExists(url) {
    var http = new XMLHttpRequest();
    http.open('HEAD', url, false);
    http.send();
    return http.status != 404;
}

function showAlert(message, style, autodismiss) {

    var alertWrapper = $(".ui-message-wrapper");
    alertWrapper.empty();

    var html = "";
    var dismissableClass = "alert-dismissable";
    if(autodismiss === 'undefined')
    {
        autodismiss = true;
    };

    html = "<div class='alert alert-" + (style || "success") + " " + dismissableClass + "' >";
    html = html + "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-hidden=\"true\">&times;</button>";
    html = html + message + "</div>";

    alertWrapper.append(html);
    alertWrapper.show();

    if (autodismiss || true) alertWrapper.delay(8000).fadeOut(300);

};