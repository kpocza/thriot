using System;
using IoT.Framework.DataAccess;

namespace IoT.UnitTestHelpers
{
    public class DevSqlConnectionParametersResolver : IConnectionParametersResolver
    {
        public string ManagementConnectionName
        {
            get { throw new NotImplementedException();}
        }

        public string ManagementConnectionString
        {
            get { return @"Server=.\SQLEXPRESS;Database=Thriot;Trusted_Connection=True;"; }
        }

        public string ManagementConnectionProvider
        {
            get { return "System.Data.SqlClient"; }
        }
    }
}