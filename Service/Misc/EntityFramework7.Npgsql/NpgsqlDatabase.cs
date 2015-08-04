// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using EntityFramework7.Npgsql.Query;
using JetBrains.Annotations;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Query.Methods;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Update;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.Logging;

namespace EntityFramework7.Npgsql
{
    public class NpgsqlDatabase : RelationalDatabase
    {
        public NpgsqlDatabase(
            [NotNull] IModel model,
            [NotNull] IEntityKeyFactorySource entityKeyFactorySource,
            [NotNull] IEntityMaterializerSource entityMaterializerSource,
            [NotNull] IClrAccessorSource<IClrPropertyGetter> clrPropertyGetterSource,
            [NotNull] NpgsqlDatabaseConnection connection,
            [NotNull] ICommandBatchPreparer batchPreparer,
            [NotNull] IBatchExecutor batchExecutor,
            [NotNull] IDbContextOptions options,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IRelationalValueBufferFactoryFactory valueBufferFactoryFactory,
            [NotNull] IMethodCallTranslator compositeMethodCallTranslator,
            [NotNull] IMemberTranslator compositeMemberTranslator,
            [NotNull] IRelationalTypeMapper typeMapper)
            : base(
                model,
                entityKeyFactorySource,
                entityMaterializerSource,
                clrPropertyGetterSource,
                connection,
                batchPreparer,
                batchExecutor,
                options,
                loggerFactory,
                valueBufferFactoryFactory,
                compositeMethodCallTranslator,
                compositeMemberTranslator,
                typeMapper)
        {
        }

        protected override RelationalQueryCompilationContext CreateQueryCompilationContext(
            ILinqOperatorProvider linqOperatorProvider,
            IResultOperatorHandler resultOperatorHandler,
            IQueryMethodProvider enumerableMethodProvider,
            IMethodCallTranslator compositeMethodCallTranslator,
            IMemberTranslator compositeMemberTranslator)
        {
            Check.NotNull(linqOperatorProvider, nameof(linqOperatorProvider));
            Check.NotNull(resultOperatorHandler, nameof(resultOperatorHandler));
            Check.NotNull(enumerableMethodProvider, nameof(enumerableMethodProvider));
            Check.NotNull(compositeMethodCallTranslator, nameof(compositeMethodCallTranslator));
            Check.NotNull(compositeMemberTranslator, nameof(compositeMemberTranslator));

            return new NpgsqlQueryCompilationContext(
                Model,
                Logger,
                linqOperatorProvider,
                resultOperatorHandler,
                EntityMaterializerSource,
                EntityKeyFactorySource,
                ClrPropertyGetterSource,
                enumerableMethodProvider,
                compositeMethodCallTranslator,
                compositeMemberTranslator,
                ValueBufferFactoryFactory,
                TypeMapper);
        }
    }
}
