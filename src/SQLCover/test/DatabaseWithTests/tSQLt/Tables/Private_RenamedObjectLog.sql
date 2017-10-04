CREATE TABLE [tSQLt].[Private_RenamedObjectLog] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [ObjectId]     INT            NOT NULL,
    [OriginalName] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK__Private_RenamedObjectLog__Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

