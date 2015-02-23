#include <stdio.h>
#include <iostream>
#include <string.h>
#include "ManagementClient.h"
#include "../RestConnection.h"
#include "ArduinoJson.h"
#include "common.h"

using namespace std;

TelemetryDataSinksMetadataClient::TelemetryDataSinksMetadataClient(RestConnection* restConnection) 
{
	_restConnection = restConnection;
}

TelemetryDataSinksMetadata TelemetryDataSinksMetadataClient::Get()
{
	Response httpResponse =_restConnection->Get("telemetryMetadata");

	TelemetryDataSinksMetadata telemetryDataSinksMetadata;
	
	if(httpResponse.Code != 200)
		return telemetryDataSinksMetadata;

	DynamicJsonBuffer jsonBufferResponse;
	JsonObject& respJson = jsonBufferResponse.parseObject((char *)httpResponse.Body.c_str());
	
	JsonArray& incArray = respJson["Incoming"].asArray();
	for(JsonArray::iterator element = incArray.begin(); element!= incArray.end(); ++element)
	{
		TelemetryDataSinkMetadata telemetryDataSinkMetadata;
		telemetryDataSinkMetadata.Name = (*element)["Name"].asString();
		telemetryDataSinkMetadata.Description = (*element)["Description"].asString();
		JsonArray& paramMap = (*element)["ParametesToInput"].asArray();

		for(JsonArray::iterator param = paramMap.begin(); param!= paramMap.end(); ++param)
		{
			telemetryDataSinkMetadata.ParametersToInput.push_back((*param).asString());
		}
		telemetryDataSinksMetadata.Incoming.push_back(telemetryDataSinkMetadata);
	} 	
	
	return telemetryDataSinksMetadata;
}

