using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHS.UWT.TPM.Data
{
    public class BHSShipmentSearchDO
    {
        public string ShipmentId { get; set; }
        public string BOLNumber { get; set; }
        public DateTime ScheduledShipDate { get; set; }
    }
}
