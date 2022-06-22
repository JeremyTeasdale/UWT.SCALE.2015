//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace BHS.UWT.BLL
//{
//    public class MicroHoldTransDetail
//    {
//        public IList<String> Lines { get; set; }
//        public MicroHoldParsingValues parsingValues;
//        public bool ignoreRemainingDetails;
//        public bool ignoreDetail;
//        public bool header;
//        private string po;

//        public MicroHoldTransDetail(string line, MicroHoldParsingValues parsingValue)
//        {
//            initialize(line, parsingValue);
//        }

//        public MicroHoldTransDetail(string line, MicroHoldParsingValues parsingValue, string Po)
//        {
//            if (!string.IsNullOrEmpty(Po))
//                this.po = Po;
//            initialize(line, parsingValue);
//        }

//        private void initialize(string line, MicroHoldParsingValues parsingValue)
//        {
//            ignoreRemainingDetails = false;
//            ignoreDetail = false;
//            header = false;

//            if (line.StartsWith("*856"))
//                ignoreDetail = true;
//            if (line.StartsWith("*1*"))
//            {
//                ignoreDetail = true;
//                header = true;
//                if (!line.Contains("Product on Quality Hold"))
//                {
//                    ignoreRemainingDetails = true;
//                }
//            }
//            this.parsingValues = parsingValue;
//            this.Lines = line.Split(new string[] { "~" }, StringSplitOptions.None).ToList();
//        }


//        public string Item
//        {
//            get
//            {
//                return Value(parsingValues.itemKey, parsingValues.itemIdentifier, parsingValues.itemIndex).Trim();
//                //return Value("LIN", 3).Trim();
//            }
//        }

//        public string Lot
//        {
//            get
//            {
//                return Value(parsingValues.lotKey, parsingValues.lotIdentifier, parsingValues.lotIndex).Trim();
//            }
//        }

//        public string Po
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(po))
//                    return Value(parsingValues.PoKey, parsingValues.PoIdentifier, parsingValues.PoIndex).Trim();
//                else
//                    return po;
//            }
//        }

//        public string PoLine
//        {
//            get
//            {
//                return Value(parsingValues.PoLineKey, parsingValues.PoLineIdentifier, parsingValues.PoLineIndex).Trim();
//            }
//        }

//        private string Value(string SW, string Identifier, int Index)
//        {
//            if (string.IsNullOrEmpty(SW))
//                return "";
            
//            if (string.IsNullOrEmpty(Identifier))
//                return Value(SW, Index);

//            var s = (from f in Lines
//                     where f.StartsWith(SW) && f.Contains(Identifier)
//                     select f).FirstOrDefault();
//            return s.Split(new string[] { "*" }, StringSplitOptions.None)[Index];
//        }

//        private string Value(string SW, int Index)
//        {
//            var s = (from f in Lines
//                     where f.StartsWith(SW)
//                     select f).FirstOrDefault();

//            return s.Split(new string[] { "*" }, StringSplitOptions.None)[Index];
//        }
//    }
//}
