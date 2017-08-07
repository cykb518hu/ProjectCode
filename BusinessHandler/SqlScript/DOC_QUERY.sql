
IF EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[GET_DOC_QUERY]') AND TYPE IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GET_DOC_QUERY]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GET_DOC_QUERY]
(
@OrderByField nvarchar(100)=null,
@CityName varchar(max) =null,
@KeyWord varchar(max) =null,
@MeetingDate varchar(50)=null,
@DeployeDate varchar(50)=null,
@IsChecked varchar(5)=null,
@IsImportant varchar(5)=null,
@offset int =0,
@limit int=10,
@Total int =1
)
as
declare @sqlstr varchar(max)
begin

set @sqlstr='
SELECT DISTINCT D.DOC_GUID, D.CITY_NM , D.DOC_TYPE,D.DOC_SOURCE, D.DOC_PATH,D.CHECKED,D.IMPORTANT,D.READABLE,
Q.MEETING_DATE,Q.SEARCH_DATE, C.DEPLOYE_DATE
 FROM  DBO.DOCUMENT D INNER JOIN DBO.QUERY Q ON D.DOC_GUID=Q.DOC_GUID INNER JOIN DBO.QUERY_ENTRY QE ON QE.QUERY_GUID=Q.QUERY_GUID 
 LEFT JOIN DBO.CITY C ON C.CITY_NM=D.CITY_NM where 1=1'

if @CityName is not null 
	begin
		set @sqlstr=@sqlstr+' and D.CITY_NM IN ('+ @CityName+')'
	end
if @KeyWord is not null 
	begin
		set @sqlstr=@sqlstr+' and QE.KEYWORD IN ('+ @KeyWord+')'
	end
if @MeetingDate is not null 
	begin
		set @sqlstr=@sqlstr+' and Q.MEETING_DATE > '''+ @MeetingDate+''''
	end
if @DeployeDate is not null 
	begin
		set @sqlstr=@sqlstr+' and C.DEPLOYE_DATE = '''+ @DeployeDate+''''
	end
if @IsChecked is not null
   begin
		set @sqlstr=@sqlstr+' and D.CHECKED = '''+ @IsChecked+''''
	end
if @IsImportant is not null
   begin
		set @sqlstr=@sqlstr+' and D.IMPORTANT = '''+ @IsImportant+''''
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

