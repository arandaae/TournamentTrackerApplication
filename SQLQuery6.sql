USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spPeople_Insert]    Script Date: 6/3/2022 1:34:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ===========================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[spPeople_Insert]
	-- Add the parameters for the stored procedure here
	@FirstName nvarchar(100),
	@LastName nvarchar(100),
	@EmailAddress nvarchar(100),
	@CellphoneNumber varchar(20),
	@id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.People (FirstName, LastName, EmailAddress, CellphoneNumber)
	VALUES (@FirstName, @LastName, @EmailAddress, @CellphoneNumber);

	SELECT @id =SCOPE_IDENTITY();
END
