update "Setting" set "Value"='http://localhost/msvc/v1/messaging' where "Category"='Microservice' AND "Config"='MessagingServiceEndpoint';
update "Setting" set "Value"='http://localhost/papi/v1/telemetryDataSinkSetup' where "Category"='Microservice' AND "Config"='TelemetrySetupServiceEndpoint';

update "Setting" set "Value"='http://ubuntuthriottesthost/api/v1' where "Category"='PublicUrl' AND "Config"='ManagementApiUrl';
update "Setting" set "Value"='http://ubuntuthriottesthost/papi/v1' where "Category"='PublicUrl' AND "Config"='PlatformApiUrl';
update "Setting" set "Value"='http://ubuntuthriottesthost:8080' where "Category"='PublicUrl' AND "Config"='PlatformWsUrl';
update "Setting" set "Value"='http://ubuntuthriottesthost/rapi/v1' where "Category"='PublicUrl' AND "Config"='ReportingApiUrl';
update "Setting" set "Value"='http://ubuntuthriottesthost/central' where "Category"='PublicUrl' AND "Config"='WebsiteUrl';


