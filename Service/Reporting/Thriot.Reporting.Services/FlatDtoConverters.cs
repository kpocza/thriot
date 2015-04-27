using System;
using System.Collections.Generic;
using System.Linq;
using Thriot.Framework;
using Newtonsoft.Json.Linq;
using Thriot.Objects.Model;
using Thriot.Plugins.Core;
using Thriot.Reporting.Dto;

namespace Thriot.Reporting.Services
{
    internal static class FlatDtoConverters
    {
        internal static FlatReportDto CurrentDataReport(IEnumerable<Small> devices, IEnumerable<TelemetryData> telemetryDataList)
        {
            var flatReportDto = new FlatReportDto
            {
                Properties = new List<string>(), 
                Rows = new List<FlatRowDto>()
            };

            foreach (var telemetryData in telemetryDataList)
            {
                var fields = ExtractFields(telemetryData.Payload);
                var flatRowDto = new FlatRowDto
                {
                    DeviceId = telemetryData.DeviceId,
                    Name = devices.Single(d => d.Id == telemetryData.DeviceId).Name,
                    Timestamp = telemetryData.Time.ToUnixTime(),
                    Fields = fields
                };
                flatReportDto.Rows.Add(flatRowDto);

                FillProperties(fields, flatReportDto.Properties);
            }

            return flatReportDto;
        }

        internal static FlatReportDto TimeSeriesReport(IEnumerable<Small> devices, IEnumerable<TelemetryData> telemetryDataList)
        {
            var flatReportDto = new FlatReportDto
            {
                Properties = new List<string>(),
                Rows = new List<FlatRowDto>()
            };

            Small currentDevice = null;

            foreach (var telemetryData in telemetryDataList)
            {
                if (currentDevice == null || currentDevice.Id != telemetryData.DeviceId)
                {
                    currentDevice = new Small
                    {
                        Id = telemetryData.DeviceId,
                        Name = devices.Single(d => d.Id == telemetryData.DeviceId).Name
                    };
                }
                var fields = ExtractFields(telemetryData.Payload);
                var flatRowDto = new FlatRowDto
                {
                    DeviceId = currentDevice.Id,
                    Name = currentDevice.Name,
                    Timestamp = telemetryData.Time.ToUnixTime(),
                    Fields = fields
                };
                flatReportDto.Rows.Add(flatRowDto);

                FillProperties(fields, flatReportDto.Properties);
            }

            return flatReportDto;
        }

        private static List<FlatPair> ExtractFields(string payload)
        {
            var dictionary = new List<FlatPair>();
            var jToken = JObject.Parse(payload);

            foreach (var child in jToken.Children())
            {
                var prop = (JProperty)child;
                dictionary.Add(new FlatPair(prop.Name, prop.Value.ToString()));
            }

            return dictionary;
        }

        private static void FillProperties(IEnumerable<FlatPair> fields, ICollection<string> props)
        {
            foreach (var fld in fields)
            {
                if (!props.Contains(fld.Key))
                {
                    props.Add(fld.Key);
                }
            }
        }
    }
}
