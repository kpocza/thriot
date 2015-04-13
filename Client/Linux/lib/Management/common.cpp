#include "common.h"
#include <cctype>
#include <iomanip>
#include <sstream>

namespace Thriot
{
namespace Management
{
void StripQuotes(std::string &str)
{
	str.replace(0, 1, "");
	str.replace(str.length()-1, 1, "");
}
std::string UrlEncode(const std::string &str)
{
	std::ostringstream escaped;
	escaped.fill('0');
	escaped << hex;

	for(string::const_iterator i = str.begin(); i!= str.end();++i)
	{
		std::string::value_type c = (*i);

		if(isalnum(c) || c =='-' || c == '_' || c == '.' || c == '~')
		{
			escaped << c;
		}
		else
		{
			escaped << '%' << setw(2) << int((unsigned char)c);
		}
	}
	return escaped.str();
}

void ParseTelemetryDataSinkSettings(JsonObject& jsonObject, TelemetryDataSinkSettingsType& telemetryDataSinkSettings)
{
	if(jsonObject.containsKey("TelemetryDataSinkSettings"))
	{
		JsonObject& tdss = jsonObject["TelemetryDataSinkSettings"].asObject();
		if(tdss.containsKey("Incoming"))
		{
			JsonArray& incArray = tdss["Incoming"].asArray();

			for(JsonArray::iterator element = incArray.begin(); element!= incArray.end(); ++element)
			{
				TelemetryDataSinkParameters telemetryDataSinkParameters;
				telemetryDataSinkParameters.SinkName = (*element)["SinkName"].asString();
				JsonObject& paramMap = (*element)["Parameters"].asObject();

				for(JsonObject::iterator param = paramMap.begin(); param!= paramMap.end(); ++param)
				{
					telemetryDataSinkParameters.Parameters[string(param->key)] = string(param->value.asString());
				}
				telemetryDataSinkSettings.Incoming.push_back(telemetryDataSinkParameters);
			} 	
		} 
	}
}

string BuildTelmetryDataSinkParametersListJson(vector<TelemetryDataSinkParameters> telemetryDataSinkParameters)
{
	DynamicJsonBuffer jsonBuffer;

	JsonArray& array = jsonBuffer.createArray();

	for(vector<TelemetryDataSinkParameters>::iterator it = telemetryDataSinkParameters.begin(); it!= telemetryDataSinkParameters.end(); ++it)
	{
		JsonObject& item = array.createNestedObject();
		item["SinkName"] = it->SinkName.c_str();
		JsonObject& params = item.createNestedObject("Parameters");

		for(map<string, string>::iterator pit = it->Parameters.begin(); pit!= it->Parameters.end(); ++pit)
		{
			params[pit->first.c_str()] = pit->second.c_str();
		}
	}

	char buffer[1024];
	array.printTo(buffer, sizeof(buffer));

	return string(buffer);
}
}
}

