using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Serialization;

namespace BHS.UWT.BLL
{

    [XmlRoot("UWT867Message"), Serializable]
    public class UWT867Message
    {
        public string TransType {
            get
            {
                if (string.IsNullOrEmpty(ReferenceID))
                    return "Batch Release";
                return "FDA Release";
            }
        }
        
        [XmlElement]
        public string EdiControlNumber { get; set; }

        [XmlElement]
        public string TransactionSetPurposeCode { get; set; }

        [XmlElement]
        public string Reference { get; set; }
            
        [XmlElement]
        public string TransactionDate { get; set; }

        [XmlElement]
        public string ReleaseDate { get; set; }

        [XmlElement]
        public string ReleaseTime { get; set; }

        [XmlElement]
        public string ReferenceID { get; set; }

        [XmlElement]
        public string Lot { get; set; }

        [XmlElement]
        public string Lot2 { get; set; }

        [XmlElement]
        public string NumberOfDetails { get; set; }

        [XmlElement("Details")]
        public Details details { get; set; }
    }

    [XmlRoot("Details"), Serializable]
    public class Details
    {
        [XmlElement("Detail")]
        public List<Detail> detail { get; set; }
    }

    [XmlRoot("Detail"), Serializable]
    public class Detail
    {
        [XmlElement]
        public string ProductTransferTypeCode { get; set; }

        [XmlElement]
        public string Item { get; set; }

        [XmlElement]
        public string QtyQualifier { get; set; }

        [XmlElement]
        public string Qty { get; set; }

        [XmlElement]
        public string UOM { get; set; }
    }
    

    ///////////////////////////////////
    //[XmlRoot("Transactions"), Serializable]
    //public class Transactions
    //{
    //    [XmlElement("Transaction")]
    //    public List<Transaction> transactions { get; set; }
    //}

    //public class Transaction
    //{
    //    [XmlElement]
    //    public string Item { get; set; }
    //    [XmlElement]
    //    public string Lot { get; set; }
    //    [XmlElement]
    //    public string Po { get; set; }
    //    [XmlElement]
    //    public string PoLine { get; set; }
    //    [XmlElement]
    //    public string TransType { get; set; }
    //}
}
