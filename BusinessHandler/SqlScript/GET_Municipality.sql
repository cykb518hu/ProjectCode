IF EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[GET_Municipality]') AND TYPE IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GET_Municipality]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GET_Municipality]
(
@CityName varchar(max) =null,
@CountyName varchar(max) =null,
@KeyWord varchar(max) =null,
@StartMeetingDate varchar(50)=null,
@EndMeetingDate varchar(50)=null,
@OptStatus varchar(50)=null,
@FacilityType varchar(500)=null,
@DeployeDate varchar(50)=null,
@IsChecked varchar(5)=null,
@IsImportant varchar(5)=null,
@UserEmail varchar(100)=null,
@State varchar(50)=null
)
as
declare @sqlstr varchar(max)
begin

set @sqlstr='
SELECT distinct C.LONG_NM, objectid, C.color, c.city_nm,C.GUID
 FROM  DBO.DOCUMENT D  INNER JOIN DBO.DOCUMENT_CONTENT DC ON DC.DOC_GUID=D.DOC_GUID 
 INNER JOIN DBO.CITY C ON C.CITY_NM=D.CITY_NM INNER JOIN DBO.ACCOUNT_CITY AC ON AC.City_Guid=C.GUID 
 LEFT JOIN DBO.CITY_Ordinance CO ON CO.city_guid=C.GUID  where 1=1'


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
if @OptStatus is not null 
	begin
		set @sqlstr=@sqlstr+' and CO.OptStatus IN ('+ @OptStatus+')'
	end
if @FacilityType is not null 
	begin
		set @sqlstr=@sqlstr + @FacilityType
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
begin
exec(@sqlstr)
end

end

GO


