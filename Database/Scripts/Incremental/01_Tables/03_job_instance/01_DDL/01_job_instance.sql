ALTER SESSION SET CURRENT_SCHEMA=ECORE_SCHEDULER; 

create table JOB_INSTANCE
(
  id            NVARCHAR2(32) not null,
  job_id        NVARCHAR2(32) not null,
  is_locked     NUMBER(1) default 0 not null,
  locked_by     NCHAR(32) not null,
  locked_date   TIMESTAMP(6) not null,
  is_executed   NUMBER(1) default 0 not null,
  executed_date TIMESTAMP(6) not null,
  executed_by   NCHAR(32) not null,
  is_deleted    NUMBER(1) default 0 not null,
  created_by    NCHAR(32) not null,
  created_date  TIMESTAMP(6) not null,
  deleted_by    NCHAR(32),
  deleted_date  TIMESTAMP(6)
)
;
alter table JOB_INSTANCE
  add constraint PK_JOB_INSTANCE primary key (ID);
alter table JOB_INSTANCE
  add constraint FK_JOB_INSTANCE_JOB foreign key (JOB_ID)
  references JOB (ID);

