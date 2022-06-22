using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BHS.UWT.BLL
{
    public class MicroHoldParsingValues
    {
        public int MyProperty { get; set; }
        public string itemKey { get; set; }
        public string itemIdentifier { get; set; }
        public int itemIndex { get; set; }

        public string lotKey { get; set; }
        public string lotIdentifier { get; set; }
        public int lotIndex { get; set; }

        public string PoKey { get; set; }
        public string PoIdentifier { get; set; }
        public int PoIndex { get; set; }

        public string PoLineKey { get; set; }
        public string PoLineIdentifier { get; set; }
        public int PoLineIndex { get; set; }

        public MicroHoldTransType transType { get; set; }
        public string splitOn { get; set; }

        public MicroHoldParsingValues(string EDICode)
        {
            splitOn = "";
            switch (EDICode)
            {
                case "856":  //Standard hold
                    transType = MicroHoldTransType.Hold;
                    itemKey = "LIN";
                    itemIndex = 3;
                    PoKey = "PRF";
                    PoIndex = 1;
                    PoLineKey = "REF";
                    PoLineIdentifier = "P7";
                    PoLineIndex = 2;
                    splitOn = "HL";
                    break;
                case "867":  //Standard release
                    transType = MicroHoldTransType.Release;
                    itemKey = "LIN";
                    itemIndex = 3;
                    lotKey = "REF";
                    lotIdentifier = "BT";
                    lotIndex = 2;
                    break;
                case "944":  //Goods Receipt - Unknow requirements
                    transType = MicroHoldTransType.Ignore;
                    break;
                case "947":  //Marerial movement - Uknow requirements
                    transType = MicroHoldTransType.Ignore;
                    break;
                default:
                    transType = MicroHoldTransType.Ignore;
                    break;
            }

        }
    }
}
