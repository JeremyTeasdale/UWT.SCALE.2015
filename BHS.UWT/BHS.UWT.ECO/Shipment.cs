using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

using Manh.WMFW.General;
using Manh.WMFW.DataAccess;
using Manh.ILS.DAO.Interfaces;
using Manh.ILS.NHibernate.Entities;
using Manh.ILS.General;
using Manh.WMFW.Config.BL;
using Manh.WMW.Configs.General;

using BHS.ProcessService;

using System.Data;
using Manh.WMW.General;
using System.IO;
using System.Xml.Linq;

namespace BHS.UWT.ECO
{
    public class Shipment : ServiceProcess
    {        
        public Shipment(Dictionary<string, string> Params) : base(Params)
        {
        }

        public string Message { get; set; }

        private string EcoDocsDir { get; set; }

        public async override void Execute()
        {
            Executing = true;
            DateTime startTime = DateTime.Now;
            Utilities.WriteDebug(string.Format("ECO Shipment UWT Debug, {0}", "test"));

            try
            {
                await CreateChangeShipments();
                await SendStatusAdvances();
                await SendDocuments();


                Executing = false;
            }
            catch (Exception ex)
            {
                Utilities.WriteDebug(Utilities.FormatException(ex));
                Utilities.WriteException(ex, this.GetType().FullName);
            }
            finally
            {
                Executing = false;
                Utilities.WriteDebug("DONE EXECUTING");
            }

            DateTime endTime = DateTime.Now;
            Console.WriteLine(endTime.Subtract(startTime).TotalMinutes);
        }

        private async Task CreateChangeShipments()
        {
            DataTable shipmentHeaders = ECOTransHelper.GetHeaderData("BHS_ECO_GetShipmentHeaderData", "Shipment");
            if (DataManager.IsEmpty(shipmentHeaders))
            {
                return;
            }

            Tuple<string, string> createChangeURLKey = Utilities.GetUrlAndxFunctionsKey("SHIPMENT_CREATECHANGE");
            Tuple<string, string> confirmURLKey = Utilities.GetUrlAndxFunctionsKey("SHIPMENT_CONFIRM");

            List <ECOTransaction> ecoShipments = ECOTransHelper.BuildECOTransactions(shipmentHeaders, "@INTERNAL_SHIPMENT_NUM");

            //List<Task<string>> ecoTasks = new List<Task<string>>();

            foreach (ECOTransaction ecoTran in ecoShipments)
            {
                if (ecoTran.Status == "900")
                {
                    ecoTran.Url = confirmURLKey.Item1;
                    ecoTran.xFunctionsKey = confirmURLKey.Item2;
                    ecoTran.Operation = "ShipmentConfirm";
                }
                else
                {
                    ecoTran.Url = createChangeURLKey.Item1;
                    ecoTran.xFunctionsKey = createChangeURLKey.Item2;
                    ecoTran.Operation = "CreateChange";
                }

                string response = await ECOTransHelper.SendXmlToECO(ecoTran);
                //ecoTasks.Add(ECOTransHelper.SendXmlToECO(ecoTran));
            }

            //string[] completedTasks = await Task.WhenAll(ecoTasks);
        }

        private async Task SendStatusAdvances()
        {
            DataTable statusAdvances = ECOTransHelper.GetHeaderData("BHS_ECO_GetShipmentStatusAdvance", "StatusAdvance");
            if (DataManager.IsEmpty(statusAdvances))
            {
                return;
            }

            Tuple<string, string> urlAndxFunctionsKey = Utilities.GetUrlAndxFunctionsKey("STATUS_ADVANCE");

            List<ECOTransaction> ecoStatusAdvances = ECOTransHelper.BuildECOTransactions(statusAdvances, null);

            //List<Task<string>> ecoTasks = new List<Task<string>>();

            foreach (ECOTransaction ecoTran in ecoStatusAdvances)
            {
                ecoTran.Url = urlAndxFunctionsKey.Item1;
                ecoTran.xFunctionsKey = urlAndxFunctionsKey.Item2;
                ecoTran.Operation = "StatusAdvance";

                string response = await ECOTransHelper.SendXmlToECO(ecoTran);
                //ecoTasks.Add(ECOTransHelper.SendXmlToECO(ecoTran));
            }

            //string[] completedTasks = await Task.WhenAll(ecoTasks);
        }

        private async Task SendDocuments()
        {
            DataTable shipmentDocuments = ECOTransHelper.GetHeaderData("BHS_ECO_GetShipmentDocuments", "Document");
            if (DataManager.IsEmpty(shipmentDocuments))
            {
                return;
            }

            EcoDocsDir = Utilities.GetECODocumentsDirectory();

            Tuple<string, string> urlAndxFunctionsKey = Utilities.GetUrlAndxFunctionsKey("DOCUMENT");

            List<Task<Tuple<string, string, string>>> documentsTaks = new List<Task<Tuple<string, string, string>>>();
            foreach (DataRow documentRow in shipmentDocuments.Rows)
            {
                //string documentXml = GetDocumentXml(documentRow);
                documentsTaks.Add(GetDocumentXmlAsync(documentRow));
            }

            Tuple<string, string, string>[] completedDocumentTasks = await Task.WhenAll(documentsTaks);

            //List<Task<string>> ecoTasks = new List<Task<string>>();

            foreach (Tuple<string, string, string> documentData in completedDocumentTasks)
            {
                string xmlContent = documentData.Item2;
                ECOTransaction ecoDocTrans = new ECOTransaction()
                {
                    DocumentType = documentData.Item1,
                    XmlContent = xmlContent,
                    ReferenceNum = documentData.Item3,
                    Operation = "DOCUMENT",
                    Url = urlAndxFunctionsKey.Item1,
                    xFunctionsKey = urlAndxFunctionsKey.Item2
                };

                if (string.IsNullOrEmpty(xmlContent))
                {
                    ecoDocTrans.IsError = true;
                    ecoDocTrans.ErrorMsg = "Error retrieving document data, or document does not exist.";
                    ECOTransHelper.WriteECOTransactionHistory(ecoDocTrans, null);
                    continue;
                }

                string response = await ECOTransHelper.SendXmlToECO(ecoDocTrans);
                //ecoTasks.Add(ECOTransHelper.SendXmlToECO(ecoDocTrans));
            }

            //string[] completedTasks = await Task.WhenAll(ecoTasks);
        }


        private Tuple<string, string> GetDocumentXml(DataRow documentRow)
        {
            string documentType = Utilities.GetStringFromRow(documentRow, "documentType");
            string fileName = Utilities.GetStringFromRow(documentRow, "fileName");

            string filePath = Path.Combine(EcoDocsDir, fileName);


            byte[] pdfBytes = File.ReadAllBytes(filePath);
            string fileData = Convert.ToBase64String(pdfBytes);

            string xmlStr = GenerateDocumentXml(documentRow, fileData);

            Tuple<string, string> documentData = new Tuple<string, string>(documentType, xmlStr);

            return documentData;
        }

        /// <summary>
        /// Gets documentXml from passed docDataRow and returns Tuple of Xml and DocType
        /// </summary>
        private async Task<Tuple<string, string, string>> GetDocumentXmlAsync(DataRow documentRow)
        {
            string documentType = Utilities.GetStringFromRow(documentRow, "DocumentType");
            string fileName = Utilities.GetStringFromRow(documentRow, "FileName");
            string referenceNum = Utilities.GetStringFromRow(documentRow, "ReferenceNum");

            string filePath = Path.Combine(EcoDocsDir, fileName);

            string xmlStr = null;
            if (File.Exists(filePath))
            {
                byte[] pdfBytes = await Utilities.ReadAllFileAsync(filePath);
                string fileData = Convert.ToBase64String(pdfBytes);

                xmlStr = GenerateDocumentXml(documentRow, fileData);
            }
            else
            {
                Utilities.WriteDebug(string.Format("File does not exist : {0}", filePath));
            }

            Tuple<string, string, string> documentData = new Tuple<string, string, string>(documentType, xmlStr, referenceNum);

            return documentData;
        }

        public string GenerateDocumentXml(DataRow documentRow, string fileData)
        {
            string documentType = Utilities.GetStringFromRow(documentRow, "DocumentType");
            string documentGroup = Utilities.GetStringFromRow(documentRow, "DocumentGroup");
            string businessTransactionType = Utilities.GetStringFromRow(documentRow, "BusinessTransactionType");
            string referenceNum = Utilities.GetStringFromRow(documentRow, "ReferenceNum");
            string fileName = Utilities.GetStringFromRow(documentRow, "FileName");

            XElement shipmentDocXml = new XElement("Document",
                new XElement("DocumentType", documentType),
                new XElement("DocumentGroup", documentGroup),
                new XElement("BusinessTransactionType", businessTransactionType),
                new XElement("ReferenceNum", referenceNum),
                new XElement("FileName", fileName),
                new XElement("FileData", fileData)
            );

            string xmlStr = shipmentDocXml.ToString();
            // Utilities.WriteDebug(string.Format("DocumentXML : {0}", xmlStr));

            return xmlStr;
        }

    }
}
