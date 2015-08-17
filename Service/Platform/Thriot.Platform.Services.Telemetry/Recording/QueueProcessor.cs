using System;
using Thriot.Framework.Logging;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry.Recording
{
    public class QueueProcessor
    {
        private readonly IQueueReceiveAdapter _queueReceiveAdapter;
        private readonly IDirectTelemetryDataService _directTelemetryDataService;
        private readonly ILogger _logger = LoggerFactory.GetCurrentClassLogger();

        public QueueProcessor(IQueueReceiveAdapter queueReceiveAdapter, IDirectTelemetryDataService directTelemetryDataService)
        {
            _queueReceiveAdapter = queueReceiveAdapter;
            _directTelemetryDataService = directTelemetryDataService;
        }

        public void Start()
        {
            _queueReceiveAdapter.StartReceiver(telemetryData =>
            {
                try
                {
                    _logger.Info($"Recording telemetry data for device: {telemetryData.DeviceId}");

                    _directTelemetryDataService.RecordTelemetryData(telemetryData);

                    _logger.Debug($"Recording telemetry data for device: {telemetryData.DeviceId}");
                }
                catch (Exception ex)
                {
                    _logger.Error($"DeviceId: {telemetryData.DeviceId}. " + ex.ToString());
                }
            });
        }

        public void Stop()
        {
            _queueReceiveAdapter.StopReceiver();
        }
    }
}
