CREATE DATABASE XKOM;
GO
USE XKOM;
GO

CREATE TABLE [Product]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(64) NOT NULL,
	[Price] MONEY NOT NULL,
	[Description] VARCHAR(1024) NOT NULL,
	[Picture] IMAGE NULL,
	[CategoryID] INT NULL,
	[CompanyID] INT NULL,
	[IsAvailable] BIT NOT NULL,
	[IntroductionDate] DATE NULL,
	[Properties] VARCHAR(MAX) NULL
);

CREATE TABLE [ProductCategory]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(128) NOT NULL
);

CREATE TABLE [ProductCompany]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(128) NOT NULL
);

CREATE TABLE [Review]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[ProductID] INT NOT NULL,
	[Description] NVARCHAR(256) NOT NULL,
	[RatingID] INT NOT NULL,
	[UserID] INT NULL
);

CREATE TABLE [Cart]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[UserID] INT NULL,
	[IsActive] BIT NOT NULL,
	[PromoCodeID] INT NULL
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
	[MaximumMoney] MONEY NULL
);

CREATE TABLE [User]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] NVARCHAR(32) NOT NULL,
	[LastName] NVARCHAR(32) NOT NULL,
	[Password] VARCHAR(64) NOT NULL,
	[Email] VARCHAR(256) NOT NULL
);

CREATE TABLE [FavouriteProduct]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[UserID] INT NOT NULL,
	[ProductID] INT NOT NULL
);

CREATE TABLE [List]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Link] VARCHAR(128) NULL UNIQUE,
	[Name] VARCHAR(32) NOT NULL
);

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
	[Address] NVARCHAR(256) NOT NULL,
	[NeedInstallationAssistance] BIT DEFAULT(0)
);

CREATE TABLE [OrderStatus]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(16) NOT NULL
);

CREATE TABLE [PaymentMethod]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

CREATE TABLE [ReviewRating]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] VARCHAR(16) NOT NULL
);


ALTER TABLE Product
ADD FOREIGN KEY (CategoryID) REFERENCES ProductCategory (ID)
ON UPDATE CASCADE
ON DELETE SET NULL;

ALTER TABLE Product
ADD FOREIGN KEY (CompanyID) REFERENCES ProductCompany (ID)
ON UPDATE CASCADE
ON DELETE SET NULL;

ALTER TABLE Review
ADD FOREIGN KEY (ProductID) REFERENCES Product (ID)
ON UPDATE CASCADE
ON DELETE CASCADE;

ALTER TABLE Review
ADD FOREIGN KEY (RatingID) REFERENCES ReviewRating (ID)
ON UPDATE CASCADE
ON DELETE CASCADE;

ALTER TABLE Review
ADD FOREIGN KEY (UserID) REFERENCES [User] (ID)
ON UPDATE CASCADE
ON DELETE SET NULL;

ALTER TABLE Cart
ADD FOREIGN KEY (UserID) REFERENCES [User] (ID)
ON UPDATE CASCADE
ON DELETE SET NULL;

ALTER TABLE Cart 
ADD FOREIGN KEY (PromoCodeID) REFERENCES PromoCode (ID)
ON UPDATE CASCADE
ON DELETE SET NULL;

ALTER TABLE [Order]
ADD FOREIGN KEY (CartID) REFERENCES Cart (ID)
ON UPDATE CASCADE
ON DELETE NO ACTION;

ALTER TABLE [Order]
ADD FOREIGN KEY (StatusID) REFERENCES [OrderStatus] (ID)
ON UPDATE CASCADE
ON DELETE NO ACTION;

ALTER TABLE [Order]
ADD FOREIGN KEY (PaymentMethodID) REFERENCES PaymentMethod (ID)
ON UPDATE CASCADE
ON DELETE NO ACTION;

ALTER TABLE FavouriteProduct
ADD FOREIGN KEY (UserID) REFERENCES [User] (ID)
ON UPDATE CASCADE
ON DELETE CASCADE;

ALTER TABLE FavouriteProduct
ADD FOREIGN KEY (ProductID) REFERENCES Product (ID)
ON UPDATE CASCADE
ON DELETE CASCADE;

ALTER TABLE Cart_Product
ADD FOREIGN KEY (ProductID) REFERENCES Product (ID)
ON UPDATE CASCADE
ON DELETE CASCADE;

ALTER TABLE Cart_Product
ADD FOREIGN KEY (CartID) REFERENCES Cart (ID)
ON UPDATE CASCADE
ON DELETE CASCADE;

ALTER TABLE List_Product
ADD FOREIGN KEY (ProductID) REFERENCES Product (ID)
ON UPDATE CASCADE
ON DELETE CASCADE;

ALTER TABLE List_Product
ADD FOREIGN KEY (ListID) REFERENCES List (ID)
ON UPDATE CASCADE
ON DELETE CASCADE;


GO
USE master;