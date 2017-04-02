function showSpinner(obj, offset, offsetTop, offsetLeft) {
    var of = "";
    var stOf = "";
    if (offset) {
        of = "on-element";
        stOf = "top:" + offsetTop + "px;left:" + offsetLeft + "px";
    }
    var loading = "<div class='spinner active " + of + "' style='" + stOf + "'><i class='fa fa-spin fa-spinner'></i></div>";
    $(obj).before(loading);
}

function showSpinnerAfter(obj, offset, offsetTop, offsetLeft) {
    var of = "";
    var stOf = "";
    if (offset) {
        of = "on-element";
        if (offsetTop == undefined || offsetTop == null || offsetTop == '') offsetTop = 5;
        stOf = "top:" + offsetTop + "px;left:" + offsetLeft + "px";
    }
    var loading = "<div class='spinner active " + of + "' style='" + stOf + "'><i class='fa fa-spin fa-spinner'></i></div>";
    $(obj).after(loading);
}

function showSpinnerAppend(obj, offset, offsetTop, offsetLeft) {
    var of = "";
    var stOf = "";
    if (offset) {
        of = "on-element";
        stOf = "top:" + offsetTop + "px;left:" + offsetLeft + "px";
    }
    var loading = "<div class='spinner active " + of + "' style='" + stOf + "'><i class='fa fa-spin fa-spinner'></i></div>";
    $(obj).prepend(loading);
}

function showSpinnerPrepend(obj, offset, offsetTop, offsetLeft) {
    var of = "";
    var stOf = "";
    if (offset) {
        of = "on-element";
        stOf = "top:" + offsetTop + "px;left:" + offsetLeft + "px";
    }
    var loading = "<div class='spinner active " + of + "' style='" + stOf + "'><i class='fa fa-spin fa-spinner'></i></div>";
    $(obj).prepend(loading);
}

function hideSpinner(obj) {
    if (obj) {
        $(obj).parent().find(".spinner.active").remove();
    } else {
        $(".spinner").remove();
    };
}

function showSpinnerAfterAndDisable(obj, offset, offsetTop, offsetLeft) {
    $(obj).prop('disabled', true);
    $(obj).addClass('disabled');
    var of = "";
    var stOf = "";
    if (offset) {
        of = "on-element";
        stOf = "top:" + offsetTop + "px;left:" + offsetLeft + "px";
    }
    var loading = "<div class='spinner active " + of + "' style='" + stOf + "'><i class='fa fa-spin fa-spinner'></i></div>";
    $(obj).after(loading);
}

function showSpinnerAppendAndDisable(obj, offset, offsetTop, offsetLeft) {
    $(obj).prop('disabled', true);
    $(obj).addClass('disabled');
    var of = "";
    var stOf = "";
    if (offset) {
        of = "on-element";
        stOf = "top:" + offsetTop + "px;left:" + offsetLeft + "px";
    }
    var loading = "<div class='spinner active " + of + "' style='" + stOf + "'><i class='fa fa-spin fa-spinner'></i></div>";
    $(obj).prepend(loading);
}

function hideSpinnerAndEnable(obj) {
    if (obj) {
        $(obj).parent().find(".spinner.active").remove();
    } else {
        $(".spinner").remove();
    };
    $(obj).prop('disabled', false);
    $(obj).removeClass('disabled');
}