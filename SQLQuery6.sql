
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Armando Aranda>
-- Create date: <Create Date,,>
-- Description:	<Inserts Matchup Entries to the database>
-- =============================================
CREATE PROCEDURE dbo.spMatchupEntries_Insert
	-- Add the parameters for the stored procedure here
	@MatchupId int,
	@ParentMatchupId int,
	@TeamCompetingId int,
	@id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.MatchupEntries (MatchupId, ParentMatchupId, TeamCompetingId)
	VALUES (@MatchupId, @ParentMatchupId, @TeamCompetingId);

	SELECT @id = SCOPE_IDENTITY();
END
GO
