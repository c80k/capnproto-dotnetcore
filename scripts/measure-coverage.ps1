$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = "$scriptDir\.."
$coverageDir = "$rootDir\coverage"
$coverageReportDir = "$rootDir\coverage\report"
$openCover = "$env:LOCALAPPDATA\Apps\OpenCover\OpenCover.Console.exe"
$vsTestConsole = where.exe vstest.console

$runtimeTests = "$rootDir\Capnp.Net.Runtime.Tests\bin\Release\netcoreapp2.1\Capnp.Net.Runtime.Tests.dll"
$coverageOutputRuntime = "$coverageDir\cov-Capnp.Net.Runtime.xml"

$generatorTests = "$rootDir\CapnpC.CSharp.Generator.Tests\bin\Release\netcoreapp3.0\CapnpC.CSharp.Generator.Tests.dll"
$coverageOutputGenerator = "$coverageDir\cov-CapnpC.CSharp.Generator.xml"

If(!(test-path $coverageDir))
{
      New-Item -ItemType Directory -Force -Path $coverageDir
}

If(!(test-path $coverageReportDir))
{
      New-Item -ItemType Directory -Force -Path $coverageReportDir
}

& $openCover -target:"$vsTestConsole" `
  -targetArgs:"/inIsolation $runtimeTests /TestCaseFilter:`"TestCategory=Coverage`"" `
  -filter:"+[Capnp.Net.Runtime]Capnp.*" `
  -excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute" `
  -output:"$coverageOutputRuntime" `
  -mergebyhash -register:user -oldStyle

& $openCover -target:"$vsTestConsole" `
  -targetArgs:"/inIsolation $generatorTests" `
  -filter:"+[CapnpC.CSharp.Generator]CapnpC.CSharp.Generator.* -[CapnpC.CSharp.Generator]CapnpC.CSharp.Generator.Schema.*" `
  -excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute" `
  -output:"$coverageOutputGenerator" `
  -mergebyhash -register:user -oldStyle

ReportGenerator.exe -reports:"$coverageOutputRuntime;$coverageOutputGenerator" -targetdir:"$coverageReportDir" -reportTypes:"Html;Xml"
