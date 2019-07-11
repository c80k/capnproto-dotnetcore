$id = "capnpc-csharp"
$version = "1.0.0"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$prefix = "$id.$version"
$chocoDir = "$scriptDir\..\chocolatey"
$nuspecFile = "$prefix.nuspec"
$nuspecPath = "$chocoDir\$nuspecFile"
$deployDir = "$chocoDir\deploy"
$installDir = "$chocoDir\install"
$csprojDir = "$scriptDir\..\capnpc-csharp"
$csprojFile = "capnpc-csharp.csproj"

dotnet publish -c Release -r win-x86 --self-contained -o $deployDir "$csprojDir\$csprojFile"
choco pack $nuspecPath --outputdirectory $installDir
