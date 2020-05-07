IF EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[GET_DATA_List]') AND TYPE IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GET_DATA_List]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GET_DATA_List]
(
@OrderByField nvarchar(100)=null,
@offset int =0,
@limit int=10,
@Total int =1
)
as
declare @sqlstr varchar(max)
begin

set @sqlstr='
SELECT SF.StoreName, ST.City, AP.ProductId, AP.ProductName, CR.CategoryName, PR.Brand, AP.Date FROM ActiveProduct AP
LEFT JOIN StoreFront SF ON SF.StoreId=AP.StoreId
LEFT JOIN Store ST ON SF.StoreId=ST.StoreId
LEFT JOIN Product PR ON AP.ProductId=PR.ProductId
LEFT JOIN CategoryRepository CR ON CR.CategoryId=PR.CategoryId where 1=1'


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



IF EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[GET_DATA_SUB_List]') AND TYPE IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GET_DATA_SUB_List]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GET_DATA_SUB_List]
(
@ProductIdList nvarchar(1000)=null
)
as
declare @sqlstr varchar(max)
begin

set @sqlstr='
SELECT PR.ProductId, PP.Unit,SF.Qty, SF.QtyAvailable, PP.MedicalPrice, PP.RecreationalPrice FROM Product PR 
LEFT JOIN StockInformation SF ON SF.ProductId=PR.ProductId 
LEFT JOIN ProductPrice PP ON PR.ProductId=PP.ProductId AND SF.Unit=PP.Unit where 1=1'

if @ProductIdList is not null 
	begin
		set @sqlstr=@sqlstr+' and PR.ProductId IN ('+ @ProductIdList+')'
	end
begin
print (@sqlstr)
exec(@sqlstr)
end

end