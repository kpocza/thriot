app.directive('dateSelectorDirective', function () {
    return {
        template:
            '<div>' +
            '   <select ng-model="currentYear" ng-options="year as year for year in years"></select> Y' +
            '   <select ng-model="currentMonth" ng-options="month as month for month in months"></select> M' +
            '   <select ng-model="currentDay" ng-options="day as day for day in days"></select> D' +
            '</div>',
        replace: true,
        restrict: 'A',
        scope: {
            timestamp: '=dateSelectorDirective'
        },
        controller: function($scope) {
            function init() {
                $scope.years = [new Date().getUTCFullYear() - 1, new Date().getUTCFullYear()];
                $scope.months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

                $scope.$watch('timestamp', function () {
                    setToToday();

                    var date = new Date($scope.timestamp * 1000);

                    $scope.currentYear = date.getUTCFullYear();
                    $scope.currentMonth = date.getUTCMonth() + 1;
                    $scope.currentDay = date.getUTCDate();
                });
                $scope.$watch('currentYear', function () {
                    recalcDaysList();
                    recalcTimestamp();
                });
                $scope.$watch('currentMonth', function () {
                    recalcDaysList();
                    recalcTimestamp();
                });
                $scope.$watch('currentDay', function () {
                    recalcTimestamp();
                });

                setToToday();
            }

            init();

            function recalcDaysList() {
                var monthVal = parseInt($scope.currentMonth);
                if ([1, 3, 5, 7, 8, 10, 12].contains(monthVal)) {
                    end = 31;
                } else if ([4, 6, 9, 11].contains(monthVal)) {
                    end = 30;
                } else {
                    var yearVal = parseInt($scope.currentYear);
                    var end = 28 + (yearVal % 4 == 0 ? 1 : 0);
                }
                if ($scope.currentDay > end) {
                    $scope.currentDay = 1;
                }

                $scope.days = [];
                for (var d = 1; d <= end; d++) $scope.days.push(d);
            }

            function recalcTimestamp() {
                $scope.timestamp = parseInt((new Date($scope.currentYear, $scope.currentMonth - 1, $scope.currentDay).getTime()
                    - new Date().getTimezoneOffset() * 60000) / 1000, 10);
            }

            function setToToday() {
                if (!$scope.timestamp) {
                    $scope.timestamp = parseInt((new Date().getTime() - new Date().getTimezoneOffset() * 60000) / 1000, 10);
                }
            }
        }
    };
});