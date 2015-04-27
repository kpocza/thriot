using System;
using System.Collections.Generic;

namespace Thriot.Client.DotNet.Reporting
{
    /// <summary>
    /// Network-specific reporting operations
    /// </summary>
    public class NetworkClient
    {
        private readonly string _baseUrl;
        private readonly IRestConnection _restConnection;

        internal NetworkClient(string baseUrl, IRestConnection restConnection)
        {
            _baseUrl = baseUrl;
            _restConnection = restConnection;
        }

        /// <summary>
        /// Logs in with the specified network. Note that you can be logged in with only one device or one network for any <see cref="ReportingClient"/> instance.
        /// </summary>
        /// <param name="networkId">Unique network identifier</param>
        /// <param name="networkKey">Api key of the network, or any of the encosing networks, or the enclosing service</param>
        public void SetNetwork(string networkId, string networkKey)
        {
            _restConnection.Setup(_baseUrl,
                new Dictionary<string, string>
                {
                    {"X-NetworkId", networkId},
                    {"X-NetworkKey", networkKey}
                });
        }

        /// <summary>
        /// Return the telemetry data sinks that can be used to query reporting (and of course could be used to record telemetry data)
        /// 
        /// Send GET request to the APIROOT/networks/sinks Url
        /// </summary>
        /// <returns>List of SinkInfos</returns>
        public IEnumerable<SinkInfoDto> GetSinks()
        {
            var response = _restConnection.Get("networks/sinks");
            return JsonSerializer.Deserialize<IEnumerable<SinkInfoDto>>(response);
        }

        /// <summary>
        /// Query the latest recorded data for all devices in the network using the given sink in JSON format
        /// 
        /// Send GET request to the APIROOT/networks/json/sinkName url
        /// </summary>
        /// <param name="sinkName">Current data sink to query</param>
        /// <returns>Reporting entity about the current data for the devices in the network</returns>
        public CurrentDataReportDto GetCurrentData(string sinkName)
        {
            var response = _restConnection.Get("networks/json/" + sinkName);
            return JsonSerializer.Deserialize<CurrentDataReportDto>(response);
        }

        /// <summary>
        /// Query the time series data for all devices in the network using the given sink in JSON format for a given day
        /// The date must be in UTC and must be somewhere in the day what we are interested in
        /// 
        /// Send GET request to the APIROOT/networks/json/sinkName/timestamp
        /// </summary>
        /// <param name="sinkName">Time series sink</param>
        /// <param name="date"></param>
        /// <returns>Time series reporting entity for the all the devices in the network</returns>
        public TimeSeriesReportDto GetTimeSeriesReport(string sinkName, DateTime date)
        {
            var response = _restConnection.Get("networks/json/" + sinkName + "/" + (long)(date - new DateTime(1970, 1, 1)).TotalSeconds);
            return JsonSerializer.Deserialize<TimeSeriesReportDto>(response);
        }

        /// <summary>
        /// Query the latest recorded data for all devices in the network using the given sink in CSV format
        /// 
        /// Send GET request to the APIROOT/networks/csv/sinkName url
        /// </summary>
        /// <param name="sinkName">Current data sink to query</param>
        /// <returns>CSV payload about the current data for the devices in the network</returns>
        public string GetCurrentDataCsv(string sinkName)
        {
            return _restConnection.Get("networks/csv/" + sinkName);
        }

        /// <summary>
        /// Query the time series data for all devices in the network using the given sink in CSV format for a given day
        /// The date must be in UTC and must be somewhere in the day what we are interested in
        /// 
        /// Send GET request to the APIROOT/networks/csv/sinkName/timestamp
        /// </summary>
        /// <param name="sinkName">Time series sink</param>
        /// <param name="date"></param>
        /// <returns>CSV payload of time series data for the devices in the network</returns>
        public string GetTimeSeriesReportCsv(string sinkName, DateTime date)
        {
            return _restConnection.Get("networks/csv/" + sinkName + "/" + (long)(date - new DateTime(1970, 1, 1)).TotalSeconds);
        }
    }
}
