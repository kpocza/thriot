#pragma once

#include "ManagementClient.h"
#include "ArduinoJson.h"

namespace Thriot
{
namespace Management
{
void StripQuotes(std::string &str);
string UrlEncode(const std::string &str);
void ParseTelemetryDataSinkSettings(JsonObject& jsonObject, TelemetryDataSinkSettingsType& telemetryDataSinkSettings);
string BuildTelmetryDataSinkParametersListJson(vector<TelemetryDataSinkParameters> telemetryDataSinkParameters);
}
}
