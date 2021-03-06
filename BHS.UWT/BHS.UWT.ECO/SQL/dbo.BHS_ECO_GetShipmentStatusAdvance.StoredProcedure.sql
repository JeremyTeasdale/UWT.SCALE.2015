USE [ILS]
GO
/****** Object:  StoredProcedure [dbo].[BHS_ECO_GetShipmentStatusAdvance]    Script Date: 5/10/2019 11:55:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[BHS_ECO_GetShipmentStatusAdvance]
as 

SET NOCOUNT ON;

select TOP 1 'Shipment' BusinessTransactionType,
		'ASN'+ CAST(sh.TRAILING_STS AS varchar)  TransactionCode,
		sh.SHIPMENT_ID + '-' + CAST(sh.INTERNAL_SHIPMENT_NUM AS varchar) ReferenceNum,
		sh.COMPANY TradingPartnerName
from SHIPMENT_HEADER sh
	join BHS_ECO_SHIPMENT_LOG bsl
		on bsl.InternalNum = sh.INTERNAL_SHIPMENT_NUM
where CAST(sh.TRAILING_STS AS varchar) <> bsl.Status
	and sh.TRAILING_STS in (100, 300, 400, 700)
	and isnull(sh.USER_DEF11, '') <> 'Y'
	and exists
	(select 'x' from BHS_ECO_SHIPMENT_LOG ecoTrans
	where ecoTrans.ReferenceNum = sh.SHIPMENT_ID + '-' + CAST(sh.INTERNAL_SHIPMENT_NUM AS varchar)
	--and ecoTrans.Operation = 'CreateChange' 
	and ecoTrans.IsError is null)
		

GO
