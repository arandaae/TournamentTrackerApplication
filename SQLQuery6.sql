USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spTeamMembers_GetByTeam]    Script Date: 6/8/2022 1:35:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Gets the people on a given team
ALTER PROCEDURE [dbo].[spTeamMembers_GetByTeam]
	@TeamId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  SELECT p.*
  FROM dbo.TeamMembers m
  INNER JOIN dbo.People p on m.PersonId = p.id
  WHERE m.TeamId = @TeamId;

END

