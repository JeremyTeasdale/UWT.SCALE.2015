using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHS.UWT.TPM.Data
{
    public class BHSShipmentSearchResultDO
    {
        public string SHIPMENT_ID { get; set; }
        public string USER_DEF1 { get; set; }
        public string BOL_NUM_ALPHA { get; set; }
        public DateTime SCHEDULED_SHIP_DATE { get; set; }
    }
}
