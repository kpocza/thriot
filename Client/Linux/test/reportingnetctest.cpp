#include "gtest/gtest.h"
#include "ManagementClient.h"
#include "PlatformClient.h"
#include "ReportingClient.h"
#include "common.h"

using namespace Thriot::Platform;
using namespace Thriot::Reporting;

TEST(ReportingNetworkClientTest, singleNetworkGetSinks)
{
	ReportingTestInput reportingTestInput = CreateReportingTestInput();

	ReportingClient *reportingClient = new ReportingClient(RAPIURL);

	NetworkClient* networkClient = reportingClient->Network();
	networkClient->SetNetwork(reportingTestInput.Net.Id, reportingTestInput.Net.NetworkKey);
	vector<SinkInfo> sinks = networkClient->GetSinks();
	ASSERT_EQ(2, sinks.size());
	ASSERT_EQ(CurrentData, sinks[0].Type);
	ASSERT_EQ(SINKDATA, sinks[0].SinkName);
	ASSERT_EQ(TimeSeries, sinks[1].Type);
	ASSERT_EQ(SINKTIMESERIES, sinks[1].SinkName);
}

TEST(ReportingNetworkClientTest, singleDeviceEmpty)
{
	ReportingTestInput reportingTestInput = CreateReportingTestInput();

	ReportingClient *reportingClient = new ReportingClient(RAPIURL);

	NetworkClient* networkClient = reportingClient->Network();
	networkClient->SetNetwork(reportingTestInput.Net.Id, reportingTestInput.Net.NetworkKey);
	string currentDataJson = networkClient->GetCurrentDataJson(SINKDATA);
	string timeSeriesDataJson = networkClient->GetTimeSeriesJson(SINKTIMESERIES, time(NULL));
	string currentDataCsv = networkClient->GetCurrentDataCsv(SINKDATA);
	string timeSeriesDataCsv = networkClient->GetTimeSeriesCsv(SINKTIMESERIES, time(NULL));

	ASSERT_EQ("{\"Devices\":[]}", currentDataJson);
	ASSERT_EQ("DeviceId,Name,Time\r\n", currentDataCsv);

	ASSERT_TRUE(timeSeriesDataJson.size() < 200);
	ASSERT_NE(string::npos, timeSeriesDataJson.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, timeSeriesDataJson.find(reportingTestInput.Dev2.Id));
	ASSERT_NE(string::npos, timeSeriesDataJson.find("test device"));
	ASSERT_EQ(string::npos, timeSeriesDataJson.find("{\"Data\":[]}"));
	ASSERT_EQ("DeviceId,Name,Time\r\n", timeSeriesDataCsv);
}

TEST(ReportingNetworkClientTest, singleDeviceSingleEntry)
{
	ReportingTestInput reportingTestInput = CreateReportingTestInput();

	OcassionalConnectionClient *ocassionalConnectionClient = new OcassionalConnectionClient(PAPIURL, 
		reportingTestInput.Dev1.Id, reportingTestInput.Dev1.DeviceKey);

	ocassionalConnectionClient->RecordTelemetryData("{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux\"}");

	ReportingClient *reportingClient = new ReportingClient(RAPIURL);

	NetworkClient* networkClient = reportingClient->Network();
	networkClient->SetNetwork(reportingTestInput.Net.Id, reportingTestInput.Net.NetworkKey);
	string currentDataJson = networkClient->GetCurrentDataJson(SINKDATA);
	string timeSeriesDataJson = networkClient->GetTimeSeriesJson(SINKTIMESERIES, time(NULL)-3600);
	string currentDataCsv = networkClient->GetCurrentDataCsv(SINKDATA);
	string timeSeriesDataCsv = networkClient->GetTimeSeriesCsv(SINKTIMESERIES, time(NULL)-3600);

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

TEST(ReportingNetworkClientTest, singleDeviceMultiEntry)
{
	ReportingTestInput reportingTestInput = CreateReportingTestInput();

	OcassionalConnectionClient *ocassionalConnectionClient = new OcassionalConnectionClient(PAPIURL, 
		reportingTestInput.Dev1.Id, reportingTestInput.Dev1.DeviceKey);

	ocassionalConnectionClient->RecordTelemetryData("{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux\"}");
	ocassionalConnectionClient->RecordTelemetryData("{\"Temperature\": 25, \"Humidity\": 51, \"Source\": \"Linux\"}");

	ReportingClient *reportingClient = new ReportingClient(RAPIURL);

	NetworkClient* networkClient = reportingClient->Network();
	networkClient->SetNetwork(reportingTestInput.Net.Id, reportingTestInput.Net.NetworkKey);
	string currentDataJson = networkClient->GetCurrentDataJson(SINKDATA);
	string timeSeriesDataJson = networkClient->GetTimeSeriesJson(SINKTIMESERIES, time(NULL));
	string currentDataCsv = networkClient->GetCurrentDataCsv(SINKDATA);
	string timeSeriesDataCsv = networkClient->GetTimeSeriesCsv(SINKTIMESERIES, time(NULL));

	ASSERT_TRUE(currentDataJson.size() > 80);
	ASSERT_NE(string::npos, currentDataJson.find("test device"));
	ASSERT_NE(string::npos, currentDataJson.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, currentDataJson.find("Temperature"));
	ASSERT_NE(string::npos, currentDataJson.find("Linux"));
	ASSERT_EQ(string::npos, currentDataJson.find(":24"));
	ASSERT_NE(string::npos, currentDataJson.find(":25"));

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
	ASSERT_NE(string::npos, timeSeriesDataJson.find(":24"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find(":25"));

	ASSERT_TRUE(timeSeriesDataCsv.size() > 100);
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("DeviceId,Name,Time,Temperature,Humidity,Source"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("test device"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("Temperature"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("Linux"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find(",24,"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find(",25,"));
}

TEST(ReportingNetworkClientTest, multiDeviceMultiEntry)
{
	ReportingTestInput reportingTestInput = CreateReportingTestInput();

	OcassionalConnectionClient *ocassionalConnectionClient = new OcassionalConnectionClient(PAPIURL, 
		reportingTestInput.Dev1.Id, reportingTestInput.Dev1.DeviceKey);

	ocassionalConnectionClient->RecordTelemetryData("{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux\"}");
	ocassionalConnectionClient->RecordTelemetryData("{\"Temperature\": 25, \"Humidity\": 51, \"Source\": \"Linux\"}");

	ocassionalConnectionClient = new OcassionalConnectionClient(PAPIURL, 
		reportingTestInput.Dev2.Id, reportingTestInput.Dev2.DeviceKey);

	ocassionalConnectionClient->RecordTelemetryData("{\"Temperature\": 26, \"Humidity\": 50, \"Source\": \"Linux\"}");
	ocassionalConnectionClient->RecordTelemetryData("{\"Temperature\": 27, \"Humidity\": 51, \"Source\": \"Linux\"}");

	ReportingClient *reportingClient = new ReportingClient(RAPIURL);

	NetworkClient* networkClient = reportingClient->Network();
	networkClient->SetNetwork(reportingTestInput.Net.Id, reportingTestInput.Net.NetworkKey);
	string currentDataJson = networkClient->GetCurrentDataJson(SINKDATA);
	string timeSeriesDataJson = networkClient->GetTimeSeriesJson(SINKTIMESERIES, time(NULL));
	string currentDataCsv = networkClient->GetCurrentDataCsv(SINKDATA);
	string timeSeriesDataCsv = networkClient->GetTimeSeriesCsv(SINKTIMESERIES, time(NULL));

	ASSERT_TRUE(currentDataJson.size() > 140);
	ASSERT_NE(string::npos, currentDataJson.find("test device"));
	ASSERT_NE(string::npos, currentDataJson.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, currentDataJson.find("Temperature"));
	ASSERT_NE(string::npos, currentDataJson.find("Linux"));
	ASSERT_EQ(string::npos, currentDataJson.find(":24"));
	ASSERT_NE(string::npos, currentDataJson.find(":25"));
	ASSERT_EQ(string::npos, currentDataJson.find(":26"));
	ASSERT_NE(string::npos, currentDataJson.find(":27"));

	ASSERT_TRUE(currentDataCsv.size() > 100);
	ASSERT_NE(string::npos, currentDataCsv.find("DeviceId,Name,Time,Temperature,Humidity,Source"));
	ASSERT_NE(string::npos, currentDataCsv.find("test device"));
	ASSERT_NE(string::npos, currentDataCsv.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, currentDataCsv.find("Temperature"));
	ASSERT_NE(string::npos, currentDataCsv.find("Linux"));
	ASSERT_EQ(string::npos, currentDataCsv.find(",24,"));
	ASSERT_NE(string::npos, currentDataCsv.find(",25,"));
	ASSERT_EQ(string::npos, currentDataCsv.find(",26,"));
	ASSERT_NE(string::npos, currentDataCsv.find(",27,"));

	ASSERT_TRUE(timeSeriesDataJson.size() > 200);
	ASSERT_NE(string::npos, timeSeriesDataJson.find("test device"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, timeSeriesDataJson.find("Temperature"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find("Linux"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find(":24"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find(":25"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find(":26"));
	ASSERT_NE(string::npos, timeSeriesDataJson.find(":27"));

	ASSERT_TRUE(timeSeriesDataCsv.size() > 200);
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("DeviceId,Name,Time,Temperature,Humidity,Source"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("test device"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find(reportingTestInput.Dev1.Id));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("Temperature"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find("Linux"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find(",24,"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find(",25,"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find(",26,"));
	ASSERT_NE(string::npos, timeSeriesDataCsv.find(",27,"));
}
