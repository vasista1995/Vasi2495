@echo off

call dotnet tool restore
call dotnet cake %*

REM Pause if not executed from console
echo %cmdcmdline% | findstr /i /c:"%~nx0" && set standalone=1
if defined standalone pause