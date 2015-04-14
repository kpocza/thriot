#pragma once

#include <string>
#include <vector>

using namespace std;

namespace Thriot { 
class RestConnection;

namespace Reporting {

/** Telemetry data sink type*/
enum SinkType
{
	/** Sink handles current data */
	CurrentData = 1,
	/** Time series sink */
	TimeSeries = 2
};

/** Sink information entity to be used for reporting */
struct SinkInfo
{
	/** Name of the sink */
	string SinkName;
	/** Type of the sink */
	SinkType Type;
};

/** Client class used for device-level reporting */
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

/** Client class used for network-level reporting. In the reports created by this class you will find all devices under the network. */
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

/** Client class encapsulating the device-level and network-level reporting functionality */
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

