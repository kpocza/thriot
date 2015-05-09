param (
	[string]$devices = 'devices.txt',
    [int]$cnt = 10
 )

.\Thriot.Loadtester.exe env.config /generate $devices $cnt
