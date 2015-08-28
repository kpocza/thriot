param 
(
	[string]$configFile,
	[string]$targetRoot
)

function ReplaceParameter([string]$configFolder, [string]$file, [string]$paramName, [string]$to)
{
		$cs = "$configFolder\$file"
		if(Test-Path $cs)
		{
			"   $($file): setting $paramName to $to"
            $findPattern = """($paramName)"": ""(.*?)""" 
			$replaceWith = """`$1"": ""$to"""
			$(cat $cs) -replace $findPattern, $replaceWith | set-content $cs
		}
} 

($build,$connectionstring,$telemetry,$microservice,$queue,$publicurl,$api,$runtime,$path,$smtp)=(@{},@{},@{},@{},@{},@{},@{},@{},@{},@{})

cat $configFile |? {$_.StartsWith("$")} |% {$_ -replace '^(.*?)\=(.*)','$1="$2"'} | Invoke-Expression

"Configurations:"
$build
$connectionstring
$telemetry
$microservice
$queue
$publicurl
$api
$runtime
$path
$smtp

$configFolders = (
	"$targetRoot\api\approot\packages\Thriot.Management.WebApi\1.0.0\root\config",
	"$targetRoot\papi\approot\packages\Thriot.Platform.WebApi\1.0.0\root\config",
	"$targetRoot\rapi\approot\packages\Thriot.Reporting.WebApi\1.0.0\root\config",
	"$targetRoot\msvc\approot\packages\Thriot.Messaging.WebApi\1.0.0\root\config",
	"$targetRoot\web\wwwroot\config",
	"$targetRoot\websocketservice\config",
	"$targetRoot\telemetryqueueservice\config"
)

$configFolders |? {Test-Path $_} |% {
	"Processing: $_"
    ReplaceParameter $_ "connectionstring.json" "ConnectionString" $connectionstring.management
    ReplaceParameter $_ "connectionstringmsg.json" "MessagingConnection" $connectionstring.messaging

    ReplaceParameter $_ "telemetryqueue.json" "ConnectionString" $queue.connectionstring
    ReplaceParameter $_ "telemetryqueue.json" "QueueName" $queue.name

    ReplaceParameter $_ "siteRoots.js" "managementRoot" $api.management
    ReplaceParameter $_ "siteRoots.js" "reportingRoot" $api.reporting

    ReplaceParameter $_ "smtpsettings.json" "FromAddress" $smtp.fromaddress
    ReplaceParameter $_ "smtpsettings.json" "FromName" $smtp.fromname
    ReplaceParameter $_ "smtpsettings.json" "BouncesAddress" $smtp.bouncesaddress
    ReplaceParameter $_ "smtpsettings.json" "Host" $smtp.host
    ReplaceParameter $_ "smtpsettings.json" "Port" $smtp.port
    ReplaceParameter $_ "smtpsettings.json" "UserName" $smtp.username
    ReplaceParameter $_ "smtpsettings.json" "Password" $smtp.password
}

