CREATE PROCEDURE tSQLt.SetVerbose
  @Verbose BIT = 1
AS
BEGIN
  EXEC tSQLt.Private_SetConfiguration @Name = 'Verbose', @Value = @Verbose;
END;
