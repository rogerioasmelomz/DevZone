@echo off
call nant -f:IntegrationBuild.build -D:ENV.CloverNETFolder="C:\Program Files\Cenqua\Clover.NET 2.1 for .NET 2.0" 
if %ERRORLEVEL% NEQ 0 pause