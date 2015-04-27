using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Thriot.Framework.Azure.TableOperations;
using Thriot.Framework.DataAccess;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Azure
{
    public abstract class TelemetryDataSinkBase : ITelemetryDataSink
    {
        protected const string ConnectionStringString = "ConnectionString";
        protected const string TableString = "Table";
        private const string ConnectionNameString = "ConnectionName";
        
        private readonly IDynamicConnectionStringResolver _dynamicConnectionStringResolver;
        protected ITableEntityOperation TableEntityOperation;
        protected string TableName;

        protected TelemetryDataSinkBase()
        {
        }

        protected TelemetryDataSinkBase(IDynamicConnectionStringResolver dynamicConnectionStringResolver)
        {
            _dynamicConnectionStringResolver = dynamicConnectionStringResolver;
        }

        public void Setup(IDictionary<string, string> parameters)
        {
            string connectionString = null;
            if (!parameters.TryGetValue(ConnectionStringString, out connectionString))
            {
                connectionString =
                    _dynamicConnectionStringResolver.Resolve(parameters[ConnectionNameString]).ConnectionString;
            }

            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            
            TableEntityOperation = new AzureTableEntityOperations(cloudTableClient);
            TableName = parameters[TableString];

            if (!TableName.All(char.IsLetterOrDigit))
                throw new ArgumentException("Invalid table name");
        }

        public void Initialize()
        {
            TableEntityOperation.EnsureTable(TableName);
        }

        public abstract void Record(TelemetryData telemetryData);

        public IReadOnlyCollection<string> ParametersNames
        {
            get { return new[] { ConnectionStringString, TableString }; }
        }

        public IDictionary<string, string> ParameterSubstitutes
        {
            get { return new Dictionary<string, string> { { "ConnectionName", "ConnectionString" }}; }
        }
    }
}