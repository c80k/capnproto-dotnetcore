$ErrorActionPreference = 'Stop'

$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

choco install capnproto
Install-Binfile -Name capnpc-csharp -Path "$toolsDir\capnpc-csharp.exe"
