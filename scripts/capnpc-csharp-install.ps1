$id = "capnpc-csharp"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$installDir = "$scriptDir\..\chocolatey\install"

if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) { Start-Process powershell.exe "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs; exit }

choco install $id --source="'$installDir;https://chocolatey.org/api/v2'" --force

Pause