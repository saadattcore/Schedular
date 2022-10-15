ALTER SESSION SET CURRENT_SCHEMA=ECORE_SCHEDULER; 

create table JOB_LOCK
(
  job_id       NCHAR(32) not null,
  locked_by    NCHAR(32) not null,
  created_date DATE not null
)
;
alter table JOB_LOCK
  add primary key (JOB_ID);
  