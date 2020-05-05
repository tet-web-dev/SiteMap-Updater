USE [mydbname]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		tet
-- Create date: 7-18-2016
-- Description:	build sitemap urls Return one row for each Article on the website. Note the union select returns a record for the home page
-- NOTE THE APPLICAITON READS ONLY THE FIRST COLUMN [0] RETURNED INT THE RESULT SET.
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetSiteMap]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	
	SELECT '<url><loc>http://MyWebSite.com/'+ replace(title,' ','-') +'</loc><changefreq>weekly</changefreq></url>' FROM WebArticles w 
	    WHERE w.active = 1
	    UNION SELECT '<url><loc>http://MyWebSite.com</loc><changefreq>daily</changefreq></url>'
END
GO
