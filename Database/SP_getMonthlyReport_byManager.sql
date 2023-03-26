Use AspNetAuth

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE SP_getMonthlyReport_byManager
(
	@Year VARCHAR(4),
	@Month VARCHAR(20),
	@ManagerId INT
)
AS
BEGIN

	DECLARE @getMonth INT;

	SET @getMonth = (SELECT DATEPART (MM,  Convert(DateTime, CONCAT(@Month ,' 01 ', @Year))));

	SELECT Cast(SUM(s.Amount)AS DECIMAL(10, 2)), u.Name
	FROM Sales AS s WITH(NOLOCK) 
	INNER JOIN Users AS u WITH(NOLOCK) ON s.UserId = u.UserId
	WHERE Month(SaleDate) = @getMonth AND YEAR(SaleDate) = @Year
	AND u.ReportManager = @ManagerId
	GROUP BY u.UserId, u.Name

END
GO
