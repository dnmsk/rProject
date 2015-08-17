;@echo off
chcp 1251
"c:\Program Files\PostgreSQL\9.4\bin\PSQL.EXE" -f d:\AutoRestore\Unpacked\rproject_db_backup.sql %1_db postgres