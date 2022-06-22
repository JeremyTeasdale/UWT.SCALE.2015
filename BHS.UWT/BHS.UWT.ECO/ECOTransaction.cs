using System;
using System.Data;

namespace BHS.UWT.ECO
{
    class ECOTransaction
    {
        #region Properties
        public string InternalNum { get; set; }

        public string ReferenceNum { get; set; }

        public string Status { get; set; }

        public DataTable HeaderTable { get; set; }

        public DataTable DetailsTable { get; set; }

        public string Url { get; set; }

        public string xFunctionsKey { get; set; }

        public string XmlContent { get; set; }

        public string DocumentType { get; set; }

        public string Operation { get; set; }

        public string RawResponseJson { get; set; }

        public bool IsError { get; set; } = false;

        public string ErrorMsg { get; set; }

        public string XMLResult { get; set; }
        #endregion Properties

    }
}
