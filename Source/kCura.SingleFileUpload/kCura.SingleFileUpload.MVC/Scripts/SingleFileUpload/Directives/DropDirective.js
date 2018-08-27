'use strict';
dropOnMe.$inject = [];
function dropOnMe() {
    var DDO = {
        restrict: 'A',
        link: function (scope, element) {
            element[0].addEventListener('dragover', function (event) {
                scope.handleDragOver(event)
            });
            element[0].addEventListener('dragleave', function (event) {
                scope.handleDragLeave(event)
            });
            element[0].addEventListener('drop', function (event) {
                scope.handleDnDFileSelect(event)
            });
        }
    };
    return DDO;
}