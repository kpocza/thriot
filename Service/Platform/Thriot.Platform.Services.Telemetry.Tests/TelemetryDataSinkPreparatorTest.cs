using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Objects.Model;
using Thriot.Platform.Services.Telemetry.Metadata;
using Thriot.Plugins.Core;
using Thriot.Framework;
using Thriot.Framework.DataAccess;

namespace Thriot.Platform.Services.Telemetry.Tests
{
    [TestClass]
    public class TelemetryDataSinkPreparatorTest
    {
        private static int _setupCount;
        private static int _initCount;
        private static IDictionary<string, string> _dataParameters;

        [TestMethod]
        public void PrepareTest()
        {
            _setupCount = 0;
            _initCount = 0;

            var telemetryDataSinkMetadataRegistry = Substitute.For<ITelemetryDataSinkMetadataRegistry>();

            telemetryDataSinkMetadataRegistry.Incoming.Returns(new List<TelemetryDataSinkMetadata>
            {
                new TelemetryDataSinkMetadata("test", "Test Desc", typeof (IncomingData), new[] {"ConnectionString", "Key", "Table"}, new Dictionary<string, string>())
            });

            var telemetryDataSinkPreparator = new TelemetryDataSinkPreparator(telemetryDataSinkMetadataRegistry, new DynamicConnectionStringResolver(null));

            telemetryDataSinkPreparator.PrepareAndValidateIncoming(new[]
            {
                new TelemetryDataSinkParameters()
                {
                    SinkName = "test",
                    Parameters = new Dictionary<string, string> {{"ConnectionString", "n"}, {"Key", "k"}, {"Table", "t"}}
                }
            });

            Assert.AreEqual(1, _setupCount);
            Assert.AreEqual(1, _initCount);
            Assert.AreEqual(3, _dataParameters.Count);
        }

        [TestMethod]
        public void PrepareWithPresetTest()
        {
            _setupCount = 0;
            _initCount = 0;

            var telemetryDataSinkMetadataRegistry = Substitute.For<ITelemetryDataSinkMetadataRegistry>();

            telemetryDataSinkMetadataRegistry.Incoming.Returns(new List<TelemetryDataSinkMetadata>
            {
                new TelemetryDataSinkMetadata("test", "Test Desc", typeof (IncomingData), new[] {"ConnectionString", "Key", "Table"}, new Dictionary<string, string> {{"ConnectionString", "nn"}})
            });

            var telemetryDataSinkPreparator = new TelemetryDataSinkPreparator(telemetryDataSinkMetadataRegistry, new DynamicConnectionStringResolver(null));

            telemetryDataSinkPreparator.PrepareAndValidateIncoming(new[]
            {
                new TelemetryDataSinkParameters()
                {
                    SinkName = "test",
                    Parameters = new Dictionary<string, string> {{"Key", "k"}, {"Table", "t"}}
                }
            });

            Assert.AreEqual(1, _setupCount);
            Assert.AreEqual(1, _initCount);
            Assert.AreEqual(3, _dataParameters.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PrepareNoSinkFoundTest()
        {
            _setupCount = 0;
            _initCount = 0;

            var telemetryDataSinkMetadataRegistry = Substitute.For<ITelemetryDataSinkMetadataRegistry>();

            telemetryDataSinkMetadataRegistry.Incoming.Returns(new List<TelemetryDataSinkMetadata>
            {
                new TelemetryDataSinkMetadata("test", "Test Desc", typeof (IncomingData), new[] {"ConnectionString", "Key", "Table"}, new Dictionary<string, string>())
            });

            var telemetryDataSinkPreparator = new TelemetryDataSinkPreparator(telemetryDataSinkMetadataRegistry, new DynamicConnectionStringResolver(null));

            telemetryDataSinkPreparator.PrepareAndValidateIncoming(new[]
            {
                new TelemetryDataSinkParameters()
                {
                    SinkName = "test2",
                    Parameters = new Dictionary<string, string> {{"ConnectionString", "n"}, {"Key", "k"}, {"Table", "t"}}
                }
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PrepareExtraParamFoundTest()
        {
            _setupCount = 0;
            _initCount = 0;

            var telemetryDataSinkMetadataRegistry = Substitute.For<ITelemetryDataSinkMetadataRegistry>();

            telemetryDataSinkMetadataRegistry.Incoming.Returns(new List<TelemetryDataSinkMetadata>
            {
                new TelemetryDataSinkMetadata("test", "Test Desc", typeof (IncomingData), new[] {"ConnectionString", "Key", "Table"}, new Dictionary<string, string>())
            });

            var telemetryDataSinkPreparator = new TelemetryDataSinkPreparator(telemetryDataSinkMetadataRegistry, new DynamicConnectionStringResolver(null));

            telemetryDataSinkPreparator.PrepareAndValidateIncoming(new[]
            {
                new TelemetryDataSinkParameters()
                {
                    SinkName = "test",
                    Parameters = new Dictionary<string, string> {{"ConnectionString", "n"}, {"Key", "k"}, {"Table", "t"}, {"Extra", "v"}}
                }
            });
        }

        public class IncomingData : ITelemetryDataSink
        {
            public void Setup(IDynamicConnectionStringResolver dynamicConnectionStringResolver, IDictionary<string, string> parameters)
            {
                _setupCount++;
                _dataParameters = parameters;
            }

            public void Initialize()
            {
                _initCount++;
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
    }
}
