USE [PantheonBanking]
GO

-- ACCOUNT TEST DATA
IF NOT EXISTS (SELECT 1 FROM [dbo].[Account] WHERE [AccountNo]=N'22451190' AND [SortCode]=N'231120')
BEGIN
	INSERT INTO [dbo].[Account]([IsActive],[Created],[StartDate],[AccountName],[AccountNo],[SortCode], [AccountTypeId], [BaseCurrencyId]) 
	VALUES(1, getdate(), getdate(), N'Albert Jorge', N'22451190', N'231120', 1, 1)
END



