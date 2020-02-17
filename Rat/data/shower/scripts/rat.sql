-- Create a new database called 'rat'
-- Connect to the 'master' database to run this snippet
USE master
GO

-- Create the new database if it does not exist already
IF EXISTS (SELECT [name] FROM sys.databases WHERE [name] = N'Rat')
BEGIN
	ALTER DATABASE Rat SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE Rat
END

CREATE DATABASE Rat
GO

USE Rat

CREATE TABLE [dbo].[Configuration]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[Key] NVARCHAR(254) NOT NULL,
	[Value] NVARCHAR(500) NOT NULL,

	CONSTRAINT [PK_Configuration_Id] PRIMARY KEY ([Id] ASC),
	CONSTRAINT [UQ_Configuration_Key] UNIQUE ([Key])
);

GO

CREATE NONCLUSTERED INDEX [UX_Rat_Key] ON [dbo].[Configuration] ([Key]);

GO

-- Insert rows into table 'Configuration' in schema '[dbo]'
INSERT INTO [dbo].[Configuration] ([Key], [Value])
VALUES
	('A1', 'Hellow SQL'),
	('A2', 'Jello SQL'),
	('B1', 'Hallo SQL'),
	('C1', 'Hi SQL Value')
GO