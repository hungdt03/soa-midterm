USE [BankDB]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 3/1/2024 6:05:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 3/1/2024 6:05:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[UserName] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[PhoneNumber] [nvarchar](max) NOT NULL,
	[Balance] [float] NOT NULL,
	[IsTrading] [bit] NOT NULL,
 CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OTPs]    Script Date: 3/1/2024 6:05:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OTPs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OTPCode] [nvarchar](6) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[ExpiredAt] [datetime2](7) NOT NULL,
	[OTPStatus] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
 CONSTRAINT [PK_OTPs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 3/1/2024 6:05:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Amount] [float] NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[StartTransactionTime] [datetime2](7) NOT NULL,
	[EndTransactionTime] [datetime2](7) NOT NULL,
	[TransactionType] [int] NOT NULL,
	[TransactionStatus] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240301031343_initDB', N'7.0.14')
GO
SET IDENTITY_INSERT [dbo].[Accounts] ON 

INSERT [dbo].[Accounts] ([Id], [Name], [Email], [UserName], [Password], [PhoneNumber], [Balance], [IsTrading]) VALUES (1, N'Đạo Thanh Hưng', N'hungktpm1406@gmail.com', N'hungdt', N'25d55ad283aa400af464c76d713c07ad', N'56452138353', 80220000, 0)
SET IDENTITY_INSERT [dbo].[Accounts] OFF
GO
SET IDENTITY_INSERT [dbo].[OTPs] ON 

INSERT [dbo].[OTPs] ([Id], [OTPCode], [CreatedAt], [ExpiredAt], [OTPStatus], [AccountId]) VALUES (3, N'028591', CAST(N'2024-03-01T10:25:22.5290202' AS DateTime2), CAST(N'2024-03-01T10:30:22.5290215' AS DateTime2), 2, 1)
INSERT [dbo].[OTPs] ([Id], [OTPCode], [CreatedAt], [ExpiredAt], [OTPStatus], [AccountId]) VALUES (5, N'597881', CAST(N'2024-03-01T11:09:50.2854671' AS DateTime2), CAST(N'2024-03-01T11:14:50.2854696' AS DateTime2), 2, 1)
SET IDENTITY_INSERT [dbo].[OTPs] OFF
GO
SET IDENTITY_INSERT [dbo].[Transactions] ON 

INSERT [dbo].[Transactions] ([Id], [Amount], [Content], [StartTransactionTime], [EndTransactionTime], [TransactionType], [TransactionStatus], [AccountId]) VALUES (3, 9890000, N'Thanh toán học phí cho Sinh viên Quốc Hưng', CAST(N'2024-03-01T10:25:22.5293925' AS DateTime2), CAST(N'2024-03-01T10:26:30.7692231' AS DateTime2), 0, 1, 1)
INSERT [dbo].[Transactions] ([Id], [Amount], [Content], [StartTransactionTime], [EndTransactionTime], [TransactionType], [TransactionStatus], [AccountId]) VALUES (5, 9890000, N'Thanh toán học phí', CAST(N'2024-03-01T11:09:50.2859362' AS DateTime2), CAST(N'2024-03-01T11:10:04.6103990' AS DateTime2), 0, 1, 1)
SET IDENTITY_INSERT [dbo].[Transactions] OFF
GO
ALTER TABLE [dbo].[OTPs]  WITH CHECK ADD  CONSTRAINT [FK_OTPs_Accounts_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OTPs] CHECK CONSTRAINT [FK_OTPs_Accounts_AccountId]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transactions_Accounts_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_Transactions_Accounts_AccountId]
GO
