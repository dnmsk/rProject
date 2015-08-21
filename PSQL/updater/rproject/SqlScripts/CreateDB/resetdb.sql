;
drop table if exists UtmGuestReferrer;
drop table if exists GuestReferrer;
drop table if exists GuestActionLog;
drop table if exists GuestTechInfo;
drop table if exists GuestExistsBrowser;
drop table if exists UtmSubdomainRule;
drop table if exists Guest;

drop function if exists sp_makeupdateinsert(data entityrowdatatable[], fieldtypes fieldinfodatatable[], tablename character varying, isdebug boolean);
drop table if exists fieldinfodatatable;
drop table if exists entityrowdatatable;
drop table if exists othertestentity;
drop table if exists testentity;
drop table if exists schemachanges;