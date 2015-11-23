param 
(
	[string]$config,
	[string]$configtmt,
	[string]$configmsg,
	[string]$copyConfigs,
	[string]$linuxify,
	[string]$queueconfig,
	[string]$targetToPassFile = ""
)

function Choose([string]$title, [string]$message, [array]$opts, [string]$actual, [int]$default)
{
	if($actual -eq $null -or $actual -eq "")
	{
		$arr = $($opts |% {New-Object System.Management.Automation.Host.ChoiceDescription $_.Item1, $_.Item2})
		$choiceOptions = [System.Management.Automation.Host.ChoiceDescription[]]$arr

		$result = $host.ui.PromptForChoice($title, $message, $choiceOptions, $default);
		$actual = $opts[$result].Item1.Replace("&","");
	}
	else
	{
		$found = $(($opts |? {$actual -eq $_.Item1.Replace("&","")}).Count -eq 1)

		if(!$found)
		{
			echo "Invalid parameter $actual for $title"
			exit
		}
	}

	return $actual;
}

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

function LinuxifyNLogConfig([string]$configPath)
{
	$tmp = $configPath + ".tmp"
	cat $configPath |% {$_.Replace("fileName=""c:/Thriot/log", "fileName=""/var/log/thriot")} | out-file $tmp -encoding utf8
	mv -Force $tmp $configPath
}

function RestoreASPNET5([string]$project)
{
	dnu restore $project
}

function PublishASPNET5([string]$project, [string]$target)
{
	dnu publish $project --no-source --runtime active --configuration $buildConfig -o $target

	#WORKAROUND httpplatformhandler
	$webconfig=$(cat "$target\wwwroot\web.config")
	$webconfig=$webconfig.Replace("<handlers>", "<handlers>" + [Environment]::NewLine + "      <remove name=""httpplatformhandler"" />");
	$webconfig | set-content "$target\wwwroot\web.config" -Encoding UTF8
	#WORKAROUND end
}

function ConfigKeeper([string]$configDir, [string]$name, [string]$config, [string]$ext="json")
{
	mv -Force $configDir\$name.$config.$ext $configDir\$name.$ext
	rm $configDir\$name.*.$ext
}

$options = @([tuple]::Create("&azure", "Azure Table Storage"), [tuple]::Create("&sql", "Microsoft Sql 2012+ (Express)"), [tuple]::Create("&pgsql", "PostgreSql 9.4+"));
$config = Choose "Master Management Storage" $null $options $config 0

$options = @([tuple]::Create("&azure", "Azure Table Storage"), [tuple]::Create("&sql", "Microsoft Sql 2012+ (Express)"), [tuple]::Create("&pgsql", "PostgreSql 9.4+"), [tuple]::Create("&cassandra", "Cassandr 2.1+"));
$configtmt = Choose "Telemetry storage" $null $options $configtmt 0

$options = @([tuple]::Create("&sql", "Microsoft Sql 2012+ (Express)"), [tuple]::Create("&pgsql", "PostgreSql 9.4+"));
$configmsg = Choose "Messaging backend storage" $null $options $configmsg 0

$options = @([tuple]::Create("&yes", "Yes"), [tuple]::Create("&no", "No"));
$copyConfigs = Choose "Deploy configuration files?" "Answer carefully if your are deploying to a staging/production environment." $options $copyConfigs 1

$options = @([tuple]::Create("&yes", "Yes"), [tuple]::Create("&no", "No"));
$linuxify = Choose "Is this build targeting a Linux environment?" $null $options $linuxify 1

$options = @([tuple]::Create("&no", "Do not use queue"), [tuple]::Create("&azure", "Azure Queue"), [tuple]::Create("&sql", "Microsoft Sql-based queue"), [tuple]::Create("&pgsql", "PostgreSql-based queue"));
$queueconfig = Choose "Queueing solution" $null $options $queueconfig 0

"Master Management Storage: $config"
"Telemetry Storage: $configtmt"
"Messaging storage: $configmsg"
"Deploy configuration files: $copyConfigs"
"Prepare for Linux environment: $linuxify"
"Queueing solution: $queueconfig"

$targetRoot = $(pwd).Path + "\output\" + [DateTime]::Now.ToString("yyyyMMddHHmm") + "_" + $config
$solutionRoot = $(Split-Path -parent $(pwd))
$msbuild = "c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
$buildConfig = "Debug"
$dnxFullName = "dnx-clr-win-x64.1.0.0-rc1-final"
$dnxVersion = "1.0.0-rc1-final"
$dnxClr = "clr"
$dnxArch = "x64"
$runtimeFolder = $env:USERPROFILE + "\.dnx\runtimes\" + $dnxFullName

if(-not $env:Path.Contains("Common7\IDE\Extensions\Microsoft\Web Tools\External\")) {
	$env:Path = $env:Path + ";C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\Microsoft\Web Tools\External\";
}
if(-not $env:Path.Contains("node_modules\.bin\")) {
	$env:Path = $env:Path + ";$solutionRoot\Web\Thriot.Web\node_modules\.bin\";
}

dnvm use $dnxVersion -arch $dnxArch -r $dnxClr

RestoreASPNET5 $solutionRoot\Framework\Thriot.Framework.Mvc
RestoreASPNET5 $solutionRoot\Management\Thriot.Management.WebApi
RestoreASPNET5 $solutionRoot\Platform\Thriot.Platform.WebApi
RestoreASPNET5 $solutionRoot\Reporting\Thriot.Reporting.WebApi
RestoreASPNET5 $solutionRoot\Messaging\Thriot.Messaging.WebApi
RestoreASPNET5 $solutionRoot\Web\Thriot.Web

# WORKAROUND to have project.lock.json files so that dnu publish can find the nuget packages (beta6)
RestoreASPNET5 $solutionRoot\wrap

& $msbuild $solutionRoot\Thriot.Service.sln /p:Configuration=$buildConfig

PublishASPNET5 $solutionRoot\Management\Thriot.Management.WebApi $targetRoot\api
PublishASPNET5 $solutionRoot\Platform\Thriot.Platform.WebApi $targetRoot\papi
PublishASPNET5 $solutionRoot\Reporting\Thriot.Reporting.WebApi $targetRoot\rapi
PublishASPNET5 $solutionRoot\Messaging\Thriot.Messaging.WebApi $targetRoot\msvc
PublishASPNET5 $solutionRoot\Web\Thriot.Web $targetRoot\web

EnsureEmptyDirectory $targetRoot\websocketservice

& $msbuild $solutionRoot\Platform\Thriot.Platform.WebsocketService\Thriot.Platform.WebsocketService.csproj /p:Configuration=$buildConfig  /p:OutDir=$targetRoot\websocketservice

EnsureEmptyDirectory $targetRoot\plugins\bin

& $msbuild $solutionRoot\Plugins\Thriot.Plugins.Azure\Thriot.Plugins.Azure.csproj /p:Configuration=$buildConfig  /p:OutDir=$targetRoot\plugins\bin\azure
& $msbuild $solutionRoot\Plugins\Thriot.Plugins.Sql\Thriot.Plugins.Sql.csproj /p:Configuration=$buildConfig  /p:OutDir=$targetRoot\plugins\bin\sql
& $msbuild $solutionRoot\Plugins\Thriot.Plugins.PgSql\Thriot.Plugins.PgSql.csproj /p:Configuration=$buildConfig  /p:OutDir=$targetRoot\plugins\bin\pgsql
& $msbuild $solutionRoot\Plugins\Thriot.Plugins.Cassandra\Thriot.Plugins.Cassandra.csproj /p:Configuration=$buildConfig  /p:OutDir=$targetRoot\plugins\bin\cassandra


if($queueconfig -ne "no") 
{
	EnsureEmptyDirectory $targetRoot\telemetryqueueservice

	& $msbuild $solutionRoot\Platform\Thriot.Platform.TelemetryQueueService\Thriot.Platform.TelemetryQueueService.csproj /p:Configuration=$buildConfig  /p:OutDir=$targetRoot\telemetryqueueservice
}

if($copyConfigs -eq "no")
{
	$configDir = "$targetRoot\web\wwwroot\config"
	rm $configDir\*
	
	$configDir = "$targetRoot\api\approot\packages\Thriot.Management.WebApi\1.0.0\root\config"
	rm $configDir\connectionstring*
	rm $configDir\smtpsettings.json
	ConfigKeeper $configDir "services" $config

	$configDir = "$targetRoot\papi\approot\packages\Thriot.Platform.WebApi\1.0.0\root\config"
	rm $configDir\connectionstring*
	ConfigKeeper $configDir "services" $config
	ConfigKeeper $configDir "telemetryDataSink" $configtmt "xml"
	mv $configDir\telemetryDataSink.xml $configDir\telemetryDataSink.default.xml

	$configDir = "$targetRoot\rapi\approot\packages\Thriot.Reporting.WebApi\1.0.0\root\config"
	rm $configDir\connectionstring*
	ConfigKeeper $configDir "services" $config

	$configDir = "$targetRoot\msvc\approot\packages\Thriot.Messaging.WebApi\1.0.0\root\config"
	rm $configDir\connectionstring*
	ConfigKeeper $configDir "services" $config
	ConfigKeeper $configDir "servicesmsg" $configmsg

	$configDir = "$targetRoot\websocketservice\config"
	rm $configDir\connectionstring*
	ConfigKeeper $configDir "services" $config

	if($queueconfig -ne "no") {
		$configDir = "$targetRoot\telemetryqueueservice\config"
		rm $configDir\connectionstring*
		rm $configDir\telemetryqueue*
		ConfigKeeper $configDir "services" $config
	}
}
else
{
	$configDir = "$targetRoot\web\wwwroot\config"
	mv -Force $configDir\siteRoots.dev.js $configDir\siteRoots.js

	$configDir = "$targetRoot\api\approot\packages\Thriot.Management.WebApi\1.0.0\root\config"
	ConfigKeeper $configDir "connectionstring" $config
	ConfigKeeper $configDir "services" $config

	$configDir = "$targetRoot\papi\approot\packages\Thriot.Platform.WebApi\1.0.0\root\config"
	ConfigKeeper $configDir "connectionstring" $config
	ConfigKeeper $configDir "services" $config
	ConfigKeeper $configDir "telemetryDataSink" $configtmt "xml"

	$configDir = "$targetRoot\rapi\approot\packages\Thriot.Reporting.WebApi\1.0.0\root\config"
	ConfigKeeper $configDir "connectionstring" $config
	ConfigKeeper $configDir "services" $config

	$configDir = "$targetRoot\msvc\approot\packages\Thriot.Messaging.WebApi\1.0.0\root\config"
	ConfigKeeper $configDir "connectionstring" $config
	ConfigKeeper $configDir "connectionstringmsg" $configmsg
	ConfigKeeper $configDir "services" $config
	ConfigKeeper $configDir "servicesmsg" $configmsg

	$configDir = "$targetRoot\websocketservice\config"
	ConfigKeeper $configDir "connectionstring" $config
	ConfigKeeper $configDir "services" $config

	if($queueconfig -ne "no") {
		$configDir = "$targetRoot\telemetryqueueservice\config"
		ConfigKeeper $configDir "connectionstring" $config
		ConfigKeeper $configDir "services" $config

		ConfigKeeper $configDir "telemetryqueue" $queueconfig
		cp $configDir/telemetryqueue.json $targetRoot\papi\approot\packages\Thriot.Platform.WebApi\1.0.0\root\config
		cp $configDir/telemetryqueue.json $targetRoot\websocketservice\config
	}
}

EnsureEmptyDirectory $targetRoot\install\configtemplates
EnsureEmptyDirectory $targetRoot\install\storage\management
EnsureEmptyDirectory $targetRoot\install\storage\messaging
EnsureEmptyDirectory $targetRoot\install\storage\queue

if($config -eq "azure") {
	& $msbuild $solutionRoot\Misc\Thriot.CreateAzureStorage\Thriot.CreateAzureStorage.csproj /p:Configuration=$buildConfig  /p:OutDir=$targetRoot\install\storage\management
}

if($config -eq "sql") {
	& $msbuild $solutionRoot\Misc\Thriot.CreateSqlStorage\Thriot.CreateSqlStorage.csproj /p:Configuration=$buildConfig  /p:OutDir=$targetRoot\install\storage\management
	cp $solutionRoot\Misc\Thriot.CreateSqlStorage\Thriot.CreateSqlStorage.exe.Sql.config $targetRoot\install\storage\management\Thriot.CreateSqlStorage.exe.config
}

if($config -eq "pgsql") {
	& $msbuild $solutionRoot\Misc\Thriot.CreateSqlStorage\Thriot.CreateSqlStorage.csproj /p:Configuration=$buildConfig  /p:OutputPath=$targetRoot\install\storage\management
	cp $solutionRoot\Misc\Thriot.CreateSqlStorage\Thriot.CreateSqlStorage.exe.PgSql.config $targetRoot\install\storage\management\Thriot.CreateSqlStorage.exe.config
}

if($configmsg -eq "sql") {
	cp $solutionRoot\Messaging\Scripts\Sql\* $targetRoot\install\storage\messaging
}

if($configmsg -eq "pgsql") {
	cp $solutionRoot\Messaging\Scripts\PgSql\* $targetRoot\install\storage\messaging
}

if($queueconfig -eq "sql") {
	cp $solutionRoot\Platform\Scripts\Sql\* $targetRoot\install\storage\queue
}

if($queueconfig -eq "pgsql") {
	cp $solutionRoot\Platform\Scripts\PgSql\* $targetRoot\install\storage\queue
}

if($linuxify -eq "yes")
{
	LinuxifyNLogConfig $targetRoot\api\approot\packages\Thriot.Management.WebApi\1.0.0\root\config\web.nlog
	LinuxifyNLogConfig $targetRoot\papi\approot\packages\Thriot.Platform.WebApi\1.0.0\root\config\web.nlog
	LinuxifyNLogConfig $targetRoot\rapi\approot\packages\Thriot.Reporting.WebApi\1.0.0\root\config\web.nlog
	LinuxifyNLogConfig $targetRoot\msvc\approot\packages\Thriot.Messaging.WebApi\1.0.0\root\config\web.nlog
	LinuxifyNLogConfig $targetRoot\websocketservice\config\nlog.config
	if($queueconfig -ne "no") {
		LinuxifyNLogConfig $targetRoot\telemetryqueueservice\config\nlog.config
	}

	cp $solutionRoot\Build\templates\config\tinyproxy.conf $targetRoot\install\configtemplates
	cp $solutionRoot\Build\templates\config\settings.sql $targetRoot\install\storage
	cp $solutionRoot\Build\templates\config\run.sh $targetRoot\install
}

if($targetToPassFile -ne "") {
	$targetRoot > $targetToPassFile
}
