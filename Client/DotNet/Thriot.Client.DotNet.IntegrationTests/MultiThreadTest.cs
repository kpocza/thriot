using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Client.DotNet.Management;
using Thriot.Client.DotNet.Platform;

namespace Thriot.Client.DotNet.IntegrationTests
{
    [TestClass]
    public class MultiThreadTest : TestBase
    {
        [TestMethod]
        public void MultiDeviceManySendTest()
        {
            var managementClient = new ManagementClient(ManagementApi);

            managementClient.User.Register(new Register
            {
                Email = Guid.NewGuid() + "@test.hu",
                Name = "test user",
                Password = "p@ssw0rd"
            });

            var companyId = managementClient.Company.Create(new Company { Name = "company" });
            var serviceId = managementClient.Service.Create(new Service { CompanyId = companyId, Name = "service" });
            var networkId = managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "árvíztűrő tükörfúrógép" });

            var apiKey = managementClient.Service.Get(serviceId).ApiKey;

            var telemetryDataSinkParameters = new List<TelemetryDataSinkParameters>
            {
                new TelemetryDataSinkParameters
                {
                    SinkName = SinkData,
                    Parameters = new Dictionary<string, string>()
                },
                new TelemetryDataSinkParameters
                {
                    SinkName = SinkTimeSeries,
                    Parameters = new Dictionary<string, string>()
                }
            };

            managementClient.Service.UpdateIncomingTelemetryDataSinks(serviceId, telemetryDataSinkParameters);

            const int deviceCount = 25;
            const int messageCount = 2;

            var list = new List<string>();
            Parallel.For(0, deviceCount, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, (i) =>
            {
                var deviceId = managementClient.Device.Create(new Device { CompanyId = companyId, ServiceId = serviceId, NetworkId = networkId, Name = "name" + i});
                lock (list)
                {
                    list.Add(deviceId);
                }
            });

            var tasks = new List<Task>();

            for (int i = 0; i < deviceCount; i++)
            {
                var deviceId = list[i];
                var task = Task.Factory.StartNew(() =>
                {
                    var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, deviceId, apiKey);
                    for (var j = 0; j < messageCount; j++)
                    {
                        ocassionalConnectionClient.RecordTelemetryData("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}");
                        Thread.Sleep(10);
                    }
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        [TestMethod]
        public void MultiDeviceRecordAndReceiveOutgoingTest()
        {
            var managementClient = new ManagementClient(ManagementApi);

            managementClient.User.Register(new Register
            {
                Email = Guid.NewGuid() + "@test.hu",
                Name = "test user",
                Password = "p@ssw0rd"
            });

            var companyId = managementClient.Company.Create(new Company { Name = "company" });
            var serviceId = managementClient.Service.Create(new Service { CompanyId = companyId, Name = "service" });
            var networkId = managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "árvíztűrő tükörfúrógép" });

            var apiKey = managementClient.Service.Get(serviceId).ApiKey;

            const int deviceCount = 10;
            const int messageCount = 2;

            var list = new List<string>();
            Parallel.For(0, deviceCount, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, (i) =>
            {
                var deviceId = managementClient.Device.Create(new Device { CompanyId = companyId, ServiceId = serviceId, NetworkId = networkId, Name = "name" + i });
                lock (list)
                {
                    list.Add(deviceId);
                }
            });

            var tasks = new List<Task>();

            for (int i = 0; i < deviceCount; i++)
            {
                var deviceId = list[i];
                var task = Task.Factory.StartNew(() =>
                {
                    var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, deviceId, apiKey);
                    for (var j = 0; j < messageCount; j++)
                    {
                        ocassionalConnectionClient.SendMessageTo(deviceId, "msg" + j);
                        Thread.Sleep(100);
                    }
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());

            tasks.Clear();
            for (int i = 0; i < deviceCount; i++)
            {
                var deviceId = list[i];
                var task = Task.Factory.StartNew(() =>
                {
                    var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, deviceId, apiKey);
                    for (var j = 0; j < messageCount; j++)
                    {
                        var message = ocassionalConnectionClient.ReceiveAndForgetMessage();
                        Assert.IsTrue(message.Payload.Contains("msg"));
                        Assert.AreEqual(deviceId, message.SenderDeviceId);
                    }
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }
    }
}
