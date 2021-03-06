USE [ILS]
GO
/****** Object:  StoredProcedure [dbo].[BHS_ECO_GetShipmentHeaderData]    Script Date: 5/10/2019 11:55:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[BHS_ECO_GetShipmentHeaderData]
as 

SET NOCOUNT ON;

select distinct TOP 1 sh.LEADING_STS Status,
		sh.COMPANY Vendor,
		cp.NAME TradingPartnerDescription,
		sh.SHIPMENT_ID + '-' + CAST(sh.INTERNAL_SHIPMENT_NUM AS varchar) ReferenceNum,
		sh.INTERNAL_SHIPMENT_NUM ExternalReferenceNum,
		isnull(sh.ORDER_TYPE, '') OrderType,
		sh.WAREHOUSE VendorWarehouse,
		isnull(sh.CARRIER, '') DeliveryMethod,
		sh.FREIGHT_TERMS FreightTerms,
		sh.ROUTING_CODE RoutingCode,
		isnull(sh.BOL_NUM_ALPHA, '') BOLNumber,
		isnull(sh.PRO_NUM_ALPHA, '') ProNumber,
		'' SCAC,
		'' CurrencyCode,
		'' ShipToType,   --Duplicated Column
		sh.SHIP_TO ShipTo, -- Fix
		'' ShipToWarehouse,
		sh.SHIP_TO_ADDRESS1 ShipToAddress,
		sh.SHIP_TO_CITY ShipToCity,
		sh.SHIP_TO_STATE ShipToState,
		sh.SHIP_TO_POSTAL_CODE ShipToPostalCode,
		sh.SHIP_TO_COUNTRY ShipToCountry,
		sh.SHIP_TO_NAME ShipToName,
		isnull(sh.SHIP_TO_PHONE_NUM, '') ShipToPhone,
		isnull(sh.SHIP_TO_FAX_NUM, '') ShipToFax,
		'' ShipToType,
		w.warehouse ShipFrom ,
		'' ShipFromWarehouse,
		w.ADDRESS1 ShipFromAddress,
		w.CITY ShipFromCity,
		w.STATE ShipFromState,
		w.POSTAL_CODE ShipFromPostalCode,
		w.COUNTRY ShipFromCountry,
		'' ShipFromName,
		isnull(w.PHONE_NUM, '') ShipFromPhone,
		isnull(w.FAX_NUM, '') ShipFromFax,
		isnull(sh.SHIPPING_LOAD_NUM, 0) Load,
		'' RequestedDateType,
		isnull(sh.REQUESTED_DELIVERY_DATE, getdate()) RequestedDate,
		sh.SCHEDULED_SHIP_DATE ConfirmedDate,
		'' PlannedDeliveryDateStart,
		'' PlannedDeliveryDateEnd,
		sh.PLANNED_SHIP_DATE PlannedShipDate,
		isnull(sh.ACTUAL_SHIP_DATE_TIME, '') ActualShipDate,
		isnull(sh.STOP_SEQUENCE, 0) StopSequence,
		'' Tractor,
		'' Trailer,
		isnull(sh.USER_DEF1, '') UserDef1,
		isnull(sh.USER_DEF2, '') UserDef2,
		isnull(sh.USER_DEF3, '') UserDef3,
		isnull(sh.USER_DEF4, '') UserDef4,
		isnull(sh.USER_DEF5, '') UserDef5,
		isnull(sh.USER_DEF6, '') UserDef6,
		isnull(sh.USER_DEF7, 0.00000) UserDef7,
		isnull(sh.USER_DEF8, 0.00000) UserDef8,
		'' Messages
from SHIPMENT_HEADER sh
	join SHIPMENT_DETAIL SD ON SD.INTERNAL_SHIPMENT_NUM = SH.INTERNAL_SHIPMENT_NUM
	--join CARRIER c
		--on c.CARRIER = sh.CARRIER
	join WAREHOUSE w
		on w.warehouse = sh.warehouse
	join COMPANY cp
		on cp.COMPANY = sh.COMPANY
where 
		isnull(sh.USER_DEF11, '') <> 'Y' AND
		--isnull(sh.USER_DEF11, '') = 'ecoTest' AND
		isnull(cp.USER_DEF5, '') = 'ECO'
		and sh.warehouse = 'UWTMEM' AND
		(NOT EXISTS (select 'X' from BHS_ECO_SHIPMENT_LOG bsl where bsl.InternalNum = sh.INTERNAL_SHIPMENT_NUM and isnull(bsl.IsError, '') <> 'Y') 
		OR
		(EXISTS (select 'X' from BHS_ECO_SHIPMENT_LOG bsl where bsl.InternalNum = sh.INTERNAL_SHIPMENT_NUM and bsl.Status <> 'Closed') and sh.TRAILING_STS = 900))
		

GO
