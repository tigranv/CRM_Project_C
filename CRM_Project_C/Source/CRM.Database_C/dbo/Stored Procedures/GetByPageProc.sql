CREATE PROCEDURE [dbo].[GetByPageProc]
    @startFrom int,
    @numberOfRows int,
    @flag bit
AS
BEGIN
IF @flag = 0
SELECT * FROM [dbo].[Contacts]
ORDER BY ContactID ASC OFFSET @startFrom ROWS FETCH NEXT @numberOfRows ROWS ONLY
ELSE
SELECT * FROM [dbo].[Contacts]
ORDER BY ContactID DESC OFFSET @startFrom ROWS FETCH NEXT @numberOfRows ROWS ONLY
END