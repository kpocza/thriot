using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Thriot.Framework.DataAccess;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Sql
{
    public abstract class TelemetryDataSinkBase : ITelemetryDataSink
    {
        protected const string ConnectionStringString = "ConnectionString";
        protected const string TableString = "Table";
        private const string ConnectionNameString = "ConnectionName";
        
        private readonly IDynamicConnectionStringResolver _dynamicConnectionStringResolver;
        protected string ConnectionString;
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
            ConnectionString = null;
            if (!parameters.TryGetValue(ConnectionStringString, out ConnectionString))
            {
                ConnectionString =
                    _dynamicConnectionStringResolver.Resolve(parameters[ConnectionNameString]).ConnectionString;
            }

            TableName = parameters[TableString];

            if (!TableName.All(char.IsLetterOrDigit))
                throw new ArgumentException("Invalid table name");
        }

        public void Initialize()
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                string createTableIfNotExists =
                    string.Join(Environment.NewLine,
                        string.Format("IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '{0}')", TableName),
                        CreateTableStatement);

                using (var sqlCommand = new SqlCommand(createTableIfNotExists, sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public abstract void Record(TelemetryData telemetryData);

        protected abstract string CreateTableStatement { get; }

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