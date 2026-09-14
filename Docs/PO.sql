USE [MES31]
GO

/****** Object:  Table [dbo].[ProductionOrderTable]    Script Date: 2026/09/12 13:55:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductionOrderTable](
	[ProductionOrderNumber] [nvarchar](32) NOT NULL,
	[LotNumber] [nvarchar](16) NOT NULL,
	[ItemName] [nvarchar](16) NOT NULL,
	[ItemCode] [nvarchar](16) NOT NULL,
	[ManufacturingProcessName] [nvarchar](16) NOT NULL,
	[ManufacturingTankName] [nvarchar](16) NOT NULL,
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
	[ChildTableKey31] [nvarchar](32) NULL,
	[ChildTableKey32] [nvarchar](32) NULL,
	[ChildTableKey33] [nvarchar](32) NULL,
	[ChildTableKey34] [nvarchar](32) NULL,
	[ChildTableKey35] [nvarchar](32) NULL,
	[ChildTableKey36] [nvarchar](32) NULL,
	[ChildTableKey37] [nvarchar](32) NULL,
	[ChildTableKey38] [nvarchar](32) NULL,
	[ChildTableKey39] [nvarchar](32) NULL,
	[ChildTableKey40] [nvarchar](32) NULL,
	[ChildTableKey41] [nvarchar](32) NULL,
	[ChildTableKey42] [nvarchar](32) NULL,
	[ChildTableKey43] [nvarchar](32) NULL,
	[ChildTableKey44] [nvarchar](32) NULL,
	[ChildTableKey45] [nvarchar](32) NULL,
	[ChildTableKey46] [nvarchar](32) NULL,
	[ChildTableKey47] [nvarchar](32) NULL,
	[ChildTableKey48] [nvarchar](32) NULL,
	[ChildTableKey49] [nvarchar](32) NULL,
	[ChildTableKey50] [nvarchar](32) NULL,
	[ChildTableKey51] [nvarchar](32) NULL,
	[ChildTableKey52] [nvarchar](32) NULL,
	[ChildTableKey53] [nvarchar](32) NULL,
	[ChildTableKey54] [nvarchar](32) NULL,
	[ChildTableKey55] [nvarchar](32) NULL,
	[ChildTableKey56] [nvarchar](32) NULL,
	[ChildTableKey57] [nvarchar](32) NULL,
	[ChildTableKey58] [nvarchar](32) NULL,
	[ChildTableKey59] [nvarchar](32) NULL,
	[ChildTableKey60] [nvarchar](32) NULL,
PRIMARY KEY NONCLUSTERED 
(
	[ProductionOrderNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

