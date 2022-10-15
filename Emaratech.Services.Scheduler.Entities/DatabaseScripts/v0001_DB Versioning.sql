----
-- DB VERSIONING
--
CREATE TABLE DB_VERSION(
  ID NUMBER(6) NOT NULL,
  VERSION NUMBER(4) NOT NULL,
  APPLIED_BY NVARCHAR2(256) NOT NULL,
  APPLIED_DATE TIMESTAMP NOT NULL,
  SUCCESS NUMBER(1) DEFAULT 1 NOT NULL,
  ERROR_MESSAGE NVARCHAR2(1000)
);

DECLARE
  I NUMBER;
BEGIN
  SELECT COALESCE(MAX(ID)+1, 1) INTO I FROM DB_VERSION;
  EXECUTE IMMEDIATE('CREATE SEQUENCE SEQ_DB_VERSION START WITH ' || I || ' INCREMENT BY 1 NOCACHE');
END;
/

CREATE TRIGGER TRG_DB_VERSION_ID BEFORE INSERT ON DB_VERSION FOR EACH ROW
BEGIN
  IF :NEW.ID IS NULL THEN
    SELECT SEQ_DB_VERSION.NEXTVAL INTO :NEW.ID FROM DUAL;
  END IF;
  
  IF :NEW.APPLIED_BY IS NULL THEN
    SELECT USER INTO :NEW.APPLIED_BY FROM DUAL;
  END IF;
  
  IF :NEW.APPLIED_DATE IS NULL THEN
    SELECT CURRENT_TIMESTAMP INTO :NEW.APPLIED_DATE FROM DUAL;
  END IF;
END;
/

CREATE FUNCTION CURRENT_DB_VERSION
RETURN NUMBER
IS RESULT NUMBER;
BEGIN
  SELECT VERSION INTO RESULT FROM (
    SELECT VERSION FROM DB_VERSION WHERE SUCCESS = 1 ORDER BY APPLIED_DATE DESC
  ) WHERE ROWNUM = 1;
  
  RETURN RESULT;
END;
/

INSERT INTO DB_VERSION(VERSION) VALUES(0);

--
--ROLLBACK
--
DROP FUNCTION CURRENT_DB_VERSION;

DROP TABLE DB_VERSION;
DROP SEQUENCE SEQ_DB_VERSION;
