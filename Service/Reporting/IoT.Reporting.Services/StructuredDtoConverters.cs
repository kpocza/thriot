using System.Collections.Generic;
using System.Linq;
using IoT.Framework;
using IoT.Objects.Model;
using IoT.Plugins.Core;
using IoT.Reporting.Dto;

namespace IoT.Reporting.Services
{
    internal static class StructuredDtoConverters
    {
        internal static CurrentDataReportDto CurrentDataReport(IEnumerable<Small> devices, IEnumerable<TelemetryData> telemetryDataList)
        {
            var currentDataReportDto = new CurrentDataReportDto
            {
                Devices = telemetryDataList.Select(
                    tdl =>
                        new CurrentDataDeviceDto
                        {
                            DeviceId = tdl.DeviceId,
                            Name = devices.Single(d => d.Id == tdl.DeviceId).Name,
                            Payload = tdl.Payload,
                            Timestamp = tdl.Time.ToUnixTime()
                        }).ToList()
            };

            return currentDataReportDto;
        }

        internal static TimeSeriesReportDto TimeSeriesReport(IEnumerable<Small> devices, IEnumerable<TelemetryData> telemetryDataList)
        {
            var timeSeriesReport = new TimeSeriesReportDto
            {
                Devices = devices.Select(
                    d => new TimeSeriesDeviceDto {DeviceId = d.Id, Name = d.Name, Data = new List<TimeSeriesRowDto>()})
                    .ToList()
            };

            TimeSeriesDeviceDto timeSeriesDevice = null;
            foreach (var telemetryData in telemetryDataList)
            {
                if (timeSeriesDevice == null || timeSeriesDevice.DeviceId != telemetryData.DeviceId)
                {
                    timeSeriesDevice = timeSeriesReport.Devices.Single(d => d.DeviceId == telemetryData.DeviceId);
                }

                timeSeriesDevice.Data.Add(new TimeSeriesRowDto
                {
                    Timestamp = telemetryData.Time.ToUnixTime(),
                    Payload = telemetryData.Payload
                });
            }

            return timeSeriesReport;
        }
    }
}
