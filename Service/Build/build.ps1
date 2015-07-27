param 
(
	[string]$config,
	[string]$configmsg,
	[string]$env,
	[string]$firsttime,
	[string]$linuxify
)

$solutionRoot = $(Split-Path -parent $(pwd))
$msbuild = "c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
$targetRoot = $(pwd).Path + "\output\" + [DateTime]::Now.ToString("yyyyMMddHHmm") + "_" + $config

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

function BuildASPNET5([string]$project, [string]$target)
{
	dnu publish $project --no-source --configuration Debug -o $target
}

$options = @([tuple]::Create("&azure", "Azure Table Storage"), [tuple]::Create("&sql", "Microsoft Sql 2012+ (Express)"), [tuple]::Create("&pgsql", "PostgreSql 9.4+"));
$config = Choose "Central Management Storage" $null $options $config 0

$options = @([tuple]::Create("&sql", "Microsoft Sql 2012+ (Express)"), [tuple]::Create("&pgsql", "PostgreSql 9.4+"));
$configmsg = Choose "Messaging backend storage" $null $options $configmsg 0

$options = @([tuple]::Create("&dev", "Development"), [tuple]::Create("&prod", "Production"));
$env = Choose "Target environment of this build" $null $options $env 0

$options = @([tuple]::Create("&yes", "Yes"), [tuple]::Create("&no", "No"));
$firstTime = Choose "Is this this first time build for this environment?" "If this is not a first time build then the configs will be removed to protect from accidental config overwriting" $options $firstTime 1

$options = @([tuple]::Create("&yes", "Yes"), [tuple]::Create("&no", "No"));
$linuxify = Choose "Is this build targeting a Linux environment?" $null $options $linuxify 1

$config
$configmsg
$env
$firstTime
$linuxify

if(-not $env:Path.Contains("Common7\IDE\Extensions\Microsoft\Web Tools\External\")) {
	$env:Path = $env:Path + ";C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\Microsoft\Web Tools\External\";
}
if(-not $env:Path.Contains("node_modules\.bin\")) {
	$env:Path = $env:Path + ";" + $solutionRoot + "\Web\Thriot.Web\node_modules\.bin\";
}

dnvm use default

RestoreASPNET5 $solutionRoot\Framework\Thriot.Framework.Mvc
RestoreASPNET5 $solutionRoot\Management\Thriot.Management.WebApi
RestoreASPNET5 $solutionRoot\Platform\Thriot.Platform.WebApi
RestoreASPNET5 $solutionRoot\Reporting\Thriot.Reporting.WebApi
RestoreASPNET5 $solutionRoot\Messaging\Thriot.Messaging.WebApi
RestoreASPNET5 $solutionRoot\Web\Thriot.Web
RestoreASPNET5 $solutionRoot\wrap

& $msbuild $solutionRoot\Thriot.Service.sln /p:Configuration=Debug

BuildASPNET5 $solutionRoot\Management\Thriot.Management.WebApi $targetRoot\api
BuildASPNET5 $solutionRoot\Platform\Thriot.Platform.WebApi $targetRoot\papi
BuildASPNET5 $solutionRoot\Reporting\Thriot.Reporting.WebApi $targetRoot\rapi
BuildASPNET5 $solutionRoot\Messaging\Thriot.Messaging.WebApi $targetRoot\msvc
BuildASPNET5 $solutionRoot\Web\Thriot.Web $targetRoot\web

EnsureEmptyDirectory $targetRoot\websocketservice

& $msbuild $solutionRoot\Platform\Thriot.Platform.WebsocketService\Thriot.Platform.WebsocketService.csproj /p:Configuration=Debug  /p:OutDir=$targetRoot\websocketservice

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
