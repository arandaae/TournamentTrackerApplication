using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public interface IDataConnection
    {
        // Use Create for insert methods and Get for Select statements
        PrizeModel CreatePrize(PrizeModel model);
        PersonModel CreatePerson(PersonModel model);
        TeamModel CreateTeam(TeamModel model);
        List<TeamModel> GetTeam_All();
        List<PersonModel> GetPerson_All(); // Don't have to pass anything in because it is getting all of the people

    }
}
