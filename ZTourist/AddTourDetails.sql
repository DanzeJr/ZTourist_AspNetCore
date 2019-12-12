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
-- Description:	Add tour detail
-- =============================================
CREATE PROCEDURE spAddTourDestination
	-- Add the parameters for the stored procedure here
	@TourId nvarchar(30),
	@DestinationId nvarchar(30),
	@IndexNumber int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	INSERT INTO TourDetails(TourId, DestinationId, IndexNumber)
	VALUES(@TourId, @DestinationId, @IndexNumber)
END
GO
