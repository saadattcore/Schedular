ALTER SESSION SET CURRENT_SCHEMA=ECORE_SCHEDULER; 

ALTER TABLE JOB
ADD CONSTRAINT UK_JOB_ID_VERSION UNIQUE (JOB_ID,VERSION);