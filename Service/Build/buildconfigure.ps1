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
./build.ps1 -config $build.config -configtmt $build.configtmt -configmsg $build.configmsg -copyConfigs $build.copyConfigs -linuxify $build.linuxify -queueconfig $build.queueconfig -security $build.security -targetToPassFile $targetToPassFile

$targetRoot = (cat $targetToPassFile)

"Executing configurator at $targetRoot using $configFile ..."
./configure.ps1 $configFile $targetRoot
