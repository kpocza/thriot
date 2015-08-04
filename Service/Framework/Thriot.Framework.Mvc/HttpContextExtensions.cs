
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;

namespace Thriot.Framework.Mvc
{
    public static class HttpContextExtensions
    {
        public static string GetClientIpAddress(this HttpContext request)
        {
            try
            {
                var remoteIpAddress = request.GetFeature<IHttpConnectionFeature>()?.RemoteIpAddress;

                if (remoteIpAddress == null)
                    return "Unknown";

                return remoteIpAddress.ToString();
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}