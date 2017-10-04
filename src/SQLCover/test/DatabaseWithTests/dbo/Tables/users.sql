CREATE TABLE [dbo].[users] (
    [user_id]                 INT            IDENTITY (1, 1) NOT NULL,
    [user_name]               NVARCHAR (255) NOT NULL,
    [password]                NVARCHAR (255) NOT NULL,
    [company_id]              INT            NOT NULL,
    [user_type]               INT            NOT NULL,
    [pay_currency_id]         INT            NOT NULL,
    [home_office_location_id] INT            NOT NULL,
    [contract_type_id]        INT            NOT NULL,
    [parking_space_allocated] INT            NOT NULL,
    [holiday_schedule_id]     INT            NOT NULL,
    [enabled]                 INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([user_id] ASC)
);

