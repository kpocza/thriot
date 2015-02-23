#pragma once

#include "ManagementClient.h"
#include "ArduinoJson.h"

void StripQuotes(std::string &str);
string UrlEncode(const std::string &str);
void ParseTelemetryDataSinkSettings(JsonObject& jsonObject, TelemetryDataSinkSettingsType& telemetryDataSinkSettings);
string BuildTelmetryDataSinkParametersListJson(vector<TelemetryDataSinkParameters> telemetryDataSinkParameters);


