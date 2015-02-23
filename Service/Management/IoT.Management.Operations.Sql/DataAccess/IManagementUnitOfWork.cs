using IoT.Framework.Sql;

namespace IoT.Management.Operations.Sql.DataAccess
{
    public interface IManagementUnitOfWork : IUnitOfWork
    {
        DeviceRepository GetDeviceRepository();

        NetworkRepository GetNetworkRepository();

        ServiceRepository GetServiceRepository();

        CompanyRepository GetCompanyRepository();

        UserRepository GetUserRepository();

        LoginUserRepository GetLoginUserRepository();

        SettingRepository GetSettingRepository();
    }
}
