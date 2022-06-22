using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using System.Diagnostics;

using Manh.WMFW.General;
using Manh.WMFW.Config.BL;

namespace BHS.UWT.Web
{
    /// <summary>
    /// Summary description for BHSLotValidationRequired
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class BHSLotValidationRequired : System.Web.Services.WebService
    {

        private Session _session;
        public Session session
        {
            get
            {
                if (_session == null)
                    _session = new Session();
                return _session;
            }
            set
            {
                _session = value;
            }
        }

        [WebMethod]
        public bool LastWorkInstructionRequiresCountBack(int internalInstructionNum)
        {
            try
            {
                Debug.WriteLine("BHS.UWT.Web.LastWorkInstructionRequiresCountBack");
                return WorkInstruction.WorkInstrRequiresValidation(internalInstructionNum, session);
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception, string.Format("InternalInstructionNum: {0}", internalInstructionNum));
                Debug.WriteLine(exception.ToString());
                return false;
            }
        }
    }
}
