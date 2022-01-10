$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = "$scriptDir\.."
$resultsDir = "$rootDir\TestResults"
$coverageFile = "$resultsDir\*\*.xml"
$testResultsDir = "$rootDir\TestResults"
$coverageReportDir = "$rootDir\coverage"
$generatorTests = "$rootDir\CapnpC.CSharp.Generator.Tests\CapnpC.CSharp.Generator.Tests.csproj"
$runtimeTests = "$rootDir\Capnp.Net.Runtime.Tests\Capnp.Net.Runtime.Tests.csproj"

If(test-path $testResultsDir) {
  Remove-Item -Recurse -Force $testResultsDir
}

If(!(test-path $coverageReportDir)) {
  New-Item -ItemType Directory -Force -Path $coverageReportDir
}

& dotnet test $generatorTests `
  --filter TestCategory=Coverage `
  --logger console `
  --configuration Release `
  --framework net6 `
  --collect:"XPlat code coverage" `
  --results-directory $resultsDir `
  --settings "$rootDir\coverlet.runsettings"

& dotnet test $runtimeTests `
  --filter TestCategory=Coverage `
  --logger console `
  --configuration Release `
  --framework net6 `
  --collect:"XPlat code coverage" `
  --results-directory $resultsDir `
  --settings "$rootDir\coverlet.runsettings"

  ReportGenerator.exe -reports:"$coverageFile" -targetdir:"$coverageReportDir" -reportTypes:"Html;lcov"
