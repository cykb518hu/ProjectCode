
IF EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[GET_DOC_CONTENT]') AND TYPE IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GET_DOC_CONTENT]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GET_DOC_CONTENT]
(
@OrderByField nvarchar(100)=null,
@CityName varchar(max) =null,
@CountyName varchar(max) =null,
@KeyWord varchar(max) =null,
@StartMeetingDate varchar(50)=null,
@EndMeetingDate varchar(50)=null,
@DeployeDate varchar(50)=null,
@IsChecked varchar(5)=null,
@IsImportant varchar(5)=null,
@offset int =0,
@limit int=10,
@Total int =1,
@UserEmail varchar(100)=null,
@State varchar(50)=null,
@ObjectIds varchar(max)=null
)
as
declare @sqlstr varchar(max)
begin

set @sqlstr='
SELECT DISTINCT D.DOC_GUID, D.CITY_NM , D.DOC_TYPE,D.DOC_SOURCE, D.DOC_PATH,D.CHECKED,D.IMPORTANT,D.READABLE,
Q.MEETING_DATE,Q.SEARCH_DATE, C.DEPLOYE_DATE,D.COMMENT,C.LONG_NM,C.OBJECTID
 FROM  DBO.DOCUMENT D INNER JOIN DBO.QUERY Q ON D.DOC_GUID=Q.DOC_GUID 
  INNER JOIN DBO.DOCUMENT_CONTENT DC ON DC.DOC_GUID=Q.DOC_GUID 
  INNER JOIN DBO.CITY C ON C.CITY_NM=D.CITY_NM INNER JOIN DBO.ACCOUNT_CITY AC ON AC.City_Guid=C.GUID  where 1=1'

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
		set @sqlstr=@sqlstr+' and Q.MEETING_DATE >= '''+ @StartMeetingDate+''''
	end
if @EndMeetingDate is not null 
	begin
		set @sqlstr=@sqlstr+' and Q.METING_DATE <= '''+ @EndMeetingDate+''''
	end
if @DeployeDate is not null 
	begin
		set @sqlstr=@sqlstr+' and C.DEPLOYE_DATE IN ('+ @DeployeDate+')'
	end
if @IsChecked is not null
   begin
		set @sqlstr=@sqlstr+' and D.CHECKED = '''+ @IsChecked+''''
	end
if @IsImportant is not null
   begin
		set @sqlstr=@sqlstr+' and D.IMPORTANT = '''+ @IsImportant+''''
	end
if @UserEmail is not null
   begin
		set @sqlstr=@sqlstr+' and AC.EMAIL = '''+ @UserEmail+''''
	end
if @State is not null
   begin
		set @sqlstr=@sqlstr+' and C.STATES = '''+ @State+''''
	end

if @ObjectIds is not null
   begin
		set @sqlstr=@sqlstr+' and C.OBJECTID ('+ @ObjectIds+')'
	end

if(@Total=0)
begin
set @sqlstr ='SELECT * FROM (SELECT *,Row_number() over(order by '+@OrderByField+') AS IDRank from ('+ @sqlstr+') lst)  as IDWithRowNumber where IDRank > '+ cast(@offset as varchar) +' and IDRank<='+ cast((@offset+@limit) as varchar) +''
exec(@sqlstr)
end
else

begin
set @sqlstr = 'select count (*) from ('+@sqlstr+') lst'
print (@sqlstr)
exec(@sqlstr)
end

end


IF EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[GET_DOC_CONTENT_SUBLIST]') AND TYPE IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GET_DOC_CONTENT_SUBLIST]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GET_DOC_CONTENT_SUBLIST]
(
@DocIdList varchar(max) =null,
@KeyWord varchar(max) =null
)
as

begin

declare @sqlstr varchar(max)
begin

set @sqlstr='
SELECT  DC.PAGE_NUMBER, DC.CONTENT, DC.CONTENT_ID
 FROM  DBO.DOCUMENT_CONTENT DC INNER JOIN DBO.DOCUMENT D ON DC.DOC_GUID=D.DOC_GUID 
 where 1=1 '

 if @DocIdList is not null 
	begin
		set @sqlstr=@sqlstr+' and DC.DOC_GUID IN ('+ @DocIdList+')'
	end
if @KeyWord is not null 
	begin
	
		set @sqlstr=@sqlstr+' AND CONTAINS (DC.CONTENT,'''+@KeyWord+''')'
	end
 exec(@sqlstr)
end
end
GO