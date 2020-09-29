//! add common logic in js here
function showLoading() {
    setTimeout(function () {
        $.blockUI({
            message: $('#LoadingImage'),
            css: { padding: 0, margin: 0, left: (($(window).width() / 2) - 120) + 'px', width: '280px', height: '160px', textAlign: 'center', color: 'Black', border: 'none', backgroundColor: 'rgb(234,234,234)', cursor: 'wait' },
            overlayCSS: { backgroundColor: 'rgb(234,234,234)' },
            focusInput: false,
            onBlock: function () {

            }
        });
    });
}
function AngularPostOfData($http, url, dataToSend) {
    var result = new promiseResult();
    var csrf = window.top.GetCsrfTokenFromPage();
    $http.post(location.pathname + url, dataToSend, { headers: { 'AppID': AppID, 'X-CSRF-Header': csrf } })
        .then(function (data) {
            if (!data.data.Success && !(typeof data.data === 'string'))
                console.error(data.data.Message);
            else
                result.doneCallback(data);
            result.alwaysCallback();
        }, function (error) {
            alert(error.responseText);
            result.alwaysCallback();
        });
    return result;
}
function promiseResult() {
    var self = this;
    self.doneCallback = function () { }
    self.alwaysCallback = function () { }
    self.done = function (callback) {
        if (typeof callback == 'function')
            self.doneCallback = callback;
        return self;
    }
    self.always = function (callback) {
        if (typeof callback == 'function')
            self.alwaysCallback = callback;
        return self;
    }
}
function checkBrowser() {
    // Opera 8.0+
    if ((!!window.opr && !!opr.addons) || !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0) {
        return "opera";
    }
    // Firefox 1.0+
    else if (typeof InstallTrigger !== 'undefined') {
        return "firefox";
    }
    // Safari 3.0+ "[object HTMLElementConstructor]" 
    else if (/constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || safari.pushNotification)) {
        return "safari";
    }
    // Internet Explorer 6-11
    else if (false || !!document.documentMode) {
        return "msie";
    }
    // Edge 20+
    else if (!(false || !!document.documentMode) && !!window.StyleMedia) {
        return "edge";
    }
    // Chrome 1+
    else if (!!window.chrome && !!window.chrome.webstore) {
        return "chrome";
    }
    /*// Blink engine detection
    var isBlink = (isChrome || isOpera) && !!window.CSS;*/
}
function validateCharacter(text) {
    text = text.replace(/</g, "").replace(/>/g, "");
    return text;
}