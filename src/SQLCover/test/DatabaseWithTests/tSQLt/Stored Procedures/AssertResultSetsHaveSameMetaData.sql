CREATE PROCEDURE [tSQLt].[AssertResultSetsHaveSameMetaData]
@expectedCommand NVARCHAR (MAX) NULL, @actualCommand NVARCHAR (MAX) NULL
AS EXTERNAL NAME [tSQLtCLR].[tSQLtCLR.StoredProcedures].[AssertResultSetsHaveSameMetaData]

