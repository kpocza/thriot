using System;
using IoT.Framework.DataAccess;

namespace IoT.UnitTestHelpers
{
    public class DevPgSqlConnectionParametersResolver : IConnectionParametersResolver
    {
        public string ManagementConnectionName
        {
            get { throw new NotImplementedException();}
        }

        public string ManagementConnectionString
        {
            get { return @"Server=127.0.0.1;Port=5432;Database=Thriot;User Id=thriot;Password=thriot;"; }
        }

        public string ManagementConnectionProvider
        {
            get { return "Npgsql"; }
        }
    }
}