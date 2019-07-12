$ErrorActionPreference = 'Stop'

$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

Install-Binfile -Name capnpc-csharp -Path "dotnet" -Command "$toolsDir\capnpc-csharp.dll"