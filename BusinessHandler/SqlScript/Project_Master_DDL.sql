
/****** Object:  Table [dbo].[DOCUMENT]    Script Date: 2017-08-04 14:06:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DOCUMENT](
	[DOC_GUID] [nvarchar](50) NOT NULL,
	[CITY_NM] [nvarchar](100),
	[DOC_TYPE] [nvarchar](50),
	[DOC_SOURCE] [nvarchar](500),
	[DOC_PATH] [nvarchar](1000),
	[CHECKED] [nvarchar](5) DEFAULT ('False'),
	[IMPORTANT] [nvarchar](5) DEFAULT ('False'),
	[READABLE] [nvarchar](5) DEFAULT ('True'),
	[USR_CRTN_ID] [varchar](50)  DEFAULT (user_name()) ,
	[USR_CRTN_TS] [datetime] DEFAULT (getdate()),
	[USR_MDFN_ID] [varchar](50) DEFAULT (user_name()),
	[USR_MDFN_TS] [datetime] DEFAULT (getdate()),
 CONSTRAINT [PK_DBO_DOCUMENT] PRIMARY KEY CLUSTERED 
(
	[DOC_GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO


alter table dbo.DOCUMENT add  COMMENT varchar(500)


/****** Object:  Table [dbo].[QUERY]    Script Date: 2017-08-04 14:06:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QUERY](
    [QUERY_GUID] [nvarchar](50) NOT NULL,
	[DOC_GUID] [nvarchar](50) NOT NULL constraint FK_DOCUMENT_DOC_GUID foreign key references DBO.DOCUMENT(DOC_GUID),
	[MEETING_DATE] [datetime],
	[SEARCH_DATE] [datetime] DEFAULT (getdate()),
	[MEETING_TITLE] [nvarchar](500),
	[MEETING_LOCATION] [nvarchar](500),
	[USR_CRTN_ID] [varchar](50)  DEFAULT (user_name()) ,
	[USR_CRTN_TS] [datetime] DEFAULT (getdate()),
	[USR_MDFN_ID] [varchar](50) DEFAULT (user_name()),
	[USR_MDFN_TS] [datetime] DEFAULT (getdate()),
 CONSTRAINT [PK_DBO_QUERY] PRIMARY KEY CLUSTERED 
(
	[QUERY_GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO



/****** Object:  Table [dbo].[QUERY_ENTRY]    Script Date: 2017-08-04 14:06:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QUERY_ENTRY](
    [ENTRY_GUID] [nvarchar](50) NOT NULL,
	[QUERY_GUID] [nvarchar](50) NOT NULL constraint FK_QUERY_QUERY_GUID foreign key references DBO.QUERY(QUERY_GUID),
	[PAGE_NUMBER] [INT],
	[KEYWORD] [nvarchar](100),
	[COMMENT] [nvarchar](500),
	[CONTENT] TEXT,
	[USR_CRTN_ID] [varchar](50)  DEFAULT (user_name()) ,
	[USR_CRTN_TS] [datetime] DEFAULT (getdate()),
	[USR_MDFN_ID] [varchar](50) DEFAULT (user_name()),
	[USR_MDFN_TS] [datetime] DEFAULT (getdate()),
 CONSTRAINT [PK_DBO_QUERY_ENTRY] PRIMARY KEY CLUSTERED 
(
	[ENTRY_GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [dbo].[CITY]    Script Date: 2017-08-04 14:06:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CITY](
	[CITY_NM] [nvarchar](100),
	[DEPLOYE_DATE] [nvarchar](100),
	[SHORT_NM] [varchar](50),
	[COUNTY_NM] [varchar](50),
	[TYP] [varchar](50),
	[LONG_NM] [varchar](50),
) 
GO

SET ANSI_PADDING OFF
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ACCOUNT](
    [EMAIL] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[Cityes] [nvarchar](MAX),
	[Active] [nvarchar](5),
	[RoleType] [nvarchar](10),
	[Operation] [nvarchar](100),
	[USR_CRTN_ID] [varchar](50)  DEFAULT (user_name()) ,
	[USR_CRTN_TS] [datetime] DEFAULT (getdate()),
	[USR_MDFN_ID] [varchar](50) DEFAULT (user_name()),
	[USR_MDFN_TS] [datetime] DEFAULT (getdate())
) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO

INSERT INTO dbo.ACCOUNT (EMAIL,Password,Active,RoleType) values ('admin@admin.com','MTIzNDU2','Yes','Admin')


/****** Object:  Table [dbo].[USER]    Script Date: 2017-08-04 14:06:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SearchQuery](
    [Guid] [nvarchar](50) NOT NULL,
	[Title] nvarchar(200) not null,
	[Content] text not null,
	[FrequentlyUsed] int,
	[Disabled] [nvarchar](10),
	[USR_CRTN_ID] [varchar](50)  DEFAULT (user_name()) ,
	[USR_CRTN_TS] [datetime] DEFAULT (getdate()),
	[USR_MDFN_ID] [varchar](50) DEFAULT (user_name()),
	[USR_MDFN_TS] [datetime] DEFAULT (getdate())
) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MeetingNote](
    [Guid] [nvarchar](50) NOT NULL,
	[Doc_Guid] [nvarchar](50) NOT NULL,
	[Notes] nvarchar(200) not null,
	[USR_CRTN_ID] [varchar](50)  DEFAULT (user_name()) ,
	[USR_CRTN_TS] [datetime] DEFAULT (getdate()),
	[USR_MDFN_ID] [varchar](50) DEFAULT (user_name()),
	[USR_MDFN_TS] [datetime] DEFAULT (getdate())
) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO


alter table dbo.CITY add  objectid int
alter table dbo.CITY add  color varchar(10)
ALTER TABLE CITY ADD STATES VARCHAR(20)