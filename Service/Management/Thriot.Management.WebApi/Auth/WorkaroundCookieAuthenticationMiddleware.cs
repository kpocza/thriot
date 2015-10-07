using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.DataProtection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.WebEncoders;

namespace Thriot.Management.WebApi.Auth
{
    public class WorkaroundCookieAuthenticationMiddleware : CookieAuthenticationMiddleware
    {
        public WorkaroundCookieAuthenticationMiddleware(RequestDelegate next, IDataProtectionProvider dataProtectionProvider, ILoggerFactory loggerFactory, IUrlEncoder urlEncoder, IOptions<CookieAuthenticationOptions> options, ConfigureOptions<CookieAuthenticationOptions> configureOptions) : 
            base(next, dataProtectionProvider, loggerFactory, urlEncoder, options, configureOptions)
        {
            ((CookieAuthenticationNotifications) Options.Notifications).OnApplyRedirect = context =>
            {
                context.Response.StatusCode = 401;
            };
        }
    }
}
