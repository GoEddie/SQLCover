CREATE PROCEDURE tSQLt.Private_ResetNewTestClassList
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM tSQLt.Private_NewTestClassList;
END;
