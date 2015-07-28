param 
(
	[string]$config,
	[string]$configmsg,
	[string]$copyConfigs,
	[string]$linuxify
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
	dnu publish $project --no-source --configuration $buildConfig -o $target

	#WORKAROUND for publish bug
	mkdir $target\approot\runtimes\$dnxFullName
	cp -Recu $runtimeFolder\* $target\approot\runtimes\$dnxFullName

	$xml=[xml]$(cat "$target\wwwroot\web.config")
	($xml.configuration.appSettings.add |? {$_.key -eq "dnx-version"}).value = $dnxVersion
	($xml.configuration.appSettings.add |? {$_.key -eq "dnx-clr"}).value = $dnxClr
	$xml.Save("$target\wwwroot\web.config")
	#WORKAROUND end
}

function ConfigKeeper([string]$configDir, [string]$name, [string]$config, [string]$ext="json")
{
	mv -Force $configDir\$name.$config.$ext $configDir\$name.$ext
	rm $configDir\$name.*.$ext
}

$options = @([tuple]::Create("&azure", "Azure Table Storage"), [tuple]::Create("&sql", "Microsoft Sql 2012+ (Express)"), [tuple]::Create("&pgsql", "PostgreSql 9.4+"));
$config = Choose "Master Management Storage" $null $options $config 0

$options = @([tuple]::Create("&sql", "Microsoft Sql 2012+ (Express)"), [tuple]::Create("&pgsql", "PostgreSql 9.4+"));
$configmsg = Choose "Messaging backend storage" $null $options $configmsg 0

$options = @([tuple]::Create("&yes", "Yes"), [tuple]::Create("&no", "No"));
$deployConfigs = Choose "Deploy configuration files?" "Answer carefully if your are deploying to a staging/production environment." $options $deployConfigs 1

$options = @([tuple]::Create("&yes", "Yes"), [tuple]::Create("&no", "No"));
$linuxify = Choose "Is this build targeting a Linux environment?" $null $options $linuxify 1

"Master Management Storage: $config"
"Messaging storage: $configmsg"
"Deploy configuration files: $deployConfigs"
"Prepare for Linux environment: $linuxify"

$targetRoot = $(pwd).Path + "\output\" + [DateTime]::Now.ToString("yyyyMMddHHmm") + "_" + $config
$solutionRoot = $(Split-Path -parent $(pwd))
$msbuild = "c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
$buildConfig = "Debug"
$dnxFullName = "dnx-clr-win-x64.1.0.0-beta6"
$dnxVersion = "1.0.0-beta6"
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

# WORKAROUND to have project.lock.json files so that dnu publish can find the nuget packages
RestoreASPNET5 $solutionRoot\wrap

& $msbuild $solutionRoot\Thriot.Service.sln /p:Configuration=$buildConfig

PublishASPNET5 $solutionRoot\Management\Thriot.Management.WebApi $targetRoot\api
PublishASPNET5 $solutionRoot\Platform\Thriot.Platform.WebApi $targetRoot\papi
PublishASPNET5 $solutionRoot\Reporting\Thriot.Reporting.WebApi $targetRoot\rapi
PublishASPNET5 $solutionRoot\Messaging\Thriot.Messaging.WebApi $targetRoot\msvc
PublishASPNET5 $solutionRoot\Web\Thriot.Web $targetRoot\web

EnsureEmptyDirectory $targetRoot\websocketservice

& $msbuild $solutionRoot\Platform\Thriot.Platform.WebsocketService\Thriot.Platform.WebsocketService.csproj /p:Configuration=$buildConfig  /p:OutDir=$targetRoot\websocketservice

if($deployConfigs -eq "no")
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
	ConfigKeeper $configDir "telemetryDataSink" $config "xml"
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
}
else
{
}

exit

if($config.StartsWith("Dev"))
{
	mv -Force $targetRoot\web\wwwroot\config\siteRoots.dev.js $targetRoot\web\wwwroot\config\siteRoots.js 
}

if($config.StartsWith("Prod") -and $(test-path $targetRoot\web\wwwroot\config))
{
	rmdir -Recu -Force $targetRoot\web\wwwroot\config
}

EnsureEmptyDirectory $targetRoot\install;
EnsureEmptyDirectory $targetRoot\install\configtemplates;
EnsureEmptyDirectory $targetRoot\install\storage;
EnsureEmptyDirectory $targetRoot\install\storage\messaging;
EnsureEmptyDirectory $targetRoot\install\storage\management;

if($config -eq "DevAzure" -or $config -eq "ProdAzure")
{
	cp $solutionRoot\Build\templates\config\azure\* $targetRoot\install\configtemplates
	cp $solutionRoot\Messaging\Scripts\Sql\* $targetRoot\install\storage\messaging
	cp -Recu $solutionRoot\Misc\Thriot.CreateAzureStorage\bin\Debug\* $targetRoot\install\storage\management
}

if($config -eq "DevSql" -or $config -eq "ProdSql")
{
	cp $solutionRoot\Build\templates\config\sql\* $targetRoot\install\configtemplates
	cp $solutionRoot\Messaging\Scripts\Sql\* $targetRoot\install\storage\messaging
	cp -Recu $solutionRoot\Misc\Thriot.CreateSqlStorage\bin\Debug\* $targetRoot\install\storage\management
}

if($config -eq "DevPgSql" -or $config -eq "ProdPgSql")
{
	cp $solutionRoot\Build\templates\config\pgsql\* $targetRoot\install\configtemplates
	cp $solutionRoot\Messaging\Scripts\PgSql\* $targetRoot\install\storage\messaging
	cp -Recu $solutionRoot\Misc\Thriot.CreateSqlStorage\bin\DevPgSql\* $targetRoot\install\storage\management
	cp -Force $targetRoot\papi\bin\Npgsql.EntityFramework.dll $targetRoot\websocketservice
}

if($linuxify)
{
	if($config.StartsWith("Dev"))
	{
		foreach($dir in ("api", "msvc", "papi", "rapi"))
		{
			LinuxifyNLogConfig $targetRoot\$dir\Web.config
		}

		LinuxifyNLogConfig $targetRoot\websocketservice\Thriot.Platform.WebsocketService.exe.config
	}
	if($config.StartsWith("Prod"))
	{
		foreach($cnfg in $(ls $targetRoot\install\configtemplates\nlog*.config))
		{
			LinuxifyNLogConfig $cnfg.FullName
		}
	}

	cp $solutionRoot\Build\templates\config\linux\tinyproxy.conf $targetRoot\install\configtemplates
	cp $solutionRoot\Build\templates\config\linux\settings.sql $targetRoot\install\storage
	cp $solutionRoot\Build\templates\config\linux\run.sh $targetRoot\install
}
