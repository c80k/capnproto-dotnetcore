$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = "$scriptDir\.."
$coverageDir = "$rootDir\coverage"
$coverageReportDir = "$rootDir\coverage\report"
$openCover = "$env:LOCALAPPDATA\Apps\OpenCover\OpenCover.Console.exe"
$vsTestConsole = where.exe vstest.console
$coverageOutput = "$coverageDir\coverage.xml"

$runtimeTests = "$rootDir\Capnp.Net.Runtime.Tests\bin\Release\netcoreapp2.1\Capnp.Net.Runtime.Tests.dll"
$generatorTests = "$rootDir\CapnpC.CSharp.Generator.Tests\bin\Release\netcoreapp3.1\CapnpC.CSharp.Generator.Tests.dll"

If(!(test-path $coverageDir))
{
      New-Item -ItemType Directory -Force -Path $coverageDir
}

If(!(test-path $coverageReportDir))
{
      New-Item -ItemType Directory -Force -Path $coverageReportDir
}

& $openCover -version

& $openCover -target:"$vsTestConsole" `
  -targetArgs:"/inIsolation $runtimeTests /TestCaseFilter:`"TestCategory=Coverage`" /Framework:.NETCoreApp,Version=v2.1 /logger:trx;LogFileName=runtime.trx" `
  -filter:"+[Capnp.Net.Runtime]Capnp.*" `
  -excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute" `
  -output:"$coverageOutput" `
  -mergebyhash -register:user -oldStyle

& $openCover -target:"$vsTestConsole" `
  -targetArgs:"/noisolation $generatorTests /logger:trx;LogFileName=generator.trx" `
  -filter:"+[CapnpC.CSharp.Generator]CapnpC.CSharp.Generator.* -[CapnpC.CSharp.Generator]CapnpC.CSharp.Generator.Schema.*" `
  -excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute" `
  -output:"$coverageOutput" `
  -mergeoutput `
  -mergebyhash -register:user -oldStyle

ReportGenerator.exe -reports:"$coverageOutput" -targetdir:"$coverageReportDir" -reportTypes:"Html"
