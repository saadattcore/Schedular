ALTER SESSION SET CURRENT_SCHEMA=ECORE_SCHEDULER; 

create table JOB
(
  id                    NVARCHAR2(32) not null,
  job_id                NVARCHAR2(32) not null,
  name                  NVARCHAR2(256) not null,
  job_type              NVARCHAR2(256),
  version               NUMBER default 1 not null,
  is_deleted            NUMBER(1) default 0 not null,
  is_active             NUMBER(1) not null,
  created_by            NCHAR(32) not null,
  created_date          TIMESTAMP(6) not null,
  deleted_by            NCHAR(32),
  deleted_date          TIMESTAMP(6),
  clone_source_id       NVARCHAR2(32),
  clone_source_version  NUMBER,
  reverted_from_version NUMBER,
  is_enabled            NUMBER(1) default 0 not null,
  is_locked             NUMBER(1) default 0 not null,
  locked_by             NCHAR(32) not null,
  locked_date           TIMESTAMP(6) not null,
  is_executed           NUMBER(1) default 0 not null,
  executed_date         TIMESTAMP(6) not null,
  executed_by           NCHAR(32) not null
)
;
alter table JOB
  add constraint PK_JOB primary key (ID);
alter table JOB
  add constraint UN_JOB unique (ID, VERSION);

