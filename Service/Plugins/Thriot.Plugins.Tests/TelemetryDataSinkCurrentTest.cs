using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Management.Services.Dto;
using Thriot.Plugins.Core;
using Thriot.TestHelpers;
using NSubstitute;
using Thriot.Objects.Model;

namespace Thriot.Plugins.Tests
{
    [TestClass]
    public class TelemetryDataSinkCurrentTest : TestBase
    {
        protected virtual ITelemetryDataSinkCurrent GetTelemetryDataSinkCurrent()
        {
            return EnvironmentFactoryFactory.Create().TelemetryEnvironment.DataSinkCurrent;
        }

        [TestInitialize]
        public void Init()
        {
            InitializeDevice();
        }

        [TestMethod]
        public void RecordTest()
        {
            if (!IsIntegrationTest())
                return;

            var telemetryDataSinkCurrent = GetTelemetryDataSinkCurrent();
            telemetryDataSinkCurrent.Setup(null, GetCurrentDataSettings());

            telemetryDataSinkCurrent.Initialize();

            var message = new TelemetryData(_deviceId, "{ \"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}", DateTime.UtcNow);

            telemetryDataSinkCurrent.Record(message);
        }

        [TestMethod]
        public void RecordTwiceTest()
        {
            if (!IsIntegrationTest())
                return;

            var telemetryDataSinkCurrent = GetTelemetryDataSinkCurrent();
            telemetryDataSinkCurrent.Setup(null, GetCurrentDataSettings());

            telemetryDataSinkCurrent.Initialize();

            var message = new TelemetryData(_deviceId, "{ \"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}", DateTime.UtcNow);

            telemetryDataSinkCurrent.Record(message);
            telemetryDataSinkCurrent.Record(message);
        }

        [TestMethod]
        public void QueryDeviceCurrentData()
        {
            if (!IsIntegrationTest())
                return;

            var telemetryDataSinkCurrent = GetTelemetryDataSinkCurrent();
            telemetryDataSinkCurrent.Setup(null, GetCurrentDataSettings());

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

        [TestMethod]
        public void ResolveConnectringStringByNameTest()
        {
            if (!IsIntegrationTest())
                return;

            var settingOperation = Substitute.For<Thriot.Objects.Model.Operations.ISettingOperations>();

            var dynamicConnectionStringResolver = new DynamicConnectionStringResolver(settingOperation);
            var telemetryEnvrionment = EnvironmentFactoryFactory.Create().TelemetryEnvironment;

            settingOperation.Get(null).ReturnsForAnyArgs(new Setting(SettingId.GetConnection("ResolvableConnectionString"), telemetryEnvrionment.ConnectionString));

            var settingsDictionary = GetCurrentDataSettings();
            settingsDictionary.Remove(telemetryEnvrionment.ConnectionStringParamName);
            settingsDictionary[telemetryEnvrionment.ConnectionStringNameName] = "ResolvableConnectionString";

            var telemetryDataSinkCurrent = GetTelemetryDataSinkCurrent();
            telemetryDataSinkCurrent.Setup(dynamicConnectionStringResolver, settingsDictionary);

            settingOperation.ReceivedWithAnyArgs(1).Get(Arg.Any<SettingId>());
        }
    }
}
