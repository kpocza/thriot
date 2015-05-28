app.directive('deviceReportDirective', function () {
    return {
        templateUrl: '/Report/Device',
        replace: true,
        restrict: 'A',
        scope: {
          device: '='  
        },
        controller: function ($scope, $http, $window, mgmtApiUrls, reportingApiUrls, sinkTypes) {
            var currentDataSink = null;
            var timeSeriesSink = null;
            var reqConfig = null;

            function init() {
                $scope.$watch('device', function () {
                    if (!$scope.device) {
                        return;
                    }

                    reqConfig = { headers: { 'X-DeviceId': $scope.device.Id, 'X-DeviceKey': $scope.device.DeviceKey } };
                    $http.get(reportingApiUrls.devicesApi + "/sinks", reqConfig).success(function (sinks) {
                        currentDataSink = sinks.firstOrDefault(function (e) { return e.SinkType == sinkTypes.currentData; });
                        timeSeriesSink = sinks.firstOrDefault(function (e) { return e.SinkType == sinkTypes.timeSeries; });
                    });
                });
            }

            $scope.showCurrentData = function () {
                if (currentDataSink) {
                    $http.get(reportingApiUrls.devicesApi + "/json/" + currentDataSink.SinkName, reqConfig).success(function(data) {
                        if (data != null && data.Devices.length == 1) {
                            var deviceData = data.Devices[0];
                            var payload = deviceData.Payload;
                            var timestamp = deviceData.Timestamp;
                            $scope.currentDataTimestamp = timestamp;
                            $scope.currentData = payload;
                        }
                    });
                }
            }

            $scope.exportCurrentDataCsv = function() {
                if (currentDataSink) {
                    $http.get(reportingApiUrls.devicesApi + "/csv/" + currentDataSink.SinkName, reqConfig).success(function(data) {
                        saveAs(new Blob([data]), 'currentdata' + $scope.device.Id + '.csv');
                    });
                }
            }

            $scope.showTimeSeries = function() {
                if (timeSeriesSink) {
                    $http.get(reportingApiUrls.devicesApi + "/json/" + timeSeriesSink.SinkName + "/" + $scope.timestamp, reqConfig).success(function(data) {
                        if (data != null && data.Devices.length == 1) {
                            $scope.timeSeriesData = data.Devices[0].Data;
                            $scope.graphShown = false;
                        }
                    });
                }
            }

            $scope.showGraph = function () {
                $scope.graphShown = true;
            }

            $scope.exportTimeseriesCsv = function() {
                if (timeSeriesSink) {
                    $http.get(reportingApiUrls.devicesApi + "/csv/" + timeSeriesSink.SinkName + "/" + $scope.timestamp, reqConfig).success(function(data) {
                        saveAs(new Blob([data]), 'timeseries' + $scope.device.Id + '.csv');
                    });
                }
            }

            init();
        }
    };
});