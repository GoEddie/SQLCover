
CREATE FUNCTION tSQLt.Private_IsTestClass(@TestClassName NVARCHAR(MAX))
RETURNS BIT
AS
BEGIN
  RETURN 
    CASE 
      WHEN EXISTS(
             SELECT 1 
               FROM tSQLt.TestClasses
              WHERE SchemaId = tSQLt.Private_GetSchemaId(@TestClassName)
            )
      THEN 1
      ELSE 0
    END;
END;
