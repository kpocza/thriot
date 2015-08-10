using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Platform.Services.Telemetry.Dtos;
using Thriot.Platform.Services.Telemetry.Metadata;
using Thriot.Framework;
using Thriot.Objects.Model;

namespace Thriot.Platform.Services.Telemetry.Tests
{
    [TestClass]
    public class TelemetryDataSinkSetupServiceTest
    {
        [TestMethod]
        public void GetTelemetryDataSinksMetadataTest()
        {
            var telemetryDataSinkMetadataRegistry = Substitute.For<ITelemetryDataSinkMetadataRegistry>();

            telemetryDataSinkMetadataRegistry.Incoming.Returns(new List<TelemetryDataSinkMetadata>
            {
                new TelemetryDataSinkMetadata("test", "Test Desc", typeof (TelemetryDataSinkPreparatorTest.IncomingData), new[] {"ConnectionString", "Key", "Table"}, new Dictionary<string, string>())
            });

            var telemetryDataSinkPreparator = new TelemetryDataSinkPreparator(telemetryDataSinkMetadataRegistry, new DynamicConnectionStringResolver(null));

            var telemetryDataSinkSetupService = new TelemetryDataSinkSetupService(telemetryDataSinkMetadataRegistry, telemetryDataSinkPreparator);
        
            var metaDatas = telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata().Incoming;
            Assert.AreEqual(1, metaDatas.Count);
            Assert.AreEqual("test", metaDatas[0].Name);
            Assert.AreEqual("Test Desc", metaDatas[0].Description);
            Assert.AreEqual(3, metaDatas[0].ParametersToInput.Count);
        }

        [TestMethod]
        public void PrepareAndValidateIncomingTest()
        {
            var telemetryDataSinkMetadataRegistry = Substitute.For<ITelemetryDataSinkMetadataRegistry>();

            telemetryDataSinkMetadataRegistry.Incoming.Returns(new List<TelemetryDataSinkMetadata>
            {
                new TelemetryDataSinkMetadata("test", "Test Desc", typeof (TelemetryDataSinkPreparatorTest.IncomingData),
                    new[] {"ConnectionString", "Key", "Table"}, new Dictionary<string, string>())
            });

            var telemetryDataSinkPreparator = new TelemetryDataSinkPreparator(telemetryDataSinkMetadataRegistry, new DynamicConnectionStringResolver(null));

            var telemetryDataSinkSetupService = new TelemetryDataSinkSetupService(telemetryDataSinkMetadataRegistry,
                telemetryDataSinkPreparator);

            var telemetryDataSinkParametersList = new List<TelemetryDataSinkParametersDto>
            {
                new TelemetryDataSinkParametersDto
                {
                    SinkName = "test",
                    Parameters =
                        new Dictionary<string, string> {{"ConnectionString", "cs"}, {"Key", "k"}, {"Table", "t"}}
                }
            };

            telemetryDataSinkSetupService.PrepareAndValidateIncoming(telemetryDataSinkParametersList);
        }
    }
}
