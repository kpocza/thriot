makecert -n "CN=Thriot Virtual Root CA" -r -pe -a sha512 -len 4096 -cy authority -sv ThriotRootCA.pvk ThriotRootCA.cer

pvk2pfx -pvk ThriotRootCA.pvk -spc ThriotRootCA.cer -pfx ThriotRootCA.pfx -pi ThriotRootCAPwd123
