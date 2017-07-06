SELECT TOP 1
	   FileID
      ,DocumentArtifactID
      ,[Filename]
      ,[Location]
  FROM 
	[EDDSDBO].[File]
  WHERE
	DocumentArtifactID = @documentArtifactId
	AND
	[Type] = 0