using System.Collections.Generic;
using System.Linq;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        public void CreatePerson(PersonModel model)
        {
            // people is a list of PersonModel the file name PersonModels.csv convert it to FullFilePath using App.config
            // Loads the file to make sure it exists, if it doesn't exist it returns back an empty list
            // Takes the list if it is an empty list and converts it to a list of PersonModel
            // Otherwise we get of list of all of the people in our csv file
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            // Find the max ID (using Linq query)
            int currentId = 1;

            if (people.Count > 0)
            {
                currentId = people.OrderByDescending(X => X.Id).First().Id + 1; //Gives an Id of 1 if there are no people
            }

            model.Id = currentId;

            //Add the new record with the new ID (max + 1)
            people.Add(model);

            // Save the list<string> to the text file
            people.SaveToPeopleFile();
        }

        // TODO - Wire up the CreatePrize for text files.
        public void CreatePrize(PrizeModel model)
        {
            // Takes the PrizeFile, finds the FullFilePath, Loads the file, converts to PrizeModel List
            // List of PrizeModel from text file
            /// Load the text file and Convert the text to List<PrizeModel> 
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // Takes the List and order by descending by the Id order, gives the first record that has the highest Id, then adds 1
            /// Find the max ID (using Linq query)
            int currentId = 1;

            if (prizes.Count > 0)
            {
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            /// Add the new record with the new ID (max + 1)
            prizes.Add(model);

            // Convert the prizes to list<string>
            // Save the list<string> to the text file
            prizes.SaveToPrizeFile();      
        }

        public List<PersonModel> GetPerson_All()
        {
            return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels();

            /// Find the max ID (using Linq query)
            int currentId = 1;

            if (teams.Count > 0)
            {
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            teams.Add(model);

            teams.SaveToTeamFile(); // Create SaveToTeamFile in TextConnectorProcessor.cs
        }

        public List<TeamModel> GetTeam_All()
        {
            return GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels();

        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();

            int currentId = 1;

            if (tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            model.SaveRoundsToFile();

            tournaments.Add(model);

            tournaments.SaveToTournamentFile();

            TournamentLogic.UpdateTournamentResults(model); // Update the tournament to make sure any byes are moved into the next round
        }

        public List<TournamentModel> GetTournament_All()
        {
            return GlobalConfig.TournamentFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();
        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchUpToFile();
        }
    }
}
