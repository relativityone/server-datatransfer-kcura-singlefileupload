
DELETE FROM [EDDSDBO].[File] 
WHERE [DocumentArtifactID] = @DocumentID
AND [Type] = 1

