USE [MES33]
GO

/****** Object:  Table [dbo].[FillingBottleTable]    Script Date: 2026/09/12 10:24:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FillingBottleTable](
	[OrderNumber] [nvarchar](20) NULL,
	[ProcessType] [nvarchar](4) NULL,
	[ProductLotNumber] [nvarchar](10) NULL,
	[ProductItemCode] [nvarchar](12) NULL,
	[MiddleProductLotNumber] [nvarchar](16) NULL,
	[MiddleProductItemCode] [nvarchar](12) NULL,
	[BottleID] [nvarchar](10) NOT NULL,
	[SamplingGroup] [nvarchar](12) NULL,
	[BottleINumber] [int] NULL,
	[FillingNozzleNumber] [int] NULL,
	[CapTighteningTorqueValue] [int] NULL,
	[CapTighteningTorqueJudgment] [int] NULL,
	[CapTiltDetectionJudgment] [int] NULL,
	[FillingMachineNumber] [int] NULL,
	[TotalCahckJudgment] [int] NULL,
	[BottleLocation] [int] NULL,
	[FillingWeight] [bigint] NULL,
	[FillingTime] [time](7) NULL,
	[FillingStartDate] [datetime] NULL,
	[FillingEndDate] [datetime] NULL,
PRIMARY KEY NONCLUSTERED 
(
	[BottleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


