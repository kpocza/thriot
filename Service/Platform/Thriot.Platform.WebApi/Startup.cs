using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using Thriot.Framework;
using Thriot.Framework.Mvc.ApiExceptions;
using Thriot.Framework.Mvc.Logging;
using Thriot.Objects.Common;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Platform.Model;
using Thriot.Platform.Model.Messaging;
using Thriot.Platform.Services.Messaging;
using Thriot.Platform.Services.Telemetry;
using Thriot.Platform.Services.Telemetry.Configuration;
using Thriot.Platform.Services.Telemetry.Metadata;
using Thriot.Platform.WebApi.Auth;
using Thriot.ServiceClient.Messaging;

namespace Thriot.Platform.WebApi
{
    public class Startup
    {
        private readonly IApplicationEnvironment _appEnv;

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            _appEnv = appEnv;

            DtoMapper.Setup();

            ServicePointManager.DefaultConnectionLimit = 10000;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder(_appEnv.ApplicationBasePath);
            configurationBuilder.AddJsonFile("config/services.json");
            configurationBuilder.AddJsonFile("config/connectionstring.json");

            var configuration = configurationBuilder.Build();

            services.AddMvc().Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new LogActionsAttribute());
                options.Filters.Add(new ApiExceptionFilterAttribute());
            });
            services.AddCors();
            services.ConfigureCors(c => c.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

            services.AddTransient<TelemetryDataSinkSetupService>();
            services.AddTransient<Services.Messaging.MessagingService>();
            services.AddTransient<AuthenticationContext>();
            services.AddTransient<TelemetryDataService>();
            services.AddTransient<TelemetryDataSinkPreparator>();
            services.AddTransient<ICompanyOperations, Objects.Common.CachingOperations.CompanyOperations>();
            services.AddTransient<IServiceOperations, Objects.Common.CachingOperations.ServiceOperations>();
            services.AddTransient<INetworkOperations, Objects.Common.CachingOperations.NetworkOperations>();
            services.AddTransient<IDeviceOperations, Objects.Common.CachingOperations.DeviceOperations>();
            services.AddTransient<ISettingOperations, Objects.Common.CachingOperations.SettingOperations>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<Framework.DataAccess.IDynamicConnectionStringResolver, DynamicConnectionStringResolver>();
            services.AddSingleton<ITelemetryDataSinkMetadataRegistry, TelemetryDataSinkMetadataRegistry>();
            services.AddTransient<ITelemetryDataSinkResolver, TelemetryDataSinkResolver>();
            services.AddSingleton<IBatchParameters, BatchParameters>();
            services.AddTransient<IMessagingOperations, MessagingOperations>();
            services.AddTransient<IDeviceAuthenticator, DeviceAuthenticator>();
            services.AddSingleton<IMessagingService, ServiceClient.Messaging.MessagingService>();
            services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.DataAccess.ConnectionParametersResolver>();
            services.AddSingleton(_ => configuration);
            
            foreach (var extraService in Framework.ServicesResolver.Resolve(configuration, "Services"))
            {
                services.AddTransient(extraService.Key, extraService.Value);
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var serviceProvider = app.ApplicationServices;

            var messagingService = serviceProvider.GetService<IMessagingService>();
            var settingOperations = serviceProvider.GetService<ISettingOperations>();

            messagingService.Setup(settingOperations.Get(Setting.MessagingServiceEndpoint).Value, settingOperations.Get(Setting.MessagingServiceApiKey).Value);

            var telemetryDataSinkMetadataRegistry = (TelemetryDataSinkMetadataRegistry)serviceProvider.GetService<ITelemetryDataSinkMetadataRegistry>();

            var xmlSerializer = new XmlSerializer(typeof(TelemetryDataSection));
            using (var streamReader = new StreamReader(Path.Combine(_appEnv.ApplicationBasePath, "config/telemetryDataSink.xml")))
            {
                var telemetryDataSection = (TelemetryDataSection)xmlSerializer.Deserialize(streamReader);
                telemetryDataSinkMetadataRegistry.Build(telemetryDataSection);
            }
            var batchParameters = serviceProvider.GetService<IBatchParameters>();

            MessagingWorkers.Start(batchParameters, messagingService);

            var applicationShutdown = serviceProvider.GetService<IApplicationShutdown>();
            applicationShutdown.ShutdownRequested.Register(MessagingWorkers.Stop);

            app.UseCors("AllowAll");

            app.UseMvc();
        }
    }
}
