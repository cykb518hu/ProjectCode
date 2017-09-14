
IF EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[GET_DOC_QUERY_SUBLIST]') AND TYPE IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GET_DOC_QUERY_SUBLIST]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GET_DOC_QUERY_SUBLIST]
(
@DocIdList varchar(max) =null
)
as

begin

declare @sqlstr varchar(max)
begin

set @sqlstr='
SELECT  QE.PAGE_NUMBER,QE.CONTENT,QE.COMMENT,QE.KEYWORD,QE.ENTRY_GUID,d.DOC_GUID
 FROM  DBO.DOCUMENT D INNER JOIN DBO.QUERY Q ON D.DOC_GUID=Q.DOC_GUID INNER JOIN DBO.QUERY_ENTRY QE ON QE.QUERY_GUID=Q.QUERY_GUID  
 where 1=1 '

 if @DocIdList is not null 
	begin
		set @sqlstr=@sqlstr+' and D.DOC_GUID IN ('+ @DocIdList+')'
	end
 exec(@sqlstr)
end
end
GO

IF EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[GET_DOC_NOTES_AMOUNT]') AND TYPE IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GET_DOC_NOTES_AMOUNT]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GET_DOC_NOTES_AMOUNT]
(
@DocIdList varchar(max) =null
)
as

begin

declare @sqlstr varchar(max)
begin

set @sqlstr='
SELECT Doc_Guid, count(*) count FROM MeetingNote where Doc_Guid in ('+@DocIdList+') group by  Doc_Guid '
 exec(@sqlstr)

end
end
GO

