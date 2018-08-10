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
    vm.changeImage = ChangeImage;
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

    sessionStorage['____pushNo'] = '';
    var files;

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
        if ((vm.files.length + files.length) > 20) {
            msgLabel.innerHTML = "You can upload up to 20 files.";
        }
        else {
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                if (!vm.files.includes(file)) {
                    vm.files.push({ controlNumberText: file.name, file: file, status: 0 });
                }
            }
        }
    }

    function SubmitFrm() {
        if (vm.errorID == 0) {
            document.getElementById('fid').setAttribute('value', getFolder());
            document.getElementById('did').setAttribute('value', GetDID());
            document.getElementById('controlNumberText').setAttribute('value', vm.optionalControlNumber.text);
        }
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

        if ((vm.status == 0 || force)) {
            vm.status = 0;
            msgLabel.className = "message";
            msgLabel.innerHTML = "Drop your files here or <span> browse for files.</span>";
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
    function submitSimulatedForm() {
        var form = document.getElementById('btiFormDD');
        var data = new FormData(form);
        data.append('file', bkpFile);

        if (vm.errorID == 0) {
            data.append('fid', getFolder());
            data.append('fdv', document.getElementById('fdv').getAttribute('value'));
            data.append('did', GetDID());
            data.append('force', document.getElementById('force').getAttribute('value'));
            data.append('controlNumberText', document.getElementById('controlNumberText').value);
        }

        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4)
                eval(xhr.responseText.replace('<script>', '').replace('</script>', ''));
        };
        msgLabel.innerHTML = "Uploading";
        xhr.open('POST', form.action);
        xhr.send(data);
    }

    function stopPropagation(event) {
        event.stopPropagation();
        event.preventDefault();
    }

    function Cancel() {
        location.replace(location.href.replace('sfu', 'sfu.html'));
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

}
MFUController.$inject = ['$scope', '$http', '$compile'];