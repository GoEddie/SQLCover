create procedure [dbo].[a_procedure]
as

	if 1 = 2
	begin
		select 100;
	end

	if 1 = 1
	begin
		select 200;
	end

	begin
		begin
			begin
				select 500;
			end
				select 600;
		end
	end

