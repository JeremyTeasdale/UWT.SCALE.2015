using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
//using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Diagnostics;

using com.pronto.ds;
using com.pronto.general;
using com.pronto.wrappers.work;

using Manh.WMFW.Entities;
using Manh.WMFW.General;
using Manh.WMFW.DataAccess;

using Manh.WMW.Configs.General;
using Manh.WMW.General.General;
//using BHS.JJT.BLL;
//using BHS.JJT.BLL.Util;
using Manh.WMW.Inventory.General;

namespace BHS.UWT.WEB
{
    public partial class UWTUMCCPerformCount : System.Web.UI.Page
    {
        private WorkExecution _workExecution = null;

        private WorkExecution GetWorkExecutionObject
        {
            get
            {
                if (null == this._workExecution)
                {
                    this._workExecution = new WorkExecution();
                }
                return this._workExecution;
            }
        }

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
            WriteDebug("BHS Cycle Count perform CC Page Load -> t2");

            this.tbUMQty1.Focus();

            try
            {
                object currentWorkType = Session[BHS.UWT.WEB.Constants.RFCC.WORKTYPE];

                this.tbUMQty1.Attributes.Add("onkeyup", "javascript: UpdateQTY(); true;");
                this.tbUMQty2.Attributes.Add("onkeyup", "javascript: UpdateQTY(); true;");
                this.tbUMQty3.Attributes.Add("onkeyup", "javascript: UpdateQTY(); true;");
                this.txtItem.Attributes.Add("onkeyup", "javascript:this.value=this.value.toUpperCase();");
                this.txtLot.Attributes.Add("onkeyup", "javascript:this.value=this.value.toUpperCase();");

                if (Session["Message"] != null)
                {
                    lblMsg.Text = Session["Message"].ToString();
                    Session.Remove("Message");
                }

                if (!IsPostBack)
                {
                    lblLot.Text = "Lot:";

                    List<WorkConfirmDS> workConfirmationList = null;
                    List<CountGroup> countGroup = null;

                    if (Session[BHS.UWT.WEB.Constants.RFCC.COUNTGROUP] != null)
                        countGroup = (List<CountGroup>)Session[BHS.UWT.WEB.Constants.RFCC.COUNTGROUP];
                    if (countGroup.Count > 0)
                    {
                        Session.Add(BHS.UWT.WEB.Constants.RFCC.LASTCOUNTEDLOC, countGroup[0].Location);
                        workConfirmationList = RetrieveWorkDS(Session[BHS.UWT.WEB.Constants.RFCC.WORKUNIT].ToString(), countGroup[0].Location, countGroup[0].Item, countGroup[0].Company);
                        countGroup.RemoveAt(0);
                    }

                    if (workConfirmationList != null && workConfirmationList.Count > 0)
                    {
                        this.lblItem.Text = workConfirmationList[0].getItem();
                        this.lblDesc.Text = workConfirmationList[0].getItemDesc();
                        this.lblLocation.Text = workConfirmationList[0].getLocation();
                    }
                    else
                    {
                        this.lblLocation.Text = Session[BHS.UWT.WEB.Constants.RFCC.LASTCOUNTEDLOC].ToString();
                    }

                    // retrieve the list of UMs for this item
                    IList list = ItemUMRetrieval.RetrieveUMsFromItemUOM(RFSess.GetCSSession(), this.lblItem.Text, RetrieveCompany(this.lblItem.Text), RFSess.getLocalWhs());
                    Manh.WMFW.Entities.ItemUnitOfMeasureBE umBE = null;
                    if (list.Count >= 1 && !(string.IsNullOrEmpty(this.lblItem.Text)))
                    {
                        umBE = (ItemUnitOfMeasureBE)list[0];
                        this.lblUM1.Text = string.Format("{0} ({1})", umBE.QuantityUm, decimal.Round(umBE.ConversionQty, 0));
                        this.hfUMConvQty1.Value = int.Parse(decimal.Round(umBE.ConversionQty, 0).ToString()).ToString();
                        txtItem.Visible = false;
                        txtItem.Enabled = false;
                        txtLot.Visible = false;
                        txtLot.Enabled = false;
                        lblLot.Visible = false;
                        butDone.Visible = false;
                        but_Add.Visible = false;
                    }
                    else if (Session["Add"] != null && Session["Add"].ToString() == "Y")
                    {
                        this.lblUM1.Text = "CS";
                        this.hfUMConvQty1.Value = "1";
                        this.lblUM1.Visible = true;
                        this.tbUMQty1.Visible = true;
                        this.revUM1Qty.Visible = false;
                        this.revUM1Qty.Enabled = false;
                        this.lblDesc.Visible = false;
                        txtItem.Visible = true;
                        lblItem.Visible = false;
                        txtLot.Visible = true;
                        lblLot.Visible = true;
                        this.panelDef.DefaultButton = butDone.ID;
                        
                        //buttons
                        butDone.Visible = true;
                        this.butStart.Visible = true;
                        this.butDone.Focus();
                        but_Add.Visible = false;
                    }
                    else 
                    {
                        if (string.IsNullOrEmpty(lblMsg.Text))
                        {
                            butStart.Visible = true;
                            butDone.Visible = false;
                            if (list.Count > 0)
                                this.lblMsg.Text = "Confirm Empty";
                            else
                                this.lblMsg.Text = "Item unit of measures do not exist";
                        }
                        else
                        {
                            butDone.Visible = true;
                            butStart.Visible = false;
                            panelDef.DefaultButton = butDone.ID;
                        }

                        this.tbUMQty1.Visible = false;
                        lblUM1.Text = "";
                        lblDesc.Text = "";
                        txtItem.Visible = false;
                        txtLot.Visible = false;
                        lblLot.Visible = false;
                    }
                    
                    if (list.Count >= 2)
                    {
                        umBE = (ItemUnitOfMeasureBE)list[1];
                        this.lblUM2.Text = string.Format("{0} ({1})", umBE.QuantityUm, decimal.Round(umBE.ConversionQty, 0));
                        this.hfUMConvQty2.Value = int.Parse(decimal.Round(umBE.ConversionQty, 0).ToString()).ToString();
                    }
                    else
                    {
                        this.lblUM2.Visible = false;
                        this.tbUMQty2.Visible = false;
                        this.revUM2Qty.Visible = false;
                        this.revUM2Qty.Enabled = false;
                    }
                    if (list.Count >= 3)
                    {
                        umBE = (ItemUnitOfMeasureBE)list[2];
                        this.lblUM3.Text = string.Format("{0} ({1})", umBE.QuantityUm, decimal.Round(umBE.ConversionQty, 0));
                        this.hfUMConvQty3.Value = int.Parse(decimal.Round(umBE.ConversionQty, 0).ToString()).ToString();
                    }
                    else
                    {
                        this.lblUM3.Visible = false;
                        this.tbUMQty3.Visible = false;
                        this.revUM3Qty.Visible = false;
                        this.revUM3Qty.Enabled = false;

                    }
                }
            }
            catch (Exception ex)
            {
                if (RFSess != null)
                    ExceptionManager.LogException(RFSess.GetCSSession(), ex);
                else
                    ExceptionManager.LogException(new Manh.WMFW.General.Session(), ex);
            }
        }

        private List<WorkConfirmDS> RetrieveWorkDS(string workUnit, string location, string item, string company)
        {

            try
            {
                // 1) See if we have any work for this worktype that is open
                using (DataHelper dataHelper = new DataHelper(RFSess.GetCSSession()))
                {
                    string sp = "BHS_CCLotMod_WorkInstruction_CCRtrv";
                    IDataParameter[] paramArray = new IDataParameter[5];
                    paramArray[0] = dataHelper.BuildParameter("@WorkUnit", workUnit);
                    paramArray[1] = dataHelper.BuildParameter("@Location", location);
                    paramArray[2] = dataHelper.BuildParameter("@Item", item);
                    paramArray[3] = dataHelper.BuildParameter("@Comapny", company);
                    paramArray[4] = dataHelper.BuildParameter("@User", RFSess.User);
                    DataTable workData = dataHelper.GetTable(CommandType.StoredProcedure, sp, paramArray);

                    WriteDebug(string.Format("BHS Cycle Count RetrieveWorkDS -> exec BHS_CCLotMod_WorkInstruction_CCRtrv '{0}', '{1}', '{2}', '{3}', '{4}'", workUnit, location, item, company, RFSess.User));

                    List<WorkConfirmDS> workConfirmationList = new List<WorkConfirmDS>();
                    
                    int systemQuantity = 0;
                    if (workData != null && workData.Rows.Count > 0)
                    {
                        foreach (DataRow workRow in workData.Rows)
                        {
                            //work_unit, internal_instruction_num, from_loc, item, company
                            WorkConfirmDS workConfirmDS = new WorkConfirmDS();
                            workConfirmDS.setWorkUnit(DataManager.GetString(workRow, "work_unit"));
                            workConfirmDS.setInstructionNum(DataManager.GetInt(workRow, "internal_instruction_num"));
                            workConfirmDS.setLocation(DataManager.GetString(workRow, "from_loc"));
                            
                            workConfirmDS.setItem(DataManager.GetString(workRow, "item"));
                            workConfirmDS.setItemDesc(DataManager.GetString(workRow, "item_desc"));
                            workConfirmDS.setLot(DataManager.GetString(workRow, "Lot"));
                            workConfirmDS.SetExpDateTimeTime(DataManager.GetDateTime(workRow, "EXPIRATION_DATE"));
                            
                            workConfirmDS.setCompany(DataManager.GetString(workRow, "company"));
                            workConfirmDS.setWarehouse(DataManager.GetString(workRow, "from_whs"));
                            workConfirmDS.setCountedQty(DataManager.GetDouble(workRow, "system_qty"));

                            if (!string.IsNullOrEmpty(DataManager.GetString(workRow, "LOGISTICS_UNIT")))
                                workConfirmDS.setContainerID(new string[] { DataManager.GetString(workRow, "LOGISTICS_UNIT") });

                            systemQuantity += DataManager.GetInt(workRow, "system_qty");
                            BHS.UWT.WEB.WorkHandler.LockWork(RFSess.GetCSSession(), workConfirmDS.getWorkUnit(), RFSess.getUserName(), RFSess.getTeam());

                            workConfirmationList.Add(workConfirmDS);
                        
                            Session.Add("CCPlan", DataManager.GetString(workRow, "REFERENCE_ID"));
                            Session.Add(BHS.UWT.WEB.Constants.RFCC.WORKDS, workConfirmationList);
                        }

                        Session.Add(BHS.UWT.WEB.Constants.RFCC.SYSTEMQTY, systemQuantity);
                        return workConfirmationList;
                    }
                    else
                    {
                        
                        //this.lblMsg.Text = "Cycle Count has been completed.";
                    }
                }
            }
            catch (Exception ex)
            {
                if (RFSess != null)
                    ExceptionManager.LogException(RFSess.GetCSSession(), ex);
                else
                    ExceptionManager.LogException(new Manh.WMFW.General.Session(), ex);

                Response.Redirect(string.Format("SignonMenuRF.aspx?msg={0}", "MSG_WORK59"));
            }

            return new List<WorkConfirmDS>();
        }

        protected void butDone_Click(object sender, EventArgs e)
        {
            Session.Remove(BHS.UWT.WEB.Constants.RFCC.RECOUNT);
            Session.Remove(BHS.UWT.WEB.Constants.RFCC.SYSTEMQTY);
            Session.Remove(BHS.UWT.WEB.Constants.RFCC.WORKDS);
            lblMsg.Text = "";
            Response.Redirect("UWTUMCCWorkTypeSelection.aspx");
        }

        protected void but_AddClick(object sender, EventArgs e)
        {
            Session.Add("AddMode", "Y");
            this.lblUM1.Text = "CS";
            this.hfUMConvQty1.Value = "1";
            this.lblUM1.Visible = true;
            this.tbUMQty1.Visible = true;
            this.revUM1Qty.Visible = false;
            this.revUM1Qty.Enabled = false;
            this.lblDesc.Visible = false;
            butStart.Visible = true;
            txtItem.Visible = true;
            txtItem.Focus();
            lblItem.Visible = false;
            txtLot.Visible = true;
            lblLot.Visible = true;
            butDone.Visible = true;
            but_Add.Visible = false;
            panelDef.DefaultButton = butStart.ID;
        }

        private int CountedQty
        {
            get
            {
                int qty = GetIntForString(this.tbUMQty1.Text);

                if (this.tbUMQty2.Visible)
                    qty += GetIntForString(this.tbUMQty2.Text) * GetIntForString(this.hfUMConvQty2.Value);
                
                if (this.tbUMQty3.Visible)
                    qty += GetIntForString(this.tbUMQty3.Text) * GetIntForString(this.hfUMConvQty3.Value);
               

                return qty;
            }
        }

        private int GetIntForString(string data)
        {
            data = ((data != null && data.Trim().Length > 0) ? data : "0");
            return int.Parse(data);
        }

        protected void butStart_Click(object sender, EventArgs e)
        {
            WriteDebug("BHS Cycle Count Start Click -> Start");
            RFSession rfSess = null;
            
            try
            {
                if (txtItem.Enabled && txtItem.Visible == true) 
                {
                    //A new item is being added.
                    WriteDebug("BHS Cycle Count Start Click -> a new item is being added");
                    LotValidation lotValidation = new LotValidation();
                    string message = lotValidation.ValidateLot(RFSess.GetCSSession(), txtLot.Text, txtItem.Text, RetrieveCompany(txtItem.Text), RFSess.getLocalWhs());

                    if (!string.IsNullOrEmpty(message) && message != "MSG_LOT07") //the base validation fires when a new lot is being created, but in this case i'm not necessairly create a lot, i might be adjusting that lot.  SO ignore this error.  (MSG_LOT07 = "Lot already exists")
                    {
                        this.lblMsg.Text = ResourceManager.GetStringResource(RFSess.GetCSSession(), message, ResourceGroups.Msg);
                    }
                    else
                    {
                        this.lblMsg.Text = CreateCycleCountRequests();
                        if (string.IsNullOrEmpty(lblMsg.Text))
                        {
                            txtItem.Text = "";
                            txtLot.Text = "";
                            tbUMQty1.Text = "";
                            lblMsg.Text = "Count Completed";
                        }
                    }
                }
                else
                {
                    //Counting an existing item.
                    WriteDebug("BHS Cycle Count Start Click -> Counting Existing Item");
                    int SystemQty = int.Parse(Session[BHS.UWT.WEB.Constants.RFCC.SYSTEMQTY].ToString());
                    if ((SystemQty == CountedQty) || (Session[BHS.UWT.WEB.Constants.RFCC.RECOUNT] != null && int.Parse(Session[BHS.UWT.WEB.Constants.RFCC.LASTCOUNT].ToString()) == CountedQty))
                    {
                        //There is a descrepency
                        WriteDebug("BHS Cycle Count Start Click -> There is a descrepency");

                        rfSess = RFCache.rtrvRFSession(RFSupport.getString(Session, "user"), RFSupport.getString(Session, "ENVIRONMENT"));
                        List<WorkConfirmDS> workConfirmDSList = (List<WorkConfirmDS>)Session[BHS.UWT.WEB.Constants.RFCC.WORKDS];
                        DataRow dataRow = null;
                        dataRow = RetrieveLot(lblLocation.Text, lblItem.Text, rfSess);
                        int discrepancy = CountedQty - (int)DataManager.GetDecimal(dataRow, "Quantity");
                        //int tempDiscrepency = 0;

                        WriteDebug(string.Format("BHS Cycle Count Start Click -> starting look over workConfirmDSList.  workConfirmDSList.Count = {0}", workConfirmDSList.Count));
                        foreach (WorkConfirmDS workConfirmDS in workConfirmDSList)
                        {
                            BHS.UWT.WEB.WorkHandler.LockWork(rfSess.GetCSSession(), workConfirmDS.getWorkUnit(), rfSess.getUserName(), rfSess.getTeam());

                            WriteDebug(string.Format("BHS Cycle Count Start Click -> Start of loop.  WorkConfimDS internalNum = {0}", workConfirmDS.getInstructionNum()));
                            WriteDebug(string.Format("BHS Cycle Count Start Click ->     discrepancy = {0}", discrepancy));
                            WriteDebug(string.Format("BHS Cycle Count Start Click ->     workConfirmDS.getCountedQty() = {0}", workConfirmDS.getCountedQty()));

                            //tempDiscrepency = discrepancy;

                            if (discrepancy < 0 && workConfirmDS.getCountedQty() + discrepancy < 0)  //we can't drive counted quantity negative
                            {
                                WriteDebug(string.Format("BHS Cycle Count Start Click ->     If condition true.  discrepancy < 0 && workConfirmDS.getCountedQty() + discrepancy < 0)", discrepancy));
                                discrepancy = discrepancy + (int)workConfirmDS.getCountedQty();
                                WriteDebug(string.Format("BHS Cycle Count Start Click ->     discrepancy set to {0}", discrepancy));
                                workConfirmDS.setCountedQty(0);
                            }
                            else
                            {
                                WriteDebug(string.Format("BHS Cycle Count Start Click ->     If condition False.  discrepancy < 0 && workConfirmDS.getCountedQty() + discrepancy < 0)", discrepancy));
                                workConfirmDS.setCountedQty(workConfirmDS.getCountedQty() + discrepancy);
                                WriteDebug(string.Format("BHS Cycle Count Start Click ->     WorkConfirmDS counted quantity set to counted quantity + discrepancy == {0} + {1} = {2}", workConfirmDS.getCountedQty(), discrepancy, workConfirmDS.getCountedQty() + discrepancy));
                                discrepancy = 0;
                                WriteDebug(string.Format("BHS Cycle Count Start Click ->     discrepancy set to {0}", discrepancy));
                            }


                            Manh.WMFW.General.Session sesion = rfSess.GetCSSession();

                            WriteDebug(string.Format("BHS Cycle Count Start Click ->     Confirming work {0}", workConfirmDS.GetInternalNum()));
                            this.GetWorkExecutionObject.confirmCycleCount(rfSess.stSession, rfSess.GetCSSession(), workConfirmDS, false);
                        }

                        Session.Add("Message", string.Format("Location '{0}' Counted", Session[BHS.UWT.WEB.Constants.RFCC.LASTCOUNTEDLOC]));
                        RFSupport.redirect(rfSess, Response, "UWTUMCCPerformCount.aspx");
                    }
                    else
                    {
                        WriteDebug("BHS Cycle Count Start Click -> There is NOT a descrepency");

                        Session.Add(BHS.UWT.WEB.Constants.RFCC.RECOUNT, true);
                        Session.Add(BHS.UWT.WEB.Constants.RFCC.LASTCOUNT, CountedQty);
                        if (!lblHeading.Text.Contains("Recount"))
                        {
                            this.lblHeading.Text = this.lblHeading.Text + "(Recount)";
                            Session.Add("Message", "");
                            lblMsg.Text = "";
                        }
                        this.tbUMQty1.Text = "";
                        this.tbUMQty2.Text = "";
                        this.tbUMQty3.Text = "";
                    }
                }
            }
            catch (System.Threading.ThreadAbortException ex)
            {
            }
            catch (Exception ex)
            {
                WriteDebug(string.Format("BHS Cycle Count Start Click -> exception = {0}", ex.Message));

                if (rfSess != null)
                    ExceptionManager.LogException(rfSess.GetCSSession(), ex);
                else
                    ExceptionManager.LogException(new Manh.WMFW.General.Session(), ex);

                Response.Redirect(string.Format("SignonMenuRF.aspx?msg={0}", "MSG_WORK59"));
            }
        }

        public string CreateCycleCountRequests()
        {
            using (DataHelper dataHelper = new DataHelper(RFSess.GetCSSession()))
            {
                IDataParameter[] paramArray = new IDataParameter[8];
                paramArray[0] = dataHelper.BuildParameter("@Item", txtItem.Text);
                paramArray[1] = dataHelper.BuildParameter("@Location", lblLocation.Text);
                paramArray[2] = dataHelper.BuildParameter("@Warehouse", RFSess.getLocalWhs());
                paramArray[3] = dataHelper.BuildParameter("@User", RFSess.getUserName());
                paramArray[4] = dataHelper.BuildParameter("Quantity", CountedQty);
                paramArray[5] = dataHelper.BuildParameter("Plan", int.Parse(Session["Plan"].ToString()));
                paramArray[6] = dataHelper.BuildParameter("@Lot", txtLot.Text);
                paramArray[7] = dataHelper.BuildParameter("@Company", RetrieveCompany(txtItem.Text));
                DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_CCLotMod_CCR_AddPendingReview", paramArray);

                if (table != null && table.Rows.Count > 0)
                    return DataManager.GetString(table.Rows[0], "Message");
                else
                    return null;
            }
        }

        private DataRow RetrieveLot(string location, string item, RFSession rfSession)
        {
            using (DataHelper dataHelper = new DataHelper(RFSess.GetCSSession()))
            {
                IDataParameter[] paramArray = new IDataParameter[2];
                //paramArray[0] = dataHelper.BuildParameter("@Lot", (Lot));
                paramArray[0] = dataHelper.BuildParameter("@Location", location);
                paramArray[1] = dataHelper.BuildParameter("@Item", item);
                DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_CCLotMod_LocInv_VerifyLOT", paramArray);

                if (table != null && table.Rows.Count > 0)
                    return table.Rows[0];
                else
                    return null;
            }
        }

        private string RetrieveCompany(string item)
        {

            using (DataHelper dataHelper = new DataHelper(RFSess.GetCSSession()))
            {
                IDataParameter[] paramArray = new IDataParameter[1];
                paramArray[0] = dataHelper.BuildParameter("@Item", item);
                DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_CCLotMod_RetrieveCompany", paramArray);

                if (table != null && table.Rows.Count > 0)
                    return table.Rows[0]["company"].ToString();
                else
                    return null;
            }
        }

        private void WriteCCStats(decimal systemQty)
        {
            using (DataHelper dataHelper = new DataHelper(RFSess.GetCSSession()))
            {
                IDataParameter[] paramArray = new IDataParameter[8];
                paramArray[0] = dataHelper.BuildParameter("@Item", lblItem.Text);
                paramArray[1] = dataHelper.BuildParameter("@Location", lblLocation.Text);
                paramArray[2] = dataHelper.BuildParameter("@CountedQty", CountedQty);
                paramArray[3] = dataHelper.BuildParameter("@SystemQty", systemQty);
                paramArray[4] = dataHelper.BuildParameter("@Plan", Session["CCPlan"].ToString());
                paramArray[5] = dataHelper.BuildParameter("@ReconciledQty", -1);
                paramArray[6] = dataHelper.BuildParameter("@User", RFSess.User);
                paramArray[7] = dataHelper.BuildParameter("@Warehouse", RFSess.getLocalWhs());
                DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_CCLotMod_CCR_WriteStats", paramArray);
            }
        }

        private void WriteDebug(string s)
        {
            Debug.WriteLine(s);
            //While testing, i couldn't get debug view to work.  so for a while I wrote to a text file.  Coommenting it out before deploying to prod
            //File.AppendAllText(@"C:\Program Files\Manhattan Associates\ILS\2015\RF\Log\Log.txt", string.Format("{0}\r\n", s));
        }
    }
}
