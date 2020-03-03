$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = "$scriptDir\.."
$coverageDir = "$rootDir\coverage"
$coverageReportDir = "$rootDir\coverage\report"
$openCover = "$env:LOCALAPPDATA\Apps\OpenCover\OpenCover.Console.exe"
$vsTestConsole = where.exe vstest.console

$runtimeTestsDnc21 = "$rootDir\Capnp.Net.Runtime.Tests.Core21\bin\Release\netcoreapp2.1\Capnp.Net.Runtime.Tests.Core21.dll"
$coverageOutputRuntimeDnc21 = "$coverageDir\cov-Capnp.Net.Runtime-dnc21.xml"

$runtimeTestsNet471 = "$rootDir\Capnp.Net.Runtime.Tests\bin\Release\net471\Capnp.Net.Runtime.Tests.Std20.dll"
$coverageOutputRuntimeNet471 = "$coverageDir\cov-Capnp.Net.Runtime-net471.xml"

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
  -targetArgs:"/inIsolation $runtimeTestsDnc21 /TestCaseFilter:`"TestCategory=Coverage`"" `
  -filter:"+[Capnp.Net.Runtime]Capnp.*" `
  -excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute" `
  -output:"$coverageOutputRuntimeDnc21" `
  -mergebyhash -register:user -oldStyle

& $openCover -target:"$vsTestConsole" `
  -targetArgs:"/inIsolation $runtimeTestsNet471 /TestCaseFilter:`"TestCategory=Coverage`"" `
  -filter:"+[Capnp.Net.Runtime]Capnp.*" `
  -excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute" `
  -output:"$coverageOutputRuntimeNet471" `
  -mergebyhash -register:user -oldStyle

& $openCover -target:"$vsTestConsole" `
  -targetArgs:"/inIsolation $generatorTests" `
  -filter:"+[CapnpC.CSharp.Generator]CapnpC.CSharp.Generator.* -[CapnpC.CSharp.Generator]CapnpC.CSharp.Generator.Schema.*" `
  -excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute" `
  -output:"$coverageOutputGenerator" `
  -mergebyhash -register:user -oldStyle

ReportGenerator.exe -reports:"$coverageOutputRuntimeDnc21;$coverageOutputRuntimeNet471;$coverageOutputGenerator" -targetdir:"$coverageReportDir" -reportTypes:"Html;Xml"
