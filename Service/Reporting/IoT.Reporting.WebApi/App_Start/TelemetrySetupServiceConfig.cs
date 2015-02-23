using IoT.Framework;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.ServiceClient.TelemetrySetup;

namespace IoT.Reporting.WebApi
{
    public class TelemetrySetupServiceConfig
    {
        public static void Register()
        {
            var telemetryDataSinkSetupService = SingleContainer.Instance.Resolve<ITelemetryDataSinkSetupService>();

            var settingOperations = SingleContainer.Instance.Resolve<ISettingOperations>();

            telemetryDataSinkSetupService.Setup(settingOperations.Get(Setting.TelemetrySetupServiceEndpoint).Value, settingOperations.Get(Setting.TelemetrySetupServiceApiKey).Value);
        }
    }
}