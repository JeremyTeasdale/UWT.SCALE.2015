using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;

namespace BHS.UWT.BLL 
{
    class GenericExitPoint
    {
        public object Execute(object p1, object p2, object p3, object p4,
            object p5, object p6, object p7, object p8, object p9,
            object p10, object p11, object p12, object p13, object p14,
            object p15, object p16)
        {
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p1 = {0}", p1.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p2 = {0}", p2.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p3 = {0}", p3.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p4 = {0}", p4.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p5 = {0}", p5.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p6 = {0}", p6.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p7 = {0}", p7.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p8 = {0}", p8.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p9 = {0}", p9.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p10 = {0}", p10.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p11 = {0}", p11.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p12 = {0}", p12.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p13 = {0}", p13.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p14 = {0}", p14.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p15 = {0}", p15.ToString()));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.LotOverride: p16 = {0}", p16.ToString()));

            return null;
        }

        //public string SetManifestCompany(decimal shippingContainerId, bool SetCompany, string serializedSession)
        //{
        //    Session session = SessionMapper.ConvertFromLegacySession(serializedSession);
        //    try
        //    {
        //        Debug.WriteLine(string.Format("FFRManifestEP.SetManifestCompany: ShippingContainerId {0}, SetCompany {1}, Session{2}", shippingContainerId.ToString(), SetCompany.ToString(), serializedSession));
        //        //session = ;
        //        using(DataHelper dataHelper = new DataHelper(session))
        //        {
        //            IDataParameter[] parmarray = new IDataParameter[2];
        //            parmarray[0] = DataHelper.BuildParameter(session, "@InternalContainer", shippingContainerId);
        //            parmarray[1] = DataHelper.BuildParameter(session, "@SetCompany", SetCompany == true? "1":"0");
        //            Debug.WriteLine(string.Format("FFRManifestEP.SetManifestCompany: Befor SP Execute InternalContainer {0}, SetCompany {1}", parmarray[0], parmarray[1]));
        //            DataTable table = dataHelper.upGetTable(CommandType.StoredProcedure, "BHS_ShipHeaderContainer_SetCompany", parmarray);
        //            if (table.Rows != null && table.Rows.Count > 0)
        //            {
        //                Debug.WriteLine("Table Found");
        //                Debug.WriteLine(string.Format("Company = {0}", DataManager.GetString(table.Rows[0], "Company")));
        //                return DataManager.GetString(table.Rows[0], "Company");
        //            }
        //            else
        //                return null;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        ExceptionManager.LogException(session, ex);
        //        Debug.WriteLine(ex.ToString());
        //        return null;
        //    }
        //}
    }
}
