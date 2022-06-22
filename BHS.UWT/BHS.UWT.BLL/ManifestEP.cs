using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Manh.WMFW.Entities;
using Manh.WMFW.General;
using Manh.WMFW.DataAccess;
using System.Data;

namespace BHS.UWT.BLL
{
    public class ManifestEP
    {
        public object ExecuteManifestEPBefore(string SerializedSession, ShippingContainerBE shippingContainerBE, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16)
        {
            Debug.WriteLine(string.Format("ManifestEP.ExecuteManifestEPBefore: Start: Interal Container Num: {0}", shippingContainerBE.InternalContainerNum));
            shippingContainerBE.UserDef5 = SetCustomerPO(shippingContainerBE.InternalContainerNum, SerializedSession);
            Debug.WriteLine("ManifestEP.ExecuteManifestEPBefore: End");
            return null;
        }

        public string SetCustomerPO(decimal internalContainerNum, string serializedSession)
        {
            string str;
            Session session = null;
            try
            {
                session = SessionMapper.ConvertFromLegacySession(serializedSession);

                Debug.WriteLine(string.Format("ManifestEP.SetCustomerPO: InternalContainerNum {0}, Session {1}", internalContainerNum, serializedSession));
                using (DataHelper helper = new DataHelper(session))
                {
                    IDataParameter[] parameterArray = new IDataParameter[] { DataHelper.BuildParameter(session, "@InternalContainerNum", internalContainerNum) };
                    DataTable table = helper.GetTable(CommandType.StoredProcedure, "BHS_ShippingContainer_RetrieveUpdateCustomerPO", parameterArray);
                    if ((table != null) && (table.Rows.Count > 0))
                    {
                        Debug.WriteLine(table.Rows[0]);
                        Debug.WriteLine(string.Format("Customer PO = {0}", DataManager.GetString(table.Rows[0], "MinCustomerPO")));
                        return DataManager.GetString(table.Rows[0], "MinCustomerPO");
                    }
                    str = null;
                }
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception);
                Debug.WriteLine(exception.ToString());
                str = null;
            }
            return str;
        }
    }
}
