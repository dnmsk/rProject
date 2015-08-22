alter table guestreferrer alter column guestid type integer;
alter table GuestTechInfo alter column guestid type integer;
alter table utmguestreferrer alter column guestid type integer;
alter table guestactionlog alter column guestid type integer;

INSERT INTO guest(
            id, datecreated, ip)
    VALUES (-1, '2001-01-01', 'localhost');
