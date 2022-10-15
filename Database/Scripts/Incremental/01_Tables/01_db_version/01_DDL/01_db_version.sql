ALTER SESSION SET CURRENT_SCHEMA=ECORE_SCHEDULER; 

create table DB_VERSION
(
  id            NUMBER(6) not null,
  version       NUMBER(4) not null,
  applied_by    NVARCHAR2(256) not null,
  applied_date  TIMESTAMP(6) not null,
  success       NUMBER(1) default 1 not null,
  error_message NVARCHAR2(1000)
)
;

