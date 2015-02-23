using IoT.Framework.Sql;

namespace IoT.Objects.Operations.Sql.DataAccess
{
    public interface IObjectsUnitOfWork : IUnitOfWork
    {
        CompanyRepository GetCompanyRepository();

        ServiceRepository GetServiceRepository();

        NetworkRepository GetNetworkRepository();

        DeviceRepository GetDeviceRepository();

        SettingRepository GetSettingRepository();
    }
}
