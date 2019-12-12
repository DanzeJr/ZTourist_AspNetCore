-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Add an order
-- =============================================
CREATE PROCEDURE spAddOrder 
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30), 
	@UserId nvarchar(450),
	@CouponCode nvarchar(30) = NULL,
	@Comment nvarchar(500) = NULL,
	@OrderDate datetime = GETDATE,
	@Status nvarchar(30) = 'Processing'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	INSERT INTO [Order](Id, UserId, CouponCode, Comment, OrderDate, [Status])
	VALUES(@Id, @UserId, @CouponCode, @Comment, @OrderDate, @Status)
END
GO
