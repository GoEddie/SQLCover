CREATE TABLE [Accelerator].[Color] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [ColorName] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Color_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

