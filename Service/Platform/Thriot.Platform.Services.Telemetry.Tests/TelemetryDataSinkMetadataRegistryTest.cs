using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Platform.Services.Telemetry.Configuration;
using Thriot.Platform.Services.Telemetry.Metadata;
using Thriot.Plugins.Core;
using Thriot.Framework.DataAccess;

namespace Thriot.Platform.Services.Telemetry.Tests
{
    [TestClass]
    public class TelemetryDataSinkMetadataRegistryTest
    {
        [TestMethod]
        public void FullLoadTest()
        {
            var telemetryDataSinksSection = new TelemetryDataSection
            {
                Incoming = new TelemetryDataSinkElement[]
                {
                    new TelemetryDataSinkElement
                    {
                        Name = "data1",
                        Type = typeof(IncomingData).AssemblyQualifiedName,
                        Description = "d1",
                        ParameterPresets = new TelemetrySinkParameter[]
                        {
                            new TelemetrySinkParameter { Key = "ConnectionString", Value = "v1" }
                        }
                    },
                    new TelemetryDataSinkElement
                    {
                        Name = "data2",
                        Type = typeof(IncomingData2).AssemblyQualifiedName,
                        Description = "d2",
                        ParameterPresets = new TelemetrySinkParameter[]
                        {
                            new TelemetrySinkParameter {Key = "ConnectionName", Value = "v2" }
                        }
                    },
                    new TelemetryDataSinkElement
                    {
                        Name = "ts",
                        Type = typeof(IncomingTimeSeries).AssemblyQualifiedName,
                        Description = "ts"
                    },
                }
            };

            var registry = new TelemetryDataSinkMetadataRegistry();

            registry.Build(telemetryDataSinksSection);

            Assert.AreEqual(3, registry.Incoming.Count());

            var data1 = registry.Incoming.First();
            Assert.AreEqual("data1", data1.Name);
            Assert.AreEqual("d1", data1.Description);
            Assert.AreEqual(2, data1.ParametersToInput.Count());
            Assert.IsTrue(data1.ParametersToInput.Contains("Key"));
            Assert.IsTrue(data1.ParametersToInput.Contains("Table"));
            Assert.AreEqual(1, data1.ParametersPresets.Count);
            Assert.AreEqual("v1", data1.ParametersPresets["ConnectionString"]);

            var data2 = registry.Incoming.Skip(1).First();
            Assert.AreEqual("data2", data2.Name);
            Assert.AreEqual("d2", data2.Description);
            Assert.AreEqual(1, data2.ParametersToInput.Count());
            Assert.IsTrue(data2.ParametersToInput.Contains("Table"));
            Assert.AreEqual(1, data2.ParametersPresets.Count);
            Assert.AreEqual("v2", data2.ParametersPresets["ConnectionName"]);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void UnknownTypeTest()
        {
            var telemetryDataSinksSection = new TelemetryDataSection
            {
                Incoming = new TelemetryDataSinkElement[]
                {
                    new TelemetryDataSinkElement
                    {
                        Name = "data1",
                        Type = "IoT.Plaform.Services.Tests.IncomingData2341234, IoT.Plaform.Services.Tests"
                    }
                }
            };

            var registry = new TelemetryDataSinkMetadataRegistry();

            registry.Build(telemetryDataSinksSection);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SamekeyTest()
        {
            var telemetryDataSinksSection = new TelemetryDataSection
            {
                Incoming = new TelemetryDataSinkElement[]
                {
                    new TelemetryDataSinkElement {Name = "data", Type = typeof(IncomingData).AssemblyQualifiedName},
                    new TelemetryDataSinkElement {Name = "data", Type = typeof(IncomingData).AssemblyQualifiedName}
                }
            };

            var registry = new TelemetryDataSinkMetadataRegistry();

            registry.Build(telemetryDataSinksSection);
        }
    }

    public class IncomingData : ITelemetryDataSink
    {
        public void Setup(IDynamicConnectionStringResolver dynamicConnectionStringResolver, IDictionary<string, string> parameters)
        {
        }

        public void Initialize()
        {
        }

        public void Record(TelemetryData message)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<string> ParametersNames
        {
            get { return new[] { "ConnectionString", "Key", "Table" }; }
        }

        public IDictionary<string, string> ParameterSubstitutes
        {
            get { return new Dictionary<string, string>(); }
        }
    }

    public class IncomingData2 : ITelemetryDataSink
    {
        public void Setup(IDynamicConnectionStringResolver dynamicConnectionStringResolver, IDictionary<string, string> parameters)
        {
        }

        public void Initialize()
        {
        }

        public void Record(TelemetryData message)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<string> ParametersNames
        {
            get { return new[] {"ConnectionString", "Table"}; }
        }

        public IDictionary<string, string> ParameterSubstitutes
        {
            get { return new Dictionary<string, string> {{"ConnectionName", "ConnectionString"}}; }
        }
    }

    public class IncomingTimeSeries : ITelemetryDataSink
    {
        public void Setup(IDynamicConnectionStringResolver dynamicConnectionStringResolver, IDictionary<string, string> parameters)
        {
        }

        public void Initialize()
        {
        }

        public void Record(TelemetryData message)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<string> ParametersNames
        {
            get { return new[] { "ConnectionString", "Table" }; }
        }

        public IDictionary<string, string> ParameterSubstitutes
        {
            get { return new Dictionary<string, string>(); }
        }
    }
}
