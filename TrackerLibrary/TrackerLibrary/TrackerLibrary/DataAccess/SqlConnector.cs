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
        public PersonModel CreatePerson(PersonModel model)
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

                return model;
            }
        }

        //TODO - Make the CreatePrize method actually save to the database
        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
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

                return model;
            }
        }

        public TeamModel CreateTeam(TeamModel model)
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

                return model;
            }
        }

        /// <summary>
        /// Create Tournament Entry
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                SaveTournament(connection, model);

                SaveTournamentPrizes(connection, model);

                SaveTournamentEntries(connection, model);
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

                connection.Execute("dbo.TournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
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
    }
}
