using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Plugins.Core;

namespace Thriot.Reporting.Tests
{
    public static class IncomingStubs
    {
        public class CurrentDataStub : ITelemetryDataSinkCurrent
        {
            public void Setup(IDictionary<string, string> parameters)
            {
            }

            public void Initialize()
            {
            }

            public void Record(TelemetryData message)
            {
            }

            public IReadOnlyCollection<string> ParametersNames
            {
                get { return new String[0]; }
            }

            public IDictionary<string, string> ParameterSubstitutes
            {
                get { return new Dictionary<string, string>(); }
            }

            public IEnumerable<TelemetryData> GetCurrentData(IEnumerable<string> deviceIds)
            {
                throw new NotImplementedException();
            }
        }

        public class TimeSeriesStub : ITelemetryDataSinkTimeSeries
        {
            public void Setup(IDictionary<string, string> parameters)
            {
            }

            public void Initialize()
            {

            }

            public void Record(TelemetryData message)
            {
            }

            public IReadOnlyCollection<string> ParametersNames
            {
                get { return new String[0]; }
            }

            public IDictionary<string, string> ParameterSubstitutes
            {
                get { return new Dictionary<string, string>(); }
            }

            public IEnumerable<TelemetryData> GetTimeSeries(IEnumerable<string> deviceIds, DateTime date)
            {
                throw new NotImplementedException();
            }
        }
    }
}
