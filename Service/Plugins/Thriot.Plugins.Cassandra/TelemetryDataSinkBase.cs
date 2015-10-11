using Cassandra;
using System;
using System.Collections.Generic;
using Thriot.Framework.DataAccess;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Cassandra
{
    public abstract class TelemetryDataSinkBase : ITelemetryDataSink
    {
        protected const string ContactPointsString = "ContactPoints";
        protected const string KeyspaceString = "Keyspace";
        protected const string TableString = "Table";
        private const string ContactPointNameString = "ContactPointName";

        protected string KeyspaceName;
        protected string TableName;

        protected ISession _session;

        public void Setup(IDynamicConnectionStringResolver dynamicConnectionStringResolver, IDictionary<string, string> parameters)
        {
            string connectionPoints = null;
            if (!parameters.TryGetValue(ContactPointsString, out connectionPoints))
            {
                connectionPoints =
                    dynamicConnectionStringResolver.Resolve(parameters[ContactPointNameString]).ConnectionString;
            }

            var connectionPointList = connectionPoints.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            KeyspaceName = parameters[KeyspaceString];
            TableName = parameters[TableString];

            var cluster = Cluster.Builder().AddContactPoints(connectionPointList).Build();
            _session = cluster.Connect(KeyspaceName);
        }

        public void Initialize()
        {
            _session.CreateKeyspaceIfNotExists(KeyspaceName);
            CreateTable();
        }
        protected abstract void CreateTable();

        public abstract void Record(TelemetryData message);

        public IReadOnlyCollection<string> ParametersNames
        {
            get
            {
                return new[] { ContactPointsString, KeyspaceString, TableString };
            }
        }

        public IDictionary<string, string> ParameterSubstitutes
        {
            get { return new Dictionary<string, string> { { ContactPointNameString, ContactPointsString } }; }
        }
    }
}
