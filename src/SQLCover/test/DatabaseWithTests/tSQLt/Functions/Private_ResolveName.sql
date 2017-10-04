
CREATE FUNCTION tSQLt.Private_ResolveName(@Name NVARCHAR(MAX))
RETURNS TABLE 
AS
RETURN
  WITH resolvedNames(ord, schemaId, objectId, quotedSchemaName, quotedObjectName, quotedFullName, isTestClass, isTestCase, isSchema) AS
  (SELECT 1, schemaId, NULL, quotedSchemaName, NULL, quotedSchemaName, isTestClass, 0, 1
     FROM tSQLt.Private_ResolveSchemaName(@Name)
    UNION ALL
   SELECT 2, schemaId, objectId, quotedSchemaName, quotedObjectName, quotedFullName, 0, isTestCase, 0
     FROM tSQLt.Private_ResolveObjectName(@Name)
    UNION ALL
   SELECT 3, NULL, NULL, NULL, NULL, NULL, 0, 0, 0
   )
   SELECT TOP(1) schemaId, objectId, quotedSchemaName, quotedObjectName, quotedFullName, isTestClass, isTestCase, isSchema
     FROM resolvedNames
    WHERE schemaId IS NOT NULL 
       OR ord = 3
    ORDER BY ord
