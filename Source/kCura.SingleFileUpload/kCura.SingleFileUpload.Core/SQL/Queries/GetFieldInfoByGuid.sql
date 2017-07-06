SELECT
	F.ArtifactID,
	F.DisplayName
FROM 
	EDDSDBO.Field F
INNER JOIN
	EDDSDBO.ArtifactGuid AF
	ON
	AF.ArtifactID = F.ArtifactID
WHERE
	AF.ArtifactGuid = @artifactGuid