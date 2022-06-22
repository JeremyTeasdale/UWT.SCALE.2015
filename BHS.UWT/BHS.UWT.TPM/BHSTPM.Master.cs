using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Security;

namespace BHS.UWT.TPM
{
    public partial class BHSTPM : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.IsNewSession || HttpContext.Current.Session == null || HttpContext.Current.Session["User"] == null)
            {
                // user needs to authenticate
                FormsAuthentication.SignOut();
                HttpContext.Current.Session.Clear();
                Response.Redirect("UserSignon.aspx");
            }

            // left
            if (!string.IsNullOrEmpty(SessionHelper.ImgLeft))
            {
                imgLeft.ImageUrl = SessionHelper.ImgLeft;
                linkLeft.NavigateUrl = SessionHelper.URLLeft;
            }
            else
            {
                imgLeft.Visible = false;
                linkLeft.Visible = false;
            }

            // center
            if (!string.IsNullOrEmpty(SessionHelper.ImgCenter))
            {
                imgCenter.ImageUrl = SessionHelper.ImgCenter;
                linkCenter.NavigateUrl = SessionHelper.URLCenter;
            }
            else
            {
                imgCenter.Visible = false;
                linkCenter.Visible = false;
            }

            // right
            if (!string.IsNullOrEmpty(SessionHelper.ImgRight))
            {
                imgRight.ImageUrl = SessionHelper.ImgRight;
                linkRight.NavigateUrl = SessionHelper.URLRight;
            }
            else
            {
                imgRight.Visible = false;
                linkRight.Visible = false;
            }       
        }
    }
}
