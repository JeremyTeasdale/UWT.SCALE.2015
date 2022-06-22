using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq.SqlClient;

using BHS.UWT.TPM;
using System.Xml;
using System.IO;
using BHS.UWT.TPM.Data;
using BHS.UWT.TPM.Data.Repositories;

namespace BHS.UWT.TPM
{
    public class GeneralRepository
    {
        #region Properties

        private ILSDBDataContext _db;
        public ILSDBDataContext DB
        {
            get
            {
                if (_db == null)
                    _db = new ILSDBDataContext();

                return _db;
            }
        }

        public static string TPMFileDirectory { get; set; }
        public string GetTPMFileDirectory
        {
            get
            {
                TPMFileDirectory = (from sc in DB.SYSTEM_CONFIG_DETAILs
                       where sc.SYS_KEY == "TPMFileDirectory"
                            && sc.RECORD_TYPE == "Web Inq" 
                       select sc.SYSTEM_VALUE).FirstOrDefault();

                return TPMFileDirectory;
            }
        }

        public string GetTPMBOLName
        {
            get
            {
                return (from sc in DB.SYSTEM_CONFIG_DETAILs
                        where sc.SYS_KEY == "TPMBOLName"
                             && sc.RECORD_TYPE == "Web Inq"
                        select sc.SYSTEM_VALUE).FirstOrDefault();
            }
        }

        public string GetTPMPackListName
        {
            get
            {
                return (from sc in DB.SYSTEM_CONFIG_DETAILs
                        where sc.SYS_KEY == "TPMPackListName"
                             && sc.RECORD_TYPE == "Web Inq"
                        select sc.SYSTEM_VALUE).FirstOrDefault();
            }
        }

        #endregion
        
        #region Public Methods

        public bool DoesUserExist(string username)
        {
            return (from users in DB.WEB_USERs
                    where users.WEB_USER1.ToUpper() == (username ?? "").ToUpper()
                    select users.WEB_USER1).Any();
        }

        public BHS.UWT.TPM.Data.WEB_USER GetUser(string username)
        {
            return (from users in DB.WEB_USERs
                    where users.WEB_USER1.ToUpper() == (username ?? "").ToUpper()
                    select new BHS.UWT.TPM.Data.WEB_USER()
                    {
                        AVAILABILITY_CHECKING = users.AVAILABILITY_CHECKING,
                        CLICKWRAP_ACCEPTED = users.CLICKWRAP_ACCEPTED,
                        COMPANY = users.COMPANY,
                        COMPANY_AUTH = users.COMPANY_AUTH,
                        CUSTOMER = users.CUSTOMER,
                        CUSTOMER_AUTH = users.CUSTOMER_AUTH,
                        CUSTOMER_GROUP = users.CUSTOMER_GROUP,
                        DATE_TIME_STAMP = users.DATE_TIME_STAMP,
                        DEFAULT_LANGUAGE = users.DEFAULT_LANGUAGE,
                        EMAIL_ADDRESS = users.EMAIL_ADDRESS,
                        ITEM_SELECTION_AUTH = users.ITEM_SELECTION_AUTH,
                        MARQUEE_MSG = users.MARQUEE_MSG,
                        ORDER_PROCESSING = users.ORDER_PROCESSING,
                        PASSWORD = users.PASSWORD,
                        PROCESS_STAMP = users.PROCESS_STAMP,
                        SHIP_TO_SELECT_AUTH = users.SHIP_TO_SELECT_AUTH,
                        USER_DEF1 = users.USER_DEF1,
                        USER_DEF2 = users.USER_DEF2,
                        USER_DEF3 = users.USER_DEF3,
                        USER_DEF4 = users.USER_DEF4,
                        USER_DEF5 = users.USER_DEF5,
                        USER_DEF6 = users.USER_DEF6,
                        USER_DEF7 = users.USER_DEF7,
                        USER_DEF8 = users.USER_DEF8,
                        USER_STAMP = users.USER_STAMP,
                        USER_TYPE = users.USER_TYPE,  
                        VENDOR = users.VENDOR,
                        WEB_USER1 = users.WEB_USER1
                        
                    }).FirstOrDefault();
        }


        // WILL TO DO: verify that this code change only returns shipments that a user has company access to
        // take into account the possible web user company_auth values and when we need to join on the WEB_USER_COMPANY_ASSIGNMENT table
        // one other requirement from UWT besides splitting by company...
        // any packing list or BOLs that was created on or before June 31, 2015 they require that all users from both Home products and personal care to have access to these documents

        public PagedList<BHSShipmentSearchResultDO> GetShipments(string company, BHSShipmentSearchDO shipmentSearchDO, int index, int pageSize)
        {
            return (from s in DB.BHS_TPM_ShipmentSearch(SessionHelper.User, shipmentSearchDO.ShipmentId, shipmentSearchDO.BOLNumber, shipmentSearchDO.ScheduledShipDate)
                    select new BHS.UWT.TPM.Data.BHSShipmentSearchResultDO()
                    {
                        SHIPMENT_ID = s.SHIPMENT_ID,
                        USER_DEF1 = s.USER_DEF1,
                        SCHEDULED_SHIP_DATE = s.SCHEDULED_SHIP_DATE,
                        BOL_NUM_ALPHA = s.BOL_NUM_ALPHA
                    }).ToList().ToPagedList<BHS.UWT.TPM.Data.BHSShipmentSearchResultDO>(index, pageSize);
            /*
            return (from s in DB.SHIPMENT_HEADERs 
                    join wu in DB.WEB_USER_COMPANY_ASSIGNMENTs on s.COMPANY equals wu.COMPANY
                    where (s.COMPANY == wu.COMPANY || (string.IsNullOrEmpty(company) && s.COMPANY == wu.COMPANY))
                        && (SqlMethods.Like(s.SHIPMENT_ID, shipmentSearchDO.ShipmentId) || (string.IsNullOrEmpty(shipmentSearchDO.ShipmentId) && s.SHIPMENT_ID == s.SHIPMENT_ID))
                        && (SqlMethods.Like(s.USER_DEF1, shipmentSearchDO.BOLNumber) || (string.IsNullOrEmpty(shipmentSearchDO.BOLNumber) && (s.USER_DEF1 == s.USER_DEF1 || s.USER_DEF1 == null)))
                        && (s.SCHEDULED_SHIP_DATE == shipmentSearchDO.ScheduledShipDate || (shipmentSearchDO.ScheduledShipDate == DateTime.MaxValue && s.SCHEDULED_SHIP_DATE == s.SCHEDULED_SHIP_DATE)) 
                    orderby s.SHIPMENT_ID
                    select new BHS.UWT.TPM.Data.SHIPMENT_HEADER()
                    {
                        SHIPMENT_ID = s.SHIPMENT_ID,
                        USER_DEF1 = s.USER_DEF1,
                        SCHEDULED_SHIP_DATE = s.SCHEDULED_SHIP_DATE,
                        BOL_NUM_ALPHA = s.BOL_NUM_ALPHA
                    }
                   ).ToPagedList<BHS.UWT.TPM.Data.SHIPMENT_HEADER>(index, pageSize);
             */

        }

        #endregion
    }
}
