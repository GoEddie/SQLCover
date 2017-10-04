

/*******************************************************************************************/
/*******************************************************************************************/
/*******************************************************************************************/
CREATE FUNCTION tSQLt.Private_GetOriginalTableName(@SchemaName NVARCHAR(MAX), @TableName NVARCHAR(MAX)) --DELETE!!!
RETURNS NVARCHAR(MAX)
AS
BEGIN
  RETURN (SELECT CAST(value AS NVARCHAR(4000))
    FROM sys.extended_properties
   WHERE class_desc = 'OBJECT_OR_COLUMN'
     AND major_id = OBJECT_ID(@SchemaName + '.' + @TableName)
     AND minor_id = 0
     AND name = 'tSQLt.FakeTable_OrgTableName');
END;
