ALTER SESSION SET CURRENT_SCHEMA=ECORE_SCHEDULER; 

create sequence SEQ_DB_VERSION
minvalue 1
maxvalue 9999999999999999999999999999
start with 5
increment by 1
nocache;

