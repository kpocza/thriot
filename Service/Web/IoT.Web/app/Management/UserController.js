function UserController($scope, $http, cookies, $window, mgmtApiUrls, infoService, localStorage) {
    $scope.userInfo = {
        email: '',
        name: '',
        password: ''
    }

    $scope.register = function() {
        $http.post(mgmtApiUrls.usersApi + '/register', { email: $scope.userInfo.email, name: $scope.userInfo.name, password: $scope.userInfo.password })
            .success(function(regResponse) {
                if (regResponse.NeedsActivation) {
                    localStorage.put('messageWarning', 'Please check you email and activate your Thriot account.');
                    $window.location.href = '/';
                } else {
                    cookies.set('authToken', regResponse.AuthToken);
                    infoService.navigateToRightPlace(true);
                }
            });
    }

    $scope.login = function() {
        $http.post(mgmtApiUrls.usersApi + '/login', { email: $scope.userInfo.email, password: $scope.userInfo.password })
            .success(function(token) {
                cookies.set('authToken', JSON.parse(token));
                infoService.navigateToRightPlace(true);
            });
    }
};

function UserStateController($scope, cookies, $window, infoService, $document) {
    $scope.logoff = function () {
        cookies.remove('authToken');
        $window.location.href = '/';
    }

    $scope.isLoggedIn = function () {
        return cookies.get('authToken') != null;
    }

    $scope.gotoapp = function() {
        infoService.navigateToRightPlace(true);
    }
};

