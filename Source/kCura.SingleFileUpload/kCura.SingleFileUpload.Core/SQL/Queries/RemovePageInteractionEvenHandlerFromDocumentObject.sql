DECLARE @EHArtictID INT = (SELECT TOP 1 ArtifactID
							FROM [EDDSDBO].[ActiveSyncs] WITH (NOLOCK)
							WHERE ClassName ='kCura.SingleFileUpload.Resources.EventHandlers.DocumentPageInteractionEventHandler')

DELETE
FROM [EDDSDBO].[ApplicationEventHandler]
WHERE EventHandlerArtifactID = @EHArtictID

DELETE
FROM [EDDSDBO].[ActiveSyncs]
WHERE ArtifactID = @EHArtictID