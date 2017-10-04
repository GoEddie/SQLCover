
CREATE FUNCTION tSQLt.Private_ResolveObjectName(@Name NVARCHAR(MAX))
RETURNS TABLE 
AS
RETURN
  WITH ids(schemaId, objectId) AS
       (SELECT SCHEMA_ID(OBJECT_SCHEMA_NAME(OBJECT_ID(@Name))),
               OBJECT_ID(@Name)
       ),
       idsWithNames(schemaId, objectId, quotedSchemaName, quotedObjectName) AS
        (SELECT schemaId, objectId,
         QUOTENAME(SCHEMA_NAME(schemaId)) AS quotedSchemaName, 
         QUOTENAME(OBJECT_NAME(objectId)) AS quotedObjectName
         FROM ids
        )
  SELECT schemaId, 
         objectId, 
         quotedSchemaName,
         quotedObjectName,
         quotedSchemaName + '.' + quotedObjectName AS quotedFullName, 
         CASE WHEN LOWER(quotedObjectName) LIKE '[[]test%]' 
               AND objectId = OBJECT_ID(quotedSchemaName + '.' + quotedObjectName,'P') 
              THEN 1 ELSE 0 END AS isTestCase
    FROM idsWithNames;
    
