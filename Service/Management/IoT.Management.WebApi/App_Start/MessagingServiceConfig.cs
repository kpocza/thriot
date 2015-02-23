using IoT.Framework;
using IoT.Management.Services;
using IoT.ServiceClient.Messaging;

namespace IoT.Management.WebApi
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
