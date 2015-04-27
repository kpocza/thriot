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
    [RoutePrefix("v1/devices")]
    [WebApiDeviceAuthenticator]
    public class DeviceController : ApiController, ILoggerOwner
    {
        private readonly DeviceReportingService _reportingService;
        private readonly DeviceAuthenticationContext _deviceAuthenticationContext;

        public DeviceController(DeviceReportingService reportingService, DeviceAuthenticationContext deviceAuthenticationContext)
        {
            _reportingService = reportingService;
            _deviceAuthenticationContext = deviceAuthenticationContext;
        }

        [Route("sinks")]
        [HttpGet]
        public IEnumerable<SinkInfoDto> GetSinks()
        {
            var deviceId = _deviceAuthenticationContext.GetContextDevice(this.Request);

            return _reportingService.GetSinks(deviceId);
        }

        [Route("json/{sinkName}")]
        [HttpGet]
        public CurrentDataReportDto GetCurrentDataJson(string sinkName)
        {
            var deviceId = _deviceAuthenticationContext.GetContextDevice(this.Request);

            return _reportingService.CurrentDataStructuredReport(new SinkAndDeviceDto {DeviceId = deviceId, SinkName = sinkName});
        }

        [Route("json/{sinkName}/{timestamp:long}")]
        [HttpGet]
        public TimeSeriesReportDto GetTimeSeriesReportJson(string sinkName, long timestamp)
        {
            var deviceId = _deviceAuthenticationContext.GetContextDevice(this.Request);

            return
                _reportingService.TimeSeriesStructuredReport(
                    new SinkAndDeviceDto {DeviceId = deviceId, SinkName = sinkName},
                    DateTimeExtensions.FromUnixTime(timestamp));
        }

        [Route("csv/{sinkName}")]
        [HttpGet]
        public HttpResponseMessage GetCurrentDataCsv(string sinkName)
        {
            var deviceId = _deviceAuthenticationContext.GetContextDevice(this.Request);

            var data = _reportingService.CurrentDataFlatReport(new SinkAndDeviceDto { DeviceId = deviceId, SinkName = sinkName });

            return CsvFormatter.ToHttpResponseMessage(data);
        }

        [Route("csv/{sinkName}/{timestamp:long}")]
        [HttpGet]
        public HttpResponseMessage GetTimeSeriesReportCsv(string sinkName, long timestamp)
        {
            var deviceId = _deviceAuthenticationContext.GetContextDevice(this.Request);

            var data = _reportingService.TimeSeriesFlatReport(
                    new SinkAndDeviceDto { DeviceId = deviceId, SinkName = sinkName },
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
            get { return _deviceAuthenticationContext.GetContextDevice(this.Request); }
        }
    }
}
