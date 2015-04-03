param (
	[string]$solutionRoot = "$(pwd)\..\",
	[string]$targetRoot = $null,
	[string]$msbuild = "c:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe"
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
	$targetRoot = $(pwd).Path + "\output\" + [DateTime]::Now.ToString("yyyyMMddHHmm")
}

&$msbuild $solutionRoot\Iot.Client.DotNet.sln /p:Configuration=Debug /p:DebugSymbols=true

EnsureEmptyDirectory $targetRoot;

cp -Recu -Force $solutionRoot\Iot.Client.DotNet\bin\Debug\* $targetRoot\
