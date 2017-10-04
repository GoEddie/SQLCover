
CREATE FUNCTION tSQLt.Private_GetSchemaId(@SchemaName NVARCHAR(MAX))
RETURNS INT
AS
BEGIN
  RETURN (
    SELECT TOP(1) schema_id
      FROM sys.schemas
     WHERE @SchemaName IN (name, QUOTENAME(name), QUOTENAME(name, '"'))
     ORDER BY 
        CASE WHEN name = @SchemaName THEN 0 ELSE 1 END
  );
END;
