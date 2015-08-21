--guest
CREATE TABLE guest (
	id 	   	 serial NOT NULL,
	datecreated      timestamp without time zone not null,
	ip 		 character varying(15),
	CONSTRAINT pk_guest PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE guest
  OWNER TO postgres;
GRANT ALL ON TABLE guest TO postgres;
GRANT SELECT ON TABLE guest TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE guest TO writer;

--guestreferrer
CREATE TABLE guestreferrer (
  id serial NOT NULL,
  guestid bigint NOT NULL,
  datecreated timestamp without time zone NOT NULL,
  urlreferrer character varying(512),
  urltarget character varying(512),
  CONSTRAINT pk_guestreferrer PRIMARY KEY (id),
  CONSTRAINT fk_guestreferrer_guest FOREIGN KEY (guestid)
      REFERENCES guest (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE guestreferrer
  OWNER TO postgres;
GRANT ALL ON TABLE guestreferrer TO postgres;
GRANT SELECT ON TABLE guestreferrer TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE guestreferrer TO writer;

--GuestExistsBrowser
CREATE TABLE GuestExistsBrowser
(
  id serial NOT NULL,
  browsertype character varying(128),
  version numeric(4,2),
  os character varying(128),
  ismobile boolean,
  isbot boolean,
  useragent character varying(256),
  datecreated timestamp without time zone NOT NULL,
  CONSTRAINT pk_GuestExistsBrowser PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE GuestExistsBrowser
  OWNER TO postgres;
GRANT ALL ON TABLE GuestExistsBrowser TO postgres;
GRANT SELECT ON TABLE GuestExistsBrowser TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE GuestExistsBrowser TO writer;

--GuestTechInfo
CREATE TABLE GuestTechInfo
(
  id serial NOT NULL,
  guestid bigint not null,
  guestexistsbrowserid integer not null,
  datecreated timestamp without time zone NOT NULL,
  CONSTRAINT pk_GuestTechInfo PRIMARY KEY (id),
  CONSTRAINT fk_GuestTechInfo_guest FOREIGN KEY (guestid)
      REFERENCES guest (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT fk_GuestTechInfo_GuestExistsBrowser FOREIGN KEY (GuestExistsBrowserid)
      REFERENCES GuestExistsBrowser (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE GuestTechInfo
  OWNER TO postgres;
GRANT ALL ON TABLE GuestTechInfo TO postgres;
GRANT SELECT ON TABLE GuestTechInfo TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE GuestTechInfo TO writer;


--UtmGuestReferrer
CREATE TABLE utmguestreferrer
(
  id serial NOT NULL,
  guestid bigint not null,
  campaign character varying(128),
  medium character varying(128),
  source character varying(128),
  datecreated timestamp without time zone NOT NULL,
  CONSTRAINT pk_utmguestreferrer PRIMARY KEY (id),
  CONSTRAINT fk_utmguestreferrer_guest FOREIGN KEY (guestid)
      REFERENCES guest (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE utmguestreferrer
  OWNER TO postgres;
GRANT ALL ON TABLE utmguestreferrer TO postgres;
GRANT SELECT ON TABLE utmguestreferrer TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE utmguestreferrer TO writer;


--utmsubdomainrule
CREATE TABLE utmsubdomainrule
(
  id serial NOT NULL,
  subdomainname character varying(128),
  targetdomain character varying(128),
  datecreated timestamp without time zone NOT NULL default now(),
  CONSTRAINT pk_utmsubdomainrule PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE utmsubdomainrule
  OWNER TO postgres;
GRANT ALL ON TABLE utmsubdomainrule TO postgres;
GRANT SELECT ON TABLE utmsubdomainrule TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE utmsubdomainrule TO writer;


--guestactionlog
CREATE TABLE guestactionlog
(
  id serial NOT NULL,
  guestid bigint not null,
  utmsubdomainruleid integer not null,
  action integer not null,
  arg	 integer,
  datecreated timestamp without time zone NOT NULL,
  CONSTRAINT pk_guestactionlog PRIMARY KEY (id),
  CONSTRAINT fk_guestactionlog_guest FOREIGN KEY (guestid)
      REFERENCES guest (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT fk_guestactionlog_utmsubdomainrule FOREIGN KEY (utmsubdomainruleid)
      REFERENCES utmsubdomainrule (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE guestactionlog
  OWNER TO postgres;
GRANT ALL ON TABLE guestactionlog TO postgres;
GRANT SELECT ON TABLE guestactionlog TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE guestactionlog TO writer;
