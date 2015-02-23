function ServiceViewModel($scope, $http, $attrs, $window, viewUrls, mgmtApiUrls, infoService) {
    $scope.networks = [];
    $scope.addNetworkVisible = false;
    $scope.editServiceVisible = false;

    $scope.showAddNetwork = function () {
        dismiss();

        $scope.addNetworkVisible = true;
        $scope.addNetwork = { Name: '' };
    }

    $scope.addNetworkSave = function () {
        dismiss();

        $http.post(mgmtApiUrls.networksApi, {
            name: $scope.addNetwork.Name,
            companyId: $scope.service.CompanyId,
            serviceId: $scope.service.Id
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

    $scope.showEditService = function () {
        dismiss();

        $scope.editServiceVisible = true;
        $scope.editService = { Name: $scope.service.Name };
    }

    $scope.saveService = function () {
        dismiss();

        $http.put(mgmtApiUrls.servicesApi, {
                name: $scope.editService.Name,
                companyId: $scope.service.CompanyId,
                id: $scope.service.Id
        })
        .success(function () {
            init();
        });
    }

    $scope.cancelInput = function () {
        dismiss();
    }

    function init() {
        infoService.callWithInfo(function (info) {
            $scope.canNavigateToCompany = info.CanCreateService || info.CanDeleteService;
        });

        $http.get(mgmtApiUrls.servicesApi + '/' + $attrs.serviceId)
            .success(function (service) {
                $scope.parentCompanyPageUrl = viewUrls.company + '/' + service.CompanyId;
                $scope.service = service;
            });

        $http.get(mgmtApiUrls.servicesApi + '/' + $attrs.serviceId + '/networks')
            .success(function (networks) {
                $scope.networks = networks;
            });
    }

    function dismiss() {
        $scope.addNetworkVisible = false;
        $scope.editServiceVisible = false;
    }

    init();
};
