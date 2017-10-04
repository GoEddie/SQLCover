
CREATE FUNCTION tSQLt.Private_GetForeignKeyRefColumns(
    @ConstraintObjectId INT
)
RETURNS TABLE
AS
RETURN SELECT STUFF((
                 SELECT ','+QUOTENAME(rci.name) FROM sys.foreign_key_columns c
                   JOIN sys.columns rci
                  ON rci.object_id = c.referenced_object_id
                  AND rci.column_id = c.referenced_column_id
                   WHERE @ConstraintObjectId = c.constraint_object_id
                   FOR XML PATH(''),TYPE
                   ).value('.','NVARCHAR(MAX)'),1,1,'')  AS ColNames;
