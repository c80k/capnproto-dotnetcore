$myDir = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet "$myDir\..\bin\capnpc-csharp.dll"
