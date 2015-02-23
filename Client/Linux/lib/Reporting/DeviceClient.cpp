#include "ReportingClient.h"
#include "ArduinoJson.h"
#include "../RestConnection.h"
#include <sstream>

DeviceClient::DeviceClient(RestConnection* restConnection)
{
	_restConnection = restConnection;
}

void DeviceClient::SetDevice(const string& deviceId, const string& deviceKey)
{
	_restConnection->ClearRequestHeaders();
	_restConnection->AddRequestHeader("X-DeviceId", deviceId);
	_restConnection->AddRequestHeader("X-DeviceKey", deviceKey);
}

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


string DeviceClient::GetCurrentDataJson(const string& sinkName)
{
	Response httpResponse =_restConnection->Get("devices/json/" + sinkName);

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}

string DeviceClient::GetTimeSeriesJson(const string& sinkName, unsigned long time)
{
	stringstream ss;
	ss << time;

	Response httpResponse =_restConnection->Get("devices/json/" + sinkName + "/" + ss.str());

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}


string DeviceClient::GetCurrentDataCsv(const string& sinkName)
{
	Response httpResponse =_restConnection->Get("devices/csv/" + sinkName);

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}

string DeviceClient::GetTimeSeriesCsv(const string& sinkName, unsigned long time)
{
	stringstream ss;
	ss << time;

	Response httpResponse =_restConnection->Get("devices/csv/" + sinkName + "/" + ss.str());

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}

