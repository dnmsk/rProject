
do  $$
begin
	if not exists (select 1 from pg_class where relname='hibernate_sequence') 
	then
		CREATE SEQUENCE hibernate_sequence
		  INCREMENT 1
		  MINVALUE 1
		  MAXVALUE 9223372036854775807
		  START 8
		  CACHE 1;
		ALTER SEQUENCE hibernate_sequence
		  OWNER TO postgres;
	end if;	
	
	if not exists (select 1 from pg_tables where tablename='schemachanges') 
	then
		CREATE TABLE SchemaChanges(
		   SchemaChangeID bigint NOT NULL DEFAULT nextval('hibernate_sequence'::regclass),
		   MajorReleaseNumber varchar(2) NOT NULL,
		   MinorReleaseNumber varchar(2) NOT NULL,
		   PointReleaseNumber varchar(4) NOT NULL,
		   ScriptName varchar(250) NOT NULL,
		   DateApplied timestamp NOT NULL,
		
			CONSTRAINT PK_SchemaChangeLog
				PRIMARY KEY (SchemaChangeID)
		);
		GRANT SELECT ON TABLE schemachanges TO public;
		GRANT UPDATE, INSERT, DELETE ON TABLE schemachanges TO GROUP writer;		
		GRANT SELECT, USAGE ON SEQUENCE public.hibernate_sequence TO public;			
	end if;
end$$;

