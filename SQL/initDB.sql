CREATE DATABASE XKOM;
GO
USE XKOM;
GO

CREATE TABLE [Product]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(32) NOT NULL,
	[Price] DECIMAL(8,2) NOT NULL,
	[CategoryID] INT NULL,
	[CompanyID] INT NULL,
	[NumberAvailable] INT NOT NULL,
	[IntroductionDate] DATE NULL,
	[Properties] VARCHAR(MAX) NULL
);

CREATE TABLE [ProductCategory]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(64) NOT NULL UNIQUE
);

CREATE TABLE [ProductCompany]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(64) NOT NULL UNIQUE
);

CREATE TABLE [Review]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[ProductID] INT NOT NULL,
	[Description] NVARCHAR(256) NOT NULL,
	[StarRating] INT NOT NULL, CHECK(StarRating >= 1 AND StarRating <= 6),
	[UserID] INT NULL
);

CREATE TABLE [Cart]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[UserID] INT NULL,
	[Discount] DECIMAL(8,2) NULL
);

CREATE TABLE [Cart_Product]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[ProductID] INT NOT NULL,
	[CartID] INT NOT NULL
);

CREATE TABLE [PromoCode]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Code] CHAR(16) NOT NULL,
	[StartDate] DATETIME NOT NULL,
	[EndDate] DATETIME NOT NULL,
	[Percentage] INT NOT NULL, CHECK([Percentage] <= 100 AND [Percentage] >= 0),
	[MaximumMoney] DECIMAL(8,2) NULL
);

CREATE TABLE [User]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] NVARCHAR(32) NOT NULL,
	[LastName] NVARCHAR(32) NOT NULL,
	[Password] VARCHAR(32) NOT NULL,
	[Email] VARCHAR(256) NOT NULL UNIQUE,
	[ActiveCartID] INT NULL
);

CREATE UNIQUE NONCLUSTERED INDEX idx_User_NullableUnique
ON [User](ActiveCartID)
WHERE ActiveCartID IS NOT NULL;

CREATE TABLE [FavouriteProduct]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[UserID] INT NOT NULL,
	[ProductID] INT NOT NULL
);

CREATE TABLE [List]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Link] VARCHAR(128) NULL,
	[Name] VARCHAR(32) NOT NULL DEFAULT 'newlist'
);

CREATE UNIQUE NONCLUSTERED INDEX idx_List_NullableUnique
ON [List](Link)
WHERE Link IS NOT NULL;

CREATE TABLE [List_Product]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[ProductID] INT NOT NULL,
	[ListID] INT NOT NULL
);

CREATE TABLE [Order]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[CartID] INT NOT NULL,
	[StatusID] INT NOT NULL,
	[OrderDate] DATETIME NOT NULL,
	[PaymentMethodID] INT NULL,
	[Price] MONEY NOT NULL,
	[ShipmentInfoID] INT NOT NULL UNIQUE,
	[NeedInstallationAssistance] BIT NOT NULL DEFAULT(0)
);

CREATE TABLE [ShipmentInfo]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[CityID] INT NOT NULL,
	[StreetName] VARCHAR(64) NOT NULL,
	[BuildingNumber] INT NOT NULL,
	[ApartmentNumber] INT NOT NULL
);

CREATE TABLE [City]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(64) NOT NULL UNIQUE
);

CREATE TABLE [OrderStatus]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(16) NOT NULL UNIQUE
);

CREATE TABLE [PaymentMethod]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(32) NOT NULL UNIQUE
);



ALTER TABLE Product
ADD FOREIGN KEY (CategoryID) REFERENCES ProductCategory (ID)
ON DELETE SET NULL;

ALTER TABLE Product
ADD FOREIGN KEY (CompanyID) REFERENCES ProductCompany (ID)
ON DELETE SET NULL;

ALTER TABLE Review
ADD FOREIGN KEY (ProductID) REFERENCES Product (ID)
ON DELETE CASCADE;

ALTER TABLE Review
ADD FOREIGN KEY (UserID) REFERENCES [User] (ID)
ON DELETE SET NULL;

ALTER TABLE Cart
ADD FOREIGN KEY (UserID) REFERENCES [User] (ID)
ON DELETE SET NULL;

ALTER TABLE [Order]
ADD FOREIGN KEY (CartID) REFERENCES Cart (ID)
ON DELETE NO ACTION;

ALTER TABLE [Order]
ADD FOREIGN KEY (StatusID) REFERENCES [OrderStatus] (ID)
ON DELETE NO ACTION;

ALTER TABLE [Order]
ADD FOREIGN KEY (PaymentMethodID) REFERENCES PaymentMethod (ID)
ON DELETE NO ACTION;

ALTER TABLE FavouriteProduct
ADD FOREIGN KEY (UserID) REFERENCES [User] (ID)
ON DELETE CASCADE;

ALTER TABLE FavouriteProduct
ADD FOREIGN KEY (ProductID) REFERENCES Product (ID)
ON DELETE CASCADE;

ALTER TABLE Cart_Product
ADD FOREIGN KEY (ProductID) REFERENCES Product (ID)
ON DELETE CASCADE;

ALTER TABLE Cart_Product
ADD FOREIGN KEY (CartID) REFERENCES Cart (ID)
ON DELETE CASCADE;

ALTER TABLE List_Product
ADD FOREIGN KEY (ProductID) REFERENCES Product (ID)
ON DELETE CASCADE;

ALTER TABLE List_Product
ADD FOREIGN KEY (ListID) REFERENCES List (ID)
ON DELETE CASCADE;

ALTER TABLE [Order]
ADD FOREIGN KEY (ShipmentInfoID) REFERENCES ShipmentInfo (ID)
ON DELETE NO ACTION;

ALTER TABLE ShipmentInfo
ADD FOREIGN KEY (CityID) REFERENCES City (ID)
ON DELETE NO ACTION;

ALTER TABLE [User]
ADD FOREIGN KEY (ActiveCartID) REFERENCES Cart (ID)
ON DELETE NO ACTION;

GO

CREATE TRIGGER [deleteActiveCart]
		ON [User]
			AFTER DELETE
		AS
		BEGIN
			DELETE FROM [Cart] 
			WHERE ID IN (SELECT ActiveCartID FROM deleted)
		END;

GO
USE master;