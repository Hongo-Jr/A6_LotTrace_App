USE [MES31]
GO

/****** Object:  Table [dbo].[ProductionResultsTable]    Script Date: 2026/09/12 13:55:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductionResultsTable](
	[ProductionOrderNumber] [nvarchar](32) NOT NULL,
	[LotNumber] [nvarchar](16) NOT NULL,
	[ItemName] [nvarchar](16) NOT NULL,
	[ItemCode] [nvarchar](16) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[ManufacturingProcessName] [nvarchar](32) NULL,
	[ManufacturingTankName] [nvarchar](16) NULL,
	[ChildTableKey01] [nvarchar](32) NULL,
	[ChildTableKey02] [nvarchar](32) NULL,
	[ChildTableKey03] [nvarchar](32) NULL,
	[ChildTableKey04] [nvarchar](32) NULL,
	[ChildTableKey05] [nvarchar](32) NULL,
	[ChildTableKey06] [nvarchar](32) NULL,
	[ChildTableKey07] [nvarchar](32) NULL,
	[ChildTableKey08] [nvarchar](32) NULL,
	[ChildTableKey09] [nvarchar](32) NULL,
	[ChildTableKey10] [nvarchar](32) NULL,
	[ChildTableKey11] [nvarchar](32) NULL,
	[ChildTableKey12] [nvarchar](32) NULL,
	[ChildTableKey13] [nvarchar](32) NULL,
	[ChildTableKey14] [nvarchar](32) NULL,
	[ChildTableKey15] [nvarchar](32) NULL,
	[ChildTableKey16] [nvarchar](32) NULL,
	[ChildTableKey17] [nvarchar](32) NULL,
	[ChildTableKey18] [nvarchar](32) NULL,
	[ChildTableKey19] [nvarchar](32) NULL,
	[ChildTableKey20] [nvarchar](32) NULL,
	[ChildTableKey21] [nvarchar](32) NULL,
	[ChildTableKey22] [nvarchar](32) NULL,
	[ChildTableKey23] [nvarchar](32) NULL,
	[ChildTableKey24] [nvarchar](32) NULL,
	[ChildTableKey25] [nvarchar](32) NULL,
	[ChildTableKey26] [nvarchar](32) NULL,
	[ChildTableKey27] [nvarchar](32) NULL,
	[ChildTableKey28] [nvarchar](32) NULL,
	[ChildTableKey29] [nvarchar](32) NULL,
	[ChildTableKey30] [nvarchar](32) NULL,
PRIMARY KEY NONCLUSTERED 
(
	[ProductionOrderNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

