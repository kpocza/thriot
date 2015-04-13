#pragma once

#include <string>
#include <vector>

using namespace std;

namespace Thriot { 
class RestConnection;

namespace Reporting {

enum SinkType
{
	CurrentData = 1,
	TimeSeries = 2
};

struct SinkInfo
{
	string SinkName;
	SinkType Type;
};

class DeviceClient
{
	private:
		RestConnection* _restConnection;

	public:
		DeviceClient(RestConnection* restConnection);

		void SetDevice(const string& deviceId, const string& deviceKey);
		vector<SinkInfo> GetSinks();

		string GetCurrentDataJson(const string& sinkName);
		string GetTimeSeriesJson(const string& sinkName, unsigned long time);

		string GetCurrentDataCsv(const string& sinkName);
		string GetTimeSeriesCsv(const string& sinkName, unsigned long time);
};

class NetworkClient
{
	private:
		RestConnection* _restConnection;

	public:
		NetworkClient(RestConnection* restConnection);

		void SetNetwork(const string& networkId, const string& networkKey);
		vector<SinkInfo> GetSinks();

		string GetCurrentDataJson(const string& sinkName);
		string GetTimeSeriesJson(const string& sinkName, unsigned long time);

		string GetCurrentDataCsv(const string& sinkName);
		string GetTimeSeriesCsv(const string& sinkName, unsigned long time);
};

class ReportingClient
{
	private:
		RestConnection* _restConnection;
		DeviceClient* _deviceClient;
		NetworkClient* _networkClient;

	public:
		ReportingClient(const string& baseUrl);
		~ReportingClient();

		DeviceClient* Device();
		NetworkClient* Network();
};
}}

