
CREATE PROCEDURE tSQLt.Private_RunMethodHandler
  @RunMethod NVARCHAR(MAX),
  @TestResultFormatter NVARCHAR(MAX) = NULL,
  @TestName NVARCHAR(MAX) = NULL
AS
BEGIN
  SELECT @TestResultFormatter = ISNULL(@TestResultFormatter,tSQLt.GetTestResultFormatter());

  EXEC tSQLt.Private_Init;
  IF(@@ERROR = 0)
  BEGIN  
    IF(EXISTS(SELECT * FROM sys.parameters AS P WHERE P.object_id = OBJECT_ID(@RunMethod) AND name = '@TestName'))
    BEGIN
      EXEC @RunMethod @TestName = @TestName, @TestResultFormatter = @TestResultFormatter;
    END;
    ELSE
    BEGIN  
      EXEC @RunMethod @TestResultFormatter = @TestResultFormatter;
    END;
  END;
END;
