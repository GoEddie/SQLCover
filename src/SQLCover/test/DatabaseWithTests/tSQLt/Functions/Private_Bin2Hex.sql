CREATE FUNCTION tSQLt.Private_Bin2Hex(@vb VARBINARY(MAX))
RETURNS TABLE
AS
RETURN
  SELECT X.S AS bare, '0x'+X.S AS prefix
    FROM (SELECT LOWER(CAST('' AS XML).value('xs:hexBinary(sql:variable("@vb") )','VARCHAR(MAX)')))X(S);
