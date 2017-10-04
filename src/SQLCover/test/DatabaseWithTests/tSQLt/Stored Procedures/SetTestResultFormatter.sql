


CREATE PROCEDURE tSQLt.SetTestResultFormatter
    @Formatter NVARCHAR(4000)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM sys.extended_properties WHERE [name] = N'tSQLt.ResultsFormatter')
    BEGIN
        EXEC sp_dropextendedproperty @name = N'tSQLt.ResultsFormatter',
                                    @level0type = 'SCHEMA',
                                    @level0name = 'tSQLt',
                                    @level1type = 'PROCEDURE',
                                    @level1name = 'Private_OutputTestResults';
    END;

    EXEC sp_addextendedproperty @name = N'tSQLt.ResultsFormatter', 
                                @value = @Formatter,
                                @level0type = 'SCHEMA',
                                @level0name = 'tSQLt',
                                @level1type = 'PROCEDURE',
                                @level1name = 'Private_OutputTestResults';
END;
