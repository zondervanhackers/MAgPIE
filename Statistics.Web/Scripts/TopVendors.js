var myVendorApp = angular.module('VendorApp', []);
var category = "FT";
myVendorApp.controller('VendorController', function ($scope, VendorFactory) {
    VendorFactory.getVendors().then(function (result)
    { $scope.Vendors = angular.fromJson(result); },
    function () { alert('Error getting Vendors') })
});

myVendorApp.factory('VendorFactory', function ($http, $q) {
    return {
        getVendors: function () {
            var deferred = $q.defer();

            $http({ method: 'GET', url: '../Journal/TopVendors/?category=FT' }).success(deferred.resolve).error(deferred.reject);

            return deferred.promise;
        }
    }
});