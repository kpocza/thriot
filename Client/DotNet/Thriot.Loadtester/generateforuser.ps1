param (
	[string]$devices = 'devices.txt',
    [int]$cnt = 10,
	[string]$email,
	[string]$password
 )

.\Thriot.Loadtester.exe env.config /generateforuser $devices $cnt $email $password
