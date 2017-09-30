

IF EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[GET_CITY_ALLNOTE]') AND TYPE IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GET_CITY_ALLNOTE]
GO
SET ANSI_NULLS ON
GO

GO
/****** Object:  StoredProcedure [dbo].[GET_CITY_ALLNOTE]    Script Date: 9/30/2017 10:28:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create PROCEDURE [dbo].[GET_CITY_ALLNOTE]
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
@CityGuid varchar(50)=null
)
as
declare @sqlstr varchar(max)
begin

set @sqlstr='
SELECT DISTINCT CO.*,C.CITY_NM
 FROM  DBO.DOCUMENT D INNER JOIN DBO.QUERY Q ON D.DOC_GUID=Q.DOC_GUID INNER JOIN DBO.QUERY_ENTRY QE ON QE.QUERY_GUID=Q.QUERY_GUID 
 INNER JOIN DBO.CITY C ON C.CITY_NM=D.CITY_NM INNER JOIN DBO.ACCOUNT_CITY AC ON AC.City_Guid=C.GUID 
 INNER JOIN DBO.CITY_Ordinance CO ON CO.CITY_GUID=C.GUID where 1=1'

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
		set @sqlstr=@sqlstr+' and QE.KEYWORD IN ('+ @KeyWord+')'
	end
if @StartMeetingDate is not null 
	begin
		set @sqlstr=@sqlstr+' and Q.MEETING_DATE >= '''+ @StartMeetingDate+''''
	end
if @EndMeetingDate is not null 
	begin
		set @sqlstr=@sqlstr+' and Q.MEETING_DATE <= '''+ @EndMeetingDate+''''
	end
if @DeployeDate is not null 
	begin
		set @sqlstr=@sqlstr+' and C.DEPLOYE_DATE IN ('+ @DeployeDate+')'
	end
if @UserEmail is not null
   begin
		set @sqlstr=@sqlstr+' and AC.EMAIL = '''+ @UserEmail+''''
	end
if @CityGuid is not null
   begin
		set @sqlstr=@sqlstr+' and C.GUID = '''+ @CityGuid+''''
	end
if(@Total=0)
begin

set @sqlstr ='SELECT * FROM (SELECT *,Row_number() over(order by '+@OrderByField+') AS IDRank from ('+ @sqlstr+') lst)  as IDWithRowNumber where IDRank > '+ cast(@offset as varchar) +' and IDRank<='+ cast((@offset+@limit) as varchar) +''
print(@sqlstr)
exec(@sqlstr)
end
else

begin
set @sqlstr = 'select count (*) from ('+@sqlstr+') lst'
exec(@sqlstr)
end

end

