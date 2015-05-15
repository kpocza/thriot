#include "ReportingClient.h"
#include "ArduinoJson.h"
#include "../RestConnection.h"
#include <sstream>

namespace Thriot { namespace Reporting {

/**
Create a new device-level reporting client

@param restConnection Class instance that represents the REST connection functionality
*/
DeviceClient::DeviceClient(RestConnection* restConnection)
{
	_restConnection = restConnection;
}


/**
Instructs the client to query telemetry data for the device in the parameter

@param deviceId 32-charcters long device id
@param deviceKey device API key, or encapsulating network or service API key
*/
void DeviceClient::SetDevice(const string& deviceId, const string& deviceKey)
{
	_restConnection->ClearRequestHeaders();
	_restConnection->AddRequestHeader("X-DeviceId", deviceId);
	_restConnection->AddRequestHeader("X-DeviceKey", deviceKey);
}

/**
Query the sinks that can be used for generating reports for the device
The function returns a list of valid telemetry data sinks otherwise an uninitialized list.

@return List of available telemetry data sinks*/
vector<SinkInfo> DeviceClient::GetSinks()
{
	vector<SinkInfo> sinkInfos;

	Response httpResponse =_restConnection->Get("devices/sinks");

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
Query the last recorded telemetry data by the device in JSON

@param sinkName A current data telemetry data sink

@return Telemetry data payload in JSON or empty string on error */
string DeviceClient::GetCurrentDataJson(const string& sinkName)
{
	Response httpResponse =_restConnection->Get("devices/json/" + sinkName);

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}

/**
Query the telemetry data recorded by the device in time series format for a given day in JSON

@param sinkName A time series telemetry data sink
@param time Time in a given day anywhere. It's specified as UNIX timestamp (seconds elapsed since 1970.1.1.)

@return Telemetry payload in time series format in JSON or empty string on error */
string DeviceClient::GetTimeSeriesJson(const string& sinkName, unsigned long time)
{
	stringstream ss;
	ss << time;

	Response httpResponse =_restConnection->Get("devices/json/" + sinkName + "/" + ss.str());

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}


/**
Query the last recorded telemetry data by the device in CSV

@param sinkName A current data telemetry data sink

@return Telemetry data payload in CSV or empty string on error */
string DeviceClient::GetCurrentDataCsv(const string& sinkName)
{
	Response httpResponse =_restConnection->Get("devices/csv/" + sinkName);

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}

/**
Query the telemetry data recorded by the device in time series format for a given day in CSV

@param sinkName A time series telemetry data sink
@param time Time in a given day anywhere. It's specified as UNIX timestamp (seconds elapsed since 1970.1.1.)

@return Telemetry payload in time series format in CSV or empty string on error */
string DeviceClient::GetTimeSeriesCsv(const string& sinkName, unsigned long time)
{
	stringstream ss;
	ss << time;

	Response httpResponse =_restConnection->Get("devices/csv/" + sinkName + "/" + ss.str());

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}
}}

