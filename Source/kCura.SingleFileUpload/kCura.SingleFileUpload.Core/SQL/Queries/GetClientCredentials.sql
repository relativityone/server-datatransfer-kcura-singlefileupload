SELECT OC.Id, OC.Secrets
FROM EDDSDBO.OAuth2Client AS OC WITH (NOLOCK)
INNER JOIN EDDSDBO.SystemArtifact AS SA WITH (NOLOCK) ON OC.ArtifactID = SA.ArtifactID
WHERE SystemArtifactIdentifier = 'SystemOAuth2Client'