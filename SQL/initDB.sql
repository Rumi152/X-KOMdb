CREATE DATABASE XKOM;
GO
USE XKOM;
GO

CREATE TABLE [Product]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name]
	[Price]
	[Description]
	[Picture]
	[CategoryID]
	[CompanyID]
	[IsAvailable]
	[IntroductionDate]
	[Properties]
);

CREATE TABLE [ProductCategory]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name]
);

CREATE TABLE [ProductCompany]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name]
);

CREATE TABLE [Review]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Description]
	[Rating]
	[UserID]
);

CREATE TABLE [Cart]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[UserID]
	[IsActive]
	[PromoCodeID]
);

CREATE TABLE [Cart_Product]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[ProductID]
	[CartID]
);

CREATE TABLE [PromoCode]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Code]
	[StartDate]
	[EndDate]
	[Percentage]
	[MaximumMoney]
);

CREATE TABLE [User]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name]
	[LastName]
	[Password]
	[Email]
);

CREATE TABLE [FavouriteProduct]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[UserID]
	[ProductID]
);

CREATE TABLE [List]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Link] UNIQUE
	[Name]
);

CREATE TABLE [List_Product]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[ProductID]
	[ListID]
);

CREATE TABLE [Order]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[CartID]
	[StatusID]
	[OrderDate]
	[PaymentMethodID]
	[Price]
	[Address]
);

CREATE TABLE [OrderStatus]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name]
);

CREATE TABLE [PaymentMethod]
(
	[ID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name]
);