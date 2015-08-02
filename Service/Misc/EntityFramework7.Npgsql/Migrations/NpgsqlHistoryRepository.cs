﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Migrations.History;
using Microsoft.Data.Entity.Migrations.Operations;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Migrations
{
    // TODO: Log
    public class NpgsqlHistoryRepository : IHistoryRepository
    {
        private readonly NpgsqlDatabaseConnection  _connection;
        private readonly IRelationalDatabaseCreator _creator;
        private readonly Type _contextType;
        private readonly NpgsqlUpdateSqlGenerator _sql;

        public NpgsqlHistoryRepository(
            [NotNull] NpgsqlDatabaseConnection connection,
            [NotNull] IRelationalDatabaseCreator creator,
            [NotNull] DbContext context,
            [NotNull] NpgsqlUpdateSqlGenerator sqlGenerator)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(creator, nameof(creator));
            Check.NotNull(context, nameof(context));
            Check.NotNull(sqlGenerator, nameof(sqlGenerator));

            _connection = connection;
            _creator = creator;
            _contextType = context.GetType();
            _sql = sqlGenerator;
        }

        public virtual bool Exists()
        {
            var exists = false;

            if (!_creator.Exists())
            {
                return exists;
            }

            var command = (SqlCommand)_connection.DbConnection.CreateCommand();
            command.CommandText =
                @"SELECT 1 FROM [INFORMATION_SCHEMA].[TABLES]
WHERE [TABLE_SCHEMA] = N'dbo' AND [TABLE_NAME] = '__MigrationHistory' AND [TABLE_TYPE] = 'BASE TABLE'";

            _connection.Open();
            try
            {
                exists = command.ExecuteScalar() != null;
            }
            finally
            {
                _connection.Close();
            }

            return exists;
        }

        public virtual IReadOnlyList<IHistoryRow> GetAppliedMigrations()
        {
            var rows = new List<HistoryRow>();

            if (!Exists())
            {
                return rows;
            }

            _connection.Open();
            try
            {
                var command = (SqlCommand)_connection.DbConnection.CreateCommand();
                command.CommandText =
                    @"SELECT [MigrationId], [ProductVersion]
FROM [dbo].[__MigrationHistory]
WHERE [ContextKey] = @ContextKey ORDER BY [MigrationId]";
                command.Parameters.AddWithValue("@ContextKey", _contextType.FullName);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(new HistoryRow(reader.GetString(0), reader.GetString(1)));
                    }
                }
            }
            finally
            {
                _connection.Close();
            }

            return rows;
        }

        public virtual string Create(bool ifNotExists)
        {
            var builder = new IndentedStringBuilder();

            if (ifNotExists)
            {
                builder.AppendLine("IF NOT EXISTS(SELECT * FROM [INFORMATION_SCHEMA].[TABLES] WHERE[TABLE_SCHEMA] = N'dbo' AND[TABLE_NAME] = '__MigrationHistory' AND[TABLE_TYPE] = 'BASE TABLE')");
                builder.IncrementIndent();
            }

            builder
                .AppendLine("CREATE TABLE [dbo].[__MigrationHistory] (");
            using (builder.Indent())
            {
                builder
                    .AppendLine("[MigrationId] nvarchar(150) NOT NULL,")
                    .AppendLine("[ContextKey] nvarchar(300) NOT NULL,")
                    .AppendLine("[ProductVersion] nvarchar(32) NOT NULL,")
                    .AppendLine("CONSTRAINT [PK_MigrationHistory] PRIMARY KEY ([MigrationId], [ContextKey])");
            }
            builder.Append(");");

            return builder.ToString();
        }

        public virtual MigrationOperation GetDeleteOperation(string migrationId)
        {
            Check.NotEmpty(migrationId, nameof(migrationId));

            return new SqlOperation
            {
                Sql = new StringBuilder()
                    .AppendLine("DELETE FROM [dbo].[__MigrationHistory]")
                    .Append("WHERE [MigrationId] = '").Append(_sql.EscapeLiteral(migrationId))
                        .Append("' AND [ContextKey] = '").Append(_sql.EscapeLiteral(_contextType.FullName))
                        .AppendLine("';")
                    .ToString()
            };
        }

        public virtual MigrationOperation GetInsertOperation(IHistoryRow row)
        {
            Check.NotNull(row, nameof(row));

            return new SqlOperation
            {
                Sql = new StringBuilder()
                    .AppendLine("INSERT INTO [dbo].[__MigrationHistory] ([MigrationId], [ContextKey], [ProductVersion])")
                    .Append("VALUES ('").Append(_sql.EscapeLiteral(row.MigrationId)).Append("', '")
                        .Append(_sql.EscapeLiteral(_contextType.FullName)).Append("', '")
                        .Append(_sql.EscapeLiteral(row.ProductVersion)).AppendLine("');")
                    .ToString()
            };
        }

        public virtual string BeginIfNotExists(string migrationId)
        {
            Check.NotEmpty(migrationId, nameof(migrationId));

            return new StringBuilder()
                .Append("IF NOT EXISTS(SELECT * FROM [dbo].[__MigrationHistory] WHERE [MigrationId] = '")
                    .Append(_sql.EscapeLiteral(migrationId)).Append("' AND [ContextKey] = '")
                    .Append(_sql.EscapeLiteral(_contextType.FullName)).AppendLine("')")
                .Append("BEGIN")
                .ToString();
        }

        public virtual string BeginIfExists(string migrationId)
        {
            Check.NotEmpty(migrationId, nameof(migrationId));

            return new StringBuilder()
                .Append("IF EXISTS(SELECT * FROM [dbo].[__MigrationHistory] WHERE [MigrationId] = '")
                    .Append(_sql.EscapeLiteral(migrationId)).Append("' AND [ContextKey] = '")
                    .Append(_sql.EscapeLiteral(_contextType.FullName)).AppendLine("')")
                .Append("BEGIN")
                .ToString();
        }

        public virtual string EndIf()
        {
            return "END";
        }
    }
}
