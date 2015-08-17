--
-- PostgreSQL database dump
--

-- Dumped from database version 9.2.0
-- Dumped by pg_dump version 9.2.0
-- Started on 2013-08-09 12:51:47

SET statement_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

--
-- TOC entry 257 (class 3079 OID 11727)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- TOC entry 2517 (class 0 OID 0)
-- Dependencies: 257
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;



--
-- TOC entry 195 (class 1259 OID 133453)
-- Name: othertestentity; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE othertestentity (
    id bigint NOT NULL,
    name character varying(450) NOT NULL
);


ALTER TABLE public.othertestentity OWNER TO postgres;

--
-- TOC entry 196 (class 1259 OID 133456)
-- Name: othertestentity_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE othertestentity_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.othertestentity_id_seq OWNER TO postgres;

--
-- TOC entry 2597 (class 0 OID 0)
-- Dependencies: 196
-- Name: othertestentity_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE othertestentity_id_seq OWNED BY othertestentity.id;


--
-- TOC entry 2598 (class 0 OID 0)
-- Dependencies: 196
-- Name: othertestentity_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('othertestentity_id_seq', 1, false);



--
-- TOC entry 211 (class 1259 OID 133501)
-- Name: testentity; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE testentity (
    id bigint NOT NULL,
    name character varying(450) NOT NULL,
    enum integer
);


ALTER TABLE public.testentity OWNER TO postgres;

--
-- TOC entry 212 (class 1259 OID 133504)
-- Name: testentity_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE testentity_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.testentity_id_seq OWNER TO postgres;

--
-- TOC entry 2650 (class 0 OID 0)
-- Dependencies: 212
-- Name: testentity_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE testentity_id_seq OWNED BY testentity.id;


--
-- TOC entry 2651 (class 0 OID 0)
-- Dependencies: 212
-- Name: testentity_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('testentity_id_seq', 1, false);


--
-- TOC entry 2253 (class 2604 OID 133543)
-- Name: id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY othertestentity ALTER COLUMN id SET DEFAULT nextval('othertestentity_id_seq'::regclass);


--
-- TOC entry 2269 (class 2604 OID 133550)
-- Name: id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY testentity ALTER COLUMN id SET DEFAULT nextval('testentity_id_seq'::regclass);


--
-- TOC entry 2339 (class 2606 OID 133580)
-- Name: pk_othertestentity; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY othertestentity
    ADD CONSTRAINT pk_othertestentity PRIMARY KEY (id);


--
-- TOC entry 2357 (class 2606 OID 133596)
-- Name: pk_testentity; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY testentity
    ADD CONSTRAINT pk_testentity PRIMARY KEY (id);


--
-- TOC entry 2596 (class 0 OID 0)
-- Dependencies: 195
-- Name: othertestentity; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE othertestentity FROM PUBLIC;
REVOKE ALL ON TABLE othertestentity FROM postgres;
GRANT ALL ON TABLE othertestentity TO postgres;
GRANT SELECT ON TABLE othertestentity TO PUBLIC;
GRANT INSERT,DELETE,UPDATE ON TABLE othertestentity TO writer;


--
-- TOC entry 2599 (class 0 OID 0)
-- Dependencies: 196
-- Name: othertestentity_id_seq; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON SEQUENCE othertestentity_id_seq FROM PUBLIC;
REVOKE ALL ON SEQUENCE othertestentity_id_seq FROM postgres;
GRANT ALL ON SEQUENCE othertestentity_id_seq TO postgres;
GRANT SELECT,USAGE ON SEQUENCE othertestentity_id_seq TO PUBLIC;


--
-- TOC entry 2649 (class 0 OID 0)
-- Dependencies: 211
-- Name: testentity; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE testentity FROM PUBLIC;
REVOKE ALL ON TABLE testentity FROM postgres;
GRANT ALL ON TABLE testentity TO postgres;
GRANT SELECT ON TABLE testentity TO PUBLIC;
GRANT INSERT,DELETE,UPDATE ON TABLE testentity TO writer;


--
-- TOC entry 2652 (class 0 OID 0)
-- Dependencies: 212
-- Name: testentity_id_seq; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON SEQUENCE testentity_id_seq FROM PUBLIC;
REVOKE ALL ON SEQUENCE testentity_id_seq FROM postgres;
GRANT ALL ON SEQUENCE testentity_id_seq TO postgres;
GRANT SELECT,USAGE ON SEQUENCE testentity_id_seq TO PUBLIC;


--
-- TOC entry 169 (class 1259 OID 133334)
-- Name: schemachanges; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE schemachanges (
    schemachangeid bigint DEFAULT nextval('hibernate_sequence'::regclass) NOT NULL,
    majorreleasenumber character varying(2) NOT NULL,
    minorreleasenumber character varying(2) NOT NULL,
    pointreleasenumber character varying(4) NOT NULL,
    scriptname character varying(250) NOT NULL,
    dateapplied timestamp without time zone NOT NULL
);


ALTER TABLE public.schemachanges OWNER TO postgres;

--
-- TOC entry 2306 (class 2606 OID 133339)
-- Name: pk_schemachangelog; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY schemachanges
    ADD CONSTRAINT pk_schemachangelog PRIMARY KEY (schemachangeid);
	

--
-- TOC entry 2629 (class 0 OID 0)
-- Dependencies: 169
-- Name: schemachanges; Type: ACL; Schema: public; Owner: postgres
--


REVOKE ALL ON TABLE schemachanges FROM PUBLIC;
REVOKE ALL ON TABLE schemachanges FROM postgres;
GRANT ALL ON TABLE schemachanges TO postgres;
GRANT INSERT,DELETE,UPDATE ON TABLE schemachanges TO writer;
GRANT SELECT ON TABLE schemachanges TO PUBLIC;


CREATE TABLE entityrowdatatable (
    entityid bigint NOT NULL,
    fieldinfoid smallint NOT NULL,
    value character varying
);


ALTER TABLE public.entityrowdatatable OWNER TO postgres;

GRANT ALL ON TABLE public.entityrowdatatable TO postgres;
GRANT SELECT ON TABLE public.entityrowdatatable TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE public.entityrowdatatable TO writer;

--
-- TOC entry 197 (class 1259 OID 17163)
-- Name: fieldinfodatatable; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE fieldinfodatatable (
    id smallint NOT NULL,
    fieldname character varying NOT NULL,
    fieldtype character varying NOT NULL,
    isidfield boolean NOT NULL
);


ALTER TABLE public.fieldinfodatatable OWNER TO postgres;

GRANT ALL ON TABLE public.fieldinfodatatable TO postgres;
GRANT SELECT ON TABLE public.fieldinfodatatable TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE public.fieldinfodatatable TO writer;

--
-- TOC entry 336 (class 1255 OID 17169)
-- Name: sp_makeupdateinsert(entityrowdatatable[], fieldinfodatatable[], character varying, boolean); Type: FUNCTION; Schema: public; Owner: postgres
--
CREATE OR REPLACE FUNCTION sp_makeupdateinsert(data entityrowdatatable[], fieldtypes fieldinfodatatable[], tablename character varying, isdebug boolean DEFAULT (0)::boolean)
  RETURNS SETOF bigint AS
$BODY$
/*
select * from sp_MakeUpdateInsert(data:=((E'{}')::EntityRowDataTable[]),fieldTypes:=((E'{}')::FieldInfoDataTable[]),tableName:=((E'PageDonor')::text), isDebug := true)

select * from sp_MakeUpdateInsert(data:=((array[row(-1,1,NULL),row(-1,2,'27226'),row(-1,3,'forum.rubcovsk.ru/showthread.php?s=3c5146d52240876e2f7b4aa8e5210d16&t=8256&page=8'),row(-1,4,'0'),row(-1,5,NULL),row(-1,6,NULL),row(-1,7,NULL),row(-1,8,NULL),row(-1,9,NULL),row(-1,10,NULL),row(-1,11,NULL),row(-1,12,NULL),row(-1,13,NULL),row(-1,14,NULL),row(-1,15,NULL),row(-1,16,'XML'),row(-2,1,NULL),row(-2,2,'27226'),row(-2,3,'forum.rubcovsk.ru/showthread.php?s=6a4854f2abea703af1c9fa04cf75b735&p=156242'),row(-2,4,'0'),row(-2,5,NULL),row(-2,6,NULL),row(-2,7,NULL),row(-2,8,NULL),row(-2,9,NULL),row(-2,10,NULL),row(-2,11,NULL),row(-2,12,NULL),row(-2,13,NULL),row(-2,14,NULL),row(-2,15,NULL),row(-2,16,'XML'),row(-3,1,NULL),row(-3,2,'27226'),row(-3,3,'forum.rubcovsk.ru/showthread.php?s=459c9461742da322ca04d991650d571a&t=8238&page=2'),row(-3,4,'0'),row(-3,5,NULL),row(-3,6,NULL),row(-3,7,NULL),row(-3,8,NULL),row(-3,9,NULL),row(-3,10,NULL),row(-3,11,NULL),row(-3,12,NULL),row(-3,13,NULL),row(-3,14,NULL),row(-3,15,NULL),row(-3,16,'XML'),row(-4,1,NULL),row(-4,2,'27226'),row(-4,3,'forum.rubcovsk.ru/tags.php?tag=%d0%b7%d0%bd%d0%b0%d0%bc%d0%b5%d0%bd%d0%b8%d1%82%d0%be%d1%81%d1%82%d0%b8'),row(-4,4,'0'),row(-4,5,NULL),row(-4,6,NULL),row(-4,7,NULL),row(-4,8,NULL),row(-4,9,NULL),row(-4,10,NULL),row(-4,11,NULL),row(-4,12,NULL),row(-4,13,NULL),row(-4,14,NULL),row(-4,15,NULL),row(-4,16,'XML'),row(-5,1,NULL),row(-5,2,'27226'),row(-5,3,'forum.rubcovsk.ru/showthread.php?s=385901eb95224173a41db5c9abb83f20&t=8574&page=13'),row(-5,4,'0'),row(-5,5,NULL),row(-5,6,NULL),row(-5,7,NULL),row(-5,8,NULL),row(-5,9,NULL),row(-5,10,NULL),row(-5,11,NULL),row(-5,12,NULL),row(-5,13,NULL),row(-5,14,NULL),row(-5,15,NULL),row(-5,16,'XML'),row(-6,1,NULL),row(-6,2,'27226'),row(-6,3,'forum.rubcovsk.ru/archive/index.php/f-313.html'),row(-6,4,'0'),row(-6,5,NULL),row(-6,6,NULL),row(-6,7,NULL),row(-6,8,NULL),row(-6,9,NULL),row(-6,10,NULL),row(-6,11,NULL),row(-6,12,NULL),row(-6,13,NULL),row(-6,14,NULL),row(-6,15,NULL),row(-6,16,'XML'),row(-7,1,NULL),row(-7,2,'27226'),row(-7,3,'forum.rubcovsk.ru/showthread.php?t=13750'),row(-7,4,'0'),row(-7,5,NULL),row(-7,6,NULL),row(-7,7,NULL),row(-7,8,NULL),row(-7,9,NULL),row(-7,10,NULL),row(-7,11,NULL),row(-7,12,NULL),row(-7,13,NULL),row(-7,14,NULL),row(-7,15,NULL),row(-7,16,'XML'),row(-8,1,NULL),row(-8,2,'27226'),row(-8,3,'forum.rubcovsk.ru/tags.php?tag=%d0%b3%d0%be%d1%80%d0%be%d0%b4'),row(-8,4,'0'),row(-8,5,NULL),row(-8,6,NULL),row(-8,7,NULL),row(-8,8,NULL),row(-8,9,NULL),row(-8,10,NULL),row(-8,11,NULL),row(-8,12,NULL),row(-8,13,NULL),row(-8,14,NULL),row(-8,15,NULL),row(-8,16,'XML'),row(-9,1,NULL),row(-9,2,'27226'),row(-9,3,'forum.rubcovsk.ru/showthread.php?t=13717'),row(-9,4,'0'),row(-9,5,NULL),row(-9,6,NULL),row(-9,7,NULL),row(-9,8,NULL),row(-9,9,NULL),row(-9,10,NULL),row(-9,11,NULL),row(-9,12,NULL),row(-9,13,NULL),row(-9,14,NULL),row(-9,15,NULL),row(-9,16,'XML'),row(-10,1,NULL),row(-10,2,'27226'),row(-10,3,'forum.rubcovsk.ru/archive/index.php/t-6900.html'),row(-10,4,'0'),row(-10,5,NULL),row(-10,6,NULL),row(-10,7,NULL),row(-10,8,NULL),row(-10,9,NULL),row(-10,10,NULL),row(-10,11,NULL),row(-10,12,NULL),row(-10,13,NULL),row(-10,14,NULL),row(-10,15,NULL),row(-10,16,'XML'),row(-11,1,NULL),row(-11,2,'27226'),row(-11,3,'forum.rubcovsk.ru/showthread.php?t=7806'),row(-11,4,'0'),row(-11,5,NULL),row(-11,6,NULL),row(-11,7,NULL),row(-11,8,NULL),row(-11,9,NULL),row(-11,10,NULL),row(-11,11,NULL),row(-11,12,NULL),row(-11,13,NULL),row(-11,14,NULL),row(-11,15,NULL),row(-11,16,'XML'),row(-12,1,NULL),row(-12,2,'27226'),row(-12,3,'forum.rubcovsk.ru/member.php?u=134'),row(-12,4,'0'),row(-12,5,NULL),row(-12,6,NULL),row(-12,7,NULL),row(-12,8,NULL),row(-12,9,NULL),row(-12,10,NULL),row(-12,11,NULL),row(-12,12,NULL),row(-12,13,NULL),row(-12,14,NULL),row(-12,15,NULL),row(-12,16,'XML'),row(-13,1,NULL),row(-13,2,'27226'),row(-13,3,'forum.rubcovsk.ru/archive/index.php/t-7806.html'),row(-13,4,'0'),row(-13,5,NULL),row(-13,6,NULL),row(-13,7,NULL),row(-13,8,NULL),row(-13,9,NULL),row(-13,10,NULL),row(-13,11,NULL),row(-13,12,NULL),row(-13,13,NULL),row(-13,14,NULL),row(-13,15,NULL),row(-13,16,'XML')])::EntityRowDataTable[]),
fieldTypes:=((array[row(1,'ID','bigint',TRUE),row(2,'SiteID','bigint',FALSE),row(3,'Url','varchar',FALSE),row(4,'Status','smallint',FALSE),row(5,'IndexDate','timestamp without time zone',FALSE),row(6,'WrongIndexDate','timestamp without time zone',FALSE),row(7,'ResponseTime','int',FALSE),row(8,'PageSize','int',FALSE),row(9,'IntLinkCount','int',FALSE),row(10,'ExtLinkCount','int',FALSE),row(11,'IsDisabled','boolean',FALSE),row(12,'Nesting','smallint',FALSE),row(13,'Pr','smallint',FALSE),row(14,'DailyPrice','decimal(20, 6)',FALSE),row(15,'NewDailyPrice','decimal(20, 6)',FALSE),row(16,'Creator','varchar',FALSE)])::FieldInfoDataTable[]),
	tableName:=((E'PageDonor')::text), isDebug := true)



CREATE TABLE test
(
  id bigserial NOT NULL,
  test_field character varying,
  test_field1 character varying
)
WITH (
  OIDS=FALSE
);
ALTER TABLE test
  OWNER TO postgres;

select * from sp_MakeUpdateInsert((array[(1, 2, 'test11')::EntityRowDataTable,
				(1, 3, 'test11')::EntityRowDataTable,
				(-1, 2, 'test_insert1')::EntityRowDataTable,
				(-1, 3, 'test_insert1')::EntityRowDataTable,
				(-2, 2, 'test1_insert2')::EntityRowDataTable,
				(2, 2, 'test22')::EntityRowDataTable]),

				(array[
				(1,'id','bigint', 1::boolean)::FieldInfoDataTable,		
				(2, 'test_field', 'varchar', 0::boolean)::FieldInfoDataTable,	
				(3, 'test_field1', 'varchar', 0::boolean)::FieldInfoDataTable
				]),		
				
				'test',true)
*/
declare
rec record;
keyFieldName varchar;

updateFieldsStatement varchar;
updateSubSelectStatement varchar;

insertFields varchar;
insertFieldsStatement varchar;

sqlUpdate varchar;
sqlInsert varchar;
begin
SET DateStyle TO ISO,MDY;
	 select  ft.FieldName into keyFieldName 
		from (select * from unnest(
		fieldTypes
		)) as ft
		where ft.IsIDField = 1::boolean limit 1; 
		--raise notice '%',keyFieldName;


	for rec in select *
		from (
			select
					tmp.ID,
					FieldName,
					'cast(max(case when result.FieldInfoID = ' || CAST(tmp.ID as varchar) || ' then result.Value else null end) as ' || FieldType || ')' as statementField
				from (select * from unnest(
				fieldTypes
				)
				/*where IsIDField = 0::boolean*/) tmp
			) as t
	loop
		updateFieldsStatement := case when rec.FieldName <> keyFieldName 
						then coalesce(updateFieldsStatement || ',', '') || '
						' || rec.FieldName || ' = r.' || rec.FieldName
						else updateFieldsStatement
						end ;
		updateSubSelectStatement := case when rec.FieldName <> keyFieldName 
						then coalesce(updateSubSelectStatement || ',', '') || '
									' || rec.statementField || ' as ' || rec.FieldName
						else updateSubSelectStatement
						end;
		insertFieldsStatement := case when rec.FieldName <> keyFieldName 
						then coalesce(insertFieldsStatement || ',', '') || '
						' || rec.statementField
						else insertFieldsStatement
						end;
		insertFields := case when rec.FieldName <> keyFieldName 
						then coalesce(insertFields || ',', '') || rec.FieldName
						else insertFields
						end;
	END LOOP;
		--raise notice '%','updateFieldsStatement';
		--raise notice '%',updateFieldsStatement;
		--raise notice '%','updateSubSelectStatement';
		--raise notice '%',updateSubSelectStatement;
		--raise notice '%','insertFieldsStatement';
		--raise notice '%',insertFieldsStatement;
		--raise notice '%','insertFields';
		--raise notice '%',insertFields;





	sqlUpdate := '
	update ' || tableName || ' t 
		set ' || updateFieldsStatement || '
			from  (
					select 
							EntityID, ' ||
							updateSubSelectStatement || '
						from (select * from unnest($1)) as result
							where result.EntityID > 0
							group by result.EntityID
				) as r
					where (r.EntityID = t.' || keyFieldName || ');';
	
	
	if(isDebug = true) then
		raise notice '%','sqlUpdate';
		raise notice '%',sqlUpdate;
	end if;

	create temporary table tmp_id(id bigint) on commit drop;
	
	
	sqlInsert := '
	with insert_rows as (
		
		insert into ' || tableName || ' 
				(' || insertFields || ')
		
			select ' || insertFieldsStatement || '
				from (select * from unnest($1)) as result 
			where result.EntityID < 0
			group by result.EntityID
			order by result.EntityID desc
			returning '||tableName||'.id)
		
		insert into tmp_id (id) select insert_rows.id::bigint from insert_rows;
		';
     
	if(isDebug = true) then
		raise notice '%','sqlInsert';
		raise notice '%',sqlInsert;
	end if;
	
        
	-- raise often when the table is null
	if( data <> ((E'{}'))) then
	-- raise often when the table is null
	raise notice'123';
		execute format(sqlUpdate)
			using data;
     
		execute format(sqlInsert)
			using data;
	end if;

     return query select tmp_id.id from tmp_id;
	


end;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION sp_makeupdateinsert(entityrowdatatable[], fieldinfodatatable[], character varying, boolean)
  OWNER TO postgres;

  
INSERT INTO SchemaChanges
	   (MajorReleaseNumber
	   ,MinorReleaseNumber
	   ,PointReleaseNumber
	   ,ScriptName
	   ,DateApplied)
VALUES
	   ('01'
	   ,'01'
	   ,'0000'
	   ,'baseline after 15/08/2015'
	   ,current_timestamp); 