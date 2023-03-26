Use AspNetAuth

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE SP_getMonthlySales_FiscalYear
(
	@Year VARCHAR(4)
)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @TempMonth TABLE
	(
		NameMonth VARCHAR(20),
		ValueMonth VARCHAR(2),
		TotalSales VARCHAR(30)
	)

	;with months (date)
	AS
	(
		SELECT cast(@Year+'-01-01' as date)
		UNION ALL
		SELECT DATEADD(month,1,date)
		from months
		where  DATEADD(month,1,date)<= case when @Year=YEAR(getdate()) THEN CAST(getdate() as date) ELSE cast(@Year+'-12-31' as date) END
	)
	INSERT INTO @TempMonth(NameMonth, ValueMonth)
	select CONVERT(varchar, Datename(month,date)),
	MONTH(Datename(month,date) + FORMAT(GETDATE(), ' d, yyyy'))
	from months;

	WITH MonthlySales AS (
		SELECT  CAST(SUM(Amount) AS DECIMAL(10, 2)) AS SalesAmount, MONTH(SaleDate) AS SaleMonth
		FROM Sales WITH(NOLOCK)
		WHERE YEAR(SaleDate) = @Year
		GROUP BY MONTH(SaleDate)
	)
	UPDATE @TempMonth
	SET TotalSales = (CASE 
						WHEN (MonthlySales.SalesAmount IS NULL) THEN 'NA'
						ELSE MonthlySales.SalesAmount
					 END)
	FROM MonthlySales
	WHERE MonthlySales.SaleMonth = ValueMonth

	UPDATE @TempMonth SET TotalSales = 'NA' WHERE TotalSales IS NULL
	select NameMonth, TotalSales from @TempMonth
END;
GO
