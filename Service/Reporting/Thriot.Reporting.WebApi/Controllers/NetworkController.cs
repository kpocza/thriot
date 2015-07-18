using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using Thriot.Framework;
using Thriot.Framework.Logging;
using Thriot.Reporting.Dto;
using Thriot.Reporting.Services;
using Thriot.Reporting.WebApi.Auth;
using Thriot.Reporting.WebApi.Formatters;

namespace Thriot.Reporting.WebApi.Controllers
{
    [Route("v1/networks")]
    [WebApiNetworkAuthorization]
    public class NetworkController : Controller, ILoggerOwner
    {
        private readonly NetworkReportingService _reportingService;
        private readonly NetworkAuthenticationContext _networkAuthenticationContext;

        public NetworkController(NetworkReportingService reportingService, NetworkAuthenticationContext networkAuthenticationContext)
        {
            _reportingService = reportingService;
            _networkAuthenticationContext = networkAuthenticationContext;
        }

        [HttpGet("sinks")]
        public IEnumerable<SinkInfoDto> GetSinks()
        {
            var networkId = _networkAuthenticationContext.GetContextNetwork(this.Context);

            return _reportingService.GetSinks(networkId);
        }

        [HttpGet("json/{sinkName}")]
        public CurrentDataReportDto GetCurrentDataJson(string sinkName)
        {
            var networkId = _networkAuthenticationContext.GetContextNetwork(this.Context);

            return _reportingService.CurrentDataStructuredReport(new SinkAndNetworkDto { NetworkId = networkId, SinkName = sinkName });
        }

        [HttpGet("json/{sinkName}/{timestamp:long}")]
        public TimeSeriesReportDto GetTimeSeriesReportJson(string sinkName, long timestamp)
        {
            var networkId = _networkAuthenticationContext.GetContextNetwork(this.Context);

            return
                _reportingService.TimeSeriesStructuredReport(
                    new SinkAndNetworkDto { NetworkId = networkId, SinkName = sinkName },
                    DateTimeExtensions.FromUnixTime(timestamp));
        }

        [HttpGet("csv/{sinkName}")]
        public IActionResult GetCurrentDataCsv(string sinkName)
        {
            var networkId = _networkAuthenticationContext.GetContextNetwork(this.Context);
            var data = _reportingService.CurrentDataFlatReport(new SinkAndNetworkDto { NetworkId = networkId, SinkName = sinkName });

            return CsvFormatter.ToHttpResponseMessage(data);
        }

        [HttpGet("csv/{sinkName}/{timestamp:long}")]
        public IActionResult GetTimeSeriesReportCsv(string sinkName, long timestamp)
        {
            var networkId = _networkAuthenticationContext.GetContextNetwork(this.Context);

            var data = _reportingService.TimeSeriesFlatReport(
                    new SinkAndNetworkDto { NetworkId = networkId, SinkName = sinkName },
                    DateTimeExtensions.FromUnixTime(timestamp));

            return CsvFormatter.ToHttpResponseMessage(data);
        }

        private static readonly ILogger _logger = LoggerFactory.GetCurrentClassLogger();

        public ILogger Logger
        {
            get { return _logger; }
        }

        public string UserDefinedLogValue
        {
            get { return _networkAuthenticationContext.GetContextNetwork(this.Context); }
        }
    }
}
