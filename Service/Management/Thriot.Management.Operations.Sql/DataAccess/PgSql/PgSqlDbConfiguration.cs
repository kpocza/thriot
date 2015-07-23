using System.Configuration;
using System.Data.Entity;
using Npgsql;

namespace Thriot.Management.Operations.Sql.DataAccess.PgSql
{
    public class PgSqlDbConfiguration : DbConfiguration
    {
        static PgSqlDbConfiguration()
        {
            var configSection =
                "<system.data>" +
                "<DbProviderFactories>" +
                "<remove invariant = \"Npgsql\" />" +
                "<add name = \"Npgsql Data Provider\" invariant = \"Npgsql\" support = \"FF\" description = \".Net Framework Data Provider for Postgresql\" type = \"Npgsql.NpgsqlFactory, Npgsql\" />" +
                "</DbProviderFactories>" +
                "</system.data>";

            var dataSet = ConfigurationManager.GetSection("system.data") as System.Data.DataSet;

            dataSet.Tables[0].Rows.Add(configSection);
        }

        public PgSqlDbConfiguration()
        {
            SetProviderServices("Npgsql", new NpgsqlServices());
            SetDefaultConnectionFactory(new NpgsqlConnectionFactory());
        }
    }
}
