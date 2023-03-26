Use AspNetAuth

CREATE Table Users 
(
	[UserId] INT IDENTITY(100, 1) PRIMARY KEY NOT NULL, 
	[Name] NVARCHAR(256) NOT NULL,
	[Password] NVARCHAR(MAX) NOT NULL,
	[LoginId] INT NULL,
	[RoleCode] varchar(20) NULL,
	ReportManager INT,
	CreateDate DateTime DEFAULT GETDATE()
)

CREATE Table [UserLogin]
(
	[LoginId] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL, 
	[Status] bit NULL,
	[LoginDate] DateTime NuLL,
	[UserId] int REFERENCES Users([UserId])
)

CREATE Table [UserRoles]
(
	[RoleId] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL, 
	[RoleName] nvarchar(50) NOT NULL,
	[RoleCode] AS 'R00' + CAST([RoleId] AS VARCHAR(10)) PERSISTED
)


CREATE Table Sales
(
	[SaleId] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL, 
	SaleDate DATETIME NULL,
	Amount Money NULL,
	UpdateDate DateTime NULL,
	[UserId] int REFERENCES Users([UserId])
)

