param (
	[string]$env = "env.config",
	[string]$operation = "",
	[string]$devices = "devices.txt",
    [int]$from = 0,
	[int]$to = 10,
	[int]$batch = 2,
	[int]$sleep = 50,
	[int]$extra = 0
 )

$inbatch=""
$batchCnt = 0;
($from..($to-1)) |% {
	$inbatch+= [string]$_ + ","
	$batchCnt++;

	if($batchCnt -eq $batch) {

		Start-Process .\Thriot.Loadtester -Argumentlist "$env /$operation $devices $inbatch $sleep $extra"
		$inbatch="";
		$batchCnt=0
		sleep 2
	}
}

if($batchCnt -gt 0) {
	Start-Process .\Thriot.Loadtester -Argumentlist "$env /$operation $devices $inbatch $sleep $extra"
 }

## Examples:

# Record telmetry data using the occasionally connected client for devices 0-199, 20 devices/simulator process, wait 500 ms between posts for every device
#.\exec.ps1 -devices devices.txt -operation ocrecord -from 0 -to 200 -batch 20 -sleep 500

# Send message using the occasionally connected client from devices 360-400 to devices 0-360 (extra parameter) , 20 devices/simulator process, wait 500 ms between posts for every device
#.\exec.ps1 -devices devices.txt -operation ocsendto -from 360 -to 400 -batch 20 -sleep 500 -extra 360

# Receive message with QoS 0-level using the occasionally connected client for devices 0-360 in 20 batches, sleeping 300 ms
#.\exec.ps1 -devices devices.txt -operation ocrecvforget -from 0 -to 360 -batch 20 -sleep 300
# Receive message with QoS 1-level using the occasionally connected client for devices 0-360 in 20 batches, sleeping 300 ms
#.\exec.ps1 -devices devices.txt -operation ocrecvcommit -from 0 -to 360 -batch 20 -sleep 300


# These do similar operations but with the persistently connected client

#.\exec.ps1 -devices devices.txt -operation precord -from 0 -to 400 -batch 10 -sleep 100

#.\exec.ps1 -devices devices.txt -operation psendto -from 360 -to 400 -batch 10 -sleep 100 -extra 360
#.\exec.ps1 -devices devices.txt -operation precvforget -from 0 -to 360 -batch 10 -sleep 100
#.\exec.ps1 -devices devices.txt -operation precvcommit -from 0 -to 360 -batch 10 -sleep 100





#.\exec.ps1 -devices devices.txt -operation precord -from 0 -to 3000 -batch 100 -sleep 100
#.\exec.ps1 -devices devices.txt -operation psendto -from 2500 -to 3000 -batch 100 -sleep 5000 -extra 2500
#.\exec.ps1 -devices devices.txt -operation precvforget -from 0 -to 2500 -batch 100 -sleep 1000
#.\exec.ps1 -devices devices.txt -operation precvcommit -from 0 -to 2500 -batch 100 -sleep 1000