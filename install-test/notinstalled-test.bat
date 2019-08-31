@echo off
capnpc-sharp < NUL
if %ERRORLEVEL% neq 9009 exit /b 1
exit /b 0