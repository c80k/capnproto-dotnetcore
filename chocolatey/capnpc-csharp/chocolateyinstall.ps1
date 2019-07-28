$ErrorActionPreference = 'Stop'

$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

Install-ChocolateyPowershellCommand -PackageName 'capnpc-csharp' -PsFileFullPath "$toolsDir\capnpc-csharp.ps1"
