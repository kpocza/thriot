app.directive('telemetryDataSinksDirective', function () {
    return {
        template:
        '<div>' +
        '  <div ng-if="!editMode">' +
        '    <h4>Telemetry Data Sinks</h4>' +
        '    <div ng-repeat="inc in incoming">' +
        '      {{getIncomingDescription(inc.SinkName)}}<br />' +
        '    </div>' +
        '    <a href="#" ng-click="startEdit()">Edit</a>' +
        '  </div>' +
        '  <div class="embeddedinputform" ng-if="editMode">' +
        '    <h4>Edit Telemetry Data Sinks</h4>' +
        '    <div ng-repeat="inc in incoming">' +
        '      <select ng-model="inc.SinkName" ng-options="incMeta.Name as incMeta.Description for incMeta in incomingMetadata">' +
        '      </select>' +
        '      <div ng-repeat="param in getIncomingParameterMetaData(inc.SinkName)" style="margin-top:5px">' +
        '        <label>{{param}}</label><br /><input type="text" name="{{param}}" ng-model="inc.Parameters[param]" />' +
        '      </div>' +
        '      <a href="#" ng-click="removeIncoming(inc)">Remove Telemetry Data Sink</a>' +
        '      <br />' +
        '      <br />' +
        '    </div>' +
        '    <button ng-click="addIncoming()">Add Telemetry Data Sink</button> <br />' +
        '    <br />' +
        '    <button ng-click="saveIncoming()">Save</button>' +
        '    <a href="#" ng-click="cancelMSInput()">Cancel</a>' +
        '  </div>' +
        '  <br />' +
        '</div>',
        replace: true,
        restrict: 'A',
        scope: {
            companyId: "=",
            serviceId: "=",
            networkId: "="
        },
        controller: function($scope, $http, $window, mgmtApiUrls) {
            $scope.incomingMetadata = [];
            $scope.incoming = [];
            $scope.savedIncoming = '';
            $scope.editMode = false;

            function init() {
                $scope.dismiss();

                $http.get(mgmtApiUrls.telemetryMetadataApi)
                    .success(function (telemetryMetadata) {
                        $scope.incomingMetadata = telemetryMetadata.Incoming;
                        if ($scope.companyId) {
                            $http.get(mgmtApiUrls.companiesApi + '/' + $scope.companyId)
                                .success(function (company) {
                                    loadData(company.TelemetryDataSinkSettings.Incoming);
                                });
                        }
                        if ($scope.serviceId) {
                            $http.get(mgmtApiUrls.servicesApi + '/' + $scope.serviceId)
                                .success(function (service) {
                                    loadData(service.TelemetryDataSinkSettings.Incoming);
                                });
                        }
                        if ($scope.networkId) {
                            $http.get(mgmtApiUrls.networksApi + '/' + $scope.networkId)
                                .success(function (network) {
                                    loadData(network.TelemetryDataSinkSettings.Incoming);
                                });
                        }
                    });

            }

            $scope.addIncoming = function () {
                $scope.incoming.push({ SinkName: null, Parameters: {} });
            }

            $scope.removeIncoming = function (item) {
                $scope.incoming.remove(item);
            }

            $scope.saveIncoming = function () {
                if ($scope.companyId) {
                    $http.post(mgmtApiUrls.companiesApi + '/' + $scope.companyId + '/incomingTelemetryDataSinks', $scope.incoming).success(function () {
                        $scope.dismiss();
                        $scope.savedIncoming = JSON.stringify($scope.incoming);
                    });
                }
                if ($scope.serviceId) {
                    $http.post(mgmtApiUrls.servicesApi + '/' + $scope.serviceId + '/incomingTelemetryDataSinks', $scope.incoming).success(function () {
                        $scope.dismiss();
                        $scope.savedIncoming = JSON.stringify($scope.incoming);
                    });
                }
                if ($scope.networkId) {
                    $http.post(mgmtApiUrls.networksApi + '/' + $scope.networkId + '/incomingTelemetryDataSinks', $scope.incoming).success(function () {
                        $scope.dismiss();
                        $scope.savedIncoming = JSON.stringify($scope.incoming);
                    });
                }
            }

            $scope.cancelMSInput = function () {
                dismissChanges();
                $scope.dismiss();
            }

            $scope.startEdit = function() {
                $scope.editMode = true;
            }

            $scope.dismiss = function() {
                $scope.editMode = false;
            }

            $scope.getIncomingParameterMetaData = function (selectedValue) {
                var selectedItem = $scope.incomingMetadata.firstOrDefault(function (e) { return e.Name == selectedValue; });
                if (selectedItem == null)
                    return [];

                return selectedItem.ParametersToInput;
            }

            $scope.getIncomingDescription = function (selectedValue) {
                var selectedItem = $scope.incomingMetadata.firstOrDefault(function (e) { return e.Name == selectedValue; });
                if (selectedItem == null)
                    return '';

                return selectedItem.Description;
            }

            function loadData(incoming) {
                $scope.incoming = incoming;
                $scope.savedIncoming = JSON.stringify(incoming);
            }

            function dismissChanges() {
                $scope.incoming = JSON.parse($scope.savedIncoming);
            }

            init();
        }
    };
});