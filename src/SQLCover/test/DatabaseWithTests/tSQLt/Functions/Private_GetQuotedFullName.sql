
CREATE FUNCTION tSQLt.Private_GetQuotedFullName(@Objectid INT)
RETURNS NVARCHAR(517)
AS
BEGIN
    DECLARE @QuotedName NVARCHAR(517);
    SELECT @QuotedName = QUOTENAME(OBJECT_SCHEMA_NAME(@Objectid)) + '.' + QUOTENAME(OBJECT_NAME(@Objectid));
    RETURN @QuotedName;
END;
