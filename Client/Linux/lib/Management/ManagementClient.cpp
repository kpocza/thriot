#include "ManagementClient.h"
#include "../RestConnection.h"

using namespace Thriot;

namespace Thriot { namespace Management {

/**
Initializes a new management client that encapsulates all entity operations.

@param baseUrl Base url of the Management API
*/
ManagementClient::ManagementClient(const string& baseUrl)
{
	_restConnection = new RestConnection(baseUrl);

	_userManagementClient = new UserManagementClient(_restConnection);
	_companyManagementClient = new CompanyManagementClient(_restConnection);
	_serviceManagementClient = new ServiceManagementClient(_restConnection);
	_networkManagementClient = new NetworkManagementClient(_restConnection);
	_deviceManagementClient = new DeviceManagementClient(_restConnection);
	_telemetryDataSinksMetadataClient = new TelemetryDataSinksMetadataClient(_restConnection);
}

/**
Cleanup the management client
*/
ManagementClient::~ManagementClient()
{
	delete _telemetryDataSinksMetadataClient;
	delete _deviceManagementClient;
	delete _networkManagementClient;
	delete _serviceManagementClient;
	delete _companyManagementClient;
	delete _userManagementClient;
	delete _restConnection;
}

/**
Return the User management client instance

@return User management client instance*/
UserManagementClient* ManagementClient::User()
{
	return _userManagementClient;
}

/**
Return the Company management client instance

@return Company management client instance*/
CompanyManagementClient* ManagementClient::Company()
{
	return _companyManagementClient;
}

/**
Return the Service management client instance

@return Service management client instance*/
ServiceManagementClient* ManagementClient::Service()
{
	return _serviceManagementClient;
}

/**
Return the Network management client instance

@return Network management client instance*/
NetworkManagementClient* ManagementClient::Network()
{
	return _networkManagementClient;
}

/**
Return the Device management client instance

@return Device management client instance*/
DeviceManagementClient* ManagementClient::Device()
{
	return _deviceManagementClient;
}

/**
Return the Telemetry data sink metadata management client instance

@return Telemetry data sink metadata management client instance*/
TelemetryDataSinksMetadataClient* ManagementClient::TelemetryDataSinksMetadata()
{
	return _telemetryDataSinksMetadataClient;
}
}}
