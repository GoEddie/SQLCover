CREATE PROCEDURE [tSQLt].[ResultSetFilter]
@ResultsetNo INT NULL, @Command NVARCHAR (MAX) NULL
AS EXTERNAL NAME [tSQLtCLR].[tSQLtCLR.StoredProcedures].[ResultSetFilter]

