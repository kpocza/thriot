app.constant('viewUrls', {
        root: '/',
        companies: '/Mgmt/Companies',
        company: '/Mgmt/Company',
        service: '/Mgmt/Service',
        network: '/Mgmt/Network',
        device: '/Mgmt/Device',
        networkReport: '/Mgmt/NetworkReport'
    });

app.service('mgmtApiUrls', function mgmtApiUrls(siteRoots) {
    var self = {
        infoApi: siteRoots.managementRoot + '/info',
        usersApi: siteRoots.managementRoot + '/users',
        companiesApi: siteRoots.managementRoot + '/companies',
        servicesApi: siteRoots.managementRoot + '/services',
        networksApi: siteRoots.managementRoot + '/networks',
        devicesApi: siteRoots.managementRoot + '/devices',
        telemetryMetadataApi: siteRoots.managementRoot + '/telemetryMetadata'
    };

    return self;
});

app.service('reportingApiUrls', function reportingApiUrls(siteRoots) {
    var self = {
        networksApi: siteRoots.reportingRoot + '/networks',
        devicesApi: siteRoots.reportingRoot + '/devices'
    };

    return self;
});

app.constant('sinkTypes', {
        currentData: 1,
        timeSeries: 2
    });