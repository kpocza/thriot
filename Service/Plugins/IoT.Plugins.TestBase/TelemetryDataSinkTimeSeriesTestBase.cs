using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using IoT.Framework.DataAccess;
using IoT.Management.Dto;
using IoT.Plugins.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Plugins.TestBase
{
    public abstract class TelemetryDataSinkTimeSeriesTestBase : TestBase
    {
        protected abstract ITelemetryDataSinkTimeSeries GetTelemetryDataSinkTimeSeries();
        protected abstract string GetConnectionString();

        public virtual void RecordTest()
        {
            if (!IsIntegrationTest())
                return;
            
            var telemetryDataSinkTimeSeries = GetTelemetryDataSinkTimeSeries();
            telemetryDataSinkTimeSeries.Setup(new Dictionary<string, string>
            {
                {"ConnectionString", GetConnectionString()},
                {"Table", "TimeSeries"}
            });

            telemetryDataSinkTimeSeries.Initialize();

            var message = new TelemetryData(_deviceId, "{ \"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}", DateTime.UtcNow);

            telemetryDataSinkTimeSeries.Record(message);
        }

        public virtual void RecordTwoTest()
        {
            if (!IsIntegrationTest())
                return;

            var telemetryDataSinkTimeSeries = GetTelemetryDataSinkTimeSeries();
            telemetryDataSinkTimeSeries.Setup(new Dictionary<string, string>
            {
                {"ConnectionString", GetConnectionString()},
                {"Table", "TimeSeries"}
            });

            telemetryDataSinkTimeSeries.Initialize();

            var message1 = new TelemetryData(_deviceId, "{ \"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}", DateTime.UtcNow);
            telemetryDataSinkTimeSeries.Record(message1);

            var message2 = new TelemetryData(_deviceId, "{ \"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}", DateTime.UtcNow.AddSeconds(1));
            telemetryDataSinkTimeSeries.Record(message2);
        }

        public virtual void TryRecordTwiceTest()
        {
            if (!IsIntegrationTest())
                throw new StorageAccessException(HttpStatusCode.Conflict);

            var telemetryDataSinkTimeSeries = GetTelemetryDataSinkTimeSeries();
            telemetryDataSinkTimeSeries.Setup(new Dictionary<string, string>
            {
                {"ConnectionString", GetConnectionString()},
                {"Table", "TimeSeries"}
            });

            telemetryDataSinkTimeSeries.Initialize();

            var message = new TelemetryData(_deviceId, "{ \"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}", DateTime.UtcNow);

            telemetryDataSinkTimeSeries.Record(message);
            telemetryDataSinkTimeSeries.Record(message);
        }

        public virtual void QueryDeviceTimeSeries()
        {
            if (!IsIntegrationTest())
                return;

            var telemetryDataSinkTimeSeries = GetTelemetryDataSinkTimeSeries();
            telemetryDataSinkTimeSeries.Setup(new Dictionary<string, string>
            {
                {"ConnectionString", GetConnectionString()},
                {"Table", "TimeSeries"}
            });

            telemetryDataSinkTimeSeries.Initialize();

            var devices = new Dictionary<string, List<TelemetryData>>();

            for (int i = 0; i < 2; i++)
            {
                var device = new DeviceDto()
                {
                    NetworkId = _networkId,
                    CompanyId = _companyId,
                    ServiceId = _serviceId,
                    Name = "new device" + i
                };

                var deviceId = _deviceService.Create(device);
                devices.Add(deviceId, new List<TelemetryData>());
            }

            var flatKeys = devices.Keys.ToList();
            for (int i = 0; i < 100; i++)
            {
                var deviceId = flatKeys[i%2];
                var message = new TelemetryData(deviceId, "{ \"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Date.AddMinutes(i) + "}", DateTime.UtcNow.Date.AddMinutes(i));

                telemetryDataSinkTimeSeries.Record(message);
                devices[deviceId].Add(message);

                // records for other day that shouldn't be returned by the query
                if (i%11 == 0)
                {
                    message = new TelemetryData(deviceId,
                        "{ \"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Date.AddMinutes(i) + "}",
                        DateTime.UtcNow.Date.AddMinutes(i).AddDays(-2));
                    telemetryDataSinkTimeSeries.Record(message);
                }
            }

            var telemetryDataRecords = telemetryDataSinkTimeSeries.GetTimeSeries(devices.Keys, DateTime.UtcNow /* only date part matters*/);

            var groups = telemetryDataRecords.GroupBy(tdr => tdr.DeviceId);
            Assert.AreEqual(devices.Keys.Count, groups.Count());
            Assert.AreEqual(100, telemetryDataRecords.Count());

            foreach (var grp in groups)
            {
                var expected = devices[grp.Key];
                var real = grp.ToList();
                for (int i = 0; i < expected.Count; i++)
                {
                    Assert.AreEqual(expected[i].Payload, real[i].Payload);
                    Assert.AreEqual(expected[i].Time, real[i].Time);
                }
            }
        }
    }
}
