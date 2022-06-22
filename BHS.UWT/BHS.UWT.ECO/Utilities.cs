using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.IO;
using Manh.WMFW.DataAccess;
using System.Data;
using Manh.WMFW.General;

using System.Net.Http;
using System.Net;
using System.Xml;
using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using BHS.ProcessService.Utils;
using Manh.WMW.Configs.General;
using Manh.ILS.NHibernate.Entities;
using Manh.ILS.Configs.Interfaces;
using Manh.ILS.General;
using Manh.WMW.General;

namespace BHS.UWT.ECO
{
    class Utilities
    {
        private static Session LocalSession = new Session();

        public static string DataTablesToXml(ECOTransaction ecoShipment)
        {
            DataTable header = ecoShipment.HeaderTable;
            DataTable details = ecoShipment.DetailsTable;

            // Create Header DataSet from header DataTable and get XML
            DataSet headerSet = new DataSet("HeaderRoot");
            headerSet.Tables.Add(header.Copy()); // Copy() fixes "DataTable already belongs to another DataSet"
            string headerXmlStr = headerSet.GetXml();//headerSet.GetXml();

            // Convert Header XML to XmlDocument to add Details inner Fragment
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(headerXmlStr);

            // Create Details DataSet from detail DataTable and get XML
            if ((details != null && !DataManager.IsEmpty(details)) || header.TableName == "Shipment")
            {
                DataSet detailsSet = new DataSet("Details");
                detailsSet.Tables.Add(details.Copy()); // Copy() fixes "DataTable already belongs to another DataSet"
                string detailsXmlStr = detailsSet.GetXml();

                XmlDocumentFragment xfrag = xdoc.CreateDocumentFragment();
                xfrag.InnerXml = detailsXmlStr;
                xdoc.DocumentElement.FirstChild.AppendChild(xfrag);
            }

            XElement xElement = XElement.Load(new XmlNodeReader(xdoc));
            XElement xChild = xElement.Descendants().First(); // Remove Placeholder "HeaderRoot" level
            string xmlString = xChild.ToString();
            //WriteDebug(string.Format("FULL shipmentXmlString : {0}", xmlString));

            return xmlString;
        }

        public static JObject JsonStringToObject(string json)
        {
            JObject jsonObj = JObject.Parse(json);

            return jsonObj;
        }

        public static async Task<byte[]> ReadAllFileAsync(string filePath)
        {
            byte[] result;
            using (FileStream SourceStream = File.Open(filePath, FileMode.Open))
            {
                result = new byte[SourceStream.Length];
                await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);

                return result;
            }
        }
 
        public static void WriteDebug(string text, [CallerFilePath]string callerFilePath = null, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            string callerTypeName = Path.GetFileNameWithoutExtension(callerFilePath);
            string dirName = new DirectoryInfo(@Path.GetDirectoryName(callerFilePath)).Name;

            string debugString = string.Format("{0} : {1} : {2} : {3}", dirName + "." + callerTypeName, member, line, text);
            //Debug.WriteLine(debugString);
            BHS.ProcessService.Utils.DebugManager.WriteLn(debugString);
        }

        public static string FormatException(Exception ex)
        {
            return string.Format("ExceptionCaught {0} {1} {2}", ex.Message, Environment.NewLine, ex.StackTrace);
        }

        public static void WriteException(Exception ex, params object[] parameters)
        {
            ExceptionManager.LogException(LocalSession, ex, parameters);
        }

        /// <summary>
        /// Safe wrapper for base DataManager.GetString(), returns empty string if no column
        /// </summary>
        /// <returns></returns>
        public static string GetStringFromRow(DataRow row, string colName)
        {
            string value = null;

            if (row.Table.Columns.Contains(colName))
            {
                value = DataManager.GetString(row, colName);
            }

            return value;
        }

        public static Tuple<string, string> GetUrlAndxFunctionsKey(string identifier)
        {
            //GenericConfigDetailRetrieval _genericRetrieval = new GenericConfigDetailRetrieval();
            //GenericConfigDetail ecoEndpointValues = SpringNetFactory.GetObject<IGenericConfigDetailRetrieval>().GetGenericConfigDetail("ECO_ENDPOINTS", identifier);
            DataRow ecoEndpointValues = GenericConfigDetailRetrieval.GetGenericConfig(LocalSession, "ECO_ENDPOINTS", identifier);
            string url = null;
            string xFunctionsKey = null;

            if (ecoEndpointValues != null)
            {
                url = GetStringFromRow(ecoEndpointValues, "SYS1VALUE"); //ecoEndpointValues.Sys1value;
                xFunctionsKey = GetStringFromRow(ecoEndpointValues, "SYS2VALUE"); //ecoEndpointValues.Sys2value;
            }

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(xFunctionsKey))
            {
                //System.Diagnostics.Debug.WriteLine(url);
                //System.Diagnostics.Debug.WriteLine(xFunctionsKey);
                throw new Exception(string.Format("ECO Service : Must have Url and xFunctionsKey defined for {0} Action", identifier));
            }
            Tuple<string, string> urlAndxFunctionsKey = new Tuple<string, string>(url, xFunctionsKey);
            return urlAndxFunctionsKey;
        }

        public static string GetECODocumentsDirectory()
        {
            string ecoDocsDir = GenericConfigDetailRetrieval.GetGenericSystemValue1(LocalSession, "ECO_VALUES", "DOCS_DIR");
            if (string.IsNullOrEmpty(ecoDocsDir))
            {
                throw new Exception("ECO Service : Could not find ECO Documents Directory");
            }

            return ecoDocsDir;
        }
    }
}
