using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using Manh.WMFW.DataAccess;
using Manh.WMFW.General;

namespace BHS.UWT.Web
{
    public class Uom
    {
        public string QuantityUM { get; set; }
        public decimal ConversionQty { get; set; }

        public Uom (DataRow row)
        {
            QuantityUM = DataManager.GetString(row, "QUANTITY_UM");
            ConversionQty = DataManager.GetDecimal(row, "CONVERSION_QTY");
        }

        public static List<Uom> GetUomList(int internalInstructionNum, Session session)
        {
            List<Uom> Uoms = new List<Uom>();

            using (DataHelper dataHelper = new DataHelper(session))
            {
                IDataParameter[] paramArray = new IDataParameter[1];
                paramArray[0] = dataHelper.BuildParameter("@internalInstructionNum", internalInstructionNum);

                DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_CountBack_GetUoms", paramArray);

                foreach (DataRow row in table.Rows)
                {
                    Uoms.Add(new Uom(row));
                }

                return Uoms;
            }
        }
    }
}
