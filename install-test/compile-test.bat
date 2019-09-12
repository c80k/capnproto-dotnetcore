@echo off
capnp compile -ocsharp -I%cd% .\capnp\schema.capnp
if %ERRORLEVEL% neq 0 exit /b 1
if exist .\capnp\schema.capnp.cs (    
  del .\capnp\schema.capnp.cs
) else (
  exit /b 2
)
