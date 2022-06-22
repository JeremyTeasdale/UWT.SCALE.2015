using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BHS.UWT.TPM.Data;

namespace BHS.UWT.TPM
{
    public static class SessionHelper
    {
        public static string CurrentCompany
        {
            get
            {
                return HttpContext.Current.Session["Company"] as string;
            }
        }

        // IMG LEFT
        public static string ImgLeft
        {
            get
            {
                return HttpContext.Current.Session["150"] as string;
            }
        }

        // IMG CENTER
        public static string ImgCenter
        {
            get
            {
                return HttpContext.Current.Session["140"] as string;
            }
        }

        // IMG RIGHT
        public static string ImgRight
        {
            get
            {
                return HttpContext.Current.Session["160"] as string;
            }
        }

        // URL LEFT
        public static string URLLeft
        {
            get
            {
                return HttpContext.Current.Session["180"] as string;
            }
        }

        // URL CENTER
        public static string URLCenter
        {
            get
            {
                return HttpContext.Current.Session["170"] as string;
            }
        }

        // URL CENTER
        public static string URLRight
        {
            get
            {
                return HttpContext.Current.Session["190"] as string;
            }
        }

        // USER
        public static string User
        {
            get
            {
                return HttpContext.Current.Session["User"] as string;
            }
        }

        // Shipment Search DO
        public static BHSShipmentSearchDO BHSShipmentSearchDO
        {
            get
            {
                if (HttpContext.Current.Session["BHSSearchDO"] != null)
                    return HttpContext.Current.Session["BHSSearchDO"] as BHSShipmentSearchDO;
                else
                    return null;
            }
        }
    }
}
