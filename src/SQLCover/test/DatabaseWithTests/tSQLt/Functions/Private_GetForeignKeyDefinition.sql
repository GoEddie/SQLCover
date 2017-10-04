
CREATE FUNCTION tSQLt.Private_GetForeignKeyDefinition(
    @SchemaName NVARCHAR(MAX),
    @ParentTableName NVARCHAR(MAX),
    @ForeignKeyName NVARCHAR(MAX),
    @NoCascade BIT
)
RETURNS TABLE
AS
RETURN SELECT 'CONSTRAINT ' + name + ' FOREIGN KEY (' +
              parCols + ') REFERENCES ' + refName + '(' + refCols + ')'+
              CASE WHEN @NoCascade = 1 THEN ''
                ELSE delete_referential_action_cmd + ' ' + update_referential_action_cmd 
              END AS cmd,
              CASE 
                WHEN RefTableIsFakedInd = 1
                  THEN 'CREATE UNIQUE INDEX ' + tSQLt.Private::CreateUniqueObjectName() + ' ON ' + refName + '(' + refCols + ');' 
                ELSE '' 
              END CreIdxCmd
         FROM (SELECT QUOTENAME(SCHEMA_NAME(k.schema_id)) AS SchemaName,
                      QUOTENAME(k.name) AS name,
                      QUOTENAME(OBJECT_NAME(k.parent_object_id)) AS parName,
                      QUOTENAME(SCHEMA_NAME(refTab.schema_id)) + '.' + QUOTENAME(refTab.name) AS refName,
                      parCol.ColNames AS parCols,
                      refCol.ColNames AS refCols,
                      'ON UPDATE '+
                      CASE k.update_referential_action
                        WHEN 0 THEN 'NO ACTION'
                        WHEN 1 THEN 'CASCADE'
                        WHEN 2 THEN 'SET NULL'
                        WHEN 3 THEN 'SET DEFAULT'
                      END AS update_referential_action_cmd,
                      'ON DELETE '+
                      CASE k.delete_referential_action
                        WHEN 0 THEN 'NO ACTION'
                        WHEN 1 THEN 'CASCADE'
                        WHEN 2 THEN 'SET NULL'
                        WHEN 3 THEN 'SET DEFAULT'
                      END AS delete_referential_action_cmd,
                      CASE WHEN e.name IS NULL THEN 0
                           ELSE 1 
                       END AS RefTableIsFakedInd
                 FROM sys.foreign_keys k
                 CROSS APPLY tSQLt.Private_GetForeignKeyParColumns(k.object_id) AS parCol
                 CROSS APPLY tSQLt.Private_GetForeignKeyRefColumns(k.object_id) AS refCol
                 LEFT JOIN sys.extended_properties e
                   ON e.name = 'tSQLt.FakeTable_OrgTableName'
                  AND e.value = OBJECT_NAME(k.referenced_object_id)
                 JOIN sys.tables refTab
                   ON COALESCE(e.major_id,k.referenced_object_id) = refTab.object_id
                WHERE k.parent_object_id = OBJECT_ID(@SchemaName + '.' + @ParentTableName)
                  AND k.object_id = OBJECT_ID(@SchemaName + '.' + @ForeignKeyName)
               )x;
