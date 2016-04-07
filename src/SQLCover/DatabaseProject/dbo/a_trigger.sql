create trigger [a_trigger]
	on a_table
	for delete, insert, update
	as
	begin
		set nocount on

		select 999;

		begin
			select 8;

		end
	end
