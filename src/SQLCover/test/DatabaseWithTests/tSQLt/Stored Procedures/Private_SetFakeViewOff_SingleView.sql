
CREATE PROCEDURE [tSQLt].[Private_SetFakeViewOff_SingleView]
  @ViewName NVARCHAR(MAX)
AS
BEGIN
  DECLARE @Cmd NVARCHAR(MAX),
          @SchemaName NVARCHAR(MAX),
          @TriggerName NVARCHAR(MAX);
          
  SELECT @SchemaName = QUOTENAME(OBJECT_SCHEMA_NAME(ObjId)),
         @TriggerName = QUOTENAME(OBJECT_NAME(ObjId) + '_SetFakeViewOn')
    FROM (SELECT OBJECT_ID(@ViewName) AS ObjId) X;
  
  SET @Cmd = 'DROP TRIGGER %SCHEMA_NAME%.%TRIGGER_NAME%;';
      
  SET @Cmd = REPLACE(@Cmd, '%SCHEMA_NAME%', @SchemaName);
  SET @Cmd = REPLACE(@Cmd, '%TRIGGER_NAME%', @TriggerName);
  
  EXEC(@Cmd);
END;
