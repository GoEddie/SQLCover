CREATE TABLE [Accelerator].[Particle] (
    [Id]      INT             IDENTITY (1, 1) NOT NULL,
    [X]       DECIMAL (10, 2) NOT NULL,
    [Y]       DECIMAL (10, 2) NOT NULL,
    [Value]   NVARCHAR (MAX)  NOT NULL,
    [ColorId] INT             NOT NULL,
    CONSTRAINT [PK_Point_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ParticleColor] FOREIGN KEY ([ColorId]) REFERENCES [Accelerator].[Color] ([Id])
);

