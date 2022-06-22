//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace BHS.UWT.BLL
//{
//    class MicroHoldTrans
//    {
//        public MicroHoldParsingValues parsingValues;
//        public List<MicroHoldTransDetail> details;

//        public MicroHoldTrans(string line)
//        {
//            parsingValues = new MicroHoldParsingValues(line.Substring(1, 3));
//            details = new List<MicroHoldTransDetail>();

//            if (parsingValues.transType != MicroHoldTransType.Ignore)
//            {
//                if (string.IsNullOrEmpty(parsingValues.splitOn))
//                    details.Add(new MicroHoldTransDetail(line, parsingValues));
//                else
//                {
//                    string Po = "";
//                    foreach (string detailLine in line.Split(new string[] { parsingValues.splitOn }, StringSplitOptions.None))
//                    {
//                        //TODO - Save ERP Order
//                        MicroHoldTransDetail detail = new MicroHoldTransDetail(detailLine, parsingValues, Po);
//                        if (detail.header)
//                            Po = detail.Po;
//                        if (detail.ignoreRemainingDetails)
//                            break;
//                        if (!detail.ignoreDetail)
//                            details.Add(detail);
//                    }
//                }
//            }
//        }
//    }
//}
