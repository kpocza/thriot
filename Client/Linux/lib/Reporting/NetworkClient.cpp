#include "ReportingClient.h"
#include "ArduinoJson.h"
#include "../RestConnection.h"
#include <sstream>

NetworkClient::NetworkClient(RestConnection* restConnection)
{
	_restConnection = restConnection;
}

void NetworkClient::SetNetwork(const string& networkId, const string& networkKey)
{
	_restConnection->ClearRequestHeaders();
	_restConnection->AddRequestHeader("X-NetworkId", networkId);
	_restConnection->AddRequestHeader("X-NetworkKey", networkKey);
}

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


string NetworkClient::GetCurrentDataJson(const string& sinkName)
{
	Response httpResponse =_restConnection->Get("networks/json/" + sinkName);

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}

string NetworkClient::GetTimeSeriesJson(const string& sinkName, unsigned long time)
{
	stringstream ss;
	ss << time;

	Response httpResponse =_restConnection->Get("networks/json/" + sinkName + "/" + ss.str());

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}


string NetworkClient::GetCurrentDataCsv(const string& sinkName)
{
	Response httpResponse =_restConnection->Get("networks/csv/" + sinkName);

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}

string NetworkClient::GetTimeSeriesCsv(const string& sinkName, unsigned long time)
{
	stringstream ss;
	ss << time;

	Response httpResponse =_restConnection->Get("networks/csv/" + sinkName + "/" + ss.str());

	if(httpResponse.Code!= 200)
		return "";

	return httpResponse.Body;
}

