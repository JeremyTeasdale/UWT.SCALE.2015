USE [ILS]
GO
/****** Object:  StoredProcedure [dbo].[BHS_ECO_WriteTransactionHistory]    Script Date: 5/10/2019 11:55:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[BHS_ECO_WriteTransactionHistory](

       @InternalNum nvarchar(255),
	   @ReferenceNum nvarchar(255),
       @Status nvarchar(255),
       @Operation nvarchar(255),
       @IsError nvarchar(255),
       @ErrorMsg nvarchar(255),
       @Data nvarchar(255),
	   @DocumentType nvarchar(255)


)
AS 

SET NOCOUNT ON

-- CreateChange History
If @Operation = 'CreateChange'
	begin
	INSERT INTO BHS_ECO_SHIPMENT_LOG (InternalNum, ReferenceNum, Status, Operation, IsError, ErrorMsg, Data)

	select INTERNAL_SHIPMENT_NUM, @ReferenceNum, @Status, @Operation, @IsError, @ErrorMsg, @Data
	from SHIPMENT_HEADER
	where INTERNAL_SHIPMENT_NUM = @InternalNum
			and not exists (select 'X' from BHS_ECO_SHIPMENT_LOG where InternalNum = INTERNAL_SHIPMENT_NUM)
	end

If @Operation = 'CreateChange' and @Status = '900'
	begin
	update BHS_ECO_SHIPMENT_LOG
	set Status = 'Closed',
		Operation = @Operation,
		IsError = @IsError,
		ErrorMsg = @ErrorMsg,
		Data = @Data,
		DateTime = GETDATE()
	where ReferenceNum = @ReferenceNum
	end

If @Operation = 'CreateChange' and @Status <> '900'
	begin
	update BHS_ECO_SHIPMENT_LOG
	set Status = @Status,
		Operation = @Operation,
		IsError = @IsError,
		ErrorMsg = @ErrorMsg,
		Data = @Data,
		DateTime = GETDATE()
	where ReferenceNum = @ReferenceNum
	end

-- Status Advance History

If @Operation = 'StatusAdvance' and @IsError is null
	begin
	update BHS_ECO_SHIPMENT_LOG
	set Status = right(@Status, 3),
		Operation = @Operation,
		IsError = @IsError,
		ErrorMsg = @ErrorMsg,
		Data = @Data,
		DateTime = GETDATE()
	where ReferenceNum = @ReferenceNum
	end

--Document History
If @Operation = 'DOCUMENT'
	begin
	INSERT INTO BHS_ECO_Document_LOG (InternalNum, ReferenceNum, Status, Operation, IsError, ErrorMsg, Data, DocumentType)

	select InternalNum, @ReferenceNum, Status, @Operation, @IsError, @ErrorMsg, @Data, @DocumentType
	from BHS_ECO_SHIPMENT_LOG
	where ReferenceNum = @ReferenceNum

	update sh
	set sh.USER_DEF11 = 'Y'
	from SHIPMENT_HEADER sh
		join BHS_ECO_SHIPMENT_LOG bsl
			on bsl.InternalNum = sh.INTERNAL_SHIPMENT_NUM
	where bsl.ReferenceNum = @ReferenceNum and @IsError is null
	end
		

GO
