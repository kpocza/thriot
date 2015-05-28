function CompaniesViewModel($scope, $http, $window, viewUrls, mgmtApiUrls, infoService) {
    $scope.services = [];
    $scope.addCompanyVisible = false;

    $scope.showAdd = function() {
        $scope.addCompanyVisible = !$scope.addCompanyVisible;
        $scope.addCompany = { Name: '' };
    }

    $scope.manageCompany = function(id) {
        $window.location.href = viewUrls.company + '/' + id;
    }

    $scope.add = function() {
        $scope.addCompanyVisible = false;

        $http.post(mgmtApiUrls.companiesApi, { name: $scope.addCompany.Name })
            .success(function() {
            init();
        });
    }

    $scope.cancel = function () {
        $scope.addCompanyVisible = false;
    }

    $scope.delete = function (id) {
        if (!$window.confirm('Are you sure?'))
            return;

        $http.delete(mgmtApiUrls.companiesApi + '/' + id)
            .success(function () {
                init();
            });
    }

    function init() {
        infoService.navigateToRightPlace();

        $http.get(mgmtApiUrls.companiesApi)
            .success(function (companies) {
                $scope.companies = companies;
            });

        $http.get(mgmtApiUrls.infoApi)
            .success(function (info) {
                if (info.ServieProfile == 'SingleCompany') {
                    $window.location.href = viewUrls.company + '/' + info.PrebuiltCompany;
                }
                if (info.ServieProfile == 'SingleSingle') {
                    $window.location.href = viewUrls.service + '/' + info.PrebuiltService;
                }
            });
    }

    init();
};
