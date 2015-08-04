// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.DependencyInjection;

namespace EntityFramework7.Npgsql
{
    public class NpgsqlOptionsExtension : RelationalOptionsExtension
    {
        public NpgsqlOptionsExtension()
        {
        }

        public NpgsqlOptionsExtension([NotNull] NpgsqlOptionsExtension copyFrom)
            : base(copyFrom)
        {
        }

        public override void ApplyServices(EntityFrameworkServicesBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.AddNpgsql();
        }
    }
}
