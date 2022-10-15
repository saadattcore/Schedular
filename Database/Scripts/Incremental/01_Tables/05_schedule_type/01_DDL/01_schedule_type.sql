ALTER SESSION SET CURRENT_SCHEMA=ECORE_SCHEDULER; 

create table SCHEDULE_TYPE
(
  id   NVARCHAR2(32) not null,
  type NVARCHAR2(256) not null
)
;
alter table SCHEDULE_TYPE
  add constraint PK_SCHEDULE_TYPE primary key (ID);
