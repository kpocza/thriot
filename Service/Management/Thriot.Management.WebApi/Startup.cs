using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.OptionsModel;
using Thriot.Framework;
using Thriot.Framework.Mvc.ApiExceptions;
using Thriot.Framework.Mvc.Logging;
using Thriot.Management.Model.Exceptions;

namespace Thriot.Management.WebApi
{
    public class Startup
    {
        private readonly IApplicationEnvironment _appEnv;

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
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

            Framework.Logging.NLogLogger.SetConfiguration(
                System.IO.Path.Combine(System.IO.Path.Combine(appEnv.ApplicationBasePath, "config"), "web.nlog"));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(_appEnv.ApplicationBasePath);
            configurationBuilder.AddJsonFile("config/services.json");
            configurationBuilder.AddJsonFile("config/connectionstring.json");
            configurationBuilder.AddJsonFile("config/smtpsettings.json");
            
            var configuration = configurationBuilder.Build();

            services.AddMvc();
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new LogActionsAttribute());
                options.Filters.Add(new ApiExceptionFilterAttribute());
            });

            services.AddCors(
                c =>
                    c.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

            ConfigureThriotServices(services, configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var serviceProvider = app.ApplicationServices;

            var messagingServiceClient = serviceProvider.GetService<Messaging.Services.Client.IMessagingServiceClient>();
            var telemetryDataSinkSetupServiceClient = serviceProvider.GetService<Platform.Services.Client.ITelemetryDataSinkSetupServiceClient>();

            var settingProvider = (Services.SettingProvider)serviceProvider.GetService<Services.ISettingProvider>();

            messagingServiceClient.Setup(settingProvider.MessagingServiceEndpoint, settingProvider.MessagingServiceApiKey);
            telemetryDataSinkSetupServiceClient.Setup(settingProvider.TelemetrySetupServiceEndpoint, settingProvider.TelemetrySetupServiceApiKey);

            app.UseIISPlatformHandler();

            app.UseCookieAuthentication(options =>
            {
                options.CookieHttpOnly = false;
                options.ExpireTimeSpan = System.TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.CookieName = "ThriotMgmtAuth";
                options.AutomaticAuthentication = true;
                ((CookieAuthenticationEvents)options.Events).OnRedirect = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.FromResult(0);
                };
            });

            app.UseCors("AllowAll");
            app.UseMvc();
        }

        private void ConfigureThriotServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<Services.UserService>();
            services.AddTransient<Services.CompanyService>();
            services.AddTransient<Services.ServiceService>();
            services.AddTransient<Services.NetworkService>();
            services.AddTransient<Services.DeviceService>();
            services.AddTransient<Services.InfoService>();
            services.AddTransient<Services.TelemetryMetadataService>();
            services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.DataAccess.ConnectionParametersResolver>();
            services.AddScoped<Services.IAuthenticationContext, WebApi.Auth.WebAuthenticationContext>();
            services.AddSingleton<Services.ISettingProvider, Services.SettingProvider>();
            services.AddTransient<Services.ICapabilityProvider, Services.CapabilityProvider>();
            services.AddTransient<Services.IEnvironmentPrebuilder, Services.EnvironmentPrebuilder>();
            services.AddTransient<Services.IMailer, WebApi.WebFunctions.Mailer>();
            services.AddSingleton(_ => configuration);

            services.AddSingleton<Platform.Services.Client.ITelemetryDataSinkSetupServiceClient, Platform.Services.Client.TelemetryDataSinkSetupServiceClient>();
            services.AddSingleton<Messaging.Services.Client.IMessagingServiceClient, Messaging.Services.Client.MessagingServiceClient>();

            foreach (var extraService in configuration.AsTypeMap("Services"))
            {
                services.AddTransient(extraService.Key, extraService.Value);
            }
        }

        private Framework.Mails.MailTemplate GetTemplate(string name)
        {
            return Framework.Mails.MailTemplate.Create(name, LoadContent(name, "subject"), LoadContent(name, "txt"), LoadContent(name, "html"));
        }

        private string LoadContent(string name, string extension)
        {
            var mailTemplatesPath = System.IO.Path.Combine(_appEnv.ApplicationBasePath, "MailTemplates");
            var pathToRead = System.IO.Path.Combine(mailTemplatesPath, name + "." + extension);
            
            return System.IO.File.ReadAllText(pathToRead);
        }
    }
}
