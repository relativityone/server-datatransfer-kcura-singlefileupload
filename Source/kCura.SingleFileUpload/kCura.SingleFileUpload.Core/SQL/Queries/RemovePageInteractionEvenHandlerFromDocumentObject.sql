DECLARE @EHArtictID INT = (SELECT TOP 1 ArtifactID
							FROM [EDDSDBO].[ActiveSyncs] WITH (NOLOCK)
							WHERE ClassName ='kCura.SingleFileUpload.Resources.EventHandlers.DocumentPageInteractionEventHandler')

IF OBJECT_ID(N'EDDSDBO.ApplicationEventHandler') IS NOT NULL
BEGIN
	DELETE
	FROM [EDDSDBO].[ApplicationEventHandler]
	WHERE EventHandlerArtifactID = @EHArtictID
END

DELETE
FROM [EDDSDBO].[ActiveSyncs]
WHERE ArtifactID = @EHArtictID