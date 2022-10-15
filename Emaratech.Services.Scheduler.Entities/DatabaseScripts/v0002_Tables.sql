--
-- INITIAL DATABASE SCHEMA
--

CREATE TABLE JOB (
 ID				NVARCHAR2(32) NOT NULL,
 JOB_ID			NVARCHAR2(32) NOT NULL,
 NAME				NVARCHAR2(256) NOT NULL, 
 JOB_TYPE			NVARCHAR2(256), 
 VERSION          NUMBER DEFAULT 1 NOT NULL,
 IS_DELETED       NUMBER(1) DEFAULT 0 NOT NULL,
 IS_ACTIVE        NUMBER(1) NOT NULL,
 CREATED_BY       NCHAR(32) NOT NULL,
 CREATED_DATE     TIMESTAMP NOT NULL,
 DELETED_BY       NCHAR(32),
 DELETED_DATE     TIMESTAMP,
 CLONE_SOURCE_ID	NVARCHAR2(32),
 CLONE_SOURCE_VERSION		NUMBER,
 REVERTED_FROM_VERSION	NUMBER
);
ALTER TABLE "JOB" ADD CONSTRAINT "PK_JOB" PRIMARY KEY("ID");
ALTER TABLE "JOB" ADD CONSTRAINT "UN_JOB" UNIQUE ("ID","VERSION");

CREATE TABLE JOB_SOURCE (
 "ID" NVARCHAR2(32) NOT NULL,
 "CLASS" NVARCHAR2(256) NOT NULL,
 "PROCESS" NVARCHAR2(256) NOT NULL, 
 "API" NVARCHAR2(256) NOT NULL
 ---JOB SOURCE CAN'T BE CHANGED THUS NO VERSION OR CREATED DELETED. ALL IS ACCOUNTED FOR IN JOB INSTANCE
);
ALTER TABLE "JOB_SOURCE" ADD CONSTRAINT "PK_JOB_SOURCE" PRIMARY KEY("ID");
ALTER TABLE "JOB_SOURCE" ADD CONSTRAINT "FK_JOB_SOURCE_JOB" FOREIGN KEY ("ID") REFERENCES JOB ("ID");

CREATE TABLE JOB_PARAMETER
(
  JOB_ID  NVARCHAR2(32) NOT NULL,
  START_VERSION  NUMBER NOT NULL,
  END_VERSION  NUMBER,
  NAME   NVARCHAR2(256),
  VALUE   NVARCHAR2(256)
);
ALTER TABLE "JOB_PARAMETER" ADD CONSTRAINT "FK_JOB_PARAMETER_JOB" FOREIGN KEY ("JOB_ID") REFERENCES JOB ("ID");

CREATE TABLE SCHEDULE_TYPE (
 "ID" NVARCHAR2(32) NOT NULL,
 "TYPE" NVARCHAR2(256) NOT NULL
);
ALTER TABLE "SCHEDULE_TYPE" ADD CONSTRAINT "PK_SCHEDULE_TYPE" PRIMARY KEY("ID");

CREATE TABLE JOB_SCHEDULE
(
  ID			NVARCHAR2(32) NOT NULL,
  JOB_ID		NVARCHAR2(32) NOT NULL,
  SCHEDULE_TYPE	NVARCHAR2(32) NOT NULL,
  SCHEDULE_FREQUENCY NUMBER DEFAULT 1 NOT NULL,
  VERSION      NUMBER DEFAULT 1 NOT NULL,  
  IS_DELETED   NUMBER(1) DEFAULT 0 NOT NULL,
  CREATED_BY   NCHAR(32) NOT NULL,
  CREATED_DATE TIMESTAMP NOT NULL,
  DELETED_BY   NCHAR(32),
  DELETED_DATE TIMESTAMP
);
ALTER TABLE "JOB_SCHEDULE" ADD CONSTRAINT "PK_JOB_SCHEDULE" PRIMARY KEY("ID");
ALTER TABLE "JOB_SCHEDULE" ADD CONSTRAINT "FK_JOB_SCHEDULE_SCHEDULE_TYPE" FOREIGN KEY ("SCHEDULE_TYPE") REFERENCES SCHEDULE_TYPE ("ID");
ALTER TABLE "JOB_SCHEDULE" ADD CONSTRAINT "FK_JOB_SCHEDULE_JOB" FOREIGN KEY ("JOB_ID") REFERENCES JOB ("ID");


CREATE TABLE SYSTEM_JOB
(
  ID            NVARCHAR2(32) NOT NULL,
  SYSTEM_KEY    NVARCHAR2(32) NOT NULL,
  JOB_ID		NVARCHAR2(32) NOT NULL,
  CREATED_DATE  TIMESTAMP NOT NULL,
  CREATED_BY    NCHAR(32) NOT NULL,
  DELETED_DATE  TIMESTAMP,
  DELETED_BY    NCHAR(32),
  IS_DELETED    NUMBER(1) DEFAULT 0 NOT NULL
);  
ALTER TABLE SYSTEM_JOB
  ADD CONSTRAINT PK_SYSTEM_JOB PRIMARY KEY (ID);

INSERT INTO "DB_VERSION"("VERSION") VALUES(1);

--
-- ROLLBACK
--

DROP TABLE "SYSTEM_JOB";  
DROP TABLE "SCHEDULE_TYPE" CASCADE CONSTRAINTS;  
DROP TABLE "JOB_SCHEDULE" CASCADE CONSTRAINTS; 
DROP TABLE "JOB_PARAMETERS" CASCADE CONSTRAINTS; 
DROP TABLE "JOB_SOURCE" CASCADE CONSTRAINTS; 
DROP TABLE "JOB" CASCADE CONSTRAINTS; 

INSERT INTO "DB_VERSION"("VERSION") VALUES(0);
