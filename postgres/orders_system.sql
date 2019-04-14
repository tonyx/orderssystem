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



--
-- Name: courses_categoryid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.courses_categoryid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



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



--
-- Name: defaultactionablestates; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.defaultactionablestates (
    defaultactionablestatesid integer DEFAULT nextval('public.defaulwaiteractionablestates_seq'::regclass) NOT NULL,
    stateid integer NOT NULL
);



--
-- Name: enablers_elablersid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.enablers_elablersid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: enablers; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.enablers (
    enablersid integer DEFAULT nextval('public.enablers_elablersid_seq'::regclass) NOT NULL,
    roleid integer NOT NULL,
    stateid integer NOT NULL,
    categoryid integer NOT NULL
);



--
-- Name: roles; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.roles (
    roleid integer NOT NULL,
    rolename character varying(30) NOT NULL,
    comment character varying(50)
);



--
-- Name: states_stateid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.states_stateid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



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



--
-- Name: incredientdecrementid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.incredientdecrementid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



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



--
-- Name: ingredientcategory; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.ingredientcategory (
    ingredientcategoryid integer NOT NULL,
    name character varying(120) NOT NULL,
    description character varying(4000),
    visibility boolean NOT NULL
);



--
-- Name: ingredient_categoryid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredient_categoryid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



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



--
-- Name: ingredientcourseid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredientcourseid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



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



--
-- Name: ingredientid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredientid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



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



--
-- Name: ingredientpriceid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredientpriceid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



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



--
-- Name: observers_observerid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.observers_observerid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: observers; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.observers (
    observersid integer DEFAULT nextval('public.observers_observerid_seq'::regclass) NOT NULL,
    stateid integer NOT NULL,
    roleid integer NOT NULL,
    categoryid integer NOT NULL
);



--
-- Name: observers_observersid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.observers_observersid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



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



--
-- Name: orderoutgroup_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.orderoutgroup_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: orderoutgroup; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.orderoutgroup (
    ordergroupid integer DEFAULT nextval('public.orderoutgroup_id_seq'::regclass) NOT NULL,
    printcount integer NOT NULL,
    orderid integer NOT NULL,
    groupidentifier integer NOT NULL
);



--
-- Name: suborderid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.suborderid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



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



--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.orderitems_orderitemid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



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



--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.orderitemstates_orderitemstates_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



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



--
-- Name: orders_orderid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.orders_orderid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



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

SELECT pg_catalog.setval('public.archivedorderslog_id_seq', 98, true);


--
-- Data for Name: archivedorderslogbuffer; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.archivedorderslogbuffer (archivedlogbufferid, archivedtime, orderid) FROM stdin;
82	2018-10-02 13:44:40.296432	450
83	2018-10-02 13:44:41.577363	448
86	2018-10-02 13:52:31.435716	447
89	2018-10-03 10:59:07.878491	449
90	2018-10-03 10:59:09.759008	453
91	2018-10-03 10:59:11.406777	454
92	2018-10-03 18:27:13.423577	455
93	2018-10-03 18:39:07.077773	456
94	2018-10-03 20:06:06.214199	457
95	2018-10-03 20:58:01.481195	458
96	2018-10-03 20:58:03.51703	459
97	2018-10-03 22:57:11.665505	460
98	2018-10-07 14:37:42.719994	462
\.


--
-- Data for Name: coursecategories; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.coursecategories (categoryid, name, visibile) FROM stdin;
57	panini	t
58	drink	t
59	primi piatti	t
60	dessert	t
61	secondi	t
\.


--
-- Data for Name: courses; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.courses (courseid, name, description, price, categoryid, visibility) FROM stdin;
401	hamburger completo		5.00	57	t
402	coca cola		3.00	58	t
403	maccheroni con pomodoro		8.00	59	t
404	ice cream		4.00	60	t
405	bistecca		8.00	61	t
\.


--
-- Name: courses_categoryid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.courses_categoryid_seq', 61, true);


--
-- Name: courses_courseid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.courses_courseid_seq', 405, true);


--
-- Data for Name: defaultactionablestates; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.defaultactionablestates (defaultactionablestatesid, stateid) FROM stdin;
28	1
\.


--
-- Name: defaulwaiteractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.defaulwaiteractionablestates_seq', 29, true);


--
-- Data for Name: enablers; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.enablers (enablersid, roleid, stateid, categoryid) FROM stdin;
188	25	2	57
\.


--
-- Name: enablers_elablersid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.enablers_elablersid_seq', 189, true);


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
150	48	cordon blue		t	f	f	0.00	f	unità
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
350	401	115	1.00
351	401	116	\N
352	401	119	1.00
353	401	159	\N
355	403	117	30.00
356	405	116	\N
357	405	153	\N
\.


--
-- Name: ingredientcourseid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientcourseid_seq', 357, true);


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
10	120	10.00	f	f	1.00	1.00
16	138	10.00	f	f	1.00	1.00
42	171	1.00	f	f	6.00	6.00
19	128	10.00	f	f	1.00	1.00
43	172	1.00	f	f	6.00	6.00
57	119	1.00	f	f	0.50	0.50
38	143	1.00	f	f	3.00	3.00
48	120	30.00	f	f	3.00	3.00
15	147	10.00	f	f	1.00	1.00
17	131	10.00	f	f	1.00	1.00
39	150	1.00	f	f	3.00	3.00
46	124	20.00	f	f	2.00	2.00
12	123	10.00	f	f	1.00	1.00
11	124	10.00	f	f	1.00	1.00
33	169	1.00	f	f	2.00	2.00
25	134	10.00	f	f	1.50	1.50
61	115	1.00	f	f	3.00	3.00
47	120	20.00	f	f	2.00	2.00
37	148	1.00	f	f	3.00	3.00
13	145	10.00	f	f	1.00	1.00
41	170	1.00	f	f	6.00	6.00
8	116	10.00	f	f	0.50	0.50
28	156	10.00	f	f	1.50	1.50
58	159	1.00	f	f	1.00	1.00
54	115	2.00	f	f	5.00	5.00
35	144	1.00	f	f	3.00	3.00
31	161	1.00	f	f	2.00	2.00
40	152	10.00	f	f	4.00	4.00
59	168	1.00	f	f	2.00	2.00
32	129	1.00	f	f	2.00	2.00
36	139	1.00	f	f	3.00	3.00
22	164	10.00	f	f	1.00	1.00
23	166	10.00	f	f	1.00	1.00
26	122	10.00	f	f	1.50	1.50
29	167	10.00	f	f	1.00	1.00
14	146	10.00	f	f	1.00	1.00
27	142	10.00	f	f	1.50	1.50
24	165	10.00	f	f	1.00	1.00
55	125	20.00	f	f	1.50	1.50
9	117	10.00	f	f	0.50	0.50
20	125	10.00	f	f	1.00	1.00
62	139	2.00	t	t	4.00	4.00
\.


--
-- Name: ingredientpriceid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientpriceid_seq', 62, true);


--
-- Data for Name: observers; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.observers (observersid, stateid, roleid, categoryid) FROM stdin;
208	2	25	57
\.


--
-- Name: observers_observerid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.observers_observerid_seq', 209, true);


--
-- Name: observers_observersid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.observers_observersid_seq', 1, false);


--
-- Data for Name: orderitems; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orderitems (orderitemid, courseid, quantity, orderid, comment, price, stateid, archived, startingtime, closingtime, ordergroupid, hasbeenrejected, suborderid, isinsasuborder, printcount) FROM stdin;
1037	401	1	450		8.00	6	\N	2018-10-02 13:30:22.982386	2018-10-02 13:43:21.380604	250	f	\N	f	0
1046	401	1	456		8.00	6	\N	2018-10-03 18:32:19.862042	2018-10-03 18:38:47.636755	256	f	\N	f	0
1033	401	1	449		5.00	6	\N	2018-10-02 12:56:21.295913	2018-10-02 13:31:46.509193	248	f	190	t	0
1034	401	1	449		5.00	6	\N	2018-10-02 13:02:56.185711	2018-10-02 13:31:45.074723	248	f	190	t	0
1036	401	1	449		6.50	6	\N	2018-10-02 13:22:36.9375	2018-10-02 13:31:43.601504	249	f	191	t	0
1045	401	1	455		7.00	6	\N	2018-10-03 18:17:40.641164	2018-10-03 18:38:51.165685	255	f	193	t	0
1044	402	1	455		3.00	6	\N	2018-10-03 18:14:16.5468	2018-10-03 18:38:52.162196	254	f	192	t	0
1035	402	1	449		3.00	1	\N	2018-10-02 13:08:52.600411	\N	249	f	\N	f	0
1043	401	1	455		5.00	6	\N	2018-10-03 18:13:40.929254	2018-10-03 18:38:53.089423	254	f	192	t	0
1055	403	1	463		8.00	2	\N	2018-10-07 14:35:07.03255	\N	265	f	0	f	0
1040	401	1	453		5.00	1	\N	2018-10-02 13:59:59.050286	\N	252	t	\N	f	0
1041	401	1	454		5.00	1	\N	2018-10-02 22:21:22.258844	\N	253	t	\N	f	0
1054	405	1	463		6.00	2	\N	2018-10-07 14:33:12.085588	\N	266	f	200	t	0
1042	401	1	454	mod	4.00	1	\N	2018-10-02 22:25:55.120431	\N	253	t	\N	f	0
1047	401	1	457	ben cotto	6.00	6	\N	2018-10-03 19:50:40.228689	2018-10-03 20:03:51.594147	257	f	194	t	0
1048	401	1	457		5.00	6	\N	2018-10-03 20:00:04.013568	2018-10-03 20:05:00.754754	258	f	195	t	0
1056	401	1	463		9.00	1	\N	2018-10-07 18:20:53.411383	\N	266	f	\N	f	0
1049	401	1	458		5.00	6	\N	2018-10-03 20:08:12.503949	2018-10-03 20:46:46.694339	259	f	\N	f	0
1038	401	1	450		5.00	6	\N	2018-10-02 13:30:27.623254	2018-10-02 13:32:26.653339	250	f	\N	f	0
1050	401	1	459		5.00	6	\N	2018-10-03 20:46:15.416546	2018-10-03 20:47:03.052712	260	f	\N	f	0
1051	401	1	459		5.00	6	\N	2018-10-03 20:57:33.376268	2018-10-03 22:56:23.880479	261	f	\N	f	0
1052	401	1	461		8.00	6	\N	2018-10-03 22:52:27.845675	2018-10-03 22:56:32.206268	262	f	196	t	0
1053	402	1	461		3.00	2	\N	2018-10-03 22:55:38.592566	\N	263	f	197	t	0
\.


--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orderitems_orderitemid_seq', 1056, true);


--
-- Data for Name: orderitemstates; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orderitemstates (orderitemstatesid, orderitemid, stateid, startingtime) FROM stdin;
1982	1033	1	2018-10-02 12:56:21.295913
1983	1034	1	2018-10-02 13:02:56.185711
1984	1035	1	2018-10-02 13:08:52.600411
1985	1033	2	2018-10-02 13:10:24.121616
1986	1034	2	2018-10-02 13:10:24.143424
1987	1033	6	2018-10-02 13:18:11.692256
1988	1034	2	2018-10-02 13:22:01.562329
1989	1036	1	2018-10-02 13:22:36.9375
1990	1036	2	2018-10-02 13:23:07.354623
1991	1033	2	2018-10-02 13:24:25.783447
1992	1033	2	2018-10-02 13:25:08.758777
1993	1037	1	2018-10-02 13:30:22.982386
1994	1038	1	2018-10-02 13:30:27.623254
1995	1038	2	2018-10-02 13:31:24.845203
1996	1037	2	2018-10-02 13:31:25.920015
1997	1036	6	2018-10-02 13:31:43.601504
1998	1034	6	2018-10-02 13:31:45.074723
1999	1033	6	2018-10-02 13:31:46.509193
2000	1038	6	2018-10-02 13:32:26.653339
2001	1037	2	2018-10-02 13:42:36.489264
2002	1037	6	2018-10-02 13:43:21.380604
2004	1040	1	2018-10-02 13:59:59.050286
2005	1040	2	2018-10-02 14:00:00.570177
2006	1041	1	2018-10-02 22:21:22.258844
2007	1042	1	2018-10-02 22:25:55.120431
2008	1041	2	2018-10-02 22:29:02.48767
2009	1042	2	2018-10-02 22:29:59.841457
2010	1043	1	2018-10-03 18:13:40.929254
2011	1044	1	2018-10-03 18:14:16.5468
2012	1043	2	2018-10-03 18:15:48.198426
2013	1044	2	2018-10-03 18:15:48.221989
2014	1045	1	2018-10-03 18:17:40.641164
2015	1045	2	2018-10-03 18:20:27.045537
2016	1046	1	2018-10-03 18:32:19.862042
2017	1046	2	2018-10-03 18:33:42.930778
2018	1046	6	2018-10-03 18:38:47.636755
2019	1045	6	2018-10-03 18:38:51.165685
2020	1044	6	2018-10-03 18:38:52.162196
2021	1043	6	2018-10-03 18:38:53.089423
2022	1041	2	2018-10-03 18:39:57.915164
2023	1047	1	2018-10-03 19:50:40.228689
2024	1048	1	2018-10-03 20:00:04.013568
2025	1047	2	2018-10-03 20:01:03.905422
2026	1047	6	2018-10-03 20:03:51.594147
2027	1048	2	2018-10-03 20:04:25.606373
2028	1048	6	2018-10-03 20:05:00.754754
2029	1049	1	2018-10-03 20:08:12.503949
2030	1049	2	2018-10-03 20:08:14.442246
2031	1050	1	2018-10-03 20:46:15.416546
2032	1050	2	2018-10-03 20:46:18.057278
2033	1049	6	2018-10-03 20:46:46.694339
2034	1050	6	2018-10-03 20:47:03.052712
2035	1051	1	2018-10-03 20:57:33.376268
2036	1051	2	2018-10-03 20:57:39.113683
2037	1052	1	2018-10-03 22:52:27.845675
2038	1052	2	2018-10-03 22:54:29.282537
2039	1053	1	2018-10-03 22:55:38.592566
2040	1053	2	2018-10-03 22:55:40.845355
2041	1051	6	2018-10-03 22:56:23.880479
2042	1052	6	2018-10-03 22:56:32.206268
2043	1054	1	2018-10-07 14:33:12.085588
2044	1055	1	2018-10-07 14:35:07.03255
2045	1055	2	2018-10-07 14:35:28.608859
2046	1054	2	2018-10-07 14:37:07.00883
2047	1056	1	2018-10-07 18:20:53.411383
\.


--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orderitemstates_orderitemstates_id_seq', 2047, true);


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
249	0	449	2
248	1	449	1
250	0	450	1
252	0	453	1
253	0	454	1
254	1	455	1
255	2	455	2
256	1	456	1
258	0	457	2
257	1	457	1
259	0	458	1
260	1	459	1
261	0	459	2
262	1	461	1
263	1	461	2
265	2	463	2
264	1	463	1
266	0	463	3
\.


--
-- Name: orderoutgroup_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orderoutgroup_id_seq', 266, true);


--
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orders (orderid, "table", person, ongoing, userid, startingtime, closingtime, voided, archived, total, adjustedtotal, plaintotalvariation, percentagevariataion, adjustispercentage, adjustisplain, forqruserarchived) FROM stdin;
458	1		f	170	2018-10-03 20:07:29.974746	2018-10-03 20:58:01.479193	f	t	5.00	5.00	0.00	0.00	f	f	t
453	3		t	170	2018-10-02 13:59:32.449038	2018-10-03 10:59:09.756359	f	t	5.00	5.00	0.00	0.00	f	f	t
457	1		f	2	2018-10-03 19:50:19.703867	2018-10-03 20:06:06.209554	f	t	11.00	11.00	0.00	0.00	f	f	\N
450	2		f	2	2018-10-02 13:30:18.902624	2018-10-02 13:44:40.291154	f	t	13.00	13.00	0.00	0.00	f	f	\N
455	4		f	2	2018-10-03 18:13:29.502354	2018-10-03 18:27:13.417978	f	t	15.00	15.00	-5.00	0.00	f	t	\N
448	3		f	2	2018-09-28 15:20:41.149321	2018-10-02 13:44:41.573372	f	t	0.00	0.00	0.00	0.00	f	f	\N
449	1		t	2	2018-10-02 12:55:58.549307	2018-10-03 10:59:07.876485	t	t	16.50	16.50	0.00	0.00	f	f	\N
456	3		f	2	2018-10-03 18:32:11.970005	2018-10-03 18:39:07.073838	f	t	8.00	8.00	0.00	0.00	f	f	\N
454	2		t	2	2018-10-02 22:21:18.327387	2018-10-03 10:59:11.403699	t	t	4.00	4.00	0.00	0.00	f	f	\N
462	4		t	170	2018-10-03 22:59:24.972412	2018-10-07 14:37:42.715502	f	t	0.00	0.00	0.00	0.00	f	f	\N
461	6		t	2	2018-10-03 22:51:55.26182	\N	f	f	11.00	9.90	0.00	-10.00	t	f	\N
459	1		f	2	2018-10-03 20:46:11.973257	2018-10-03 20:58:03.513372	f	t	10.00	10.00	0.00	0.00	f	f	\N
447	1		t	2	2018-09-28 11:20:49.881203	2018-10-02 13:52:31.431727	t	t	0.00	0.00	0.00	-10.00	t	f	\N
460	5		t	2	2018-10-03 21:12:53.642454	2018-10-03 22:57:11.658955	f	t	0.00	0.00	0.00	0.00	f	f	\N
463	7		t	2	2018-10-07 14:31:42.599904	\N	f	f	14.00	12.60	0.00	-10.00	t	f	\N
\.


--
-- Name: orders_orderid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orders_orderid_seq', 463, true);


--
-- Data for Name: printerforcategory; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.printerforcategory (printerforcategoryid, categoryid, printerid, stateid) FROM stdin;
40	57	25	2
41	58	28	2
42	61	25	2
43	59	25	2
\.


--
-- Name: printerforcategory_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.printerforcategory_id_seq', 43, true);


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
44	401	non ci sono più' funghi	2018-10-02 13:18:57.523549	1034
45	401	funghi	2018-10-02 13:23:18.219128	1033
46	401	boh	2018-10-02 13:24:42.869936	1033
47	401	ingredienti mancanti	2018-10-02 13:32:06.121971	1037
48	401	non mi va	2018-10-02 22:30:23.425752	1041
49	401	x	2018-10-03 20:03:05.785011	1040
50	401	x	2018-10-03 20:03:18.904531	1041
51	401	x	2018-10-03 20:03:22.025259	1042
\.


--
-- Name: rejectedorderitems_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.rejectedorderitems_id_seq', 51, true);


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
\.


--
-- Name: roles_roleid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.roles_roleid_seq', 26, true);


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
190	449	10.00	\N	f	2018-10-02 13:48:11.269038
191	449	6.50	\N	f	2018-10-02 13:48:54.437523
192	455	8.00	\N	t	2018-10-03 18:25:50.843916
193	455	7.00	\N	t	2018-10-03 18:26:22.20885
194	457	6.00	\N	f	2018-10-03 20:05:26.686485
195	457	5.00	\N	f	2018-10-03 20:05:29.941193
196	461	8.00	\N	f	2018-10-03 22:57:48.407125
197	461	3.00	\N	f	2018-10-03 22:57:53.872079
200	463	8.00	\N	f	2018-10-07 14:39:58.457064
\.


--
-- Name: suborderid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.suborderid_seq', 200, true);


--
-- Data for Name: temp_user_actionable_states; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.temp_user_actionable_states (tempuseractionablestateid, userid, stateid) FROM stdin;
\.


--
-- Name: temp_user_actionable_states_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.temp_user_actionable_states_seq', 13, true);


--
-- Data for Name: temp_user_default_actionable_states; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.temp_user_default_actionable_states (tempmuseractionablestatesid, stateid) FROM stdin;
13	1
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
170	0HDIGWVUW63G		t	f	21	f	2018-10-03 22:59:24.965774	t	f	4	\N	f
\.


--
-- Name: users_userid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.users_userid_seq', 170, true);


--
-- Data for Name: variations; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.variations (variationsid, orderitemid, ingredientid, tipovariazione, plailnumvariation, ingredientpriceid) FROM stdin;
936	1036	125	PER_PREZZO_INGREDIENTE	\N	55
937	1033	143	✋	\N	\N
938	1037	143	PER_PREZZO_INGREDIENTE	\N	38
941	1037	168	unità	1	\N
943	1038	116	🚫	\N	\N
944	1038	125	✋	\N	\N
945	1037	159	🚫	\N	\N
950	1041	119	unità	-1	\N
953	1042	159	🚫	\N	\N
954	1042	168	unità	1	\N
955	1045	159	🚫	\N	\N
956	1045	168	unità	1	\N
957	1046	150	PER_PREZZO_INGREDIENTE	\N	39
958	1046	116	😍	\N	\N
960	1047	116	🚫	\N	\N
966	1047	125	PER_PREZZO_INGREDIENTE	\N	20
967	1052	143	PER_PREZZO_INGREDIENTE	\N	38
968	1054	116	😍	\N	\N
969	1056	139	PER_PREZZO_INGREDIENTE	\N	62
\.


--
-- Name: variations_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.variations_seq', 969, true);


--
-- Name: voidedorderslog_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.voidedorderslog_id_seq', 219, true);


--
-- Data for Name: voidedorderslogbuffer; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.voidedorderslogbuffer (voidedorderslogbufferid, voidedtime, orderid, userid) FROM stdin;
217	2018-10-02 13:02:46.552562	447	2
218	2018-10-02 22:20:56.17596	449	2
219	2018-10-03 18:40:15.656141	454	2
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
293	170	1
\.


--
-- Name: waiteractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.waiteractionablestates_seq', 293, true);


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

