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
        private const string PrizesFile = "PrizeModel.csv";

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
