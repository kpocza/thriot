using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;
using Thriot.Framework.Mvc.ApiExceptions;
using Thriot.Framework.Mvc.Logging;
using System.Linq;

namespace Thriot.Management.WebApiA5
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;
            Services.DtoMapper.Setup();
            Framework.Mails.MailTemplateStore.Instance.Add(GetTemplate("Activation"));
            Framework.Mails.MailTemplateStore.Instance.Add(GetTemplate("ResetPassword"));

            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.UseNagleAlgorithm = false;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new Configuration();
            configuration.AddJsonFile("services.json");
            configuration.AddJsonFile("connectionstring.json");

            services.AddMvc().Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new LogActionsAttribute());
                options.Filters.Add(new ApiExceptionFilterAttribute());
            });

            services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.Mvc.ConnectionParametersResolver>();
            services.AddScoped<Services.IAuthenticationContext, WebApi.Auth.WebAuthenticationContext>();
            services.AddSingleton<Services.ISettingProvider, Services.SettingProvider>();
            services.AddTransient<Services.ICapabilityProvider, Services.CapabilityProvider>();
            services.AddTransient<Services.IEnvironmentPrebuilder, Services.EnvironmentPrebuilder>();
            services.AddTransient<Services.IMailer, WebApi.WebFunctions.Mailer>();
            services.AddSingleton<IConfiguration>(_ => configuration);

            services.AddSingleton<ServiceClient.TelemetrySetup.ITelemetryDataSinkSetupService, ServiceClient.TelemetrySetup.TelemetryDataSinkSetupService>();
            services.AddSingleton<ServiceClient.Messaging.IMessagingService, ServiceClient.Messaging.MessagingService>();

            foreach(var extraService in configuration.GetSubKeys("Services"))
            {
                var intf = extraService.Key;
                var impl = configuration.Get("Services:" + intf);

                services.AddTransient(System.Type.GetType(intf), System.Type.GetType(impl));
            }

            var serviceProvider = services.BuildServiceProvider();

            var messagingService = serviceProvider.GetService<ServiceClient.Messaging.IMessagingService>();
            var telemetryDataSinkSetupService = serviceProvider.GetService<ServiceClient.TelemetrySetup.ITelemetryDataSinkSetupService>();

            var settingProvider = (Services.SettingProvider)serviceProvider.GetService<Services.ISettingProvider>();

            messagingService.Setup(settingProvider.MessagingServiceEndpoint, settingProvider.MessagingServiceApiKey);
            telemetryDataSinkSetupService.Setup(settingProvider.TelemetrySetupServiceEndpoint, settingProvider.TelemetrySetupServiceApiKey);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
            
            app.UseCors(Microsoft.AspNet.Cors.Core.CorsConstants.AccessControlAllowOrigin);

            app.UseCookieAuthentication(options => 
            {
                options.CookieHttpOnly = false;
                options.ExpireTimeSpan = System.TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.CookieName = "ThriotMgmtAuth";
            });
        }

        private Framework.Mails.MailTemplate GetTemplate(string name)
        {
            return Framework.Mails.MailTemplate.Create(name, LoadContent(name, "subject"), LoadContent(name, "txt"), LoadContent(name, "html"));
        }

        private string LoadContent(string name, string extension)
        {
            var mailTemplatesPath = System.IO.Path.Combine(_env.WebRootPath, "MailTemplates");

            var pathToRead = System.IO.Path.Combine(mailTemplatesPath, name + "." + extension);
            
            return System.IO.File.ReadAllText(pathToRead);
        }

    }
}
