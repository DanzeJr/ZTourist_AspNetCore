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
-- Description:	GetPriceByOrderId
-- =============================================
CREATE PROCEDURE spDeleteDestinationById
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	DELETE FROM TourDetails WHERE TourId = @Id
END
GO
