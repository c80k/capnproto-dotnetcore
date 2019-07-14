$id = "capnpc-csharp"
$id_win_x86 = "capnpc-csharp-win-x86"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$chocoDir = "$scriptDir\..\chocolatey"
$csprojDir = "$scriptDir\..\capnpc-csharp"
$csprojFile = "capnpc-csharp.csproj"

dotnet publish -c Release -r win-x86 --self-contained -o "$chocoDir\$id_win_x86\bin" "$csprojDir\$csprojFile"
dotnet publish -c Release -o "$chocoDir\$id\bin" "$csprojDir\$csprojFile"

choco pack "$chocoDir\$id\$id.nuspec" --outputdirectory "$chocoDir\install"
choco pack "$chocoDir\$id_win_x86\$id_win_x86.nuspec" --outputdirectory "$chocoDir\install"
