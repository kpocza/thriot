using System;
using IoT.Framework.DataAccess;

namespace IoT.UnitTestHelpers
{
    public class DevAzureConnectionParametersResolver : IConnectionParametersResolver
    {
        public string ManagementConnectionName
        {
            get { throw new NotImplementedException(); }
        }

        public string ManagementConnectionString
        {
            get { return "UseDevelopmentStorage=true"; }
        }

        public string ManagementConnectionProvider
        {
            get { throw new NotImplementedException(); }
        }
    }
}