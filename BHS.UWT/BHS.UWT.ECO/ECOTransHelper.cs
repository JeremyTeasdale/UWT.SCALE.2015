using Manh.WMFW.DataAccess;
using Manh.WMFW.General;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace BHS.UWT.ECO
{
    class ECOTransHelper
    {
        private static Session LocalSession = new Session();

        public static List<ECOTransaction> BuildECOTransactions(DataTable headerTable, string detailsIdentifier)
        {
            List<ECOTransaction> ecoTransactions = new List<ECOTransaction>();
            foreach (DataRow headerRow in headerTable.Rows)
            {
                string referenceNum = Utilities.GetStringFromRow(headerRow, "ReferenceNum");
                string internalNum = referenceNum.Substring(referenceNum.LastIndexOf("-") + 1);
                string status = Utilities.GetStringFromRow(headerRow, "Status") ?? Utilities.GetStringFromRow(headerRow, "TransactionCode");
                Utilities.WriteDebug(string.Format("Shipment : {0} , {1} , Status {2}", internalNum, referenceNum, status));

                ECOTransaction ecoTran = new ECOTransaction();
                ecoTran.InternalNum = internalNum;
                ecoTran.ReferenceNum = referenceNum;
                ecoTran.Status = status;

                DataTable detailsData = new DataTable();
                if (!string.IsNullOrEmpty(detailsIdentifier))
                {
                    // Consider getting all at once rather than by shipment
                    detailsData = GetDetailData("BHS_ECO_GetShipmentDetailsData", detailsIdentifier, internalNum);
                }

                DataTable headerClone = headerTable.Clone();
                headerClone.ImportRow(headerRow);

                ecoTran.HeaderTable = headerClone;
                ecoTran.DetailsTable = detailsData;

                string XmlContent = Utilities.DataTablesToXml(ecoTran);
                ecoTran.XmlContent = XmlContent;

                ecoTransactions.Add(ecoTran);
            }

            Utilities.WriteDebug(string.Format("ecoShipments Count : {0}", ecoTransactions.Count));

            return ecoTransactions;
        }

        public static List<ECOTransaction> BuildECOInventoryTransactions(DataTable inventoryTable)
        {
            List<ECOTransaction> ecoTransactions = new List<ECOTransaction>();
            foreach (DataRow headerRow in inventoryTable.Rows)
            {
                ECOTransaction ecoTran = new ECOTransaction();

                DataTable inventoryClone = inventoryTable.Clone();
                inventoryClone.ImportRow(headerRow);

                ecoTran.HeaderTable = inventoryClone;

                string XmlContent = Utilities.DataTablesToXml(ecoTran);
                ecoTran.XmlContent = XmlContent;

                ecoTransactions.Add(ecoTran);
            }

            Utilities.WriteDebug(string.Format("Inventory Records Count : {0}", ecoTransactions.Count));

            return ecoTransactions;
        }

        public static DataTable GetHeaderData(string storedProcedureName, string tableName)
        {
            using (DataHelper dataHelper = new DataHelper(LocalSession))
            {
                DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, storedProcedureName);
                table.TableName = string.IsNullOrEmpty(tableName) ? "HeaderTable" : tableName;
                Utilities.WriteDebug(string.Format("Rows: {0}", table.Rows.Count));
                return table;
            }
        }

        private static DataTable GetDetailData(string storedProcedureName, string internalNumKey, string internalNumValue)
        {
            using (DataHelper dataHelper = new DataHelper(LocalSession))
            {
                try
                {
                    int integerInternalNumValue = Int32.Parse(internalNumValue);
                    DataTable table = dataHelper.GetTable(CommandType.StoredProcedure, storedProcedureName, dataHelper.BuildParameter(internalNumKey, integerInternalNumValue));
                    table.TableName = "Detail";
                    Utilities.WriteDebug(string.Format("Detail Rows: {0}", table.Rows.Count));
                    return table;
                }
                catch (Exception ex)
                {
                    Utilities.WriteDebug(string.Format("Something went wrong: {0}", ex));
                    return null;
                }
            }
        }

        //private static HttpClient httpClient = new HttpClient();
        private static TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, -1);
        public static async Task<string> SendXmlToECO(ECOTransaction ecoTran)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = InfiniteTimeSpan;
                // Set for global httpClient and re-use
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string content = ecoTran.XmlContent;
                string contentSnippet = content.Replace(Environment.NewLine, string.Empty);
                Utilities.WriteDebug(string.Format("{0} {1} {2}", ecoTran.Operation, ecoTran.Url, ecoTran.xFunctionsKey));
                Utilities.WriteDebug(string.Format("{0}", contentSnippet));
                httpClient.DefaultRequestHeaders.Add("x-functions-key", ecoTran.xFunctionsKey);

                StringContent httpContent = new StringContent(content, Encoding.UTF8, "application/xml");

                HttpResponseMessage response = await httpClient.PostAsync(ecoTran.Url, httpContent);
                string responseJson = await response.Content.ReadAsStringAsync();
                Utilities.WriteDebug(string.Format("responseJson : {0}", responseJson));

                if (!response.IsSuccessStatusCode)
                {
                    ecoTran.IsError = true;
                    ecoTran.ErrorMsg = response.ReasonPhrase;

                    Utilities.WriteDebug(string.Format("ResponseCode : {0} {1}", response.StatusCode.ToString(), response.ReasonPhrase));
                }

                AddResponseToECOTrans(ecoTran, responseJson);

                WriteECOTransactionHistory(ecoTran, responseJson);

                return responseJson;
            }
        }

        public static async Task<string> SendInventoryXmlToECO(ECOTransaction ecoTran)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = InfiniteTimeSpan;
                // Set for global httpClient and re-use
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string content = ecoTran.XmlContent;
                string contentSnippet = content.Replace(Environment.NewLine, string.Empty);
                Utilities.WriteDebug(string.Format("{0} {1} {2}", ecoTran.Operation, ecoTran.Url, ecoTran.xFunctionsKey));
                Utilities.WriteDebug(string.Format("{0}", contentSnippet));
                httpClient.DefaultRequestHeaders.Add("x-functions-key", ecoTran.xFunctionsKey);

                StringContent httpContent = new StringContent(content, Encoding.UTF8, "application/xml");

                HttpResponseMessage response = await httpClient.PostAsync(ecoTran.Url, httpContent);
                string responseJson = await response.Content.ReadAsStringAsync();
                Utilities.WriteDebug(string.Format("responseJson : {0}", responseJson));

              

                if (!response.IsSuccessStatusCode)
                {
                    ecoTran.IsError = true;
                    ecoTran.ErrorMsg = response.ReasonPhrase;

                    Utilities.WriteDebug(string.Format("ResponseCode : {0} {1}", response.StatusCode.ToString(), response.ReasonPhrase));
                }

                AddResponseToECOTrans(ecoTran, responseJson);

                WriteECOTransactionHistory(ecoTran, responseJson);

                return responseJson;
            }
        }

        public static void WriteECOTransactionHistory(ECOTransaction ecoTran, string data)
        {
            using (DataHelper dataHelper = new DataHelper(LocalSession))
            {
                List<IDataParameter> paramArray = new List<IDataParameter> {
                    dataHelper.BuildParameter("@InternalNum", ecoTran.InternalNum),
                    dataHelper.BuildParameter("@ReferenceNum", ecoTran.ReferenceNum),
                    dataHelper.BuildParameter("@Status", ecoTran.Status),
                    dataHelper.BuildParameter("@Operation", ecoTran.Operation),
                    dataHelper.BuildParameter("@IsError", ecoTran.IsError ? "Y" : null),
                    dataHelper.BuildParameter("@ErrorMsg", ecoTran.ErrorMsg),
                    dataHelper.BuildParameter("@Data", data),
                    dataHelper.BuildParameter("@DocumentType", ecoTran.DocumentType)
                };

                string storedProcName = "BHS_ECO_WriteTransactionHistory";

                int insertedRowsCount = dataHelper.Update(CommandType.StoredProcedure, storedProcName, paramArray.ToArray());
            }
        }

        private static void AddResponseToECOTrans(ECOTransaction ecoTran, string responseJson)
        {
            try
            {
                ecoTran.RawResponseJson = responseJson;

                // SUCCESS : { "ErrorMsg": null, "IsError": false, "XMLResult": null }
                // ERROR : { "id": "56c106dc-2078-4d79-a1ba-ffa8605d49a7", "requestId": "359698fa-b204-44a1-b36b-050e7a974fd3",
                // "statusCode": 500, "errorCode": 0, "message": "An error has occurred. For more information, please check the logs for error ID 56c106dc-2078-4d79-a1ba-ffa8605d49a7" }
                if (!string.IsNullOrEmpty(responseJson))
                {
                    JObject responseObj = Utilities.JsonStringToObject(responseJson);
                    ecoTran.IsError = (bool)(responseObj["IsError"] ?? false);
                    ecoTran.ErrorMsg = (string)(responseObj["ErrorMsg"] ?? responseObj["message"]); // get from actual ECO Json error response if exists
                    ecoTran.XMLResult = (string)responseObj["XMLResult"];

                    if (!string.IsNullOrEmpty(ecoTran.ErrorMsg))
                    {
                        ecoTran.IsError = true;
                    }
                }                
            }
            catch (Exception ex)
            {
                Utilities.WriteDebug(string.Format("JObject parse ExceptionCaught {0} {1} responseJson : {2}", ex.Message, ex.StackTrace, responseJson));
            }
        }

    }
}
