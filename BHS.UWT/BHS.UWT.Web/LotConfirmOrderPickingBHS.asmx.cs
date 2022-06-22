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
    public class LotConfirmOrderPickingBHS : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public bool RequireLotValidation(string internalInstructionNum)
        {
            bool requireLotValidation = false;

            // spin up new ils session
            Session session = null;

            try
            {
                session = new Session();

                using (DataHelper dataHelper = new DataHelper(session))
                {
                    IDataParameter[] paramArray = new IDataParameter[1];
                    paramArray[0] = dataHelper.BuildParameter("@internal_instruction_num", internalInstructionNum);

                    DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_CCLotMod_RequireLotValidation", paramArray);
                    // if we get data back, requires lot validation
                    requireLotValidation = table != null && table.Rows.Count > 0;
                }
            }
            catch(Exception ex)
            {
                ExceptionManager.LogException(session, ex);
                Debug.WriteLine(ex.ToString());
            }

            return requireLotValidation;
        }
    }
}
