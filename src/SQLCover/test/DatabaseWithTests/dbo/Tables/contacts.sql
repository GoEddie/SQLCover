CREATE TABLE [dbo].[contacts] (
    [contact_id]   INT           IDENTITY (1, 1) NOT NULL,
    [first_name]   NVARCHAR (50) NOT NULL,
    [last_name]    NVARCHAR (50) NOT NULL,
    [phone_number] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([contact_id] ASC)
);

