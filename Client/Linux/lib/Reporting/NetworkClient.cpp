#include "ReportingClient.h"
#include "ArduinoJson.h"
#include "../RestConnection.h"
#include <sstream>

namespace Thriot { namespace Reporting {

/**
Create a new network-level reporting client

@param restConnection Class instance that represents the REST connection functionality
*/
NetworkClient::NetworkClient(RestConnection* restConnection)
{
	_restConnection = restConnection;
}

/**
Instructs the client to query telemetry data for the network in the parameter

@param networkId 32-charcters long network id
@param networkKey encapsulating network or service API key
*/
void NetworkClient::SetNetwork(const string& networkId, const string& networkKey)
{
	_restConnection->ClearRequestHeaders();
	_restConnection->AddRequestHeader("X-NetworkId", networkId);
	_restConnection->AddRequestHeader("X-NetworkKey", networkKey);
}

/**
Query the sinks that can be used for generating reports for the network
The function returns a list of valid telemetry data sinks otherwise an uninitialized list.

@return List of available telemetry data sinks*/
vector<SinkInfo> NetworkClient::GetSinks()
{
	vector<SinkInfo> sinkInfos;

	Response httpResponse =_restConnection->Get("networks/sinks");

	if(httpResponse.Code!= 200)
		return sinkInfos;

	DynamicJsonBuffer jsonBufferResponse;
	JsonArray& respJson = jsonBufferResponse.parseArray((char *)httpResponse.Body.c_str());

	for(JsonArray::iterator element = respJson.begin(); element!= respJson.end(); ++element)
	{
		SinkInfo sinkInfo;
		sinkInfo.SinkName = (*element)["SinkName"].asString();
		sinkInfo.Type = (SinkType)(int8_t)((*element)["SinkType"]);
		sinkInfos.push_back(sinkInfo);
	}

	return sinkInfos;
}

/**
Query the last recorded telemetry data by all the devices in the network in JSON

@param sinkName A current data telemetry data sink

@return Telemetry data payload in JSON or empty string on error */
string NetworkClient::GetCurrentDataJson(const string& sinkName)
{
	Response httpResponse =_restConnection->Get("networks/json/" + sinkName);

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}

/**
Query the telemetry data recorded by all the devices in the network in time series format for a given day in JSON

@param sinkName A time series telemetry data sink
@param time Time in a given day anywhere. It's specified as UNIX timestamp (seconds elapsed since 1970.1.1.)

@return Telemetry payload in time series format in JSON or empty string on error */
string NetworkClient::GetTimeSeriesJson(const string& sinkName, unsigned long time)
{
	stringstream ss;
	ss << time;

	Response httpResponse =_restConnection->Get("networks/json/" + sinkName + "/" + ss.str());

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}

/**
Query the last recorded telemetry data by all the devices in the network in CSV

@param sinkName A current data telemetry data sink

@return Telemetry data payload in CSV or empty string on error */
string NetworkClient::GetCurrentDataCsv(const string& sinkName)
{
	Response httpResponse =_restConnection->Get("networks/csv/" + sinkName);

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}

/**
Query the telemetry data recorded by all the devices in the network in time series format for a given day in CSV

@param sinkName A time series telemetry data sink
@param time Time in a given day anywhere. It's specified as UNIX timestamp (seconds elapsed since 1970.1.1.)

@return Telemetry payload in time series format in CSV or empty string on error */
string NetworkClient::GetTimeSeriesCsv(const string& sinkName, unsigned long time)
{
	stringstream ss;
	ss << time;

	Response httpResponse =_restConnection->Get("networks/csv/" + sinkName + "/" + ss.str());

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}
}}

