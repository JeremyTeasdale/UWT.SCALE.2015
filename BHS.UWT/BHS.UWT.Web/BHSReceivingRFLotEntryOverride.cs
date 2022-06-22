using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Manh.WMFW.DataAccess;
using System.Data;
using Manh.WMFW.General;
using System.IO;

namespace BHS.UWT.Web
{
    public class BHSReceivingRFLotEntryOverride : com.pronto.rf.Receiving.LotEntry
    {
        protected override void PrintHiddenFieldsAfterForm()
        {
            base.PrintHiddenFieldsAfterForm();

            var HTML = string.Empty;

            try
            {
                HTML = this.sbHtml.ToString();

                // get item and recient num
                var re = "(name=\"(?<key>HIDDENINTRECNUM)\"|name=\"(?<key>HIDDENXREFITEM)\") value=\"(?<val>.*?)\"";

                var rx = new Regex(re, RegexOptions.IgnoreCase);

                var matches = rx.Matches(HTML);

                var intReceiptNum = (from Match m in matches
                                     where m.Groups["key"].Value == "HIDDENINTRECNUM"
                                     select m.Groups["val"].Value).SingleOrDefault();

                Debug.WriteLine(string.Format("BHS.UWT.Web.BHSReceivingRFLotEntryOverride: Internal Receipt Num: {0}", intReceiptNum));

                var item = (from Match m in matches
                            where m.Groups["key"].Value == "HIDDENXREFITEM"
                            select m.Groups["val"].Value).SingleOrDefault();

                Debug.WriteLine(string.Format("BHS.UWT.Web.BHSReceivingRFLotEntryOverride: Item: {0}", item));

                var lotExpDateArray = GetPreviousLotExpirationDate(this.rfSess.GetCSSession(), item, Convert.ToInt32(intReceiptNum));

                var lot = lotExpDateArray[0];
                var expDate = lotExpDateArray[1];

                if (!string.IsNullOrEmpty(lot))
                {
                    HTML = FillLot(HTML, lot);
                }

                Debug.WriteLine(string.Format("BHS.UWT.Web.BHSReceivingRFLotEntryOverride: Lot: {0}", lot));

                if (!string.IsNullOrEmpty(expDate))
                {
                    DateTime expd;
                    if (DateTime.TryParse(expDate, out expd))
                    {
                        HTML = FillExpirationDate(HTML, expd);
                    }
                }

                Debug.WriteLine(string.Format("BHS.UWT.Web.BHSReceivingRFLotEntryOverride: Expiration Date: {0}", expDate));

                base.sbHtml.Length = 0;
                base.sbHtml.Append(HTML);

                Debug.WriteLine("BHS.UWT.Web.BHSReceivingRFLotEntryOverride: End");
            }
            catch (Exception ex)
            {
                Manh.WMFW.General.ExceptionManager.LogException(this.rfSess.GetCSSession(), ex, "HTML: " + HTML);
            }
        }

        #region Fill Lot

        private static string FillLot(string HTML, string lot)
        {
            var oldlot = "<Input  type=TEXT name=LOT onfocus=\"javascript:this.select();\" onblur=\"LotTextChanged(true)\" maxlength=\"25.0\" size=\"15\">";
            var newlot = string.Format("<Input  type=TEXT name=LOT onfocus=\"javascript:this.select();\" onblur=\"LotTextChanged(true)\" maxlength=\"25.0\" size=\"15\" value=\"{0}\">", lot);

            HTML = HTML.Replace(oldlot, newlot);

            return HTML;
        }

        #endregion

        #region Fill Expiration Date

        private static string FillExpirationDate(string HTML, DateTime expd)
        {
            var oldmonth = "<Input  type=text name=MONTH onfocus=\"javascript:this.select();\"  id=\"MONTH\" size=\"1\" maxlength=\"2.0\">";
            var oldday = "<Input  type=text name=DAY onfocus=\"javascript:this.select();\"  id=\"DAY\" size=\"1\" maxlength=\"2.0\">";
            var oldyear = "<Input  type=text name=YEAR value=\"2010\" onfocus=\"javascript:this.select();\"  id=\"YEAR\" size=\"2\" maxlength=\"4.0\">";

            var newmonth = string.Format("<Input  type=text name=MONTH onfocus=\"javascript:this.select();\"  id=\"MONTH\" size=\"1\" maxlength=\"2.0\" value=\"{0}\">", expd.Month);
            var newday = string.Format("<Input  type=text name=DAY onfocus=\"javascript:this.select();\"  id=\"DAY\" size=\"1\" maxlength=\"2.0\" value=\"{0}\">", expd.Day);
            var newyear = string.Format("<Input  type=text name=YEAR value=\"{0}\" onfocus=\"javascript:this.select();\"  id=\"YEAR\" size=\"2\" maxlength=\"4.0\">", expd.Year);

            HTML = HTML.Replace(oldmonth, newmonth);
            HTML = HTML.Replace(oldday, newday);
            HTML = HTML.Replace(oldyear, newyear);

            // remove JS that is clearing the values
            HTML = HTML.Replace("Form1.DAY.value =\"\";", "");
            HTML = HTML.Replace("Form1.MONTH.value =\"\";", "");
            HTML = HTML.Replace("Form1.YEAR.value =\"\";", "");

            return HTML;
        }

        #endregion

        #region Get Previous Lot Expiration Date

        private string[] GetPreviousLotExpirationDate(Session session, string item, int internalReceiptNum)
        {
            // hit db and update 
            var expDate = string.Empty;
            var lot = string.Empty;

            try
            {
                using (DataHelper helper = new DataHelper(session))
                {
                    var parameterArray = new IDataParameter[2] { DataHelper.BuildParameter(session, "@internal_receipt_num", internalReceiptNum),
                                                                 DataHelper.BuildParameter(session, "@item", item) };

                    var table = helper.GetTable(CommandType.StoredProcedure, "BHS_ShippingContainer_GetPreviousLotExpDate", parameterArray);

                    if ((table != null) && (table.Rows.Count > 0))
                    {
                        lot = DataManager.GetString(table.Rows[0], "LOT");
                        expDate = DataManager.GetString(table.Rows[0], "EXPIRATION_DATE_TIME");
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception);
                Debug.WriteLine(exception.ToString());
            }

            return new string[] { lot, expDate };
        }

        #endregion
    }
}
