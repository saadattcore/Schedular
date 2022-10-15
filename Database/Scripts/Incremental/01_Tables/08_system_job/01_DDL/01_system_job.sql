ALTER SESSION SET CURRENT_SCHEMA=ECORE_SCHEDULER; 

create table SYSTEM_JOB
(
  id           NVARCHAR2(32) not null,
  system_key   NVARCHAR2(32) not null,
  job_id       NVARCHAR2(32) not null,
  created_date TIMESTAMP(6) not null,
  created_by   NCHAR(32) not null,
  deleted_date TIMESTAMP(6),
  deleted_by   NCHAR(32),
  is_deleted   NUMBER(1) default 0 not null
)
;
alter table SYSTEM_JOB
  add constraint PK_SYSTEM_JOB primary key (ID);

