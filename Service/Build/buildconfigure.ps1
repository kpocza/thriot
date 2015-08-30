param 
(
	[string]$configFile
)

$targetToPassFile = [System.IO.Path]::GetTempFileName()

($build)=(@{})
cat $configFile |? {$_.StartsWith("`$build")} |% {$_ -replace '^(.*?)\=(.*)','$1="$2"'} | Invoke-Expression

"Build configurations:"
$build

"Executing build..."
./build.ps1 -config $build.config -configmsg $build.configmsg -copyConfigs $build.copyConfigs -linuxify $build.linuxify -queueconfig $build.queueconfig -targetToPassFile $targetToPassFile

$targetRoot = (cat $targetToPassFile)

"Executing configurator at $targetRoot using $configFile ..."
./configure.ps1 $configFile $targetRoot
