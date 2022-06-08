using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerUI
{
    // This method will allow multiple forms to call call the PrizeModel without calling the entire CreatePrizeForm
    // Whoever implements this contract, will have one method that takes in a PrizeModel but returns nothing
    public interface IPrizeRequester
    {
        void PrizeComplete(PrizeModel model);
    }
}
