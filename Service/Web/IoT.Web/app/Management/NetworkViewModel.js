function NetworkViewModel($scope, $http, $attrs, $window, viewUrls, mgmtApiUrls) {
    $scope.devices = [];
    $scope.networks = [];
    $scope.addNetworkVisible = false;
    $scope.editServiceVisible = false;

    $scope.showAddDevice = function () {
        dismiss();

        $scope.addDeviceVisible = true;
        $scope.addDevice = { Name: '' };
    }

    $scope.addDeviceSave = function () {
        dismiss();

        $http.post(mgmtApiUrls.devicesApi, {
            name: $scope.addDevice.Name,
            companyId: $scope.network.CompanyId,
            serviceId: $scope.network.ServiceId,
            networkId: $scope.network.Id
        })
        .success(function () {
              init();
        });
    }

    $scope.manageDevice = function (id) {
        $window.location.href = viewUrls.device + '/' + id;
    }

    $scope.deleteDevice = function (id) {
        if (!$window.confirm('Are you sure?'))
            return;

        $http.delete(mgmtApiUrls.devicesApi + '/' + id)
            .success(function () {
                init();
            });
    }

    $scope.showAddNetwork = function () {
        dismiss();

        $scope.addNetworkVisible = true;
        $scope.addNetwork = { Name: '' };
    }

    $scope.addNetworkSave = function () {
        dismiss();

        $http.post(mgmtApiUrls.networksApi, {
            name: $scope.addNetwork.Name,
            companyId: $scope.network.CompanyId,
            serviceId: $scope.network.ServiceId,
            parentNetworkId: $scope.network.Id
        })
        .success(function () {
            init();
        });
    }

    $scope.manageNetwork = function (id) {
        $window.location.href = viewUrls.network + '/' + id;
    }

    $scope.deleteNetwork = function (id) {
        if (!$window.confirm('Are you sure?'))
            return;

        $http.delete(mgmtApiUrls.networksApi + '/' + id)
            .success(function () {
                init();
            });
    }

    $scope.showEditNetwork = function () {
        dismiss();

        $scope.editNetworkVisible = true;
        $scope.editNetwork = { Name: $scope.network.Name };
    }

    $scope.saveNetwork = function () {
        dismiss();

        $http.put(mgmtApiUrls.networksApi, {
                name: $scope.editNetwork.Name,
                companyId: $scope.network.CompanyId,
                serviceId: $scope.network.ServiceId,
                id: $scope.network.Id
        })
        .success(function () {
            init();
        });
    }

    $scope.cancelInput = function () {
        dismiss();
    }

    function init() {
        $scope.networkReportPageUrl = viewUrls.networkReport + '/' + $attrs.networkId;

        $http.get(mgmtApiUrls.networksApi + '/' + $attrs.networkId)
            .success(function (network) {
                $scope.parentServicePageUrl = viewUrls.service + '/' + network.ServiceId;
                if (network.ParentNetworkId) {
                    $scope.parentNetworkPageUrl = viewUrls.network + '/' + network.ParentNetworkId;
                }
                $scope.network = network;
            });

        $http.get(mgmtApiUrls.networksApi + '/' + $attrs.networkId + '/networks')
            .success(function (networks) {
                $scope.networks = networks;
            });

        $http.get(mgmtApiUrls.networksApi + '/' + $attrs.networkId + '/devices')
            .success(function (devices) {
                $scope.devices = devices;
            });
    }

    function dismiss() {
        $scope.addNetworkVisible = false;
        $scope.addDeviceVisible = false;
        $scope.editNetworkVisible = false;
    }

    init();
};
