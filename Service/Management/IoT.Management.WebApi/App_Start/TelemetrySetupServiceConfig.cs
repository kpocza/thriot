using IoT.Framework;
using IoT.Management.Services;
using IoT.ServiceClient.TelemetrySetup;

namespace IoT.Management.WebApi
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