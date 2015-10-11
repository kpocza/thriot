using MgmtOp = Thriot.Management.Model.Operations;
using ObjOp = Thriot.Objects.Model.Operations;

namespace Thriot.TestHelpers
{
    public interface IManagementEnvironment
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
    }
}
