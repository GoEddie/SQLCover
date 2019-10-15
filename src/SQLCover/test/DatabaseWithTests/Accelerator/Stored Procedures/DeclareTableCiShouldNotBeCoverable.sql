CREATE PROCEDURE [dbo].[DeclareTableCiShouldNotBeCoverable]
	@param1 int = 0,
	@param2 int
AS
	SELECT @param1, @param2


	DECLARE @T TABLE(A INT INDEX SS CLUSTERED, abc INT, RRR INT)
	INSERT INTO @T
	SELECT 1, 1, 1;

RETURN 0
