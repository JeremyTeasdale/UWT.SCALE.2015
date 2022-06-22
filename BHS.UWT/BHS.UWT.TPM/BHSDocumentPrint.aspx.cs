using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BHS.UWT.TPM.Data;

namespace BHS.UWT.TPM
{

    public partial class BHSDocumentPrint : System.Web.UI.Page
    {
        private GeneralRepository _generalRepository = null;
         public GeneralRepository GeneralRepository
        {
            get
            {
                if(_generalRepository == null)
                    _generalRepository = new GeneralRepository();

                return _generalRepository;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {       
            string fileName;
            string docType = Request["BHSType"].ToString();
            string shipment = Request["BHSShipment"].ToString();
            
            if (docType == "BOL")
                fileName = string.Format(GeneralRepository.GetTPMBOLName, shipment);
            else
                fileName = string.Format(GeneralRepository.GetTPMPackListName, shipment);
            
            DisplayPDF(fileName);
           
        }

        public void DisplayPDF(string reportName)
        {
            try
            {
                string filePath = Path.Combine(GeneralRepository.GetTPMFileDirectory, reportName);
                System.Diagnostics.Debug.WriteLine(string.Format("BHS.UWT.TPM.BHSDocumentPrint -> {0}", filePath));

                if (File.Exists(filePath))
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                    {
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.Buffer = true;

                        byte[] buffer = new byte[(int)fileStream.Length];
                        fileStream.Read(buffer, 0, (int)fileStream.Length);



                        Response.ContentType = "application/pdf";
                        Response.AddHeader("Content-Length", fileStream.Length.ToString());
                        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.pdf", reportName));

                        Response.BinaryWrite(buffer);
                        Response.Flush();
                        Response.End();
                    }

                }
                else
                {
                    Response.Write("<SCRIPT LANGUAGE=\"JavaScript\">alert(\"Document Not Found\")</SCRIPT>");
                }
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("BHSShipmentResults.DisplayPDF -> ReportName {0}, Ex: {1}", reportName, ex));
                Response.Write("<SCRIPT LANGUAGE=\"JavaScript\">alert(\"File in use\")</SCRIPT>");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("BHSShipmentResults.DisplayPDF -> ReportName {0}, Ex: {1}", reportName, ex));
                Response.Write("<SCRIPT LANGUAGE=\"JavaScript\">alert(\"Technical Error\")</SCRIPT>");
            }
        }
    }
}
