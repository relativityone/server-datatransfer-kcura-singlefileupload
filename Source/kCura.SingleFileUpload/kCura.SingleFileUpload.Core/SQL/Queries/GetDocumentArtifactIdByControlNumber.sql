 DECLARE 
	     @CNField VARCHAR(200)

 SELECT 
	@CNField = AVM.ColumnName 
 FROM 
	EDDSDBO.Field AS F WITH (NOLOCK)
INNER JOIN 
	EDDSDBO.ArtifactViewField AS AVM WITH (NOLOCK) 
	ON 
	F.ArtifactViewFieldID = AVM.ArtifactViewFieldID
 WHERE 
	FieldArtifactTypeID = 10 
	AND 
	FieldCategoryID = 2

EXEC('SELECT ArtifactID FROM EDDSDBO.Document WITH (NOLOCK) WHERE '+@CNField+'='''+@ControlNumber+'''')