CREATE TABLE [tSQLt].[TestResult] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Class]         NVARCHAR (MAX) NOT NULL,
    [TestCase]      NVARCHAR (MAX) NOT NULL,
    [Name]          AS             ((quotename([Class])+'.')+quotename([TestCase])),
    [TranName]      NVARCHAR (MAX) NOT NULL,
    [Result]        NVARCHAR (MAX) NULL,
    [Msg]           NVARCHAR (MAX) NULL,
    [TestStartTime] DATETIME       CONSTRAINT [DF:TestResult(TestStartTime)] DEFAULT (getdate()) NOT NULL,
    [TestEndTime]   DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

