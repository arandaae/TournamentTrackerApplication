
-- =============================================
-- Author:		<Armando Aranda>
-- Create date: <Create Date,,>
-- Description:	<Inserts Team Members to database>
-- =============================================
CREATE PROCEDURE dbo.spTeamMembers_Insert
	-- Add the parameters for the stored procedure here
	@TeamId int,
	@PersonId int,
	@id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.TeamMembers(TeamId, PersonId)
	VALUES (@TeamId, @PersonId);

	SELECT @id = SCOPE_IDENTITY();
END
GO
