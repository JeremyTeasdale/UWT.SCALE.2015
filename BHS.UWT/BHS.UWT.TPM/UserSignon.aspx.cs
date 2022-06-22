using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace BHS.UWT.TPM
{
    public partial class UserSignon : System.Web.UI.Page
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

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            string userName = UserName.Text;
            string passWord = Password.Text;

            var user = GeneralRepository.GetUser(userName);

            if (user == null)
            {
                // user doesn't exist. display validation message and stay on page. 
                var err = new CustomValidator()
                {
                    ValidationGroup = "Login1",
                    IsValid = false,
                    ErrorMessage = "User does not exist."
                };

                Page.Validators.Add(err);
                return;
            }

            if (user.PASSWORD != passWord)
            {
                // password isn't correct. display validation message and stay on page.
                var errPW = new CustomValidator()
                {
                    ValidationGroup = "Login1",
                    IsValid = false,
                    ErrorMessage = "Incorrect password."
                };
                Page.Validators.Add(errPW);
                return;
            }

            // user is good to go. authenticate user 
            FormsAuthentication.SetAuthCookie(user.WEB_USER1, false);

            // store username in session for later use
            HttpContext.Current.Session["User"] = user.WEB_USER1;

            // redirect to welcome page.
            Response.Redirect("Welcome.aspx");

        }


    }
}