ALTER SESSION SET CURRENT_SCHEMA=ECORE_SCHEDULER; 

create table JOB_SCHEDULE
(
  id                 NVARCHAR2(32) not null,
  job_id             NVARCHAR2(32) not null,
  schedule_type      NVARCHAR2(32) not null,
  schedule_frequency NUMBER default 1 not null,
  version            NUMBER default 1 not null,
  is_deleted         NUMBER(1) default 0 not null,
  created_by         NCHAR(32) not null,
  created_date       TIMESTAMP(6) not null,
  deleted_by         NCHAR(32),
  deleted_date       TIMESTAMP(6)
)
;
alter table JOB_SCHEDULE
  add constraint PK_JOB_SCHEDULE primary key (ID);
alter table JOB_SCHEDULE
  add constraint FK_JOB_SCHEDULE_JOB foreign key (JOB_ID)
  references JOB (ID);
alter table JOB_SCHEDULE
  add constraint FK_JOB_SCHEDULE_SCHEDULE_TYPE foreign key (SCHEDULE_TYPE)
  references SCHEDULE_TYPE (ID);

