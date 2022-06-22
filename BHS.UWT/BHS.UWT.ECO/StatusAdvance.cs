using Manh.WMFW.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using BHS.ProcessService;
using System.Threading.Tasks;
using Manh.WMFW.General;
using Manh.WMW.General;

namespace BHS.UWT.ECO
{
    public class StatusAdvance : ServiceProcess
    {
        public StatusAdvance(Dictionary<string, string> Params) : base(Params)
        {
        }

        public async override void Execute()
        {
            Executing = true;
            Utilities.WriteDebug(string.Format("ECO StatusAdvance UWT Debug, {0}", "test"));

            try
            {
                await SendStatusAdvances();

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
        }

        public async Task SendStatusAdvances()
        {
            DataTable statusAdvances = ECOTransHelper.GetHeaderData("BHS_ECO_GetShipmentStatusAdvance", "StatusAdvance");
            if (DataManager.IsEmpty(statusAdvances))
            {
                return;
            }

             Tuple<string, string> urlAndxFunctionsKey = Utilities.GetUrlAndxFunctionsKey("STATUS_ADVANCE");

            List<ECOTransaction> ecoStatusAdvances = ECOTransHelper.BuildECOTransactions(statusAdvances, null);

            List<Task<string>> ecoTasks = new List<Task<string>>();

            foreach (ECOTransaction ecoTran in ecoStatusAdvances)
            {
                ecoTran.Url = urlAndxFunctionsKey.Item1;
                ecoTran.xFunctionsKey = urlAndxFunctionsKey.Item2;
                ecoTran.Operation = "StatusAdvance";

                //string response = await ECOTransHelper.SendXmlToECO(ecoTran);
                ecoTasks.Add(ECOTransHelper.SendXmlToECO(ecoTran));
            }

            string[] completedTasks = await Task.WhenAll(ecoTasks);
        }

        
    }
}
