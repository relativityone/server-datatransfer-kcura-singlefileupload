
(function () {
    'use strict';

    angular
    .module('sfuapp', [])
    .controller('sfuctrl', SFUController);

    SFUController.$inject = ['$scope', '$http', '$compile'];

    function SFUController($scope, $http, $compile) {
        var dialog = window.parent.parent.$("#uploadInfoDiv");
        var dialog_overlay = window.parent.parent.$(".ui-widget-overlay");
        var vm = $scope;
        vm.simulateFileClick = function (force) {
            simulateFileClick(force);
        };
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
        vm.title = errorID == 0 ? (ChangeImage ? (NewImage || !HasImages ? "Upload Image" : "Replace Image") : (FDV ? "Replace Document" : "New Document")) : "Processing Document";
        vm.tempDocId = 0;


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
            }

            if (vm.changeImage) {
                deleteImagesAndRedactions();
            }
            else {
                document.getElementById('btiForm').submit();
                notifyUploadStarted();
            }

        }

        function HandleDragOver(event) {
            stopPropagation(event);
            $scope.$apply(function () {
                vm.status = 4;
                getdH().children[2].className = "message";
                getdH().children[2].innerHTML = "Drop your file here or <span> click to select a file.</span>";
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
                vm.status = 1;
                files = event.dataTransfer.files;

                bkpFile = files[0];
                submitSimulatedForm();
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
            }
            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4)
                    eval(xhr.responseText.replace('<script>', '').replace('</script>', ''));
            };
            notifyUploadStarted();
            checkUpload();
            xhr.open('POST', form.action);
            xhr.send(data);
        }

        function simulateFileClick(force) {
            if (vm.status == 0 || force) {
                document.getElementById('file').click();
            }
        }

        function deleteImagesAndRedactions() {
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
                        document.getElementById('btiForm').submit();
                        notifyUploadStarted();
                    }
                }, function (error) {
                    console.error(error);
                });
        }

        function updateImageDocument(fileLocation) {
            var profileArtifact = $("#_fileTypeSelector_ImagingProfileSelector", window.parent.parent.$("#_documentViewer__documentIdentifierFrame").contents()).attr("defaultvalue");

            $http.post("/Relativity.Rest/api/Relativity.Imaging.Services.Interfaces.IImagingModule/Imaging Job Service/ImageDocumentAsync",
                {
                    "imageDocumentJob": {
                        "WorkspaceId": AppID,
                        "DocumentId": GetDID(),
                        "ProfileId": profileArtifact,
                        "AlternateNativeLocation": fileLocation,
                        "RemoveAlternateNativeAfterImaging": false
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
                        setTimeout(function () {
                            window.parent.location.reload();
                        }, fromDocumentViewer ? 2000 : 3000);
                    }
                }, function (error) {
                    console.error(error);
                    setTimeout(function () {
                        window.parent.location.reload();
                    }, 2000);
                });

        }

        function getFileLocation() {
            AngularPostOfData($http, "/GetFileLocation", {
                did: GetDID()
            }).done(function (result) {
                if (!!result.data.Data) {
                    var fileLocation = result.data.Data;
                    
                    updateImageDocument(fileLocation);
                }
            });
        }
        function checkUpload() {
            var resultString = sessionStorage['____pushNo'] || '';
            if (resultString) {
                sessionStorage['____pushNo'] = '';
                var result = JSON.parse(resultString);
                if (vm.errorID != 0 || GetDID() != -1 || !result.Success || (result.Message != '' && result.Message != null) || (document.getElementById('force') != null && document.getElementById('force').getAttribute('value') == 'true')) {

                    manageResult(result);
                }
                else
                    checkUploadStatus(JSON.parse(resultString));
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
                    if (result.data != "-1")
                        manageResult(resultString, true);
                    else
                        checkUploadStatus(resultString);
                })
            }, 500);
        }

        function manageResult(result, removeDigest) {
            if (result.Success && (result.Message == '' || result.Message == null)) {
                if (removeDigest)
                    vm.status = 3;
                else
                    $scope.$apply(function () {
                        vm.status = 3;
                    });
                var footerHtml = !vm.changeImage ? "Document uploaded successfully!" : (vm.newImage ? "Document image uploaded succesfully!" : "Document image replaced succesfully!");
                getdH().children[2].className = "message";
                getdH().children[2].innerHTML = footerHtml;
                var fnc = function () { window.parent.location.reload() };
                if (vm.errorID == 0) {
                    var fromDocumentViewer = document.getElementById('fdv').getAttribute('value') == 'true';
                    if (window.parent.parent.location.pathname.toLowerCase().indexOf("external.aspx") == -1) {
                        var $parentWinViewerSecurity = window.parent.window;
                        $parentWinViewerSecurity = $parentWinViewerSecurity.$($parentWinViewerSecurity);
                        var documentViewer = $parentWinViewerSecurity[0].documentViewer;
                        documentViewer.SetViewer("Image");
                        var fnc = function () { window.parent.parent.location.replace(window.parent.parent.location) };
                    }

                    if (vm.changeImage) {
                        getFileLocation();
                    }
                    else {
                        setTimeout(fnc, fromDocumentViewer ? 2000 : 3000);
                    }
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
                getdH().children[2].className = "msgDetails";
                var elem = $("<div class=\"content\">A document with the same name already exists. <br/> Do you want to replace it?</div>");
                $(getdH().children[2]).html(elem);

            }

            else {
                var status = result.Message.indexOf('permissions') == -1 ? 2 : 6;
                if (removeDigest)
                    vm.status = status;
                else
                    $scope.$apply(function () {
                        vm.status = status;
                    });
                var message = result.Message;
                console.error("SFU: " + result.Message);
                getdH().children[2].className = "msgDetails";
                getdH().children[2].innerHTML = "<div class='error' title='" + message + "'><div><img src='/Relativity/CustomPages/1738ceb6-9546-44a7-8b9b-e64c88e47320/Content/Images/Error_Icon.png' /><span>Error: " + message + "</span></div></div>";
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
                getdH().children[2].innerHTML = "Uploading";
                checkUpload();
            })
        }

        function updateStatus(status, message) {

            vm.status = status;

            getdH().children[2].className = "message";
            getdH().children[2].innerHTML = message;
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

            getdH().children[2].innerHTML = "Uploading";
            document.getElementById('force').setAttribute('value', 'true');
            if (bkpFile)
                submitSimulatedForm();
            else
                SubmitFrm();
        }
    }
})();