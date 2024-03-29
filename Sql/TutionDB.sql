USE [TutionDB]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 3/1/2024 6:06:59 PM ******/
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
/****** Object:  Table [dbo].[Students]    Script Date: 3/1/2024 6:06:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Students](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StudentCode] [nvarchar](8) NOT NULL,
	[FullName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tutions]    Script Date: 3/1/2024 6:06:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tutions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Amount] [float] NOT NULL,
	[TutionCode] [nvarchar](max) NOT NULL,
	[StartAt] [datetime2](7) NOT NULL,
	[EndAt] [datetime2](7) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Status] [int] NOT NULL,
	[TransactionId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
 CONSTRAINT [PK_Tutions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240301030848_initBD', N'7.0.14')
GO
SET IDENTITY_INSERT [dbo].[Students] ON 

INSERT [dbo].[Students] ([Id], [StudentCode], [FullName]) VALUES (1, N'52100551', N'Nguyễn Quốc Hưng')
INSERT [dbo].[Students] ([Id], [StudentCode], [FullName]) VALUES (2, N'52100773', N'Nguyễn Văn Biên')
SET IDENTITY_INSERT [dbo].[Students] OFF
GO
SET IDENTITY_INSERT [dbo].[Tutions] ON 

INSERT [dbo].[Tutions] ([Id], [Amount], [TutionCode], [StartAt], [EndAt], [Description], [Status], [TransactionId], [StudentId]) VALUES (1, 9890000, N'HPHK2-2023/2024', CAST(N'2024-01-28T00:00:00.0000000' AS DateTime2), CAST(N'2024-02-28T00:00:00.0000000' AS DateTime2), N'Học phí học kì 2 năm 2023-2024 của sinh viên Nguyễn Quốc Hưng', 1, 3, 1)
INSERT [dbo].[Tutions] ([Id], [Amount], [TutionCode], [StartAt], [EndAt], [Description], [Status], [TransactionId], [StudentId]) VALUES (2, 9890000, N'HPHK2-2023/2024', CAST(N'2024-01-28T00:00:00.0000000' AS DateTime2), CAST(N'2024-02-28T00:00:00.0000000' AS DateTime2), N'Học phí học kì 2 năm 2023-2024 của sinh viên Nguyễn Văn Biên', 1, 5, 2)
SET IDENTITY_INSERT [dbo].[Tutions] OFF
GO
ALTER TABLE [dbo].[Tutions]  WITH CHECK ADD  CONSTRAINT [FK_Tutions_Students_StudentId] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Tutions] CHECK CONSTRAINT [FK_Tutions_Students_StudentId]
GO
