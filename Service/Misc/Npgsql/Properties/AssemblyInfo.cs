using System;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Resources;
using System.Security;

// Additional assembly attributes are defined in GlobalAssemblyInfo.cs

[assembly: AssemblyTitle("Npgsql - .Net Data Provider for PostgreSQL")]
[assembly: AssemblyDescription(".Net Data Provider for PostgreSQL")]

[assembly: InternalsVisibleTo("EntityFramework7.Npgsql")]


// Contains assembly attributes shared by all Npgsql projects

[assembly: CLSCompliant(false)]
[assembly: AllowPartiallyTrustedCallers()]
[assembly: SecurityRules(SecurityRuleSet.Level1)]
[assembly: AssemblyCompany("Npgsql Development Team")]
[assembly: AssemblyProduct("Npgsql")]
[assembly: AssemblyCopyright("Copyright © 2002 - 2014 Npgsql Development Team")]
[assembly: AssemblyTrademark("")]
[assembly: NeutralResourcesLanguage("en", UltimateResourceFallbackLocation.MainAssembly)]

// Customized
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: AssemblyInformationalVersion("3.1.0-beta7-1+Branch.develop.Sha.a017bfc066ce372041992d892423c34089090402")]