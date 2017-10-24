
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
	[Notes] nvarchar(2000) not null,
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
CREATE TABLE [dbo].[ACCOUNT_CITY](
    [City_Guid] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
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

ALTER TABLE CITY ADD [GUID] VARCHAR(50)


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CITY_Ordinance](
    [City_Guid] [varchar](50) NOT NULL,
	[OptStatus] [varchar](10) ,
	[DraftDate] [varchar](20) ,
	[FinalDate] [varchar](20) ,
	[Measurement] [varchar](20) ,

	[BufferSchoolFeet] [varchar](10) ,
	[BufferSchoolNote] [nvarchar](1000) ,
	[BufferDaycareFeet] [varchar](10) ,
	[BufferDaycareNote] [nvarchar](1000) ,
	[BufferParkFeet] [varchar](10) ,
	[BufferParkNote] [nvarchar](1000) ,
	[BufferSDMFeet] [varchar](10) ,
	[BufferSDMNote] [nvarchar](1000) ,
	[BufferReligiousFeet] [varchar](10) ,
	[BufferReligiousNote] [nvarchar](1000) ,
	[BufferOtherFeet] [varchar](10) ,
	[BufferOtherNote] [nvarchar](1000) ,
	[BufferResidentialFeet] [varchar](10) ,
	[BufferResidentialNote] [nvarchar](1000) ,
	[BufferRoadFeet] [varchar](10) ,
	[BufferRoadNote] [nvarchar](1000) ,

	[FacililtyGrPermit] [varchar](5) ,
	[FacililtyGrZoningInd] [varchar](10) ,
	[FacililtyGrZoningCom] [varchar](10) ,
	[FacililtyGrLimit] [varchar](10) ,
	[FacililtyGrNote] [nvarchar](1000) ,

	[FacililtyProvPermit] [varchar](5) ,
	[FacililtyProvZoningInd] [varchar](10) ,
	[FacililtyProvZoningCom] [varchar](10) ,
	[FacililtyProvLimit] [varchar](10) ,
	[FacililtyProvNote] [nvarchar](1000) ,

	[FacililtyProcPermit] [varchar](5) ,
	[FacililtyProcZoningInd] [varchar](10) ,
	[FacililtyProcZoningCom] [varchar](10) ,
	[FacililtyProcLimit] [varchar](10) ,
	[FacililtyProcNote] [nvarchar](1000) ,

	[FacililtySCPermit] [varchar](5) ,
	[FacililtySCZoningInd] [varchar](10) ,
	[FacililtySCZoningCom] [varchar](10) ,
	[FacililtySCLimit] [varchar](10) ,
	[FacililtySCNote] [nvarchar](1000) ,

	[FacililtySTPermit] [varchar](5) ,
	[FacililtySTZoningInd] [varchar](10) ,
	[FacililtySTZoningCom] [varchar](10) ,
	[FacililtySTLimit] [varchar](10) ,
	[FacililtySTNote] [nvarchar](1000) ,
	[USR_CRTN_ID] [varchar](50)  DEFAULT (user_name()) ,
	[USR_CRTN_TS] [datetime] DEFAULT (getdate()),
	[USR_MDFN_ID] [varchar](50) DEFAULT (user_name()),
	[USR_MDFN_TS] [datetime] DEFAULT (getdate())
) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO

ALTER table [dbo].[CITY_Ordinance] add  CityFileName varchar(200)


ALTER table [dbo].[CITY_Ordinance] alter column  FacililtyGrZoningInd varchar(100)
ALTER table [dbo].[CITY_Ordinance] alter column  FacililtyGrZoningCom varchar(100)

ALTER table [dbo].[CITY_Ordinance] alter column  FacililtyProvZoningInd varchar(100)
ALTER table [dbo].[CITY_Ordinance] alter column  FacililtyProvZoningCom varchar(100)



ALTER table [dbo].[CITY_Ordinance] alter column  FacililtyProcZoningInd varchar(100)
ALTER table [dbo].[CITY_Ordinance] alter column  FacililtyProcZoningCom varchar(100)

ALTER table [dbo].[CITY_Ordinance] alter column  FacililtySCZoningInd varchar(100)
ALTER table [dbo].[CITY_Ordinance] alter column  FacililtySCZoningCom varchar(100)

ALTER table [dbo].[CITY_Ordinance] alter column  FacililtySTZoningInd varchar(100)
ALTER table [dbo].[CITY_Ordinance] alter column  FacililtySTZoningCom varchar(100)


ALTER TABLE [dbo].[MeetingNote] ALTER COLUMN NOTES NVARCHAR(2000)


ALTER table [dbo].[CITY_Ordinance] alter column  BufferSchoolFeet int
ALTER table [dbo].[CITY_Ordinance] alter column  BufferDaycareFeet int
ALTER table [dbo].[CITY_Ordinance] alter column  BufferParkFeet int
ALTER table [dbo].[CITY_Ordinance] alter column  BufferSDMFeet int
ALTER table [dbo].[CITY_Ordinance] alter column  BufferReligiousFeet int
ALTER table [dbo].[CITY_Ordinance] alter column  BufferResidentialFeet int
ALTER table [dbo].[CITY_Ordinance] alter column  BufferRoadFeet int
ALTER table [dbo].[CITY_Ordinance] alter column  BufferOtherFeet int


ALTER table [dbo].[MeetingNote] add Tags varchar(100)

ALTER table [dbo].[MeetingNote] add FutureDate varchar(20)

--2017-10-23