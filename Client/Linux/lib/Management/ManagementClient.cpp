#include "ManagementClient.h"
#include "../RestConnection.h"

using namespace Thriot;

namespace Thriot { namespace Management {

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

UserManagementClient* ManagementClient::User()
{
	return _userManagementClient;
}

CompanyManagementClient* ManagementClient::Company()
{
	return _companyManagementClient;
}

ServiceManagementClient* ManagementClient::Service()
{
	return _serviceManagementClient;
}

NetworkManagementClient* ManagementClient::Network()
{
	return _networkManagementClient;
}

DeviceManagementClient* ManagementClient::Device()
{
	return _deviceManagementClient;
}

TelemetryDataSinksMetadataClient* ManagementClient::TelemetryDataSinksMetadata()
{
	return _telemetryDataSinksMetadataClient;
}
}}
