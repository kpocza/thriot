param (
	[string]$devices = 'devices.txt',
    [int]$cnt = 10
 )

.\IoT.Loadtester.exe env.config /generate $devices $cnt
