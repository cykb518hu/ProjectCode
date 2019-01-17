
IF EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[GET_DOC_MeetingNote]') AND TYPE IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GET_DOC_MeetingNote]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GET_DOC_MeetingNote]
(
@OrderByField nvarchar(100)=null,
@CityName varchar(max) =null,
@CountyName varchar(max) =null,
@KeyWord varchar(max) =null,
@StartNoteDate varchar(50)=null,
@EndNoteDate varchar(50)=null,
@DeployeDate varchar(50)=null,
@Notes varchar(200)=null,
@offset int =0,
@limit int=10,
@Total int =1,
@UserEmail varchar(100)=null
)
as
declare @sqlstr varchar(max)
begin

set @sqlstr='
SELECT DISTINCT D.DOC_GUID, D.CITY_NM , D.DOC_TYPE,
D.MEETING_DATE,D.USR_CRTN_TS AS SEARCH_DATE, C.DEPLOYE_DATE, case when( m.Notes is not null) then 0
else 1 end as noteorder
 FROM  DBO.DOCUMENT D inner JOIN DBO.MeetingNote M ON M.DOC_GUID=D.DOC_GUID 
 INNER JOIN DBO.CITY C ON C.CITY_NM=D.CITY_NM  INNER JOIN DBO.ACCOUNT_CITY AC ON AC.City_Guid=C.GUID  where 1=1 and  D.IMPORTANT=''false'' '

if @CityName is not null 
	begin
		set @sqlstr=@sqlstr+' and D.CITY_NM IN ('+ @CityName+')'
	end
if @CountyName is not null 
	begin
		set @sqlstr=@sqlstr+' and C.COUNTY_NM IN ('+ @CountyName+')'
	end
if @StartNoteDate is not null 
	begin
		set @sqlstr=@sqlstr+' and M.USR_MDFN_TS >= '''+ @StartNoteDate+''''
	end
if @EndNoteDate is not null 
	begin
		set @sqlstr=@sqlstr+' and M.USR_MDFN_TS <= '''+ @EndNoteDate+''''
	end
if @DeployeDate is not null 
	begin
		set @sqlstr=@sqlstr+' and C.DEPLOYE_DATE IN ('+ @DeployeDate+')'
	end

if @Notes is not null
   begin
		set @sqlstr=@sqlstr+' and M.NOTES LIKE ''%'+ @Notes+'%'''
	end
if @UserEmail is not null
   begin
		set @sqlstr=@sqlstr+' and AC.EMAIL = '''+ @UserEmail+''''
	end

if(@Total=0)
begin

set @sqlstr ='SELECT * FROM (SELECT *,Row_number() over(order by '+@OrderByField+') AS IDRank from ('+ @sqlstr+') lst)  as IDWithRowNumber where IDRank > '+ cast(@offset as varchar) +' and IDRank<='+ cast((@offset+@limit) as varchar) +''
exec(@sqlstr)
end
else

begin
set @sqlstr = 'select count (*) from ('+@sqlstr+') lst'
exec(@sqlstr)
end

end

GO



IF EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[GET_MeetingCalendar]') AND TYPE IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GET_MeetingCalendar]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GET_MeetingCalendar]
(
@CityName varchar(max) =null,
@CountyName varchar(max) =null,
@KeyWord varchar(max) =null,
@StartMeetingDate varchar(50)=null,
@EndMeetingDate varchar(50)=null,
@OptStatus varchar(50)=null,
@FacilityType varchar(500)=null,
@DeployeDate varchar(50)=null,
@UserEmail varchar(100)=null,
@State varchar(50)=null
)
as
declare @sqlstr varchar(max)
begin

set @sqlstr='
SELECT DISTINCT M.FutureDate,M.Notes, D.CITY_NM,D.DOC_TYPE,D.DOC_GUID
 FROM  DBO.DOCUMENT D INNER JOIN DBO.DOCUMENT_CONTENT DC ON DC.DOC_GUID=D.DOC_GUID 
 inner JOIN DBO.MeetingNote M ON M.DOC_GUID=D.DOC_GUID 
 INNER JOIN DBO.CITY C ON C.CITY_NM=D.CITY_NM  INNER JOIN DBO.ACCOUNT_CITY AC ON AC.City_Guid=C.GUID  
  LEFT JOIN DBO.CITY_Ordinance CO ON CO.city_guid=C.GUID where 1=1 and  D.IMPORTANT=''false'' and M.FutureDate is not null and M.FutureDate <> '''' '

if @CityName is not null 
	begin
		set @sqlstr=@sqlstr+' and D.CITY_NM IN ('+ @CityName+')'
	end
if @CountyName is not null 
	begin
		set @sqlstr=@sqlstr+' and C.COUNTY_NM IN ('+ @CountyName+')'
	end
if @KeyWord is not null 
	begin
		set @sqlstr=@sqlstr+' AND CONTAINS (DC.CONTENT,'''+@KeyWord+''')'
	end
if @StartMeetingDate is not null 
	begin
		set @sqlstr=@sqlstr+' and D.MEETING_DATE >= '''+ @StartMeetingDate+''''
	end
if @EndMeetingDate is not null 
	begin
		set @sqlstr=@sqlstr+' and D.MEETING_DATE <= '''+ @EndMeetingDate+''''
	end
if @DeployeDate is not null 
	begin
		set @sqlstr=@sqlstr+' and C.DEPLOYE_DATE IN ('+ @DeployeDate+')'
	end
if @OptStatus is not null 
	begin
		set @sqlstr=@sqlstr+' and CO.OptStatus IN ('+ @OptStatus+')'
	end
if @FacilityType is not null 
	begin
		set @sqlstr=@sqlstr + @FacilityType
	end
if @UserEmail is not null
   begin
		set @sqlstr=@sqlstr+' and AC.EMAIL = '''+ @UserEmail+''''
	end
if @State is not null
   begin
		set @sqlstr=@sqlstr+' and C.STATES = '''+ @State+''''
	end


begin

exec(@sqlstr)
end
end

GO


GO


