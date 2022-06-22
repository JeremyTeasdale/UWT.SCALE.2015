using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace BHS.UWT.WEB
{
    public class CountGroup
    {
        public CountGroup(string location, string item, string company)
        {
            this.Location = location;
            this.Item = item;
            this.Company = company;
        }

        private string _location;

        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        private string _item;

        public string Item
        {
            get { return _item; }
            set { _item = value; }
        }

        private string _company;

        public string Company
        {
            get { return _company; }
            set { _company = value; }
        }
	
    }
}
