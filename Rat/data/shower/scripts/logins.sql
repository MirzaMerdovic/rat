USE master
GO
IF NOT EXISTS (
    SELECT [name]
        FROM sys.sql_logins
        WHERE [name] = N'rat_app'
)
    CREATE LOGIN rat_app WITH PASSWORD = 'rat_app', CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO