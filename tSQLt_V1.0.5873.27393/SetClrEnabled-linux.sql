EXEC sp_configure 'show advanced options', 1
RECONFIGURE
GO

EXEC sp_configure 'clr enabled', 1
EXEC sp_configure 'clr strict security', 0
RECONFIGURE
GO