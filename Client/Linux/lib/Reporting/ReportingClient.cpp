
#include "ReportingClient.h"
#include "../RestConnection.h"

using namespace std;

namespace Thriot { namespace Reporting {

/**
Craete a new instance of the Reporting client

@param baseUrl Base Reporting API url
*/
ReportingClient::ReportingClient(const string& baseUrl)
{
	_restConnection = new RestConnection(baseUrl);
	_deviceClient = new DeviceClient(_restConnection);
	_networkClient = new NetworkClient(_restConnection);
}

/** Cleanup the instance */
ReportingClient::~ReportingClient()
{
	delete _networkClient;
	delete _deviceClient;
	delete _restConnection;
}


/**
Return the device-level reporting client

@return */
DeviceClient* ReportingClient::Device()
{
	return _deviceClient;
}

/**
Return the network-level reporting client

@return */
NetworkClient* ReportingClient::Network()
{
	return _networkClient;
}
}}
