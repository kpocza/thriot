using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Dnx.Runtime;
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
using Thriot.Messaging.Services.Client;
using Thriot.Platform.Services.Telemetry.Recording;
using Thriot.Plugins.Core;

namespace Thriot.Platform.WebApi
{
    public class Startup
    {
        private readonly IApplicationEnvironment _appEnv;

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            _appEnv = appEnv;

            Framework.Logging.NLogLogger.SetConfiguration(
                Path.Combine(Path.Combine(appEnv.ApplicationBasePath, "config"), "web.nlog"));

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

            configurationBuilder.AddJsonFile("config/telemetryqueue.json", true);

            var configuration = configurationBuilder.Build();

            services.AddMvc();
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new LogActionsAttribute());
                options.Filters.Add(new ApiExceptionFilterAttribute());
            });

            services.AddCors();
            services.ConfigureCors(c => c.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

            ConfigureThriotServices(services, configuration);

            ConfigureTelemetryDataService(services, configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var serviceProvider = app.ApplicationServices;

            var messagingServiceClient = serviceProvider.GetService<IMessagingServiceClient>();
            var settingOperations = serviceProvider.GetService<ISettingOperations>();

            messagingServiceClient.Setup(settingOperations.Get(Setting.MessagingServiceEndpoint).Value, settingOperations.Get(Setting.MessagingServiceApiKey).Value);

            var telemetryDataSinkMetadataRegistry = (TelemetryDataSinkMetadataRegistry)serviceProvider.GetService<ITelemetryDataSinkMetadataRegistry>();

            var xmlSerializer = new XmlSerializer(typeof(TelemetryDataSection));
            using (var streamReader = new StreamReader(Path.Combine(_appEnv.ApplicationBasePath, "config/telemetryDataSink.xml")))
            {
                var telemetryDataSection = (TelemetryDataSection)xmlSerializer.Deserialize(streamReader);
                telemetryDataSinkMetadataRegistry.Build(telemetryDataSection);
            }
            var batchParameters = serviceProvider.GetService<IBatchParameters>();

            MessagingWorkers.Start(batchParameters, messagingServiceClient);

            var applicationShutdown = serviceProvider.GetService<IApplicationShutdown>();
            applicationShutdown.ShutdownRequested.Register(MessagingWorkers.Stop);

            app.UseCors("AllowAll");

            app.UseMvc();
        }

        private void ConfigureThriotServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TelemetryDataSinkSetupService>();
            services.AddTransient<MessagingService>();
            services.AddTransient<AuthenticationContext>();
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
            services.AddSingleton<IMessagingServiceClient, MessagingServiceClient>();
            services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.DataAccess.ConnectionParametersResolver>();
            services.AddSingleton(_ => configuration);

            foreach (var extraService in ConfigurationAdapter.LoadServiceConfiguration(configuration, "Services"))
            {
                services.AddTransient(extraService.Key, extraService.Value);
            }
        }

        private void ConfigureTelemetryDataService(IServiceCollection services, IConfiguration configuration)
        {
            if (!ConfigurationAdapter.HasRootSection(configuration, "TelemetryQueue"))
            {
                services.AddTransient<ITelemetryDataService, DirectTelemetryDataService>();
            }
            else
            {
                services.AddTransient<ITelemetryDataService, QueueingTelemetryDataService>();

                var telemetryQueueConfiguration = ConfigurationAdapter.AsMap(configuration, "TelemetryQueue");
                var queueSendAdapterType = Type.GetType(telemetryQueueConfiguration["QueueSendAdapter"]);

                services.AddTransient<IQueueSendAdapter>(_ =>
                {
                    var queueSendAdapter = (IQueueSendAdapter)Activator.CreateInstance(queueSendAdapterType);
                    queueSendAdapter.Setup(telemetryQueueConfiguration);
                    return queueSendAdapter;
                });
            }
        }
    }
}
