using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using Thriot.Framework;
using Thriot.Framework.Logging;
using Thriot.Reporting.Services.Dto;
using Thriot.Reporting.Services;
using Thriot.Reporting.WebApi.Auth;
using Thriot.Reporting.WebApi.Formatters;

namespace Thriot.Reporting.WebApi.Controllers
{
    [Route("v1/devices")]
    [WebApiDeviceAuthorization]
    public class DeviceController : Controller, ILoggerOwner
    {
        private readonly DeviceReportingService _reportingService;
        private readonly DeviceAuthenticationContext _deviceAuthenticationContext;

        public DeviceController(DeviceReportingService reportingService, DeviceAuthenticationContext deviceAuthenticationContext)
        {
            _reportingService = reportingService;
            _deviceAuthenticationContext = deviceAuthenticationContext;
        }

        [HttpGet("sinks")]
        public IEnumerable<SinkInfoDto> GetSinks()
        {
            var deviceId = _deviceAuthenticationContext.GetContextDevice(this.HttpContext);

            return _reportingService.GetSinks(deviceId);
        }

        [HttpGet("json/{sinkName}")]
        public CurrentDataReportDto GetCurrentDataJson(string sinkName)
        {
            var deviceId = _deviceAuthenticationContext.GetContextDevice(this.HttpContext);

            return _reportingService.CurrentDataStructuredReport(new SinkAndDeviceDto {DeviceId = deviceId, SinkName = sinkName});
        }

        [HttpGet("json/{sinkName}/{timestamp:long}")]
        public TimeSeriesReportDto GetTimeSeriesReportJson(string sinkName, long timestamp)
        {
            var deviceId = _deviceAuthenticationContext.GetContextDevice(this.HttpContext);

            return
                _reportingService.TimeSeriesStructuredReport(
                    new SinkAndDeviceDto {DeviceId = deviceId, SinkName = sinkName},
                    DateTimeExtensions.FromUnixTime(timestamp));
        }

        [HttpGet("csv/{sinkName}")]
        public IActionResult GetCurrentDataCsv(string sinkName)
        {
            var deviceId = _deviceAuthenticationContext.GetContextDevice(this.HttpContext);

            var data = _reportingService.CurrentDataFlatReport(new SinkAndDeviceDto { DeviceId = deviceId, SinkName = sinkName });

            return CsvFormatter.ToHttpResponseMessage(data);
        }

        [HttpGet("csv/{sinkName}/{timestamp:long}")]
        public IActionResult GetTimeSeriesReportCsv(string sinkName, long timestamp)
        {
            var deviceId = _deviceAuthenticationContext.GetContextDevice(this.HttpContext);

            var data = _reportingService.TimeSeriesFlatReport(
                    new SinkAndDeviceDto { DeviceId = deviceId, SinkName = sinkName },
                    DateTimeExtensions.FromUnixTime(timestamp));

            return CsvFormatter.ToHttpResponseMessage(data);
        }

        private static readonly ILogger _logger = LoggerFactory.GetCurrentClassLogger();

        public ILogger Logger => _logger;

        public string UserDefinedLogValue => _deviceAuthenticationContext.GetContextDevice(this.HttpContext);
    }
}
