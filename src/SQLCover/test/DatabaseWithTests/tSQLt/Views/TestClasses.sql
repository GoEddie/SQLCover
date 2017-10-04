
----------------------------------------------------------------------
CREATE VIEW tSQLt.TestClasses
AS
  SELECT s.name AS Name, s.schema_id AS SchemaId
    FROM sys.extended_properties ep
    JOIN sys.schemas s
      ON ep.major_id = s.schema_id
   WHERE ep.name = N'tSQLt.TestClass';
