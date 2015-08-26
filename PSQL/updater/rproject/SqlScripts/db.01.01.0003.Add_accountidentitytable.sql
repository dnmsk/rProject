--AccountIdentity
CREATE TABLE AccountIdentity
(
  id serial NOT NULL,
  guestid bigint not null,
    email character varying(256),
    password character varying(256),
    name character varying(100),
    fbid bigint,
    vkid bigint,
    okid bigint,
    vktoken character varying(128),
    oktoken character varying(128),
    fbtoken character varying(128),
    phone character varying(15),
  datecreated timestamp without time zone NOT NULL,
  datelastlogin timestamp without time zone,
  CONSTRAINT pk_AccountIdentity PRIMARY KEY (id),
  CONSTRAINT fk_AccountIdentity_guest FOREIGN KEY (guestid)
      REFERENCES guest (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE AccountIdentity
  OWNER TO postgres;
GRANT ALL ON TABLE AccountIdentity TO postgres;
GRANT SELECT ON TABLE AccountIdentity TO public;
GRANT UPDATE, INSERT, DELETE ON TABLE AccountIdentity TO writer;