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

    $scope.activationResend = function() {
        $http.post(mgmtApiUrls.usersApi + '/resendActivationEmail', { email: $scope.userInfo.email })
            .success(function() {
                localStorage.put('messageWarning', 'Please check you email and activate your Thriot account.');
                $window.location.href = '/';
            });
    }

    $scope.sendForgotPasswordEmail = function () {
        $http.post(mgmtApiUrls.usersApi + '/sendForgotPasswordEmail', { email: $scope.userInfo.email })
            .success(function () {
                localStorage.put('messageWarning', 'Please check you email and reset your Thriot password.');
                $window.location.href = '/';
            });
    }

    $scope.resetPassword = function () {
        var hash = $window.location.hash.replace('#', '');
        var hashParts = hash.split(',');
        $http.post(mgmtApiUrls.usersApi + '/resetPassword', { userId: hashParts[0], confirmationCode: hashParts[1], password: $scope.userInfo.password })
            .success(function () {
                localStorage.put('messageWarning', 'Please login with your new password.');
                $window.location.href = '/';
            });
    }

    $scope.changePassword = function () {
        $http.post(mgmtApiUrls.usersApi + '/changePassword', { oldPassword: $scope.userInfo.oldPassword, newPassword: $scope.userInfo.newPassword })
            .success(function () {
                localStorage.put('messageWarning', 'Please try to login with your new password.');
                $window.location.href = '/';
            });
    }
};

function UserStateController($scope, cookies, $window, infoService, viewUrls) {
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

    $scope.changePassword = function() {
        $window.location.href = viewUrls.changePasswordPage;
    }
};

