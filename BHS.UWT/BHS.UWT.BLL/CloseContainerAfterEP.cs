using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.IO;

using Manh.WMFW.Entities;
using Manh.WMW.General;
using Manh.WMFW.General;
using Manh.WMFW.DataAccess;
using Manh.WMFW.Config.BL;
using com.pronto.bl.outex;
using com.pronto.dbutility.process;
using Manh.ILS.NHibernate.Entities;
using Manh.ILS.Utility.BL;

namespace BHS.UWT.BLL
{
    public class CloseContainerAfterEP : IWorkFlowStep
    {
        #region IWorkFlowStep Members

        public object ExecuteStep(Session session, params object[] parameters)
        {
            Debug.WriteLine("CloseContainerAfterEP.ExecuteStep: Start");

            ShippingContainer be = parameters[0] as ShippingContainer;

            Debug.WriteLine("CloseContainerAfterEP.ExecuteStep: Position 1 ");

            
            WriteShipmentInfo(session, be);

            Debug.WriteLine("CloseContainerAfterEP.ExecuteStep: Position 2 ");

            var cont = GetShippingContainerDO(session, be);
            var legSession = SessionMapper.ConvertToLegacySession(session).ToString();
           string genssccerror; 
            var ss = new ShippingSupport();
            var ucc = ss.genSSCC(legSession, cont, out genssccerror);

            var customerucc = GetCustomerUCC(be, session);

            CheckDigitAlgorithmUcc128 ucc128 = new CheckDigitAlgorithmUcc128();

            if (customerucc != null)
            {
                ucc = customerucc ;
            }



            Debug.WriteLine("CloseContainerAfterEP.ExecuteStep: Position 3 ");

            WriteDebugForContainer(be, ucc);

            Debug.WriteLine("CloseContainerAfterEP.ExecuteStep: Position 4 ");

            UpdateContainerId(be, ucc, session);



            Debug.WriteLine("BHS.UWT.ExitPoints.CloseContainerAfterEP: End");

            return null;
        }

        #region Update Container Id

        private void UpdateContainerId(ShippingContainer be, string ucc, Session session)
        {
            // hit db and update 
            try
            {
                using (DataHelper helper = new DataHelper(session))
                {
                    var parameterArray = new IDataParameter[2] { DataHelper.BuildParameter(session, "@ucc", ucc),
                                                                 DataHelper.BuildParameter(session, "@internal_container_num", be.InternalContainerNum) };

                    helper.Update(CommandType.StoredProcedure, "BHS_ShippingContainer_UpdateContainerId", parameterArray);
                }
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception);
                Debug.WriteLine(exception.ToString());
            }
        }

        private string GetCustomerUCC(ShippingContainer be, Session session)
        {
            // hit db and update 
            string newucc = null;
            try
            {
                using (DataHelper helper = new DataHelper(session))
                {
                    var parameterArray = new IDataParameter[1] { DataHelper.BuildParameter(session, "@internal_container_num", be.InternalContainerNum) };


                    var table =  helper.GetTable(CommandType.StoredProcedure, "BHS_UCC_ASSIGNMENT_CONTAINER", parameterArray);

                    if ((table != null) && (table.Rows.Count > 0))
                    {
                        
                        string returndata = DataManager.GetString(table.Rows[0], "Container_ID");
                        if(returndata != null && returndata.Length > 0)
                        {
                            newucc = returndata;
                        }
                    }


                 }
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception);
                Debug.WriteLine(exception.ToString());
                return null;
            }

            return newucc;
        }

        #endregion

        #region Write Debug For Container

        private void WriteDebugForContainer(ShippingContainer be, string ucc)
        {
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.CloseContainerAfterEP: Internal Shipment Num = {0}",be.InternalShipmentNum ));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.CloseContainerAfterEP: Internal Container Num = {0}", be.InternalContainerNum));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.CloseContainerAfterEP: Container Id = {0}", be.ContainerId));
            Debug.WriteLine(string.Format("BHS.UWT.ExitPoints.CloseContainerAfterEP: UCC = {0}", ucc));
        }

        #endregion

        #region Get Shipping Container DO

        private ShippingContainerDO GetShippingContainerDO(Session session, ShippingContainer be)
        {
            var cont = new ShippingContainerDO(session);
            cont.setInternalContainerNum((int)be.InternalContainerNum);
            cont.InitDOFromDataRow(cont.GetRecordWithKey());

            return cont;
        }

        #endregion

        #region Write Shipment Info

        private void WriteShipmentInfo(Session session, ShippingContainer be)
        {
            try
            {
                int internalShipmentNum = be.InternalShipmentNum.InternalShipmentNum;
                using (DataHelper helper = new DataHelper(session))
                {
                    
                    var parameterArray = new IDataParameter[] { DataHelper.BuildParameter(session, "@InternalShipmentNum", internalShipmentNum) };
                    var table = helper.GetTable(CommandType.StoredProcedure, "BHS_ShipmentHeader_GetBOLEDIData", parameterArray);

                    // make sure we have at least 1 shipment header row, if not then do nothing do not generate a file

                    if ((table != null) && (table.Rows.Count > 0))
                    {
                        var sb = new StringBuilder();

                        // SHIPMENT HEADER

                        var delim = "|";

                        // really only always should only be one shipment header record

                        foreach (DataRow r in table.Rows)
                        {
                            /*
                              
                            USER_DEF1, CARRIER, COMPANY, ERP_ORDER, FREIGHT_TERMS, INTERFACE_RECORD_ID, PLANNED_SHIP_DATE, PRO_NUM_ALPHA,
                            ROUTING_CODE, SHIP_TO, SHIP_TO_ADDRESS1, SHIP_TO_ADDRESS2, SHIP_TO_ADDRESS3, SHIP_TO_CITY, SHIP_TO_COUNTRY, SHIP_TO_NAME,
                            SHIP_TO_POSTAL_CODE, SHIP_TO_STATE, SHIPMENT_ID
                              
                             */

                            sb.Append(DataManager.GetString(r, "PREFIX")).Append(delim);
                            sb.Append(DataManager.GetString(r, "USER_DEF1")).Append(delim);
                            sb.Append(DataManager.GetString(r, "CARRIER")).Append(delim);
                            sb.Append(DataManager.GetString(r, "COMPANY")).Append(delim);
                            sb.Append(DataManager.GetString(r, "ERP_ORDER")).Append(delim);
                            sb.Append(DataManager.GetString(r, "FREIGHT_TERMS")).Append(delim);
                            sb.Append(DataManager.GetString(r, "INTERFACE_RECORD_ID")).Append(delim);
                            sb.Append(DataManager.GetString(r, "PLANNED_SHIP_DATE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "PRO_NUM_ALPHA")).Append(delim);
                            sb.Append(DataManager.GetString(r, "ROUTING_CODE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "SHIP_TO")).Append(delim);
                            sb.Append(DataManager.GetString(r, "SHIP_TO_ADDRESS1")).Append(delim);
                            sb.Append(DataManager.GetString(r, "SHIP_TO_ADDRESS2")).Append(delim);
                            sb.Append(DataManager.GetString(r, "SHIP_TO_ADDRESS3")).Append(delim);
                            sb.Append(DataManager.GetString(r, "SHIP_TO_CITY")).Append(delim);
                            sb.Append(DataManager.GetString(r, "SHIP_TO_COUNTRY")).Append(delim);
                            sb.Append(DataManager.GetString(r, "SHIP_TO_NAME")).Append(delim);
                            sb.Append(DataManager.GetString(r, "SHIP_TO_POSTAL_CODE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "SHIP_TO_STATE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "SHIPMENT_ID")).Append(delim);

                            sb.Append(Environment.NewLine);
                        }

                        table = helper.GetTable(CommandType.StoredProcedure, "BHS_ShipmentDetail_GetBOLEDIData", parameterArray);

                        // SHIPMENT DETAIL

                        foreach (DataRow r in table.Rows)
                        {
                            /*
                             
                               COMPANY, CUSTOMER_PO, ERP_ORDER, INTERFACE_RECORD_ID, ITEM, NMFC_CODE, QUANTITY_UM, SHIPMENT_ID,
                               TOTAL_QTY, TOTAL_WEIGHT, USER_DEF2, USER_DEF3
                              
                            */

                            sb.Append(DataManager.GetString(r, "PREFIX")).Append(delim);
                            sb.Append(DataManager.GetString(r, "COMPANY")).Append(delim);
                            sb.Append(DataManager.GetString(r, "CUSTOMER_PO")).Append(delim);
                            sb.Append(DataManager.GetString(r, "ERP_ORDER")).Append(delim);
                            sb.Append(DataManager.GetString(r, "INTERFACE_RECORD_ID")).Append(delim);
                            sb.Append(DataManager.GetString(r, "ITEM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "NMFC_CODE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "QUANTITY_UM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "SHIPMENT_ID")).Append(delim);
                            sb.Append(DataManager.GetString(r, "TOTAL_QTY")).Append(delim);
                            sb.Append(DataManager.GetString(r, "TOTAL_WEIGHT")).Append(delim);
                            sb.Append(DataManager.GetString(r, "USER_DEF2")).Append(delim);
                            sb.Append(DataManager.GetString(r, "USER_DEF3")).Append(delim);

                            sb.Append(Environment.NewLine);
                        }

                        table = helper.GetTable(CommandType.StoredProcedure, "BHS_ShippingContainer_GetBOLEDIData", parameterArray);

                        // SHIPPING CONTAINER

                        foreach (DataRow r in table.Rows)
                        {
                            /*

                                  INTERNAL_CONTAINER_NUM, CONTAINER_ID, warehouse, CONTAINER_TYPE, CONTAINER_CLASS
                                  status
                                  STATUS_FAILED
                                  PARENT
                                  WEIGHT
                                  WEIGHT_UM
                                  VOLUME
                                  LENGTH
                                  WIDTH
                                  HEIGHT
                                  DIMENSION_UM
                                  VOLUME_UM
                                  VALUE
                                  INTERNAL_SHIPMENT_NUM
                                  INTERNAL_SHIPMENT_LINE_NUM
                                  COMPANY
                                  ITEM
                                  QUANTITY
                                  QUANTITY_UM
                                  NMFC_CODE
                                  STATUS_FLOW_NAME
                                  HAZARDOUS_CODE
                                  MANIFEST_STATE
                                  MANIFEST_CARR_SERVICE_SYMBOL
                                  MANIFEST_DATE_TIME
                                  TRACKING_NUMBER
                                  TOTAL_FREIGHT_CHARGE
                                  BASE_FREIGHT_CHARGE
                                  FREIGHT_DISCOUNT
                                  ACCESSORIAL_CHARGE
                                  USER_DEF1
                                  USER_DEF2
                                  USER_DEF3
                                  USER_DEF4
                                  USER_DEF5
                                  USER_DEF6
                                  USER_DEF7
                                  USER_DEF8
                                  USER_STAMP
                                  PROCESS_STAMP
                                  DATE_TIME_STAMP
                                  INTERNAL_ORDER_NUM
                                  MANIFEST_FOR_DATE
                                  TREE_UNIT
                                  PLANNED_DELIVERY_DATE_TIME
                                  BUNDLE_ID
                                  MSN
                                  LOCATION
                                  GROUP_POSITION
                                  LAUNCH_NUM
                                  CONTAINER_COUNT_NUMBER
                                  CONTAINER_COUNT_TOTAL
                                  GROUP_NUM
                                  WORK_ZONE
                                  INTERFACE_RECORD_ID
                                  PARENT_CONTAINER_ID
                                  INTERNAL_SHIP_ALLOC_NUM
                                  WORK_CREATED
                                  HUNDREDWEIGHT
                                  MANIFEST_ID
                                  MANIFEST_CLOSE_DATE_TIME
                                  EPC
                                  LOT
                                  URI
                                  RETURNS_TOTAL_FREIGHT_CHARGE
                                  RETURNS_BASE_FREIGHT_CHARGE
                                  RETURNS_FREIGHT_DISCOUNT
                                  RETURNS_ACCESSORIAL_CHARGE
                                  RETURNS_CALLTAG_FEE
                                  RETURNS_TRACKING_NUMBER
                                  RETURNS_MSN
                                  RETURNS_PRINT_LABEL
                                  WORLD_EASE_ID
                                  WORLD_EASE_GROUP_STATUS
                                  LOGISTICS_UNIT
                                  PARENT_LOGISTICS_UNIT
                                  PALLET_SEQUENCE
                                  ORIGINAL_PICK_LOC
                                  INTERNAL_MOP_NUMBER
                                  QC_STATUS
                                  QC_ASSIGNMENT_ID
                             */

                            sb.Append(DataManager.GetString(r, "PREFIX")).Append(delim);
                            sb.Append(DataManager.GetString(r, "INTERNAL_CONTAINER_NUM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "CONTAINER_ID")).Append(delim);
                            sb.Append(DataManager.GetString(r, "warehouse")).Append(delim);
                            sb.Append(DataManager.GetString(r, "CONTAINER_TYPE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "CONTAINER_CLASS")).Append(delim);
                            sb.Append(DataManager.GetString(r, "status")).Append(delim);
                            sb.Append(DataManager.GetString(r, "STATUS_FAILED")).Append(delim);
                            sb.Append(DataManager.GetString(r, "PARENT")).Append(delim);
                            sb.Append(DataManager.GetString(r, "WEIGHT")).Append(delim);
                            sb.Append(DataManager.GetString(r, "WEIGHT_UM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "VOLUME")).Append(delim);
                            sb.Append(DataManager.GetString(r, "LENGTH")).Append(delim);
                            sb.Append(DataManager.GetString(r, "WIDTH")).Append(delim);
                            sb.Append(DataManager.GetString(r, "HEIGHT")).Append(delim);
                            sb.Append(DataManager.GetString(r, "DIMENSION_UM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "VOLUME_UM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "VALUE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "INTERNAL_SHIPMENT_NUM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "INTERNAL_SHIPMENT_LINE_NUM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "COMPANY")).Append(delim);
                            sb.Append(DataManager.GetString(r, "ITEM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "QUANTITY")).Append(delim);
                            sb.Append(DataManager.GetString(r, "QUANTITY_UM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "NMFC_CODE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "STATUS_FLOW_NAME")).Append(delim);
                            sb.Append(DataManager.GetString(r, "HAZARDOUS_CODE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "MANIFEST_STATE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "MANIFEST_CARR_SERVICE_SYMBOL")).Append(delim);
                            sb.Append(DataManager.GetString(r, "MANIFEST_DATE_TIME")).Append(delim);
                            sb.Append(DataManager.GetString(r, "TRACKING_NUMBER")).Append(delim);
                            sb.Append(DataManager.GetString(r, "TOTAL_FREIGHT_CHARGE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "BASE_FREIGHT_CHARGE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "FREIGHT_DISCOUNT")).Append(delim);
                            sb.Append(DataManager.GetString(r, "ACCESSORIAL_CHARGE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "USER_DEF1")).Append(delim);
                            sb.Append(DataManager.GetString(r, "USER_DEF2")).Append(delim);
                            sb.Append(DataManager.GetString(r, "USER_DEF3")).Append(delim);
                            sb.Append(DataManager.GetString(r, "USER_DEF4")).Append(delim);
                            sb.Append(DataManager.GetString(r, "USER_DEF5")).Append(delim);
                            sb.Append(DataManager.GetString(r, "USER_DEF6")).Append(delim);
                            sb.Append(DataManager.GetString(r, "USER_DEF7")).Append(delim);
                            sb.Append(DataManager.GetString(r, "USER_DEF8")).Append(delim);
                            sb.Append(DataManager.GetString(r, "USER_STAMP")).Append(delim);
                            sb.Append(DataManager.GetString(r, "PROCESS_STAMP")).Append(delim);
                            sb.Append(DataManager.GetString(r, "DATE_TIME_STAMP")).Append(delim);
                            sb.Append(DataManager.GetString(r, "INTERNAL_ORDER_NUM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "MANIFEST_FOR_DATE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "TREE_UNIT")).Append(delim);
                            sb.Append(DataManager.GetString(r, "PLANNED_DELIVERY_DATE_TIME")).Append(delim);
                            sb.Append(DataManager.GetString(r, "BUNDLE_ID")).Append(delim);
                            sb.Append(DataManager.GetString(r, "MSN")).Append(delim);
                            sb.Append(DataManager.GetString(r, "LOCATION")).Append(delim);
                            sb.Append(DataManager.GetString(r, "GROUP_POSITION")).Append(delim);
                            sb.Append(DataManager.GetString(r, "LAUNCH_NUM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "CONTAINER_COUNT_NUMBER")).Append(delim);
                            sb.Append(DataManager.GetString(r, "CONTAINER_COUNT_TOTAL")).Append(delim);
                            sb.Append(DataManager.GetString(r, "GROUP_NUM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "WORK_ZONE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "INTERFACE_RECORD_ID")).Append(delim);
                            sb.Append(DataManager.GetString(r, "PARENT_CONTAINER_ID")).Append(delim);
                            sb.Append(DataManager.GetString(r, "INTERNAL_SHIP_ALLOC_NUM")).Append(delim);
                            sb.Append(DataManager.GetString(r, "WORK_CREATED")).Append(delim);
                            sb.Append(DataManager.GetString(r, "HUNDREDWEIGHT")).Append(delim);
                            sb.Append(DataManager.GetString(r, "MANIFEST_ID")).Append(delim);
                            sb.Append(DataManager.GetString(r, "MANIFEST_CLOSE_DATE_TIME")).Append(delim);
                            sb.Append(DataManager.GetString(r, "EPC")).Append(delim);
                            sb.Append(DataManager.GetString(r, "LOT")).Append(delim);
                            sb.Append(DataManager.GetString(r, "URI")).Append(delim);
                            sb.Append(DataManager.GetString(r, "RETURNS_TOTAL_FREIGHT_CHARGE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "RETURNS_BASE_FREIGHT_CHARGE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "RETURNS_FREIGHT_DISCOUNT")).Append(delim);
                            sb.Append(DataManager.GetString(r, "RETURNS_ACCESSORIAL_CHARGE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "RETURNS_CALLTAG_FEE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "RETURNS_TRACKING_NUMBER")).Append(delim);
                            sb.Append(DataManager.GetString(r, "RETURNS_MSN")).Append(delim);
                            sb.Append(DataManager.GetString(r, "RETURNS_PRINT_LABEL")).Append(delim);
                            sb.Append(DataManager.GetString(r, "WORLD_EASE_ID")).Append(delim);
                            sb.Append(DataManager.GetString(r, "WORLD_EASE_GROUP_STATUS")).Append(delim);
                            sb.Append(DataManager.GetString(r, "LOGISTICS_UNIT")).Append(delim);
                            sb.Append(DataManager.GetString(r, "PARENT_LOGISTICS_UNIT")).Append(delim);
                            sb.Append(DataManager.GetString(r, "PALLET_SEQUENCE")).Append(delim);
                            sb.Append(DataManager.GetString(r, "ORIGINAL_PICK_LOC")).Append(delim);
                            sb.Append(DataManager.GetString(r, "INTERNAL_MOP_NUMBER")).Append(delim);
                            sb.Append(DataManager.GetString(r, "QC_STATUS")).Append(delim);
                            sb.Append(DataManager.GetString(r, "QC_ASSIGNMENT_ID")).Append(delim);

                            sb.Append(Environment.NewLine);
                        }

                        // path

                        var prefix = "EDI211";
                        var dtFormat = DateTime.Now.ToString("HH_mm_ss");
                        var suffix = "_AMupl";

                        var fileName = string.Format("{0}{1}{2}{3}", prefix, internalShipmentNum, dtFormat, suffix);

                        Debug.WriteLine(fileName);

                        var path = SystemConfigRetrieval.GetStringSystemValue(session, "150", "Interface");

                        Debug.WriteLine(path);

                        using (StreamWriter outfile = new StreamWriter(Path.Combine(path, fileName)))
                        {
                            outfile.Write(sb.ToString());
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionManager.LogException(session, exception);
                Debug.WriteLine(exception.ToString());
            }
        }

        #endregion

        #endregion
    }
}
