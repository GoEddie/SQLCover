CREATE procedure security.login_user (@user_name nvarchar(50), @hashed_password nvarchar(255))
	as
	
		if not exists(select * from dbo.users where user_name = @user_name and password = @hashed_password)
		begin
			RAISERROR ('username or password not found', 16, 1)
		end
	
		if not exists(select * from dbo.users where user_name = @user_name and password = @hashed_password and enabled = 1)
		begin
			raiserror( 'user is not enabled', 16, 1);	
		end
	
		exec add_audit_login @user_name;
	
		select user_id from dbo.users where user_name = @user_name and password = @hashed_password and enabled = 1
