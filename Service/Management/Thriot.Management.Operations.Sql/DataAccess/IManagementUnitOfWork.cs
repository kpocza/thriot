﻿using Thriot.Framework.Sql;

namespace Thriot.Management.Operations.Sql.DataAccess
{
    public interface IManagementUnitOfWork : IUnitOfWork
    {
        void ExecuteScript(string script);

        DeviceRepository GetDeviceRepository();

        NetworkRepository GetNetworkRepository();

        ServiceRepository GetServiceRepository();

        CompanyRepository GetCompanyRepository();

        UserRepository GetUserRepository();

        LoginUserRepository GetLoginUserRepository();

        SettingRepository GetSettingRepository();

        UserCompanyRepository GetUserCompanyRepository();
    }
}
