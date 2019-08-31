@echo off
capnp compile -ocsharp -I%cd% .\capnp\schema.capnp
if %ERRORLEVEL% neq 0 exit /b 1
if exist .\capnp\schema.cs (    
  del .\capnp\schema.cs
) else (
  exit /b 2
)
