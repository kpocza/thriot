param (
	[string]$solutionRoot = "$(pwd)\..\",
	[string]$targetRoot = "c:\Thriot\",
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

&$msbuild $solutionRoot\Iot.Service.sln /p:Configuration=$config /p:DebugSymbols=true

&$msbuild $solutionRoot\Web\IoT.Web\IoT.Web.csproj /p:Configuration=$config /p:DeployOnBuild=true /p:AutoParameterizationWebConfigConnectionStrings=false /p:DeployTarget=Package /p:OutputPath=bin\$config /p:_PackageTempDir=$targetRoot\web /p:DebugSymbols=true
&$msbuild $solutionRoot\Management\IoT.Management.WebApi\IoT.Management.WebApi.csproj /p:Configuration=$config /p:DeployOnBuild=true /p:AutoParameterizationWebConfigConnectionStrings=false /p:DeployTarget=Package /p:OutputPath=bin\$config /p:_PackageTempDir=$targetRoot\api /p:DebugSymbols=true
&$msbuild $solutionRoot\Platform\IoT.Platform.WebApi\IoT.Platform.WebApi.csproj /p:Configuration=$config /p:DeployOnBuild=true /p:AutoParameterizationWebConfigConnectionStrings=false /p:DeployTarget=Package /p:OutputPath=bin\$config /p:_PackageTempDir=$targetRoot\papi /p:DebugSymbols=true
&$msbuild $solutionRoot\Messaging\IoT.Messaging.WebApi\IoT.Messaging.WebApi.csproj /p:Configuration=$config /p:DeployOnBuild=true /p:AutoParameterizationWebConfigConnectionStrings=false /p:DeployTarget=Package /p:OutputPath=bin\$config /p:_PackageTempDir=$targetRoot\msvc /p:DebugSymbols=true
&$msbuild $solutionRoot\Reporting\IoT.Reporting.WebApi\IoT.Reporting.WebApi.csproj /p:Configuration=$config /p:DeployOnBuild=true /p:AutoParameterizationWebConfigConnectionStrings=false /p:DeployTarget=Package /p:OutputPath=bin\$config /p:_PackageTempDir=$targetRoot\rapi /p:DebugSymbols=true

EnsureEmptyDirectory $targetRoot\websocketservice;

cp -Recu -Force $solutionRoot\Platform\IoT.Platform.WebsocketService\bin\$config\* $targetRoot\websocketservice\

if($config -eq "DevAzure" -or $config -eq "DevSql") {
	mv -Force $targetRoot\web\config\siteRoots.dev.js $targetRoot\web\config\siteRoots.js 
}

if($config -eq "ProdAzure" -or $config -eq "ProdSql") {
	rmdir -Recu -Force $targetRoot\web\config
}

EnsureEmptyDirectory $targetRoot\install;
EnsureEmptyDirectory $targetRoot\install\configtemplates;
EnsureEmptyDirectory $targetRoot\install\storage;
EnsureEmptyDirectory $targetRoot\install\storage\messaging;
EnsureEmptyDirectory $targetRoot\install\storage\management;

if($config -eq "DevAzure" -or $config -eq "ProdAzure") {
	cp $solutionRoot\Build\templates\config\azure\* $targetRoot\install\configtemplates
	cp $solutionRoot\Messaging\Scripts\* $targetRoot\install\storage\messaging
	cp -Recu $solutionRoot\Misc\IoT.CreateAzureStorage\bin\Debug\* $targetRoot\install\storage\management
}

if($config -eq "DevSql" -or $config -eq "ProdSql") {
	cp $solutionRoot\Build\templates\config\sql\* $targetRoot\install\configtemplates
	cp $solutionRoot\Messaging\Scripts\* $targetRoot\install\storage\messaging
	cp -Recu $solutionRoot\Misc\IoT.CreateSqlStorage\bin\Debug\* $targetRoot\install\storage\management
}
