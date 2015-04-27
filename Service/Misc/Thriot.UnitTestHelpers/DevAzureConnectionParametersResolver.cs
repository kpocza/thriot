using System;
using Thriot.Framework.DataAccess;

namespace Thriot.TestHelpers
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