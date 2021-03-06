USE [ILS]
GO
/****** Object:  StoredProcedure [dbo].[BHS_ECO_GetShipmentDetailsData]    Script Date: 5/10/2019 11:55:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[BHS_ECO_GetShipmentDetailsData](
	@INTERNAL_SHIPMENT_NUM int
)
as 

SET NOCOUNT ON;

select
		'' PurchaseOrderReferenceNum,
		'' PurchaseOrderLineNum,
		sd.REQUESTED_QTY POQuantity,
		sd.TOTAL_QTY Quantity,
		sd.QUANTITY_UM QuantityUM,
		sd.ITEM TradingPartnerItem,
		sd.ITEM HostItem,
		isnull(sd.ITEM_DEPARTMENT, '') ItemDepartment,
		sd.ITEM_DESC ItemDescription,
		case	
			when sd.LOT_CONTROLLED = 'N' Then 'False'
			when sd.LOT_CONTROLLED = 'Y' Then 'True'
		end BatchControlled,
		isnull(sd.LOT, '') BatchRegex,
		isnull(sd.HARMONIZED_CODE, '') HarmonizedCode,
		sd.TOTAL_WEIGHT Weight,
		isnull(sd.WEIGHT_UM, '') WeightUM,
		'False' ExpirationDateRequired,
		'False' ExpirationDateDisplay,
		'' UnitBarcode,
		sd.ITEM_NET_PRICE UnitCost,
		sd.QUANTITY_UM UnitDisplayName,
		'' PackBarcode,
		0.00000 PackCost,
		sd.TOTAL_QTY PackQuantity,
		sd.QUANTITY_UM PackDisplayName,
		'' TareBarcode,
		0.00000 TareCost,
		sd.TOTAL_QTY TareQuantity,
		'' TareDisplayName,
		isnull(sd.COUNTRY_OF_ORIGIN, '') CountryOfOrigin,
		isnull(sd.USER_DEF1, '') UserDef1,
		isnull(sd.USER_DEF2, '') UserDef2,
		isnull(sd.USER_DEF3, '') UserDef3,
		isnull(sd.USER_DEF4, '') UserDef4,
		isnull(sd.USER_DEF5, '') UserDef5,
		isnull(sd.USER_DEF6, '') UserDef6,
		isnull(sd.USER_DEF7, 0.00000) UserDef7,
		isnull(sd.USER_DEF8, 0.00000) UserDef8
from SHIPMENT_DETAIL sd
	join SHIPMENT_HEADER_VIEW shv
		on shv.INTERNAL_SHIPMENT_NUM = sd.INTERNAL_SHIPMENT_NUM
where sd.INTERNAL_SHIPMENT_NUM = @INTERNAL_SHIPMENT_NUM
		and shv.TOTAL_LINES < 20
		

GO
