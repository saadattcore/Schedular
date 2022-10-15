


ALTER TABLE "JOB" 

ADD  (
	IS_ENABLED		NUMBER(1) DEFAULT 0 NOT NULL,
	IS_LOCKED		NUMBER(1) DEFAULT 0 NOT NULL,
 LOCKED_BY		NCHAR(32) NOT NULL,
 LOCKED_DATE	TIMESTAMP NOT NULL,
 IS_EXECUTED   NUMBER(1) DEFAULT 0 NOT NULL,
 EXECUTED_DATE	TIMESTAMP NOT NULL,
 EXECUTED_BY	NCHAR(32) NOT NULL);

INSERT INTO "DB_VERSION"("VERSION") VALUES(3);

--
-- ROLLBACK
--

ALTER TABLE "JOB" 

DROP (
 IS_LOCKED,
 LOCKED_BY,
 LOCKED_DATE,
 IS_EXECUTED,
 EXECUTED_DATE,
 EXECUTED_BY);


INSERT INTO "DB_VERSION"("VERSION") VALUES(2);