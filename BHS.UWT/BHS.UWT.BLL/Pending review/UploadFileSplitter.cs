using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Diagnostics;
using System.Data;

using BHS.ProcessService;

using Manh.WMFW.General;
using Manh.WMFW.Config.BL;
using Manh.WMFW.Entities;
using Manh.WMFW.DataAccess;


namespace BHS.UWT.BLL
{
    class UploadFileSplitter : ServiceProcess
    {
        public UploadFileSplitter(Dictionary<string, string> Params)
            : base(Params)
        {
            Debug.Write("Initializing");
            session = new Session();

            UploadPendingFolder = SystemConfigRetrieval.GetStringSystemValue(session, "150", "Interface");
            FinalUploadFolder = SystemConfigRetrieval.GetStringSystemValue(session, "BhsFinalUploadFolder", "Interface");
            OutputFolder = SystemConfigRetrieval.GetStringSystemValue(session, "40", "Interface");

            if (!Directory.Exists(UploadPendingFolder))
                throw new Exception("Upload Folder not found, verify the upload folder configured in interface system value exists");

            if (!Directory.Exists(FinalUploadFolder))
                throw new Exception("Final Upload Folder not found, verify the Final upload folder configured in interface system value exists");

            if (!Directory.Exists(OutputFolder))
                throw new Exception("Output Folder not found, verify the output folder configured in interface system value exists");

            ParsingValues = GetLineParsingValues();
            CompanyPrefix = GetCompanyPrefixMaping();

            this._params = Params;
            Name = Params["Name"];
            Frequency = int.Parse(Params["Frequency"]);
            _maxError = int.Parse(Params["MaxError"]);
            Debug.Write("Initialized");
        }

        #region properties
        public Session session { get; set; }
        public string UploadPendingFolder { get; set; }
        public string FinalUploadFolder { get; set; }
        public string OutputFolder { get; set; }
        public Dictionary<string, int> ParsingValues;
        public Dictionary<string, List<string>> NewFiles;
        public Dictionary<string, string> CompanyPrefix;
        private static int _maxError;
        #endregion properties

        public override void Execute()
        {
            Executing = true;
            debug("EXECUTING");
            FileInfo[] fileInfo;
            DirectoryInfo directoryInfo;

            try
            {
                directoryInfo = new DirectoryInfo(UploadPendingFolder);
                fileInfo = directoryInfo.GetFiles("*");

                foreach (FileInfo f in fileInfo)
                {
                    string[] fileLines = File.ReadAllLines(f.FullName);
                    string company;
                    debug(string.Format("Found File {0}", f.Name));

                    
                    NewFiles = GetNewFilesDictionary();
                    //debug(string.Format("File {0} appears to be a shipment file", f.Name));
                    File.Move(f.FullName, Path.Combine(OutputFolder, f.Name));

                    foreach (string line in fileLines)
                    {
                        company = parseCompanyFromFileLine(line);
                        debug(string.Format("Parsed Company {0} with prefix {1} from line {2}", company, CompanyPrefix[company], line));
                        if (string.IsNullOrEmpty(CompanyPrefix[company]))
                            debug("Not writing to File");
                        else
                            NewFiles[CompanyPrefix[company]].Add(line);
                    }

                    WriteNewFiles(NewFiles, f);
                    debug(string.Format("Done with File {0}", f.Name));
                }

                debug(string.Format("No More file to process"));
                
            }
            
                
            catch (Exception ex)
            {   
                debug(string.Format("Catch {0} {1}", ex.Message, ex.StackTrace));
                ExceptionManager.LogException(session, ex, this);
            }
            finally
            {
                Executing = false;
                debug("DONE EXECUTING");
            }
        }
        private Dictionary<string, List<string>> GetNewFilesDictionary()
        {
            Dictionary<string, List<string>> PrefixAndData = new Dictionary<string, List<string>>();

            foreach (CompanyBE company in CompanyRetrieval.RtrvAllCompanyBEs(session))
            {
                if (!string.IsNullOrEmpty(company.UserDef3) && !PrefixAndData.Keys.Contains(company.UserDef3))
                {
                    PrefixAndData.Add(company.UserDef3, new List<string>());
                    debug(string.Format(".GetNewFilesDictionary added Company {0} with prefix '{1}'", company.Company, company.UserDef3));
                }
                else
                {
                    debug(string.Format(".GetNewFilesDictionary NOT adding {0} with prefix '{1}'", company.Company, company.UserDef3));
                }
            }

            return PrefixAndData;
        }
        private Dictionary<string, string> GetCompanyPrefixMaping()
        {

            Dictionary<string, string> Companies = new Dictionary<string, string>();

            foreach (CompanyBE company in CompanyRetrieval.RtrvAllCompanyBEs(session))
            {
                Companies.Add(company.Company, company.UserDef3);
                debug(string.Format(".GetCompanyPrefixMaping added company {0} with prefix {1}", company.Company, company.UserDef3));
            }

            return Companies;
        }
        private Dictionary<string, int> GetLineParsingValues()
        {
            Dictionary<string, int> ParsingValues = new Dictionary<string, int>();
            using (DataHelper dataHelper = new DataHelper(session))
            {
                DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, "BHS_UploadFileSplitter_GetParsingValues");

                foreach (DataRow r in table.Rows)
                {
                    ParsingValues.Add(DataManager.GetString(r, "MAP_NAME"), DataManager.GetInt(r, "CompanyPosition"));
                }
            }

            return ParsingValues;
        }
        private string parseCompanyFromFileLine(string line)
        {
            int ColumnContainingCompany = ParsingValues[line.Substring(0, line.IndexOf('|'))];

            //Loop through charaters in my line to find the start of my company.
            int i = 0;
            int currenColumnNum = 1;

            foreach (char c in line)
            {
                if (c == '|')
                    currenColumnNum++;
                i++;
                if (currenColumnNum == ColumnContainingCompany)
                    break;
            }

            //Use the index found in the loop to parse out company
            string company = line.Substring(i);  //remove characters before
            company = company.Substring(0, company.IndexOf('|'));  //Remove characters after.

            return company;
        }
        private void WriteNewFiles(Dictionary<string, List<string>> NewFiles, FileInfo OriginalFileName)
        {
            //foreach (List<string> newFile in NewFiles.Values)
            foreach (KeyValuePair<string,List<string>> newFile in NewFiles)
            {
                if (newFile.Value.Count > 0)
                {
                    //File.WriteAllLines(ufsc.FilePath(OriginalFileName, FinalUploadFolder), newFile.ToArray());
                    File.WriteAllLines(Path.Combine(FinalUploadFolder, string.Format("{0}-{1}",newFile.Key, OriginalFileName)), newFile.Value.ToArray());
                    debug(string.Format("writing a file for prefix {0} with {1} lines",
                        newFile.Value,
                        newFile.Value.Count)
                        );
                }
                else
                {
                    debug(string.Format("not writing a file for prefix {0}", newFile.Key));
                }
            }
        }
        private void debug(string message)
        {
            Debug.WriteLine(string.Format("{0}: {1}", this, message));
        }
    }
}
