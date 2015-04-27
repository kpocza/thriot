using Thriot.Framework;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.ServiceClient.Messaging;

namespace Thriot.Platform.WebApi
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
