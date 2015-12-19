makecert -n "CN=%1" -iv ThriotRootCA.pvk -ic ThriotRootCA.cer -pe -a sha512 -len 4096 -b 12/18/2015 -e 12/18/2025 -sky exchange -eku 1.3.6.1.5.5.7.3.1 -sv %2.pvk %2.cer

pvk2pfx -pvk %2.pvk -spc %2.cer -pfx %2.pfx -pi ThriotIoPwd123
