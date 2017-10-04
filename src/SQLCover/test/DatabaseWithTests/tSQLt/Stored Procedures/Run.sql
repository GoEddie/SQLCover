
CREATE PROCEDURE tSQLt.Run
   @TestName NVARCHAR(MAX) = NULL,
   @TestResultFormatter NVARCHAR(MAX) = NULL
AS
BEGIN
  EXEC tSQLt.Private_RunMethodHandler @RunMethod = 'tSQLt.Private_Run', @TestResultFormatter = @TestResultFormatter, @TestName = @TestName; 
END;
