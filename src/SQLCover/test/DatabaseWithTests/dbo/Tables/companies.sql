CREATE TABLE [dbo].[companies] (
    [company_id]         INT            IDENTITY (1, 1) NOT NULL,
    [company_name]       NVARCHAR (255) NOT NULL,
    [primary_address_id] INT            NOT NULL
);

