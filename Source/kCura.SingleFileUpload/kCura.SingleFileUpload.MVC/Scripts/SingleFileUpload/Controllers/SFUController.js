(function () {
    'use strict';

    angular
    .module('sfuapp', [])
    .controller('sfuctrl', SFUController);

    SFUController.$inject = ['$scope', '$http', '$compile'];

    function SFUController($scope, $http, $compile) {
        var dialog = window.parent.parent.$("#uploadInfoDiv");
        var dialog_overlay = window.parent.parent.$(".ui-widget-overlay");
        var browser = checkBrowser();
        var msgLabel = document.getElementById("msg");
        var vm = $scope;
        vm.simulateFileClick = SimulateFileClick;
        vm.handleDragOver = HandleDragOver;
        vm.handleDragLeave = HandleDragLeave;
        vm.handleDnDFileSelect = HandleDnDFileSelect;
        vm.continueRedactions = ContinueRedactions;
        vm.forceUpload = ForceUpload;
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
        vm.title = errorID == 0 ? (ChangeImage ? (NewImage || !HasImages ? "Upload Image" : "Replace Image") : FDV ?(HasNative ? "Replace Document" : "Upload Document") : "New Document") : "Processing Document";
        vm.tempDocId = 0;
        vm.choiceType = { type: 'fileName' };
        vm.optionalControlNumber = { text: '' };
        vm.focusControlNumberValue = false;
        vm.focusControlNumber = function (value) {
            vm.focusControlNumberValue = value;
        }
        vm.controlNumberSelected = function () {
            msgLabel.innerHTML = "Please type a Control Number before dropping or selecting your file.</span>";
        }


        sessionStorage['____pushNo'] = '';
        var files;
        var bkpFile = null;

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


        function SubmitFrm() {
            if (vm.errorID == 0) {
                document.getElementById('fid').setAttribute('value', getFolder());
                document.getElementById('did').setAttribute('value', GetDID());
                document.getElementById('controlNumberText').setAttribute('value', vm.optionalControlNumber.text);
            }

            var filesCount = document.getElementById("file").files.length;

            document.getElementById('btiForm').submit();
            notifyUploadStarted();

        }

        function HandleDragOver(event) {
            stopPropagation(event);
            $scope.$apply(function () {
                if (vm.choiceType.type == 'fileName') {
                    vm.status = 4;
                    msgLabel.className = "message";
                    msgLabel.innerHTML = "Drop your file here or <span> click to select a file.</span>";
                }
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

                if (vm.choiceType.type == 'fileName' || (vm.choiceType.type != 'fileName' && vm.optionalControlNumber.text != '')) {
                    vm.status = 1;
                    files = event.dataTransfer.files;
                    var item = browser == "msie" ? {} : event.dataTransfer.items[0].webkitGetAsEntry();

                    if (files.length == 1 && !item.isDirectory) {
                        bkpFile = files[0];
                        submitSimulatedForm();
                    }
                    else {
                        vm.status = 2;
                        var message = "Multiple file upload is not supported.";
                        msgLabel.className = "msgDetails";
                        msgLabel.innerHTML = "<div class='error' title='" + message + "'><div><img src='/Relativity/CustomPages/1738ceb6-9546-44a7-8b9b-e64c88e47320/Content/Images/Error_Icon.png' /><span>" + message + "</span></div></div>";
                    }
                }
            });
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
            notifyUploadStarted();
            checkUpload();
            xhr.open('POST', form.action);
            xhr.send(data);
        }

        function SimulateFileClick(force, event) {

            if ((vm.status == 0 || force)
                && (vm.choiceType.type == 'fileName' || (vm.choiceType.type != 'fileName' && vm.optionalControlNumber.text != ''))
                && !vm.focusControlNumberValue) {
                vm.status = 0;
                msgLabel.className = "message";
                msgLabel.innerHTML = "";
                if (vm.choiceType.type == 'fileName') {
                    msgLabel.innerHTML = "Drop your file here or <span> click to select a file.</span>";
                } else {
                    msgLabel.innerHTML = "Please type a Control Number before dropping or selecting your file.</span>";
                }

                document.getElementById('file').value = "";
                document.getElementById('file').click();
            }
        }

        function deleteImagesAndRedactions(resultToManage) {
            var result = undefined;

            $http.post("/Relativity.Rest/api/Relativity.Services.Document.IDocumentModule/Document Manager/DeleteImageFiles",
                {
                    "workspaceArtifactId": AppID,
                    "document": {
                        "ArtifactID": GetDID(),
                        "Identifier": ""
                    },
                    "forceDelete": true
                },
                {
                    headers: {
                        "X-CSRF-Header": '-'
                    }
                })
                .then(function (data) {

                    if (!!data) {
                        manageResult(resultToManage, true);
                    }
                }, function (error) {
                    console.error(error);
                });
        }

        function updateImageDocument(fileLocation) {

            $http.post("/Relativity.Rest/api/Relativity.Imaging.Services.Interfaces.IImagingModule/Imaging Job Service/ImageDocumentAsync",
                {
                    "imageDocumentJob": {
                        "WorkspaceId": AppID,
                        "DocumentId": GetDID(),
                        "ProfileId": ProfileArtifact,
                        "AlternateNativeLocation": fileLocation,
                        "RemoveAlternateNativeAfterImaging": true
                    }
                },
                {
                    headers: {
                        "X-CSRF-Header": '-'
                    }
                })
                .then(function (data) {

                    var fromDocumentViewer = document.getElementById('fdv').getAttribute('value') == 'true';
                    if (data) {
                        window.top.documentViewer.WaitForImaging();
                        Close();
                    }
                }, function (error) {
                    console.error("SFU: " + error);
                    msgLabel.className = "msgDetails";
                    msgLabel.innerHTML = "<div class='error' title='" + message + "'><div><img src='/Relativity/CustomPages/1738ceb6-9546-44a7-8b9b-e64c88e47320/Content/Images/Error_Icon.png' /><span>Error: " + message + "</span></div></div>";
                });

        }
        function isJson(str) {
            try {
                JSON.parse(str);
            } catch (e) {
                return false;
            }
            return true;
        }
        function checkUpload() {
            var resultString = sessionStorage['____pushNo'] || '';
            if (!!resultString) {
                sessionStorage['____pushNo'] = '';
                var result;
                resultString = resultString.replace(/\\/g, "\\\\");
                if (isJson(resultString)) {
                    result = JSON.parse(resultString);
                }
                else {
                    result = { Success: false, Message: "Failed to import due to an unexpected error. Please contact your system administrator." };
                }
                if (vm.errorID != 0 ||
                    GetDID() != -1 ||
                    !result.Success ||
                    !!result.Message ||
                    (document.getElementById('force') != null && document.getElementById('force').getAttribute('value') == 'true')) {
                    if (vm.changeImage && result.Message.indexOf("\\\\") > -1) {
                        deleteImagesAndRedactions(result);
                    }
                    else {
                        manageResult(result);
                    }
                }
                else {
                    checkUploadStatus(result);
                }
            }
            else
                idCheckTimeout = setTimeout(checkUpload, 500);
        }

        function checkUploadStatus(resultString) {
            setTimeout(function () {
                AngularPostOfData($http, "/checkUploadStatus", {
                    documentName: resultString.Data
                })
                .done(function (result) {
                    if (result.data != "-1") {
                        manageResult(resultString, true);
                    }
                    else {
                        checkUploadStatus(resultString);
                    }
                })
            }, 500);
        }

        function manageResult(result, removeDigest) {
            if (result.Success && (!result.Message || result.Message.indexOf("\\\\") === 0)) {
                if (removeDigest) {
                    vm.status = 3;
                }
                else
                    $scope.$apply(function () {
                        vm.status = 3;
                    });
                var footerHtml = !vm.changeImage ? "Document uploaded successfully!" : (vm.newImage ? "Document image uploaded succesfully!" : "Document image replaced succesfully!");
                msgLabel.className = "message";
                msgLabel.innerHTML = footerHtml;
                var fnc = function () { window.parent.location.reload() };
                if (vm.errorID == 0) {
                    var fromDocumentViewer = document.getElementById('fdv').getAttribute('value') == 'true';

                    if (vm.changeImage) {
                        updateImageDocument(result.Message);
                    }
                    else {
                        setTimeout(fnc, fromDocumentViewer ? 2000 : 3000);
                    }
                }
                else {
                    setTimeout(fnc, 3000);
                }
            }
            else if (result.Message == 'R') {
                if (removeDigest)
                    vm.status = 5;
                else
                    $scope.$apply(function () {
                        vm.status = 5;
                    });
                getdH().children[0].innerHTML = "Replace Document";
                msgLabel.className = "msgDetails";
                var elem = $("<div class=\"content\">A document with the same name already exists. <br/> Do you want to replace it?</div>");
                $(msgLabel).html(elem);

            }

            else {
                var status = result.Message.indexOf('permissions') == -1 ? 2 : 6;
                if (removeDigest) {
                    vm.status = status;
                }
                else {
                    $scope.$apply(function () {
                        vm.status = status;
                    });
                    var message = result.Message;
                    console.error("SFU: " + result.Message);
                    msgLabel.className = "msgDetails";
                    msgLabel.innerHTML = "<div class='error' title='" + message + "'><div><img src='/Relativity/CustomPages/1738ceb6-9546-44a7-8b9b-e64c88e47320/Content/Images/Error_Icon.png' /><span>Error: " + message + "</span></div></div>";
                }

            }
        }
        function notifyUploadStarted() {
            if (vm.changeImage) {
                dialog_overlay.off("click");
                var documentViewer = $(window.parent.parent.window)[0].documentViewer;
                documentViewer.SetViewer("Image");
                dialog.dialog("option", "closeOnEscape", false);
            }
            setTimeout(function () {
                if ($("#file")[0].files.length == 0)
                    return;

                $scope.$apply(function () {
                    vm.status = 1;
                });
                getdH().onclick = function () { };
                getdH().ondrop = function () { };
                msgLabel.innerHTML = "Uploading";
                checkUpload();
            })
        }

        function updateStatus(status, message) {

            vm.status = status;

            msgLabel.className = "message";
            msgLabel.innerHTML = message;
        }

        function stopPropagation(event) {
            event.stopPropagation();
            event.preventDefault();
        }
        function setDropStyle(color) {
            getdH().style['color'] = color;
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

        function ContinueRedactions() {
            vm.hasRedactions = false;
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
        function ForceUpload() {

            vm.status = 1;

            msgLabel.innerHTML = "Uploading";
            document.getElementById('force').setAttribute('value', 'true');
            if (bkpFile) {
                submitSimulatedForm();
            }
            else {
                SubmitFrm();
            }
        }

        function checkBrowser() {
            // Opera 8.0+
            if ((!!window.opr && !!opr.addons) || !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0)
                return "opera";
                // Firefox 1.0+
            else if (typeof InstallTrigger !== 'undefined')
                return "firefox";
                // Safari 3.0+ "[object HTMLElementConstructor]" 
            else if (/constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || safari.pushNotification))
                return "safari";
                // Internet Explorer 6-11
            else if (false || !!document.documentMode)
                return "msie";
                // Edge 20+
            else if (!(false || !!document.documentMode) && !!window.StyleMedia)
                return "edge";
                // Chrome 1+
            else if (!!window.chrome && !!window.chrome.webstore)
                return "chrome";

            /*// Blink engine detection
            var isBlink = (isChrome || isOpera) && !!window.CSS;*/
        }

    }
})();