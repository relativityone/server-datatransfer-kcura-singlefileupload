SELECT TOP 1
	'{0}' AS [Key],
	DisplayName AS [Value]
FROM 
	EDDSDBO.Field WITH (NOLOCK)
WHERE 
	FieldArtifactTypeID=10
	AND
	DisplayName LIKE '{1}'