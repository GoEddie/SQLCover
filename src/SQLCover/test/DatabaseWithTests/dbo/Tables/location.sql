CREATE TABLE [dbo].[location] (
    [location_id]        INT           IDENTITY (1, 1) NOT NULL,
    [address_line_1]     NVARCHAR (50) NOT NULL,
    [address_line_2]     NVARCHAR (50) NOT NULL,
    [address_line_3]     NVARCHAR (50) NOT NULL,
    [address_line_4]     NVARCHAR (50) NOT NULL,
    [address_line_5]     NVARCHAR (50) NOT NULL,
    [primary_contact_id] INT           NOT NULL
);

