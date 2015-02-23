using IoT.ServiceClient.Messaging;
using MgmtOp = IoT.Management.Model.Operations;
using ObjOp = IoT.Objects.Model.Operations;

namespace IoT.UnitTestHelpers
{
    public interface IEnvironmentFactory
    {
        MgmtOp.IUserOperations MgmtUserOperations { get; }

        MgmtOp.ICompanyOperations MgmtCompanyOperations { get; }

        MgmtOp.IServiceOperations MgmtServiceOperations { get; }

        MgmtOp.INetworkOperations MgmtNetworkOperations { get; }

        MgmtOp.IDeviceOperations MgmtDeviceOperations { get; }

        MgmtOp.ISettingOperations MgmtSettingOperations { get; }

        ObjOp.ICompanyOperations ObjCompanyOperations { get; }

        ObjOp.IServiceOperations ObjServiceOperations { get; }

        ObjOp.INetworkOperations ObjNetworkOperations { get; }

        ObjOp.IDeviceOperations ObjDeviceOperations { get; }

        ObjOp.ISettingOperations ObjSettingOperations { get; }

        IMessagingService MessagingService { get; }
    }
}
