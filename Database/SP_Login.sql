USE [AspNetAuth]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE SP_Login
(
	@username nvarchar(100),
	@password nvarchar(max)
)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @CurrentDate DateTime;
	DECLARE @userID INT;
	DECLARE @LoginID INT;

	Set @CurrentDate = DATEADD(HOUR, 8, GETDATE());

	IF EXISTS(SELECT TOP 1 1 from Users WITH(NOLOCK) WHERE Name = @username AND Password = @password)
	BEGIN
		SET @userID = (SELECT TOP 1 [UserId] from Users WITH(NOLOCK) WHERE Name = @username AND Password = @password);
		IF NOT EXISTS(SELECT TOP 1 1 from [UserLogin] WITH(NOLOCK) WHERE UserID = @userID)
		BEGIN
			INSERT INTO [UserLogin]([Status], [LoginDate], [UserId]) 
			VALUES(1, @CurrentDate, @userID);
			SET @LoginID = SCOPE_IDENTITY();
			UPDATE [Users] SET LoginId = @LoginID Where UserId = @userID
		END
		ELSE
		BEGIN
			UPDATE [UserLogin] SET [Status] = 1, [LoginDate] = @CurrentDate WHERE [UserId] = @userID
		END
	END

	SELECT * FROM Users WITH(NOLOCK) WHERE [UserId] = @userID
END

GO