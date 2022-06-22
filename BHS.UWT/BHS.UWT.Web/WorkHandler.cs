using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;


using Manh.WMFW.General;
using Manh.WMFW.DataAccess;
using Manh.WMW.Reporting.General;

namespace BHS.UWT.WEB
{
    public class WorkHandler
    {

        public static bool LockWork(Session session, string workUnit, string lockUser, string lockTeam)
        {
            int num = 0;
            using (DataHelper helper = new DataHelper(session))
            {
                num = helper.Update(CommandType.Text, " update \tWORK_INSTRUCTION \tset \t  USER_ASSIGNED = @user, \t  TEAM_ASSIGNED = @team, \t  CONDITION = @INPROCESS,\t  START_DATE_TIME = \t\tcase \t\t when START_DATE_TIME is null then getDate() \t\t else START_DATE_TIME \t\tend  where \t  WORK_UNIT = @workUnit \t  and    CONDITION <> @CLOSED \t  and (        USER_ASSIGNED is null         or         USER_ASSIGNED = @user) ", new IDataParameter[] { helper.BuildParameter("@user", lockUser), helper.BuildParameter("@team", lockTeam), helper.BuildParameter("@INPROCESS", com.pronto.general.Constants.stINPROCESS), helper.BuildParameter("@workUnit", workUnit), helper.BuildParameter("@CLOSED", com.pronto.general.Constants.stCLOSED) });
            }
            if (num <= 0)
            {
                return false;
            }

            HistoryHandling.WriteTransHistoryWorkUnitStart(SessionMapper.GetSerializedSessionString(session), "WorkInstructionManipulation.LockWork", workUnit);

            return true;
        }



    }
}

