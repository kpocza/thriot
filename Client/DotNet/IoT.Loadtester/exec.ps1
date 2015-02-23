param (
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

		Start-Process .\IoT.Loadtester -Argumentlist "/$operation $devices $inbatch $sleep $extra"
		$inbatch="";
		$batchCnt=0
		sleep 2
	}
}

if($batchCnt -gt 0) {
	Start-Process .\IoT.Loadtester -Argumentlist "/$operation $devices $inbatch $sleep $extra"
 }

#Examples:
#.\exec.ps1 -devices devices.txt -operation ocrecord -from 0 -to 200 -batch 10 -sleep 100
#.\exec.ps1 -devices devices.txt -operation ocrecvforget -from 0 -to 360 -batch 10 -sleep 100
#.\exec.ps1 -devices devices.txt -operation ocrecvcommit -from 0 -to 360 -batch 10 -sleep 100
#.\exec.ps1 -devices devices.txt -operation ocsendto -from 360 -to 400 -batch 10 -sleep 100 -extra 360
#.\exec.ps1 -devices devices.txt -operation precord -from 0 -to 400 -batch 10 -sleep 100
#.\exec.ps1 -devices devices.txt -operation precvforget -from 0 -to 360 -batch 10 -sleep 100
#.\exec.ps1 -devices devices.txt -operation precvcommit -from 0 -to 360 -batch 10 -sleep 100
#.\exec.ps1 -devices devices.txt -operation psendto -from 360 -to 400 -batch 10 -sleep 100 -extra 360

#.\exec.ps1 -devices devices.txt -operation precord -from 0 -to 3000 -batch 100 -sleep 100
#.\exec.ps1 -devices devices.txt -operation psendto -from 2500 -to 3000 -batch 100 -sleep 5000 -extra 2500
#.\exec.ps1 -devices devices.txt -operation precvforget -from 0 -to 2500 -batch 100 -sleep 1000
#.\exec.ps1 -devices devices.txt -operation precvcommit -from 0 -to 2500 -batch 100 -sleep 1000