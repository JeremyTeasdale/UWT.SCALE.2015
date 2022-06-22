using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;
using System.Data;

using Manh.WMW.General;
using Manh.WMFW.Entities;
using Manh.WMFW.General;
using Manh.WMFW.DataAccess;

namespace BHS.UWT.BLL
{
    public class ReplenishmentLotOverride : IWorkFlowStep
    {
        //public object Execute(string serializedSession, string workUnit, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16)
        public object ExecuteStep(Session session, params object[] parameters)
        {
            string workUnit = parameters[0].ToString();

            //Session session = SessionMapper.ConvertFromLegacySession(serializedSession);
            try
            {
                Debug.WriteLine(string.Format("BHS.UWT.BLL.ExitPoints.ReplenishmentLotOverride: Session = {0}, workUnit = {1}", session, workUnit));
                using (DataHelper dataHelper = new DataHelper(session))
                {
                    IDataParameter[] parmarray = new IDataParameter[1];
                    parmarray[0] = DataHelper.BuildParameter(session, "@work_unit", workUnit);
                    dataHelper.Update(CommandType.StoredProcedure, "BHS_EP_ReplenishmentLotOverride", parmarray);
                    
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.LogException(session, ex);
                ExceptionManager.LogException(session, ex);
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
