ALTER SESSION SET CURRENT_SCHEMA=ECORE_SCHEDULER; 

create table JOB_PARAMETER
(
  job_id        NVARCHAR2(32) not null,
  start_version NUMBER not null,
  end_version   NUMBER,
  name          NVARCHAR2(256),
  value         NVARCHAR2(256)
)
;
alter table JOB_PARAMETER
  add constraint FK_JOB_PARAMETER_JOB foreign key (JOB_ID)
  references JOB (ID);

