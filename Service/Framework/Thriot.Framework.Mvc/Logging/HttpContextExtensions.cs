
using Microsoft.AspNet.Http;

namespace Thriot.Framework.Mvc.Logging
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