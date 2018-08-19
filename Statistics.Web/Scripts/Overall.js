var app = angular.module('plunker', ['rzModule', 'ngAnimate', 'ui.bootstrap']);
var category = "Full Text";
var Months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
var Years = [2010, 2011, 2012, 2013, 2014, 2015];
var startDate = "";
var endDate = "";
var FullText = ["Full Text"];
var RecordViews = ["Record Views"];
var ResultClicks = ["Result Clicks"];
var Searches = ["Searches"];
//
//MAIN CONTROLLER
//
app.controller('MainCtrl', function ($scope, GetInfoFactory, GetChartDataFactory, myFactory, VendorFactory, DatabaseFactory) {
    $scope.dateSlider = {
        min: 30,
        max: 50,
        ceil: 71,
        floor: 0
    };
    $scope.VendorCategory = "Full Text";
    $scope.GroupSort = function(data)
    {
        //Look into sending Json data already sorted.
        //Most likely will have to write a sort function for array of hashes with arrays.
        //When sorting sort inner array first so The top value is the highest.
    }
    $scope.groups = [
    {
        title: 'EbscoHost',
        content: { "Full Text": 30, "Record Views": 190, "Result Clicks": 90, "Searches": 85 }
    },
    {
        title: 'Acedmic Search Premier',
        content: { "Full Text": 50, "Record Views": 110, "Result Clicks": 20, "Searches": 55 }
    },
    {
        title: 'ATLA',
        content: { "Full Text": 120, "Record Views": 90, "Result Clicks": 51, "Searches": 30 }
    },
    {
        title: 'Source',
        content: { "Full Text": 90, "Record Views": 160, "Result Clicks": 53, "Searches": 29 }
    },
    {
        title: 'Source1',
        content: { "Full Text": 100, "Record Views": 130, "Result Clicks": 55, "Searches": 26 }
    }];
    $scope.sliderChanged = function () {
        GetChartDataFactory.GetChartData().then(function (result) {
            FullText = ["Full Text"];
            RecordViews = ["Record Views"];
            ResultClicks = ["Result Clicks"];
            Searches = ["Searches"];
            for (i = 0; i < result.Journals.length; i++) {
                FullText.push(result.Journals[i][0].value);
                RecordViews.push(result.Journals[i][1].value);
                ResultClicks.push(result.Journals[i][2].value);
                Searches.push(result.Journals[i][3].value);
            }
            var numArray = ['Times'];
            for (i = $scope.dateSlider.min; i <= $scope.dateSlider.max; i++) {
                numArray.push(i);
            }
            $scope.chart = c3.generate({
                bindto: '#chart',
                data: {
                    x: 'Times',
                    columns: [
                        numArray,//[Time Axis, 'time1', 'time1', 'time2']
                        FullText,
                        RecordViews,
                        ResultClicks,
                        Searches
                    ],
                    type: 'area-spline'
                },
                axis: {
                    x: {
                        tick: {
                            format: function (x) { return $scope.DateTranslate(x); }
                        }
                    }
                }
            });
            //$scope.chart.hide(['Result Clicks', 'Record Views', 'Searches']);
        },
            function () { alert('Error Retrieving Chart Data'); });
        
        //$scope.SortedVendors = function()
        //{
        //    var sorted = [];
        //    for (var key in $scope.groups) {
        //        sorted[sorted.length] = {category: $scope.groups[key],value: key};
        //    }
        //    return sorted.sort();
        //}
    };
    $scope.init = function () {
        GetInfoFactory.GetInfo().then(
            function (result) {
                Years = [];
                for (i = result.startYear; i <= result.endYear; i++)
                    Years.push(i);
                $scope.dateSlider.floor = result.startMonth;
                if (result.startYear == result.endYear)
                    $scope.dateSlider.ceil = result.endMonth;
                else
                    $scope.dateSlider.ceil = (((result.endYear - 2000) - (result.startYear - 2000)) * 12) + result.endMonth - result.startMonth;
                $scope.dateSlider.min = $scope.dateSlider.floor;
                $scope.dateSlider.max = $scope.dateSlider.ceil;
                //console.log("Min: " + $scope.dateSlider.min + " Max" + $scope.dateSlider.max + " (" + $scope.dateSlider.floor + "-" + $scope.dateSlider.ceil + ")");
                $scope.sliderChanged();
            },
            function () { alert('Error Loading WebPage'); });
    }
    $scope.init();
    $scope.$on("slideEnded", $scope.sliderChanged);
    $scope.DateTranslate = function(datenumber)
    {
        if (datenumber <= 0) {
            return Months[0] + " " + Years[0];
        }
        else if (datenumber >= $scope.dateSlider.ceil) {
            return Months[Months.length - 1] + " " + Years[Years.length - 1];
        }
        else {
            var tempyear = Math.floor(datenumber / Months.length);
            var tempmonth = Math.floor(datenumber - (Months.length * tempyear));
            return Months[tempmonth] + " " + Years[tempyear];
        }
    }
    myFactory.getFullText().then(
    function (result) { $scope.Overall = result; },
    function () { alert('Error getting FullText Data'); });

    VendorFactory.getVendors().then(
        function (result) { $scope.Vendors = result; },
        function () { alert('Error getting Vendor Data'); });

    DatabaseFactory.getDatabases().then(
        function (result) { $scope.Databases = result; },
        function () { alert('Error getting Database Data'); });

    $scope.setCategory = function ($event, $category) {
        if (document.getElementById($category).classList.contains("selected")) {
            document.getElementById($category).classList.remove("selected");
            $scope.chart.hide($category);
        }
        else {
            document.getElementById($category).classList.add("selected");
            $scope.chart.show($category);
        }

        VendorFactory.getVendors().then(
            function (result) { $scope.Vendors = result; },
            function () { alert('Error getting Vendors'); });

        DatabaseFactory.getDatabases().then(
            function (result) { $scope.Databases = result; },
            function () { alert('Error getting Databases'); });
    };
});
//
//FACTORIES
//
app.factory('myFactory', function ($http, $q) {
    return {
        getFullText: function () {
            var deferred = $q.defer();

            $http({ method: 'GET', url: '../Journal/Overall' }).success(deferred.resolve).error(deferred.reject);

            return angular.fromJson(deferred.promise);
        }
    }
});
app.factory('VendorFactory', function ($http, $q) {
    return {
        getVendors: function () {
            var deferred = $q.defer();

            $http({ method: 'GET', url: '../Journal/TopVendors/?category='.concat(category) }).success(deferred.resolve).error(deferred.reject);

            return angular.fromJson(deferred.promise);
        }
    }
});
app.factory('DatabaseFactory', function ($http, $q) {
    return {
        getDatabases: function () {
            var deferred = $q.defer();

            $http({ method: 'GET', url: '../Journal/TopDatabases/?category='.concat(category) }).success(deferred.resolve).error(deferred.reject);

            return angular.fromJson(deferred.promise);
        }
    }
});
app.factory('GetInfoFactory', function ($http, $q) {
    return {
        GetInfo: function () {
            var deffered = $q.defer();

            $http({ method: 'GET', url: '../Journal/BeginInfo' }).success(deffered.resolve).error(deffered.reject);

            return angular.fromJson(deffered.promise);
        }
    }
});
app.factory('GetChartDataFactory', function ($http, $q) {
    return {
        GetChartData: function () {
            var deffered = $q.defer();

            $http({ method: 'GET', url: '../Journal/ChartData' }).success(deffered.resolve).error(deffered.reject);

            return angular.fromJson(deffered.promise);
        }
    }
});