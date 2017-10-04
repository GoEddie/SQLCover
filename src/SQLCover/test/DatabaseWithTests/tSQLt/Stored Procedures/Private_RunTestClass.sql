
CREATE PROCEDURE tSQLt.Private_RunTestClass
  @TestClassName NVARCHAR(MAX)
AS
BEGIN
    DECLARE @TestCaseName NVARCHAR(MAX);
    DECLARE @TestClassId INT; SET @TestClassId = tSQLt.Private_GetSchemaId(@TestClassName);
    DECLARE @SetupProcName NVARCHAR(MAX);
    EXEC tSQLt.Private_GetSetupProcedureName @TestClassId, @SetupProcName OUTPUT;
    
    DECLARE testCases CURSOR LOCAL FAST_FORWARD 
        FOR
     SELECT tSQLt.Private_GetQuotedFullName(object_id)
       FROM sys.procedures
      WHERE schema_id = @TestClassId
        AND LOWER(name) LIKE 'test%';

    OPEN testCases;
    
    FETCH NEXT FROM testCases INTO @TestCaseName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        EXEC tSQLt.Private_RunTest @TestCaseName, @SetupProcName;

        FETCH NEXT FROM testCases INTO @TestCaseName;
    END;

    CLOSE testCases;
    DEALLOCATE testCases;
END;
