 DECLARE 
	     @CNField VARCHAR(200)
 SELECT 
	@CNField = REPLACE(DisplayName, ' ', '') 
 FROM 
	EDDSDBO.Field 
 WHERE 
	FieldArtifactTypeID = 10 
	AND 
	FieldCategoryID = 2

EXEC('SELECT ArtifactID FROM EDDSDBO.Document WHERE '+@CNField+'='''+@ControlNumber+'''')