using System;
using System.Collections.Generic;

namespace IoT.Client.DotNet.Reporting
{
    /// <summary>
    /// Device-specific reporting operations
    /// </summary>
    public class DeviceClient
    {
        private readonly string _baseUrl;
        private readonly IRestConnection _restConnection;

        internal DeviceClient(string baseUrl, IRestConnection restConnection)
        {
            _baseUrl = baseUrl;
            _restConnection = restConnection;
        }

        /// <summary>
        /// Logs in with the specified device. Note that you can be logged in with only one device or one network for any <see cref="ReportingClient"/> instance.
        /// </summary>
        /// <param name="deviceId">Unique device identifier</param>
        /// <param name="deviceKey">Api key of the device, or any of the encosing networks, or the enclosing service</param>
        public void SetDevice(string deviceId, string deviceKey)
        {
            _restConnection.Setup(_baseUrl,
                new Dictionary<string, string>
                {
                    {"X-DeviceId", deviceId},
                    {"X-DeviceKey", deviceKey}
                });
        }

        /// <summary>
        /// Return the telemetry data sinks that can be used to query reporting (and of course could be used to record telemetry data)
        /// 
        /// Send GET request to the APIROOT/devices/sinks Url
        /// </summary>
        /// <returns>List of SinkInfos</returns>
        public IEnumerable<SinkInfoDto> GetSinks()
        {
            var response = _restConnection.Get("devices/sinks");
            return JsonSerializer.Deserialize<IEnumerable<SinkInfoDto>>(response);
        }

        /// <summary>
        /// Query the latest recorded data using the given sink in JSON format
        /// 
        /// Send GET request to the APIROOT/devices/json/sinkName url
        /// </summary>
        /// <param name="sinkName">Current data sink to query</param>
        /// <returns>Reporting entity about the current data for the device</returns>
        public CurrentDataReportDto GetCurrentData(string sinkName)
        {
            var response = _restConnection.Get("devices/json/" + sinkName);
            return JsonSerializer.Deserialize<CurrentDataReportDto>(response);
        }

        /// <summary>
        /// Query the time series data using the given sink in JSON format for a given day
        /// The date must be in UTC and must be somewhere in the day what we are interested in
        /// 
        /// Send GET request to the APIROOT/devices/json/sinkName/timestamp
        /// </summary>
        /// <param name="sinkName">Time series sink</param>
        /// <param name="date"></param>
        /// <returns>Time series reporting entity for the device</returns>
        public TimeSeriesReportDto GetTimeSeriesReport(string sinkName, DateTime date)
        {
            var response = _restConnection.Get("devices/json/" + sinkName + "/" + (long)(date - new DateTime(1970, 1, 1)).TotalSeconds);
            return JsonSerializer.Deserialize<TimeSeriesReportDto>(response);
        }

        /// <summary>
        /// Query the latest recorded data using the given sink in CSV format
        /// 
        /// Send GET request to the APIROOT/devices/csv/sinkName url
        /// </summary>
        /// <param name="sinkName">Current data sink to query</param>
        /// <returns>CSV payload about the current data for the device</returns>
        public string GetCurrentDataCsv(string sinkName)
        {
            return _restConnection.Get("devices/csv/" + sinkName);
        }

        /// <summary>
        /// Query the time series data using the given sink in CSV format for a given day
        /// The date must be in UTC and must be somewhere in the day what we are interested in
        /// 
        /// Send GET request to the APIROOT/devices/csv/sinkName/timestamp
        /// </summary>
        /// <param name="sinkName">Time series sink</param>
        /// <param name="date"></param>
        /// <returns>CSV payload of time series data for the device</returns>
        public string GetTimeSeriesReportCsv(string sinkName, DateTime date)
        {
            return _restConnection.Get("devices/csv/" + sinkName + "/" + (long)(date - new DateTime(1970, 1, 1)).TotalSeconds);
        }
    }
}
