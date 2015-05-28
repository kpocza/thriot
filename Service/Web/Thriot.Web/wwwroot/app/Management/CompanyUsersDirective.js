app.directive('companyUsersDirective', function () {
    return {
        template:
        '<div>' +
        '    <h3>Users</h3>' +
        '    <table class="item-list" width="100%">' +
        '        <tr>' +
        '            <th width="50%">Name</th>' +
        '            <th width="30%">Email</th>' +
        '            <th>Operations</th>' +
        '        </tr>' +
        '        <tr ng-repeat="user in users">' +
        '            <td>{{user.Name}}</td>' +
        '            <td>{{user.Email}}</td>' +
        '            <td><a href="#" ng-click="viewUser(user.Id)">View</td>' +
        '        </tr>' +
        '    </table>' +
        '    <br />' +
        '    <div ng-if="!addUserVisible">' +
        '        <button ng-click="addUserStart()">Add User</button>' +
        '    </div>' +
        '    <div class="embeddedinputform" ng-if="addUserVisible">' +
        '        <label class="thinlabelwidth" for="addEmail">Email</label><input type="text" name="addEmail" ng-model="addUser.email" /><br />' +
        '        <button ng-click="addUserSave()">Add</button>' +
        '        <a href="#" ng-click="cancelInput()">Cancel</a>' +
        '    </div>' +
        '    <br />' +
        '</div>',
        replace: true,
        restrict: 'A',
        scope: {
            companyId: "=",
        },
        controller: function ($scope, $http, mgmtApiUrls) {
            $scope.users = [];
            $scope.addUser = {};
            $scope.addUserVisible = false;

            function init() {
                _loadList();

                $scope.$watch('companyId', _loadList);
            }

            function _loadList() {
                dismiss();

                if (!$scope.companyId) {
                    return;
                }

                $http.get(mgmtApiUrls.companiesApi + '/' + $scope.companyId + '/users')
                    .success(function (users) {
                        $scope.users = users;
                    });
            }

            $scope.addUserStart = function () {
                $scope.addUserVisible = true;
                $scope.addUser = {};
            }

            $scope.addUserSave = function() {
                $http.get(mgmtApiUrls.usersApi + '/byemail/' + encodeURIComponent($scope.addUser.email) + '/')
                    .success(function(user) {
                        if (user.Id) {
                            $http.post(mgmtApiUrls.companiesApi + '/adduser/', { companyId: $scope.companyId, userId: user.Id })
                                .success(function() {
                                    _loadList();
                                }).error(function() { dismiss(); });
                        }
                    });
            }

            $scope.cancelInput = function() {
                dismiss();
            }

            function dismiss() {
                $scope.addUserVisible = false;
                $scope.addUser = {};
            }

            init();
        }
    };
});