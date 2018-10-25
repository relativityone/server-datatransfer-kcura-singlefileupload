'use strict';

var MFUController = function ($scope, $http, $compile) {
    var dialog = window.parent.parent.$("#uploadInfoDiv");
    var dialog_overlay = window.parent.parent.$(".ui-widget-overlay");
    var browser = checkBrowser();
    var msgLabel = document.getElementById("msg");
    var vm = $scope;
    var controlNumberMessage = "The Control Number you selected is already in use. Try again.";
    var sizeMessage = "You can't upload files greater than 2GB in size.";
    vm.simulateFileClick = SimulateFileClick;
    vm.handleDragOver = HandleDragOver;
    vm.handleDragLeave = HandleDragLeave;
    vm.handleDnDFileSelect = HandleDnDFileSelect
    vm.submitFrm = SubmitFrm;
    vm.cancel = Cancel;
    vm.close = Close;
    vm.status = 0;
    vm.errorID = errorID;
    vm.newImage = NewImage;
    vm.hasRedactions = HasRedactions;
    vm.hasNative = HasNative;
    vm.title = errorID == 0 ? "New Documents" : "Processing Document";
    vm.tempDocId = 0;
    vm.optionalControlNumber = { text: '' };
    vm.focusControlNumberValue = false;
    vm.uploaded = false;
    vm.focusControlNumber = function (value) {
        vm.focusControlNumberValue = value;
    }
    vm.files = [];
    vm.uploadFiles = UploadFiles;
    vm.uploadFile = UploadFile;
    vm.cancelFile = CancelFile;
    sessionStorage['____pushNo'] = '';
    var files;
    vm.timelapse;
    vm.startTime;
    vm.totalFiles = 0;
    vm.maxFiles = MaxFilesToUpload;
    var idCheckTimeout;

    function getdH() {
        return document.getElementById('dropHere');
    }

    function dialogChanges() {
        var externalFrame = $($(window.parent.parent.document).find('#_externalPage')[0].contentDocument);
        externalFrame.find('.dynamic-content-modal-close').hide();
        externalFrame.find('.modal-context').click(function () {
            switch (vm.status) {
                case (1):
                    externalFrame.find('dynamic-content-modal-wgt').show();
                    break;
                case (3):
                    Close()
                    break;
                default:
                    if (vm.uploaded) {
                        Close();
                    } else {
                        externalFrame.find('dynamic-content-modal-wgt').hide();
                        location.replace(location.href.replace('sfu', 'sfu.html'));
                    }
            }
        });
    }
    try {
        dialogChanges();
    } catch (e) { }

    function Addfiles(files) {
        cleanFiles();
        ChangeModalHeight(520);
        var focusindex = undefined;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var found = -1;
            vm.files.some(function (element,i) {
                var result = false;
                if (element.file.name == file.name) {
                    element.status = 4;
                    found = i;
                    result = true;
                }
                return result;
            });
            if (found === -1) {
                if (vm.files.length < vm.maxFiles) {
                    vm.files.push({ controlNumberText: file.name, file: file, status: 0, errorMessage: "" });
                } else {
                    break;
                }
            } else {
                focusindex = found;
            }
        }
        if (focusindex) {
            setTimeout(function () {
                window.location.hash = '#file' + focusindex;
            }, 100)
        }
        vm.totalFiles = vm.files.length;
    }
    function cleanFiles() {
        vm.files = vm.files.filter(function (element) {
            return element.status != 3;
        });
    }
    function SubmitFrm() {
        files = document.getElementById("file").files;
        if (browser == "msie") {
            Addfiles(files);
        }
        else {
            $scope.$apply(function () {
                Addfiles(files);
            });
        }
    }

    function HandleDragOver(event) {
        stopPropagation(event);
        $scope.$apply(function () {
            if (vm.files.length === 0) {
                vm.status = 4;
            }
            msgLabel.className = "message";
            msgLabel.innerHTML = "Drop your files here or <span> browse for files.</span>";
            getdH().style.borderColor = "rgb(28, 84, 171)";
        });
    }
    function HandleDragLeave(event) {
        stopPropagation(event);
        $scope.$apply(function () {
            if (vm.files.length === 0) {
                vm.status = 0;
            }
            getdH().style.borderColor = "#c3d2e7";
        });

    }

    function HandleDnDFileSelect(event) {
        stopPropagation(event);
        getdH().style.borderColor = "#c3d2e7";
        $scope.$apply(function () {
            if (vm.status != 1) {
                if (vm.files.length < vm.maxFiles) {
                    files = event.dataTransfer.files;
                    var item = browser == "msie" ? {} : event.dataTransfer.items[0].webkitGetAsEntry();
                    if (!item.isDirectory) {
                        Addfiles(files);
                    }
                }
                vm.status = 0;
            }
        });
    }
    function SimulateFileClick(force) {
        vm.status = 0;
        msgLabel.className = "message";
        msgLabel.innerHTML = "Drop your files here or <span> browse for files.</span>";
        if (vm.files.length < vm.maxFiles) {
            document.getElementById('file').value = "";
            document.getElementById('file').click();
        }
    }
    function isJson(str) {
        try {
            JSON.parse(str);
        } catch (e) {
            return false;
        }
        return true;
    }

    function UploadFiles() {
        msgLabel.className = "message";
        msgLabel.innerHTML = "Drop your files here or <span> browse for files.</span>";
        cleanFiles();
        vm.totalFiles = vm.files.length;
        if (vm.totalFiles > 0) {
            vm.status = 0;
            calculateTime();
            vm.status = 1;
            vm.fileIndex = 0;
            UploadFile(0);
        }
        else {
            vm.status = 0;
        }
        if (!vm.files.length) {
            ChangeModalHeight(335);
        }
    }

    function UploadFile(fileIndex, retry) {
        var form = document.getElementById('btiFormDD');
        var data = new FormData(form);
        var file = vm.files[fileIndex];
        if (retry) {
            vm.status = 0;
            calculateTime();
            vm.totalFiles = 1;
            vm.status = 1;
            vm.fileIndex = 0;
        }
        file.status = 1;
        if (vm.errorID == 0) {
            data.append('file', file.file);
            data.append('fid', getFolder());
            if (file.controlNumberText != file.file.name) {
                data.append('controlNumberText', file.controlNumberText);
            }
        }
        if (ValidateFileSize(file.file)) {
            var xhr = new XMLHttpRequest();
            var csrf = window.top.GetCsrfTokenFromPage();
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4) {
                    if (xhr.status==200) {
                        eval(xhr.responseText.replace('<script>', '').replace('</script>', ''));
                    }
                    else {
                        sessionStorage['____pushNo'] = '{"Success":false,"Message":"Failed to import due to an unexpected error. Please contact your system administrator."}';
                    }
                    CompleteUpload(fileIndex, file, retry);
                }
            };
            dialog.dialog("option", "closeOnEscape", false);
            xhr.open('POST', form.action);
            xhr.setRequestHeader('X-CSRF-Header', csrf);
            xhr.send(data);
        }
        else {
            sessionStorage['____pushNo'] = '{"Success":false,"Message":"' + sizeMessage + '"}';
            setTimeout(function () {
                CompleteUpload(fileIndex, file, retry)
            }, 10);
        }
    }
    function CompleteUpload(fileIndex, file, retry) {
        var resultString = sessionStorage['____pushNo'] || '';
        sessionStorage['____pushNo'] = '';
        checkUpload(file, resultString);
        if ((fileIndex < vm.totalFiles - 1) && !retry) {
            fileIndex++;
            UploadFile(fileIndex);
        }
        vm.fileIndex++;
        Uploading();
    }

    function checkUpload(file, resultString) {
        if (!!resultString) {
            var result;
            resultString = resultString.replace(/\\/g, "\\\\");
            if (isJson(resultString)) {
                result = JSON.parse(resultString);
            }
            else {
                result = { Success: false, Message: "Failed to import due to an unexpected error. Please contact your system administrator." };
            }
            if (vm.errorID != 0 || GetDID() != -1 || !result.Success || !!result.Message) {
                manageResult(file, result);
            }
            else {
                checkUploadStatus(file, result);
            }
        }
        else
            idCheckTimeout = setTimeout(
                function () { checkUpload(file, resultString) }, 500);
    }

    function checkUploadStatus(file, resultString) {
        setTimeout(function () {
            resultString.Data = resultString.Data.replace(/\/39\//g, "'").replace(/\/34\//g, '"');
            AngularPostOfData($http, "/checkUploadStatus", {
                documentName: resultString.Data
            })
                .done(function (result) {
                    if (result.data != "-1") {
                        manageResult(file, resultString, true);
                    }
                    else {
                        checkUploadStatus(file, resultString);
                    }
                })
        }, 500);
    }

    function manageResult(file, result, removeDigest) {
        if (result.Success && (!result.Message || result.Message.indexOf("\\\\") === 0)) {
            if (removeDigest) {
                file.status = 3;
            }
            else {
                $scope.$apply(function () {
                    file.status = 3;
                });
            }
        }
        else {
            var status = result.Message.indexOf('permissions') == -1 ? 2 : 6;
            if (result.Message === controlNumberMessage || result.Message === sizeMessage) {
                status = 5;
            }
            if (removeDigest) {
                file.status = status;
            }
            else {
                $scope.$apply(function () {
                    file.status = status;
                    file.errorMessage = result.Message;
                });
            }
        }
    }
    function Uploading() {
        var elem = document.getElementById("progressBar");
        var porcent = 100 / vm.totalFiles;
        var width = porcent * vm.fileIndex;
        setTimeout(function () {
            elem.style.width = width + '%';
            if ((width) >= 100) {
                $scope.$apply(function () {
                    vm.status = 3;
                    vm.uploaded = true;
                });
                elem.style.width = '1%';
            }
        }, 100);
    }

    function stopPropagation(event) {
        event.stopPropagation();
        event.preventDefault();
    }

    function Cancel() {
        var filesDontRemove = [];
        for (var i = 0; i < vm.files.length; i++) {
            var file = vm.files[i];
            if (vm.status == 1 && !(file.status == 0 || file.status == 4)) {
                filesDontRemove.push(file)
            }
        }
        vm.files = filesDontRemove;
        vm.totalFiles = vm.files.length;
        if (!vm.files.length) {
            vm.status = 0;
            ChangeModalHeight(335);
        }
    }

    function Close() {
        if (!!window.top.relativity && !!window.top.relativity.redirectionHelper && typeof window.top.relativity.redirectionHelper.handleNavigateListPageRedirect === 'function') {
            window.top.relativity.redirectionHelper.handleNavigateListPageRedirect(window.top.location.href)
        } else {
            window.parent.location.reload()
        }
    }
    function getFolder() {
        var id = '-1';
        var wN = window.parent.frames['externalPage'] || window.parent.parent.frames['externalPage'];
        if (wN) {
            var $out = wN.window.$;
            if ($out('.browser-folder.browser-icon-active', wN.document).length)
                id = $out('.jstree-node[aria-selected=true]', wN.document).attr('id');
        }
        id = id || '-1';
        if (id.indexOf('_') > -1)
            id = id.split('_')[1];
        return id;
    }

    function GetQueryStringValueByName(search, key) {
        return decodeURIComponent(search.replace(new RegExp("^(?:.*[&\\?]" + encodeURIComponent(key).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
    }
    function GetDID() {
        var did = -1;
        if (document.getElementById('fdv') != null && document.getElementById('fdv').getAttribute('value') == 'true')
            did = GetQueryStringValueByName(window.parent.location.search, "ArtifactID");
        return did;
    }
    function calculateTime(apply) {
        if (vm.status == 0) {
            vm.startTime = new Date();
        }
        var today = new Date();
        var diference = (today.getTime() - vm.startTime.getTime()) / 1000;
        var s = Math.floor(diference % 60);
        diference = diference / 60;
        var m = Math.floor(diference % 60);
        diference = diference / 60;
        var h = Math.floor(diference % 24);
        m = checkTime(m);
        s = checkTime(s);
        h = checkTime(h);
        if (apply) {
            $scope.$apply(function () {
                vm.timelapse = h + ":" + m + ":" + s;
            });

        } else {
            vm.timelapse = h + ":" + m + ":" + s;
        }
        if (vm.status != 3) {
            var t = setTimeout(function () { calculateTime(true) }, 500);
        }
    }
    function checkTime(i) {
        if (i < 10) { i = "0" + i };  // add zero in front of numbers < 10
        return i;
    }
    function CancelFile(index) {
        vm.files.splice(index, 1);
        vm.totalFiles = vm.files.length;
        if (!vm.files.length) {
            vm.status = 0;
            ChangeModalHeight(335);
        }
    }
    function ChangeModalHeight(height) {
        window.frameElement.parentElement.style.height = height + "px";
    }

    function ValidateFileSize(file) {
        var canUpload = file.size <= 2147483648;
        return canUpload;
    }
}
MFUController.$inject = ['$scope', '$http', '$compile'];