function DeviceViewModel($scope, $http, $attrs, viewUrls, mgmtApiUrls, reportingApiUrls, sinkTypes) {
    $scope.editDeviceVisible = false;

    $scope.showEditDevice = function () {
        dismiss();

        $scope.editDeviceVisible = true;
        $scope.editDevice = { Name: $scope.device.Name };
    }

    $scope.saveDevice = function () {
        dismiss();

        $http.put(mgmtApiUrls.devicesApi, {
                name: $scope.editDevice.Name,
                companyId: $scope.device.CompanyId,
                serviceId: $scope.device.ServiceId,
                networkId: $scope.device.NetworkId,
                id: $scope.device.Id
        })
        .success(function () {
            init();
        });
    }

    $scope.cancelInput = function () {
        dismiss();
    }

    $scope.showCurrentData = function() {
        if ($scope.device) {
            var reqConfig = { headers: { 'X-DeviceId': $scope.device.Id, 'X-DeviceKey': $scope.device.DeviceKey } };

            $http.get(reportingApiUrls.devicesApi + "/sinks", reqConfig).success(function(sinks) {
                var currentDataSink = sinks.firstOrDefault(function (e) { return e.SinkType == sinkTypes.currentData; });
                $http.get(reportingApiUrls.devicesApi + "/json/" + currentDataSink.SinkName, reqConfig).success(function (data) {
                    if (data != null && data.Devices.length == 1) {
                        var deviceData = data.Devices[0];
                        var payload = deviceData.Payload;
                        var timestamp = deviceData.Timestamp;
                        $scope.currentDataTimestamp = timestamp;
                        $scope.currentData = payload;
                    }
                });
            });
        }
    }

    $scope.exportCurrentDataCsv = function () {
        if ($scope.device) {
            var reqConfig = { headers: { 'X-DeviceId': $scope.device.Id, 'X-DeviceKey': $scope.device.DeviceKey } };

            $http.get(reportingApiUrls.devicesApi + "/sinks", reqConfig).success(function (sinks) {
                var currentDataSink = sinks.firstOrDefault(function (e) { return e.SinkType == sinkTypes.currentData; });
                $http.get(reportingApiUrls.devicesApi + "/csv/" + currentDataSink.SinkName, reqConfig).success(function (data) {
                    saveAs(new Blob([data]), 'currentdata' + $scope.device.Id + '.csv');
                });
            });
        }
    }

    $scope.showTimeSeries = function () {
        if ($scope.device) {
            var reqConfig = { headers: { 'X-DeviceId': $scope.device.Id, 'X-DeviceKey': $scope.device.DeviceKey } };

            $http.get(reportingApiUrls.devicesApi + "/sinks", reqConfig).success(function(sinks) {
                var timeSeriesSink = sinks.firstOrDefault(function(e) { return e.SinkType == sinkTypes.timeSeries; });
                $http.get(reportingApiUrls.devicesApi + "/json/" + timeSeriesSink.SinkName + "/" + $scope.timestamp, reqConfig).success(function (data) {
                    if (data != null && data.Devices.length == 1) {
                        $scope.timeSeriesData = data.Devices[0].Data;
                        $scope.graphShown = false;
                    }
                });
            });
        }
    }

    $scope.showGraph = function () {
        $scope.graphShown = true;
    }

    $scope.exportTimeseriesCsv = function () {
        if ($scope.device) {
            var reqConfig = { headers: { 'X-DeviceId': $scope.device.Id, 'X-DeviceKey': $scope.device.DeviceKey } };

            $http.get(reportingApiUrls.devicesApi + "/sinks", reqConfig).success(function (sinks) {
                var timeSeriesSink = sinks.firstOrDefault(function (e) { return e.SinkType == sinkTypes.timeSeries; });
                $http.get(reportingApiUrls.devicesApi + "/csv/" + timeSeriesSink.SinkName + "/" + $scope.timestamp, reqConfig).success(function (data) {
                    saveAs(new Blob([data]), 'timeseries' +  $scope.device.Id + '.csv');
                });
            });
        }
    }

    function init() {
        $scope.timestamp = null;

        $http.get(mgmtApiUrls.devicesApi + '/' + $attrs.deviceId)
            .success(function (device) {
                $scope.parentServicePageUrl = viewUrls.service + '/' + device.ServiceId;
                $scope.parentNetworkPageUrl = viewUrls.network + '/' + device.NetworkId;
                $scope.device = device;
            });
    }

    function dismiss() {
        $scope.editDeviceVisible = false;
    }

    init();
};
