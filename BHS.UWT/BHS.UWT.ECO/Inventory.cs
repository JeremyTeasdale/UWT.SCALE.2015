using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

using Manh.WMFW.General;
using Manh.WMFW.DataAccess;
using Manh.ILS.DAO.Interfaces;
using Manh.ILS.NHibernate.Entities;
using Manh.ILS.General;
using Manh.WMFW.Config.BL;
using Manh.WMW.Configs.General;

using BHS.ProcessService;

using System.Data;
using Manh.WMW.General;
using System.IO;
using System.Xml.Linq;

namespace BHS.UWT.ECO
{
    class Inventory : ServiceProcess
    {
        public Inventory(Dictionary<string, string> Params) : base(Params)
        {
        }

        public string Message { get; set; }

        private string EcoDocsDir { get; set; }

        public async override void Execute()
        {
            Executing = true;
            DateTime startTime = DateTime.Now;
            Utilities.WriteDebug(string.Format("ECO Inventory UWT Debug, {0}", "test"));

            try
            {
                await CreateChangedInventory();
            


                Executing = false;
            }
            catch (Exception ex)
            {
                Utilities.WriteDebug(Utilities.FormatException(ex));
                Utilities.WriteException(ex, this.GetType().FullName);
            }
            finally
            {
                Executing = false;
                Utilities.WriteDebug("DONE EXECUTING");
            }

            DateTime endTime = DateTime.Now;
            Console.WriteLine(endTime.Subtract(startTime).TotalMinutes);
        }

        private async Task CreateChangedInventory()
        {
            DataTable inventoryNumbers = ECOTransHelper.GetHeaderData("BHS_ECO_GetInventoryData", "OnHandInventoryRow");
            if (DataManager.IsEmpty(inventoryNumbers))
            {
                return;
            }

            Tuple<string, string> urlAndxFunctionsKey = Utilities.GetUrlAndxFunctionsKey("INVENTORY");

            List<ECOTransaction> ecoInventory = ECOTransHelper.BuildECOInventoryTransactions(inventoryNumbers);

            //List<Task<string>> ecoTasks = new List<Task<string>>();

            StringBuilder XMLcontent = new StringBuilder("<OnHandInventory>");

            foreach (ECOTransaction ecoTran in ecoInventory)
            {
                XMLcontent.Append("\n" + ecoTran.XmlContent);  
            }

            XMLcontent.Append("\n </OnHandInventory>");

            ECOTransaction finalECOTransaction = new ECOTransaction();
            finalECOTransaction.Url = urlAndxFunctionsKey.Item1;
            finalECOTransaction.xFunctionsKey = urlAndxFunctionsKey.Item2;
            finalECOTransaction.XmlContent = XMLcontent.ToString();
            finalECOTransaction.Operation = "Inventory";



            string response = await ECOTransHelper.SendInventoryXmlToECO(finalECOTransaction);

            //string response = await ECOTransHelper.SendInventoryXmlToECO(ecoTran);
            //string[] completedTasks = await Task.WhenAll(ecoTasks);
        }

     

       

       
    }
}
