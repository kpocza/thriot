using System;
using System.Collections.Generic;
using System.Linq;
using IoT.Framework.DataAccess;
using IoT.Plugins.Core;
using Npgsql;

namespace IoT.Plugins.PgSql
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
            using (var sqlConnection = new NpgsqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new NpgsqlCommand(CreateTableStatement, sqlConnection))
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