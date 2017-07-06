 DELETE FROM [EDDSDBO].[Redaction]
 WHERE ID IN (
			 SELECT R.ID
			 FROM [EDDSDBO].[Document] AS D WITH(NOLOCK)
			 INNER JOIN [EDDSDBO].[File] AS F WITH(NOLOCK)
			 ON D.ArtifactID = F.DocumentArtifactID
			 INNER JOIN [EDDSDBO].[Redaction] AS R WITH(NOLOCK)
			 ON R.FileGuid = F.[Guid]
			 WHERE D.ArtifactID = @DocumentID)

UPDATE [EDDSDBO].[ProductionInformation]
SET HasRedactions = 0
WHERE Document = @DocumentID OR Document = @DocumentTempID