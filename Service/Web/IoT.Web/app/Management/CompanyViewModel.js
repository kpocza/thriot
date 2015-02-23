function CompanyViewModel($scope, $http, $attrs, $window, viewUrls, mgmtApiUrls, infoService) {
    $scope.services = [];
    $scope.addServiceVisible = false;
    $scope.editCompanyVisible = false;

    $scope.showAddService = function () {
        dismiss();

        $scope.addServiceVisible = true;
        $scope.addService = { Name: '' };
    }

    $scope.addServiceSave = function () {
        dismiss();

        $http.post(mgmtApiUrls.servicesApi, {
            name: $scope.addService.Name,
            companyId: $scope.company.Id
        })
        .success(function () {
            init();
        });
    }

    $scope.manageService = function (id) {
        $window.location.href = viewUrls.service + '/' + id;
    }

    $scope.deleteService = function (id) {
        if (!$window.confirm('Are you sure?'))
            return;

        $http.delete(mgmtApiUrls.servicesApi + '/' + id)
            .success(function () {
                init();
            });
    }

    $scope.showEditCompany = function () {
        dismiss();

        $scope.editCompanyVisible = true;
        $scope.editCompany = { Name: $scope.company.Name };
    }

    $scope.saveCompany = function () {
        dismiss();

        $http.put(mgmtApiUrls.companiesApi, {
            id: $scope.company.Id,
            name: $scope.editCompany.Name
        })
        .success(function () {
            init();
        });
    }

    $scope.cancelInput = function () {
        dismiss();
    }

    function init() {
        infoService.navigateToRightPlace();
        infoService.callWithInfo(function(info) {
            $scope.canNavigateToCompanies = info.CanCreateCompany || info.CanDeleteCompany;
        });

        $http.get(mgmtApiUrls.companiesApi + '/' + $attrs.companyId)
            .success(function (company) {
                $scope.company = company;
            });

        $http.get(mgmtApiUrls.companiesApi + '/' + $attrs.companyId + '/services')
            .success(function (services) {
                $scope.services = services;
            });
    }

    function dismiss() {
        $scope.addServiceVisible = false;
        $scope.editCompanyVisible = false;
    }

    init();
};
