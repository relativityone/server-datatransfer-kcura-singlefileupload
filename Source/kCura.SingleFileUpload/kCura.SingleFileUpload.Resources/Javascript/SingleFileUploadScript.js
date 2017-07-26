//======================================================
//V 0.1
//======================================================
var iframe = window.parent.parent.$("#_documentViewer__documentIdentifierFrame").contents();
$('.reviewActionBarTop > .reviewActionBarGroup:first > .reviewActionBarActionButton:first').before('<a class="reviewActionBarActionButton" style="cursor: pointer;" onclick="openSFUModal()">Replace Document</a>');

function openSFUModal() {
    var $myWin = window.parent.parent.window;
    var $out = $myWin.$;
    $myWin = $out($myWin);
    var w = 400;
    var h = 335;
    if ($out('#uploadInfoDiv').length == 0) {
        $out(
            '<div id="uploadInfoDiv" style="height: 335px"><iframe style="border:none;width:100%; height: 335px;" src="/relativity/custompages/1738ceb6-9546-44a7-8b9b-e64c88e47320/sfu.html?AppID={{APPID}}&fdv=true&image=false&newImage=false&docID={{DocID}}"></iframe></div>')
            .dialog({
                autoOpen: false,
                modal: true,
                width: w,
                height: h,
                resizable: false,
                draggable: false,
                closeOnEscape: true,
                open: function () {
                    $dialog.dialog('option', 'width', w);
                    $dialog.dialog('option', 'height', h);
                }
            });
        setTimeout(function () {
            $dialog = $out('#uploadInfoDiv');
            var ntk = $dialog
                .css('padding', '0px')
                .prev().height(0).css('padding', '0px').css('background-color', '#fff')[0].childNodes[0];
            if (ntk.removeNode)
                ntk.removeNode();
            else
                ntk.remove();
            $dialog.dialog('option', 'width', w);
            $dialog.dialog('option', 'height', h);
            $dialog.dialog('open');
            $dialog.width(w);
            $out('.ui-widget-overlay').css('background', 'rgba(54,25,25,.2)').css('opacity', '1');
            $out('.ui-dialog.ui-widget.ui-widget-content.ui-corner-all').css('top', '95px').css('box-shadow', '1px 1px 3px #b7b7b7').css('background-color', '#fff').css('border', '1px solid #c1c1c1');
            $out('.ui-dialog .ui-dialog-titlebar .ui-dialog-titlebar-close').hide();
            $out('.ui-widget-overlay').click(function () {
                $dialog.dialog('close');
            });
        });
    }
    else {
        $out('#uploadInfoDiv').remove();
        setTimeout(openSFUModal(changeImage));
    }
}

