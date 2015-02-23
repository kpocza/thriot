function NetworkReportViewModel($scope, $http, $attrs, $window, viewUrls, mgmtApiUrls, reportingApiUrls, sinkTypes) {
    var currentDataSink = null;
    var timeSeriesSink = null;
    var reqConfig = null;

    $scope.showCurrentData = function() {
        $http.get(reportingApiUrls.networksApi + "/json/" + currentDataSink.SinkName, reqConfig).success(function(data) {
            $scope.currentDataList = data.Devices;
        });
    }

    $scope.exportCurrentDataCsv = function () {
        $http.get(reportingApiUrls.networksApi + "/csv/" + currentDataSink.SinkName, reqConfig).success(function (data) {
            saveAs(new Blob([data]), 'currentdata' + $scope.network.Id + '.csv');
        });
    }

    $scope.showTimeSeries = function() {
        $http.get(reportingApiUrls.networksApi + "/json/" + timeSeriesSink.SinkName + "/" + $scope.timestamp, reqConfig).success(function (data) {
            if (data != null && data.Devices.length > 0) {
                $scope.timeSeriesDataList = data.Devices;
                $scope.graphShown = false;
            }
        });
    }

    $scope.showGraph = function () {
        $scope.graphShown = true;
    }

    $scope.exportTimeseriesCsv = function () {
        $http.get(reportingApiUrls.networksApi + "/csv/" + timeSeriesSink.SinkName + "/" + $scope.timestamp, reqConfig).success(function (data) {
            saveAs(new Blob([data]), 'timeseries' + $scope.network.Id + '.csv');
        });
    }

    function init() {
        $scope.networkPageUrl = viewUrls.network + '/' + $attrs.networkId;

        $http.get(mgmtApiUrls.networksApi + '/' + $attrs.networkId)
            .success(function(network) {
                $scope.network = network;
                reqConfig = { headers: { 'X-NetworkId': network.Id, 'X-NetworkKey': network.NetworkKey } };

                $http.get(reportingApiUrls.networksApi + "/sinks", reqConfig).success(function (sinks) {
                    currentDataSink = sinks.firstOrDefault(function (e) { return e.SinkType == sinkTypes.currentData; });
                    timeSeriesSink = sinks.firstOrDefault(function (e) { return e.SinkType == sinkTypes.timeSeries; });
                });
            });
    }
    init();
};
