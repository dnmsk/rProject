@echo off
call updater\rproject\sqlscripts\createDB\createdb.bat %1
cd %~dp0\