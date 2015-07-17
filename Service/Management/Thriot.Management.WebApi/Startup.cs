﻿using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using System.Net;
using Thriot.Framework.Mvc.ApiExceptions;
using Thriot.Framework.Mvc.Logging;
using Thriot.Management.Model.Exceptions;

namespace Thriot.Management.WebApiA5
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly IApplicationEnvironment _appEnv;

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            _env = env;
            _appEnv = appEnv;

            Services.DtoMapper.Setup();
            Framework.Mails.MailTemplateStore.Instance.Add(GetTemplate("Activation"));
            Framework.Mails.MailTemplateStore.Instance.Add(GetTemplate("ResetPassword"));

            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            ApiExceptionRegistry.AddItem(typeof(ActivationRequiredException), HttpStatusCode.Forbidden);
            ApiExceptionRegistry.AddItem(typeof(ActivationException), HttpStatusCode.Forbidden);
            ApiExceptionRegistry.AddItem(typeof(ConfirmationException), HttpStatusCode.Forbidden);
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

            services.AddTransient<Services.UserService>();
            services.AddTransient<Services.CompanyService>();
            services.AddTransient<Services.ServiceService>();
            services.AddTransient<Services.NetworkService>();
            services.AddTransient<Services.DeviceService>();
            services.AddTransient<Services.InfoService>();
            services.AddTransient<Services.TelemetryMetadataService>();
            services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.Mvc.ConnectionParametersResolver>();
            services.AddScoped<Services.IAuthenticationContext, WebApi.Auth.WebAuthenticationContext>();
            services.AddSingleton<Services.ISettingProvider, Services.SettingProvider>();
            services.AddTransient<Services.ICapabilityProvider, Services.CapabilityProvider>();
            services.AddTransient<Services.IEnvironmentPrebuilder, Services.EnvironmentPrebuilder>();
            services.AddTransient<Services.IMailer, WebApi.WebFunctions.Mailer>();
            services.AddSingleton<IConfiguration>(_ => configuration);

            services.AddSingleton<ServiceClient.TelemetrySetup.ITelemetryDataSinkSetupService, ServiceClient.TelemetrySetup.TelemetryDataSinkSetupService>();
            services.AddSingleton<ServiceClient.Messaging.IMessagingService, ServiceClient.Messaging.MessagingService>();
            foreach(var extraService in configuration.GetConfigurationSection("Services").GetConfigurationSections())
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
            app.UseCors(Microsoft.AspNet.Cors.Core.CorsConstants.AccessControlAllowOrigin);

            app.UseCookieAuthentication(options => 
            {
                options.CookieHttpOnly = false;
                options.ExpireTimeSpan = System.TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.CookieName = "ThriotMgmtAuth";
                options.AutomaticAuthentication = true;
            });

            app.UseMvc();
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
