;
drop function if exists sp_makeupdateinsert(data entityrowdatatable[], fieldtypes fieldinfodatatable[], tablename character varying, isdebug boolean);
drop table if exists fieldinfodatatable;
drop table if exists entityrowdatatable;
drop table if exists othertestentity;
drop table if exists testentity;