CREATE TABLE [dbo].[schedule] (
    [schedule_id] INT  IDENTITY (1, 1) NOT NULL,
    [date]        DATE NOT NULL,
    [approved]    INT  DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([schedule_id] ASC)
);

