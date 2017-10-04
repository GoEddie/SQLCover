

CREATE PROCEDURE tSQLt.Private_RunCursor
  @TestResultFormatter NVARCHAR(MAX),
  @GetCursorCallback NVARCHAR(MAX)
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @TestClassName NVARCHAR(MAX);
  DECLARE @TestProcName NVARCHAR(MAX);

  DECLARE @TestClassCursor CURSOR;
  EXEC @GetCursorCallback @TestClassCursor = @TestClassCursor OUT;
----  
  WHILE(1=1)
  BEGIN
    FETCH NEXT FROM @TestClassCursor INTO @TestClassName;
    IF(@@FETCH_STATUS<>0)BREAK;

    EXEC tSQLt.Private_RunTestClass @TestClassName;
    
  END;
  
  CLOSE @TestClassCursor;
  DEALLOCATE @TestClassCursor;
  
  EXEC tSQLt.Private_OutputTestResults @TestResultFormatter;
END;
