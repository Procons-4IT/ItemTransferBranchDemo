using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemTransferBranchDemo
{
   public class AddonInfoInfo
    {
       public static bool InstallUDFs()
       {
           try
           {
               //Add The From Branch
               B1Helper.AddField("toWhs", "To Warehouse", "OIGE", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoYesNoEnum.tNO, false);
               B1Helper.AddField("fromWhs", "From Warehouse", "OIGE", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoYesNoEnum.tNO, false);
               B1Helper.AddField("toBranch", "To Branch", "OIGE", SAPbobsCOM.BoFieldTypes.db_Alpha, 50,SAPbobsCOM.BoYesNoEnum.tNO, false);

               B1Helper.AddField("isTransfered", "Transfered", "OIGE", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoYesNoEnum.tYES, SAPbobsCOM.BoFldSubTypes.st_None, false, "N", "P,Processed","NP,Not Processed","N,Not a Transfer");

           }
           catch (Exception ex)
           {
               return false;
           }
           return true;
            
       }

    }
}
