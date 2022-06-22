using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BHS.UWT.TPM
{
    public partial class Welcome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DocumentSearch_Click(object sender, EventArgs e)
        {
            // redirect to welcome page.
            Response.Redirect("BHSShipmentSearch.aspx");
        }
    }
}