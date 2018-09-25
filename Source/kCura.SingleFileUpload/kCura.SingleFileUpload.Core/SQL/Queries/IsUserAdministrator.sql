--Validation if the user belong of workspace group administrator
DECLARE @GroupAdminArtifactID INT
SELECT	@GroupAdminArtifactID = C.[WorkspaceAdminGroupID]
FROM	[eddsdbo].[Case] AS C WITH(NOLOCK)
WHERE	C.ArtifactID = @workspaceArtifactID

--return true o false if is user administrator or his group is workspace group administrator
SELECT	CAST(CASE WHEN COUNT(G.ArtifactID) > 0 THEN 1 ELSE 0 END AS BIT) AS HasPermission
FROM	EDDSDBO.[Group] AS G WITH(NOLOCK)
WHERE	G.ArtifactID IN (
			--User group
			SELECT	GU.GroupArtifactID
			FROM	EDDSDBO.[GroupUser] AS GU WITH(NOLOCK)
			WHERE	GU.UserArtifactID = @userArtifactID
			GROUP BY GU.GroupArtifactID
		) AND (G.GroupType = 1 OR G.ArtifactID = @GroupAdminArtifactID)