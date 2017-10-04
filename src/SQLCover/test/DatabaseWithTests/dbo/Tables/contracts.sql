CREATE TABLE [dbo].[contracts] (
    [contract_id]          INT          IDENTITY (1, 1) NOT NULL,
    [contract_description] VARCHAR (25) NOT NULL,
    PRIMARY KEY CLUSTERED ([contract_id] ASC)
);

