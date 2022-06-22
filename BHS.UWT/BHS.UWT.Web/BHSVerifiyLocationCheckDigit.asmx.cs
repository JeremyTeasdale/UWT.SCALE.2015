using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Manh.WMFW.DataAccess;
using Manh.WMFW.General;
using System.Diagnostics;
using System.Data;
using com.pronto.general;
using Manh.WMW.Configs.General;
using System.Threading;

namespace BHS.UWT.Web
{
    /// <summary>
    /// Summary description for Test1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class BHSVerifiyLocationCheckDigit : System.Web.Services.WebService
    {
        [WebMethod]
        public bool IsValid(string userName, string environment, string doc, string checkDigit)
        {

            var session = new SessionFactory().Create();

            var cd = string.Empty;

            var verificationMethod = "Check Digit";
            string chkdgt = "Check Digit";
            Debug.WriteLine("BHSCHECKDIGIT Start." );
            try
            {
                using (DataHelper dataHelper = new DataHelper(session))
                {
                    IDataParameter[] paramArray = new IDataParameter[2];
                    paramArray[0] = dataHelper.BuildParameter("@Location", doc);
                    paramArray[1] = dataHelper.BuildParameter("@User_Name", userName);

                    DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_IMMD_DOCK_TNSF_CHECKDGTVERF", paramArray);  
                    if (table != null && table.Rows.Count > 0)
                    {
                        verificationMethod = table.Rows[0]["VERIFICATION_METH"].ToString();
                        cd = table.Rows[0]["CHECK_DIG"].ToString();
                    }
                    else
                    {
                        Debug.WriteLine("No Results for Location:  " + doc);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.LogException( session, ex, string.Format(" BHSCHECKDIGIT Failed to Retreive Location Check Digit information."));
                Debug.WriteLine("BHSCHECKDIGIT Failed to Retreive Location Check Digit information." + ex.ToString());
            }

            // if we the location does not have check digit assigned or the check passed is valid, return true

            var isValid = false;

            isValid = verificationMethod != chkdgt || (verificationMethod == Constants.stCHECKDIG && checkDigit == cd) ? true : false;

            return isValid;

        }
    }
}

