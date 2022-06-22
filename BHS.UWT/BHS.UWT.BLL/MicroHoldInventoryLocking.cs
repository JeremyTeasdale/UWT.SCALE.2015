using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Data;

using BHS.ProcessService;

using Manh.WMFW.General;
using Manh.WMW.General;
using Manh.WMFW.Config.BL;
using Manh.WMFW.DataAccess;

using Manh.ILS.NHibernate.Entities;
using Manh.ILS.DAO.Interfaces;
using Manh.ILS.General;

using System.Xml.Serialization;

namespace BHS.UWT.BLL
{
    class MicroHoldInventoryLocking : ServiceProcess
    {
        public MicroHoldInventoryLocking(Dictionary<string, string> Params) : base(Params)
        {
            LocalSession = new Session();

            InputFolder = SystemConfigRetrieval.GetStringSystemValue(LocalSession, "MicroHoldFolder", "Interface");
            OutputFolder = SystemConfigRetrieval.GetStringSystemValue(LocalSession, "40", "Interface");
            MicroHoldExtension = SystemConfigRetrieval.GetStringSystemValue(LocalSession, "MicroHoldExtension", "Interface");

            if (!Directory.Exists(InputFolder))
                throw new Exception("No Micro Hold Input Folder found, verify the MicroHoldFolder interface system value exists");

            if (!Directory.Exists(OutputFolder))
                throw new Exception("No Micro Hold Input Folder found, verify the MicroHoldFolder interface system value exists");

            if (string.IsNullOrEmpty(MicroHoldExtension))
                throw new Exception("No MicroHoldExtension found, verify the MicroHoldExtension interface system value exists");
            
            this._params = Params;
            Name = Params["Name"];
            Frequency = int.Parse(Params["Frequency"]);
            _maxError = int.Parse(Params["MaxError"]);
        }

        #region properties
        private static int _maxError;
        public Session LocalSession { get; set; }
        public string InputFolder { get; set; }
        public string OutputFolder { get; set; }
        public string MicroHoldExtension { get; set; }
        #endregion

        public override void Execute()
        {
            Executing = true;
            Debug.WriteLine(string.Format("BHS.UWT.BLL.MicroHoldInventoryLocking.Execute() Executing: {0}", this.Executing));

            try
            {

                using (DataHelper dataHelper = new DataHelper(LocalSession))
                {
                    foreach (FileInfo fileInfo in FindAndLockMicroholdFiles())
                    {
                        ProcessFile(fileInfo, dataHelper);
                        SetFileAsProcessed(fileInfo.FullName, Path.Combine(OutputFolder, fileInfo.Name));
                    }

                    dataHelper.Update(CommandType.StoredProcedure, "BHS_MicroHold_UpdateInventoryWithCache");
                    dataHelper.Update(CommandType.StoredProcedure, "BHS_MicroHold_LockReceipts");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("BHS.uwt.BLL.MicroHoldInventoryLocking: Catch {0} {1}", ex.Message, ex.StackTrace));
                ExceptionManager.LogException(LocalSession, ex, "BHS.UWT.BLL.MicroHoldInventoryLocking", LocalSession.UserProfile.UserName);
            }
            finally
            {
                Executing = false;
            }
        }

        private List<FileInfo> FindAndLockMicroholdFiles()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(InputFolder);
            FileInfo[] fileInfo = directoryInfo.GetFiles(string.Format("*{0}", MicroHoldExtension)).ToArray();

            List<FileInfo> newFiles = new List<FileInfo>();
            //FileInfo[] fileInfo = directoryInfo.GetFiles("*.*").Where(x => x.Extension == MicroHoldExtension).ToArray();

            foreach (FileInfo f in fileInfo)
            {
                newFiles.Add(ChangeExtension(f.FullName, ".tmp"));
            }

            return newFiles;
        }

        private void ProcessFile(FileInfo fileInfo, DataHelper dataHelper)
        {
            XmlSerializer serializer;
            FileStream loadStream;
            UWT867Message uwt867Message;
            serializer = new XmlSerializer(typeof(UWT867Message));
            loadStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            uwt867Message = (UWT867Message)serializer.Deserialize(loadStream);
            loadStream.Close();

            if (!string.IsNullOrEmpty(uwt867Message.Lpn) && string.IsNullOrEmpty(uwt867Message.StsCode))
            {
                Exception ex = UwtDebugger.BuildException(
                    "File Contained a status code but no LPN",
                    new List<string> {
                        string.Format("file name = {0}", fileInfo.Name),
                        string.Format("LPN = '{0}'", uwt867Message.Lpn),
                        string.Format("status code= '{0}'", uwt867Message.StsCode)
                    }
                );
                UwtDebugger.WriteToAuditLog(ex, LocalSession);
                return;
            }
            if (string.IsNullOrEmpty(uwt867Message.Lpn) && !string.IsNullOrEmpty(uwt867Message.StsCode))
            {
                Exception ex = UwtDebugger.BuildException(
                    "File Contained an LPN but no status code",
                    new List<string> {
                        string.Format("file name = {0}", fileInfo.Name),
                        string.Format("LPN = '{0}'", uwt867Message.Lpn),
                        string.Format("status code= '{0}'", uwt867Message.StsCode)
                    }
                );
                UwtDebugger.WriteToAuditLog(ex, LocalSession);
                return;

            }

            IDataParameter[] paramArray = new IDataParameter[6];
            string lot = uwt867Message.Lot;
            string po = uwt867Message.ReferenceID;
            string transType = uwt867Message.TransType;
            
            int i = 0;
            foreach (Detail d in uwt867Message.details.detail)
            {
                string item = d.Item;
                string poLine = "";
                i++;

                //paramArray[0] = dataHelper.BuildParameter("@ITEM", item);
                //paramArray[1] = dataHelper.BuildParameter("@LOT", lot);
                //paramArray[2] = dataHelper.BuildParameter("@PO", po);
                //paramArray[3] = dataHelper.BuildParameter("@POLINE", poLine);
                //paramArray[4] = dataHelper.BuildParameter("@TRANSTYPE", transType);
                //paramArray[5] = dataHelper.BuildParameter("@FILENAME", fileInfo.Name);

                Debug.WriteLine(string.Format("BHS.UWT.BLL.MicroHoldInventoryLocking: File {0} running stored proc 'BHS_MicroHold_PopulateCache {1}, {2}, {3}, {4}, {5}, {0}'", fileInfo.Name, item, lot, po, poLine, transType));
                //dataHelper.Update(CommandType.StoredProcedure, "BHS_MicroHold_PopulateCache", paramArray)
                dataHelper.Update(CommandType.StoredProcedure, "BHS_MicroHold_PopulateCache",
                    dataHelper.BuildParameter("@ITEM", item),
                    dataHelper.BuildParameter("@LOT", lot),
                    dataHelper.BuildParameter("@PO", po),
                    dataHelper.BuildParameter("@POLINE", poLine),
                    dataHelper.BuildParameter("@TRANSTYPE", transType),
                    dataHelper.BuildParameter("@FILENAME", fileInfo.Name),
                    dataHelper.BuildParameter("@LPN", uwt867Message.Lpn),
                    dataHelper.BuildParameter("@StsCode", uwt867Message.StsCode)
                    ); ;
            }

            //WriteProcessHistory("130", "100", string.Format("file name = {0}", fileInfo.Name), "", "", "", string.Format("Microhold file {0}.", fileInfo.Name), "BHS_MicroHold", "ILSSRV", LocalSession.DefaultWarehouse);
        }

        public FileInfo ChangeExtension(string sourceFileName, string newExtension)
        {
            string destFileName = Path.ChangeExtension(sourceFileName, newExtension);

            if (File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }
            File.Move(sourceFileName, destFileName);

            return new FileInfo(destFileName);
        }

        public void SetFileAsProcessed(string sourceFileName, string destFileName)
        {
            if (File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }
            File.Move(sourceFileName, destFileName);
            ChangeExtension(destFileName, ".processed");
        }

        public void WriteProcessHistory(string process, string action, string Identifier1, string Identifier2, string Identifier3, string Identifier4,
                                        string Message, string ProcessStamp, string User, string Warehouse)
        {

            ProcessHistory procHistory = new ProcessHistory();

            procHistory.Process = process;
            procHistory.UserStamp = User;
            procHistory.ActivityDateTime = DateTime.Now;
            procHistory.DateTimeStamp = DateTime.Now;

            procHistory.Action = action;
            procHistory.Identifier1 = Identifier1;
            procHistory.Identifier2 = Identifier2;
            procHistory.Identifier3 = Identifier3;
            procHistory.Identifier4 = Identifier4;
            procHistory.Message = Message.Length > 500 ? Message.Substring(0, 500) : Message;
            procHistory.ProcessStamp = procHistory.ProcessStamp;
            procHistory.Warehouse = Warehouse;

            IProcessHistoryDAO procHistoryDAO = (IProcessHistoryDAO)SpringNetFactory.GetObject("IProcessHistoryDAO");

            procHistoryDAO.Save(procHistory);
        }
    }


}
