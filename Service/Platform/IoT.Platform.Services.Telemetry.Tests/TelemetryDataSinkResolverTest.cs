using System.Collections.Generic;
using System.Linq;
using IoT.Framework;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.Platform.Services.Telemetry.Metadata;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IoT.Platform.Services.Telemetry.Tests
{
    [TestClass]
    public class TelemetryDataSinkResolverTest
    {
        [TestMethod]
        public void ResolveIncomingEmptyAllTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkeOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkMetadataRegistry = Substitute.For<ITelemetryDataSinkMetadataRegistry>();

            var deviceId = Identity.Next();

            var serviceId = Identity.Next();
            var networkId = Identity.Next();
            var companyId = Identity.Next();

            deviceOperations.Get(deviceId).Returns(TestDataCreator.Device(deviceId, null, networkId, serviceId, companyId, 0));
            serviceOperations.Get(serviceId)
                .Returns(TestDataCreator.Service(serviceId, null,
                    companyId, new TelemetryDataSinkSettings {Incoming = new List<TelemetryDataSinkParameters>()}));
            companyOperations.Get(companyId)
                .Returns(TestDataCreator.Company(companyId, new TelemetryDataSinkSettings { Incoming = new List<TelemetryDataSinkParameters>() }));
            networkeOperations.Get(networkId)
                .Returns(TestDataCreator.Network(networkId, "key", null, serviceId, companyId, new TelemetryDataSinkSettings { Incoming = new List<TelemetryDataSinkParameters>() }));

            var telemetryDataSinkResolver = new TelemetryDataSinkResolver(deviceOperations, networkeOperations, serviceOperations, companyOperations,
                telemetryDataSinkMetadataRegistry);

            Assert.AreEqual(0, telemetryDataSinkResolver.ResolveIncoming(deviceId).Count());
        }

        [TestMethod]
        public void ResolveIncomingNullAllTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkeOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkMetadataRegistry = Substitute.For<ITelemetryDataSinkMetadataRegistry>();

            var deviceId = Identity.Next();

            var serviceId = Identity.Next();
            var networkId = Identity.Next();
            var companyId = Identity.Next();

            deviceOperations.Get(deviceId).Returns(TestDataCreator.Device(deviceId, null, networkId, serviceId, companyId, 0));
            serviceOperations.Get(serviceId)
                .Returns(TestDataCreator.Service(serviceId, null,
                    companyId, new TelemetryDataSinkSettings { Incoming = null }));
            companyOperations.Get(companyId)
                .Returns(TestDataCreator.Company(companyId, new TelemetryDataSinkSettings { Incoming = null }));
            networkeOperations.Get(networkId)
                 .Returns(TestDataCreator.Network(networkId, "key", null, serviceId, companyId, new TelemetryDataSinkSettings { Incoming = new List<TelemetryDataSinkParameters>() }));

            var telemetryDataSinkResolver = new TelemetryDataSinkResolver(deviceOperations, networkeOperations, serviceOperations, companyOperations,
                telemetryDataSinkMetadataRegistry);

            Assert.AreEqual(0, telemetryDataSinkResolver.ResolveIncoming(deviceId).Count());
        }

        [TestMethod]
        public void ResolveIncomingNetworkTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkeOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkMetadataRegistry = Substitute.For<ITelemetryDataSinkMetadataRegistry>();

            var deviceId = Identity.Next();

            var serviceId = Identity.Next();
            var networkId = Identity.Next();
            var companyId = Identity.Next();

            telemetryDataSinkMetadataRegistry.Incoming.Returns(new List<TelemetryDataSinkMetadata>
            {
                new TelemetryDataSinkMetadata("azureData", "data", typeof (IncomingStubs.CurrentDataStub),
                    new[] {"ConnectionString", "Table"}, new Dictionary<string, string>())
            });

            deviceOperations.Get(deviceId).Returns(TestDataCreator.Device(deviceId, null, networkId, serviceId, companyId, 0));
            serviceOperations.Get(serviceId)
                .Returns(TestDataCreator.Service(serviceId, null,
                    companyId, new TelemetryDataSinkSettings { Incoming = new List<TelemetryDataSinkParameters>() }));
            companyOperations.Get(companyId)
                .Returns(TestDataCreator.Company(companyId, new TelemetryDataSinkSettings { Incoming = new List<TelemetryDataSinkParameters>() }));
            networkeOperations.Get(networkId)
                 .Returns(TestDataCreator.Network(networkId, "key", null, serviceId, companyId, new TelemetryDataSinkSettings
                 {
                     Incoming =
                         new List<TelemetryDataSinkParameters>
                            {
                                new TelemetryDataSinkParameters()
                                {
                                    SinkName = "azureData",
                                    Parameters =
                                        new Dictionary<string, string>()
                                        {
                                            {"ConnectionString", "that"},
                                            {"Table", "t"}
                                        }
                                }
                            }
                 }));

            var telemetryDataSinkResolver = new TelemetryDataSinkResolver(deviceOperations, networkeOperations, serviceOperations, companyOperations,
                telemetryDataSinkMetadataRegistry);

            Assert.AreEqual(1, telemetryDataSinkResolver.ResolveIncoming(deviceId).Count());
            Assert.IsInstanceOfType(telemetryDataSinkResolver.ResolveIncoming(deviceId).First(), typeof(IncomingStubs.CurrentDataStub));
        }

        [TestMethod]
        public void ResolveIncomingServiceTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkeOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkMetadataRegistry = Substitute.For<ITelemetryDataSinkMetadataRegistry>();

            var deviceId = Identity.Next();

            var serviceId = Identity.Next();
            var networkId = Identity.Next();
            var companyId = Identity.Next();

            telemetryDataSinkMetadataRegistry.Incoming.Returns(new List<TelemetryDataSinkMetadata>
            {
                new TelemetryDataSinkMetadata("azureData", "data", typeof (IncomingStubs.CurrentDataStub),
                    new[] {"ConnectionString", "Table"}, new Dictionary<string, string>())
            });

            deviceOperations.Get(deviceId).Returns(TestDataCreator.Device(deviceId, null, networkId, serviceId, companyId, 0));
            serviceOperations.Get(serviceId)
                .Returns(TestDataCreator.Service(serviceId, null,
                    companyId, new TelemetryDataSinkSettings
                    {
                        Incoming =
                            new List<TelemetryDataSinkParameters>
                            {
                                new TelemetryDataSinkParameters()
                                {
                                    SinkName = "azureData",
                                    Parameters =
                                        new Dictionary<string, string>()
                                        {
                                            {"ConnectionString", "that"},
                                            {"Table", "t"}
                                        }
                                }
                            }
                    }));
            companyOperations.Get(companyId)
                .Returns(TestDataCreator.Company(companyId, new TelemetryDataSinkSettings { Incoming = new List<TelemetryDataSinkParameters>() }));
            networkeOperations.Get(networkId)
                 .Returns(TestDataCreator.Network(networkId, "key", null, serviceId, companyId, new TelemetryDataSinkSettings { Incoming = new List<TelemetryDataSinkParameters>() }));

            var telemetryDataSinkResolver = new TelemetryDataSinkResolver(deviceOperations, networkeOperations, serviceOperations, companyOperations,
                telemetryDataSinkMetadataRegistry);

            Assert.AreEqual(1, telemetryDataSinkResolver.ResolveIncoming(deviceId).Count());
            Assert.IsInstanceOfType(telemetryDataSinkResolver.ResolveIncoming(deviceId).First(), typeof(IncomingStubs.CurrentDataStub));
        }

        [TestMethod]
        public void ResolveIncomingServiceMultiTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkeOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkMetadataRegistry = Substitute.For<ITelemetryDataSinkMetadataRegistry>();

            var deviceId = Identity.Next();

            var serviceId = Identity.Next();
            var networkId = Identity.Next();
            var companyId = Identity.Next();

            telemetryDataSinkMetadataRegistry.Incoming.Returns(new List<TelemetryDataSinkMetadata>
            {
                new TelemetryDataSinkMetadata("azureData", "data", typeof (IncomingStubs.CurrentDataStub),
                    new[] {"ConnectionString", "Table"}, new Dictionary<string, string>()),
                new TelemetryDataSinkMetadata("azureTimeSeries", "data", typeof (IncomingStubs.TimeSeriesStub),
                    new[] {"ConnectionString", "Table"}, new Dictionary<string, string>())
            });

            deviceOperations.Get(deviceId).Returns(TestDataCreator.Device(deviceId, null, networkId, serviceId, companyId, 0));
            serviceOperations.Get(serviceId)
                .Returns(TestDataCreator.Service(serviceId, null,
                    companyId, new TelemetryDataSinkSettings
                    {
                        Incoming =
                            new List<TelemetryDataSinkParameters>
                            {
                                new TelemetryDataSinkParameters()
                                {
                                    SinkName = "azureData",
                                    Parameters =
                                        new Dictionary<string, string>()
                                        {
                                            {"ConnectionString", "that"},
                                            {"Table", "t"}
                                        }
                                },
                                new TelemetryDataSinkParameters()
                                {
                                    SinkName = "azureTimeSeries",
                                    Parameters =
                                        new Dictionary<string, string>()
                                        {
                                            {"ConnectionString", "that"},
                                            {"Table", "t"}
                                        }
                                }
                            }
                    }));
            companyOperations.Get(companyId)
                .Returns(TestDataCreator.Company(companyId, new TelemetryDataSinkSettings { Incoming = new List<TelemetryDataSinkParameters>() }));
            networkeOperations.Get(networkId)
                 .Returns(TestDataCreator.Network(networkId, "key", null, serviceId, companyId, new TelemetryDataSinkSettings { Incoming = new List<TelemetryDataSinkParameters>() }));

            var telemetryDataSinkResolver = new TelemetryDataSinkResolver(deviceOperations, networkeOperations, serviceOperations, companyOperations,
                telemetryDataSinkMetadataRegistry);

            Assert.AreEqual(2, telemetryDataSinkResolver.ResolveIncoming(deviceId).Count());
            Assert.IsInstanceOfType(telemetryDataSinkResolver.ResolveIncoming(deviceId).First(), typeof(IncomingStubs.CurrentDataStub));
            Assert.IsInstanceOfType(telemetryDataSinkResolver.ResolveIncoming(deviceId).Last(), typeof(IncomingStubs.TimeSeriesStub));
        }

        [TestMethod]
        public void ResolveIncomingCompanyTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkeOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkMetadataRegistry = Substitute.For<ITelemetryDataSinkMetadataRegistry>();

            var deviceId = Identity.Next();

            var serviceId = Identity.Next();
            var networkId = Identity.Next();
            var companyId = Identity.Next();

            telemetryDataSinkMetadataRegistry.Incoming.Returns(new List<TelemetryDataSinkMetadata>
            {
                new TelemetryDataSinkMetadata("azureData", "data", typeof (IncomingStubs.CurrentDataStub),
                    new[] {"ConnectionString", "Table"}, new Dictionary<string, string>())
            });

            deviceOperations.Get(deviceId).Returns(TestDataCreator.Device(deviceId, null, networkId, serviceId, companyId, 0));
            serviceOperations.Get(serviceId)
                .Returns(TestDataCreator.Service(serviceId, null,
                    companyId, new TelemetryDataSinkSettings {Incoming = new List<TelemetryDataSinkParameters>()}));

            companyOperations.Get(companyId)
                .Returns(TestDataCreator.Company(companyId, new TelemetryDataSinkSettings
                {
                    Incoming =
                        new List<TelemetryDataSinkParameters>
                        {
                            new TelemetryDataSinkParameters()
                            {
                                SinkName = "azureData",
                                Parameters =
                                    new Dictionary<string, string>()
                                    {
                                        {"ConnectionString", "that"},
                                        {"Table", "t"}
                                    }
                            }
                        }
                }));
            networkeOperations.Get(networkId)
                 .Returns(TestDataCreator.Network(networkId, "key", null, serviceId, companyId, new TelemetryDataSinkSettings { Incoming = new List<TelemetryDataSinkParameters>() }));

            var telemetryDataSinkResolver = new TelemetryDataSinkResolver(deviceOperations, networkeOperations, serviceOperations, companyOperations,
                telemetryDataSinkMetadataRegistry);

            Assert.AreEqual(1, telemetryDataSinkResolver.ResolveIncoming(deviceId).Count());
            Assert.IsInstanceOfType(telemetryDataSinkResolver.ResolveIncoming(deviceId).First(), typeof(IncomingStubs.CurrentDataStub));
        }
    }
}
