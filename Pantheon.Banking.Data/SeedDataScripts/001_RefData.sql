USE [PantheonBanking]
GO

-- ACCOUNT TYPE
IF NOT EXISTS (SELECT 1 FROM [dbo].[AccountType] WHERE [Name]=N'Current')
BEGIN
	INSERT INTO [dbo].[AccountType]([IsActive],[Created],[Name]) VALUES(1, getdate(), N'Current')
END

-- TRANSACTION TYPE
IF NOT EXISTS (SELECT 1 FROM [dbo].[TransactionType] WHERE [Name]=N'Credit')
BEGIN
	INSERT INTO [dbo].[TransactionType]([IsActive],[Created],[Name]) VALUES(1, getdate(), N'Credit')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[TransactionType] WHERE [Name]=N'Debit')
BEGIN
	INSERT INTO [dbo].[TransactionType]([IsActive],[Created],[Name]) VALUES(1, getdate(), N'Debit')
END

-- CURRENCY
IF NOT EXISTS (SELECT 1 FROM [dbo].[Currency] WHERE [Name]=N'British Pound Sterling')
BEGIN
	INSERT INTO [dbo].[Currency]([IsActive],[Created],[Name],[Symbol]) VALUES(1, getdate(), N'British Pound Sterling', N'GBP')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Currency] WHERE [Name]=N'US Dollar')
BEGIN
	INSERT INTO [dbo].[Currency]([IsActive],[Created],[Name],[Symbol]) VALUES(1, getdate(), N'US Dollar', N'USD')
END
