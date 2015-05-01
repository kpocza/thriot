param (
	[string]$solutionRoot = "$(pwd)\..\",
	[string]$targetRoot = $null,
	[string]$msbuild = "c:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe",
	[string]$config = "ProdAzure"
)

function EnsureEmptyDirectory([string]$dirPath)
{
	if(test-path $dirPath) 
	{
		rm -Force -Recu $dirPath\*
	}
	else
	{
		mkdir $dirPath 
	}
}

if(!$targetRoot -or $(![System.IO.Path]::IsPathRooted($targetRoot))) {
	$targetRoot = $(pwd).Path + "\output\" + [DateTime]::Now.ToString("yyyyMMddHHmm") + "_" + $config
}

&$msbuild $solutionRoot\Thriot.Service.sln /p:Configuration=$config /p:DebugSymbols=true

&$msbuild $solutionRoot\Web\Thriot.Web\Thriot.Web.csproj /p:Configuration=$config /p:DeployOnBuild=true /p:AutoParameterizationWebConfigConnectionStrings=false /p:DeployTarget=Package /p:OutputPath=bin\$config /p:_PackageTempDir=$targetRoot\web /p:DebugSymbols=true
&$msbuild $solutionRoot\Management\Thriot.Management.WebApi\Thriot.Management.WebApi.csproj /p:Configuration=$config /p:DeployOnBuild=true /p:AutoParameterizationWebConfigConnectionStrings=false /p:DeployTarget=Package /p:OutputPath=bin\$config /p:_PackageTempDir=$targetRoot\api /p:DebugSymbols=true
&$msbuild $solutionRoot\Platform\Thriot.Platform.WebApi\Thriot.Platform.WebApi.csproj /p:Configuration=$config /p:DeployOnBuild=true /p:AutoParameterizationWebConfigConnectionStrings=false /p:DeployTarget=Package /p:OutputPath=bin\$config /p:_PackageTempDir=$targetRoot\papi /p:DebugSymbols=true
&$msbuild $solutionRoot\Messaging\Thriot.Messaging.WebApi\Thriot.Messaging.WebApi.csproj /p:Configuration=$config /p:DeployOnBuild=true /p:AutoParameterizationWebConfigConnectionStrings=false /p:DeployTarget=Package /p:OutputPath=bin\$config /p:_PackageTempDir=$targetRoot\msvc /p:DebugSymbols=true
&$msbuild $solutionRoot\Reporting\Thriot.Reporting.WebApi\Thriot.Reporting.WebApi.csproj /p:Configuration=$config /p:DeployOnBuild=true /p:AutoParameterizationWebConfigConnectionStrings=false /p:DeployTarget=Package /p:OutputPath=bin\$config /p:_PackageTempDir=$targetRoot\rapi /p:DebugSymbols=true

EnsureEmptyDirectory $targetRoot\websocketservice;

cp -Recu -Force $solutionRoot\Platform\Thriot.Platform.WebsocketService\bin\$config\* $targetRoot\websocketservice\

if($config.StartsWith("Dev")) {
	mv -Force $targetRoot\web\config\siteRoots.dev.js $targetRoot\web\config\siteRoots.js 
}

if($config.StartsWith("Prod") -and $(test-path $targetRoot\web\config)) {
	rmdir -Recu -Force $targetRoot\web\config
}

EnsureEmptyDirectory $targetRoot\install;
EnsureEmptyDirectory $targetRoot\install\configtemplates;
EnsureEmptyDirectory $targetRoot\install\storage;
EnsureEmptyDirectory $targetRoot\install\storage\messaging;
EnsureEmptyDirectory $targetRoot\install\storage\management;

if($config -eq "DevAzure" -or $config -eq "ProdAzure") {
	cp $solutionRoot\Build\templates\config\azure\* $targetRoot\install\configtemplates
	cp $solutionRoot\Messaging\Scripts\Sql\* $targetRoot\install\storage\messaging
	cp -Recu $solutionRoot\Misc\Thriot.CreateAzureStorage\bin\Debug\* $targetRoot\install\storage\management
}

if($config -eq "DevSql" -or $config -eq "ProdSql") {
	cp $solutionRoot\Build\templates\config\sql\* $targetRoot\install\configtemplates
	cp $solutionRoot\Messaging\Scripts\Sql\* $targetRoot\install\storage\messaging
	cp -Recu $solutionRoot\Misc\Thriot.CreateSqlStorage\bin\Debug\* $targetRoot\install\storage\management
}

if($config -eq "DevPgSql" -or $config -eq "ProdPgSql") {
	cp $solutionRoot\Build\templates\config\pgsql\* $targetRoot\install\configtemplates
	cp $solutionRoot\Messaging\Scripts\PgSql\* $targetRoot\install\storage\messaging
	cp -Recu $solutionRoot\Misc\Thriot.CreateSqlStorage\bin\DevPgSql\* $targetRoot\install\storage\management
}
