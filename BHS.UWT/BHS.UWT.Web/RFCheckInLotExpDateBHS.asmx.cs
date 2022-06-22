using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Script.Services;
using Manh.WMFW.DataAccess;
using Manh.WMFW.General;
using System.Diagnostics;

namespace BHS.UWT.WEB
{
    /// <summary>
    /// Summary description for LotConfirmOrderPickingBHS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class RFCheckInLotExpDateBHS : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public string GetPreviousLotExpDate(string internalReceiptNum, string lot)
        {
            // spin up new ils session
            Session session = null;

            // LOT|EXPDATE

            var lotExpDate = "";

            /*

            try
            {
                session = new Session();

                using (DataHelper dataHelper = new DataHelper(session))
                {
                    IDataParameter[] paramArray = new IDataParameter[2];
                    paramArray[0] = dataHelper.BuildParameter("@internal_receipt_num", internalReceiptNum);
                    paramArray[1] = dataHelper.BuildParameter("@lot", lot);

                    //DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_ShippingContainer_GetPreviousLotExpDate", paramArray);

                    //if(table != null && table.Rows.Count > 0)
                    //{
                    //}
                }
            }
            catch(Exception ex)
            {
                ExceptionManager.LogException(session, ex);
                Debug.WriteLine(ex.ToString());
            }
             * 
             * */

            return "AJH";
        }
    }
}
