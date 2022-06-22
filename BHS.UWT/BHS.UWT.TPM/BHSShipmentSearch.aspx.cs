using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using BHS.UWT.TPM.Data;

namespace BHS.UWT.TPM
{
    public partial class BHSShipmentSearch : System.Web.UI.Page
    {
        #region Properties
        public GeneralRepository GeneralRepository
        {
            get
            {
                return new GeneralRepository();
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BHSShipmentSearchDO shipmentDO = SessionHelper.BHSShipmentSearchDO;
                if (shipmentDO != null)
                {
                    tbShipmentId.Text = shipmentDO.ShipmentId;
                    tbBOL.Text = shipmentDO.BOLNumber;
                    if(shipmentDO.ScheduledShipDate != DateTime.MaxValue)
                        tbScheduledShipDate.Text = shipmentDO.ScheduledShipDate.ToShortDateString();
                }
                else
                {
                    shipmentDO = new BHSShipmentSearchDO();
                }
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbBOL.Text = "";
            tbShipmentId.Text = "";
            tbScheduledShipDate.Text = "";

            Session["BHSSearchDO"] = null;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Session["BHSSearchDO"] = null;
            Response.Redirect("~/Welcome.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BHSShipmentSearchDO shipmentSearch = SessionHelper.BHSShipmentSearchDO;
            DateTime date = DateTime.Now;

            if (shipmentSearch == null)
                shipmentSearch = new BHSShipmentSearchDO();

            shipmentSearch.ShipmentId = tbShipmentId.Text;

            if (string.IsNullOrEmpty(tbScheduledShipDate.Text))
                shipmentSearch.ScheduledShipDate = DateTime.MaxValue;

            else if (DateTime.TryParse(tbScheduledShipDate.Text, out date))
                shipmentSearch.ScheduledShipDate = date;
            else
            {
                //shipmentSearch.ScheduledShipDate = DateTime.MaxValue;
                var err = new CustomValidator()
                {
                    ValidationGroup = "SearchValidation",
                    IsValid = false,
                    ErrorMessage = "Scheduled Ship Date is not a valid date format."
                };
                Page.Validators.Add(err);
                return;
            }

            shipmentSearch.BOLNumber = tbBOL.Text;
            Session["BHSSearchDO"] = shipmentSearch;

            Response.Redirect("~/BHSShipmentResults.aspx");
        }
    }
}
