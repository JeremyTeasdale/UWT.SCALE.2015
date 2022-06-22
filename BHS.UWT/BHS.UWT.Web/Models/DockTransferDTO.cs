using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHS.UWT.Web
{
    public class DockTransferDTO
    {
        public string DockLocation { get; set; }
        public int ContainerCount { get; set; }
        public int ContainerCountTotal { get; set; }
    }
}
