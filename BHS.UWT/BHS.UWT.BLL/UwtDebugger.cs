using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Diagnostics;
using Manh.WMFW.General;

namespace BHS.UWT.BLL
{
    class UwtDebugger
    {
        public static void write(string Note, string Message)
        {
            int fixedWidth = 50;
            Note = Note.PadRight(fixedWidth).Substring(0, fixedWidth);

            Debug.Write(string.Format("UWT : {0}: {1}", Note, Message));
        }

        public static void write(string Note, Exception e)
        {
            UwtDebugger.write(Note, string.Format("!!~~ EXCEPTION ENCOUNTERED!!"));
            UwtDebugger.write(Note, string.Format("!!~~ Message {0}", e.Message));
            UwtDebugger.write(Note, string.Format("!!~~ callstack: "));

            if (e != null && e.StackTrace != null)
            {
                foreach (string line in e.StackTrace.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    UwtDebugger.write(Note, string.Format("!!~~ \t\t{0} ", line));
                }
            }
            else
            {
                write(Note, string.Format("!!~~ \t\t{0} ", "No Stacktrace in Audit log"));
            }

            UwtDebugger.write(Note, string.Format("!!~~ END EXCEPTION!!"));
        }

        public static void write(string Note, DataTable table, string tableName)
        {
            UwtDebugger.write(Note, string.Format("Start Table {0}", tableName));

            string columns = "";
            foreach (DataColumn c in table.Columns)
            {
                columns += c.ColumnName + ", ";
            }

            UwtDebugger.write(Note, columns);

            foreach (DataRow r in table.Rows)
            {
                string rowN = "";

                foreach (object o in r.ItemArray)
                {
                    rowN += o.ToString() + ", ";
                }

                UwtDebugger.write(Note, rowN);
            }

            UwtDebugger.write(Note, string.Format("End Table {0}", tableName));
        }

        public static void write(string ClassName, object[] objects)
        {
            if (objects == null)
            {
                write(ClassName, string.Format("Object array is null"));
                return;
            }

            for (int i = 0; i < objects.Length; i++)
            {
                //write(ClassName, string.Format("debug parm[{0}]", i));
                object o = objects[i];

                if (o == null)
                {
                    write(ClassName, string.Format("Object {0} is null", i));
                }
                else
                {
                    write(ClassName, string.Format("Object {0} type {1} = '{2}'", i, o.GetType(), o.ToString()));

                    IEnumerable<object> enumerable = (o as IEnumerable<object>);
                    if (enumerable != null && !(o is string))
                    {
                        foreach (object o2 in enumerable)
                        {
                            if (o2 == null)
                            {
                                write(ClassName, string.Format("\t sub Object is null"));
                            }
                            else
                            {
                                write(ClassName, string.Format("\t sub object {0} = {1}", o2.GetType(), o2));
                            }
                        }
                    }
                }
            }
        }

        public static Exception BuildException(string error, List<string> AuditLogParms)
        {
            Exception e = new Exception(error);
            e.Data.Add("ScaleAuditParms", AuditLogParms);
            return e;
        }

        public static void WriteToAuditLog(Exception e, Session session, params string[] additionalParms)
        {
            write("WriteToAuditLog", "Start");
            write("WriteToAuditLog", e);

            try
            {
                if (session == null)
                    session = SessionFactory.CreateSession();

                //Build parms

                List<string> exceptionParms = e.Data["ScaleAuditParms"] as List<string>;

                List<string> auditParms = new List<string>();

                if (e.Data["ScaleAuditParms"] != null)
                {
                    foreach (string parm in e.Data["ScaleAuditParms"] as List<string>)
                    {
                        auditParms.Add(parm);
                    }

                }

                if (additionalParms != null)
                {
                    foreach (string parm in additionalParms)
                    {
                        auditParms.Add(parm);
                    }
                }

                //Write Audit
                if (auditParms.Count > 0)
                    ExceptionManager.LogException(session, e, auditParms.ToArray());
                else
                    ExceptionManager.LogException(session, e);

                write("WriteToAuditLog", "End");
            }
            catch (Exception e2)
            {
                write("WriteToAuditLog", "failed");
                write("WriteToAuditLog", e2);
            }
        }

        public static void WriteToAuditLog(Exception e, Session session)
        {
            WriteToAuditLog(e, session, null);
        }

        //public static void write(string note, IRestResponse response)
        //{
        //    UwtDebugger.write(note, string.Format("ErrorException = {0}", response.ErrorException));
        //    UwtDebugger.write(note, string.Format("ErrorMessage = {0}", response.ErrorMessage));
        //    UwtDebugger.write(note, string.Format("IsSuccessful = {0}", response.IsSuccessful));
        //    UwtDebugger.write(note, string.Format("StatusCode = {0}", response.StatusCode));
        //    UwtDebugger.write(note, string.Format("Content = {0}", response.Content));
        //}

    }
}
