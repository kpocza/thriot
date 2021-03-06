﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Thriot.Plugins.Core;
using Thriot.Framework.DataAccess;

namespace Thriot.Platform.Services.Telemetry.Tests
{
    public static class IncomingStubs
    {
        public static string DeviceId { get; private set; }
        public static int RecordCounter { get; private set; }

        public static void Initialize(string deviceId)
        {
            DeviceId = deviceId;
            RecordCounter = 0;
        }


        public class CurrentDataStub : ITelemetryDataSink
        {
            public void Setup(IDynamicConnectionStringResolver dynamicConnectionStringResolver, IDictionary<string, string> parameters)
            {
            }

            public void Initialize()
            {

            }

            public void Record(TelemetryData message)
            {
                Assert.AreEqual(DeviceId, message.DeviceId);
                Assert.AreEqual(24, JToken.Parse(message.Payload)["Temperature"].Value<int>());
                RecordCounter++;
            }

            public IReadOnlyCollection<string> ParametersNames
            {
                get { return new String[0]; }
            }

            public IDictionary<string, string> ParameterSubstitutes
            {
                get { return new Dictionary<string, string>(); }
            }
        }

        public class TimeSeriesStub : ITelemetryDataSink
        {
            public void Setup(IDynamicConnectionStringResolver dynamicConnectionStringResolver, IDictionary<string, string> parameters)
            {
            }

            public void Initialize()
            {

            }

            public void Record(TelemetryData message)
            {
                Assert.AreEqual(DeviceId, message.DeviceId);
                Assert.AreEqual(24, JToken.Parse(message.Payload)["Temperature"].Value<int>());
                RecordCounter++;
            }

            public IReadOnlyCollection<string> ParametersNames
            {
                get { return new String[0]; }
            }

            public IDictionary<string, string> ParameterSubstitutes
            {
                get { return new Dictionary<string, string>(); }
            }
        }
    }
}
