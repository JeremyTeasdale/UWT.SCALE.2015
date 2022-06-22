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
    public partial class BHSShipmentResults : System.Web.UI.Page
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
            try
            {
                if (!IsPostBack)
                {
                    LoadShipments(1);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("~/Welcome.aspx");
            }
        }


        public void LoadShipments(int page)
        {
            // clear table
            tblShipments.Dispose();
            
            // get paged items    
            int index = 1;
            if (!string.IsNullOrEmpty(ddlPages.Text))
                index = int.Parse(ddlPages.Text);

            PagedList<BHSShipmentSearchResultDO> shipments = GeneralRepository.GetShipments(SessionHelper.CurrentCompany, SessionHelper.BHSShipmentSearchDO, page, AppHelper.PagingSize);
            
            if (shipments.Count() > 0)
            {
                for (int i = 0; i < shipments.Count; i++)
                {
                    TableRow row = new TableRow();
                    row.ID = shipments[i].SHIPMENT_ID;

                    // Shipment Id
                    TableCell shipmentCell = new TableCell();
                    shipmentCell.Text = shipments[i].SHIPMENT_ID;
                    row.Cells.Add(shipmentCell);


                    //Bill of Lading Number
                    TableCell billofLadingNumber = new TableCell();
                    billofLadingNumber.Text = shipments[i].USER_DEF1;
                    row.Cells.Add(billofLadingNumber);


                    //Scheduled Ship Date
                    TableCell scheduledShipDateCell = new TableCell();
                    scheduledShipDateCell.Text = shipments[i].SCHEDULED_SHIP_DATE.ToShortDateString();
                    row.Cells.Add(scheduledShipDateCell);

                    TableCell bolCell = new TableCell();
                    bolCell.HorizontalAlign = HorizontalAlign.Right;
                    
                    HyperLink bolLink = new HyperLink();
                    bolLink.NavigateUrl = string.Format("~/BHSDocumentPrint.aspx?BHSType={0}&BHSShipment={1}", "BOL", shipments[i].USER_DEF1);
                    bolLink.Target = "_blank";
                    bolLink.Text = "BOL";
                    bolCell.Controls.Add(bolLink);
                    row.Cells.Add(bolCell);

                    TableCell pckLstCell = new TableCell();
                    pckLstCell.HorizontalAlign = HorizontalAlign.Right;

                    HyperLink pckLink = new HyperLink();
                    pckLink.NavigateUrl = string.Format("~/BHSDocumentPrint.aspx?BHSType={0}&BHSShipment={1}", "PCKLST", shipments[i].USER_DEF1);
                    bolLink.Target = "_blank";
                    pckLink.Text = "Pack List";

                    pckLstCell.Controls.Add(pckLink);
                    row.Cells.Add(pckLstCell);

                    tblShipments.Rows.Add(row);
                }

                SetupPaging(shipments);
                ResolvePagerView(page);
            }
        }

        void SetupPaging(PagedList<BHSShipmentSearchResultDO> shipments)
        {
            int pageCount = 0;
            int totalRecords = shipments.TotalCount;
            
            pageCount = totalRecords / AppHelper.PagingSize;
            if (totalRecords % AppHelper.PagingSize > 0)
                pageCount++;

            lblPageCount.Text = pageCount.ToString();

            //load up the list items
            for (int i = 1; i <= pageCount; i++)
            {
                ddlPages.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
            if (lblPageCount.Text == "1" || lblPageCount.Text == "0")
            {
                btnNext.Enabled = false;
                btnLast.Enabled = false;
            }
        }

        void ResolvePagerView(int currentPage)
        {
            int pageCount = 1;

            int.TryParse(lblPageCount.Text, out pageCount);

            int nextPage = currentPage + 1;
            int prevPage = currentPage - 1;

            btnPrev.Enabled = true;
            btnNext.Enabled = true;
            btnLast.Enabled = true;
            btnFirst.Enabled = true;


            if ((currentPage == pageCount))
            {
                btnNext.Enabled = false;
                btnLast.Enabled = false;
            }

            if ((currentPage == 1) || (pageCount == 0))
            {
                btnPrev.Enabled = false;
                btnFirst.Enabled = false;
            }
        }
        protected void pager_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string pageCommand = btn.CommandArgument;
            int currentPage = ddlPages.SelectedIndex + 1;

            switch (pageCommand)
            {
                case "First":
                    currentPage = 1;
                    break;
                case "Prev":
                    currentPage--;
                    break;
                case "Next":
                    currentPage++;
                    break;
                case "Last":
                    currentPage = int.Parse(lblPageCount.Text);
                    break;
            }

            //reset the DropDown
            ddlPages.SelectedValue = currentPage.ToString();

            //reload the grid
            LoadShipments(currentPage);
        }

        protected void ddlPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            //reload the grid
            LoadShipments(ddlPages.SelectedIndex + 1);
        }

        protected void tbCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/BHSShipmentSearch.aspx");
        }
        
    }
}
