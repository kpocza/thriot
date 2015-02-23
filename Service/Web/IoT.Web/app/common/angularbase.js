var app = angular.module('iot', ['ngCookies']);

app.factory('authenticationInterceptor', ['cookies', function (cookies) {
    return {
        request: function (config) {
            var authToken = cookies.get('authToken');
            if (authToken) {
                config.headers['Authorization'] = 'Basic ' + authToken;
            }
            return config;
        }
    };
}]);

app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('authenticationInterceptor');
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