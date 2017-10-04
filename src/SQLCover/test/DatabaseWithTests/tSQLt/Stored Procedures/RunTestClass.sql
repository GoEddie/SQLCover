
CREATE PROCEDURE tSQLt.RunTestClass
   @TestClassName NVARCHAR(MAX)
AS
BEGIN
    EXEC tSQLt.Run @TestClassName;
END
