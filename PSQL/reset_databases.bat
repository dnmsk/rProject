@echo off
if not "%BUILD_IS_PERSONAL%"=="true" goto :notpersonal
call reset_databases_local.bat %1
goto :exit
:notpersonal
echo Build is not personal, skipping reset databases step
:exit
cd %~dp0\