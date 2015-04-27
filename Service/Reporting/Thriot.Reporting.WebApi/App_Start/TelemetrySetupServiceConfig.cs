using Thriot.Framework;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.ServiceClient.TelemetrySetup;

namespace Thriot.Reporting.WebApi
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