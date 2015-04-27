using System;
using Thriot.Framework.DataAccess;

namespace Thriot.TestHelpers
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