rem This batch file assumes to be run from a Visual Studio developer command prompt
rem This is necessary because we need to locate vstest.console.exe

powershell -File measure-coverage.ps1
