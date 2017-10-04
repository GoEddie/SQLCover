
CREATE PROCEDURE tSQLt.RunNew
AS
BEGIN
  EXEC tSQLt.Private_RunMethodHandler @RunMethod = 'tSQLt.Private_RunNew';
END;
