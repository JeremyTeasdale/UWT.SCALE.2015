using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using com.pronto.general;
using com.pronto.ds;

using Manh.WMW.General.General;
using Manh.WMFW.General;
using Manh.WMFW.DataAccess;
using Manh.WMW.Inventory;
using Manh.WMW.Inventory.General;
using Manh.ILS.Inventory.Interfaces;
using Manh.ILS.General;
using com.pronto.dbutility.process;


namespace BHS.UWT.WEB
{
    public partial class UWTUMCCWorkTypeSelection : System.Web.UI.Page
    {
        private RFSession _rfSess;

        public RFSession RFSess
        {
            get
            {
                if (_rfSess == null)
                {
                    this._rfSess = RFCache.rtrvRFSession(RFSupport.getString(Session, "user"), RFSupport.getString(Session, "ENVIRONMENT"));
                }

                if (_rfSess == null)
                {
                    Response.Redirect(string.Format("SignonMenuRF.aspx?msg={0}", "MSG_WORK59"));
                }

                return _rfSess;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[BHS.UWT.WEB.Constants.RFCC.RDLOCATION] != null && Session[BHS.UWT.WEB.Constants.RFCC.RDLOCATION].ToString().Length > 0)
            {
                this.ProcessWorkTypeAndLocation(false);
            }

            this.ddlWorkTypes.Focus();

            this.tbLocation.Attributes.Add("onkeyup", "javascript:this.value=this.value.toUpperCase();");

            object currentWorkType = Session[BHS.UWT.WEB.Constants.RFCC.WORKTYPE];

            if (RFSess == null)
            {
                Response.Redirect(string.Format("SignonMenuRF.aspx?msg={0}", "MSG_WORK59"));
            }

            if (!IsPostBack)
            {
                // Load Work Types
                string sql = @"select work_type from work_type where work_group = 'Cycle Counting' and user_def1 = 'Y' order by work_type";

                Manh.WMFW.General.Session session = SessionMapper.ConvertFromLegacySession(RFSess);

                DataHelper dataHelper = new DataHelper(session);

                IDataParameter[] paramArray = new IDataParameter[0];
                DataTable workTypeData = dataHelper.GetTable(CommandType.Text, sql, paramArray);

                if (workTypeData != null)
                {
                    foreach (DataRow workTypeRow in workTypeData.Rows)
                    {
                        ddlWorkTypes.Items.Add(workTypeRow["work_type"].ToString());
                    }

                    if (currentWorkType != null)
                        ddlWorkTypes.SelectedValue = currentWorkType.ToString();
                }
            }

            this.tbLocation.Focus();
        }

        protected void butStart_Click(object sender, EventArgs e)
        {
            ProcessWorkTypeAndLocation(true);
        }

        private void ProcessWorkTypeAndLocation(bool action)
        {
            DataHelper dataHelper = null;
            try
            {
                Session.Add(BHS.UWT.WEB.Constants.RFCC.WORKTYPE, ddlWorkTypes.SelectedValue);
                Session.Add(BHS.UWT.WEB.Constants.RFCC.LOCATION, tbLocation.Text);

                // 1) See if the location is a multi-item location
                dataHelper = new DataHelper(RFSess.GetCSSession());
                string sp = "BHS_CCLotMod_LocInv_AnalyzeCCRequests";

                IDataParameter[] paramArray = new IDataParameter[4];
                paramArray[0] = dataHelper.BuildParameter("@Location", tbLocation.Text);
                paramArray[1] = dataHelper.BuildParameter("@WorkType", ddlWorkTypes.SelectedValue);
                paramArray[2] = dataHelper.BuildParameter("@User", RFSess.User);
                paramArray[3] = dataHelper.BuildParameter("@Warehouse", RFSess.getDefaultWhs());

                DataTable locData = dataHelper.GetTable(CommandType.StoredProcedure, sp, paramArray);

                

                if (locData != null && locData.Rows.Count > 0)
                {

                    List<CountGroup> countGroups = GetCountGroups(locData);
                    Session.Add(BHS.UWT.WEB.Constants.RFCC.COUNTGROUP, countGroups); 

                    Session.Add(BHS.UWT.WEB.Constants.RFCC.SINGLEITEM, DataManager.GetString(locData.Rows[0], "MULTI_ITEM"));
                    Session.Add("Plan", DataManager.GetString(locData.Rows[0], "Plan"));
                    Session.Add(BHS.UWT.WEB.Constants.RFCC.WORKUNIT, DataManager.GetString(locData.Rows[0], "WORK_UNIT")); 
                    Session.Remove(BHS.UWT.WEB.Constants.RFCC.LOCATION);
                    Session.Add(BHS.UWT.WEB.Constants.RFCC.LOCATION, DataManager.GetString(locData.Rows[0], "LOCATION"));

                    RFSupport.redirect(RFSess, Response, "UWTUMCCPerformCount.aspx");
                }
                else
                {
                    lblMsg.Text = "Please see manager for work.";   
                }
            }
            catch(Exception ex)
            {
                ExceptionManager.LogException(RFSess, ex);
                Response.Redirect(string.Format("SignonMenuRF.aspx?msg={0}", "MSG_WORK59"));
            }

        }

        protected void butCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("SignonMenuRF.aspx");
        }

        private List<CountGroup> GetCountGroups(DataTable dataTable)
        {
            List<CountGroup> countGroups = new List<CountGroup>();

            foreach(DataRow row in dataTable.Rows)
            {
                countGroups.Add(new CountGroup(row["Location"].ToString(), row["Item"].ToString(), row["company"].ToString()));
            }

            return countGroups;
        }
    }
}
