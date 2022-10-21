create schema public;

comment on schema public is 'standard public schema';

alter schema public owner to postgres;

create type buyln_type as
(
    stockid integer,
    name varchar(12),
    wareid smallint,
    price money,
    qty smallint,
    qtyre smallint
);

alter type buyln_type owner to postgres;

create table entities
(
    typ smallint not null,
    status smallint default 0 not null,
    name varchar(12) not null,
    tip varchar(30),
    created timestamp(0),
    creator varchar(10),
    adapted timestamp(0),
    adapter varchar(10),
    oked timestamp(0),
    oker varchar(10),
    state smallint
);

alter table entities owner to postgres;

create table dailys
(
    orgid integer,
    dt date,
    itemid smallint,
    count integer,
    amt money,
    qty integer
)
    inherits (entities);

alter table dailys owner to postgres;

create table ledgers_
(
    seq integer,
    acct varchar(20),
    name varchar(12),
    amt integer,
    bal integer,
    cs uuid,
    blockcs uuid,
    stamp timestamp(0)
);

alter table ledgers_ owner to postgres;

create table peerledgs_
(
    peerid smallint
)
    inherits (ledgers_);

alter table peerledgs_ owner to postgres;

create table users
(
    id serial not null
        constraint users_pk
            primary key,
    tel varchar(11) not null,
    addr varchar(50),
    im varchar(28),
    credential varchar(32),
    admly smallint default 0 not null,
    orgid smallint,
    orgly smallint default 0 not null,
    idcard varchar(18),
    icon bytea
)
    inherits (entities);

alter table users owner to postgres;

create table tests
(
    id serial not null
        constraint tests_pk
            primary key,
    orgid integer,
    level integer
)
    inherits (entities);

alter table tests owner to postgres;

create table stocks
(
    id serial not null
        constraint stocks_pk
            primary key,
    shpid integer,
    itemid integer,
    unit varchar(4),
    unitstd varchar(4),
    unitx money,
    price money,
    "off" money,
    min smallint,
    max smallint,
    step smallint,
    icon bytea,
    pic bytea
)
    inherits (entities);

alter table stocks owner to postgres;

create table regs
(
    id smallint not null,
    idx smallint,
    num smallint
)
    inherits (entities);

alter table regs owner to postgres;

create table peers_
(
    id smallint not null
        constraint peers__pk
            primary key,
    weburl varchar(50),
    secret varchar(16)
)
    inherits (entities);

alter table peers_ owner to postgres;

create table orgs
(
    id serial not null
        constraint orgs_pk
            primary key,
    prtid integer,
    ctrid integer,
    license varchar(20),
    trust boolean,
    regid smallint,
    addr varchar(30),
    x double precision,
    y double precision,
    tel varchar(11),
    link varchar(100),
    sprid integer,
    rvrid integer,
    icon bytea
)
    inherits (entities);

alter table orgs owner to postgres;

create table marks
(
    id smallserial not null
        constraint marks_pk
            primary key,
    icon bytea
)
    inherits (entities);

alter table marks owner to postgres;

create table lots
(
    id serial not null
        constraint lots_pk
            primary key,
    itemid integer,
    srcid integer,
    ctrid integer,
    ctring boolean,
    price money,
    "off" money,
    cap integer,
    remain integer,
    min integer,
    max integer,
    step integer,
    nstart integer,
    nend integer
)
    inherits (entities);

alter table lots owner to postgres;

create table items
(
    id serial not null
        constraint items_pk
            primary key,
    srcid integer,
    store smallint,
    duration smallint,
    agt boolean,
    unit varchar(4),
    unitpkg varchar(4),
    unitx smallint[],
    icon bytea,
    pic bytea,
    m1 bytea,
    m2 bytea,
    m3 bytea,
    m4 bytea
)
    inherits (entities);

alter table items owner to postgres;

create table clears
(
    id serial not null,
    dt date,
    orgid integer not null,
    sprid integer not null,
    orders integer,
    total money,
    rate money,
    pay integer
)
    inherits (entities);

alter table clears owner to postgres;

create table cats
(
    idx smallint,
    size smallint,
    constraint cats_pk
        primary key (typ)
)
    inherits (entities);

alter table cats owner to postgres;

create table buys
(
    id bigserial not null
        constraint buys_pk
            primary key,
    shpid integer not null,
    mktid integer not null,
    uid integer not null,
    uname varchar(10),
    utel varchar(11),
    uaddr varchar(20),
    uim varchar(28),
    lines buyln_type[],
    fee money,
    pay money,
    refund money
)
    inherits (entities);

alter table buys owner to postgres;

create table books
(
    id bigserial not null
        constraint books_pk
            primary key,
    shpid integer not null,
    mktid integer not null,
    ctrid integer not null,
    srcid integer not null,
    zonid integer not null,
    itemid integer,
    lotid integer,
    unit varchar(4),
    unitx smallint,
    unitpkg varchar(4),
    price money,
    "off" money,
    qty integer,
    cut integer,
    pay money,
    refund money
)
    inherits (entities);

alter table books owner to postgres;

create table accts_
(
    no varchar(20),
    v integer
)
    inherits (entities);

alter table accts_ owner to postgres;

create view orgs_vw(typ, status, name, tip, created, creator, adapted, adapter, oker, oked, state, id, prtid, ctrid, license, trust, regid, addr, x, y, tel, link, sprid, sprname, sprtel, sprim, rvrid, icon) as
SELECT o.typ,
       o.status,
       o.name,
       o.tip,
       o.created,
       o.creator,
       o.adapted,
       o.adapter,
       o.oker,
       o.oked,
       o.state,
       o.id,
       o.prtid,
       o.ctrid,
       o.license,
       o.trust,
       o.regid,
       o.addr,
       o.x,
       o.y,
       o.tel,
       o.link,
       o.sprid,
       m.name             AS sprname,
       m.tel              AS sprtel,
       m.im               AS sprim,
       o.rvrid,
       o.icon IS NOT NULL AS icon
FROM orgs o
         LEFT JOIN users m
                   ON o.sprid =
                      m.id;

alter table orgs_vw owner to postgres;

create view users_vw(typ, status, name, tip, created, creator, adapted, adapter, oked, oker, state, id, tel, addr, im, credential, admly, orgid, orgly, idcard, icon) as
SELECT u.typ,
       u.status,
       u.name,
       u.tip,
       u.created,
       u.creator,
       u.adapted,
       u.adapter,
       u.oked,
       u.oker,
       u.state,
       u.id,
       u.tel,
       u.addr,
       u.im,
       u.credential,
       u.admly,
       u.orgid,
       u.orgly,
       u.idcard,
       u.icon IS NOT NULL AS icon
FROM users u;

alter table users_vw owner to postgres;

create function first_agg(anyelement, anyelement) returns anyelement
    immutable
    strict
    parallel safe
    language sql
as $$
SELECT $1
$$;

alter function first_agg(anyelement, anyelement) owner to postgres;

create function last_agg(anyelement, anyelement) returns anyelement
    immutable
    strict
    parallel safe
    language sql
as $$
SELECT $2
$$;

alter function last_agg(anyelement, anyelement) owner to postgres;

create aggregate first(anyelement) (
    sfunc = first_agg,
    stype = anyelement,
    parallel = safe
    );

alter aggregate first(anyelement) owner to postgres;

create aggregate last(anyelement) (
    sfunc = last_agg,
    stype = anyelement,
    parallel = safe
    );

alter aggregate last(anyelement) owner to postgres;

