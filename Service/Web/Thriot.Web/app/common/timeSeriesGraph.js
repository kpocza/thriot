app.directive('timeSeriesGraphDirective', function () {
    return {
        template:
            '<div>' +
                '<div name="graphPlaceHolder"></div>' +
                '  <div>' +
                '    <span ng-repeat="(prop, val) in properties">' +
                '      <input type="checkbox" name="{{prop}}" ng-checked="val == 1" ng-click="changeSeriesSelection(prop)" /><label for="{{prop}}">{{prop}}</label>' +
                '    </span>' +
                '  </div>' +
                '</div>',
        replace: true,
        restrict: 'A',
        scope: {
            timeSeriesData: '=timeSeriesGraphDirective',
            graphShown: '='
        },
        link: function (scope, element) {
            scope.element = element;
        },
        controller: function ($scope) {
            function init() {
                $scope.$watch('graphShown', function() {
                    if ($scope.graphShown) {
                        showGraph();
                    } else {
                        hideGraph();
                    }
                });
            }

            function getFlot() {
                return $scope.element.find('div[name="graphPlaceHolder"]');
            }

            function showGraph() {
                $scope.element.show();
                var flot = getFlot();
                flot.width(800);
                flot.height(400);
                flot.show();

                $scope.properties = {};

                for (var i = 0, len = $scope.timeSeriesData.length; i < len; i++) {
                    var ts = $scope.timeSeriesData[i];
                    ts.Obj = JSON.parse(ts.Payload);
                    for (var prop in ts.Obj) {
                        if (ts.Obj.hasOwnProperty(prop)) {
                            $scope.properties[prop] = '1';
                        }
                    }
                }

                $scope.series = [];
                for (prop in $scope.properties) {
                    if ($scope.properties.hasOwnProperty(prop)) {
                        var serie = { label: prop, data: [] };
                        for (i = 0, len = $scope.timeSeriesData.length; i < len; i++) {
                            ts = $scope.timeSeriesData[i];

                            if (ts.Obj.hasOwnProperty(prop)) {
                                serie.data.push([ts.Timestamp, ts.Obj[prop]]);
                            }
                        }
                        $scope.series.push(serie);
                    }
                }

                flot.plot($scope.series, { zoom: { interactive: true }, pan: { interactive: true } });
            }

            function hideGraph() {
                var flot = getFlot();
                flot.width(0);
                flot.height(0);
                flot.hide();
                $scope.properties = {};
                $scope.element.hide();
            }

            $scope.changeSeriesSelection = function (prop) {
                var flot = getFlot();
                $scope.properties[prop] = 1 - $scope.properties[prop];

                var currentSeries = [];
                for (var s = 0; s < $scope.series.length; s++) {
                    var serie = $scope.series[s];
                    for (var p in $scope.properties) {
                        if (serie.label == p && $scope.properties[p] == 1) {
                            currentSeries.push(serie);
                            break;
                        }
                    }
                }

                flot.plot(currentSeries, { zoom: { interactive: true }, pan: { interactive: true } });
            }

            init();
        }
    };
});