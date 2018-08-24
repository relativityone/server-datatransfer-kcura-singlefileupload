'use strict';

var MFUController = function ($scope, $http, $compile) {
    var dialog = window.parent.parent.$("#uploadInfoDiv");
    var dialog_overlay = window.parent.parent.$(".ui-widget-overlay");
    var browser = checkBrowser();
    var msgLabel = document.getElementById("msg");
    var vm = $scope;
    vm.simulateFileClick = SimulateFileClick;
    vm.handleDragOver = HandleDragOver;
    vm.handleDragLeave = HandleDragLeave;
    vm.handleDnDFileSelect = HandleDnDFileSelect
    vm.submitFrm = SubmitFrm;
    vm.cancel = Cancel;
    vm.close = Close;
    vm.status = 0;
    vm.showMessage = true;
    vm.errorID = errorID;
    vm.newImage = NewImage;
    vm.hasRedactions = HasRedactions;
    vm.hasNative = HasNative;
    vm.title = errorID == 0 ? "New Documents" : "Processing Document";
    vm.tempDocId = 0;
    vm.optionalControlNumber = { text: '' };
    vm.focusControlNumberValue = false;
    vm.focusControlNumber = function (value) {
        vm.focusControlNumberValue = value;
    }
    vm.files = [];
    vm.uploadFiles = UploadFiles;
    vm.uploadFile = UploadFile;
    vm.cancelFile = CancelFile;
    vm.removeFile = RemoveFile;
    vm.process = Process;
    sessionStorage['____pushNo'] = '';
    var files;
    vm.timelapse;
    vm.startTime;
    vm.totalFiles = 0;
    var idCheckTimeout;

    function getdH() {
        return document.getElementById('dropHere');
    }

    function dialogChanges() {
        var externalFrame = $($(window.parent.parent.document).find('#_externalPage')[0].contentDocument);
        externalFrame.find('.dynamic-content-modal-close').hide();
        externalFrame.find('.modal-context').click(function () {
            externalFrame.find('dynamic-content-modal-wgt').hide();
            location.replace(location.href.replace('sfu', 'sfu.html'));
        });
    }
    try {
        dialogChanges();
    } catch (e) { }

    function Addfiles(files) {
        cleanFiles();
        if ((vm.files.length + files.length) > 20) {
            vm.status = 2;
            msgLabel.className = "msgDetails";
            msgLabel.innerHTML = "<div class='error' title='You can upload up to 20 files.'><div><img src='/Relativity/CustomPages/1738ceb6-9546-44a7-8b9b-e64c88e47320/Content/Images/Error_Icon.png' /><span>You can upload up to 20 files.</span></div></div>";
        }
        else {
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                var found = vm.files.find(function (element) {
                    return element.file.name == file.name;
                });
                if (!found) {
                    vm.files.push({ controlNumberText: file.name, file: file, status: 0, errorMessage: "" });
                }
            }
        }
        vm.totalFiles = vm.files.length;
    }
    function cleanFiles() {
        vm.files = vm.files.filter(element => element.status != 3);
    }
    function SubmitFrm() {
        files = document.getElementById("file").files;
        $scope.$apply(function () {
            Addfiles(files);
        });
    }

    function HandleDragOver(event) {
        stopPropagation(event);
        $scope.$apply(function () {
            vm.status = 4;
            msgLabel.className = "message";
            msgLabel.innerHTML = "Drop your files here or <span> browse for files.</span>";
        });
    }
    function HandleDragLeave(event) {
        stopPropagation(event);
        $scope.$apply(function () {
            vm.status = 0;
        });

    }

    function HandleDnDFileSelect(event) {
        stopPropagation(event);

        $scope.$apply(function () {
            files = event.dataTransfer.files;
            var item = browser == "msie" ? {} : event.dataTransfer.items[0].webkitGetAsEntry();

            if (!item.isDirectory) {
                Addfiles(files);
                vm.status = 0;
            }
            else {
                vm.status = 2;
                var message = "Multiple file upload is not supported.";
                msgLabel.className = "msgDetails";
                msgLabel.innerHTML = "<div class='error' title='" + message + "'><div><img src='/Relativity/CustomPages/1738ceb6-9546-44a7-8b9b-e64c88e47320/Content/Images/Error_Icon.png' /><span>" + message + "</span></div></div>";
            }
        });
    }
    function SimulateFileClick(force) {
        vm.status = 0;
        msgLabel.className = "message";
        msgLabel.innerHTML = "Drop your files here or <span> browse for files.</span>";
        document.getElementById('file').value = "";
        document.getElementById('file').click();
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
        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                eval(xhr.responseText.replace('<script>', '').replace('</script>', ''));
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
        };
        dialog.dialog("option", "closeOnEscape", false);
        xhr.open('POST', form.action);
        xhr.send(data);
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
                });
                elem.style.width = '1%';
            }
        }, 500);
    }

    function stopPropagation(event) {
        event.stopPropagation();
        event.preventDefault();
    }

    function Cancel() {
        if (vm.status == 0) {
            Close();
        }
        else {
            window.parent.location.reload();
        }
    }

    function Close() {

        var modalCls = $('.modal-container', window.parent.document).find(".dynamic-content-modal-close")[0];

        if (modalCls != null) {
            $(modalCls).click();
        }
        else {
            window.parent.$('#uploadInfoDiv').dialog('close');
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
    }
    function RemoveFile(file) {
        file.lastStatus = file.status;
        file.status = 4;
    }
    function Process(file, index) {
        if (index > -1) {
            CancelFile(index);
        }
        else {
            file.status = file.lastStatus;
        }
    }
}
MFUController.$inject = ['$scope', '$http', '$compile'];