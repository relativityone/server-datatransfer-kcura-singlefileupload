SELECT COUNT(1)
 FROM [EDDSDBO].[Document] AS D WITH(NOLOCK)
 INNER JOIN [EDDSDBO].[File] AS F WITH(NOLOCK)
 ON D.ArtifactID = F.DocumentArtifactID
 INNER JOIN [EDDSDBO].[Redaction] AS R WITH(NOLOCK)
 ON R.FileGuid = F.[Guid]
 WHERE D.ArtifactID = @DocumentID