using Thriot.Management.Model.Operations;
using Thriot.Plugins.Core;
using Thriot.Messaging.Services.Client;
using MgmtOp = Thriot.Management.Model.Operations;
using ObjOp = Thriot.Objects.Model.Operations;

namespace Thriot.TestHelpers
{
    public interface IEnvironmentFactory
    {
        IUserOperations MgmtUserOperations { get; }

        ICompanyOperations MgmtCompanyOperations { get; }

        IServiceOperations MgmtServiceOperations { get; }

        INetworkOperations MgmtNetworkOperations { get; }

        IDeviceOperations MgmtDeviceOperations { get; }

        ISettingOperations MgmtSettingOperations { get; }

        Objects.Model.Operations.ICompanyOperations ObjCompanyOperations { get; }

        Objects.Model.Operations.IServiceOperations ObjServiceOperations { get; }

        Objects.Model.Operations.INetworkOperations ObjNetworkOperations { get; }

        Objects.Model.Operations.IDeviceOperations ObjDeviceOperations { get; }

        Objects.Model.Operations.ISettingOperations ObjSettingOperations { get; }

        IMessagingServiceClient MessagingServiceClient { get; }

        string TelemetryConnectionString { get; }

        ITelemetryDataSinkCurrent TelemetryDataSinkCurrent { get; }

        ITelemetryDataSinkTimeSeries TelemetryDataSinkTimeSeries { get; }
    }
}
