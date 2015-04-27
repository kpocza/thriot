using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Thriot.Framework;
using Thriot.Framework.Logging;
using Thriot.Reporting.Dto;
using Thriot.Reporting.Services;
using Thriot.Reporting.WebApi.Auth;
using Thriot.Reporting.WebApi.Formatters;

namespace Thriot.Reporting.WebApi.Controllers
{
    [RoutePrefix("v1/networks")]
    [WebApiNetworkAuthenticator]
    public class NetworkController : ApiController, ILoggerOwner
    {
        private readonly NetworkReportingService _reportingService;
        private readonly NetworkAuthenticationContext _networkAuthenticationContext;

        public NetworkController(NetworkReportingService reportingService, NetworkAuthenticationContext networkAuthenticationContext)
        {
            _reportingService = reportingService;
            _networkAuthenticationContext = networkAuthenticationContext;
        }

        [Route("sinks")]
        [HttpGet]
        public IEnumerable<SinkInfoDto> GetSinks()
        {
            var networkId = _networkAuthenticationContext.GetContextNetwork(this.Request);

            return _reportingService.GetSinks(networkId);
        }

        [Route("json/{sinkName}")]
        [HttpGet]
        public CurrentDataReportDto GetCurrentDataJson(string sinkName)
        {
            var networkId = _networkAuthenticationContext.GetContextNetwork(this.Request);

            return _reportingService.CurrentDataStructuredReport(new SinkAndNetworkDto { NetworkId = networkId, SinkName = sinkName });
        }

        [Route("json/{sinkName}/{timestamp:long}")]
        [HttpGet]
        public TimeSeriesReportDto GetTimeSeriesReportJson(string sinkName, long timestamp)
        {
            var networkId = _networkAuthenticationContext.GetContextNetwork(this.Request);

            return
                _reportingService.TimeSeriesStructuredReport(
                    new SinkAndNetworkDto { NetworkId = networkId, SinkName = sinkName },
                    DateTimeExtensions.FromUnixTime(timestamp));
        }

        [Route("csv/{sinkName}")]
        [HttpGet]
        public HttpResponseMessage GetCurrentDataCsv(string sinkName)
        {
            var networkId = _networkAuthenticationContext.GetContextNetwork(this.Request);
            var data = _reportingService.CurrentDataFlatReport(new SinkAndNetworkDto { NetworkId = networkId, SinkName = sinkName });

            return CsvFormatter.ToHttpResponseMessage(data);
        }

        [Route("csv/{sinkName}/{timestamp:long}")]
        [HttpGet]
        public HttpResponseMessage GetTimeSeriesReportCsv(string sinkName, long timestamp)
        {
            var networkId = _networkAuthenticationContext.GetContextNetwork(this.Request);

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
            get { return _networkAuthenticationContext.GetContextNetwork(this.Request); }
        }
    }
}
