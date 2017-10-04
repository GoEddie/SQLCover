
CREATE PROCEDURE tSQLt.RunWithXmlResults
   @TestName NVARCHAR(MAX) = NULL
AS
BEGIN
  EXEC tSQLt.Run @TestName = @TestName, @TestResultFormatter = 'tSQLt.XmlResultFormatter';
END;
