CREATE DATABASE EateryDB
GO
USE [EateryDB]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_General_Split]    Script Date: 20/05/2021 7:23:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_General_Split]
(
	@list VARCHAR(MAX),
	@delimiter VARCHAR(5)
)
RETURNS @retVal TABLE (Id INT IDENTITY(1,1), Value VARCHAR(MAX))
AS
BEGIN
	WHILE (CHARINDEX(@delimiter, @list) > 0)
	BEGIN
		INSERT INTO @retVal (Value)
		SELECT Value = LTRIM(RTRIM(SUBSTRING(@list, 1, CHARINDEX(@delimiter, @list) - 1)))
		SET @list = SUBSTRING(@list, CHARINDEX(@delimiter, @list) + LEN(@delimiter), LEN(@list))
	END
	INSERT INTO @retVal (Value)
	SELECT Value = LTRIM(RTRIM(@list))
	RETURN 
END
GO
/****** Object:  Table [dbo].[msDish]    Script Date: 20/05/2021 7:23:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[msDish](
	[DishID] [int] IDENTITY(1,1) NOT NULL,
	[DishTypeID] [int] NOT NULL,
	[DishName] [varchar](200) NOT NULL,
	[DishPrice] [int] NOT NULL,
	[AuditedActivity] [char](1) NOT NULL,
	[AuditedTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DishID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[msDishType]    Script Date: 20/05/2021 7:23:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[msDishType](
	[DishTypeID] [int] IDENTITY(1,1) NOT NULL,
	[DishTypeName] [varchar](100) NOT NULL,
	[AuditedActivity] [char](1) NOT NULL,
	[AuditedTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DishTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[msDish]  WITH CHECK ADD FOREIGN KEY([DishTypeID])
REFERENCES [dbo].[msDishType] ([DishTypeID])
GO
/****** Object:  StoredProcedure [dbo].[Dish_Delete]    Script Date: 20/05/2021 7:23:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Jonathan Ibrahim
 * Date: 10 Mar 2021
 * Purpose: Delete dish
 */
CREATE PROCEDURE [dbo].[Dish_Delete]
	@DishIDs VARCHAR(MAX)
AS
BEGIN
	UPDATE msDish
	SET AuditedActivity = 'D',
		AuditedTime = GETDATE()
	WHERE DishID IN (SELECT value FROM fn_General_Split(@DishIDs, ','))
END
GO
/****** Object:  StoredProcedure [dbo].[Dish_Get]    Script Date: 20/05/2021 7:23:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Jonathan Ibrahim
 * Date: 10 Mar 2021
 * Purpose: Get semua dish
 */
CREATE PROCEDURE [dbo].[Dish_Get]
AS
BEGIN
	SELECT 
		DishID,
		DishTypeID,
		DishName, 
		DishPrice 
	FROM msDish WITH(NOLOCK)
	WHERE AuditedActivity <> 'D'
END
GO
/****** Object:  StoredProcedure [dbo].[Dish_GetByID]    Script Date: 20/05/2021 7:23:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Jonathan Ibrahim
 * Date: 10 Mar 2021
 * Purpose: Get dish tertentu by Id
 */
CREATE PROCEDURE [dbo].[Dish_GetByID]
	@DishId INT
AS
BEGIN
	SELECT 
		DishID,
		DishTypeID,
		DishName, 
		DishPrice 
	FROM msDish WITH(NOLOCK)
	WHERE DishId = @DishId AND AuditedActivity <> 'D'
END
GO
/****** Object:  StoredProcedure [dbo].[Dish_InsertUpdate]    Script Date: 20/05/2021 7:23:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Jonathan Ibrahim
 * Date: 10 Mar 2021
 * Purpose: Insert atau update dish
 */


CREATE PROCEDURE [dbo].[Dish_InsertUpdate]
	@DishID INT OUTPUT,
	@DishTypeID INT,
	@DishName VARCHAR(100),
	@DishPrice INT
AS
BEGIN
	DECLARE @RetVal INT
	IF EXISTS (SELECT 1 FROM msDish WITH(NOLOCK) WHERE DishID = @DishID AND AuditedActivity <> 'D')
	BEGIN
		UPDATE msDish
		SET DishName = @DishName,
			DishTypeID = @DishTypeID,
			DishPrice = @DishPrice,
			AuditedActivity = 'U',
			AuditedTime = GETDATE()
		WHERE DishID = @DishID AND AuditedActivity <> 'D'
		SET @RetVal = @DishID
	END
	ELSE
	BEGIN
		INSERT INTO msDish 
		(DishName, DishTypeID, DishPrice, AuditedActivity, AuditedTime)
		VALUES
		(@DishName, @DishTypeID, @DishPrice, 'I', GETDATE())
		SET @RetVal = SCOPE_IDENTITY()
	END
	SELECT @DishId = @RetVal
END
GO
/****** Object:  StoredProcedure [dbo].[DishType_Get]    Script Date: 20/05/2021 7:23:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Jonathan Ibrahim
 * Date: 10 Mar 2021
 * Purpose: Get semua dish type
 */
CREATE PROCEDURE [dbo].[DishType_Get]
AS
BEGIN
	SELECT DishTypeID, DishTypeName FROM msDishType WITH(NOLOCK) 
	WHERE AuditedActivity <> 'D'
END
GO
/****** Object:  StoredProcedure [dbo].[DishType_GetByID]    Script Date: 20/05/2021 7:23:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Jonathan Ibrahim
 * Date: 10 Mar 2021
 * Purpose: Get dish type by ID
 */
CREATE PROCEDURE [dbo].[DishType_GetByID]
	@DishTypeID INT
AS
BEGIN
	SELECT DishTypeID, DishTypeName
	FROM msDishType WITH(NOLOCK)
	WHERE DishTypeID = @DishTypeID AND AuditedActivity <> 'D'
END
GO
-- SEEDING msDishType
INSERT INTO msDishType (DishTypeName,AuditedActivity,AuditedTime)
VALUES ('Rumahan','I',GETDATE()), ('Restoran','I',GETDATE()), ('Pinggiran','I',GETDATE())

GO

/**
 * Created by: Jonathan William Kurniawan
 * Date: 1 April 2022
 * Info: Table Recipe
 */
CREATE TABLE [dbo].[msRecipe](
	[RecipeID] [int] IDENTITY(1,1) NOT NULL,
	[DishID] [int] NOT NULL,
	[RecipeName] [varchar](200) NOT NULL,
	[RecipeDesc] [varchar](max) NOT NULL,
	[AuditedActivity] [char](1) NOT NULL,
	[AuditedTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RecipeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[msRecipe]  WITH CHECK ADD FOREIGN KEY([DishID])
REFERENCES [dbo].[msDish] ([DishID])

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**
 * Created by: Jonathan William Kurniawan
 * Date: 1 April 2022
 * Purpose: Get Recipe by DishID
 */

CREATE PROCEDURE [dbo].[Recipe_GetByDishID]
	@DishId INT
AS
BEGIN
	SELECT 
		RecipeID,
		DishID,
		RecipeName, 
		RecipeDesc 
	FROM msRecipe WITH(NOLOCK)
	WHERE DishId = @DishId AND AuditedActivity <> 'D'
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/**
 * Created by: Jonathan William Kurniawan
 * Date: 1 April 2022
 * Purpose: Insert/Update Recipe
 */

CREATE PROCEDURE [dbo].[Recipe_InsertUpdate]
	@RecipeID INT OUTPUT,
	@DishID INT,
	@RecipeName VARCHAR(100),
	@RecipeDesc VARCHAR(MAX)
AS
BEGIN
	DECLARE @RetVal INT
	IF EXISTS (SELECT 1 FROM msRecipe WITH(NOLOCK) WHERE RecipeID = @RecipeID AND AuditedActivity <> 'D')
	BEGIN
		UPDATE msRecipe
		SET RecipeName = @RecipeName,
			RecipeDesc = @RecipeDesc,
			AuditedActivity = 'U',
			AuditedTime = GETDATE()
		WHERE RecipeID = @RecipeID AND AuditedActivity <> 'D'
		SET @RetVal = @RecipeID
	END
	ELSE
	BEGIN
		INSERT INTO msRecipe
		(RecipeName, DishID, RecipeDesc, AuditedActivity, AuditedTime)
		VALUES
		(@RecipeName, @DishID, @RecipeDesc, 'I', GETDATE())
		SET @RetVal = SCOPE_IDENTITY()
	END
	SELECT @RecipeID = @RetVal
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/**
 * Created by: Jonathan William Kurniawan
 * Date: 1 April 2022
 * Purpose: Delete Recipe
 */

CREATE PROCEDURE [dbo].[Recipe_Delete]
	@RecipeIDs VARCHAR(MAX)
AS
BEGIN
	UPDATE msRecipe
	SET AuditedActivity = 'D',
		AuditedTime = GETDATE()
	WHERE RecipeID IN (SELECT value FROM fn_General_Split(@RecipeIDs, ','))
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/**
 * Created by: Jonathan William Kurniawan
 * Date: 1 April 2022
 * Purpose: Get Recipe by ID
 */

CREATE PROCEDURE [dbo].[Recipe_GetByID]
	@RecipeId INT
AS
BEGIN
	SELECT 
		RecipeID,
		DishID,
		RecipeName, 
		RecipeDesc 
	FROM msRecipe WITH(NOLOCK)
	WHERE RecipeID = @RecipeId AND AuditedActivity <> 'D'
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/**
 * Created by: Jonathan William Kurniawan
 * Date: 1 April 2022
 * Info: Table Recipe
 */

CREATE TABLE [dbo].[msIngredient](
	[IngredientID] [int] IDENTITY(1,1) NOT NULL,
	[RecipeID] [int] NOT NULL,
	[IngredientName] [varchar](200) NOT NULL,
	[Qty] [int] NOT NULL,
	[Unit] [varchar](10) NOT NULL,
	[AuditedActivity] [char](1) NOT NULL,
	[AuditedTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IngredientID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[msIngredient]  WITH CHECK ADD FOREIGN KEY([RecipeID])
REFERENCES [dbo].[msRecipe] ([RecipeID])

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/**
 * Created by: Jonathan William Kurniawan
 * Date: 1 April 2022
 * Purpose: Insert/Update Ingredient
 */

CREATE PROCEDURE [dbo].[Ingredient_InsertUpdate]
	@IngredientID INT OUTPUT,
	@RecipeID INT,
	@IngredientName VARCHAR(100),
	@Qty INT,
	@Unit VARCHAR(10)
AS
BEGIN
	DECLARE @RetVal INT
	IF EXISTS (SELECT 1 FROM msIngredient WITH(NOLOCK) WHERE IngredientID = @IngredientID AND AuditedActivity <> 'D')
	BEGIN
		UPDATE msIngredient
		SET IngredientName = @IngredientName,
			Qty = @Qty,
			Unit = @Unit,
			AuditedActivity = 'U',
			AuditedTime = GETDATE()
		WHERE IngredientID = @IngredientID AND AuditedActivity <> 'D'
		SET @RetVal = @IngredientID
	END
	ELSE
	BEGIN
		INSERT INTO msIngredient
		(IngredientName, RecipeID, Qty,Unit, AuditedActivity, AuditedTime)
		VALUES
		(@IngredientName,@RecipeID, @Qty, @Unit, 'I', GETDATE())
		SET @RetVal = SCOPE_IDENTITY()
	END
	SELECT @IngredientID = @RetVal
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**
 * Created by: Jonathan William Kurniawan
 * Date: 1 April 2022
 * Purpose: Get Ingredient by Recipe ID
 */


CREATE PROCEDURE [dbo].[Ingredient_GetByRecipeID]
	@RecipeId INT
AS
BEGIN
	SELECT 
		IngredientID,
		RecipeID,
		IngredientName,
		Qty,
		Unit
	FROM msIngredient WITH(NOLOCK)
	WHERE RecipeID = @RecipeId AND AuditedActivity <> 'D'
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**
 * Created by: Jonathan William Kurniawan
 * Date: 1 April 2022
 * Purpose: Get Ingredient by ID
 */


CREATE PROCEDURE [dbo].[Ingredient_GetByID]
	@IngredientId INT
AS
BEGIN
	SELECT 
		IngredientID,
		RecipeID,
		IngredientName,
		Qty,
		Unit
	FROM msIngredient WITH(NOLOCK)
	WHERE IngredientID = @IngredientId AND AuditedActivity <> 'D'
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**
 * Created by: Jonathan William Kurniawan
 * Date: 1 April 2022
 * Purpose: Delete Ingredient
 */


CREATE PROCEDURE [dbo].[Ingredient_Delete]
	@IngredientIDs VARCHAR(MAX)
AS
BEGIN
	UPDATE msIngredient
	SET AuditedActivity = 'D',
		AuditedTime = GETDATE()
	WHERE IngredientID IN (SELECT value FROM fn_General_Split(@IngredientIDs, ','))
END

GO

SELECT * FROM msDishType
GO
SELECT * FROM msDish
GO
SELECT * FROM msRecipe
GO
SELECT * FROM msIngredient