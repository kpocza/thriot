// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Metadata
{
    public class NpgsqlEntityTypeBuilder
    {
        private readonly EntityType _entityType;

        public NpgsqlEntityTypeBuilder([NotNull] EntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            _entityType = entityType;
        }

        public virtual NpgsqlEntityTypeBuilder Table([CanBeNull] string tableName)
        {
            Check.NullButNotEmpty(tableName, nameof(tableName));

            _entityType.Npgsql().Table = tableName;

            return this;
        }

        public virtual NpgsqlEntityTypeBuilder Table([CanBeNull] string tableName, [CanBeNull] string schemaName)
        {
            Check.NullButNotEmpty(tableName, nameof(tableName));
            Check.NullButNotEmpty(schemaName, nameof(schemaName));

            _entityType.Npgsql().Table = tableName;
            _entityType.Npgsql().Schema = schemaName;

            return this;
        }
    }
}
