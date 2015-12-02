using System.ServiceProcess;
using Microsoft.Extensions.DependencyInjection;
using Thriot.Platform.Services.Telemetry.Recording;

namespace Thriot.Platform.TelemetryQueueService
{
    public partial class TelemetryQueueServiceInstance : ServiceBase
    {
        private QueueProcessor _queueProcessor;

        public TelemetryQueueServiceInstance()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var servicesSetup = new ServicesSetup();
            servicesSetup.Setup();

            var serviceProvider = servicesSetup.GetServiceProvider();
            _queueProcessor = serviceProvider.GetService<QueueProcessor>();

            _queueProcessor.Start();
        }

        protected override void OnStop()
        {
            _queueProcessor.Stop();
        }
    }
}
