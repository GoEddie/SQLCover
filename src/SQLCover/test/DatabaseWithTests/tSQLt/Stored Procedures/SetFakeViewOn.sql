
CREATE PROCEDURE [tSQLt].[SetFakeViewOn]
  @SchemaName NVARCHAR(MAX)
AS
BEGIN
  DECLARE @ViewName NVARCHAR(MAX);
    
  DECLARE viewNames CURSOR LOCAL FAST_FORWARD FOR
  SELECT QUOTENAME(OBJECT_SCHEMA_NAME(object_id)) + '.' + QUOTENAME([name]) AS viewName
    FROM sys.views
   WHERE schema_id = SCHEMA_ID(@SchemaName);
  
  OPEN viewNames;
  
  FETCH NEXT FROM viewNames INTO @ViewName;
  WHILE @@FETCH_STATUS = 0
  BEGIN
    EXEC tSQLt.Private_SetFakeViewOn_SingleView @ViewName;
    
    FETCH NEXT FROM viewNames INTO @ViewName;
  END;
  
  CLOSE viewNames;
  DEALLOCATE viewNames;
END;
