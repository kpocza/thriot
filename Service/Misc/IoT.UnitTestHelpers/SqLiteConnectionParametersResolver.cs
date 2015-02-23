using System;
using IoT.Framework.DataAccess;

namespace IoT.UnitTestHelpers
{
    public class SqLiteConnectionParametersResolver : IConnectionParametersResolver
    {
        public string ManagementConnectionName
        {
            get { throw new NotImplementedException(); }
        }


        public string ManagementConnectionString
        {
            get { return "Data Source=tmp.db;Version=3;New=True;"; }
        }

        public string ManagementConnectionProvider
        {
            get { return "System.Data.SQLite"; }
        }
    }
}