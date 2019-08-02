sessionStorage['____pushNo'] = '';
var bkpFile = null;
var files; var idCheckTimeout;
function getdH() { return document.getElementById('dropHere'); }
function checkUpload() {
    var resultString = sessionStorage['____pushNo'] || '';
    if (resultString) {
        sessionStorage['____pushNo'] = '';
        manageResult(JSON.parse(resultString));
    }
    else
        idCheckTimeout = setTimeout(checkUpload, 500);
}
function manageResult(result) {
    if (result.Success) {   
        var fromDocumentViewer = document.getElementById('fdv').getAttribute('value') == 'true';
        getdH().children[1].className = "fa fa-check-circle fa-5x";
        var footerHtml = "<div>Document uploaded successfully!</div>";
        if (!fromDocumentViewer)
            footerHtml += '<a href="/Relativity/Case/Document/Review.aspx?' + result.Data + '&profilerMode=View&ArtifactTypeID=10&useNewSource=true" target="_top">Open Document<a>';
        getdH().children[2].className = "messageWithEvents";
        getdH().children[2].innerHTML = footerHtml;
        var fnc = function () { window.parent.location.reload() };
        if (window.parent.parent.location.pathname.toLowerCase().indexOf("external.aspx") == -1){
            var $parentWinViewerSecurity = window.parent.window;
            $parentWinViewerSecurity = $parentWinViewerSecurity.$($parentWinViewerSecurity);
            var documentViewer = $parentWinViewerSecurity[0].documentViewer;
            documentViewer.SetViewer("Long Text");
            var fnc = function () { window.parent.parent.location.replace(window.parent.parent.location) };
        }
        setTimeout(fnc, fromDocumentViewer ? 1000 : 2000);
    }
    else if (result.Message == 'R') {
        getdH().children[1].className = "msgDetails";
        getdH().children[1].innerHTML = "<div class=\"title\">Replace Document</div><div class=\"content\">A document with the same name already exists. Do you want to replace it?</div>";
        getdH().children[2].className = "buttonsZone";
        getdH().children[2].innerHTML = "<button class=\"btnPrimary\" onclick=\"forceUpload()\">Replace Document</button><button onclick=\"window.location.reload()\">Cancel</button>";
    }
    else {
        getdH().children[1].className = "fa fa-exclamation-triangle fa-5x";
        getdH().children[2].innerHTML = result.Message;
        setTimeout(function () { location.reload() }, 1000);
    }
}
function submitFrm() {
    document.getElementById('fid').setAttribute('value', getFolder());
    document.getElementById('did').setAttribute('value', GetDID());
    document.getElementById('btiForm').submit();
    notifyUploadStarted();
}
function notifyUploadStarted() {
    setTimeout(function () {
        getdH().children[1].innerHTML = "";
        getdH().children[1].className = "fa fa-cog fa-spin fa-5x fa-fw";
        getdH().children[2].className = "message"
        getdH().children[2].innerHTML = "Uploading";
        getdH().onclick = function () { };
        getdH().ondrop = function () { };
        checkUpload();
    })
}
function stopPropagation(event) {
    event.stopPropagation();
    event.preventDefault();
}
function setDropStyle(color) {
    getdH().style['color'] = color;
}
function handleDragOver(event) {
    stopPropagation(event);
    setDropStyle('#BBB');
}
function handleDragLeave(event) {
    stopPropagation(event);
    setDropStyle('#000');
}
function handleDnDFileSelect(event) {
    stopPropagation(event);
    setDropStyle('#000');

    files = event.dataTransfer.files;

    bkpFile = files[0];
    submitSimulatedForm();
}
function submitSimulatedForm() {
    var form = document.getElementById('btiFormDD');
    var data = new FormData(form);
    data.append('file', bkpFile);
    data.append('meta', { 'fid': getFolder(), 'fdv': document.getElementById('fdv').getAttribute('value'), 'did': GetDID(), 'force': document.getElementById('force').getAttribute('value') });   

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
function simulateFileClick() {

    document.getElementById('file').click();
}  
function getFolder() {    
    var id = '-1';
    var wN = window.parent.frames['externalPage'] || window.parent.parent.frames['externalPage'];
    if (wN){
        var $out = wN.window.$;
        if ($out('.browser-folder.browser-icon-active', wN.document).length)
            id = $out('.jstree-node.jstree-leaf[aria-selected=true]', wN.document).attr('id');
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
    if (document.getElementById('fdv').getAttribute('value') == 'true')
        did = GetQueryStringValueByName(window.parent.location.search, "ArtifactID");
    return did;
}
function forceUpload() {
    document.getElementById('force').setAttribute('value', 'true');
    if (bkpFile)
        submitSimulatedForm();
    else
        submitFrm();
}