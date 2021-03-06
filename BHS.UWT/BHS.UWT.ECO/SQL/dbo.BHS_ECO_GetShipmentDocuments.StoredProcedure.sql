USE [ILS]
GO
/****** Object:  StoredProcedure [dbo].[BHS_ECO_GetShipmentDocuments]    Script Date: 5/10/2019 11:55:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[BHS_ECO_GetShipmentDocuments]
as 

SET NOCOUNT ON;

select case 
			when eco.docType = 'Bill of Lading' Then 'Bill of Lading'
			when eco.docType = 'Packing List' Then 'Packing List'
		end DocumentType,
		'Shipping Docs'  DocumentGroup,
		'Shipment' BusinessTransactionType,
		sh.SHIPMENT_ID + '-' + CAST(sh.INTERNAL_SHIPMENT_NUM AS varchar) ReferenceNum,
		case
			when eco.docType = 'Bill of Lading' Then 'BOL-'+ sh.USER_DEF1 + '.pdf'
			when eco.docType = 'Packing List' Then 'PL-'+ sh.USER_DEF1 + '.pdf'
		end FileName
		--(select SYS1VALUE from GENERIC_CONFIG_DETAIL where RECORD_TYPE = 'ECO_VALUES' and IDENTIFIER = 'DOCS_DIR') + '\BOL-'+ sh.USER_DEF1 + '.pdf' FileName	
from SHIPMENT_HEADER sh
	join BHS_ECO_SHIPMENT_LOG bsl
		on bsl.InternalNum = sh.INTERNAL_SHIPMENT_NUM
	join (values ('Bill of Lading'), ('Packing List')) eco(docType)
		on 1=1
where not exists ( select 1 from BHS_ECO_DOCUMENT_LOG where InternalNum = sh.INTERNAL_SHIPMENT_NUM and DocumentType = eco.docType)
		and sh.TRAILING_STS = 900
		and isnull(sh.USER_DEF11, '') <> 'Y'
		

GO
