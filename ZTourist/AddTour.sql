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
-- Description:	Add a Tour
-- =============================================
CREATE PROCEDURE spAddTour
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30),
	@Name nvarchar(100) = NULL,
	@Description nvarchar(500) = NULL,
	@FromDate datetime = GETDATE,
	@ToDate datetime = NULL,
	@AdultFare decimal(18,2) = NULL,
	@KidFare decimal(18,2) = NULL,
	@MaxGuest int = NULL,
	@Image nvarchar(200) = NULL,
	@Transport nvarchar(100) = NULL,
	@IsActive bit = 'TRUE' 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	INSERT INTO Tour(Id, Name, Description, FromDate, ToDate, AdultFare, KidFare, MaxGuest, Image, Transport, IsActive)
	VALUES(@Id, @Name, @Description, @FromDate, @ToDate, @AdultFare, @KidFare, @MaxGuest, @Image, @Transport, @IsActive)
END
GO
