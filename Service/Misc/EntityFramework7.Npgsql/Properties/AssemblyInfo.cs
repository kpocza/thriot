// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using System.Resources;
using System;
using System.Security;

[assembly: AssemblyTitle("EntityFramework7.Npgsql")]
[assembly: AssemblyDescription("PostgreSQL provider for Entity Framework 7")]



// Contains assembly attributes shared by all Npgsql projects

[assembly: CLSCompliant(false)]
[assembly: AllowPartiallyTrustedCallers()]
[assembly: SecurityRules(SecurityRuleSet.Level1)]
[assembly: AssemblyCompany("Npgsql Development Team")]
[assembly: AssemblyProduct("Npgsql")]
[assembly: AssemblyCopyright("Copyright © 2002 - 2014 Npgsql Development Team")]
[assembly: AssemblyTrademark("")]
[assembly: NeutralResourcesLanguage("en", UltimateResourceFallbackLocation.MainAssembly)]

// The following version attributes get rewritten by GitVersion as part of the build
[assembly: AssemblyVersion("3.1.0.0")]
[assembly: AssemblyFileVersion("3.1.0.0")]
[assembly: AssemblyInformationalVersion("3.1.0-beta7-1+Branch.develop.Sha.a017bfc066ce372041992d892423c34089090402")]