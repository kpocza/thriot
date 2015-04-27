app.directive('urlInfoDirective', function () {
    return {
        template:
        '<div>' +
        '    <h3>Api access urls</h3>' +
        '    <div ng-if="!loaded">' +
        '        <a href="#" ng-click="showUrls()">Show urls</a>' +
        '        <br />' +
        '    </div>' +
        '    <div ng-if="loaded">' +
        '        <b>Website url:</b><br />' +
        '        <em>{{urlInfo.WebsiteUrl}}</em>' +
        '        <br />' +
        '        <b>Management API url:</b><br />' +
        '        <em>{{urlInfo.ManagementApiUrl}}</em>' +
        '        <br />' +
        '        <b>Platform API url:</b><br />' +
        '        <em>{{urlInfo.PlatformApiUrl}}</em>' +
        '        <br />' +
        '        <b>Platform Websockets url:</b><br />' +
        '        <em>{{urlInfo.PlatformWsUrl}}</em>' +
        '        <br />' +
        '        <b>Reporting API url:</b><br />' +
        '        <em>{{urlInfo.ReportingApiUrl}}</em>' +
        '        <br />' +
        '    </div>' +
        '</div>',
        replace: true,
        restrict: 'A',
        controller: function ($scope, $http, mgmtApiUrls) {
            $scope.loaded = false;

            $scope.showUrls = function() {
                $http.get(mgmtApiUrls.infoApi + '/url')
                    .success(function (urls) {
                        $scope.urlInfo = urls;
                        $scope.loaded = true;
                });
            }
        }
    };
});