using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using com.pronto.general;
//using Manh.ILS.General.RF.Interfaces;
using Manh.ILS.General.Interfaces;
using Manh.ILS.General;
using Manh.WMFW.Config.BL;
using Manh.WMW.General;
using Manh.WMFW.General;

namespace BHS.UWT.WEB
{
    public partial class MANHRF : System.Web.UI.MasterPage
   { 
        public string _bhsCss;
        //public string BhsCss
        //{
        //    get
        //    {

        //        if (string.IsNullOrEmpty(_bhsCss))
        //        {
        //            try
        //            {
        //                _bhsCss = ((IRFRetrieval)SpringNetFactory.GetObject("IRFRetrieval")).GetRFStyleSheet();
        //            }
        //            catch(Exception ex)
        //            {
        //                _bhsCss = "RFStylesMedium.css";
        //            }
        //        }
        //        return _bhsCss;
        //    }
        //}
            
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (ThreadManager.CurrentSession == null)
                    ThreadManager.CurrentSession = SessionMapper.ConvertFromLegacySession(RFCache.rtrvRFSession(RFSupport.getString(Session, "user"), RFSupport.getString(Session, "ENVIRONMENT")));

                //if (Session["BHSCSS"] == null || string.IsNullOrEmpty(Session["BHSCSS"].ToString()))
                //    Session["BHSCSS"] = BhsCss;
            }
            catch (Exception ex)
            {
                Response.Redirect(string.Format("SignonMenuRF.aspx?msg={0}", "MSG_WORK59"));
            }
        }
    }
}
