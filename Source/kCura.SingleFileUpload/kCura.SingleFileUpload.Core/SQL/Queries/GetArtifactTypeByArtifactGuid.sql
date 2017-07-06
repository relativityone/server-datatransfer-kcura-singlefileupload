SELECT O.DescriptorArtifactTypeID
FROM [EDDSDBO].[ArtifactGuid] AS AG WITH (NOLOCK)
INNER JOIN [EDDSDBO].ObjectType AS O WITH (NOLOCK)
ON AG.ArtifactID = O.ArtifactID
WHERE AG.[ArtifactGuid] = @Guid

