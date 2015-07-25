#
# PI2Scripts.ps1
# http://ms-iot.github.io/content/en-US/win10/samples/PowerShell.htm
# https://www.hackster.io/windowsiot/powershell-to-connect-to-a-machine-running-windows-10
# Utils and startup: http://ms-iot.github.io/content/en-US/win10/tools/CommandLineUtils.htm
net start WinRM


Set-Item WSMan:\localhost\Client\TrustedHosts -Value 192.168.178.71

Enter-pssession -ComputerName  192.168.178.71 -Credential Administrator 

#PWD: p@ssw0rd
