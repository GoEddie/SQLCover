create procedure [dbo].[a_procedure_with_goto]
as

declare @i int = 0;

someLabel:

if @i = 0
begin
	set @i = 999;
	goto someLabel;
end

print 'done'

