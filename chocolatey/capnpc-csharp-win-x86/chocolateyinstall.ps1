$ErrorActionPreference = 'Stop'

$binDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)\..\bin"

Install-Binfile -Name capnpc-csharp -Path "$binDir\capnpc-csharp.exe"
