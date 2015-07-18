using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using System.Net;
using Thriot.Framework.Mvc.ApiExceptions;
using Thriot.Framework.Mvc.Logging;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Reporting.Services;
using Thriot.Reporting.WebApi.Auth;
using Thriot.ServiceClient.TelemetrySetup;

namespace Thriot.Reporting.WebApi
{
    public class Startup
    {
        private readonly IApplicationEnvironment _appEnv;

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            _appEnv = appEnv;

            ServicePointManager.DefaultConnectionLimit = 10000;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder(_appEnv.ApplicationBasePath);
            configurationBuilder.AddJsonFile("services.json");
            configurationBuilder.AddJsonFile("connectionstring.json");

            var configuration = configurationBuilder.Build();

            services.AddMvc().Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new LogActionsAttribute());
                options.Filters.Add(new ApiExceptionFilterAttribute());
            });

            services.AddTransient<DeviceReportingService>();
            services.AddTransient<NetworkReportingService>();
            services.AddTransient<DeviceAuthenticationContext>();
            services.AddTransient<NetworkAuthenticationContext>();
            services.AddSingleton<ITelemetryDataSinkSetupService, TelemetryDataSinkSetupService>();
            services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.Mvc.ConnectionParametersResolver>();
            services.AddSingleton<Framework.DataAccess.IDynamicConnectionStringResolver, DynamicConnectionStringResolver>();
            services.AddTransient<ITelemetryDataSinkProcessor, TelemetryDataSinkProcessor>();
            services.AddTransient<IDeviceAuthenticator, Objects.Common.DeviceAuthenticator> ();
            services.AddTransient<INetworkAuthenticator, Objects.Common.NetworkAuthenticator> ();
            services.AddSingleton(_ => configuration);

            foreach (var extraService in Framework.Mvc.ServicesResolver.Resolve(configuration, "Services"))
            {
                services.AddTransient(extraService.Key, extraService.Value);
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var serviceProvider = app.ApplicationServices;

            var telemetryDataSinkSetupService = serviceProvider.GetService<ITelemetryDataSinkSetupService>();
            var settingOperations = serviceProvider.GetService<ISettingOperations>();

            telemetryDataSinkSetupService.Setup(settingOperations.Get(Setting.TelemetrySetupServiceEndpoint).Value, settingOperations.Get(Setting.TelemetrySetupServiceApiKey).Value);

            app.UseCors(Microsoft.AspNet.Cors.Core.CorsConstants.AccessControlAllowOrigin);

            app.UseMvc();
        }
    }
}
