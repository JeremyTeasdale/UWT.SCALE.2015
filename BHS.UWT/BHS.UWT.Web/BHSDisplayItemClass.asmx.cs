using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Manh.WMFW.DataAccess;
using Manh.WMFW.General;
using System.Diagnostics;

namespace BHS.UWT.Web
{
    /// <summary>
    /// Summary description for BHSDisplayItemClass
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class BHSDisplayItemClass : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetItemClass(string item)
        {
            Session session = null;

            var itemClass = "";

            try
            {
                session = new Session();

                using (DataHelper dataHelper = new DataHelper(session))
                {
                    IDataParameter[] paramArray = new IDataParameter[1];
                    paramArray[0] = dataHelper.BuildParameter("@item", item);

                    DataRow row = dataHelper.GetRow(CommandType.StoredProcedure, "BHS_GetItemClass", paramArray);
                    
                    if (row != null)
                    {
                        itemClass = DataManager.GetString(row, "ITEM_CLASS");
                    }
                }
            }

            catch {

            }

            return itemClass;
        }
    }
}
