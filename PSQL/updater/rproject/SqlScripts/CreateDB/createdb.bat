;@echo off
chcp 1251
"c:\Program Files\PostgreSQL\9.4\bin\PSQL.EXE" -v DB_PREFIX=%1 -f %~dp0\createdb.sql postgres postgres

