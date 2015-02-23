using System.Configuration;
using System.Net;
using System.Threading;
using System.Web.Http;
using IoT.Framework;
using IoT.Framework.Web;
using IoT.Framework.Web.ApiExceptions;
using IoT.Framework.Web.Logging;
using IoT.Platform.Model;
using IoT.Platform.Services.Messaging;
using IoT.Platform.Services.Telemetry;
using IoT.Platform.Services.Telemetry.Configuration;
using IoT.Platform.Services.Telemetry.Metadata;
using IoT.Platform.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.BuilderProperties;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(Startup))] 

namespace IoT.Platform.WebApi
{
    public  class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServicePointManager.DefaultConnectionLimit = 10000;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            var config = new HttpConfiguration();
            config.DependencyResolver = new UnityWebApiResolver(SingleContainer.Instance.Container);
            config.MapHttpAttributeRoutes();
            config.Filters.Add(new LogActionsAttribute());
            config.Filters.Add(new ApiExceptionFilterAttribute());

            app.UseCors(CorsOptions.AllowAll);
            
            app.UseWebApi(config);

            var messagingService = MessagingServiceConfig.Register();

            DtoMapper.Setup();
            var telemetryDataSinkMetadataRegistry = (TelemetryDataSinkMetadataRegistry)SingleContainer.Instance.Resolve<ITelemetryDataSinkMetadataRegistry>();
            var telemetryDataSection = (TelemetryDataSection)ConfigurationManager.GetSection("telemetryDataSink");
            telemetryDataSinkMetadataRegistry.Build(telemetryDataSection);

            var batchParameters = SingleContainer.Instance.Resolve<IBatchParameters>();
            
            MessagingWorkers.Start(batchParameters, messagingService);

            var properties = new AppProperties(app.Properties);
            var token = properties.OnAppDisposing;

            if (token != CancellationToken.None)
            {
                token.Register(MessagingWorkers.Stop);
            }
        }
    }
}