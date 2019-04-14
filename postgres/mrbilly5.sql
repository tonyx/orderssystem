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
    visibile boolean DEFAULT true
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
    ordergroupid integer,
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
    subtotal numeric NOT NULL,
    comment character varying(30),
    payed boolean NOT NULL,
    creationtime timestamp without time zone NOT NULL
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
    f.payed
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
    a.canmanagecourses
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

SELECT pg_catalog.setval('public.archivedorderslog_id_seq', 81, true);


--
-- Data for Name: archivedorderslogbuffer; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.archivedorderslogbuffer (archivedlogbufferid, archivedtime, orderid) FROM stdin;
\.


--
-- Data for Name: coursecategories; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.coursecategories (categoryid, name, visibile) FROM stdin;
43	panini	t
47	contorni e insalate	t
48	schiacciate & toast	t
49	kebab	t
50	crepes salate	t
51	piadine	t
52	dessert	t
53	bar	t
54	birre alla spina	t
55	birre in bottiglia	t
42	secondi	t
46	frittate	t
45	antipasti & sfizi	t
41	primi	t
56	Thai	t
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
196	bucatini all'amatriciana		8.00	41	t
197	gnocchetti di radicchio e gorgonzola		10.00	41	t
198	Scialatielli zucchine e provola		10.00	41	t
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
\.


--
-- Name: courses_categoryid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.courses_categoryid_seq', 56, true);


--
-- Name: courses_courseid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.courses_courseid_seq', 400, true);


--
-- Data for Name: defaultactionablestates; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.defaultactionablestates (defaultactionablestatesid, stateid) FROM stdin;
28	1
\.


--
-- Name: defaulwaiteractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.defaulwaiteractionablestates_seq', 28, true);


--
-- Data for Name: enablers; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.enablers (enablersid, roleid, stateid, categoryid) FROM stdin;
157	25	1	43
158	25	2	43
160	25	1	49
161	25	2	49
\.


--
-- Name: enablers_elablersid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.enablers_elablersid_seq', 162, true);


--
-- Name: incredientdecrementid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.incredientdecrementid_seq', 114, true);


--
-- Data for Name: ingredient; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredient (ingredientid, ingredientcategoryid, name, description, visibility, allergen, updateavailabilityflag, availablequantity, checkavailabilityflag, unitmeasure) FROM stdin;
143	48	roast beef		t	f	f	0.00	f	gr
151	52	salsa rosa		t	f	f	0.00	f	gr
164	52	salsa tonnata		t	f	f	0.00	f	gr
173	52	salsa piccante		t	f	f	0.00	f	gr
159	47	pan brioche		t	f	f	0.00	f	gr
134	50	bacon		t	f	f	0.00	f	gr
153	48	carne di manzo		t	f	f	0.00	f	gr
152	50	bresaola		t	f	f	0.00	f	gr
141	52	crema di funghi porcini		t	f	f	0.00	f	gr
168	47	barros		t	f	f	0.00	f	unità
148	48	cotoletta		t	f	f	0.00	f	gr
136	52	barbecue		t	f	f	0.00	f	gr
128	49	gorgonzola		t	f	f	0.00	f	gr
115	48	hamburger		t	f	f	0.00	f	unità
131	49	ceddar		t	f	f	0.00	f	gr
140	51	friarielli		t	f	f	0.00	f	gr
170	48	hamburger di pollo		t	f	f	0.00	f	unità
171	48	hamburger di angus		t	f	f	0.00	f	unità
172	48	hamburger di chianina		t	f	f	0.00	f	unità
116	51	insalata		t	f	f	0.00	f	gr
137	48	hot dog		t	f	f	0.00	f	gr
125	51	funghi		t	f	f	0.00	f	gr
122	51	melanzane a funghetti		t	f	f	0.00	f	gr
138	49	mozzarella		t	f	f	0.00	f	gr
135	52	mayonese		t	f	f	0.00	f	gr
150	48	cordon blue		t	f	f	0.00	f	gr
118	51	patate		t	f	f	0.00	f	gr
133	51	insalata coleslaw		t	f	f	0.00	f	gr
146	50	pancetta		t	f	f	0.00	f	gr
154	51	peperoni		t	f	f	0.00	f	gr
142	51	patate al rosmarino		t	f	f	0.00	f	gr
149	51	parmigiana di melanzan		t	f	f	0.00	f	gr
144	48	petto di pollo		t	f	f	0.00	f	gr
117	51	pomodoro		t	f	f	0.00	f	gr
155	48	polpettine di scottona		t	f	f	0.00	f	gr
120	50	prosciutto cotto		t	f	f	0.00	f	gr
121	49	provola		t	f	f	0.00	f	gr
124	50	prosciutto crudo		t	f	f	0.00	f	gr
126	51	rucola		t	f	f	0.00	f	gr
132	51	cipolla sfumata		t	f	f	0.00	f	gr
127	52	salse		t	f	f	0.00	f	gr
139	48	salsiccia		t	f	f	0.00	f	gr
167	51	cipolla		t	f	f	0.00	f	gr
145	49	scaglie di parmigiano		t	f	f	0.00	f	gr
123	50	speck		t	f	f	0.00	f	gr
161	47	saltimbocca		t	f	f	0.00	f	unità
147	51	melanzane alla griglia		t	f	f	0.00	f	gr
160	47	sfilatino		t	f	f	0.00	f	gr
119	49	sottiletta		t	f	f	0.00	f	unità
130	48	wurstel		t	f	f	0.00	f	gr
165	48	wurstel a pezzi		t	f	f	0.00	f	gr
158	52	salsa barbecue		t	f	f	0.00	f	gr
157	51	patate al ceddar		t	f	f	0.00	f	gr
156	51	anelli di cipolla		t	f	f	0.00	f	gr
166	51	cipolla cruda		t	f	f	0.00	f	gr
169	48	frittata di cipolle		t	f	f	0.00	f	unità
129	48	uova		t	f	f	0.00	f	gr
\.


--
-- Name: ingredient_categoryid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredient_categoryid_seq', 52, true);


--
-- Data for Name: ingredientcategory; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientcategory (ingredientcategoryid, name, description, visibility) FROM stdin;
47	pane		t
48	carne		t
49	formaggi		t
50	salumi		t
51	verdure		t
52	salse		t
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
213	249	115	1.00
232	259	115	1.00
238	259	169	1.00
270	273	119	1.00
300	281	119	1.00
\.


--
-- Name: ingredientcourseid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientcourseid_seq', 349, true);


--
-- Data for Name: ingredientdecrement; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientdecrement (ingredientdecrementid, orderitemid, typeofdecrement, presumednormalquantity, recordedquantity, preparatorid, registrationtime, ingredientid) FROM stdin;
\.


--
-- Name: ingredientid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientid_seq', 173, true);


--
-- Data for Name: ingredientincrement; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientincrement (ingredientincrementid, ingredientid, comment, unitofmeasure, quantity, userid, registrationtime) FROM stdin;
\.


--
-- Name: ingredientincrementid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientincrementid_seq', 6, true);


--
-- Data for Name: ingredientprice; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientprice (ingredientpriceid, ingredientid, quantity, isdefaultadd, isdefaultsubtract, addprice, subtractprice) FROM stdin;
54	115	2.00	f	f	5.00	5.00
43	172	1.00	f	f	6.00	6.00
48	120	30.00	f	f	3.00	3.00
17	131	10.00	f	f	1.00	1.00
13	145	10.00	f	f	1.00	1.00
35	144	1.00	f	f	3.00	3.00
26	122	10.00	f	f	1.50	1.50
10	120	10.00	f	f	1.00	1.00
32	129	1.00	f	f	2.00	2.00
28	156	10.00	f	f	1.50	1.50
19	128	10.00	f	f	1.00	1.00
29	167	10.00	f	f	1.00	1.00
20	125	10.00	f	f	1.00	1.00
36	139	1.00	f	f	3.00	3.00
15	147	10.00	f	f	1.00	1.00
27	142	10.00	f	f	1.50	1.50
38	143	1.00	f	f	3.00	3.00
44	159	1.00	f	f	1.00	1.00
42	171	1.00	f	f	6.00	6.00
47	120	20.00	f	f	2.00	2.00
11	124	10.00	f	f	1.00	1.00
46	124	20.00	f	f	2.00	2.00
30	168	1.00	f	f	2.00	2.00
9	117	10.00	f	f	0.50	0.50
16	138	10.00	f	f	1.00	1.00
41	170	1.00	f	f	6.00	6.00
40	152	10.00	f	f	4.00	4.00
24	165	10.00	f	f	1.00	1.00
39	150	1.00	f	f	3.00	3.00
14	146	10.00	f	f	1.00	1.00
23	166	10.00	f	f	1.00	1.00
45	119	1.00	f	f	0.50	0.50
12	123	10.00	f	f	1.00	1.00
37	148	1.00	f	f	3.00	3.00
8	116	10.00	f	f	0.50	0.50
31	161	1.00	f	f	2.00	2.00
22	164	10.00	f	f	1.00	1.00
25	134	10.00	f	f	1.50	1.50
33	169	1.00	f	f	2.00	2.00
53	115	1.00	t	t	3.00	3.00
\.


--
-- Name: ingredientpriceid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientpriceid_seq', 54, true);


--
-- Data for Name: observers; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.observers (observersid, stateid, roleid, categoryid) FROM stdin;
177	1	25	43
178	2	25	43
180	1	25	49
181	2	25	49
\.


--
-- Name: observers_observerid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.observers_observerid_seq', 182, true);


--
-- Name: observers_observersid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.observers_observersid_seq', 1, false);


--
-- Data for Name: orderitems; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orderitems (orderitemid, courseid, quantity, orderid, comment, price, stateid, archived, startingtime, closingtime, ordergroupid, hasbeenrejected, suborderid, isinsasuborder, printcount) FROM stdin;
1024	245	1	447	bla	7.00	6	\N	2018-09-28 11:30:33.416479	2018-09-28 17:43:53.020992	240	f	0	f	0
1023	245	1	447		5.00	6	\N	2018-09-28 11:20:58.448784	2018-09-28 16:04:53.23715	239	f	188	t	0
1025	308	1	447		8.50	2	\N	2018-09-28 12:08:16.585374	\N	241	f	188	t	0
1026	196	1	447		8.00	2	\N	2018-09-28 12:53:47.52209	\N	242	f	188	t	0
1028	199	1	448		13.00	6	\N	2018-09-28 15:20:57.790726	2018-09-29 20:06:35.06425	244	f	189	t	0
1029	298	1	448		6.00	6	\N	2018-09-28 16:03:52.242929	2018-09-28 17:43:56.267026	245	f	189	t	0
1030	245	1	448		8.00	6	\N	2018-09-28 16:21:46.202533	2018-09-28 17:43:56.860462	246	f	189	t	0
1031	245	1	447		5.00	6	\N	2018-09-28 17:07:55.067815	2018-09-28 17:43:57.441028	247	f	\N	f	0
1032	245	1	448		5.00	6	\N	2018-09-28 17:36:35.978179	2018-09-28 17:43:58.617053	246	f	\N	f	0
1027	245	1	447		5.00	6	\N	2018-09-28 12:54:01.22954	2018-09-28 17:43:55.426116	243	f	0	f	0
\.


--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orderitems_orderitemid_seq', 1032, true);


--
-- Data for Name: orderitemstates; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orderitemstates (orderitemstatesid, orderitemid, stateid, startingtime) FROM stdin;
1954	1023	1	2018-09-28 11:20:58.448784
1955	1023	2	2018-09-28 11:27:02.26392
1956	1024	1	2018-09-28 11:30:33.416479
1957	1024	2	2018-09-28 12:06:02.408897
1958	1025	1	2018-09-28 12:08:16.585374
1959	1025	2	2018-09-28 12:39:27.362834
1960	1026	1	2018-09-28 12:53:47.52209
1961	1026	2	2018-09-28 12:53:50.831478
1962	1027	1	2018-09-28 12:54:01.22954
1963	1027	2	2018-09-28 12:54:03.124836
1964	1028	1	2018-09-28 15:20:57.790726
1965	1028	2	2018-09-28 15:21:40.337457
1966	1029	1	2018-09-28 16:03:52.242929
1967	1029	2	2018-09-28 16:04:23.649835
1968	1023	6	2018-09-28 16:04:53.23715
1969	1030	1	2018-09-28 16:21:46.202533
1970	1030	2	2018-09-28 16:22:04.595267
1971	1031	1	2018-09-28 17:07:55.067815
1972	1031	2	2018-09-28 17:08:49.55041
1973	1032	1	2018-09-28 17:36:35.978179
1974	1032	2	2018-09-28 17:36:58.844982
1975	1024	6	2018-09-28 17:43:53.020992
1976	1027	6	2018-09-28 17:43:55.426116
1977	1029	6	2018-09-28 17:43:56.267026
1978	1030	6	2018-09-28 17:43:56.860462
1979	1031	6	2018-09-28 17:43:57.441028
1980	1032	6	2018-09-28 17:43:58.617053
1981	1028	6	2018-09-29 20:06:35.06425
\.


--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orderitemstates_orderitemstates_id_seq', 1981, true);


--
-- Data for Name: orderoutgroup; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orderoutgroup (ordergroupid, printcount, orderid, groupidentifier) FROM stdin;
239	1	447	1
240	1	447	2
241	1	447	3
242	1	447	4
243	1	447	5
244	1	448	1
245	1	448	2
247	1	447	6
246	1	448	3
\.


--
-- Name: orderoutgroup_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orderoutgroup_id_seq', 247, true);


--
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orders (orderid, "table", person, ongoing, userid, startingtime, closingtime, voided, archived, total, adjustedtotal, plaintotalvariation, percentagevariataion, adjustispercentage, adjustisplain, forqruserarchived) FROM stdin;
447	1		t	2	2018-09-28 11:20:49.881203	\N	f	f	38.50	34.65	0.00	-10.00	t	f	\N
448	3		f	2	2018-09-28 15:20:41.149321	\N	f	f	32.00	32.00	0.00	0.00	f	f	\N
\.


--
-- Name: orders_orderid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orders_orderid_seq', 448, true);


--
-- Data for Name: printerforcategory; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.printerforcategory (printerforcategoryid, categoryid, printerid, stateid) FROM stdin;
38	43	28	2
39	46	25	2
\.


--
-- Name: printerforcategory_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.printerforcategory_id_seq', 39, true);


--
-- Data for Name: printerforreceiptandinvoice; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.printerforreceiptandinvoice (printerforcategoryid, printinvoice, printreceipt, printerid) FROM stdin;
1	t	t	25
\.


--
-- Name: printerforreceiptandinvoice_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.printerforreceiptandinvoice_id_seq', 1, true);


--
-- Data for Name: printers; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.printers (printerid, name) FROM stdin;
25	PDFwriter
28	PDFwriter_2
29	PDFwriter_3
\.


--
-- Name: printers_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.printers_id_seq', 29, true);


--
-- Data for Name: rejectedorderitems; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.rejectedorderitems (rejectedorderitemid, courseid, cause, timeofrejection, orderitemid) FROM stdin;
\.


--
-- Name: rejectedorderitems_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.rejectedorderitems_id_seq', 43, true);


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
\.


--
-- Name: roles_roleid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.roles_roleid_seq', 25, true);


--
-- Data for Name: states; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.states (stateid, isinitial, isfinal, statusname, nextstateid, isexceptional, creatingingredientdecrement) FROM stdin;
1	t	f	COLLECTING	2	f	f
6	f	t	DONE	\N	f	f
2	f	f	TOBEWORKED	6	f	f
\.


--
-- Name: states_stateid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.states_stateid_seq', 6, true);


--
-- Data for Name: suborder; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.suborder (suborderid, orderid, subtotal, comment, payed, creationtime) FROM stdin;
188	447	21.50	\N	f	2018-09-29 20:05:57.455085
189	448	27.00	\N	f	2018-09-29 20:07:09.117411
\.


--
-- Name: suborderid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.suborderid_seq', 189, true);


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
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.users (userid, username, password, enabled, canvoidorders, role, canmanageallorders, creationtime, istemporary, canchangetheprice, "table", consumed, canmanagecourses) FROM stdin;
2	tonyx	a66fb91959ab47eaf8d9ce0e2fd750ed36ffd5bf7cdc806c3fd3f6f80623bb9e	t	t	1	t	\N	f	t	\N	\N	f
165	utente	258c2a65992dae5f7175d6ad61cd242da2aa82339305a8b12eeb7d81a32ad2dd	t	f	23	f	\N	f	f	\N	\N	f
166	tonino	87cfad1b5f9782a36c7b8fd877c65a6f7d96e6499149323826ad4bc327bbf37c	t	f	23	t	\N	f	f	\N	\N	f
167	cucina	41613c0e1d7698d83847b8a78d03c6c4c617c631f75e8a4cff531ef11cf0deed	t	f	25	f	\N	f	f	\N	\N	f
\.


--
-- Name: users_userid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.users_userid_seq', 167, true);


--
-- Data for Name: variations; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.variations (variationsid, orderitemid, ingredientid, tipovariazione, plailnumvariation, ingredientpriceid) FROM stdin;
910	1023	168	unità	1	\N
916	1024	146	PER_PREZZO_INGREDIENTE	\N	14
917	1024	164	PER_PREZZO_INGREDIENTE	\N	22
919	1025	143	PER_PREZZO_INGREDIENTE	\N	38
920	1028	143	PER_PREZZO_INGREDIENTE	\N	38
922	1029	143	✋	\N	\N
923	1030	115	unità	1	\N
927	1031	143	PER_PREZZO_INGREDIENTE	\N	38
929	1031	115	unità	-1	\N
931	1032	143	PER_PREZZO_INGREDIENTE	\N	38
932	1032	115	unità	-1	\N
\.


--
-- Name: variations_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.variations_seq', 932, true);


--
-- Name: voidedorderslog_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.voidedorderslog_id_seq', 216, true);


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
292	167	1
113	2	1
\.


--
-- Name: waiteractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.waiteractionablestates_seq', 292, true);


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
-- Name: printerforreceiptandinvoice invoice_receipt_printer_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforreceiptandinvoice
    ADD CONSTRAINT invoice_receipt_printer_fk FOREIGN KEY (printerid) REFERENCES public.printers(printerid) MATCH FULL;


--
-- Name: archivedorderslogbuffer order_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.archivedorderslogbuffer
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
-- Name: TABLE orders; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orders TO suave;


--
-- Name: TABLE users; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.users TO suave;


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

