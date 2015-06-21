#include "gtest/gtest.h"
#include "ManagementClient.h"
#include "PlatformClient.h"
#include "ReportingClient.h"
#include "common.h"

using namespace Thriot::Platform;
using namespace Thriot::Reporting;

TEST(ReportingDeviceClientTest, singleDeviceGetSinks)
{
	ReportingTestInput reportingTestInput = CreateReportingTestInput();

	ReportingClient *reportingClient = new ReportingClient(RAPIURL);

	DeviceClient* deviceClient = reportingClient->Device();
	deviceClient->SetDevice(reportingTestInput.Dev1.Id, reportingTestInput.Dev1.DeviceKey);
	vector<SinkInfo> sinks = deviceClient->GetSinks();
	ASSERT_EQ(2, sinks.size());
	ASSERT_EQ(CurrentData, sinks[0].Type);
	ASSERT_EQ(SINKDATA, sinks[0].SinkName);
	ASSERT_EQ(TimeSeries, sinks[1].Type);
	ASSERT_EQ(SINKTIMESERIES, sinks[1].SinkName);
}

TEST(ReportingDeviceClientTest, singleDeviceEmpty)
{
	ReportingTestInput reportingTestInput = CreateReportingTestInput();

	ReportingClient *reportingClient = new ReportingClient(RAPIURL);

	DeviceClient* deviceClient = reportingClient->Device();
	deviceClient->SetDevice(reportingTestInput.Dev1.Id, reportingTestInput.Dev1.DeviceKey);
	string currentDataJson = deviceClient->GetCurrentDataJson(SINKDATA);
	string timeSeriesDataJson = deviceClient->GetTimeSeriesJson(SINKTIMESERIES, time(NULL));
	string currentDataCsv = deviceClient->GetCurrentDataCsv(SINKDATA);
	string timeSeriesDataCsv = deviceClient->GetTimeSeriesCsv(SINKTIMESERIES, time(NULL));

	ASSERT_EQ("{\"Devices\":[]}", currentDataJson);
	ASSERT_EQ("DeviceId,Name,Time\r\n", currentDataCsv);

	ASSERT_TRUE(timeSeriesDataJson.size() < 100);
	ASSERT_NE(string::npos, timeSeriesDataJson.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, timeSeriesDataJson.find("test device"));
	ASSERT_EQ(string::npos, timeSeriesDataJson.find("{\"Data\":[]}"));
	ASSERT_EQ("DeviceId,Name,Time\r\n", timeSeriesDataCsv);
}

TEST(ReportingDeviceClientTest, singleDeviceSingleEntry)
{
	ReportingTestInput reportingTestInput = CreateReportingTestInput();

	OccasionallyConnectionClient *occasionallyConnectionClient = new OccasionallyConnectionClient(PAPIURL, 
		reportingTestInput.Dev1.Id, reportingTestInput.Dev1.DeviceKey);

	occasionallyConnectionClient->RecordTelemetryData("{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux\"}");

	ReportingClient *reportingClient = new ReportingClient(RAPIURL);

	DeviceClient* deviceClient = reportingClient->Device();
	deviceClient->SetDevice(reportingTestInput.Dev1.Id, reportingTestInput.Dev1.DeviceKey);
	string currentDataJson = deviceClient->GetCurrentDataJson(SINKDATA);
	string timeSeriesDataJson = deviceClient->GetTimeSeriesJson(SINKTIMESERIES, time(NULL));
	string currentDataCsv = deviceClient->GetCurrentDataCsv(SINKDATA);
	string timeSeriesDataCsv = deviceClient->GetTimeSeriesCsv(SINKTIMESERIES, time(NULL));

	ASSERT_TRUE(currentDataJson.size() > 80);
	ASSERT_NE(string::npos, currentDataJson.find("test device"));
	ASSERT_NE(string::npos, currentDataJson.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, currentDataJson.find("Temperature"));
	ASSERT_NE(string::npos, currentDataJson.find("Linux"));

	ASSERT_TRUE(currentDataCsv.size() > 50);
	ASSERT_NE(string::npos, currentDataCsv.find("DeviceId,Name,Time,Temperature,Humidity,Source"));
	ASSERT_NE(string::npos, currentDataCsv.find("test device"));
	ASSERT_NE(string::npos, currentDataCsv.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, currentDataCsv.find("Temperature"));
	ASSERT_NE(string::npos, currentDataCsv.find("Linux"));

	ASSERT_TRUE(timeSeriesDataJson.size() > 100);
	ASSERT_NE(string::npos, timeSeriesDataJson.find("test device"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, timeSeriesDataJson.find("Temperature"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find("Linux"));

	ASSERT_TRUE(timeSeriesDataCsv.size() > 100);
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("DeviceId,Name,Time,Temperature,Humidity,Source"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("test device"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("Temperature"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("Linux"));
}

TEST(ReportingDeviceClientTest, singleDeviceMultiEntry)
{
	ReportingTestInput reportingTestInput = CreateReportingTestInput();

	OccasionallyConnectionClient *occasionallyConnectionClient = new OccasionallyConnectionClient(PAPIURL, 
		reportingTestInput.Dev1.Id, reportingTestInput.Dev1.DeviceKey);

	occasionallyConnectionClient->RecordTelemetryData("{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux\"}");
	usleep(1000);
	occasionallyConnectionClient->RecordTelemetryData("{\"Temperature\": 25, \"Humidity\": 51, \"Source\": \"Linux\"}");

	ReportingClient *reportingClient = new ReportingClient(RAPIURL);

	DeviceClient* deviceClient = reportingClient->Device();
	deviceClient->SetDevice(reportingTestInput.Dev1.Id, reportingTestInput.Dev1.DeviceKey);
	string currentDataJson = deviceClient->GetCurrentDataJson(SINKDATA);
	string timeSeriesDataJson = deviceClient->GetTimeSeriesJson(SINKTIMESERIES, time(NULL));
	string currentDataCsv = deviceClient->GetCurrentDataCsv(SINKDATA);
	string timeSeriesDataCsv = deviceClient->GetTimeSeriesCsv(SINKTIMESERIES, time(NULL));

	ASSERT_TRUE(currentDataJson.size() > 80);
	ASSERT_NE(string::npos, currentDataJson.find("test device"));
	ASSERT_NE(string::npos, currentDataJson.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, currentDataJson.find("Temperature"));
	ASSERT_NE(string::npos, currentDataJson.find("Linux"));
	ASSERT_EQ(string::npos, currentDataJson.find(":24,"));
	ASSERT_NE(string::npos, currentDataJson.find(":25,"));

	ASSERT_TRUE(currentDataCsv.size() > 50);
	ASSERT_NE(string::npos, currentDataCsv.find("DeviceId,Name,Time,Temperature,Humidity,Source"));
	ASSERT_NE(string::npos, currentDataCsv.find("test device"));
	ASSERT_NE(string::npos, currentDataCsv.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, currentDataCsv.find("Temperature"));
	ASSERT_NE(string::npos, currentDataCsv.find("Linux"));
	ASSERT_EQ(string::npos, currentDataCsv.find(",24,"));
	ASSERT_NE(string::npos, currentDataCsv.find(",25,"));

	ASSERT_TRUE(timeSeriesDataJson.size() > 100);
	ASSERT_NE(string::npos, timeSeriesDataJson.find("test device"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, timeSeriesDataJson.find("Temperature"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find("Linux"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find(":24,"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find(":25,"));

	ASSERT_TRUE(timeSeriesDataCsv.size() > 100);
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("DeviceId,Name,Time,Temperature,Humidity,Source"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("test device"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("Temperature"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("Linux"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find(",24,"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find(",25,"));
}
