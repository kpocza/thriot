using IoT.Framework;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.ServiceClient.Messaging;

namespace IoT.Platform.WebApi
{
    public class MessagingServiceConfig
    {
        public static IMessagingService Register()
        {
            var messagingService = SingleContainer.Instance.Resolve<IMessagingService>();
            var settingOperations = SingleContainer.Instance.Resolve<ISettingOperations>();

            messagingService.Setup(settingOperations.Get(Setting.MessagingServiceEndpoint).Value, settingOperations.Get(Setting.MessagingServiceApiKey).Value);

            return messagingService;
        }
    }
}
