
-- =============================================
-- Author:		<Armando Aranda,,Name>
-- Create date: <05/28/2022,,>
-- Description:	<This will pass data from the application to the database ,,>
-- Changes made: Add any changes made here
-- =============================================
CREATE PROCEDURE dbo.spPrizes_Insert
	-- Add the parameters for the stored procedure here
	@PlaceNumber int,
	@PlaceName nvarchar(50),
	@PrizeAmount money,
	@PrizePercentage float,
	@id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements. Records modified
	-- will not be passed back
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	-- This will create a record into the prize table
	INSERT INTO dbo.Prizes (PlaceNumber, PlaceName,PrizeAmount,PrizePercentage)
	VALUES (@PlaceNumber, @PlaceName, @PrizeAmount, @PrizePercentage)

	--The last indentity written to the table will be put into the id variable
	SELECT @id =SCOPE_IDENTITY();
END
GO
