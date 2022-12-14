

CREATE TABLE JOB_INSTANCE
(
  ID			NVARCHAR2(32) NOT NULL,
  JOB_ID		NVARCHAR2(32) NOT NULL,
  IS_LOCKED		NUMBER(1) DEFAULT 0 NOT NULL,
  LOCKED_BY		NCHAR(32) NOT NULL,
  LOCKED_DATE	TIMESTAMP NOT NULL,
  IS_EXECUTED   NUMBER(1) DEFAULT 0 NOT NULL,
  EXECUTED_DATE	TIMESTAMP NOT NULL,
  EXECUTED_BY	NCHAR(32) NOT NULL,
  IS_DELETED    NUMBER(1) DEFAULT 0 NOT NULL,
  CREATED_BY    NCHAR(32) NOT NULL,
  CREATED_DATE  TIMESTAMP NOT NULL,
  DELETED_BY    NCHAR(32),
  DELETED_DATE  TIMESTAMP
);
ALTER TABLE "JOB_INSTANCE" ADD CONSTRAINT "PK_JOB_INSTANCE" PRIMARY KEY("ID");
ALTER TABLE "JOB_INSTANCE" ADD CONSTRAINT "FK_JOB_INSTANCE_JOB" FOREIGN KEY ("JOB_ID") REFERENCES JOB ("ID");


INSERT INTO "DB_VERSION"("VERSION") VALUES(2);

--
-- ROLLBACK
--

DROP TABLE "JOB_INSTANCE" CASCADE CONSTRAINTS;  

INSERT INTO "DB_VERSION"("VERSION") VALUES(1);
