using System;
using System.Data;
using System.Configuration;
using System.Web;


namespace BHS.UWT.WEB
{
    public static class Constants
    {
        public static readonly string ITEM = "ITEM";
        public static readonly string ITEMDESCRIPTION = "ITEMDECRIPTION";
        public static readonly string LOCATION = "LOCATION";
        public static readonly string COMPANY = "COMPANY";
        public static readonly string TOLOCATION = "TOLOCATION";
        public static readonly string ADJUSTMENTTYPE = "AdjType";

        public static class StripTruck
        {
            public static readonly string BAD = "BAD";
            public static readonly string GOOD = "GOOD";
            public static readonly string ENTERED = "ENTERED";
            public static readonly string ERROR = "ERROR";
            public static readonly string PROCESSED = "PROCESSED";
        }

        public static class VTQStatus
        {
            public static readonly string ERROR = "ERROR";
            public static readonly string PROCESSED = "PROCESSED";
        }

        public static class RFCodeDateCC
        {
            public static readonly string LICENSEPLACE = "LICENSEPLACE";
            public static readonly string ITEM = "ITEM";
            public static readonly string QUANTITY = "QUANTITY";
            public static readonly string CODEDATE = "CODEDATE";
            public static readonly string LOCATION = "LOCATION";
        }

        public static class RFCC
        {
            public static readonly string WORKTYPE = "BHS||WORKTYPE";
            public static readonly string LOCATION = "BHS||LOCATION";
            public static readonly string RDLOCATION = "BHS||RDLOCATION";
            public static readonly string WORKDS = "BHS||WORKDS";
            public static readonly string SYSTEMQTY = "BHS||SYSTEMQTY";
            public static readonly string RECOUNT = "BHS||RECOUNT";
            public static readonly string LASTCOUNT = "BHS||LASTCOUNT";
            public static readonly string SINGLEITEM = "BHS||SINGLEITEM";
            public static readonly string WORKUNIT = "BHS||WORKUNIT";
            public static readonly string COUNTGROUP = "BHS||COUNTGROUP";
            public static readonly string LASTCOUNTEDLOC = "BHS||LASTCOUNTEDLOC";
        }

        public static class RFCCREC
        {
            public static readonly string CCREQLIST = "BHS||CCREQLIST";
        }
    }
}
