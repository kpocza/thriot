
#include "ReportingClient.h"
#include "../RestConnection.h"

using namespace std;

ReportingClient::ReportingClient(const string& baseUrl)
{
	_restConnection = new RestConnection(baseUrl);
	_deviceClient = new DeviceClient(_restConnection);
	_networkClient = new NetworkClient(_restConnection);
}

ReportingClient::~ReportingClient()
{
	delete _networkClient;
	delete _deviceClient;
	delete _restConnection;
}

DeviceClient* ReportingClient::Device()
{
	return _deviceClient;
}

NetworkClient* ReportingClient::Network()
{
	return _networkClient;
}

