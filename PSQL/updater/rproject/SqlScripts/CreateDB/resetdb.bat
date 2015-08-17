@echo off
chcp 1251
"c:\Program Files\PostgreSQL\9.4\bin\PSQL.EXE" -f %~dp0\resetdb.sql %1_db postgres

