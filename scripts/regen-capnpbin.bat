@echo off
cd "%~dp0\..\CapnpC.CSharp.Generator.Tests\No Resources"
for /f %%f in ('dir /b "*.capnp"') do capnp compile -o- %%f -I"..\..\include" > "..\Embedded Resources\%%f.bin"