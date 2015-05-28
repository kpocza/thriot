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
