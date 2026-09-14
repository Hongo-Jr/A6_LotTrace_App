USE [MES33]
GO

/****** Object:  Table [dbo].[FillingOrderResultTable]    Script Date: 2026/09/12 10:22:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FillingOrderResultTable](
	[OrderNumber] [nvarchar](20) NOT NULL,
	[ProcessType] [nvarchar](4) NULL,
	[FillingBottleNumberResult_OK] [int] NULL,
	[FillingBottleNumberResult_NG] [int] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[MasterKey] [nvarchar](24) NOT NULL,
 CONSTRAINT [PK__FillingO__CAC5E74331E70DB7] PRIMARY KEY NONCLUSTERED 
(
	[MasterKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


