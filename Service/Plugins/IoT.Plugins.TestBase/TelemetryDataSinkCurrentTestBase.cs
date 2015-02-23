using System;
using System.Collections.Generic;
using System.Linq;
using IoT.Management.Dto;
using IoT.Plugins.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Plugins.TestBase
{
    public abstract class TelemetryDataSinkCurrentTestBase : TestBase
    {
        protected abstract ITelemetryDataSinkCurrent GetTelemetryDataSinkCurrent();
        protected abstract string GetConnectionString();

        public virtual void RecordTest()
        {
            if (!IsIntegrationTest())
                return;

            var telemetryDataSinkCurrent = GetTelemetryDataSinkCurrent();
            telemetryDataSinkCurrent.Setup(new Dictionary<string, string>
            {
                {"ConnectionString", GetConnectionString()},
                {"Table", "CurrentData"}
            });

            telemetryDataSinkCurrent.Initialize();

            var message = new TelemetryData(_deviceId, "{ \"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}", DateTime.UtcNow);

            telemetryDataSinkCurrent.Record(message);
        }

        public virtual void RecordTwiceTest()
        {
            if (!IsIntegrationTest())
                return;

            var telemetryDataSinkCurrent = GetTelemetryDataSinkCurrent();
            telemetryDataSinkCurrent.Setup(new Dictionary<string, string>
            {
                {"ConnectionString", GetConnectionString()},
                {"Table", "CurrentData"}
            });

            telemetryDataSinkCurrent.Initialize();

            var message = new TelemetryData(_deviceId, "{ \"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}", DateTime.UtcNow);

            telemetryDataSinkCurrent.Record(message);
            telemetryDataSinkCurrent.Record(message);
        }

        public virtual void QueryDeviceCurrentData()
        {
            if (!IsIntegrationTest())
                return;

            var telemetryDataSinkCurrent = GetTelemetryDataSinkCurrent();
            telemetryDataSinkCurrent.Setup(new Dictionary<string, string>
            {
                {"ConnectionString", GetConnectionString()},
                {"Table", "CurrentData"}
            });

            telemetryDataSinkCurrent.Initialize();

            var devices = new Dictionary<string, string>();

            for (int i = 0; i < 30; i++)
            {
                var device = new DeviceDto()
                {
                    NetworkId = _networkId,
                    CompanyId = _companyId,
                    ServiceId = _serviceId,
                    Name = "new device" + i
                };

                var deviceId = _deviceService.Create(device);
                devices.Add(deviceId, null);
            }

            foreach (var deviceId in devices.Keys.ToList())
            {
                var message = new TelemetryData(deviceId, "{ \"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}", DateTime.UtcNow);

                telemetryDataSinkCurrent.Record(message);
                devices[deviceId] = message.Payload;
            }

            var telemetryDataRecords = telemetryDataSinkCurrent.GetCurrentData(devices.Keys);

            Assert.AreEqual(devices.Keys.Count, telemetryDataRecords.Count());
            Assert.IsTrue(telemetryDataRecords.All(t => devices[t.DeviceId] == t.Payload));
        }
    }
}
