using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Reporting.Services;
using Thriot.ServiceClient.TelemetrySetup;
using Thriot.Framework;

namespace Thriot.Reporting.Tests
{
    [TestClass]
    public class TelemetryDataSinkProcessorTests
    {
        [TestMethod]
        public void GetSinksForNetwork()
        {
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkSetupServiceClient = Substitute.For<ITelemetryDataSinkSetupServiceClient>();

            networkOperations.Get("2").Returns(new Network
            {
                ServiceId = "3", 
                CompanyId = "3", 
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>
                    {
                        new TelemetryDataSinkParameters
                        {
                            SinkName = "currentdata",
                            Parameters = new Dictionary<string, string>()
                        },
                        new TelemetryDataSinkParameters
                        {
                            SinkName = "timeseries",
                            Parameters = new Dictionary<string, string>()
                        }
                    }
                }
            });

            telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata()
                .Returns(c => new TelemetryDataSinksMetadataDto()
                {
                    Incoming = new List<TelemetryDataSinkMetadataDto>
                    {
                        new TelemetryDataSinkMetadataDto
                        {
                            Name = "currentdata",
                            ParametersPresets = new Dictionary<string, string>(),
                            ParametersToInput = new List<string>(),
                            TypeName = typeof (IncomingStubs.CurrentDataStub).AssemblyQualifiedName
                        },
                        new TelemetryDataSinkMetadataDto
                        {
                            Name = "timeseries",
                            ParametersPresets = new Dictionary<string, string>(),
                            ParametersToInput = new List<string>(),
                            TypeName = typeof (IncomingStubs.TimeSeriesStub).AssemblyQualifiedName
                        }
                    }
                });

            var telemetryDataSinkProcessor = new TelemetryDataSinkProcessor(telemetryDataSinkSetupServiceClient, networkOperations, serviceOperations, companyOperations, new DynamicConnectionStringResolver(null));

            var sinks = telemetryDataSinkProcessor.GetSinksForNetwork("2");

            Assert.AreEqual(2, sinks.Count());
        }

        [TestMethod]
        public void GetSinksForParentNetwork()
        {
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkSetupServiceClient = Substitute.For<ITelemetryDataSinkSetupServiceClient>();

            networkOperations.Get("2").Returns(new Network
            {
                ServiceId = "3",
                ParentNetworkId = "pnid",
                CompanyId = "3",
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>()
                }
            });

            networkOperations.Get("pnid").Returns(new Network
            {
                ServiceId = "3",
                CompanyId = "3",
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>
                    {
                        new TelemetryDataSinkParameters
                        {
                            SinkName = "currentdata",
                            Parameters = new Dictionary<string, string>()
                        }
                    }
                }
            });

            telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata()
                .Returns(c => new TelemetryDataSinksMetadataDto()
                {
                    Incoming = new List<TelemetryDataSinkMetadataDto>
                    {
                        new TelemetryDataSinkMetadataDto
                        {
                            Name = "currentdata",
                            ParametersPresets = new Dictionary<string, string>(),
                            ParametersToInput = new List<string>(),
                            TypeName = typeof (IncomingStubs.CurrentDataStub).AssemblyQualifiedName
                        }
                    }
                });

            var telemetryDataSinkProcessor = new TelemetryDataSinkProcessor(telemetryDataSinkSetupServiceClient, networkOperations, serviceOperations, companyOperations, new DynamicConnectionStringResolver(null));

            var sinks = telemetryDataSinkProcessor.GetSinksForNetwork("2");

            Assert.AreEqual(1, sinks.Count());
        }

        [TestMethod]
        public void GetSinksForService()
        {
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkSetupServiceClient = Substitute.For<ITelemetryDataSinkSetupServiceClient>();

            networkOperations.Get("2").Returns(new Network
            {
                ServiceId = "3",
                CompanyId = "3",
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>()
                }
            });

            serviceOperations.Get("3").Returns(new Service
            {
                CompanyId = "3",
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>
                    {
                        new TelemetryDataSinkParameters
                        {
                            SinkName = "currentdata",
                            Parameters = new Dictionary<string, string>()
                        }
                    }
                }
            });

            telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata()
                .Returns(c => new TelemetryDataSinksMetadataDto()
                {
                    Incoming = new List<TelemetryDataSinkMetadataDto>
                    {
                        new TelemetryDataSinkMetadataDto
                        {
                            Name = "currentdata",
                            ParametersPresets = new Dictionary<string, string>(),
                            ParametersToInput = new List<string>(),
                            TypeName = typeof (IncomingStubs.CurrentDataStub).AssemblyQualifiedName
                        }
                    }
                });

            var telemetryDataSinkProcessor = new TelemetryDataSinkProcessor(telemetryDataSinkSetupServiceClient, networkOperations, serviceOperations, companyOperations, new DynamicConnectionStringResolver(null));

            var sinks = telemetryDataSinkProcessor.GetSinksForNetwork("2");

            Assert.AreEqual(1, sinks.Count());
        }

        [TestMethod]
        public void GetSinksForCompany()
        {
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkSetupServiceClient = Substitute.For<ITelemetryDataSinkSetupServiceClient>();

            networkOperations.Get("2").Returns(new Network
            {
                ServiceId = "3",
                CompanyId = "4",
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>()
                }
            });

            serviceOperations.Get("3").Returns(new Service
            {
                CompanyId = "3",
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>()
                }
            });

            companyOperations.Get("4").Returns(new Company
            {
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>
                    {
                        new TelemetryDataSinkParameters
                        {
                            SinkName = "currentdata",
                            Parameters = new Dictionary<string, string>()
                        }
                    }
                }
            });

            telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata()
                .Returns(c => new TelemetryDataSinksMetadataDto()
                {
                    Incoming = new List<TelemetryDataSinkMetadataDto>
                    {
                        new TelemetryDataSinkMetadataDto
                        {
                            Name = "currentdata",
                            ParametersPresets = new Dictionary<string, string>(),
                            ParametersToInput = new List<string>(),
                            TypeName = typeof (IncomingStubs.CurrentDataStub).AssemblyQualifiedName
                        }
                    }
                });

            var telemetryDataSinkProcessor = new TelemetryDataSinkProcessor(telemetryDataSinkSetupServiceClient, networkOperations, serviceOperations, companyOperations, new DynamicConnectionStringResolver(null));

            var sinks = telemetryDataSinkProcessor.GetSinksForNetwork("2");

            Assert.AreEqual(1, sinks.Count());
        }

        [TestMethod]
        public void GetSinksNothing()
        {
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkSetupServiceClient = Substitute.For<ITelemetryDataSinkSetupServiceClient>();

            networkOperations.Get("2").Returns(new Network
            {
                ServiceId = "3",
                CompanyId = "4",
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>()
                }
            });

            serviceOperations.Get("3").Returns(new Service
            {
                CompanyId = "3",
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>()
                }
            });

            companyOperations.Get("4").Returns(new Company
            {
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>()
                }
            });

            telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata()
                .Returns(c => new TelemetryDataSinksMetadataDto()
                {
                    Incoming = new List<TelemetryDataSinkMetadataDto>
                    {
                        new TelemetryDataSinkMetadataDto
                        {
                            Name = "currentdata",
                            ParametersPresets = new Dictionary<string, string>(),
                            ParametersToInput = new List<string>(),
                            TypeName = typeof (IncomingStubs.CurrentDataStub).AssemblyQualifiedName
                        }
                    }
                });

            var telemetryDataSinkProcessor = new TelemetryDataSinkProcessor(telemetryDataSinkSetupServiceClient, networkOperations, serviceOperations, companyOperations, new DynamicConnectionStringResolver(null));

            var sinks = telemetryDataSinkProcessor.GetSinksForNetwork("2");

            Assert.AreEqual(0, sinks.Count());
        }

        [TestMethod]
        public void WorkerTelemetryDataSinkTest()
        {
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkSetupServiceClient = Substitute.For<ITelemetryDataSinkSetupServiceClient>();

            networkOperations.Get("2").Returns(new Network
            {
                ServiceId = "3",
                CompanyId = "3",
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>
                    {
                        new TelemetryDataSinkParameters
                        {
                            SinkName = "currentdata",
                            Parameters = new Dictionary<string, string>()
                        }
                    }
                }
            });

            telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata()
                .Returns(c => new TelemetryDataSinksMetadataDto()
                {
                    Incoming = new List<TelemetryDataSinkMetadataDto>
                    {
                        new TelemetryDataSinkMetadataDto
                        {
                            Name = "currentdata",
                            ParametersPresets = new Dictionary<string, string>(),
                            ParametersToInput = new List<string>(),
                            TypeName = typeof (IncomingStubs.CurrentDataStub).AssemblyQualifiedName
                        }
                    }
                });

            var telemetryDataSinkProcessor = new TelemetryDataSinkProcessor(telemetryDataSinkSetupServiceClient, networkOperations, serviceOperations, companyOperations, new DynamicConnectionStringResolver(null));

            var sink = telemetryDataSinkProcessor.WorkerTelemetryDataSink("currentdata", "2");

            Assert.IsNotNull(sink);
        }

        [TestMethod]
        public void WorkerTelemetryDataSinkNotFoundTest()
        {
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();
            var companyOperations = Substitute.For<ICompanyOperations>();
            var telemetryDataSinkSetupServiceClient = Substitute.For<ITelemetryDataSinkSetupServiceClient>();

            networkOperations.Get("2").Returns(new Network
            {
                ServiceId = "3",
                CompanyId = "3",
                TelemetryDataSinkSettings = new TelemetryDataSinkSettings
                {
                    Incoming = new List<TelemetryDataSinkParameters>
                    {
                        new TelemetryDataSinkParameters
                        {
                            SinkName = "currentdata",
                            Parameters = new Dictionary<string, string>()
                        }
                    }
                }
            });

            telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata()
                .Returns(c => new TelemetryDataSinksMetadataDto()
                {
                    Incoming = new List<TelemetryDataSinkMetadataDto>()
                });

            var telemetryDataSinkProcessor = new TelemetryDataSinkProcessor(telemetryDataSinkSetupServiceClient, networkOperations, serviceOperations, companyOperations, new DynamicConnectionStringResolver(null));

            var sink = telemetryDataSinkProcessor.WorkerTelemetryDataSink("currentdatanosuch", "2");

            Assert.IsNull(sink);
        }
    }
}
