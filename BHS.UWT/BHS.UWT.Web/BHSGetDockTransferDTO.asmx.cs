using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Manh.WMFW.DataAccess;
using Manh.WMFW.General;
using System.Diagnostics;
using System.Data;

namespace BHS.UWT.Web
{
    /// <summary>
    /// Summary description for Test1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class BHSGetDockTransferDTO : System.Web.Services.WebService
    {
        [WebMethod]
        public DockTransferDTO GetDTO(string containerId)
        {
            var dto = new DockTransferDTO();

            // spin up new ils session
            Session session = null;

            try
            {
                session = new Session();
                DataTable table;
                using (DataHelper dataHelper = new DataHelper(session))
                {
                    IDataParameter[] paramArray = new IDataParameter[1];
                    paramArray[0] = dataHelper.BuildParameter("@containerId", containerId);

                    table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_GetDefaultNextDockLocation", paramArray);

                    if ((table != null) && (table.Rows.Count == 1))
                    {
                        dto.DockLocation = DataManager.GetString(table.Rows[0], 0);
                    }
                }
                
                var cmdText = "SELECT CONTAINER_COUNT_NUMBER, CONTAINER_COUNT_TOTAL FROM SHIPPING_CONTAINER WHERE CONTAINER_ID = @containerId";
                using (DataHelper helper = new DataHelper(session))
                {
                    table = helper.GetTable(CommandType.Text, cmdText, new IDataParameter[] { helper.BuildParameter("@containerId", containerId) });
                }

                if (table.Rows.Count > 0)
                {
                    dto.ContainerCount = DataManager.GetInt(table, 0, "CONTAINER_COUNT_NUMBER");
                    dto.ContainerCountTotal = DataManager.GetInt(table, 0, "CONTAINER_COUNT_TOTAL");
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.LogException(session, ex);
                Debug.WriteLine(ex.ToString());
            }

            return dto;
        }
    }
}
