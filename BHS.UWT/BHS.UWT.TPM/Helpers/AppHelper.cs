using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace BHS.UWT.TPM
{
    public static class AppHelper
    {
        #region Path Helpers

        public static string ContentRoot
        {
            get
            {
                string contentVirtualRoot = "~/Content";
                return VirtualPathUtility.ToAbsolute(contentVirtualRoot);
            }
        }

        public static string CSSRoot
        {
            get
            {
                return string.Format("{0}/{1}", ContentRoot, "CSS");
            }
        }

        public static string IconRoot
        {
            get
            {
                return string.Format("{0}/{1}/{2}", ContentRoot, "Images", "Icons");
            }
        }

        public static string ImageRoot
        {
            get
            {
                return string.Format("{0}/{1}", ContentRoot, "Images");
            }
        }

        public static string ScriptRoot
        {
            get
            {
                return string.Format("{0}/{1}", ContentRoot, "Scripts");
            }
        }


        public static string CSSUrl(string cssFile)
        {
            string result = string.Format("{0}/{1}", CSSRoot, cssFile);
            return result;
        }

        public static string ImageURL(string imageFile)
        {
            string result = string.Format("{0}/{1}", ImageRoot, imageFile);
            return result;
        }

        public static string IconURL(string iconFile)
        {
            string result = string.Format("{0}/{1}", IconRoot, iconFile);
            return result;
        }

        public static string ScriptUrl(string scriptFile)
        {
            string result = string.Format("{0}/{1}", ScriptRoot, scriptFile);
            return result;
        }

        #endregion

        #region Display Helpers

        public static int PagingSize
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationSettings.AppSettings["PagingSize"].ToString());
                }
                catch (Exception ex)
                {
                    return 15;
                }
            }
        }

        #endregion
    }
}
