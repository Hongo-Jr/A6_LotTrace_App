USE [MES33]
GO

/****** Object:  Table [dbo].[FillingDrumcanTable]    Script Date: 2026/09/12 10:23:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FillingDrumcanTable](
	[OrderNumber] [nvarchar](20) NULL,
	[ProcessType] [nvarchar](4) NULL,
	[ProductLotNumber] [nvarchar](10) NULL,
	[ProductItemCode] [nvarchar](12) NULL,
	[MiddleProductLotNumber] [nvarchar](16) NULL,
	[MiddleProductItemCode] [nvarchar](12) NULL,
	[DrumcanNumber] [int] NOT NULL,
	[FillingNozzleNumber] [int] NULL,
	[CapTighteningTorqueValue_Big] [int] NULL,
	[CapTighteningTorqueJudgment_Big] [int] NULL,
	[CapTiltDetectionJudgment_Big] [int] NULL,
	[TotalCahckJudgment] [int] NULL,
	[CapTighteningTorqueValue_Small] [int] NULL,
	[FillingWeightJudgment] [int] NULL,
	[BottleLocation] [int] NULL,
	[FillingWeight] [bigint] NULL,
	[FillingTime] [time](7) NULL,
	[FillingStartDate] [datetime] NULL,
	[FillingEndDate] [datetime] NULL,
	[CapTighteningTorqueJudgment_Small] [int] NULL,
	[CapTiltDetectionJudgment_Small] [int] NULL,
PRIMARY KEY NONCLUSTERED 
(
	[DrumcanNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


