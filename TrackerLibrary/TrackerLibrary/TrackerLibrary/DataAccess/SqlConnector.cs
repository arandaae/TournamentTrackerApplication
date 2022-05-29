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
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("TrackerUI.Properties.Settings.TrackerUIConn")))
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
    }
}
