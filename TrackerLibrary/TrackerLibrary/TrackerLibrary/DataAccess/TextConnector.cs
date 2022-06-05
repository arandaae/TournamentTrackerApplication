using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        // The value inside of PrizesFile will always stay the same
        // Prizes in PrizesFile will be uppercase because it is const
        private const string PrizesFile = "PrizeModels.csv";
        private const string PeopleFile = "PersonModels.csv";

        public PersonModel CreatePerson(PersonModel model)
        {
            // people is a list of PersonModel the file name PersonModels.csv convert it to FullFilePath using App.config
            // Loads the file to make sure it exists, if it doesn't exist it returns back an empty list
            // Takes the list if it is an empty list and converts it to a list of PersonModel
            // Otherwise we get of list of all of the people in our csv file
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

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
            people.SaveToPeopleFile(PeopleFile);

            return model;
        }

        // TODO - Wire up the CreatePrize for text files.
        public PrizeModel CreatePrize(PrizeModel model)
        {
            // Takes the PrizeFile, finds the FullFilePath, Loads the file, converts to PrizeModel List
            // List of PrizeModel from text file
            /// Load the text file and Convert the text to List<PrizeModel> 
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

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
            prizes.SaveToPrizeFile(PrizesFile);

            return model;
            
        }
    }
}
