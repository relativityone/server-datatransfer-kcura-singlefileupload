DECLARE @WsID INT 

SET @WsID = (SELECT TOP 1 ArtifactID
 					  FROM [EDDSDBO].[Artifact] WITH(NOLOCK)
					  WHERE ArtifactTypeID = 8)



  SELECT ArtifactID, Name
  FROM (
	  SELECT DISTINCT(AC.[PermissionID]) AS ArtifactID, P.Name, [EDDSDBO].HasPermission(@UserID, @WsID, AC.[PermissionID]) AS HasPermission
	  FROM [EDDSDBO].[AccessControlListPermission] AS AC WITH(NOLOCK)
	  INNER JOIN [EDDSDBO].[Permission] AS P WITH(NOLOCK)
	  ON P.[PermissionID] = AC.[PermissionID]
	  AND AC.[PermissionID] IN (SELECT [PermissionID] 
								FROM [EDDSDBO].[ArtifactTypePermission]  WITH(NOLOCK) 
								WHERE ArtifactTypeID IN (10))) AS [Permissions]
WHERE [Permissions].HasPermission = 1
