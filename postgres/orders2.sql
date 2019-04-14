--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.0
-- Dumped by pg_dump version 9.6.8

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: archivedorderslogbuffer; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.archivedorderslogbuffer (
    archivedlogbufferid integer NOT NULL,
    archivedtime timestamp without time zone NOT NULL,
    orderid integer NOT NULL
);


ALTER TABLE public.archivedorderslogbuffer OWNER TO "Tonyx";

--
-- Name: archivedorderslog_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.archivedorderslog_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.archivedorderslog_id_seq OWNER TO "Tonyx";

--
-- Name: archivedorderslog_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.archivedorderslog_id_seq OWNED BY public.archivedorderslogbuffer.archivedlogbufferid;


--
-- Name: coursecategories; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.coursecategories (
    categoryid integer NOT NULL,
    name character varying(50) NOT NULL,
    visibile boolean DEFAULT true,
    abstract boolean NOT NULL
);


ALTER TABLE public.coursecategories OWNER TO "Tonyx";

--
-- Name: courses; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.courses (
    courseid integer NOT NULL,
    name character varying(120) NOT NULL,
    description character varying(4000),
    price numeric(10,2) DEFAULT 0,
    categoryid integer NOT NULL,
    visibility boolean NOT NULL
);


ALTER TABLE public.courses OWNER TO "Tonyx";

--
-- Name: coursedetails2; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.coursedetails2 AS
 SELECT a.courseid,
    a.name,
    a.description,
    a.price,
    c.name AS coursecategoryname,
    c.categoryid,
    a.visibility
   FROM (public.courses a
     JOIN public.coursecategories c ON ((a.categoryid = c.categoryid)))
  ORDER BY a.courseid;


ALTER TABLE public.coursedetails2 OWNER TO "Tonyx";

--
-- Name: courses_categoryid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.courses_categoryid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.courses_categoryid_seq OWNER TO "Tonyx";

--
-- Name: courses_categoryid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.courses_categoryid_seq OWNED BY public.coursecategories.categoryid;


--
-- Name: courses_courseid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.courses_courseid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.courses_courseid_seq OWNER TO "Tonyx";

--
-- Name: courses_courseid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.courses_courseid_seq OWNED BY public.courses.courseid;


--
-- Name: customerdata_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.customerdata_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.customerdata_id_seq OWNER TO "Tonyx";

--
-- Name: customerdata; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.customerdata (
    customerdataid integer DEFAULT nextval('public.customerdata_id_seq'::regclass) NOT NULL,
    data character varying(4000) NOT NULL,
    name character varying(300) NOT NULL
);


ALTER TABLE public.customerdata OWNER TO "Tonyx";

--
-- Name: defaulwaiteractionablestates_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.defaulwaiteractionablestates_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.defaulwaiteractionablestates_seq OWNER TO "Tonyx";

--
-- Name: defaultactionablestates; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.defaultactionablestates (
    defaultactionablestatesid integer DEFAULT nextval('public.defaulwaiteractionablestates_seq'::regclass) NOT NULL,
    stateid integer NOT NULL
);


ALTER TABLE public.defaultactionablestates OWNER TO "Tonyx";

--
-- Name: enablers_elablersid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.enablers_elablersid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.enablers_elablersid_seq OWNER TO "Tonyx";

--
-- Name: enablers; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.enablers (
    enablersid integer DEFAULT nextval('public.enablers_elablersid_seq'::regclass) NOT NULL,
    roleid integer NOT NULL,
    stateid integer NOT NULL,
    categoryid integer NOT NULL
);


ALTER TABLE public.enablers OWNER TO "Tonyx";

--
-- Name: roles; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.roles (
    roleid integer NOT NULL,
    rolename character varying(30) NOT NULL,
    comment character varying(50)
);


ALTER TABLE public.roles OWNER TO "Tonyx";

--
-- Name: states_stateid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.states_stateid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.states_stateid_seq OWNER TO "Tonyx";

--
-- Name: states; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.states (
    stateid integer DEFAULT nextval('public.states_stateid_seq'::regclass) NOT NULL,
    isinitial boolean NOT NULL,
    isfinal boolean NOT NULL,
    statusname character varying(30) NOT NULL,
    nextstateid integer,
    isexceptional boolean NOT NULL,
    creatingingredientdecrement boolean NOT NULL
);


ALTER TABLE public.states OWNER TO "Tonyx";

--
-- Name: enablersrolestatuscategories; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.enablersrolestatuscategories AS
 SELECT a.enablersid,
    c.rolename,
    d.name AS categoryname,
    e.statusname
   FROM (((public.enablers a
     JOIN public.roles c ON ((a.roleid = c.roleid)))
     JOIN public.coursecategories d ON ((a.categoryid = d.categoryid)))
     JOIN public.states e ON ((a.stateid = e.stateid)));


ALTER TABLE public.enablersrolestatuscategories OWNER TO "Tonyx";

--
-- Name: incredientdecrementid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.incredientdecrementid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.incredientdecrementid_seq OWNER TO "Tonyx";

--
-- Name: ingredient; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.ingredient (
    ingredientid integer NOT NULL,
    ingredientcategoryid integer NOT NULL,
    name character varying(120) NOT NULL,
    description character varying(4000),
    visibility boolean NOT NULL,
    allergen boolean NOT NULL,
    updateavailabilityflag boolean NOT NULL,
    availablequantity numeric(10,2),
    checkavailabilityflag boolean NOT NULL,
    unitmeasure character varying(20) NOT NULL
);


ALTER TABLE public.ingredient OWNER TO "Tonyx";

--
-- Name: ingredientcategory; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.ingredientcategory (
    ingredientcategoryid integer NOT NULL,
    name character varying(120) NOT NULL,
    description character varying(4000),
    visibility boolean NOT NULL
);


ALTER TABLE public.ingredientcategory OWNER TO "Tonyx";

--
-- Name: ingredient_categoryid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredient_categoryid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.ingredient_categoryid_seq OWNER TO "Tonyx";

--
-- Name: ingredient_categoryid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.ingredient_categoryid_seq OWNED BY public.ingredientcategory.ingredientcategoryid;


--
-- Name: ingredientcourse; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.ingredientcourse (
    ingredientcourseid integer NOT NULL,
    courseid integer NOT NULL,
    ingredientid integer NOT NULL,
    quantity numeric(10,2)
);


ALTER TABLE public.ingredientcourse OWNER TO "Tonyx";

--
-- Name: ingredientcourseid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredientcourseid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.ingredientcourseid_seq OWNER TO "Tonyx";

--
-- Name: ingredientcourseid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.ingredientcourseid_seq OWNED BY public.ingredientcourse.ingredientcourseid;


--
-- Name: ingredientdecrement; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.ingredientdecrement (
    ingredientdecrementid integer DEFAULT nextval('public.incredientdecrementid_seq'::regclass) NOT NULL,
    orderitemid integer NOT NULL,
    typeofdecrement character varying(30) NOT NULL,
    presumednormalquantity numeric(10,2),
    recordedquantity numeric(10,2),
    preparatorid integer NOT NULL,
    registrationtime timestamp without time zone NOT NULL,
    ingredientid integer NOT NULL
);


ALTER TABLE public.ingredientdecrement OWNER TO "Tonyx";

--
-- Name: orderitems; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.orderitems (
    orderitemid integer NOT NULL,
    courseid integer NOT NULL,
    quantity integer NOT NULL,
    orderid integer NOT NULL,
    comment character varying(50),
    price numeric(10,2),
    stateid integer NOT NULL,
    archived boolean,
    startingtime timestamp without time zone NOT NULL,
    closingtime timestamp without time zone,
    ordergroupid integer NOT NULL,
    hasbeenrejected boolean NOT NULL,
    suborderid integer,
    isinsasuborder boolean DEFAULT false,
    printcount integer NOT NULL
);


ALTER TABLE public.orderitems OWNER TO "Tonyx";

--
-- Name: ingredientdecrementview; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.ingredientdecrementview AS
 SELECT a.ingredientdecrementid,
    a.orderitemid,
    c.quantity AS numberofcourses,
    c.closingtime,
    c.courseid,
    d.name AS coursename,
    a.typeofdecrement,
    a.presumednormalquantity,
    a.recordedquantity,
    a.registrationtime,
    a.preparatorid,
    a.ingredientid,
    b.updateavailabilityflag,
    b.checkavailabilityflag,
    b.availablequantity,
    b.name AS ingredientname,
    e.quantity AS ingredientquantity,
    b.unitmeasure
   FROM ((((public.ingredientdecrement a
     JOIN public.ingredient b ON ((a.ingredientid = b.ingredientid)))
     JOIN public.orderitems c ON ((a.orderitemid = c.orderitemid)))
     JOIN public.courses d ON ((c.courseid = d.courseid)))
     LEFT JOIN public.ingredientcourse e ON (((e.ingredientid = a.ingredientid) AND (e.courseid = c.courseid))));


ALTER TABLE public.ingredientdecrementview OWNER TO "Tonyx";

--
-- Name: ingredientdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.ingredientdetails AS
 SELECT b.ingredientid,
    b.name AS ingredientname,
    b.allergen,
    b.visibility,
    d.visibility AS categoryvisibility,
    d.ingredientcategoryid
   FROM (public.ingredient b
     JOIN public.ingredientcategory d ON ((b.ingredientcategoryid = d.ingredientcategoryid)));


ALTER TABLE public.ingredientdetails OWNER TO "Tonyx";

--
-- Name: ingredientid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredientid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.ingredientid_seq OWNER TO "Tonyx";

--
-- Name: ingredientid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.ingredientid_seq OWNED BY public.ingredient.ingredientid;


--
-- Name: ingredientincrementid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredientincrementid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.ingredientincrementid_seq OWNER TO "Tonyx";

--
-- Name: ingredientincrement; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.ingredientincrement (
    ingredientincrementid integer DEFAULT nextval('public.ingredientincrementid_seq'::regclass) NOT NULL,
    ingredientid integer NOT NULL,
    comment character varying(100) NOT NULL,
    unitofmeasure character varying(10) NOT NULL,
    quantity numeric(10,2) NOT NULL,
    userid integer NOT NULL,
    registrationtime timestamp without time zone NOT NULL
);


ALTER TABLE public.ingredientincrement OWNER TO "Tonyx";

--
-- Name: ingredientofcourses; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.ingredientofcourses AS
 SELECT a.courseid,
    b.ingredientid,
    a.name AS coursename,
    b.name AS ingredientname,
    c.quantity,
    b.availablequantity,
    c.ingredientcourseid,
    b.allergen,
    b.visibility,
    d.visibility AS categoryvisibility,
    b.checkavailabilityflag,
    d.ingredientcategoryid,
    b.unitmeasure
   FROM (((public.courses a
     JOIN public.ingredientcourse c ON ((a.courseid = c.courseid)))
     JOIN public.ingredient b ON ((c.ingredientid = b.ingredientid)))
     JOIN public.ingredientcategory d ON ((b.ingredientcategoryid = d.ingredientcategoryid)));


ALTER TABLE public.ingredientofcourses OWNER TO "Tonyx";

--
-- Name: ingredientpriceid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredientpriceid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.ingredientpriceid_seq OWNER TO "Tonyx";

--
-- Name: ingredientprice; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.ingredientprice (
    ingredientpriceid integer DEFAULT nextval('public.ingredientpriceid_seq'::regclass) NOT NULL,
    ingredientid integer NOT NULL,
    quantity numeric(10,2) NOT NULL,
    isdefaultadd boolean NOT NULL,
    isdefaultsubtract boolean NOT NULL,
    addprice numeric(10,2) NOT NULL,
    subtractprice numeric(10,2) NOT NULL
);


ALTER TABLE public.ingredientprice OWNER TO "Tonyx";

--
-- Name: ingredientpricedetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.ingredientpricedetails AS
 SELECT a.ingredientpriceid,
    a.ingredientid,
    a.quantity,
    a.isdefaultadd,
    a.isdefaultsubtract,
    a.addprice,
    a.subtractprice,
    b.name
   FROM (public.ingredientprice a
     JOIN public.ingredient b ON ((a.ingredientid = b.ingredientid)))
  ORDER BY b.name;


ALTER TABLE public.ingredientpricedetails OWNER TO "Tonyx";

--
-- Name: invoicesid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.invoicesid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.invoicesid_seq OWNER TO "Tonyx";

--
-- Name: invoices; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.invoices (
    invoicesid integer DEFAULT nextval('public.invoicesid_seq'::regclass) NOT NULL,
    data character varying(4000) NOT NULL,
    invoicenumber integer NOT NULL,
    customerdataid integer,
    date timestamp without time zone NOT NULL,
    suborderid integer,
    orderid integer
);


ALTER TABLE public.invoices OWNER TO "Tonyx";

--
-- Name: orders; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.orders (
    orderid integer NOT NULL,
    "table" character varying(100) NOT NULL,
    person character varying(120) NOT NULL,
    ongoing boolean DEFAULT true,
    userid integer NOT NULL,
    startingtime timestamp without time zone NOT NULL,
    closingtime timestamp without time zone,
    voided boolean DEFAULT false,
    archived boolean DEFAULT false NOT NULL,
    total numeric(10,2),
    adjustedtotal numeric(10,2),
    plaintotalvariation numeric(10,2) DEFAULT 0,
    percentagevariataion numeric(10,2) DEFAULT 0,
    adjustispercentage boolean DEFAULT false,
    adjustisplain boolean DEFAULT false,
    forqruserarchived boolean
);


ALTER TABLE public.orders OWNER TO "Tonyx";

--
-- Name: users; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.users (
    userid integer NOT NULL,
    username character varying(200) NOT NULL,
    password character varying(200) NOT NULL,
    enabled boolean DEFAULT true NOT NULL,
    canvoidorders boolean DEFAULT false,
    role integer NOT NULL,
    canmanageallorders boolean DEFAULT false NOT NULL,
    creationtime timestamp without time zone,
    istemporary boolean NOT NULL,
    canchangetheprice boolean NOT NULL,
    "table" character varying(200),
    consumed boolean,
    canmanagecourses boolean NOT NULL
);


ALTER TABLE public.users OWNER TO "Tonyx";

--
-- Name: nonarchivedorderdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.nonarchivedorderdetails AS
 SELECT a.orderid,
    a."table",
    a.person,
    a.ongoing,
    a.userid,
    a.startingtime,
    a.closingtime,
    a.voided,
    a.total,
    a.adjustedtotal,
    a.archived,
    b.username,
    a.forqruserarchived
   FROM (public.orders a
     JOIN public.users b ON ((a.userid = b.userid)))
  WHERE ((a.archived = false) AND (a.voided = false) AND (( SELECT count(*) AS count
           FROM (public.orderitems c
             JOIN public.states d ON ((c.stateid = d.stateid)))
          WHERE ((c.orderid = a.orderid) AND (NOT d.isinitial))) > 0))
  ORDER BY a.startingtime;


ALTER TABLE public.nonarchivedorderdetails OWNER TO "Tonyx";

--
-- Name: nonemptyorderdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.nonemptyorderdetails AS
 SELECT a.orderid,
    a."table",
    a.person,
    a.ongoing,
    a.userid,
    a.startingtime,
    a.closingtime,
    a.voided,
    a.total,
    a.adjustedtotal,
    a.archived,
    b.username,
    a.forqruserarchived
   FROM (public.orders a
     JOIN public.users b ON ((a.userid = b.userid)))
  WHERE (( SELECT count(*) AS count
           FROM public.orderitems c
          WHERE (c.orderid = a.orderid)) > 0)
  ORDER BY a.startingtime;


ALTER TABLE public.nonemptyorderdetails OWNER TO "Tonyx";

--
-- Name: observers_observerid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.observers_observerid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.observers_observerid_seq OWNER TO "Tonyx";

--
-- Name: observers; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.observers (
    observersid integer DEFAULT nextval('public.observers_observerid_seq'::regclass) NOT NULL,
    stateid integer NOT NULL,
    roleid integer NOT NULL,
    categoryid integer NOT NULL
);


ALTER TABLE public.observers OWNER TO "Tonyx";

--
-- Name: observers_observersid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.observers_observersid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.observers_observersid_seq OWNER TO "Tonyx";

--
-- Name: observersrolestatuscategories; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.observersrolestatuscategories AS
 SELECT a.observersid,
    c.rolename,
    d.name AS categoryname,
    e.statusname
   FROM (((public.observers a
     JOIN public.roles c ON ((a.roleid = c.roleid)))
     JOIN public.coursecategories d ON ((a.categoryid = d.categoryid)))
     JOIN public.states e ON ((a.stateid = e.stateid)));


ALTER TABLE public.observersrolestatuscategories OWNER TO "Tonyx";

--
-- Name: orderdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.orderdetails AS
 SELECT a.orderid,
    a."table",
    a.person,
    a.ongoing,
    a.userid,
    a.startingtime,
    a.closingtime,
    a.voided,
    a.total,
    a.adjustedtotal,
    a.archived,
    b.username,
    a.forqruserarchived
   FROM (public.orders a
     JOIN public.users b ON ((a.userid = b.userid)))
  ORDER BY a.startingtime;


ALTER TABLE public.orderdetails OWNER TO "Tonyx";

--
-- Name: orderoutgroup_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.orderoutgroup_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.orderoutgroup_id_seq OWNER TO "Tonyx";

--
-- Name: orderoutgroup; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.orderoutgroup (
    ordergroupid integer DEFAULT nextval('public.orderoutgroup_id_seq'::regclass) NOT NULL,
    printcount integer NOT NULL,
    orderid integer NOT NULL,
    groupidentifier integer NOT NULL
);


ALTER TABLE public.orderoutgroup OWNER TO "Tonyx";

--
-- Name: suborderid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.suborderid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.suborderid_seq OWNER TO "Tonyx";

--
-- Name: suborder; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.suborder (
    suborderid integer DEFAULT nextval('public.suborderid_seq'::regclass) NOT NULL,
    orderid integer NOT NULL,
    subtotal numeric(10,2) NOT NULL,
    comment character varying(30),
    payed boolean NOT NULL,
    creationtime timestamp without time zone NOT NULL,
    tendercodesid integer,
    subtotaladjustment numeric(10,2) DEFAULT 0,
    subtotalpercentadjustment numeric(10,2) DEFAULT 0
);


ALTER TABLE public.suborder OWNER TO "Tonyx";

--
-- Name: orderitemdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.orderitemdetails AS
 SELECT a.orderitemid,
    a.comment,
    a.orderid,
    a.quantity,
    b.name,
    b.price AS originalprice,
    a.price,
    b.categoryid,
    b.courseid,
    c.statusname,
    a.stateid,
    d.userid,
    a.startingtime,
    a.closingtime,
    d."table",
    d.person,
    e.username,
    a.hasbeenrejected,
    a.suborderid,
    a.isinsasuborder,
    g.groupidentifier,
    g.ordergroupid,
    (COALESCE(f.payed, false) OR d.archived) AS payed
   FROM ((((((public.orderitems a
     JOIN public.courses b ON ((a.courseid = b.courseid)))
     JOIN public.states c ON ((a.stateid = c.stateid)))
     JOIN public.orders d ON ((d.orderid = a.orderid)))
     JOIN public.users e ON ((d.userid = e.userid)))
     JOIN public.orderoutgroup g ON ((a.ordergroupid = g.ordergroupid)))
     LEFT JOIN public.suborder f ON ((a.suborderid = f.suborderid)));


ALTER TABLE public.orderitemdetails OWNER TO "Tonyx";

--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.orderitems_orderitemid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.orderitems_orderitemid_seq OWNER TO "Tonyx";

--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.orderitems_orderitemid_seq OWNED BY public.orderitems.orderitemid;


--
-- Name: orderitemstates; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.orderitemstates (
    orderitemstatesid integer NOT NULL,
    orderitemid integer NOT NULL,
    stateid integer NOT NULL,
    startingtime timestamp without time zone NOT NULL
);


ALTER TABLE public.orderitemstates OWNER TO "Tonyx";

--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.orderitemstates_orderitemstates_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.orderitemstates_orderitemstates_id_seq OWNER TO "Tonyx";

--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.orderitemstates_orderitemstates_id_seq OWNED BY public.orderitemstates.orderitemstatesid;


--
-- Name: orderoutgroupdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.orderoutgroupdetails AS
 SELECT a.ordergroupid,
    a.printcount,
    a.orderid,
    a.groupidentifier,
    b."table",
    b.person
   FROM (public.orderoutgroup a
     JOIN public.orders b ON ((a.orderid = b.orderid)));


ALTER TABLE public.orderoutgroupdetails OWNER TO "Tonyx";

--
-- Name: orders_orderid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.orders_orderid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.orders_orderid_seq OWNER TO "Tonyx";

--
-- Name: orders_orderid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.orders_orderid_seq OWNED BY public.orders.orderid;


--
-- Name: paymentid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.paymentid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.paymentid_seq OWNER TO "Tonyx";

--
-- Name: paymentitem; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.paymentitem (
    paymentid integer DEFAULT nextval('public.paymentid_seq'::regclass) NOT NULL,
    suborderid integer,
    orderid integer,
    tendercodesid integer NOT NULL,
    amount numeric(10,2) NOT NULL
);


ALTER TABLE public.paymentitem OWNER TO "Tonyx";

--
-- Name: tendercodesid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.tendercodesid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.tendercodesid_seq OWNER TO "Tonyx";

--
-- Name: tendercodes; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.tendercodes (
    tendercodesid integer DEFAULT nextval('public.tendercodesid_seq'::regclass) NOT NULL,
    tendercode integer NOT NULL,
    tendername character varying(50) NOT NULL
);


ALTER TABLE public.tendercodes OWNER TO "Tonyx";

--
-- Name: paymentitemdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.paymentitemdetails AS
 SELECT a.paymentid,
    a.suborderid,
    a.orderid,
    a.tendercodesid,
    a.amount,
    b.tendername,
    b.tendercode AS tendercodeidentifier
   FROM (public.paymentitem a
     JOIN public.tendercodes b ON ((a.tendercodesid = b.tendercodesid)));


ALTER TABLE public.paymentitemdetails OWNER TO "Tonyx";

--
-- Name: printerforcategory_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.printerforcategory_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.printerforcategory_id_seq OWNER TO "Tonyx";

--
-- Name: printerforcategory; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.printerforcategory (
    printerforcategoryid integer DEFAULT nextval('public.printerforcategory_id_seq'::regclass) NOT NULL,
    categoryid integer NOT NULL,
    printerid integer NOT NULL,
    stateid integer NOT NULL
);


ALTER TABLE public.printerforcategory OWNER TO "Tonyx";

--
-- Name: printers_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.printers_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.printers_id_seq OWNER TO "Tonyx";

--
-- Name: printers; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.printers (
    printerid integer DEFAULT nextval('public.printers_id_seq'::regclass) NOT NULL,
    name character varying(60) NOT NULL
);


ALTER TABLE public.printers OWNER TO "Tonyx";

--
-- Name: printerforcategorydetail; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.printerforcategorydetail AS
 SELECT a.printerforcategoryid,
    a.categoryid,
    a.printerid,
    a.stateid,
    b.name AS categoryname,
    c.name AS printername,
    e.statusname AS statename
   FROM (((public.printerforcategory a
     JOIN public.coursecategories b ON ((a.categoryid = b.categoryid)))
     JOIN public.printers c ON ((a.printerid = c.printerid)))
     JOIN public.states e ON ((a.stateid = e.stateid)));


ALTER TABLE public.printerforcategorydetail OWNER TO "Tonyx";

--
-- Name: printerforreceiptandinvoice_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.printerforreceiptandinvoice_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.printerforreceiptandinvoice_id_seq OWNER TO "Tonyx";

--
-- Name: printerforreceiptandinvoice; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.printerforreceiptandinvoice (
    printerforcategoryid integer DEFAULT nextval('public.printerforreceiptandinvoice_id_seq'::regclass) NOT NULL,
    printinvoice boolean DEFAULT false NOT NULL,
    printreceipt boolean DEFAULT false NOT NULL,
    printerid integer NOT NULL
);


ALTER TABLE public.printerforreceiptandinvoice OWNER TO "Tonyx";

--
-- Name: rejectedorderitems_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.rejectedorderitems_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.rejectedorderitems_id_seq OWNER TO "Tonyx";

--
-- Name: rejectedorderitems; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.rejectedorderitems (
    rejectedorderitemid integer DEFAULT nextval('public.rejectedorderitems_id_seq'::regclass) NOT NULL,
    courseid integer NOT NULL,
    cause character varying(100) NOT NULL,
    timeofrejection timestamp without time zone NOT NULL,
    orderitemid integer NOT NULL
);


ALTER TABLE public.rejectedorderitems OWNER TO "Tonyx";

--
-- Name: roles_roleid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.roles_roleid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.roles_roleid_seq OWNER TO "Tonyx";

--
-- Name: roles_roleid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.roles_roleid_seq OWNED BY public.roles.roleid;


--
-- Name: tempuseractionablestates_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.tempuseractionablestates_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.tempuseractionablestates_seq OWNER TO "Tonyx";

--
-- Name: temp_user_actionable_states; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.temp_user_actionable_states (
    tempuseractionablestateid integer DEFAULT nextval('public.tempuseractionablestates_seq'::regclass) NOT NULL,
    userid integer NOT NULL,
    stateid integer NOT NULL
);


ALTER TABLE public.temp_user_actionable_states OWNER TO "Tonyx";

--
-- Name: temp_user_actionable_states_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.temp_user_actionable_states_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.temp_user_actionable_states_seq OWNER TO "Tonyx";

--
-- Name: temp_user_default_actionable_states; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.temp_user_default_actionable_states (
    tempmuseractionablestatesid integer DEFAULT nextval('public.temp_user_actionable_states_seq'::regclass) NOT NULL,
    stateid integer NOT NULL
);


ALTER TABLE public.temp_user_default_actionable_states OWNER TO "Tonyx";

--
-- Name: users_userid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.users_userid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.users_userid_seq OWNER TO "Tonyx";

--
-- Name: users_userid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.users_userid_seq OWNED BY public.users.userid;


--
-- Name: usersview; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.usersview AS
 SELECT a.userid,
    a.username,
    a.password,
    a.enabled,
    a.canvoidorders,
    a.role,
    b.rolename,
    a.istemporary,
    a.creationtime,
    a.consumed,
    a.canmanagecourses,
    a.canmanageallorders
   FROM (public.users a
     JOIN public.roles b ON ((a.role = b.roleid)))
  ORDER BY a.username;


ALTER TABLE public.usersview OWNER TO "Tonyx";

--
-- Name: variations; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.variations (
    variationsid integer NOT NULL,
    orderitemid integer NOT NULL,
    ingredientid integer NOT NULL,
    tipovariazione character varying(30) NOT NULL,
    plailnumvariation integer,
    ingredientpriceid integer
);


ALTER TABLE public.variations OWNER TO "Tonyx";

--
-- Name: variationdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.variationdetails AS
 SELECT a.variationsid,
    a.orderitemid,
    a.ingredientid,
    a.tipovariazione,
    b.name AS ingredientname,
    b.allergen,
    a.plailnumvariation,
    c.courseid,
    d.userid,
    a.ingredientpriceid,
    e.quantity
   FROM ((((public.variations a
     JOIN public.ingredient b ON ((a.ingredientid = b.ingredientid)))
     JOIN public.orderitems c ON ((a.orderitemid = c.orderitemid)))
     JOIN public.orders d ON ((c.orderid = d.orderid)))
     LEFT JOIN public.ingredientprice e ON ((a.ingredientpriceid = e.ingredientpriceid)))
  ORDER BY b.name;


ALTER TABLE public.variationdetails OWNER TO "Tonyx";

--
-- Name: variations_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.variations_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.variations_seq OWNER TO "Tonyx";

--
-- Name: variations_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.variations_seq OWNED BY public.variations.variationsid;


--
-- Name: voidedorderslogbuffer; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.voidedorderslogbuffer (
    voidedorderslogbufferid integer NOT NULL,
    voidedtime timestamp without time zone NOT NULL,
    orderid integer NOT NULL,
    userid integer NOT NULL
);


ALTER TABLE public.voidedorderslogbuffer OWNER TO "Tonyx";

--
-- Name: voidedorderslog_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.voidedorderslog_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.voidedorderslog_id_seq OWNER TO "Tonyx";

--
-- Name: voidedorderslog_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.voidedorderslog_id_seq OWNED BY public.voidedorderslogbuffer.voidedorderslogbufferid;


--
-- Name: waiteractionablestates_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.waiteractionablestates_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.waiteractionablestates_seq OWNER TO "Tonyx";

--
-- Name: waiteractionablestates; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.waiteractionablestates (
    waiterwatchablestatesid integer DEFAULT nextval('public.waiteractionablestates_seq'::regclass) NOT NULL,
    userid integer NOT NULL,
    stateid integer NOT NULL
);


ALTER TABLE public.waiteractionablestates OWNER TO "Tonyx";

--
-- Name: archivedorderslogbuffer archivedlogbufferid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.archivedorderslogbuffer ALTER COLUMN archivedlogbufferid SET DEFAULT nextval('public.archivedorderslog_id_seq'::regclass);


--
-- Name: coursecategories categoryid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.coursecategories ALTER COLUMN categoryid SET DEFAULT nextval('public.courses_categoryid_seq'::regclass);


--
-- Name: courses courseid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.courses ALTER COLUMN courseid SET DEFAULT nextval('public.courses_courseid_seq'::regclass);


--
-- Name: ingredient ingredientid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredient ALTER COLUMN ingredientid SET DEFAULT nextval('public.ingredientid_seq'::regclass);


--
-- Name: ingredientcategory ingredientcategoryid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcategory ALTER COLUMN ingredientcategoryid SET DEFAULT nextval('public.ingredient_categoryid_seq'::regclass);


--
-- Name: ingredientcourse ingredientcourseid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcourse ALTER COLUMN ingredientcourseid SET DEFAULT nextval('public.ingredientcourseid_seq'::regclass);


--
-- Name: orderitems orderitemid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitems ALTER COLUMN orderitemid SET DEFAULT nextval('public.orderitems_orderitemid_seq'::regclass);


--
-- Name: orderitemstates orderitemstatesid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitemstates ALTER COLUMN orderitemstatesid SET DEFAULT nextval('public.orderitemstates_orderitemstates_id_seq'::regclass);


--
-- Name: orders orderid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orders ALTER COLUMN orderid SET DEFAULT nextval('public.orders_orderid_seq'::regclass);


--
-- Name: roles roleid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.roles ALTER COLUMN roleid SET DEFAULT nextval('public.roles_roleid_seq'::regclass);


--
-- Name: users userid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.users ALTER COLUMN userid SET DEFAULT nextval('public.users_userid_seq'::regclass);


--
-- Name: variations variationsid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.variations ALTER COLUMN variationsid SET DEFAULT nextval('public.variations_seq'::regclass);


--
-- Name: voidedorderslogbuffer voidedorderslogbufferid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.voidedorderslogbuffer ALTER COLUMN voidedorderslogbufferid SET DEFAULT nextval('public.voidedorderslog_id_seq'::regclass);


--
-- Name: archivedorderslog_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.archivedorderslog_id_seq', 282, true);


--
-- Data for Name: archivedorderslogbuffer; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.archivedorderslogbuffer (archivedlogbufferid, archivedtime, orderid) FROM stdin;
267	2019-03-06 18:02:10.709851	578
268	2019-03-18 12:53:45.696464	580
270	2019-03-18 12:54:30.764693	579
271	2019-04-09 16:59:05.200468	558
272	2019-04-09 16:59:06.795155	567
273	2019-04-09 16:59:08.263744	572
274	2019-04-09 16:59:09.432253	576
275	2019-04-09 16:59:11.392239	577
276	2019-04-09 17:01:35.905525	583
277	2019-04-09 17:03:25.93323	559
278	2019-04-09 17:03:57.671279	571
280	2019-04-09 17:59:05.323945	573
282	2019-04-09 21:29:15.165196	585
\.


--
-- Data for Name: coursecategories; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.coursecategories (categoryid, name, visibile, abstract) FROM stdin;
67	pranzo lavoro	t	t
69	pizze	t	f
64	amari, digestivi & liquors	t	f
45	antipasti & sfizi	t	f
53	bar	t	f
57	alcolici	t	f
66	coperti	t	f
49	kebab	t	f
50	crepes salate	t	f
51	piadine	t	f
52	dessert	t	f
47	contorni e insalate	t	f
55	birre in bottiglia	t	f
54	birre alla spina	t	f
46	frittate	t	f
58	shot	t	f
42	secondi	t	f
60	mules	t	f
62	dessert fritti	t	f
63	dessert freddi	t	f
65	grappa	t	f
61	dessert caldi	t	f
43	panini	t	f
41	primi	t	f
48	schiacciate & toast	t	f
56	thai	t	f
\.


--
-- Data for Name: courses; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.courses (courseid, name, description, price, categoryid, visibility) FROM stdin;
209	gnocchetti ai 4 formaggi		10.00	41	t
210	scaloppina ai funghi porcini		18.00	42	t
168	affettato salumi e formaggi x 2 persone		18.00	45	t
169	fritto billy 5 pz.		5.00	45	t
170	sfizio billy 17 pz.		17.00	45	t
171	patate steak house		6.00	45	t
172	patate fritte		4.00	45	t
173	patate e wurstel		5.00	45	t
174	patate e salsiccia		5.00	45	t
175	patate e "onion rings"		5.00	45	t
176	patate e formaggio		6.00	45	t
177	patate salsiccia e cheddar		7.00	45	t
178	patate wurstel e sottiletta		7.00	45	t
179	bruschette al pomodoro 3 pz 		3.00	45	t
211	scaloppina al limone o al vino bianco		13.00	42	t
180	bruschette al lardo di colonnata e rosmarino 3 pz.		4.50	45	t
212	pollo ai ferri		10.00	42	t
182	bruschette miste 3 pz.		4.50	45	t
183	involtini di melanzane con rucola, crudo e aceto balsamico		6.00	45	t
184	bresaola, rucola, scaglie di grana  pomodorini		15.00	45	t
185	jalapeños 5 pz.		5.50	45	t
186	alette e coscette di pollo 5 pz.		7.50	45	t
187	provola alla milanese 3 pz.		3.00	45	t
188	mozzarella in carrozza 3 pz.		3.00	45	t
189	medaglioni di pollo fritto 5pz		5.00	45	t
190	medaglioni di pollo special 4 pz		6.00	45	t
191	olive all'ascolana 5 pz.		4.00	45	t
192	arancini 3 pz		3.00	45	t
193	croquette 3 pz		3.00	45	t
194	club sandwich		14.00	45	t
195	fiori di zucca ripieni con prosciutto cotto e mozz. 3 pz.		7.50	45	t
198	scialatielli zucchine e provola		10.00	41	t
197	gnocchetti di radicchio e gorgonzola		10.00	41	t
199	ravioli capresi		10.00	41	t
200	pennette broccoli e salsiccia		8.00	41	t
201	spaghetti sciuè sciuè		8.00	41	t
202	pennette aumm aumm		8.00	41	t
203	spaghetti alla puttanesca		8.00	41	t
204	spaghetti alla carbonara		8.00	41	t
205	pennette panna, prosciutto e funghi		8.00	41	t
206	pasta e fagioli		8.00	41	t
207	tagliatelle funghi porcini e salsiccia		12.00	41	t
208	spaghetti al limone		9.00	41	t
213	cotoletta di vitello con patate		13.00	42	t
214	straccetti di vitello al vino rosso		12.00	42	t
215	salsiccia e broccoli o patate		12.00	42	t
216	filetto alla griglia con patate steak house		22.00	42	t
217	filetto al pepe verde		20.00	42	t
218	filetto al gorgonzola		20.00	42	t
219	filetto all'aceto balsamico o vino rosso		20.00	42	t
220	filetto ai funghi porcini		22.00	42	t
221	tagliata di filetto con pomodorini, rucola, scaglie di parmigiano e aceto balsamico		22.00	42	t
222	pollo al curry		12.00	42	t
223	involtini di vitello con ripieno al prosciutto cotto e provola		13.00	42	t
224	roast beef con salsa tonnata		15.00	42	t
225	omelette		6.00	46	t
226	frittata con zucchine		7.00	46	t
227	frittata con verdure		8.00	46	t
228	frittata di cipolle		7.00	46	t
229	frittata di prosciutto cotto e funghi		8.00	46	t
230	frittata ai 4 formaggi		8.00	46	t
231	misto di verdure alla griglia		5.00	47	t
232	patate al rosmarino		5.00	47	t
233	broccoli saltati		5.00	47	t
234	involtini di melenzane		6.00	47	t
235	parmigiana di melenzane 		6.00	47	t
236	funghi trifolati		5.00	47	t
237	melenzane a funghetti		5.00	47	t
238	INSALATA LAVINIA		10.00	47	t
239	INSALATA DI FAGIOLI		9.00	47	t
240	INSALATA CLEOPATRA		10.00	47	t
241	INSALATA ENEA		8.00	47	t
242	INSALATA VENERE		6.00	47	t
243	CAPONATA (con biscotti di grano duro)		10.00	47	t
244	INSALATA CAPRESE		10.00	47	t
245	MARCO		5.00	43	t
248	RENATO		7.00	43	t
256	PIETRO		9.00	43	t
251	MARIA		7.00	43	t
249	ALDO		6.00	43	t
250	LUCIO		6.00	43	t
252	CARLO		6.00	43	t
255	MISTER BILLY		10.50	43	t
253	NUNZIO		7.00	43	t
254	CICCIO		7.50	43	t
263	RICCARDO		6.00	43	t
259	GIOVANNI		9.00	43	t
257	PICONE		8.50	43	t
260	ISOLOTTO		11.00	43	t
258	AMERICANO		7.50	43	t
261	MARCELLO		5.00	43	t
265	FLAVIO		5.00	43	t
262	LUCA		5.00	43	t
264	RAFFAELLO		5.50	43	t
267	ARMANDO		7.50	43	t
268	MONICA		6.00	43	t
266	ALESSANDRO		7.00	43	t
246	ROSARIO		6.50	43	t
269	CARMELO		6.00	43	t
270	VINCENZO		6.00	43	t
271	GIORGIA		6.00	43	t
272	ELENA		6.00	43	t
273	ANTONIO		6.50	43	t
274	CIRO		6.00	43	t
196	bucatini all'amatriciana		8.00	41	t
247	SERGIO		5.00	43	t
277	EMILIANO		5.50	43	t
278	GIACOMO		5.00	43	t
279	BRUNO		5.50	43	t
280	LUIGI		7.50	43	t
281	FABRIZIO		6.50	43	t
282	MARIANO		5.50	43	t
283	SILVIA		5.50	43	t
284	ROBERTA		6.00	43	t
285	MANUELA		6.00	43	t
286	ALICE		6.50	43	t
287	GIULIANO		6.50	43	t
288	VALENTINA		7.00	43	t
289	PHILLY CHEESE STEAK		9.00	43	t
290	prosciutto crudo e sottiletta		5.00	48	t
291	mozzarella e pomodoro		5.00	48	t
292	pancetta e provola		5.00	48	t
293	prosciutto cotto e sottiletta		5.00	48	t
294	mozzarella, pomodoro e prosicutto cotto		6.00	48	t
295	crudo, melanzane alla griglia e rucola		6.00	48	t
296	speck e sottiletta		5.00	48	t
297	speck, melanzane sott'olio e sottiletta		5.00	48	t
320	affogato al cappuccino con panna (bayles a kahlua)		8.00	52	t
299	piadina con kebab, insalata, cipolle, carote e tzaziki		6.00	49	t
298	piadina con kebab, insalata, pomodoro, patate e tzaziki		6.00	49	t
300	piadina con kebab, provola affumicata, funchi, patatate e tzaziki		7.00	49	t
301	piadina con kebab, mozzarella, rucola, pomodorini e tzaziki		7.00	49	t
302	kebab, insalata, pomodoro, patate e tzaziki al piatto		11.00	49	t
303	salsiccia, rucola, pomodorini, funghi		10.00	50	t
304	speck, provola, melanzane a funghtti		7.00	50	t
305	prosciutto cotto, crema ai 4 formaggi, funghi		9.00	50	t
306	crudo, mozzarella, insalata e pomodoro		8.00	50	t
307	salsiccia, patate al rosmarino, crema di funghi porcini		10.00	50	t
308	tonno, pomodoro e mozzarella		5.50	51	t
309	speck, pomodoro, mozzarella, rucola e maionese		6.50	51	t
310	speck, rucola, provola, crema ai funghi		6.50	51	t
311	doppia sottiletta, speck, rucola, melanzane alla griglia		6.50	51	t
312	semifreddo al torroncino		5.00	52	t
313	crepes alla nutella o cioccolata bianca		3.00	52	t
314	crepes cioccolato bianco e mandorle		4.00	52	t
315	waffel nutella o cioccolata bianca		4.00	52	t
316	waffel black & white		4.50	52	t
317	waffel nutella e cocco		5.00	52	t
318	waffel ferrero roches e frutti di bosco		5.00	52	t
319	affogato al caffè con panna		6.00	52	t
321	gelato fritto alla vaniglia		4.50	52	t
322	gelato alla vaniglia		5.00	52	t
323	affogato all'amarena con panna		7.00	52	t
324	dessert del giorno		5.00	52	t
325	pepite dolci fritte		5.00	52	t
326	acqua piccola		2.00	53	t
327	acqua grande		3.00	53	t
328	coca cola alla spina piccola		2.50	53	t
329	coca cola alla spina media		3.00	53	t
330	coca cola alla spina grande		4.00	53	t
331	bibita in lattina		2.50	53	t
332	bicchiere di vino		5.00	53	t
333	the freddo		3.00	53	t
334	bibite varie		3.00	53	t
335	caffè		1.50	53	t
336	Caffetteria		3.00	53	t
337	amari o limoncello		5.00	53	t
338	red bull		4.00	53	t
339	long drink		7.00	53	t
340	birra bionda piccola		2.50	54	t
341	birra bionda media		4.00	54	t
342	birra bionda litro		9.00	54	t
343	birra speciale piccola		3.00	54	t
344	birra speciale media		5.00	54	t
345	birra speciale litro		12.00	54	t
356	corona messico		4.00	55	t
346	flamingo 2 litri bionda		17.00	54	t
347	flamingo 3 litri bionda		26.00	54	t
348	flamingo 2 litri speciale		24.00	54	t
349	flamingo 3 litri speciale		35.00	54	t
350	amy di grand saint bernard italia		6.00	55	t
351	beck's germania		4.00	55	t
352	biava di melchiori italia		6.00	55	t
353	bionda trentina di melchiori italia		6.00	55	t
354	budweiser USA		4.00	55	t
355	chang thailandia		5.00	55	t
357	desperados fancia		5.00	55	t
358	duvel belgio		5.00	55	t
359	franziskaner germania		5.00	55	t
360	heineken olanda		4.00	55	t
361	IPA selvatica di maso alta italia		6.00	55	t
362	kwak belgio		5.00	55	t
363	nastro azzurro italia		4.00	55	t
364	paulaner weisse germania		5.00	55	t
365	roen bio di melchiori italia		6.00	55	t
366	saint bernardus prior belgio		5.00	55	t
367	singha thailandia		5.00	55	t
368	stella artois olanda		5.00	55	t
369	tennet's scozia		5.00	55	t
370	weizen trentina di melchiori italia		6.00	55	t
371	vuoto		6.00	43	t
275	FRANCESCO		6.50	43	t
276	EUGENIO		6.00	43	t
372	CANADESE		10.00	43	t
374	involtini di verdure		8.00	56	t
375	PANINOBASE		0.10	43	t
376	ciambelle di gamberi 4 pz.		8.00	56	t
378	nuvolette di drago		3.00	56	t
377	gamberi in tempura 10 pz.		15.00	56	t
379	fritto misto Thai		18.00	56	t
380	Gamberoni fritti in pasta Kataifi 4pz.		15.00	56	t
381	Riso saltato con verdure		8.00	56	t
382	Riso saltato con verure e pollo		10.00	56	t
383	riso saltato con verdure e carne di maiale		9.00	56	t
384	riso saltato con verure e gamberi		11.00	56	t
385	tagliolini di riso saltati con verdure		8.00	56	t
386	tagliolini di riso saltati con verdure e pollo		10.00	56	t
387	tagliolini di riso saltati con verdure e carne di maiale 		9.00	56	t
388	tagliolini di riso saltati con verdure e gamberi		11.00	56	t
389	pollo saltato con verure e anacardi		14.00	56	t
390	pollo saltato con basilico e peperoncino		12.00	56	t
391	carne di maiale saltata con basilico e peperoncino		11.00	56	t
392	gamberi saltati con basilico e peperoncino		13.00	56	t
393	pollo saltato con salsa di ostriche		12.00	56	t
394	carne di maiale saltata con salsa di ostriche		11.00	56	t
395	gamberi saltati con salsa di ostriche		13.00	56	t
396	zuppa agropiccante di pollo		13.00	56	t
397	pollo saltato con salsa al curry e latte di cocco		12.00	56	t
398	carne di maiale saltata con salsa al curry e latte di cocco		11.00	56	t
399	gamberoni saltati con salsa al curry e latte di cocco		13.00	56	t
400	ghang		5.00	55	t
401	After-eight		6.00	57	t
402	Alexander		6.00	57	t
403	Americano		6.00	57	t
404	Aper-Tass		6.00	57	t
405	Apple Mojito		6.00	57	t
406	Bacardi Cockail		6.00	57	t
407	Bay breeze		6.00	57	t
408	Black Russian		6.00	57	t
409	Boulevardier		6.00	57	t
410	Bounty		6.00	57	t
411	Buck's Fizz		6.00	57	t
412	Caipiriña		6.00	57	t
413	Caipiroska		6.00	57	t
414	Cairo Cocktail		6.00	57	t
415	Campri Orange		6.00	57	t
416	Cosmopolitan		6.00	57	t
417	Cuba Libre		6.00	57	t
418	Daiquiri		6.00	57	t
419	Espresso Martini		6.00	57	t
420	French Connection		6.00	57	t
421	Gin Fizz		6.00	57	t
422	Godfather		6.00	57	t
423	Godmother		6.00	57	t
424	Golden Cadillac		6.00	57	t
425	Grasshopper		6.00	57	t
426	Horses Neck		6.00	57	t
427	Jamaica		6.00	57	t
428	Japan Ice		6.00	57	t
429	John Collins		6.00	57	t
430	Kir		6.00	57	t
431	Kir Royal		6.00	57	t
432	Long Island		6.00	57	t
433	Manhattan		6.00	57	t
434	Mar Dei Caraibi		6.00	57	t
435	Margarita		6.00	57	t
436	Martini Cocktail		6.00	57	t
437	Martini Sweet		6.00	57	t
438	Martini Vodka		6.00	57	t
439	Melon Ball		6.00	57	t
440	Mojito		6.00	57	t
441	Negroni		6.00	57	t
442	Negroski		6.00	57	t
443	Passion Fruit		6.00	57	t
444	Piña Colada		6.00	57	t
445	Red Devil		6.00	57	t
446	Rusty Nail		6.00	57	t
447	Screwdriver		6.00	57	t
448	Sex on the beach		6.00	57	t
449	Sidecar		6.00	57	t
450	Sweet Years		6.00	57	t
451	Tequila Sunrise		6.00	57	t
452	Whiskey Sour		6.00	57	t
453	White Russian		6.00	57	t
454	Woo Woo		6.00	57	t
455	Arcobaleno		4.00	58	t
456	Assenzio		4.00	58	t
457	B-52		4.00	58	t
458	Balena Bianca		4.00	58	t
459	Black & White		4.00	58	t
460	Blowjob		4.00	58	t
461	Cervelletto		4.00	58	t
462	Estintore		4.00	58	t
463	Flamin' Dr. Pepper		4.00	58	t
464	Hulk		4.00	58	t
465	Lager-Bomb		4.00	58	t
466	Kamikaze		4.00	58	t
467	Misterbilly Shot		4.00	58	t
468	Nasa		4.00	58	t
469	Panfragola		4.00	58	t
470	Pantera Rosa		4.00	58	t
471	Patron cafè		4.00	58	t
472	Pokemon Ball		4.00	58	t
473	Spagnola		4.00	58	t
474	Tequila Boom Boom		4.00	58	t
475	Tequila (sale e limone)		4.00	58	t
476	Tequila scura (arancia e cannella)		4.00	58	t
477	Moscow Mule		7.00	60	t
478	Greek Mule		7.00	60	t
479	Mediterran Mule		7.00	60	t
480	London Mule		7.00	60	t
481	Carribbean Mule		7.00	60	t
482	Mexican Mule		7.00	60	t
485	Waffel Cioccolato Bianco		4.00	61	t
486	Waffel Frutti di Bosco		5.00	61	t
487	Waffel Ferrero Roches		5.00	61	t
491	Crepes Cioccolato bianco		3.00	61	t
489	Waffel Cioccolato Bianco e Nero		4.50	61	t
490	Waffel Cioccolato Bianco e Mandorle		5.00	61	t
492	Crepes Frutti di Bosco		4.00	61	t
493	Crepes Ferrero Roches		4.00	61	t
494	Crepes Cioccolato Bianco e Mandorle		4.00	61	t
495	Crepes Cioccolato Bianco e Nero		4.50	61	t
496	Gelato Fritto alla Vaniglia		4.50	62	t
497	Pepita Gusto Bonet		5.00	62	t
498	Pepita Gusto Pastiera		5.00	62	t
499	Pepita Gusto Crema		5.00	62	t
500	Misto Pepite		6.00	62	t
501	Semifreddo al Torroncino		5.00	63	t
502	Cheescake Frutti di Bosco		5.00	63	t
503	Cheescake Oreo		5.00	63	t
504	Mousse al Cioccolato		5.00	63	t
505	Tortino Cocco&Nutella		5.00	63	t
506	Gelato Vaniglia		5.00	63	t
507	Affogato al Caffè con panna		6.00	63	t
508	Affogato all'Amarena con panna		7.00	63	t
509	Affogato Bayles & Kalhua con panna		8.00	63	t
510	Dessert Del Giorno		6.00	63	t
511	Amaro Del Capo		5.00	64	t
512	Averna		5.00	64	t
513	Fernet Branca		5.00	64	t
514	Finocchietto		5.00	64	t
515	Jagermeister		5.00	64	t
516	Limoncello		5.00	64	t
517	Liquirizia		5.00	64	t
518	Lucano		5.00	64	t
519	Mandarinetto		5.00	64	t
520	Montenegro		5.00	64	t
521	Ramazzotti		5.00	64	t
522	Unicum		5.00	64	t
523	Tresolitre Berta		9.00	65	t
524	Bric(de)Gaian Berta		9.00	65	t
525	Roccanivo Berta		9.00	65	t
526	Monprà Berta		9.00	65	t
527	Grappa Nardini		5.00	65	t
528	Grappa Nonino		5.00	65	t
529	coperto		1.00	66	t
530	pranzo convenzionato		8.00	67	t
531	poco convenzionato		12.00	67	t
532	cena		15.00	67	t
534	margherita		7.50	69	t
\.


--
-- Name: courses_categoryid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.courses_categoryid_seq', 69, true);


--
-- Name: courses_courseid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.courses_courseid_seq', 535, true);


--
-- Data for Name: customerdata; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.customerdata (customerdataid, data, name) FROM stdin;
9	sdfinserire qui tutti i dati dell'aziendasdfsafasfd  ooooXXXXYYYY	asfasfuHHHHHHHH
10	biribiribi	zut
11	sdasfdasdfasdfasdfasdsadfasdfasdf	dasdfasdfasdfasdf
12	pranzo 	tonino
\.


--
-- Name: customerdata_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.customerdata_id_seq', 12, true);


--
-- Data for Name: defaultactionablestates; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.defaultactionablestates (defaultactionablestatesid, stateid) FROM stdin;
30	1
\.


--
-- Name: defaulwaiteractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.defaulwaiteractionablestates_seq', 30, true);


--
-- Data for Name: enablers; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.enablers (enablersid, roleid, stateid, categoryid) FROM stdin;
160	25	1	49
161	25	2	49
164	26	1	56
165	26	2	56
166	26	6	56
167	25	1	47
168	25	2	47
169	25	6	47
170	25	1	48
171	25	2	48
172	25	6	48
176	25	1	42
177	25	2	42
178	25	6	42
182	23	1	43
183	25	1	41
184	25	2	41
185	25	6	41
186	25	1	43
187	25	2	43
188	2	1	43
189	2	2	43
190	2	6	43
\.


--
-- Name: enablers_elablersid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.enablers_elablersid_seq', 190, true);


--
-- Name: incredientdecrementid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.incredientdecrementid_seq', 270, true);


--
-- Data for Name: ingredient; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredient (ingredientid, ingredientcategoryid, name, description, visibility, allergen, updateavailabilityflag, availablequantity, checkavailabilityflag, unitmeasure) FROM stdin;
148	48	cotoletta		t	f	f	0.00	f	unità
150	48	cordon blue		t	f	f	0.00	f	unità
152	50	bresaola		t	f	f	0.00	f	unità
159	47	pan brioche		t	f	f	0.00	f	unità
175	47	piadina		t	f	f	0.00	f	gr
151	52	salsa rosa		t	f	f	0.00	f	gr
176	47	toast		t	f	f	0.00	f	unità
173	52	salsa piccante		t	f	f	0.00	f	gr
177	53	panna		t	f	f	0.00	f	gr
115	48	hamburger		t	f	f	-7.00	f	unità
178	54	arancino		t	f	f	0.00	f	unità
153	48	carne di manzo		t	f	f	0.00	f	unità
136	52	barbecue		t	f	f	0.00	f	gr
137	48	hot dog		t	f	f	0.00	f	unità
155	48	polpettine di scottona		t	f	f	0.00	f	unità
143	48	roast beef		t	f	f	0.00	f	unità
140	51	friarielli		t	f	f	0.00	f	gr
170	48	hamburger di pollo		t	f	f	0.00	f	unità
171	48	hamburger di angus		t	f	f	0.00	f	unità
172	48	hamburger di chianina		t	f	f	0.00	f	unità
139	48	salsiccia		t	f	f	0.00	f	unità
130	48	wurstel		t	f	f	0.00	f	unità
165	48	wurstel a pezzi		t	f	f	0.00	f	unità
138	49	mozzarella		t	f	f	0.00	f	gr
135	52	mayonese		t	f	f	0.00	f	gr
118	51	patate		t	f	f	0.00	f	gr
133	51	insalata coleslaw		t	f	f	0.00	f	gr
146	50	pancetta		t	f	f	0.00	f	gr
154	51	peperoni		t	f	f	0.00	f	gr
149	51	parmigiana di melanzan		t	f	f	0.00	f	gr
132	51	cipolla sfumata		t	f	f	0.00	f	gr
127	52	salse		t	f	f	0.00	f	gr
167	51	cipolla		t	f	f	0.00	f	gr
145	49	scaglie di parmigiano		t	f	f	0.00	f	gr
161	47	saltimbocca		t	f	f	0.00	f	unità
160	47	sfilatino		t	f	f	0.00	f	gr
119	49	sottiletta		t	f	f	0.00	f	unità
158	52	salsa barbecue		t	f	f	0.00	f	gr
157	51	patate al ceddar		t	f	f	0.00	f	gr
169	48	frittata di cipolle		t	f	f	0.00	f	unità
116	51	insalata		t	f	f	0.00	f	unità
117	51	pomodoro		t	f	f	0.00	f	unità
120	50	prosciutto cotto		t	f	f	0.00	f	unità
124	50	prosciutto crudo		t	f	f	0.00	f	unità
141	52	crema di funghi porcini		t	f	f	0.00	f	unità
123	50	speck		t	f	f	0.00	f	unità
121	49	provola		t	f	f	0.00	f	unità
147	51	melanzane alla griglia		t	f	f	0.00	f	unità
126	51	rucola		t	f	f	0.00	f	unità
131	49	ceddar		t	f	f	0.00	f	unità
128	49	gorgonzola		t	f	f	0.00	f	unità
125	51	funghi		t	f	f	0.00	f	unità
166	51	cipolla cruda		t	f	f	0.00	f	unità
174	51	zucchine alla griglia		t	f	f	0.00	f	unità
164	52	salsa tonnata		t	f	f	0.00	f	unità
134	50	bacon		t	f	f	0.00	f	unità
122	51	melanzane a funghetti		t	f	f	0.00	f	unità
142	51	patate al rosmarino		t	f	f	0.00	f	unità
156	51	anelli di cipolla		t	f	f	0.00	f	unità
129	48	uova		t	f	f	0.00	f	unità
144	48	petto di pollo		t	f	f	0.00	f	unità
\.


--
-- Name: ingredient_categoryid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredient_categoryid_seq', 55, true);


--
-- Data for Name: ingredientcategory; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientcategory (ingredientcategoryid, name, description, visibility) FROM stdin;
51	verdure		t
47	pane		t
49	formaggi		t
48	carne		t
52	salse		t
50	salumi		t
53	dolci		t
54	fritti		t
\.


--
-- Data for Name: ingredientcourse; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientcourse (ingredientcourseid, courseid, ingredientid, quantity) FROM stdin;
142	245	116	\N
143	245	117	\N
145	247	118	\N
148	246	120	\N
149	246	125	\N
151	250	123	\N
152	250	121	\N
154	251	121	\N
155	251	124	\N
156	251	126	\N
159	254	115	2.00
160	254	116	\N
163	255	115	2.00
164	255	121	\N
166	255	124	\N
167	255	116	\N
169	255	117	\N
172	256	146	\N
173	256	129	\N
174	256	118	\N
177	257	120	\N
178	257	116	\N
179	257	117	\N
180	257	125	\N
181	257	130	\N
182	257	127	\N
184	258	131	\N
185	258	134	\N
186	258	118	\N
187	258	158	\N
188	261	137	\N
189	261	116	\N
190	261	117	\N
191	262	137	\N
192	262	118	\N
193	263	137	\N
194	263	118	\N
195	263	138	\N
196	264	139	\N
197	264	140	\N
198	265	139	\N
199	265	118	\N
200	267	139	\N
201	267	121	\N
202	267	142	\N
203	267	141	\N
204	245	159	\N
206	247	159	1.00
207	246	159	1.00
209	248	138	\N
210	248	124	\N
211	248	125	\N
212	248	159	\N
215	249	121	\N
216	249	122	\N
217	249	159	\N
218	251	159	\N
220	252	159	\N
221	252	119	2.00
222	252	118	\N
223	252	127	\N
224	253	128	\N
225	253	124	\N
226	253	118	\N
227	253	159	\N
228	254	117	\N
229	255	159	\N
230	256	159	\N
231	258	159	\N
233	259	159	\N
234	259	116	\N
235	259	117	\N
236	259	138	\N
237	259	118	\N
239	261	159	\N
240	263	159	\N
241	264	159	\N
242	265	159	\N
243	267	159	\N
244	268	143	\N
245	268	116	\N
246	268	117	\N
247	268	164	\N
248	268	159	\N
249	269	143	\N
250	269	116	\N
251	269	117	\N
252	269	118	\N
253	269	159	\N
254	270	143	\N
255	270	116	\N
256	270	117	\N
257	270	125	\N
258	270	159	\N
259	271	143	\N
260	271	121	\N
261	271	118	\N
262	271	159	\N
263	272	144	\N
264	272	116	\N
265	272	117	\N
266	272	125	\N
267	272	159	\N
268	273	144	\N
269	273	125	\N
271	273	118	\N
272	273	159	\N
273	274	144	\N
274	274	126	\N
275	274	145	\N
276	274	159	\N
277	275	144	\N
278	275	121	\N
279	275	116	\N
280	275	118	\N
281	275	159	\N
282	276	144	\N
283	276	146	\N
284	276	128	\N
285	276	159	\N
286	277	144	\N
287	277	116	\N
288	277	147	\N
289	277	159	\N
290	278	148	\N
291	278	118	\N
292	278	159	\N
293	279	148	\N
294	279	122	\N
295	279	159	\N
296	280	148	\N
297	280	149	\N
298	280	159	\N
299	281	148	\N
301	281	120	\N
302	281	118	\N
303	281	159	\N
304	282	149	\N
305	282	159	\N
307	283	116	\N
308	283	118	\N
309	283	159	\N
310	283	150	\N
311	284	147	\N
312	284	124	\N
313	284	121	\N
314	284	159	\N
315	285	123	\N
316	285	138	\N
317	285	126	\N
318	285	173	\N
319	285	159	\N
320	286	124	\N
321	286	126	\N
322	286	121	\N
323	286	117	\N
324	286	151	\N
325	286	159	\N
326	287	146	\N
328	287	121	\N
329	287	142	\N
330	287	159	\N
331	288	152	\N
332	288	126	\N
144	247	115	1.00
333	288	145	\N
334	288	159	\N
335	289	153	\N
336	289	154	\N
337	289	167	\N
338	289	138	\N
339	289	159	\N
340	372	155	\N
341	372	121	\N
342	372	134	\N
343	372	156	\N
344	372	157	\N
345	372	158	\N
346	372	159	\N
219	252	115	1.00
347	245	115	1.00
146	246	115	1.00
147	246	119	1.00
150	250	115	1.00
153	251	115	1.00
157	253	115	1.00
170	256	115	1.00
171	256	119	1.00
175	257	115	1.00
176	257	119	1.00
183	258	115	1.00
208	248	115	1.00
232	259	115	1.00
238	259	169	1.00
270	273	119	1.00
300	281	119	1.00
350	291	138	\N
351	291	117	1.00
352	291	159	1.00
353	249	115	1.00
354	266	139	1.00
355	266	121	1.00
356	266	125	1.00
357	266	118	\N
359	534	138	\N
360	534	117	1.00
\.


--
-- Name: ingredientcourseid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientcourseid_seq', 362, true);


--
-- Data for Name: ingredientdecrement; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientdecrement (ingredientdecrementid, orderitemid, typeofdecrement, presumednormalquantity, recordedquantity, preparatorid, registrationtime, ingredientid) FROM stdin;
215	1305	NORMAL	1.00	\N	2	2019-02-05 10:20:21.631836	115
216	1306	NORMAL	1.00	\N	2	2019-02-05 10:20:21.697169	115
218	1309	NORMAL	1.00	0.00	2	2019-02-05 10:39:00.937359	115
219	1310	NORMAL	1.00	0.00	2	2019-02-05 10:39:00.937359	115
220	1311	NORMAL	1.00	\N	2	2019-02-05 11:19:28.69528	115
221	1312	NORMAL	1.00	\N	2	2019-02-05 11:20:21.970358	115
223	1315	NORMAL	1.00	0.00	2	2019-02-05 11:24:45.010389	115
224	1316	NORMAL	1.00	0.00	2	2019-02-05 11:24:45.010389	115
226	1319	NORMAL	1.00	0.00	2	2019-02-05 11:40:18.706502	115
227	1320	NORMAL	1.00	0.00	2	2019-02-05 11:40:18.706502	115
231	1324	NORMAL	1.00	\N	2	2019-02-07 15:13:39.603361	115
235	1335	NORMAL	1.00	\N	2	2019-02-08 19:26:02.819034	115
243	1341	NORMAL	1.00	\N	2	2019-02-09 16:08:15.807544	115
245	1348	NORMAL	1.00	0.00	2	2019-02-11 19:43:55.67621	115
246	1349	NORMAL	1.00	0.00	2	2019-02-11 19:43:55.67621	115
247	1350	NORMAL	1.00	\N	2	2019-02-12 16:09:30.981905	115
250	1356	NORMAL	1.00	\N	2	2019-02-27 13:16:47.03942	115
251	1361	NORMAL	1.00	\N	2	2019-02-28 21:13:21.408222	115
254	1364	NORMAL	1.00	0.00	2	2019-03-04 22:10:43.874849	115
255	1365	NORMAL	1.00	0.00	2	2019-03-04 22:10:43.874849	115
149	1111	PER_PREZZO_INGREDIENTE	1.00	\N	2	2019-01-13 16:36:25.783321	115
150	1116	NORMAL	1.00	\N	2	2019-01-14 12:05:39.929996	115
151	1121	NORMAL	3.00	\N	2	2019-01-14 21:06:24.765474	115
258	1369	NORMAL	1.00	\N	2	2019-03-18 12:46:57.211912	117
261	1374	NORMAL	1.00	\N	2	2019-04-09 17:01:29.087377	115
262	1375	unità	3.00	\N	2	2019-04-09 17:01:29.243	156
263	1358	NORMAL	1.00	\N	2	2019-04-09 17:02:39.560058	115
264	1353	NORMAL	1.00	\N	2	2019-04-09 17:03:32.322376	115
265	1376	NORMAL	1.00	\N	2	2019-04-09 17:16:20.957417	115
163	1144	NORMAL	1.00	0.00	2	2019-01-18 10:01:11.897693	115
164	1145	NORMAL	1.00	0.00	2	2019-01-18 10:01:11.897693	115
165	1146	NORMAL	1.00	0.00	2	2019-01-18 10:01:11.897693	115
166	1147	NORMAL	1.00	0.00	2	2019-01-18 10:01:11.897693	115
267	1379	NORMAL	1.00	0.00	2	2019-04-09 17:24:26.662179	115
268	1380	NORMAL	1.00	0.00	2	2019-04-09 17:24:26.662179	115
269	1354	NORMAL	1.00	\N	2	2019-04-09 17:50:33.315925	115
270	1355	NORMAL	1.00	\N	2	2019-04-11 10:11:28.369954	115
176	1175	NORMAL	1.00	\N	2	2019-01-21 18:15:14.192077	115
199	1264	NORMAL	1.00	0.00	2	2019-02-04 10:17:41.121799	115
200	1265	NORMAL	1.00	0.00	2	2019-02-04 10:17:41.121799	115
207	1296	NORMAL	1.00	\N	2	2019-02-05 09:31:22.912733	115
208	1297	NORMAL	1.00	\N	2	2019-02-05 09:31:23.060807	119
209	1298	NORMAL	1.00	\N	2	2019-02-05 09:52:46.269008	115
210	1299	NORMAL	1.00	\N	2	2019-02-05 09:53:30.933129	115
211	1299	NORMAL	2.00	\N	2	2019-02-05 09:53:30.933129	119
213	1303	NORMAL	1.00	0.00	2	2019-02-05 10:12:33.810261	115
214	1304	NORMAL	1.00	0.00	2	2019-02-05 10:12:33.810261	115
\.


--
-- Name: ingredientid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientid_seq', 179, true);


--
-- Data for Name: ingredientincrement; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientincrement (ingredientincrementid, ingredientid, comment, unitofmeasure, quantity, userid, registrationtime) FROM stdin;
7	115	trtr	unità	2.00	2	2019-01-09 17:24:50.985921
8	115	2	unità	9.00	2	2019-01-11 16:00:12.970275
9	115	w	unità	4.00	2	2019-01-11 16:28:11.276276
10	115	w	unità	2.00	2	2019-01-11 16:32:11.303674
11	115	ss	unità	9.00	2	2019-01-11 16:38:00.073771
12	115	1	unità	1.00	2	2019-01-11 16:42:30.965986
13	115	a	unità	4.00	2	2019-01-11 16:43:02.087559
14	115	e	unità	1.00	2	2019-01-11 16:51:11.003974
15	115	2	unità	1.00	2	2019-01-11 17:32:16.479474
16	115	2	unità	1.00	2	2019-01-12 09:45:32.579603
17	115	2	unità	4.00	2	2019-01-12 13:33:23.135357
18	115	e	unità	3.00	2	2019-01-12 13:36:01.171426
19	115	2	unità	1.00	2	2019-01-12 13:38:50.278144
20	115	3	unità	2.00	2	2019-01-12 13:40:25.960859
21	115	2	unità	8.00	2	2019-01-12 13:53:12.744629
22	115	1	unità	4.00	2	2019-01-12 13:53:57.961271
23	115	2	unità	4.00	2	2019-01-12 13:58:59.471626
24	115	5	unità	1.00	2	2019-01-12 13:59:37.089735
25	115	1	unità	1.00	2	2019-01-12 14:30:10.036977
26	115	s	unità	3.00	2	2019-01-12 14:54:47.550794
27	115	w	unità	2.00	2	2019-01-12 15:00:32.481974
28	115	r	unità	1.00	2	2019-01-12 15:18:09.179719
29	115	5	unità	2.00	2	2019-01-12 15:21:59.349855
30	115	1	unità	1.00	2	2019-01-13 10:28:14.025722
31	115	s	unità	1.00	2	2019-01-13 10:33:22.342151
32	115	1	unità	5.00	2	2019-01-13 14:16:53.527416
\.


--
-- Name: ingredientincrementid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientincrementid_seq', 32, true);


--
-- Data for Name: ingredientprice; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientprice (ingredientpriceid, ingredientid, quantity, isdefaultadd, isdefaultsubtract, addprice, subtractprice) FROM stdin;
19	128	10.00	t	t	1.00	1.00
78	117	1.00	t	t	0.50	0.50
22	164	10.00	t	t	1.00	1.00
66	131	1.00	t	t	1.00	1.00
56	119	1.00	t	t	0.50	0.50
87	159	1.00	t	t	1.00	1.00
89	178	1.00	t	t	1.00	1.00
95	115	1.00	t	t	3.00	3.00
90	134	1.00	t	t	1.50	1.50
62	123	1.00	t	t	1.00	1.00
97	170	1.00	t	t	7.00	7.00
68	174	1.00	t	t	1.00	1.00
15	147	10.00	t	t	1.00	1.00
63	146	1.00	t	t	1.00	1.00
96	172	1.00	t	t	7.00	7.00
65	121	1.00	t	t	1.00	1.00
59	120	1.00	t	t	1.00	1.00
20	125	10.00	t	t	1.00	1.00
32	129	1.00	t	t	2.00	2.00
67	126	1.00	t	t	1.00	1.00
13	145	10.00	t	t	1.00	1.00
83	138	1.00	t	t	1.00	1.00
40	152	10.00	t	t	4.00	4.00
60	124	1.00	t	t	1.00	1.00
93	148	1.00	t	t	3.00	3.00
70	139	1.00	t	t	3.00	3.00
94	169	1.00	t	t	1.00	1.00
29	167	10.00	t	t	1.00	1.00
92	150	1.00	t	t	3.00	3.00
57	116	1.00	t	t	0.50	0.50
24	165	10.00	t	t	1.00	1.00
38	143	1.00	t	t	3.00	3.00
84	175	1.00	t	t	1.00	1.00
26	122	10.00	t	t	1.50	1.50
61	141	1.00	t	t	1.00	1.00
91	153	1.00	t	t	3.00	3.00
35	144	1.00	t	t	3.00	3.00
27	142	10.00	t	t	1.50	1.50
31	161	1.00	t	t	2.00	2.00
88	177	1.00	t	t	1.00	1.00
23	166	10.00	t	t	1.00	1.00
42	171	1.00	t	t	6.00	6.00
85	176	1.00	t	t	1.00	1.00
28	156	10.00	t	t	1.50	1.50
\.


--
-- Name: ingredientpriceid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientpriceid_seq', 98, true);


--
-- Data for Name: invoices; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.invoices (invoicesid, data, invoicenumber, customerdataid, date, suborderid, orderid) FROM stdin;
22	fattura: n.1\n\ndata:18/01/2019 15:32:55\n\ninserire qui tutti i dati dell'azienda\n\n\ntotale:                12.00                                   \n\n\nimponibile: 10.91                         \n\nIVA 10 %: 1.09\n\n	1	\N	2019-01-18 15:32:55.312046	226	\N
23	fattura: n.2\n\ndata:18/01/2019 15:33:14\n\ninserire qui tutti i dati dell'azienda\n\n\ntotale:                12.00                                   \n\n\nimponibile: 10.91                         \n\nIVA 10 %: 1.09\n\n	2	\N	2019-01-18 15:33:14.334807	226	\N
24	fattura: n.3\n\ndata:18/01/2019 15:51:45\n\ninserire qui tutti i dati dell'azienda\n\n\ntotale:                12.00                                   \n\n\nimponibile: 10.91                         \n\nIVA 10 %: 1.09\n\n	3	\N	2019-01-18 15:51:45.695735	226	\N
25	fattura: n.4\n\ndata:18/01/2019 15:51:55\n\ninserire qui tutti i dati dell'azienda\n\n\ntotale:                12.00                                   \n\n\nimponibile: 10.91                         \n\nIVA 10 %: 1.09\n\n	4	\N	2019-01-18 15:51:55.524579	226	\N
26	fattura: n.5\n\ndata:18/01/2019 15:52:14\n\ninserire qui tutti i dati dell'azienda\n\n\ntotale:                12.00                                   \n\n\nimponibile: 10.91                         \n\nIVA 10 %: 1.09\n\n	5	\N	2019-01-18 15:52:14.707129	226	\N
27	fattura: n.6\n\ndata:18/01/2019 15:53:09\n\ninserire qui tutti i dati dell'azienda\n\n\ntotale:                12.00                                   \n\n\nimponibile: 10.91                         \n\nIVA 10 %: 1.09\n\n	6	\N	2019-01-18 15:53:09.195196	229	\N
28	fattura: n.7\n\ndata:18/01/2019 16:13:13\n\ninserire qui tutti i dati dell'azienda\n\n\ntotale:                12.00                                   \n\n\nimponibile: 10.91                         \n\nIVA 10 %: 1.09\n\n	7	\N	2019-01-18 16:13:13.17749	226	\N
36	fattura: n.8\n\ndata:04/02/2019 20:27:10\n\ninserire qui tutti i dati dell'azienda\n\n\ntotale:                8.00                                    \n\n\nimponibile: 7.27                          \n\nIVA 10 %: 0.73\n\n	8	\N	2019-02-04 20:27:10.380249	256	\N
37	fattura: n.9\n\ndata:04/02/2019 20:53:47\n\ninserire qui tutti i dati dell'azienda\n\n\ntotale:                8.00                                    \n\n\nimponibile: 7.27                          \n\nIVA 10 %: 0.73\n\n	9	\N	2019-02-04 20:53:47.836893	257	\N
38	fattura: n.10\n\ndata:04/02/2019 20:53:53\n\ninserire qui tutti i dati dell'azienda\n\n\ntotale:                10.00                                   \n\n\nimponibile: 9.09                          \n\nIVA 10 %: 0.91\n\n	10	\N	2019-02-04 20:53:53.599439	258	\N
39	fattura: n. 11\n\ndata:05/02/2019 09:53:02\n\ninserire qui tutti i dati dell'azienda\n\n\nTotale: 6.00\n\n\nimponibile: 5.45\n\nIVA 10 %: 0.55\n\n	11	\N	2019-02-05 09:53:02.290534	\N	550
40	fattura: n. 12\n\ndata:05/02/2019 10:39:55\n\ninserire qui tutti i dati dell'azienda\n\n\nTotale: 12.00\n\n\nimponibile: 10.91\n\nIVA 10 %: 1.09\n\n	12	\N	2019-02-05 10:39:55.559952	\N	554
41	fattura: n. 13\n\ndata:05/02/2019 11:20:31\n\ninserire qui tutti i dati dell'azienda\n\n\nTotale: 6.00\n\n\nimponibile: 5.45\n\nIVA 10 %: 0.55\n\n	13	\N	2019-02-05 11:20:31.912636	\N	556
44	fattura: n.14\n\ndata:05/02/2019 12:51:09\n\ninserire qui tutti i dati dell'azienda\n\n\ntotale:                6.00                                    \n\n\nimponibile: 5.45                          \n\nIVA 10 %: 0.55\n\n	14	\N	2019-02-05 12:51:09.847079	277	\N
45	fattura: n. 15\n\ndata:09/02/2019 16:20:42\n\ninserire qui tutti i dati dell'azienda\n\n\nTotale: 15.00\n\n\nimponibile: 13.64\n\nIVA 10 %: 1.36\n\n	15	\N	2019-02-09 16:20:42.232241	\N	568
\.


--
-- Name: invoicesid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.invoicesid_seq', 45, true);


--
-- Data for Name: observers; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.observers (observersid, stateid, roleid, categoryid) FROM stdin;
180	1	25	49
181	2	25	49
184	1	26	56
185	2	26	56
186	6	26	56
187	1	25	47
188	2	25	47
189	6	25	47
190	1	25	48
191	2	25	48
192	6	25	48
196	1	25	42
197	2	25	42
198	6	25	42
202	1	23	43
203	1	25	41
204	2	25	41
205	6	25	41
206	1	25	43
207	2	25	43
208	1	2	43
209	2	2	43
210	6	2	43
\.


--
-- Name: observers_observerid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.observers_observerid_seq', 210, true);


--
-- Name: observers_observersid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.observers_observersid_seq', 1, false);


--
-- Data for Name: orderitems; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orderitems (orderitemid, courseid, quantity, orderid, comment, price, stateid, archived, startingtime, closingtime, ordergroupid, hasbeenrejected, suborderid, isinsasuborder, printcount) FROM stdin;
1300	230	1	550		8.00	2	\N	2019-02-05 10:03:19.451669	\N	445	f	263	t	0
1192	209	1	509		10.00	2	\N	2019-01-29 17:59:23.112947	\N	357	f	\N	f	0
1144	249	1	493		6.00	6	\N	2019-01-18 10:01:53.929193	\N	324	f	226	t	1
1356	249	1	572		6.00	2	\N	2019-02-26 13:27:05.988271	\N	478	f	296	t	0
1145	249	1	493		6.00	6	\N	2019-01-18 10:01:53.943121	\N	324	f	226	t	1
1193	494	1	509		4.00	2	\N	2019-01-29 17:59:36.603835	\N	357	f	\N	f	0
1175	249	1	509		6.00	6	\N	2019-01-21 18:14:28.051547	2019-01-27 14:45:07.26341	358	f	\N	f	0
1194	336	1	509		3.00	2	\N	2019-01-29 18:00:06.294702	\N	365	f	\N	f	0
1309	249	1	554		6.00	6	\N	2019-02-05 10:39:06.715586	\N	448	f	0	f	1
1146	249	1	493		6.00	6	\N	2019-01-18 10:01:53.953239	\N	324	f	229	t	1
1147	249	1	493		6.00	6	\N	2019-01-18 10:01:53.963143	\N	324	f	229	t	1
1121	245	3	483		5.00	6	\N	2019-01-14 21:06:14.268821	2019-01-14 21:06:42.870722	315	f	\N	f	0
1111	265	1	473		6.00	6	\N	2019-01-13 16:35:36.281271	2019-01-14 10:37:10.869391	306	f	199	t	0
1310	249	1	554		6.00	6	\N	2019-02-05 10:39:06.721498	\N	448	f	0	f	1
1110	265	1	473		6.00	6	\N	2019-01-13 14:27:24.244124	2019-01-14 10:37:12.123196	305	f	201	t	0
1114	178	1	473		7.00	6	\N	2019-01-14 10:36:28.819824	2019-01-14 10:36:56.421004	308	f	201	t	0
1064	245	1	465		5.00	1	\N	2019-01-11 16:51:30.541916	2019-01-11 16:51:55.17134	270	t	\N	f	0
1065	245	1	466		5.00	6	\N	2019-01-11 16:57:23.793231	2019-01-11 16:57:52.255694	271	f	\N	f	0
1115	401	1	475		6.00	6	\N	2019-01-14 12:04:42.57157	2019-01-14 12:04:51.051614	309	f	\N	f	0
1116	245	1	476		5.00	6	\N	2019-01-14 12:05:29.436712	2019-01-14 12:05:41.567351	310	f	\N	f	0
1294	531	1	548		11.00	6	\N	2019-02-04 20:25:38.497904	\N	439	f	255	t	1
1293	509	1	548		8.00	2	\N	2019-02-04 20:25:00.47879	\N	437	f	256	t	0
1296	249	1	549		6.00	2	\N	2019-02-05 09:31:14.086524	\N	442	f	259	t	0
1297	273	1	549		6.50	2	\N	2019-02-05 09:31:19.815769	\N	442	f	260	t	0
1354	249	1	573		6.00	2	\N	2019-02-26 13:26:19.206234	\N	476	f	0	f	0
1134	302	1	489		11.00	2	\N	2019-01-15 17:56:28.870077	\N	320	f	\N	f	0
1133	209	1	489		10.00	6	\N	2019-01-15 17:54:14.316442	2019-01-15 17:56:14.519074	320	f	217	t	0
1376	258	1	585		7.50	2	\N	2019-04-09 17:16:18.100664	\N	497	f	313	f	0
1180	209	1	509		10.00	6	\N	2019-01-25 09:07:47.819997	2019-01-27 14:45:10.435694	362	f	\N	f	0
1324	249	1	562		6.00	2	\N	2019-02-05 16:02:44.662866	\N	456	f	\N	f	0
1319	249	1	558		6.00	6	\N	2019-02-05 11:40:25.81386	\N	452	f	306	t	1
1365	249	1	578		6.00	6	\N	2019-03-04 22:10:54.619279	\N	486	f	0	f	1
1364	249	1	578		6.00	6	\N	2019-03-04 22:10:54.61094	\N	486	f	0	f	1
1361	249	1	577		6.00	2	\N	2019-02-28 21:13:18.580297	\N	482	f	0	f	0
1341	249	1	567		6.00	2	\N	2019-02-09 16:04:21.872045	\N	468	f	290	t	0
1367	524	1	579		9.00	2	\N	2019-03-09 20:22:35.922747	\N	488	f	\N	f	0
1350	249	1	570		6.00	2	\N	2019-02-12 16:09:28.414188	\N	473	f	\N	f	0
1346	530	1	568	\N	8.00	6	\N	2019-02-11 11:57:42.677406	\N	471	f	0	f	1
1320	249	1	558		6.00	6	\N	2019-02-05 11:40:25.822224	\N	452	f	0	f	1
1360	196	1	572		8.00	1	\N	2019-02-27 15:07:09.723714	\N	481	f	\N	f	0
1358	249	1	559		6.00	2	\N	2019-02-27 14:53:39.143692	\N	480	f	\N	f	0
1188	311	1	509		6.50	6	\N	2019-01-26 17:46:34.180612	2019-01-29 17:58:51.107422	364	f	\N	f	0
1187	209	1	509		10.00	6	\N	2019-01-26 17:46:17.520969	2019-01-29 17:58:52.214516	363	f	\N	f	0
1189	209	2	509		10.00	6	\N	2019-01-27 13:35:49.913598	2019-01-29 17:58:53.852619	364	f	\N	f	0
1355	249	1	574		6.00	2	\N	2019-02-26 13:26:33.887178	\N	477	f	316	t	0
1298	249	1	550		6.00	2	\N	2019-02-05 09:52:42.295868	\N	443	f	262	t	0
1357	380	1	576		15.00	2	\N	2019-02-27 13:21:27.278281	\N	479	f	295	t	0
1303	249	1	552		6.00	6	\N	2019-02-05 10:12:42.336635	\N	446	f	264	t	1
1304	249	1	552		6.00	6	\N	2019-02-05 10:12:42.342944	\N	446	f	265	t	1
1305	249	1	553		6.00	2	\N	2019-02-05 10:20:10.127693	\N	447	f	266	t	0
1306	258	1	553		7.50	2	\N	2019-02-05 10:20:16.343145	\N	447	f	267	t	0
1311	249	1	555		6.00	2	\N	2019-02-05 11:19:25.384449	\N	449	f	270	t	0
1312	249	1	556		6.00	2	\N	2019-02-05 11:20:04.49114	\N	450	f	\N	f	0
1295	530	1	547	\N	8.00	6	\N	2019-02-04 20:38:24.040331	\N	434	f	\N	f	1
1315	249	1	557		6.00	6	\N	2019-02-05 11:24:54.366769	\N	451	f	271	t	1
1316	249	1	557		6.00	6	\N	2019-02-05 11:24:54.375729	\N	451	f	272	t	1
1288	530	1	546		8.00	6	\N	2019-02-04 20:00:24.201758	\N	440	f	257	t	1
1265	249	1	545		6.00	6	\N	2019-02-04 10:17:53.513035	\N	418	f	239	t	1
1264	249	1	545		6.00	6	\N	2019-02-04 10:17:53.507265	\N	418	f	240	t	1
1289	530	1	546		10.00	6	\N	2019-02-04 20:02:32.092364	\N	441	f	258	t	1
1299	252	1	551		6.00	2	\N	2019-02-05 09:53:27.413505	\N	444	f	261	t	0
1369	534	1	580		6.50	2	\N	2019-03-18 12:41:57.624113	\N	489	f	308	t	0
1335	249	1	566		6.00	2	\N	2019-02-08 19:25:58.284943	\N	465	f	\N	f	0
1336	524	1	566		9.00	2	\N	2019-02-08 19:26:00.992673	\N	465	f	\N	f	0
1374	249	1	583		6.00	2	\N	2019-04-09 16:56:11.320557	\N	493	f	\N	f	0
1375	199	1	583		14.50	2	\N	2019-04-09 16:56:23.262277	\N	493	f	\N	f	0
1353	249	1	571		6.00	2	\N	2019-02-26 13:11:42.190775	\N	475	f	\N	f	0
1349	249	1	569		6.00	6	\N	2019-02-11 19:44:08.589862	\N	472	f	0	f	1
1348	249	1	569		6.00	6	\N	2019-02-11 19:44:08.579056	\N	472	f	0	f	1
1379	249	1	573		6.00	6	\N	2019-04-09 17:24:39.413014	\N	496	f	314	f	1
1380	249	1	573		6.00	6	\N	2019-04-09 17:24:39.421883	\N	496	f	314	f	1
\.


--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orderitems_orderitemid_seq', 1382, true);


--
-- Data for Name: orderitemstates; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orderitemstates (orderitemstatesid, orderitemid, stateid, startingtime) FROM stdin;
2118	1110	2	2019-01-13 14:28:01.788085
2119	1111	1	2019-01-13 16:35:36.281271
2120	1111	2	2019-01-13 16:36:25.734759
2123	1114	1	2019-01-14 10:36:28.819824
2124	1114	2	2019-01-14 10:36:50.902501
2031	1064	1	2019-01-11 16:51:30.541916
2032	1064	2	2019-01-11 16:51:42.117097
2033	1064	6	2019-01-11 16:51:55.17134
2034	1065	1	2019-01-11 16:57:23.793231
2035	1065	2	2019-01-11 16:57:38.90611
2036	1065	6	2019-01-11 16:57:52.255694
2125	1114	6	2019-01-14 10:36:56.421004
2126	1111	6	2019-01-14 10:37:10.869391
2127	1110	6	2019-01-14 10:37:12.123196
2128	1115	1	2019-01-14 12:04:42.57157
2129	1115	2	2019-01-14 12:04:46.939962
2130	1115	6	2019-01-14 12:04:51.051614
2131	1116	1	2019-01-14 12:05:29.436712
2132	1116	2	2019-01-14 12:05:39.903768
2133	1116	6	2019-01-14 12:05:41.567351
2441	1296	1	2019-02-05 09:31:14.086524
2442	1297	1	2019-02-05 09:31:19.815769
2443	1296	2	2019-02-05 09:31:22.792502
2444	1297	2	2019-02-05 09:31:23.044472
2142	1121	1	2019-01-14 21:06:14.268821
2454	1304	1	2019-02-05 10:12:25.30057
2145	1121	2	2019-01-14 21:06:24.674125
2146	1121	6	2019-01-14 21:06:42.870722
2455	1303	2	2019-02-05 10:12:33.726971
2456	1303	1	2019-02-05 10:12:25.30057
2457	1304	2	2019-02-05 10:12:33.726971
2458	1305	1	2019-02-05 10:20:10.127693
2459	1306	1	2019-02-05 10:20:16.343145
2245	1175	1	2019-01-21 18:14:28.051547
2460	1305	2	2019-02-05 10:20:21.600068
2461	1306	2	2019-02-05 10:20:21.672044
2248	1175	2	2019-01-21 18:15:14.168683
2483	1319	1	2019-02-05 11:40:13.6367
2253	1180	1	2019-01-25 09:07:47.819997
2161	1133	1	2019-01-15 17:54:14.316442
2162	1133	2	2019-01-15 17:54:46.419989
2163	1133	6	2019-01-15 17:56:14.519074
2164	1134	1	2019-01-15 17:56:28.870077
2165	1134	2	2019-01-15 17:56:38.582189
2254	1180	2	2019-01-25 09:10:30.553772
2484	1319	2	2019-02-05 11:40:18.623853
2485	1320	2	2019-02-05 11:40:18.623853
2486	1320	1	2019-02-05 11:40:13.6367
2494	1324	2	2019-02-07 15:13:39.466773
2265	1187	1	2019-01-26 17:46:17.520969
2266	1188	1	2019-01-26 17:46:34.180612
2268	1187	2	2019-01-27 13:35:20.386779
2269	1189	1	2019-01-27 13:35:49.913598
2511	1335	1	2019-02-08 19:25:58.284943
2184	1146	1	2019-01-18 10:01:03.352663
2185	1147	2	2019-01-18 10:01:11.800262
2186	1146	6	2019-01-18 10:01:28.383011
2187	1144	2	2019-01-18 10:01:11.800262
2188	1145	2	2019-01-18 10:01:11.800262
2189	1144	6	2019-01-18 10:01:28.383011
2190	1147	1	2019-01-18 10:01:03.352663
2191	1145	1	2019-01-18 10:01:03.352663
2192	1144	1	2019-01-18 10:01:03.352663
2193	1147	6	2019-01-18 10:01:28.383011
2194	1146	2	2019-01-18 10:01:11.800262
2195	1145	6	2019-01-18 10:01:28.383011
2512	1336	1	2019-02-08 19:26:00.992673
2273	1175	6	2019-01-27 14:45:07.26341
2513	1335	2	2019-02-08 19:26:02.79127
2275	1180	6	2019-01-27 14:45:10.435694
2514	1336	2	2019-02-08 19:26:02.856933
2277	1188	2	2019-01-29 17:58:28.813025
2278	1189	2	2019-01-29 17:58:28.973502
2279	1188	6	2019-01-29 17:58:51.107422
2280	1187	6	2019-01-29 17:58:52.214516
2281	1189	6	2019-01-29 17:58:53.852619
2282	1192	1	2019-01-29 17:59:23.112947
2283	1192	2	2019-01-29 17:59:26.262878
2284	1193	1	2019-01-29 17:59:36.603835
2285	1193	2	2019-01-29 17:59:45.117067
2286	1194	1	2019-01-29 18:00:06.294702
2287	1194	2	2019-01-29 18:00:09.72389
2528	1341	2	2019-02-09 16:08:15.780139
2531	1349	1	2019-02-11 19:43:51.917903
2532	1349	2	2019-02-11 19:43:55.537097
2533	1348	1	2019-02-11 19:43:51.917903
2534	1348	2	2019-02-11 19:43:55.537097
2535	1350	1	2019-02-12 16:09:28.414188
2536	1350	2	2019-02-12 16:09:30.823484
2117	1110	1	2019-01-13 14:27:24.244124
2545	1356	2	2019-02-27 13:16:46.57487
2548	1358	1	2019-02-27 14:53:39.143692
2550	1360	1	2019-02-27 15:07:09.723714
2551	1361	1	2019-02-28 21:13:18.580297
2552	1361	2	2019-02-28 21:13:21.271668
2557	1364	1	2019-03-04 22:10:41.050866
2558	1365	2	2019-03-04 22:10:43.84414
2559	1365	1	2019-03-04 22:10:41.050866
2560	1364	2	2019-03-04 22:10:43.84414
2580	1376	1	2019-04-09 17:16:18.100664
2581	1376	2	2019-04-09 17:16:20.931204
2445	1298	1	2019-02-05 09:52:42.295868
2446	1298	2	2019-02-05 09:52:46.148015
2447	1299	1	2019-02-05 09:53:27.413505
2448	1299	2	2019-02-05 09:53:30.910847
2465	1310	1	2019-02-05 10:38:56.166872
2466	1309	1	2019-02-05 10:38:56.166872
2467	1309	2	2019-02-05 10:39:00.81679
2468	1310	2	2019-02-05 10:39:00.81679
2493	1324	1	2019-02-05 16:02:44.662866
2541	1353	1	2019-02-26 13:11:42.190775
2546	1357	1	2019-02-27 13:21:27.278281
2547	1357	2	2019-02-27 13:21:29.517673
2566	1369	1	2019-03-18 12:41:57.624113
2568	1369	2	2019-03-18 12:46:57.189333
2574	1374	1	2019-04-09 16:56:11.320557
2575	1375	1	2019-04-09 16:56:23.262277
2585	1379	1	2019-04-09 17:24:19.969659
2586	1380	2	2019-04-09 17:24:26.581514
2587	1380	1	2019-04-09 17:24:19.969659
2588	1379	2	2019-04-09 17:24:26.581514
2594	1355	2	2019-04-11 10:11:28.131956
2436	1293	1	2019-02-04 20:25:00.47879
2396	1264	2	2019-02-04 10:17:41.039123
2397	1265	2	2019-02-04 10:17:41.039123
2398	1264	1	2019-02-04 10:17:19.685486
2399	1265	1	2019-02-04 10:17:19.685486
2440	1293	2	2019-02-04 20:25:04.541986
2449	1300	1	2019-02-05 10:03:19.451669
2450	1300	2	2019-02-05 10:03:23.603961
2469	1311	1	2019-02-05 11:19:25.384449
2470	1311	2	2019-02-05 11:19:28.559771
2471	1312	1	2019-02-05 11:20:04.49114
2472	1312	2	2019-02-05 11:20:21.945673
2476	1316	2	2019-02-05 11:24:44.984278
2477	1315	2	2019-02-05 11:24:44.984278
2478	1315	1	2019-02-05 11:24:40.209971
2479	1316	1	2019-02-05 11:24:40.209971
2522	1341	1	2019-02-09 16:04:21.872045
2542	1354	1	2019-02-26 13:26:19.206234
2543	1355	1	2019-02-26 13:26:33.887178
2544	1356	1	2019-02-26 13:27:05.988271
2563	1367	1	2019-03-09 20:22:35.922747
2564	1367	2	2019-03-09 20:22:38.739952
2576	1374	2	2019-04-09 17:01:28.969133
2577	1375	2	2019-04-09 17:01:29.221389
2578	1358	2	2019-04-09 17:02:39.544833
2579	1353	2	2019-04-09 17:03:32.307048
2589	1354	2	2019-04-09 17:50:33.07957
\.


--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orderitemstates_orderitemstates_id_seq', 2594, true);


--
-- Data for Name: orderoutgroup; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orderoutgroup (ordergroupid, printcount, orderid, groupidentifier) FROM stdin;
271	1	466	1
410	0	542	1
357	1	509	1
452	1	558	1
365	1	509	2
417	1	544	1
418	1	545	1
456	1	562	1
465	1	566	1
470	1	568	5
469	1	568	1
468	1	567	1
435	0	547	2
471	1	568	99
437	1	548	1
270	1	465	1
472	1	569	1
439	0	548	2
434	1	547	99
440	0	546	2
441	0	546	1
442	1	549	1
443	1	550	1
444	1	551	1
445	1	550	2
305	1	473	1
306	1	473	2
446	1	552	1
308	0	473	3
309	1	475	1
310	0	476	1
447	1	553	1
448	1	554	1
449	1	555	1
450	1	556	1
315	1	483	1
451	1	557	1
409	0	541	1
473	1	570	1
320	1	489	1
478	1	572	1
325	0	493	4
324	1	493	1
479	1	576	1
481	0	572	2
482	1	577	1
485	0	578	3
486	0	578	1
488	1	579	1
489	1	580	1
493	1	583	1
480	1	559	1
475	1	571	1
496	2	573	7
476	1	573	1
497	1	585	8
348	1	500	2
499	1	587	1
477	1	574	1
358	0	509	6
362	1	509	12
363	1	509	11
364	1	509	10
\.


--
-- Name: orderoutgroup_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orderoutgroup_id_seq', 499, true);


--
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orders (orderid, "table", person, ongoing, userid, startingtime, closingtime, voided, archived, total, adjustedtotal, plaintotalvariation, percentagevariataion, adjustispercentage, adjustisplain, forqruserarchived) FROM stdin;
549	1		t	2	2019-02-05 09:31:06.592738	2019-02-05 10:38:27.143158	f	t	12.50	12.50	0.00	0.00	f	f	\N
573	6		t	171	2019-02-26 13:26:12.6657	2019-04-09 17:59:05.320695	f	t	18.00	16.20	0.00	-10.00	t	f	\N
568	3		t	2	2019-02-09 16:04:27.446742	2019-02-12 16:12:30.864535	f	t	8.00	8.00	0.00	0.00	f	f	\N
580	20		t	2	2019-03-18 12:28:20.538695	2019-03-18 12:53:45.693816	f	t	13.50	13.50	0.00	0.00	f	f	\N
579	99		t	2	2019-03-07 17:57:33.824037	2019-03-18 12:54:30.762478	f	t	9.00	9.00	0.00	0.00	f	f	\N
542	2		f	2	2019-02-04 09:14:09.522805	2019-02-04 10:16:51.557688	f	t	0.00	0.00	0.00	0.00	f	f	\N
550	4		t	2	2019-02-05 09:52:38.71838	2019-02-05 10:38:27.948098	f	t	14.00	14.00	0.00	0.00	f	f	\N
559	1		t	2	2019-02-05 12:50:29.973913	2019-04-09 17:03:25.931647	f	t	6.00	6.00	0.00	0.00	f	f	\N
555	8		t	2	2019-02-05 11:19:21.884296	2019-02-05 11:20:35.846106	f	t	6.00	6.00	0.00	0.00	f	f	\N
544	5		t	2	2019-02-04 09:23:27.880259	2019-02-04 10:16:52.853308	f	t	0.00	0.00	0.00	0.00	f	f	\N
551	5		t	2	2019-02-05 09:53:20.869737	2019-02-05 10:38:28.659968	f	t	6.00	6.00	0.00	0.00	f	f	\N
558	1		t	2	2019-02-05 11:40:09.69531	2019-04-09 16:59:05.194093	f	t	12.00	12.00	0.00	0.00	f	f	\N
553	4		f	2	2019-02-05 10:20:06.64575	2019-02-05 10:38:29.293164	f	t	13.50	13.50	0.00	0.00	f	f	\N
546	2		t	2	2019-02-04 10:26:22.518385	2019-02-05 09:30:55.948654	f	t	18.00	18.00	0.00	0.00	f	f	\N
548	4		t	2	2019-02-04 20:24:46.199705	2019-02-05 09:30:56.08902	f	t	19.00	19.00	0.00	0.00	f	f	\N
571	6		t	2	2019-02-26 13:11:31.535511	2019-04-09 17:03:57.669497	f	t	6.00	6.00	0.00	0.00	f	f	\N
465	1		f	2	2019-01-11 16:51:26.001065	2019-01-14 12:09:31.26963	f	t	5.00	5.00	0.00	0.00	f	f	\N
556	3		t	2	2019-02-05 11:20:00.257537	2019-02-05 11:23:25.552585	f	t	6.00	6.00	0.00	0.00	f	f	\N
586	4		t	174	2019-04-09 19:08:55.010536	\N	f	f	\N	\N	0.00	0.00	f	f	\N
569	6		t	2	2019-02-11 19:43:45.654206	2019-02-12 12:19:52.746133	f	t	12.00	12.00	0.00	0.00	f	f	\N
483	5		f	2	2019-01-14 19:34:38.751808	2019-01-14 21:05:05.809487	f	t	15.00	15.00	0.00	0.00	f	f	\N
552	9		f	2	2019-02-05 10:12:22.238204	2019-02-05 10:23:53.50884	f	t	12.00	12.00	0.00	0.00	f	f	\N
545	1		t	2	2019-02-04 10:17:14.833285	2019-02-04 10:23:54.577451	f	t	12.00	12.00	0.00	0.00	f	f	\N
547	3		t	2	2019-02-04 19:10:18.60906	2019-02-04 20:45:13.989402	f	t	8.00	8.00	0.00	0.00	f	f	\N
466	3		f	2	2019-01-11 16:57:14.258919	2019-01-14 12:09:32.739693	f	t	5.00	5.00	0.00	0.00	f	f	\N
554	4		t	2	2019-02-05 10:38:53.467719	2019-02-05 10:39:55.625786	f	t	12.00	12.00	0.00	0.00	f	f	\N
493	2		f	2	2019-01-18 09:55:35.372652	2019-02-04 09:59:13.104279	f	t	24.00	24.00	0.00	0.00	f	f	\N
581	23		t	2	2019-03-18 16:27:51.529193	\N	f	f	14.00	14.00	0.00	0.00	f	f	\N
567	2		t	2	2019-02-09 16:04:15.740133	2019-04-09 16:59:06.79196	f	t	6.00	6.00	0.00	0.00	f	f	\N
489	2		f	2	2019-01-15 17:54:09.078193	2019-01-18 17:16:51.229184	f	t	21.00	21.00	0.00	0.00	f	f	\N
509	44		f	2	2019-01-27 14:45:23.838741	2019-02-04 09:59:13.842747	f	t	69.50	69.50	0.00	0.00	f	f	\N
476	3		f	2	2019-01-14 12:05:25.479547	2019-01-14 21:07:32.808099	f	t	5.00	5.00	0.00	0.00	f	f	\N
475	3		f	2	2019-01-14 12:04:28.525177	2019-01-14 21:07:33.924489	f	t	6.00	6.00	0.00	0.00	f	f	\N
473	5		f	2	2019-01-13 14:27:09.00573	2019-01-14 21:07:38.221408	f	t	19.00	19.00	0.00	0.00	f	f	\N
557	1		f	2	2019-02-05 11:24:37.73005	2019-02-05 11:26:50.386767	f	t	12.00	12.00	0.00	0.00	f	f	\N
500	1		f	2	2019-01-20 09:43:03.89879	2019-02-04 10:16:49.890729	f	t	0.00	0.00	0.00	0.00	f	f	\N
566	11		t	2	2019-02-08 19:25:54.960778	2019-02-09 15:46:43.517685	f	t	15.00	15.00	0.00	0.00	f	f	\N
541	1		f	2	2019-02-04 09:09:33.761055	2019-02-04 10:16:50.814468	f	t	0.00	0.00	0.00	0.00	f	f	\N
562	7		t	2	2019-02-05 16:02:38.324845	2019-02-09 15:46:44.448362	f	t	6.00	6.00	0.00	0.00	f	f	\N
572	8		t	171	2019-02-26 13:11:55.252463	2019-04-09 16:59:08.260674	f	t	6.00	6.00	0.00	0.00	f	f	\N
578	12		t	2	2019-03-04 22:09:09.915066	2019-03-06 18:02:10.707109	f	t	12.00	17.00	5.00	0.00	f	t	\N
570	1		t	2	2019-02-12 16:09:24.236498	\N	f	t	6.00	6.00	0.00	0.00	f	f	\N
585	6		t	2	2019-04-09 19:08:14.281565	2019-04-09 21:29:15.161908	f	t	7.50	7.50	0.00	0.00	f	f	\N
576	10		t	2	2019-02-27 13:21:18.188114	2019-04-09 16:59:09.428987	f	t	15.00	15.00	0.00	0.00	f	f	\N
577	11		t	2	2019-02-28 21:13:13.296032	2019-04-09 16:59:11.388075	f	t	6.00	6.00	0.00	0.00	f	f	\N
587	5		t	2	2019-04-09 21:40:04.630034	\N	f	f	0.00	0.00	0.00	0.00	f	f	\N
583	99		t	2	2019-04-04 19:45:11.707894	2019-04-09 17:01:35.902485	f	t	20.50	20.50	0.00	0.00	f	f	\N
574	9		t	173	2019-02-26 13:26:29.020262	\N	f	f	6.00	6.00	0.00	0.00	f	f	\N
\.


--
-- Name: orders_orderid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orders_orderid_seq', 587, true);


--
-- Name: paymentid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.paymentid_seq', 115, true);


--
-- Data for Name: paymentitem; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.paymentitem (paymentid, suborderid, orderid, tendercodesid, amount) FROM stdin;
79	306	\N	1	7.00
21	290	\N	1	2.00
22	290	\N	2	4.00
99	\N	577	1	6.00
100	307	\N	1	7.00
101	308	\N	1	1.00
102	308	\N	3	4.50
103	\N	579	1	9.00
107	\N	571	1	6.00
38	\N	569	1	12.00
41	\N	562	1	6.00
42	\N	566	1	15.00
43	\N	570	1	6.00
45	\N	568	1	8.00
46	\N	558	1	12.00
49	296	\N	1	10.00
114	\N	573	1	16.20
115	\N	585	1	7.50
73	\N	578	1	17.00
\.


--
-- Data for Name: printerforcategory; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.printerforcategory (printerforcategoryid, categoryid, printerid, stateid) FROM stdin;
55	69	41	2
\.


--
-- Name: printerforcategory_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.printerforcategory_id_seq', 55, true);


--
-- Data for Name: printerforreceiptandinvoice; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.printerforreceiptandinvoice (printerforcategoryid, printinvoice, printreceipt, printerid) FROM stdin;
4	t	t	41
\.


--
-- Name: printerforreceiptandinvoice_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.printerforreceiptandinvoice_id_seq', 4, true);


--
-- Data for Name: printers; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.printers (printerid, name) FROM stdin;
39	_192_168_1_20
40	_192_168_1_10
41	PDFwriter
\.


--
-- Name: printers_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.printers_id_seq', 41, true);


--
-- Data for Name: rejectedorderitems; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.rejectedorderitems (rejectedorderitemid, courseid, cause, timeofrejection, orderitemid) FROM stdin;
76	245	ee	2019-02-03 20:48:38.83072	1064
77	245	ddd	2019-02-03 20:50:02.369008	1064
78	249	mancano: hamburger	2019-02-05 16:02:44.836903	1324
79	249	mancano: hamburger	2019-02-07 15:13:30.190046	1324
\.


--
-- Name: rejectedorderitems_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.rejectedorderitems_id_seq', 81, true);


--
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.roles (roleid, rolename, comment) FROM stdin;
1	admin	\N
2	powerUser	\N
21	temporary	
23	ruolo1	
24	ruolo2	
25	cucina	
26	thai	
27	cameriere	
\.


--
-- Name: roles_roleid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.roles_roleid_seq', 27, true);


--
-- Data for Name: states; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.states (stateid, isinitial, isfinal, statusname, nextstateid, isexceptional, creatingingredientdecrement) FROM stdin;
1	t	f	COLLECTING	2	f	f
6	f	t	DONE	\N	f	f
2	f	f	TOBEWORKED	6	f	t
\.


--
-- Name: states_stateid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.states_stateid_seq', 6, true);


--
-- Data for Name: suborder; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.suborder (suborderid, orderid, subtotal, comment, payed, creationtime, tendercodesid, subtotaladjustment, subtotalpercentadjustment) FROM stdin;
290	567	6.00	\N	f	2019-02-09 16:20:56.8392	\N	0.00	0.00
262	550	6.00	\N	f	2019-02-05 10:03:42.961768	1	0.00	0.00
263	550	8.00	\N	f	2019-02-05 10:03:46.599052	1	0.00	0.00
264	552	6.00	\N	f	2019-02-05 10:12:45.622517	1	0.00	0.00
265	552	6.00	\N	f	2019-02-05 10:12:49.591393	1	0.00	0.00
217	489	10.00	\N	f	2019-01-16 13:57:33.188764	1	0.00	0.00
266	553	6.00	\N	f	2019-02-05 10:20:31.60409	1	0.00	0.00
267	553	7.50	\N	f	2019-02-05 10:20:38.341693	1	0.00	0.00
270	555	6.00	\N	f	2019-02-05 11:19:38.252935	1	0.00	0.00
229	493	12.00	\N	f	2019-01-18 15:53:00.21524	1	0.00	0.00
271	557	6.00	\N	f	2019-02-05 11:24:57.077221	1	0.00	0.00
272	557	6.00	\N	f	2019-02-05 11:24:59.616816	1	0.00	0.00
226	493	12.00	\N	t	2019-01-18 14:48:43.575022	1	0.00	0.00
255	548	11.00	\N	t	2019-02-04 20:25:24.77132	1	0.00	0.00
199	473	6.00	\N	f	2019-01-14 10:38:06.10439	1	0.00	0.00
201	473	13.00	\N	f	2019-01-14 10:38:18.935629	1	0.00	0.00
256	548	8.00	\N	t	2019-02-04 20:27:04.586728	1	0.00	0.00
257	546	8.00	\N	t	2019-02-04 20:53:20.128693	1	0.00	0.00
258	546	10.00	\N	t	2019-02-04 20:53:25.242289	1	0.00	0.00
277	559	6.00	\N	t	2019-02-05 12:50:47.725651	1	0.00	0.00
259	549	6.00	\N	f	2019-02-05 09:50:49.489416	1	0.00	0.00
260	549	6.50	\N	f	2019-02-05 09:50:55.987218	1	0.00	0.00
261	551	6.00	\N	f	2019-02-05 09:53:48.740019	1	0.00	0.00
239	545	6.00	\N	t	2019-02-04 10:23:18.429274	1	0.00	0.00
240	545	6.00	\N	t	2019-02-04 10:23:32.31877	1	0.00	0.00
295	576	15.00	\N	f	2019-03-04 20:47:21.971057	\N	0.00	0.00
296	572	6.00	\N	f	2019-03-04 21:13:18.070035	\N	0.00	0.00
306	558	6.00	\N	t	2019-03-06 17:16:54.874595	\N	1.00	0.00
307	580	7.00	\N	t	2019-03-18 12:50:33.875886	\N	0.00	0.00
308	580	6.50	\N	t	2019-03-18 12:52:48.54526	\N	-1.00	0.00
316	574	6.00	\N	f	2019-04-11 10:11:42.717353	\N	0.00	0.00
\.


--
-- Name: suborderid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.suborderid_seq', 316, true);


--
-- Data for Name: temp_user_actionable_states; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.temp_user_actionable_states (tempuseractionablestateid, userid, stateid) FROM stdin;
\.


--
-- Name: temp_user_actionable_states_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.temp_user_actionable_states_seq', 12, true);


--
-- Data for Name: temp_user_default_actionable_states; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.temp_user_default_actionable_states (tempmuseractionablestatesid, stateid) FROM stdin;
\.


--
-- Name: tempuseractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.tempuseractionablestates_seq', 1, false);


--
-- Data for Name: tendercodes; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.tendercodes (tendercodesid, tendercode, tendername) FROM stdin;
1	1	CONTANTI
2	2	CREDITO
3	3	ASSEGNI
5	4	BUONI
6	5	CARTA DI CREDITO
\.


--
-- Name: tendercodesid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.tendercodesid_seq', 6, true);


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.users (userid, username, password, enabled, canvoidorders, role, canmanageallorders, creationtime, istemporary, canchangetheprice, "table", consumed, canmanagecourses) FROM stdin;
2	tonyx	a66fb91959ab47eaf8d9ce0e2fd750ed36ffd5bf7cdc806c3fd3f6f80623bb9e	t	t	1	t	\N	f	t	\N	\N	f
165	utente	258c2a65992dae5f7175d6ad61cd242da2aa82339305a8b12eeb7d81a32ad2dd	t	f	23	f	\N	f	f	\N	\N	f
166	tonino	87cfad1b5f9782a36c7b8fd877c65a6f7d96e6499149323826ad4bc327bbf37c	t	f	23	t	\N	f	f	\N	\N	f
168	pasquale	9e3f4d2cb39b7bc7094e34c1e3cc105069971acddfafe317bab77651911048e7	t	f	23	t	\N	f	f	\N	\N	f
169	maurizio	e35057dc1a6d22c71a8ec32b0a3bee66a2cfe4af5a5ab2c2c65de7fa34038905	t	f	27	f	\N	f	f	\N	\N	f
173	giuseppe	42527a7be8579db58b4740453cc6a69f82384d27efa837a858e22ab6b6e46c74	t	f	2	t	\N	f	f	\N	\N	f
171	francesco	ecd8bb6ac56e748cd085b48836bd64b4ac0fa8f48bde4259f1814f1a1c89a2e2	t	f	2	t	\N	f	f	\N	\N	f
174	39UOABZQR0IV		t	f	21	f	2019-04-09 19:08:55.006654	t	f	4	\N	f
167	cucina	41613c0e1d7698d83847b8a78d03c6c4c617c631f75e8a4cff531ef11cf0deed	f	f	25	f	\N	f	f	\N	\N	f
\.


--
-- Name: users_userid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.users_userid_seq', 174, true);


--
-- Data for Name: variations; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.variations (variationsid, orderitemid, ingredientid, tipovariazione, plailnumvariation, ingredientpriceid) FROM stdin;
1067	1369	138	🚫	\N	\N
1071	1375	156	unità	3	\N
\.


--
-- Name: variations_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.variations_seq', 1079, true);


--
-- Name: voidedorderslog_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.voidedorderslog_id_seq', 307, true);


--
-- Data for Name: voidedorderslogbuffer; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.voidedorderslogbuffer (voidedorderslogbufferid, voidedtime, orderid, userid) FROM stdin;
\.


--
-- Data for Name: waiteractionablestates; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.waiteractionablestates (waiterwatchablestatesid, userid, stateid) FROM stdin;
289	165	1
290	2	2
291	166	1
293	168	1
294	169	1
296	171	1
299	173	1
300	167	1
113	2	1
\.


--
-- Name: waiteractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.waiteractionablestates_seq', 300, true);


--
-- Name: archivedorderslogbuffer archivedlogbufferid_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.archivedorderslogbuffer
    ADD CONSTRAINT archivedlogbufferid_key PRIMARY KEY (archivedlogbufferid);


--
-- Name: coursecategories category_uniquename; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.coursecategories
    ADD CONSTRAINT category_uniquename UNIQUE (name);


--
-- Name: coursecategories coursecategories_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.coursecategories
    ADD CONSTRAINT coursecategories_pkey PRIMARY KEY (categoryid);


--
-- Name: courses courses_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.courses
    ADD CONSTRAINT courses_pkey PRIMARY KEY (courseid);


--
-- Name: customerdata customer_name_unique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.customerdata
    ADD CONSTRAINT customer_name_unique UNIQUE (name);


--
-- Name: customerdata customerdata_name_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.customerdata
    ADD CONSTRAINT customerdata_name_key UNIQUE (name);


--
-- Name: customerdata customerdata_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.customerdata
    ADD CONSTRAINT customerdata_pkey PRIMARY KEY (customerdataid);


--
-- Name: defaultactionablestates defaultactionablestateidunique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.defaultactionablestates
    ADD CONSTRAINT defaultactionablestateidunique UNIQUE (stateid);


--
-- Name: defaultactionablestates defaultactionablestates_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.defaultactionablestates
    ADD CONSTRAINT defaultactionablestates_pkey PRIMARY KEY (defaultactionablestatesid);


--
-- Name: enablers enablers_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT enablers_pkey PRIMARY KEY (enablersid);


--
-- Name: enablers enablers_tripleidunique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT enablers_tripleidunique UNIQUE (stateid, roleid, categoryid);


--
-- Name: ingredientcategory ing_cat_unique_name; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcategory
    ADD CONSTRAINT ing_cat_unique_name UNIQUE (name);


--
-- Name: ingredient ing_unique_name; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredient
    ADD CONSTRAINT ing_unique_name UNIQUE (name);


--
-- Name: ingredientdecrement ingredient_decrement_unique_ordit_ingid; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT ingredient_decrement_unique_ordit_ingid UNIQUE (orderitemid, ingredientid);


--
-- Name: ingredient ingredient_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredient
    ADD CONSTRAINT ingredient_pkey PRIMARY KEY (ingredientid);


--
-- Name: ingredientcategory ingredientcateogory_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcategory
    ADD CONSTRAINT ingredientcateogory_pkey PRIMARY KEY (ingredientcategoryid);


--
-- Name: ingredientcourse ingredientcourse_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcourse
    ADD CONSTRAINT ingredientcourse_pkey PRIMARY KEY (ingredientcourseid);


--
-- Name: ingredientdecrement ingredientincrement_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT ingredientincrement_key PRIMARY KEY (ingredientdecrementid);


--
-- Name: ingredientincrement ingredientincrementid_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientincrement
    ADD CONSTRAINT ingredientincrementid_key PRIMARY KEY (ingredientincrementid);


--
-- Name: ingredientprice ingredientprice_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientprice
    ADD CONSTRAINT ingredientprice_pkey PRIMARY KEY (ingredientpriceid);


--
-- Name: ingredientprice ingredientprice_uniq; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientprice
    ADD CONSTRAINT ingredientprice_uniq UNIQUE (ingredientid, quantity);


--
-- Name: invoices invoices_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT invoices_pkey PRIMARY KEY (invoicesid);


--
-- Name: observers observers_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT observers_pkey PRIMARY KEY (observersid);


--
-- Name: variations orderitem_ingredient_uniq; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT orderitem_ingredient_uniq UNIQUE (orderitemid, ingredientid);


--
-- Name: orderitems orderitems_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT orderitems_pkey PRIMARY KEY (orderitemid);


--
-- Name: orderitemstates orderitemstates_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitemstates
    ADD CONSTRAINT orderitemstates_pkey PRIMARY KEY (orderitemstatesid);


--
-- Name: orderoutgroup orderoutgroup_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderoutgroup
    ADD CONSTRAINT orderoutgroup_key PRIMARY KEY (ordergroupid);


--
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (orderid);


--
-- Name: paymentitem payment_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.paymentitem
    ADD CONSTRAINT payment_key PRIMARY KEY (paymentid);


--
-- Name: printerforcategory print_cat_state_unique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT print_cat_state_unique UNIQUE (categoryid, printerid, stateid);


--
-- Name: printers printer_unique_name; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printers
    ADD CONSTRAINT printer_unique_name UNIQUE (name);


--
-- Name: printerforcategory printerforcategory_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT printerforcategory_key PRIMARY KEY (printerforcategoryid);


--
-- Name: printerforreceiptandinvoice printerforreceiptandinvoice_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforreceiptandinvoice
    ADD CONSTRAINT printerforreceiptandinvoice_pkey PRIMARY KEY (printerforcategoryid);


--
-- Name: printers printers_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printers
    ADD CONSTRAINT printers_key PRIMARY KEY (printerid);


--
-- Name: rejectedorderitems rejecteddorderitemhistoryid_pk; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.rejectedorderitems
    ADD CONSTRAINT rejecteddorderitemhistoryid_pk PRIMARY KEY (rejectedorderitemid);


--
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (roleid);


--
-- Name: roles roles_rolename_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_rolename_key UNIQUE (rolename);


--
-- Name: states state_uniquename; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.states
    ADD CONSTRAINT state_uniquename UNIQUE (statusname);


--
-- Name: states stateid_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.states
    ADD CONSTRAINT stateid_pkey PRIMARY KEY (stateid);


--
-- Name: states states_statusname_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.states
    ADD CONSTRAINT states_statusname_key UNIQUE (statusname);


--
-- Name: suborder suborderid_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.suborder
    ADD CONSTRAINT suborderid_key PRIMARY KEY (suborderid);


--
-- Name: temp_user_default_actionable_states temp_user_actionable_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_default_actionable_states
    ADD CONSTRAINT temp_user_actionable_pkey PRIMARY KEY (tempmuseractionablestatesid);


--
-- Name: temp_user_actionable_states temp_user_actionable_states_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_actionable_states
    ADD CONSTRAINT temp_user_actionable_states_pkey PRIMARY KEY (tempuseractionablestateid);


--
-- Name: temp_user_default_actionable_states temp_user_actionable_states_unique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_default_actionable_states
    ADD CONSTRAINT temp_user_actionable_states_unique UNIQUE (stateid);


--
-- Name: temp_user_actionable_states tempuserstateidunique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_actionable_states
    ADD CONSTRAINT tempuserstateidunique UNIQUE (stateid, userid);


--
-- Name: tendercodes tendercode_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.tendercodes
    ADD CONSTRAINT tendercode_key PRIMARY KEY (tendercodesid);


--
-- Name: tendercodes tendercodes_unique_code; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.tendercodes
    ADD CONSTRAINT tendercodes_unique_code UNIQUE (tendercode);


--
-- Name: tendercodes tendercodes_unique_name; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.tendercodes
    ADD CONSTRAINT tendercodes_unique_name UNIQUE (tendername);


--
-- Name: observers tripleidunique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT tripleidunique UNIQUE (stateid, roleid, categoryid);


--
-- Name: orderoutgroup unique_group_order; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderoutgroup
    ADD CONSTRAINT unique_group_order UNIQUE (orderid, groupidentifier);


--
-- Name: ingredientcourse unique_ing_courrse; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcourse
    ADD CONSTRAINT unique_ing_courrse UNIQUE (courseid, ingredientid);


--
-- Name: courses uniquename; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.courses
    ADD CONSTRAINT uniquename UNIQUE (name);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (userid);


--
-- Name: variations variations_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT variations_pkey PRIMARY KEY (variationsid);


--
-- Name: voidedorderslogbuffer voided_ord_uniq_user_order; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.voidedorderslogbuffer
    ADD CONSTRAINT voided_ord_uniq_user_order UNIQUE (userid, orderid);


--
-- Name: voidedorderslogbuffer voidedorderslogbufferid_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.voidedorderslogbuffer
    ADD CONSTRAINT voidedorderslogbufferid_key PRIMARY KEY (voidedorderslogbufferid);


--
-- Name: waiteractionablestates waiteractionablestates_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.waiteractionablestates
    ADD CONSTRAINT waiteractionablestates_pkey PRIMARY KEY (waiterwatchablestatesid);


--
-- Name: waiteractionablestates waiterdstateidunique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.waiteractionablestates
    ADD CONSTRAINT waiterdstateidunique UNIQUE (stateid, userid);


--
-- Name: observers category_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT category_fk FOREIGN KEY (categoryid) REFERENCES public.coursecategories(categoryid) MATCH FULL;


--
-- Name: enablers category_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT category_fk FOREIGN KEY (categoryid) REFERENCES public.coursecategories(categoryid) MATCH FULL;


--
-- Name: variations category_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT category_fk FOREIGN KEY (ingredientpriceid) REFERENCES public.ingredientprice(ingredientpriceid) MATCH FULL;


--
-- Name: printerforcategory category_printer_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT category_printer_fk FOREIGN KEY (printerid) REFERENCES public.printers(printerid) MATCH FULL;


--
-- Name: courses categoryfk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.courses
    ADD CONSTRAINT categoryfk FOREIGN KEY (categoryid) REFERENCES public.coursecategories(categoryid) MATCH FULL;


--
-- Name: orderitems coursefk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT coursefk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL;


--
-- Name: defaultactionablestates defaultactionablestates_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.defaultactionablestates
    ADD CONSTRAINT defaultactionablestates_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: enablers enablersrole; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT enablersrole FOREIGN KEY (roleid) REFERENCES public.roles(roleid) MATCH FULL;


--
-- Name: ingredientincrement ingredient_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientincrement
    ADD CONSTRAINT ingredient_fk FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL;


--
-- Name: ingredientdecrement ingredient_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT ingredient_fk FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL;


--
-- Name: ingredient ingredientcategory_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredient
    ADD CONSTRAINT ingredientcategory_fk FOREIGN KEY (ingredientcategoryid) REFERENCES public.ingredientcategory(ingredientcategoryid) MATCH FULL;


--
-- Name: ingredientcourse ingredientcourse_course_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcourse
    ADD CONSTRAINT ingredientcourse_course_fk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL;


--
-- Name: ingredientcourse ingredientcourse_ingredient_fk2; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcourse
    ADD CONSTRAINT ingredientcourse_ingredient_fk2 FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL;


--
-- Name: ingredientprice ingredientprice_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientprice
    ADD CONSTRAINT ingredientprice_fk FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL;


--
-- Name: invoices invoice_customer_data_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT invoice_customer_data_fk FOREIGN KEY (customerdataid) REFERENCES public.customerdata(customerdataid) MATCH FULL;


--
-- Name: printerforreceiptandinvoice invoice_receipt_printer_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforreceiptandinvoice
    ADD CONSTRAINT invoice_receipt_printer_fk FOREIGN KEY (printerid) REFERENCES public.printers(printerid) MATCH FULL;


--
-- Name: paymentitem oorder_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.paymentitem
    ADD CONSTRAINT oorder_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


--
-- Name: archivedorderslogbuffer order_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.archivedorderslogbuffer
    ADD CONSTRAINT order_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


--
-- Name: invoices order_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT order_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


--
-- Name: orderitems orderdetail_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT orderdetail_fk FOREIGN KEY (ordergroupid) REFERENCES public.orderoutgroup(ordergroupid) MATCH FULL;


--
-- Name: orderitems orderfk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT orderfk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


--
-- Name: orderitemstates orderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitemstates
    ADD CONSTRAINT orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL;


--
-- Name: ingredientdecrement orderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL;


--
-- Name: orderitems orderitemstatus; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT orderitemstatus FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: orderoutgroup orderoutgroup_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderoutgroup
    ADD CONSTRAINT orderoutgroup_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


--
-- Name: ingredientdecrement preparator_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT preparator_fk FOREIGN KEY (preparatorid) REFERENCES public.users(userid) MATCH FULL;


--
-- Name: printerforcategory print_category_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT print_category_fk FOREIGN KEY (categoryid) REFERENCES public.coursecategories(categoryid) MATCH FULL;


--
-- Name: printerforcategory printer_cat_state_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT printer_cat_state_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: rejectedorderitems reject_orderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.rejectedorderitems
    ADD CONSTRAINT reject_orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL;


--
-- Name: rejectedorderitems rejected_item_course_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.rejectedorderitems
    ADD CONSTRAINT rejected_item_course_fk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL;


--
-- Name: observers role1_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT role1_fk FOREIGN KEY (roleid) REFERENCES public.roles(roleid) MATCH FULL;


--
-- Name: users rolefk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT rolefk FOREIGN KEY (role) REFERENCES public.roles(roleid) MATCH FULL;


--
-- Name: observers state1_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT state1_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: orderitemstates state2_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitemstates
    ADD CONSTRAINT state2_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: enablers state_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT state_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: invoices suborder_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT suborder_fk FOREIGN KEY (suborderid) REFERENCES public.suborder(suborderid) MATCH FULL;


--
-- Name: paymentitem suborder_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.paymentitem
    ADD CONSTRAINT suborder_fk FOREIGN KEY (suborderid) REFERENCES public.suborder(suborderid) MATCH FULL;


--
-- Name: suborder suborderorder_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.suborder
    ADD CONSTRAINT suborderorder_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


--
-- Name: temp_user_default_actionable_states temp_user_actionable_states_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_default_actionable_states
    ADD CONSTRAINT temp_user_actionable_states_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: temp_user_actionable_states tempuser_state_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_actionable_states
    ADD CONSTRAINT tempuser_state_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: temp_user_actionable_states tempuser_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_actionable_states
    ADD CONSTRAINT tempuser_user_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL;


--
-- Name: paymentitem tendercode_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.paymentitem
    ADD CONSTRAINT tendercode_fk FOREIGN KEY (tendercodesid) REFERENCES public.tendercodes(tendercodesid) MATCH FULL;


--
-- Name: ingredientincrement user_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientincrement
    ADD CONSTRAINT user_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL;


--
-- Name: orders userfk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT userfk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL;


--
-- Name: variations variationingredient_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT variationingredient_fk FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL;


--
-- Name: variations variationorderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT variationorderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL;


--
-- Name: voidedorderslogbuffer voided_ord_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.voidedorderslogbuffer
    ADD CONSTRAINT voided_ord_user_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL;


--
-- Name: voidedorderslogbuffer voided_order_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.voidedorderslogbuffer
    ADD CONSTRAINT voided_order_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


--
-- Name: waiteractionablestates waiterstate_state_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.waiteractionablestates
    ADD CONSTRAINT waiterstate_state_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: waiteractionablestates waiterstateuser_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.waiteractionablestates
    ADD CONSTRAINT waiterstateuser_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL;


--
-- Name: TABLE archivedorderslogbuffer; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.archivedorderslogbuffer TO suave;


--
-- Name: SEQUENCE archivedorderslog_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.archivedorderslog_id_seq TO suave;


--
-- Name: TABLE coursecategories; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.coursecategories TO suave;


--
-- Name: TABLE courses; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.courses TO suave;


--
-- Name: TABLE coursedetails2; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.coursedetails2 TO suave;


--
-- Name: SEQUENCE courses_categoryid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.courses_categoryid_seq TO suave;


--
-- Name: SEQUENCE courses_courseid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.courses_courseid_seq TO suave;


--
-- Name: SEQUENCE customerdata_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.customerdata_id_seq TO suave;


--
-- Name: TABLE customerdata; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.customerdata TO suave;


--
-- Name: SEQUENCE defaulwaiteractionablestates_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.defaulwaiteractionablestates_seq TO suave;


--
-- Name: TABLE defaultactionablestates; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.defaultactionablestates TO suave;


--
-- Name: SEQUENCE enablers_elablersid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.enablers_elablersid_seq TO suave;


--
-- Name: TABLE enablers; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.enablers TO suave;


--
-- Name: TABLE roles; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.roles TO suave;


--
-- Name: SEQUENCE states_stateid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.states_stateid_seq TO suave;


--
-- Name: TABLE states; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.states TO suave;


--
-- Name: TABLE enablersrolestatuscategories; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.enablersrolestatuscategories TO suave;


--
-- Name: SEQUENCE incredientdecrementid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.incredientdecrementid_seq TO suave;


--
-- Name: TABLE ingredient; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredient TO suave;


--
-- Name: TABLE ingredientcategory; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientcategory TO suave;


--
-- Name: SEQUENCE ingredient_categoryid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.ingredient_categoryid_seq TO suave;


--
-- Name: TABLE ingredientcourse; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientcourse TO suave;


--
-- Name: SEQUENCE ingredientcourseid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.ingredientcourseid_seq TO suave;


--
-- Name: TABLE ingredientdecrement; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientdecrement TO suave;


--
-- Name: TABLE orderitems; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orderitems TO suave;


--
-- Name: TABLE ingredientdecrementview; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientdecrementview TO suave;


--
-- Name: TABLE ingredientdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientdetails TO suave;


--
-- Name: SEQUENCE ingredientid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.ingredientid_seq TO suave;


--
-- Name: SEQUENCE ingredientincrementid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.ingredientincrementid_seq TO suave;


--
-- Name: TABLE ingredientincrement; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientincrement TO suave;


--
-- Name: TABLE ingredientofcourses; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientofcourses TO suave;


--
-- Name: SEQUENCE ingredientpriceid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.ingredientpriceid_seq TO suave;


--
-- Name: TABLE ingredientprice; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientprice TO suave;


--
-- Name: TABLE ingredientpricedetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientpricedetails TO suave;


--
-- Name: SEQUENCE invoicesid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.invoicesid_seq TO suave;


--
-- Name: TABLE invoices; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.invoices TO suave;


--
-- Name: TABLE orders; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orders TO suave;


--
-- Name: TABLE users; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.users TO suave;


--
-- Name: TABLE nonarchivedorderdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.nonarchivedorderdetails TO suave;


--
-- Name: TABLE nonemptyorderdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.nonemptyorderdetails TO suave;


--
-- Name: SEQUENCE observers_observerid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.observers_observerid_seq TO suave;


--
-- Name: TABLE observers; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.observers TO suave;


--
-- Name: SEQUENCE observers_observersid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.observers_observersid_seq TO suave;


--
-- Name: TABLE observersrolestatuscategories; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.observersrolestatuscategories TO suave;


--
-- Name: TABLE orderdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orderdetails TO suave;


--
-- Name: SEQUENCE orderoutgroup_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.orderoutgroup_id_seq TO suave;


--
-- Name: TABLE orderoutgroup; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orderoutgroup TO suave;


--
-- Name: SEQUENCE suborderid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.suborderid_seq TO suave;


--
-- Name: TABLE suborder; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.suborder TO suave;


--
-- Name: TABLE orderitemdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orderitemdetails TO suave;


--
-- Name: SEQUENCE orderitems_orderitemid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.orderitems_orderitemid_seq TO suave;


--
-- Name: TABLE orderitemstates; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orderitemstates TO suave;


--
-- Name: SEQUENCE orderitemstates_orderitemstates_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.orderitemstates_orderitemstates_id_seq TO suave;


--
-- Name: TABLE orderoutgroupdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orderoutgroupdetails TO suave;


--
-- Name: SEQUENCE orders_orderid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.orders_orderid_seq TO suave;


--
-- Name: SEQUENCE paymentid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.paymentid_seq TO suave;


--
-- Name: TABLE paymentitem; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.paymentitem TO suave;


--
-- Name: SEQUENCE tendercodesid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.tendercodesid_seq TO suave;


--
-- Name: TABLE tendercodes; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.tendercodes TO suave;


--
-- Name: TABLE paymentitemdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.paymentitemdetails TO suave;


--
-- Name: SEQUENCE printerforcategory_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.printerforcategory_id_seq TO suave;


--
-- Name: TABLE printerforcategory; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.printerforcategory TO suave;


--
-- Name: SEQUENCE printers_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.printers_id_seq TO suave;


--
-- Name: TABLE printers; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.printers TO suave;


--
-- Name: TABLE printerforcategorydetail; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.printerforcategorydetail TO suave;


--
-- Name: SEQUENCE printerforreceiptandinvoice_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.printerforreceiptandinvoice_id_seq TO suave;


--
-- Name: TABLE printerforreceiptandinvoice; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.printerforreceiptandinvoice TO suave;


--
-- Name: SEQUENCE rejectedorderitems_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.rejectedorderitems_id_seq TO suave;


--
-- Name: TABLE rejectedorderitems; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.rejectedorderitems TO suave;


--
-- Name: SEQUENCE roles_roleid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.roles_roleid_seq TO suave;


--
-- Name: SEQUENCE tempuseractionablestates_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.tempuseractionablestates_seq TO suave;


--
-- Name: TABLE temp_user_actionable_states; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.temp_user_actionable_states TO suave;


--
-- Name: SEQUENCE temp_user_actionable_states_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.temp_user_actionable_states_seq TO suave;


--
-- Name: TABLE temp_user_default_actionable_states; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.temp_user_default_actionable_states TO suave;


--
-- Name: SEQUENCE users_userid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.users_userid_seq TO suave;


--
-- Name: TABLE usersview; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.usersview TO suave;


--
-- Name: TABLE variations; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.variations TO suave;


--
-- Name: TABLE variationdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.variationdetails TO suave;


--
-- Name: SEQUENCE variations_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.variations_seq TO suave;


--
-- Name: TABLE voidedorderslogbuffer; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.voidedorderslogbuffer TO suave;


--
-- Name: SEQUENCE voidedorderslog_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.voidedorderslog_id_seq TO suave;


--
-- Name: SEQUENCE waiteractionablestates_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.waiteractionablestates_seq TO suave;


--
-- Name: TABLE waiteractionablestates; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.waiteractionablestates TO suave;


--
-- PostgreSQL database dump complete
--

