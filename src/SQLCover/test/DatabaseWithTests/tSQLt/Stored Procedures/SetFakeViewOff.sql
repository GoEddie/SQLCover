
CREATE PROCEDURE [tSQLt].[SetFakeViewOff]
  @SchemaName NVARCHAR(MAX)
AS
BEGIN
  DECLARE @ViewName NVARCHAR(MAX);
    
  DECLARE viewNames CURSOR LOCAL FAST_FORWARD FOR
   SELECT QUOTENAME(OBJECT_SCHEMA_NAME(t.parent_id)) + '.' + QUOTENAME(OBJECT_NAME(t.parent_id)) AS viewName
     FROM sys.extended_properties ep
     JOIN sys.triggers t
       on ep.major_id = t.object_id
     WHERE ep.name = N'SetFakeViewOnTrigger'  
  OPEN viewNames;
  
  FETCH NEXT FROM viewNames INTO @ViewName;
  WHILE @@FETCH_STATUS = 0
  BEGIN
    EXEC tSQLt.Private_SetFakeViewOff_SingleView @ViewName;
    
    FETCH NEXT FROM viewNames INTO @ViewName;
  END;
  
  CLOSE viewNames;
  DEALLOCATE viewNames;
END;
