using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manh.WMFW.Entities;
using System.Diagnostics;
using Manh.WMW.General;
using Manh.WMFW.General;
using Manh.WMFW.DataAccess;
using System.Data;
using com.pronto.bl.outex;
using Manh.ILS.NHibernate.Entities;

namespace BHS.UWT.BLL
{
    public class CloseContainerUIEP : IWorkFlowStep
    {
        #region IWorkFlowStep Members

        public object ExecuteStep(Session session, params object[] parameters)
        {
            Debug.WriteLine("BHS.UWT.ExitPoints.CloseContainerUIEP: Start. V1.0.0.20");

            ShippingContainer be = parameters[0] as ShippingContainer;

            Debug.WriteLine("BHS.UWT.ExitPoints.CloseContainerUIEP, Data:" + be.ContainerId.ToString() );

            try { 
           Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.CloseContainerUIEP: Internal Container Num = {0}",be.InternalContainerNum ));
                }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception);
                Debug.WriteLine("BHS.UWT.ExitPoints.CloseContainerUIEP: " + exception.ToString());
                return "MSG_UWTSHIPPING01";
            }
            
            var allowcp = AllowClosePallet(session, be.InternalContainerNum);

            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.CloseContainerUIEP: Allow Close Pallet = {0}", allowcp));

            Object allow = allowcp != "1" ? allowcp : null;

            Debug.WriteLine("CloseContainerEP.ExecuteStep: End");

            return allow;
        }

        private string AllowClosePallet(Session session, decimal internalContainerNum)
        {
            string allow = null;
            try
            {
                using (DataHelper helper = new DataHelper(session))
                {
                    IDataParameter[] parameterArray = new IDataParameter[] { DataHelper.BuildParameter(session, "@InternalContainerNum", internalContainerNum)};
                    DataTable table = helper.GetTable(CommandType.StoredProcedure, "BHS_ShippingContainer_AllowClosePallet", parameterArray);
                    if ((table != null) && (table.Rows.Count > 0))
                    {
                        var result = DataManager.GetString(table.Rows[0], "Result");
                        Debug.WriteLine(string.Format("Result: = {0}", result));

                        var dm = result;

                        Debug.WriteLine(string.Format("DM: = {0}", dm));

                        return dm;
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception);
                Debug.WriteLine(exception.ToString());
            }
            return allow;
        }

        #endregion
    }
}
