
/****** Object:  StoredProcedure [dbo].[spAddCouponCode]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Add a Coupon Code
-- =============================================
CREATE PROCEDURE [dbo].[spAddCouponCode]
	-- Add the parameters for the stored procedure here
	@Code nvarchar(30),
	@OffPercent int = 0,
	@StartDate datetime = GETDATE,
	@EndDate datetime = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	INSERT INTO CouponCode(Code, OffPercent, StartDate, EndDate)
	VALUES(@Code, @OffPercent, @StartDate, @EndDate)
END
GO
/****** Object:  StoredProcedure [dbo].[spAddDestination]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Add a destination
-- =============================================
CREATE PROCEDURE [dbo].[spAddDestination]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30),
	@Name nvarchar(100) = NULL,
	@Image nvarchar(500) = NULL,
	@Description nvarchar(500) = NULL,
	@Country nvarchar(50) = NULL,
	@IsActive bit = 'TRUE'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	INSERT INTO Destination(Id, [Name], [Image], [Description], Country, IsActive)
	VALUES(@Id, @Name, @Image, @Description, @Country, @IsActive)
END
GO
/****** Object:  StoredProcedure [dbo].[spAddOrder]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Add an order
-- =============================================
CREATE PROCEDURE [dbo].[spAddOrder] 
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
/****** Object:  StoredProcedure [dbo].[spAddOrderDetail]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Add an order detail
-- =============================================
CREATE PROCEDURE [dbo].[spAddOrderDetail]
	-- Add the parameters for the stored procedure here
	@OrderId nvarchar(30),
	@TourId nvarchar(30),
	@AdultTicket int = 0,
	@KidTicket int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	INSERT INTO OrderDetails(OrderId, TourId, AdultTicket, KidTicket)
	VALUES(@OrderId, @TourId, @AdultTicket, @KidTicket)
END
GO
/****** Object:  StoredProcedure [dbo].[spAddTour]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Add a Tour
-- =============================================
CREATE PROCEDURE [dbo].[spAddTour]
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
/****** Object:  StoredProcedure [dbo].[spAddTourDestination]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Add tour detail
-- =============================================
CREATE PROCEDURE [dbo].[spAddTourDestination]
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
/****** Object:  StoredProcedure [dbo].[spAddTourGuide]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Add tour guide
-- =============================================
CREATE PROCEDURE [dbo].[spAddTourGuide]
	-- Add the parameters for the stored procedure here
	@TourId nvarchar(30),
	@UserId nvarchar(450),
	@AssignDate datetime = GETDATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	INSERT INTO TourGuide(TourId, UserId, AssignDate)
	VALUES(@TourId, @UserId, @AssignDate)
END
GO
/****** Object:  StoredProcedure [dbo].[spDeleteTourDestinationsByTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Delete Tour Destinations By Tour Id
-- =============================================
CREATE PROCEDURE [dbo].[spDeleteTourDestinationsByTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	DELETE FROM TourDetails WHERE TourID = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[spDeleteTourGuidesByTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Delete Tour Guides By Tour Id
-- =============================================
CREATE PROCEDURE [dbo].[spDeleteTourGuidesByTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	DELETE FROM TourGuide WHERE TourID = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[spFindCouponByCode]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Find Coupon By Code
-- =============================================
CREATE PROCEDURE [dbo].[spFindCouponByCode]
	-- Add the parameters for the stored procedure here
	@Code nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT OffPercent, StartDate, EndDate FROM CouponCode WHERE Code = @Code AND GETDATE() BETWEEN StartDate AND EndDate
END
GO
/****** Object:  StoredProcedure [dbo].[spFindDestinationsByTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Find Destinations By Tour Id
-- =============================================
CREATE PROCEDURE [dbo].[spFindDestinationsByTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DestinationId, [Name] FROM TourDetails 
	JOIN (SELECT Id, [Name] FROM Destination) AS tbDestination 
	ON DestinationId = Id WHERE TourId = @Id 
	ORDER BY IndexNumber
END
GO
/****** Object:  StoredProcedure [dbo].[spFindGuidesByTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Find Guides By Tour Id
-- =============================================
CREATE PROCEDURE [dbo].[spFindGuidesByTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT UserId, FirstName, LastName 
	FROM TourGuide JOIN (SELECT Id, FirstName, LastName FROM AspNetUsers) AS tbUser
	ON TourGuide.UserId = tbUser.Id 
	WHERE TourID = @Id 
	ORDER BY AssignDate
END
GO
/****** Object:  StoredProcedure [dbo].[spFindTourByTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Find Tour By Tour Id
-- =============================================
CREATE PROCEDURE [dbo].[spFindTourByTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [Name], [Image], FromDate, AdultFare, KidFare, MaxGuest, DATEDIFF(DAY, FromDate, ToDate) AS Duration 
	FROM Tour WHERE Id = @Id AND IsActive = 'True'
END
GO
/****** Object:  StoredProcedure [dbo].[spFindTourByTourIdEmp]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Find Tour By Tour Id for Employee
-- =============================================
CREATE PROCEDURE [dbo].[spFindTourByTourIdEmp]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [Name], [Description], [Image], FromDate, ToDate, AdultFare, KidFare, MaxGuest, IsActive 
	FROM Tour WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[spGetAllDestinations]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get All Destinations
-- =============================================
CREATE PROCEDURE [dbo].[spGetAllDestinations]
	-- Add the parameters for the stored procedure here
	@IsActive bit = 'True',
	@Skip int = 0,
	@Fetch int = 5
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ID, Name, [Image], [Description], Country 
	FROM Destination 
	WHERE IsActive = @IsActive 
	ORDER BY Id 
	OFFSET @Skip ROWS FETCH NEXT @Fetch ROWS ONLY
END
GO
/****** Object:  StoredProcedure [dbo].[spGetAllTours]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get All Tours
-- =============================================
CREATE PROCEDURE [dbo].[spGetAllTours]
	-- Add the parameters for the stored procedure here
	@Skip tinyint = 0,
	@Fetch tinyint = 5
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Id, [Name], [Description], FromDate, ToDate, Transport, AdultFare, [Image], DATEDIFF(HOUR, FromDate, ToDate) AS Duration 
	FROM Tour 
	WHERE IsActive = 'True' 
	ORDER BY ID 
	OFFSET @Skip ROWS FETCH NEXT @Fetch ROWS ONLY
END
GO
/****** Object:  StoredProcedure [dbo].[spGetDestinationsByTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Destinations By Tour Id
-- =============================================
CREATE PROCEDURE [dbo].[spGetDestinationsByTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DestinationId, [Name] 
	FROM TourDetails JOIN (SELECT Id, [Name] FROM Destination) AS tbDes 
	ON DestinationId = Id 
	WHERE TourID = @Id ORDER BY IndexNumber
END
GO
/****** Object:  StoredProcedure [dbo].[spGetDestinationsIdName]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Destinations Id and Name
-- =============================================
CREATE PROCEDURE [dbo].[spGetDestinationsIdName]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Id, [Name] FROM Destination WHERE IsActive = 'True'
END
GO
/****** Object:  StoredProcedure [dbo].[spGetFinalDestinationByTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Final Destination By Tour Id
-- =============================================
CREATE PROCEDURE [dbo].[spGetFinalDestinationByTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP 1 [Name] FROM TourDetails 
	JOIN (SELECT Id, [Name] FROM Destination) AS tbDestination ON DestinationId = Id 
	WHERE TourId = @Id 
	ORDER BY IndexNumber DESC
END
GO
/****** Object:  StoredProcedure [dbo].[spGetGuidesByTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Guides By Tour Id
-- =============================================
CREATE PROCEDURE [dbo].[spGetGuidesByTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT UserId FROM TourGuide WHERE TourID = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[spGetMaxGuestByTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Max Guest By Tour Id
-- =============================================
CREATE PROCEDURE [dbo].[spGetMaxGuestByTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT MaxGuest FROM Tour WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[spGetNearestTours]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Nearest Tours
-- =============================================
CREATE PROCEDURE [dbo].[spGetNearestTours]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP 5 Id , [Image], AdultFare, DATEDIFF(DAY, FromDate, ToDate) AS Duration
	FROM Tour 
	WHERE IsActive = 1 
	ORDER BY FromDate
END
GO
/****** Object:  StoredProcedure [dbo].[spGetTakenSlotByTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Taken Slot By Tour Id
-- =============================================
CREATE PROCEDURE [dbo].[spGetTakenSlotByTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT SUM(AdultTicket + KidTicket) AS TakenSlot FROM OrderDetails WHERE TourID = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[spGetTotalDestinations]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Total Destinations
-- =============================================
CREATE PROCEDURE [dbo].[spGetTotalDestinations]
	-- Add the parameters for the stored procedure here
	@IsActive bit = 'True'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT COUNT(Id) AS Total FROM Destination WHERE IsActive = @IsActive
END
GO
/****** Object:  StoredProcedure [dbo].[spGetTotalSearchTours]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Total Search Tours
-- =============================================
CREATE PROCEDURE [dbo].[spGetTotalSearchTours]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30) = '%',
	@Name nvarchar(100) = '%',
	@FromDate datetime = N'1800-01-01',
	@Duration int = 2147483647,
	@MinPrice decimal = -1000000000,
	@MaxPrice decimal = 1000000000, 
	@IsActive bit = 'True',
	@IsActiveCheck bit = 'True'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT COUNT(Id) AS Total 
	FROM Tour 
	WHERE (IsActive = @IsActive OR IsActive = @IsActiveCheck) 
	AND Id LIKE @Id 
	AND [Name] LIKE @Name 
	AND FromDate >= @FromDate 
	AND AdultFare >= @MinPrice 
	AND AdultFare <= @MaxPrice 
	AND DATEDIFF(HOUR, FromDate, ToDate) <= @Duration
END
GO
/****** Object:  StoredProcedure [dbo].[spGetTotalSearchToursIncludeDestinationId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Total Search Tours Include Destination
-- =============================================
CREATE PROCEDURE [dbo].[spGetTotalSearchToursIncludeDestinationId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30) = '%',
	@Name nvarchar(100) = '%',
	@FromDate datetime = N'1800-01-01',
	@Duration int = 2147483647,
	@MinPrice decimal = -1000000000,
	@MaxPrice decimal = 1000000000,	
	@DestinationId nvarchar(30) = NULL,
	@IsActive bit = 'True',
	@IsActiveCheck bit = 'True'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT COUNT(Id) AS Total 
	FROM Tour 
	WHERE (IsActive = @IsActive OR IsActive = @IsActiveCheck) 
	AND Id LIKE @Id 
	AND [Name] LIKE @Name 
	AND FromDate >= @FromDate 
	AND AdultFare >= @MinPrice 
	AND AdultFare <= @MaxPrice 
	AND DATEDIFF(HOUR, FromDate, ToDate) <= @Duration 
	AND (@DestinationId IS NULL OR Id IN (SELECT TourId AS Id FROM TourDetails WHERE DestinationId = @DestinationId))
END
GO
/****** Object:  StoredProcedure [dbo].[spGetTotalTours]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Total Tours
-- =============================================
CREATE PROCEDURE [dbo].[spGetTotalTours]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT COUNT(Id) AS Total FROM Tour WHERE IsActive = 'True'
END
GO
/****** Object:  StoredProcedure [dbo].[spGetTourInCartByTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Tour In Cart By Tour Id
-- =============================================
CREATE PROCEDURE [dbo].[spGetTourInCartByTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [Name], [Image], AdultFare, KidFare 
	FROM Tour WHERE Id = @Id AND IsActive = 'True'
END
GO
/****** Object:  StoredProcedure [dbo].[spGetTrendingTours]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Get Trending Tours
-- =============================================
CREATE PROCEDURE [dbo].[spGetTrendingTours]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP 5 TourId, [Image], AdultFare, Duration 
	FROM OrderDetails
	JOIN (SELECT Id, [Image], AdultFare, IsActive, DATEDIFF(DAY, FromDate, ToDate) AS Duration FROM Tour) AS tbTour 
	ON TourId = tbTour.Id
	WHERE IsActive = 'True' AND OrderId IN (SELECT Id FROM [Order] WHERE [Status] <> 'Canceled')
	GROUP BY TourID, [Image], AdultFare, Duration 
	ORDER BY SUM(AdultTicket + KidTicket) DESC
END
GO
/****** Object:  StoredProcedure [dbo].[spHasOrderByTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Has Order By TourId
-- =============================================
CREATE PROCEDURE [dbo].[spHasOrderByTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TourId FROM OrderDetails WHERE TourId = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[spIsAvailableTour]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Is Available Tour
-- =============================================
CREATE PROCEDURE [dbo].[spIsAvailableTour]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Id FROM Tour WHERE Id = @Id AND IsActive = 'True' AND FromDate > GETDATE()
END
GO
/****** Object:  StoredProcedure [dbo].[spIsExistedDestinationId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Is Existed Destination Id
-- =============================================
CREATE PROCEDURE [dbo].[spIsExistedDestinationId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Id FROM Destination WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[spIsExistedTourId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Is Existed Tour Id
-- =============================================
CREATE PROCEDURE [dbo].[spIsExistedTourId]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Id FROM Tour WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[spSearchTours]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Search Tours without Destination
-- =============================================
CREATE PROCEDURE [dbo].[spSearchTours]
	-- Add the parameters for the stored procedure here
	@Skip int = 0,
	@Fetch int = 5,
	@Id nvarchar(30) = '%',
	@Name nvarchar(100) = '%',
	@FromDate datetime = N'1800-01-01',
	@Duration int = 2147483647,
	@MinPrice decimal = -1000000000,
	@MaxPrice decimal = 1000000000, 
	@IsActive bit = 'True',
	@IsActiveCheck bit = 'True'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Id, [Name], [Description], FromDate, ToDate, Transport, AdultFare, [Image], DATEDIFF(HOUR, FromDate, ToDate) AS Duration 
	FROM Tour 
	WHERE (IsActive = @IsActive OR IsActive = @IsActiveCheck)
	AND Id LIKE @Id 
	AND [Name] LIKE @Name 
	AND FromDate >= @FromDate 
	AND AdultFare >= @MinPrice 
	AND AdultFare <= @MaxPrice 
	AND DATEDIFF(HOUR, FromDate, ToDate) <= @Duration  
	ORDER BY FromDate 
	OFFSET @Skip ROWS FETCH NEXT @Fetch ROWS ONLY
END
GO
/****** Object:  StoredProcedure [dbo].[spSearchToursIncludeDestinationId]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Search Tours Include Destination Id
-- =============================================
CREATE PROCEDURE [dbo].[spSearchToursIncludeDestinationId]
	-- Add the parameters for the stored procedure here
	@Skip int = 0,
	@Fetch int = 5,
	@Id nvarchar(30) = '%',
	@Name nvarchar(100) = '%',
	@FromDate datetime = N'1800-01-01',
	@Duration int = 2147483647,
	@MinPrice decimal = -1000000000,
	@MaxPrice decimal = 1000000000, 
	@DestinationId nvarchar(30) = NULL,
	@IsActive bit = 'True',
	@IsActiveCheck bit = 'True'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Id, [Name], [Description], FromDate, ToDate, Transport, AdultFare, [Image], DATEDIFF(HOUR, FromDate, ToDate) AS Duration 
	FROM Tour 
	WHERE (IsActive = @IsActive OR IsActive = @IsActiveCheck) 
	AND Id LIKE @Id 
	AND [Name] LIKE @Name 
	AND FromDate >= @FromDate 
	AND AdultFare >= @MinPrice 
	AND AdultFare <= @MaxPrice 
	AND DATEDIFF(HOUR, FromDate, ToDate) <= @Duration 
	AND (@DestinationId IS NULL OR Id IN (SELECT TourId AS Id FROM TourDetails WHERE DestinationId = @DestinationId)) 
	ORDER BY FromDate 
	OFFSET @Skip ROWS FETCH NEXT @Fetch ROWS ONLY
END
GO
/****** Object:  StoredProcedure [dbo].[spUpdateDestination]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Update a Destination
-- =============================================
CREATE PROCEDURE [dbo].[spUpdateDestination]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(30),
	@Name nvarchar(100) = NULL,
	@Image nvarchar(500) = NULL,
	@Description nvarchar(500) = NULL,
	@Country nvarchar(50) = NULL,
	@IsActive bit = 'TRUE'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	UPDATE Destination 
	SET [Name] = @Name, [Image] = @Image, [Description] = @Description, Country = @Country, IsActive = @IsActive 
	WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[spUpdateTour]    Script Date: 14/07/2019 11:44:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vo Phi Hoang
-- Create date: 05/07/2019
-- Description:	Update a Tour
-- =============================================
CREATE PROCEDURE [dbo].[spUpdateTour]
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
	UPDATE Tour 
	SET [Name] = @Name, [Description] = @Description, FromDate = @FromDate, ToDate = @ToDate, 
	AdultFare = @AdultFare, KidFare = @KidFare, MaxGuest = @MaxGuest, [Image] = @Image, Transport = @Transport, IsActive = @IsActive 
	WHERE Id = @Id
END
GO
USE [master]
GO
ALTER DATABASE [ZTouristDB] SET  READ_WRITE 
GO
