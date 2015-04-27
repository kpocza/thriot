using Thriot.Framework;
using Thriot.Management.Services;
using Thriot.ServiceClient.TelemetrySetup;

namespace Thriot.Management.WebApi
{
    public class TelemetrySetupServiceConfig
    {
        public static void Register()
        {
            var telemetryDataSinkSetupService = SingleContainer.Instance.Resolve<ITelemetryDataSinkSetupService>();

            var settingProvider = SingleContainer.Instance.Resolve<SettingProvider>();

            telemetryDataSinkSetupService.Setup(settingProvider.TelemetrySetupServiceEndpoint, settingProvider.TelemetrySetupServiceApiKey);
        }
    }
}