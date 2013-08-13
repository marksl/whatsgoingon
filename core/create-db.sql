use master
go

IF (EXISTS (SELECT name 
FROM master.dbo.sysdatabases 
WHERE ('[' + name + ']' = 'whatsgoingon' 
OR name = 'whatsgoingon' )))
BEGIN
	ALTER DATABASE [whatsgoingon] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP DATABASE [whatsgoingon]
END

GO


CREATE DATABASE [whatsgoingon];
go

use [whatsgoingon];


CREATE TABLE [dbo].[Build](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VMajor] [int] NOT NULL,
	[VMinor] [int] NOT NULL,
	[VPatch] [int] NOT NULL,
	[VBuild] [int] NULL,
	[GitSha] [varchar](9) NULL,
	[PreviousId] [int] NULL,
	[Created] [datetime2](7) NOT NULL,
	[Modified] [datetime2](7) NOT NULL,
	[Incomplete] [bit] NOT NULL,
	[JiraProcessed] [bit] NOT NULL,
 CONSTRAINT [PK_Build] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[Build] ADD  CONSTRAINT [DF_Build_Incomplete]  DEFAULT ((0)) FOR [Incomplete]

ALTER TABLE [dbo].[Build] ADD  CONSTRAINT [DF_Build_JiraProcessed]  DEFAULT ((0)) FOR [JiraProcessed]

ALTER TABLE [dbo].[Build]  WITH CHECK ADD  CONSTRAINT [FK_Build_PreviousBuild] FOREIGN KEY([PreviousId])
REFERENCES [dbo].[Build] ([Id])

ALTER TABLE [dbo].[Build] CHECK CONSTRAINT [FK_Build_PreviousBuild]

CREATE TABLE [dbo].[Category](
	[Name] [varchar](128) NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[BuildData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BuildId] [int] NOT NULL,
	[Category] [varchar](50) NULL,
	[Type] [varchar](10) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Data] [nvarchar](max) NOT NULL,
	[Processed] [bit] NOT NULL,
 CONSTRAINT [PK_BuildData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[BuildData]  WITH CHECK ADD  CONSTRAINT [FK_Build_BuildData] FOREIGN KEY([BuildId])
REFERENCES [dbo].[Build] ([Id])
ON DELETE CASCADE

ALTER TABLE [dbo].[BuildData] CHECK CONSTRAINT [FK_Build_BuildData]

CREATE TABLE [dbo].[BuildDiff](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BuildId] [int] NOT NULL,
	[PreviousBuildDataId] [int] NULL,
	[CurrentBuildDataId] [int] NULL,
	[Diff] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_BuildDiff_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[BuildDiff]  WITH CHECK ADD  CONSTRAINT [FK_Build_BuildDiff] FOREIGN KEY([BuildId])
REFERENCES [dbo].[Build] ([Id])

ALTER TABLE [dbo].[BuildDiff] CHECK CONSTRAINT [FK_Build_BuildDiff]

ALTER TABLE [dbo].[BuildDiff]  WITH CHECK ADD  CONSTRAINT [FK_BuildDiff_Current] FOREIGN KEY([CurrentBuildDataId])
REFERENCES [dbo].[BuildData] ([Id])

ALTER TABLE [dbo].[BuildDiff] CHECK CONSTRAINT [FK_BuildDiff_Current]

ALTER TABLE [dbo].[BuildDiff]  WITH CHECK ADD  CONSTRAINT [FK_BuildDiff_Previous] FOREIGN KEY([PreviousBuildDataId])
REFERENCES [dbo].[BuildData] ([Id])

ALTER TABLE [dbo].[BuildDiff] CHECK CONSTRAINT [FK_BuildDiff_Previous]

CREATE TABLE [dbo].[BuildJira](
	[BuildId] [int] NOT NULL,
	[Jira] [varchar](10) NOT NULL,
 CONSTRAINT [PK_BuildJira] PRIMARY KEY CLUSTERED 
(
	[BuildId] ASC,
	[Jira] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[BuildJira]  WITH CHECK ADD  CONSTRAINT [FK_BuildJira_Build] FOREIGN KEY([BuildId])
REFERENCES [dbo].[Build] ([Id])

ALTER TABLE [dbo].[BuildJira] CHECK CONSTRAINT [FK_BuildJira_Build]
