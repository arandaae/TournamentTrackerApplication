using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using TrackerLibrary.Models;

/// Each step will be a different action
/// * Load the text file
/// * Convert the text to List<PrizeModel>
/// Find the max ID (using Linq query)
/// Add the new record with the new ID (max + 1)
/// Convert the prizes to list<string>
/// Save the list<string> to the text file

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        //This method will take in the file name and return back the entire path
        //PrizeModels.csv
        // The \\ will translate into one slash. Normal \ means exit
        // Adding this to the parameters makes this an extension method
        public static string FullFilePath(this string fileName)
        {
            //F:\TournamentTrackerApplication\Data\PrizeModels.csv
            return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName }";
        }

        //This will take in the full file path and load that string
        //Load the text file
        public static List<string> LoadFile(this string file)
        {
            //File is in the System.IO namespace, Exists returns a boolean, this will check if the file Exists
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            //loop through every line in the text file, the string is initialized but has no records
            //if there is no file the a new list of PrizeModel will be created 
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);
                output.Add(p);
            }

            return output;
        }

        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in models)
            {
                lines.Add($"{ p.Id },{ p.PlaceNumber },{ p.PlaceName },{ p.PrizeAmount },{ p.PrizePercentage }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);

        }
    }
}
