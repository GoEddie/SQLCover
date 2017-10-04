
		create procedure add_audit_login(@user_name nvarchar(max))
		as
			print 'audited user login'