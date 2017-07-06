SELECT 
	COUNT(1)
FROM 
	eddsdbo.[GroupUser] GU WITH(NOLOCK)
WHERE 
	GU.GroupArtifactID = 20
	AND
	GU.UserArtifactID = @UserId