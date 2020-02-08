$jversion = nbgv get-version -f json | ConvertFrom-Json
$version = $($jversion.NuGetPackageVersion)
$id = "capnpc-csharp"
$id_win_x86 = "capnpc-csharp-win-x86"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$chocoDir = "$scriptDir\..\chocolatey"
$csprojDir = "$scriptDir\..\capnpc-csharp"
$csprojFile = "capnpc-csharp.csproj"
$installDir = "$chocoDir\install"

dotnet build -c Release "$scriptDir\..\Capnp.Net.sln"
dotnet publish -c Release -r win-x86 --self-contained -o "$chocoDir\$id_win_x86\bin" "$csprojDir\$csprojFile"
# dotnet publish -c Release -o "$chocoDir\$id\bin" "$csprojDir\$csprojFile"

If(!(test-path $installDir))
{
      New-Item -ItemType Directory -Force -Path $installDir
}

Copy-Item "$scriptDir\..\LICENSE" -Destination "$chocoDir\LICENSE.txt"

choco pack "$chocoDir\$id\$id.nuspec" --version $version --outputdirectory $installDir
choco pack "$chocoDir\$id_win_x86\$id_win_x86.nuspec" --version $version --outputdirectory $installDir
Rename-Item -Path "$installDir\$id.$version.nupkg" -NewName "$id.$version-deprecated.nupkg"