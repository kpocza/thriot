using System.Configuration;
using System.Net;
using System.Threading;
using System.Web.Http;
using Thriot.Platform.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.BuilderProperties;
using Microsoft.Owin.Cors;
using Owin;
using Thriot.Framework;
using Thriot.Framework.Web;
using Thriot.Framework.Web.ApiExceptions;
using Thriot.Framework.Web.Logging;
using Thriot.Platform.Model;
using Thriot.Platform.Services.Messaging;
using Thriot.Platform.Services.Telemetry;
using Thriot.Platform.Services.Telemetry.Configuration;
using Thriot.Platform.Services.Telemetry.Metadata;

[assembly: OwinStartup(typeof(Startup))] 

namespace Thriot.Platform.WebApi
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