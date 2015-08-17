
\set DBNAME '"' :DB_PREFIX '_db"' 

CREATE DATABASE :DBNAME
  WITH OWNER = postgres
	   ENCODING = 'UTF8'
	   TABLESPACE = pg_default
	   CONNECTION LIMIT = -1;
	   
GRANT CONNECT, TEMPORARY ON DATABASE :DBNAME TO public;

GRANT ALL ON DATABASE :DBNAME TO postgres;

CREATE ROLE writer
  NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE NOREPLICATION;

CREATE ROLE read_only LOGIN
	  ENCRYPTED PASSWORD 'md5cdd16028104048a84a4a1a9bfb37fca5'
	  NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE NOREPLICATION;
	  
CREATE ROLE "user" LOGIN
  ENCRYPTED PASSWORD 'md5fe644e76c1cbf2b8e7a2fe8f62a12963'
  NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE NOREPLICATION;
GRANT writer TO "user";

\c :DBNAME;

GRANT ALL ON SCHEMA public TO writer;	
REVOKE CREATE ON SCHEMA public FROM public;