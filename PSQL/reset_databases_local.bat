@echo off
call updater\rproject\sqlscripts\createDB\resetdb.bat %1
cd %~dp0\