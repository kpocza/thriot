app.service('infoService', function InfoService($http, $window, viewUrls, mgmtApiUrls) {
    var self = {
        navigateToRightPlace: _navigateToRightPlace,
        callWithInfo: _callWithInfo
    };

    function _navigateToRightPlace(atLogin) {
        _callWithInfo(function(info) {
            if (info.ServiceProfile == 'SingleCompany') {
                _setIfNotSet(viewUrls.company + '/' + info.PrebuiltCompany);
                return;
            }
            if (info.ServiceProfile == 'SingleService') {
                _setIfNotSet(viewUrls.service + '/' + info.PrebuiltService);
                return;
            }
            if (atLogin) {
                _setIfNotSet(viewUrls.companies);
            }
        });
    }

    function _setIfNotSet(newUrl) {
        if ($window.location.pathname != newUrl) {
            $window.location.href = newUrl;
        }
    }

    var info = null;

    function _callWithInfo(action) {
        if (info) {
            action(info);
        } else {
            $http.get(mgmtApiUrls.infoApi)
                .success(function(infoDto) {
                    info = infoDto;
                    action(info);
                });
        }
    }

    return self;
});
