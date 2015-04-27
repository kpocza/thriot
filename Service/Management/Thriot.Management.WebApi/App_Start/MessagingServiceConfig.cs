using Thriot.Framework;
using Thriot.Management.Services;
using Thriot.ServiceClient.Messaging;

namespace Thriot.Management.WebApi
{
    public class MessagingServiceConfig
    {
        public static void Register()
        {
            var messagingService = SingleContainer.Instance.Resolve<IMessagingService>();

            var settingProvider = SingleContainer.Instance.Resolve<SettingProvider>();

            messagingService.Setup(settingProvider.MessagingServiceEndpoint, settingProvider.MessagingServiceApiKey);
        }
    }
}
