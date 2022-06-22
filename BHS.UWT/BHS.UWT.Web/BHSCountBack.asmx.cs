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
    /// Summary description for BHSCountBack
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class BHSCountBack : System.Web.Services.WebService
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
                return WorkInstruction.GetWorkInstruction(internalInstructionNum, session).requiresCountBack;
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception, string.Format("InternalInstructionNum: {0}", internalInstructionNum));
                Debug.WriteLine(exception.ToString());
                return false;
            }
        }

        [WebMethod]
        public bool CurrentWorkInstructionRequiresCountBack(int internalInstructionNum)
        {
            try
            {
                Debug.WriteLine("BHS.UWT.Web.LastWorkInstructionRequiresCountBack");
                return WorkInstruction.GetPrePickWorkInstruction(internalInstructionNum, session).requiresCountBack;
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception, string.Format("InternalInstructionNum: {0}", internalInstructionNum));
                Debug.WriteLine(exception.ToString());
                return false;
            }
        }

        [WebMethod]
        public WorkInstruction GetWorkInstructionDetails(int internalInstructionNum)
        {
            try
            {
                Debug.WriteLine("BHS.UWT.Web.GetWorkInstructionDetails");
                return WorkInstruction.GetWorkInstruction(internalInstructionNum, session);
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception, string.Format("InternalInstructionNum: {0}", internalInstructionNum));
                Debug.WriteLine(exception.ToString());
                return null;
            }
        }
        
        [WebMethod]
        public List<Uom> RetrieveUOM(int internalInstructionNum)
        {
            try
            {
                Debug.WriteLine("BHS.UWT.Web.RetrieveUOM");
                return Uom.GetUomList(internalInstructionNum, session);
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception, string.Format("InternalInstructionNum: {0}", internalInstructionNum));
                Debug.WriteLine(exception.ToString());
                return null;
            }
        }

        [WebMethod]
        public void SubmitCountBackQty(int internalInstructionNum, int countedQty)
        {
            try
            {
                Debug.WriteLine("BHS.UWT.Web.SubmitCountBackQty");
                WorkInstruction.SubmitCountBackQty(internalInstructionNum, countedQty, session);
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception, string.Format("InternalInstructionNum: {0}", internalInstructionNum));
                Debug.WriteLine(exception.ToString());
            }
        }

        [WebMethod]
        public string GetContinuePassword()
        {
            try
            {
                return SystemConfigRetrieval.GetStringSystemValue(session, "BhsCountBackContinuePass", "Cycle Count");
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception);
                Debug.WriteLine(exception.ToString());  
                return null;
            }
        }

        [WebMethod(EnableSession = true)]
        public void AddSessionValues(int internalInstructionNum)
        {
            try
            {
                Debug.WriteLine("BHS.UWT.Web.AddSessionValues");
                RemoveSessionValues();
                this.Session.Add("LastInternalInstructionNum", internalInstructionNum);
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception, string.Format("InternalInstructionNum: {0}", internalInstructionNum));
                Debug.WriteLine(exception.ToString());
            }
        }

        [WebMethod(EnableSession = true)]
        public void RemoveSessionValues()
        {
            try
            {
                Debug.WriteLine("BHS.UWT.Web.RemoveSessionValues");
                if (Session["LastInternalInstructionNum"] != null)
                    this.Session.Remove("LastInternalInstructionNum");
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception);
                Debug.WriteLine(exception.ToString());
            }
        }

        [WebMethod]
        public void AuditLog(string WebService, string ErrorMessage, string OriginalParms)
        {
            
            ExceptionManager.LogException(session, ErrorMessage, "BHSCountBack exception", WebService, OriginalParms);
        }
    }
}
