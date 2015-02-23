using System;
using System.Collections.Generic;

namespace IoT.Client.DotNet.Reporting
{
    public class DeviceClient
    {
        private readonly string _baseUrl;
        private readonly IRestConnection _restConnection;

        internal DeviceClient(string baseUrl, IRestConnection restConnection)
        {
            _baseUrl = baseUrl;
            _restConnection = restConnection;
        }

        public void SetDevice(string deviceId, string deviceKey)
        {
            _restConnection.Setup(_baseUrl,
                new Dictionary<string, string>
                {
                    {"X-DeviceId", deviceId},
                    {"X-DeviceKey", deviceKey}
                });
        }

        public IEnumerable<SinkInfoDto> GetSinks()
        {
            var response = _restConnection.Get("devices/sinks");
            return JsonSerializer.Deserialize<IEnumerable<SinkInfoDto>>(response);
        }

        public CurrentDataReportDto GetCurrentData(string sinkName)
        {
            var response = _restConnection.Get("devices/json/" + sinkName);
            return JsonSerializer.Deserialize<CurrentDataReportDto>(response);
        }

        public TimeSeriesReportDto GetTimeSeriesReport(string sinkName, DateTime date)
        {
            var response = _restConnection.Get("devices/json/" + sinkName + "/" + (long)(date - new DateTime(1970, 1, 1)).TotalSeconds);
            return JsonSerializer.Deserialize<TimeSeriesReportDto>(response);
        }

        public string GetCurrentDataCsv(string sinkName)
        {
            return _restConnection.Get("devices/csv/" + sinkName);
        }

        public string GetTimeSeriesReportCsv(string sinkName, DateTime date)
        {
            return _restConnection.Get("devices/csv/" + sinkName + "/" + (long)(date - new DateTime(1970, 1, 1)).TotalSeconds);
        }
    }
}
