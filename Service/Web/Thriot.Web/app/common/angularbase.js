var app = angular.module('iot', ['ngCookies']);

app.factory('authenticationInterceptor', ['cookies', function (cookies) {
    return {
        request: function (config) {
            return config;
        }
    };
}]);

app.factory('errorHandlerInterceptor', ['$window', 'localStorage', 'viewUrls', function ($window, localStorage, viewUrls) {
    return {
        responseError: function (resp) {
            var status = resp.status;
            var statusText = resp.statusText;
            var data = resp.data;

            localStorage.put('lastErrorParameters',
                JSON.stringify({ status: status, statusText: statusText, data: data }));

            $window.location.href = viewUrls.errorPage;
        }
    };
}]);

app.factory('loadingIndicatorInterceptor', ['$q', '$rootScope', function ($q, $rootScope) {
    return {
        request: function(config) {
            $rootScope.$broadcast('http-loading-started');
            return config || $q.when(config);
        },
        response: function (response) {
            $rootScope.$broadcast('http-loading-complete');
            return response || $q.when(response);
        }
    };
}]);

app.directive('loadingIndicator', function() {
    return {
        restrict: 'A',
        template: '<div ng-if="showIndicator">Loading...</div>',
        controller: function ($scope, $interval) {
            $scope.showCounter = 0;
            $scope.hideCounter = 0;
            $scope.$on("http-loading-started", function (e) {
                $scope.showCounter++;
            });

            $scope.$on("http-loading-complete", function (e) {
                $scope.hideCounter++;
                $scope.showIndicator = false;
            });
            $interval(function() {
                if ($scope.showCounter > $scope.hideCounter) {
                    $scope.showIndicator = true;
                } else {
                    $scope.showIndicator = false;
                }
            }, 1000);
        }
    };
});

app.directive('systemMessages', function ($interval, localStorage) {
    return {
        restrict: 'A',
        replace: true,
        template: '<div></div>',
        link: function (scope, element) {
            function processMessage(messageSlot, messageClass) {
                var msg = localStorage.fetch(messageSlot);
                if (msg) {
                    element.append('<br />');
                    var alertBox = $('<div>',
                    {
                        'class': 'alert ' + messageClass + ' alert-dismissible',
                        html: msg,
                        role: 'alert'
                    });
                    alertBox.prepend($('<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>'));
                    element.append(alertBox);
                }
            }
            $interval(function () {
                processMessage('messageError', 'alert-danger');
                processMessage('messageWarning', 'alert-warning');
                processMessage('messageInfo', 'alert-info');
            }, 250);
        }
    };
});

app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('authenticationInterceptor');
    $httpProvider.interceptors.push('errorHandlerInterceptor');
    $httpProvider.interceptors.push('loadingIndicatorInterceptor');
}]);

app.factory('cookies', function() {
    return {
        get: function(name) {
            return $.cookie(name);
        },
        set: function(name, value) {
            $.cookie(name, value, { path: '/' });
        },
        remove: function(name) {
            $.removeCookie(name, { path: '/' });
        }
    }
});

app.factory('localStorage', ['$window', function ($window) {
    return {
        get: function(key) {
            return $window.localStorage.getItem(key);
        },
        put: function(key, value) {
            $window.localStorage.setItem(key, value);
        },
        remove: function(key) {
            $window.localStorage.removeItem(key);
        },
        fetch: function (key) {
            var val = $window.localStorage.getItem(key);
            if (val) {
                $window.localStorage.removeItem(key);
            }
            return val;
        },
    };
}]);