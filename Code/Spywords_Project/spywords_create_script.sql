-- Table: phrase
CREATE TABLE phrase
(
  id serial NOT NULL,
  text character varying(512) not null,
  textbaseform character varying(512),
  status smallint not null,
  showsgoogle smallint,
  showsyandex smallint,
  advertisersgoogle smallint,
  advertisersyandex smallint,
  datecreated timestamp without time zone NOT NULL,
  datecollected timestamp without time zone,
  CONSTRAINT pk_phrase PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE phrase
  OWNER TO postgres;
GRANT ALL ON TABLE phrase TO postgres;
GRANT SELECT ON TABLE phrase TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE phrase TO writer;

-- Table: domain
CREATE TABLE domain
(
  id serial NOT NULL,
  domain character varying(256) not null,
  phrasesgoogle smallint,
  phrasesyandex smallint,
  advertsgoogle smallint,
  advertsyandex smallint,
  budgetgoogle smallint,
  budgetyandex smallint,
  visitsmonth smallint,
  datecreated timestamp without time zone NOT NULL,
  datecollected timestamp without time zone,
  status smallint not null,
  CONSTRAINT pk_domain PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE domain
  OWNER TO postgres;
GRANT ALL ON TABLE domain TO postgres;
GRANT SELECT ON TABLE domain TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE domain TO writer;

-- Table: domainphrase
CREATE TABLE domainphrase
(
  domainid integer not null,
  phraseid integer not null,
  CONSTRAINT fk_domainphrase_domain FOREIGN KEY (domainid)
      REFERENCES domain (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT fk_domainphrase_phrase FOREIGN KEY (phraseid)
      REFERENCES phrase (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE domainphrase
  OWNER TO postgres;
GRANT ALL ON TABLE domainphrase TO postgres;
GRANT SELECT ON TABLE domainphrase TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE domainphrase TO writer;

-- Table: domainphone
CREATE TABLE domainphone
(
  id serial NOT NULL,
  domainid integer NOT NULL,
  phone character varying(128) not null,
  datecreated timestamp without time zone NOT NULL,
  CONSTRAINT pk_domainphone PRIMARY KEY (id),
  CONSTRAINT fk_domainphone_domain FOREIGN KEY (domainid)
      REFERENCES domain (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE domainphone
  OWNER TO postgres;
GRANT ALL ON TABLE domainphone TO postgres;
GRANT SELECT ON TABLE domainphone TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE domainphone TO writer;


-- Table: domainemail
CREATE TABLE domainemail
(
  id serial NOT NULL,
  domainid integer NOT NULL,
  email character varying(256) not null,
  datecreated timestamp without time zone NOT NULL,
  CONSTRAINT pk_domainemail PRIMARY KEY (id),
  CONSTRAINT fk_domainemail_domain FOREIGN KEY (domainid)
      REFERENCES domain (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE domainemail
  OWNER TO postgres;
GRANT ALL ON TABLE domainemail TO postgres;
GRANT SELECT ON TABLE domainemail TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE domainemail TO writer;

-- Table: phraseaccount
CREATE TABLE phraseaccount
(
  id serial NOT NULL,
  accountidentityid integer NOT NULL,
  phraseid integer not null,
  datecreated timestamp without time zone NOT NULL,
  CONSTRAINT pk_phraseaccount PRIMARY KEY (id),
  CONSTRAINT fk_phraseaccount_accountidentity FOREIGN KEY (accountidentityid)
      REFERENCES accountidentity (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT fk_phraseaccount_phrase FOREIGN KEY (phraseid)
      REFERENCES phrase (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE phraseaccount
  OWNER TO postgres;
GRANT ALL ON TABLE phraseaccount TO postgres;
GRANT SELECT ON TABLE phraseaccount TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE phraseaccount TO writer;


alter table domain add column content text;
alter table domainphrase add column se smallint;
  
alter table phrase alter column Showsgoogle type integer;
alter table phrase alter column Showsyandex type integer;
alter table domain alter column phrasesgoogle type integer;
alter table domain alter column phrasesyandex type integer;
alter table domain alter column advertsgoogle type integer;
alter table domain alter column advertsyandex type integer;
alter table domain alter column budgetgoogle type integer;
alter table domain alter column budgetyandex type integer;
alter table domain alter column visitsmonth type integer;