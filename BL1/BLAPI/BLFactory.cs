using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLAPI
{
    /// <summary>
    /// factory 
    /// </summary>
    public static class BLFactory
    {
      //return instance of Bl not carryng implementation
        public static IBL GetBL()
        {
            return BL.BLImp.Instance; 
        }

       
    }
}

