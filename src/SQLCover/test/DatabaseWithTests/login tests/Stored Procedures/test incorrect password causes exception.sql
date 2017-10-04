
CREATE procedure [login tests].[test incorrect password causes exception]
as
	exec tSQLt.ExpectException 'username or password not found'
	exec tSQLt.FakeTable 'dbo.users'

	insert into dbo.users(user_name, password, [enabled])
	select 'user_name', 'password', 1

	exec security.login_user 'user_name', 'some other password'
