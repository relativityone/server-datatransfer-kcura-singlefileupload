
(function () {
    'use strict';

    angular
    .module('testapp', [])
    .controller('testctrl', TestController);

    SFUController.$inject = ['$scope', '$http'];

    function TestController($scope, $http) {
        alert('Test');
    }
})();