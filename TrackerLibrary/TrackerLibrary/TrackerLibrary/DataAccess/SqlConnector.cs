using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using System.Configuration;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        // This const string db will replace the repeating connection string
        private const string db = "TrackerUI.Properties.Settings.TrackerUIConn";
        public void CreatePerson(PersonModel model)
        {
            // create new IDbconnection, fill connection with System.Data.SqlClient
            // using statement wraps the parameters so when it hits the end of the scope it destroys the connection properly to
            // prevent against memory leaks.
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@CellphoneNumber", model.CellphoneNumber);
                //id comes out of the database, uses name:value, the : allows for the parameter to be given a value type
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                //This will run a stored procedure and assume that nothing will come back as far as a data set
                //Excute says it will call something but not pass anything back
                //This will pass in all of this information into our database stored procedure
                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");
            }
        }

        //TODO - Make the CreatePrize method actually save to the database
        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public void CreatePrize(PrizeModel model)
        {
            // create new IDbconnection, fill connection with System.Data.SqlClient
            // using statement wraps the parameters so when it hits the end of the scope it destroys the connection properly to
            //prevent against memory leaks.
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceNumber);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                //id comes out of the database, uses name:value, the : allows for the parameter to be given a value type
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                //This will run a stored procedure and assume that nothing will come back as far as a data set
                //Excute says it will call something but not pass anything back
                //This will pass in all of this information into our database
                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");
            }
        }

        public void CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@TeamName", model.TeamName);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

                // Add foreach loop to grab all Team Members
                foreach (PersonModel tm in model.TeamMembers)
                {
                    p = new DynamicParameters(); // Remove var to allow DynamicParameters to overwrite p
                    p.Add("@TeamId", model.Id);
                    p.Add("@PersonId", tm.Id);

                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }
            }
        }

        /// <summary>
        /// Create Tournament Entry
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void CreateTournament(TournamentModel model)
        {
            // The using statement here will keep one connection to the database for each method is called
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                SaveTournament(connection, model);

                SaveTournamentPrizes(connection, model);

                SaveTournamentEntries(connection, model);

                SaveTournamentRounds(connection, model);

                TournamentLogic.UpdateTournamentResults(model); // Update the tournament to make sure any byes are moved into the next round
            }
        }

        private void SaveTournament(IDbConnection connection, TournamentModel model)
        {
            var p = new DynamicParameters();
            p.Add("@TournamentName", model.TournamentName);
            p.Add("@EntryFee", model.EntryFee);
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spTournaments_Insert", p, commandType: CommandType.StoredProcedure);

            model.Id = p.Get<int>("@id");
        }

        /// <summary>
        /// Create All of the prizes entries
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
        {
            // Add foreach loop to grab all Team Members
            foreach (PrizeModel pz in model.Prizes)
            {
                var p = new DynamicParameters(); 
                p.Add("@TournamentId", model.Id);
                p.Add("@PrizeId", pz.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Create all of team entries
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        private void SaveTournamentEntries(IDbConnection connection, TournamentModel model)
        {
            foreach (TeamModel tm in model.EnteredTeams)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@TeamId", tm.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);              
            }
        }

        private void SaveTournamentRounds(IDbConnection connection, TournamentModel model)
        {
            // List<List<MatchupModel>> Rounds
            // List<MatchupEntryModel> Entries
            // These have to be saved in order, this will allow the next round to have to id it needs

            // Loop through the rounds
            // Loop through the matchups
            // Save the matchup
            

            // Loop through the rounds
            foreach (List<MatchupModel> round in model.Rounds)
            {
                // Loop through the matchups
                foreach (MatchupModel matchup in round)
                {
                    // Save the matchup
                    var p = new DynamicParameters();
                    p.Add("@TournamentId", model.Id);
                    p.Add("@MatchupRound", matchup.MatchupRound);
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spMatchups_Insert", p, commandType: CommandType.StoredProcedure);

                    matchup.Id = p.Get<int>("@id");

                    foreach (MatchupEntryModel entry in matchup.Entries)
                    {
                        // Loop through the entries and save them
                        p = new DynamicParameters();
                        p.Add("@MatchupId", matchup.Id);

                        if (entry.ParentMatchup == null)
                        {
                            p.Add("@ParentMatchupId", null);
                        }
                        else
                        {
                            p.Add("@ParentMatchupId", entry.ParentMatchup.Id);
                        }

                        if (entry.TeamCompeting == null)
                        {
                            p.Add("@TeamCompetingId", null);
                        }
                        else
                        {
                            p.Add("@TeamCompetingId", entry.TeamCompeting.Id);
                        }
                        
                        p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("dbo.spMatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);
                    }

                }
            }
        }

        /// <summary>
        /// Returns all people from the database
        /// </summary>
        /// <returns></returns>
        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;

            // Connect to SQL database
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;

            // Connect to SQL database
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("dbo.spTeam_GetAll").ToList();

                foreach (TeamModel team in output)
                {
                    // Added DynamicParameter to fix exception that was thrown for not supplying parameters
                    var p = new DynamicParameters();
                    p.Add("@TeamId", team.Id);


                    team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                }
            }

            return output;
        }

        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> output;

            // Connect to SQL database
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TournamentModel>("dbo.spTournaments_GetAll").ToList();
                var p = new DynamicParameters();

                foreach (TournamentModel t in output)
                {
                    // Populate Prizes
                    p = new DynamicParameters();
                    p.Add("@TournamentId", t.Id);

                    t.Prizes = connection.Query<PrizeModel>("dbo.spPrizes_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    // Populate Teams
                    p = new DynamicParameters();
                    p.Add("@TournamentId", t.Id);

                    t.EnteredTeams = connection.Query<TeamModel>("dbo.spTeam_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    foreach (TeamModel team in t.EnteredTeams)
                    {
                        // Added DynamicParameter to fix exception that was thrown for not supplying parameters
                        p = new DynamicParameters();
                        p.Add("@TeamId", team.Id);

                        team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                    }

                    p = new DynamicParameters();
                    p.Add("@TournamentId", t.Id);

                    // Populate Rounds
                    List<MatchupModel> matchups = connection.Query<MatchupModel>("dbo.spMatchups_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    foreach (MatchupModel m in matchups)
                    {
                        p = new DynamicParameters();
                        p.Add("@MatchupId", m.Id);

                        // Populate Rounds
                        m.Entries = connection.Query<MatchupEntryModel>("dbo.spMatchupEntries_GetByMatchup", p, commandType: CommandType.StoredProcedure).ToList();

                        List<TeamModel> allTeams = GetTeam_All(); // All teams that are available

                        if (m.WinnerId > 0)
                        {
                            // Populate each matchup (1 model)
                            m.Winner = allTeams.Where(x => x.Id == m.WinnerId).First();
                        }

                        foreach (var me in m.Entries)    
                        {
                            // Populate each entry (2 models)
                            if (me.TeamCompetingId > 0)
                            {
                                me.TeamCompeting = allTeams.Where(x => x.Id == me.TeamCompetingId).First();
                            }

                            if (me.ParentMatchupId > 0)
                            {
                                me.ParentMatchup = matchups.Where(x => x.Id == me.ParentMatchupId).First();
                            }
                        }
                    }

                    // Rounds are a List<List<MatchupModel>>
                    // Keep this foreach seperate to not confuse with other MatchupModel
                    List<MatchupModel> currRow = new List<MatchupModel>();
                    int currRound = 1;

                    foreach (MatchupModel m in matchups)
                    {
                        if (m.MatchupRound > currRound) // If the round is changed
                        {
                            t.Rounds.Add(currRow);
                            currRow = new List<MatchupModel>();
                            currRound += 1; // Move to the next round
                        }

                        currRow.Add(m);
                    }

                    t.Rounds.Add(currRow);
                }              
            }

            return output;
        }

        public void UpdateMatchup(MatchupModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                // spMatchups_Update @id, @WinnerId
                var p = new DynamicParameters();
                if (model.Winner != null)
                {
                    p.Add("@id", model.Id);
                    p.Add("@WinnerId", model.Winner.Id);

                    connection.Execute("dbo.spMatchups_Update", p, commandType: CommandType.StoredProcedure); 
                }

                // spMatchupEntries_Update, id, TeamCompetingId, Score
                foreach (MatchupEntryModel me in model.Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        p = new DynamicParameters();
                        p.Add("@id", me.Id);
                        p.Add("@TeamCompetingId", me.TeamCompeting.Id);
                        p.Add("@Score", me.Score);

                        connection.Execute("dbo.spMatchupEntries_Update", p, commandType: CommandType.StoredProcedure); 
                    }
                }

            }
        }
    }
}
