﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Metadata
{
    public interface INpgsqlPropertyAnnotations : IRelationalPropertyAnnotations
    {
        string SequenceName { get; }
        string SequenceSchema { get; }
        Sequence TryGetSequence();
    }
}
