ALTER SESSION SET CURRENT_SCHEMA=ECORE_SCHEDULER; 

create table JOB_SOURCE
(
  id      NVARCHAR2(32) not null,
  class   NVARCHAR2(256) not null,
  process NVARCHAR2(256) not null,
  api     NVARCHAR2(256) not null
)
;
alter table JOB_SOURCE
  add constraint PK_JOB_SOURCE primary key (ID);
alter table JOB_SOURCE
  add constraint FK_JOB_SOURCE_JOB foreign key (ID)
  references JOB (ID);

