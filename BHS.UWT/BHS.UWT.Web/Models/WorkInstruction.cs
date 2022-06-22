using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using Manh.WMFW.DataAccess;
using Manh.WMFW.General;

namespace BHS.UWT.Web
{
    public class WorkInstruction
    {
        public string location { get; set; }
        public string item { get; set; }
        public string desc { get; set; }
        public string lot { get; set; }
        public string qtyPicked { get; set; }
        public decimal correctQtyAtPageLoad { get; set; }
        public bool requiresCountBack { get; set; }
        public string Lpn { get; set; }

        public WorkInstruction(DataRow row)
        {
            location = DataManager.GetString(row, "FROM_LOC");
            item = DataManager.GetString(row, "ITEM");
            desc = DataManager.GetString(row, "ITEM_DESC");
            lot = DataManager.GetString(row, "Lot");
            qtyPicked = DataManager.GetString(row, "QtyPicked");
            correctQtyAtPageLoad = DataManager.GetDecimal(row, "CorrectQtyAtPageLoad");
            requiresCountBack = (DataManager.GetString(row, "RequiresCountBack") == "Y");
            Lpn = DataManager.GetString(row, "Lpn");
        }

        public static WorkInstruction GetWorkInstruction(int internalInstructionNum, Session session)
        {
            using (DataHelper dataHelper = new DataHelper(session))
            {
                IDataParameter[] paramArray = new IDataParameter[2];
                paramArray[0] = dataHelper.BuildParameter("@internalInstructionNum", internalInstructionNum);
                paramArray[1] = dataHelper.BuildParameter("@PrePick", "False");

                DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_CountBack_GetWorkInstructionDetails", paramArray);

                return (new WorkInstruction(table.Rows[0]));
            }
        }

        public static WorkInstruction GetPrePickWorkInstruction(int internalInstructionNum, Session session)
        {
            using (DataHelper dataHelper = new DataHelper(session))
            {
                IDataParameter[] paramArray = new IDataParameter[2];
                paramArray[0] = dataHelper.BuildParameter("@internalInstructionNum", internalInstructionNum);
                paramArray[1] = dataHelper.BuildParameter("@PrePick", "True");

                DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_CountBack_GetWorkInstructionDetails", paramArray);

                return (new WorkInstruction(table.Rows[0]));
            }
        }

        public static void SubmitCountBackQty(int internalInstructionNum, int Qty, Session session)
        {
            using (DataHelper dataHelper = new DataHelper(session))
            {
                IDataParameter[] paramArray = new IDataParameter[2];
                paramArray[0] = dataHelper.BuildParameter("@internalInstructionNum", internalInstructionNum);
                paramArray[1] = dataHelper.BuildParameter("@Qty", Qty);

                dataHelper.Insert(CommandType.StoredProcedure, "BHS_CountBack_SubmitCountBackQty", paramArray);
            }
        }

        public static bool WorkInstrRequiresValidation(int internalInstructionNum, Session session)
        {
            using (DataHelper dataHelper = new DataHelper(session))
            {
                IDataParameter[] paramArray = new IDataParameter[1];
                paramArray[0] = dataHelper.BuildParameter("@internalInstructionNum", internalInstructionNum);

                DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_WorkInstrRequiresValidation", paramArray);

                return table.Rows.Count > 0;
            }
        }
    }
}
