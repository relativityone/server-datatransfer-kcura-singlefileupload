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