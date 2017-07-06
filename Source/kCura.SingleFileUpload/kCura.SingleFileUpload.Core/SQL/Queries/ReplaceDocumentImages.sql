DELETE 
FROM 
	[EDDSDBO].[File]
WHERE
	DocumentArtifactID = @odocartifactID
	AND
	[Type] = 1


UPDATE
	[EDDSDBO].[File]
SET
	DocumentArtifactID = @odocartifactID
WHERE
	DocumentArtifactID = @tdocartifactID
	AND
	[Type] = 1

UPDATE
	[EDDSDBO].[Document]
SET
	[RelativityImageCount] = (SELECT TOP 1 [RelativityImageCount]
							  FROM [EDDSDBO].[Document] WITH (NOLOCK)
							  WHERE ArtifactID = @tdocartifactID)
WHERE
	ArtifactID = @odocartifactID


DECLARE @HasImagesCodeType INT = ( SELECT TOP 1 F.CodeTypeID
								   FROM EDDSDBO.Field AS F WITH (NOLOCK)
								   INNER JOIN EDDSDBO.ArtifactGuid AS AG WITH (NOLOCK)
								   ON AG.ArtifactID = F.ArtifactID
								   WHERE AG.ArtifactGuid = @HasImagesFieldGuid)

DECLARE @HasImagesCodeYes INT = ( SELECT TOP 1 AG.ArtifactID
								       FROM EDDSDBO.ArtifactGuid AS AG WITH (NOLOCK)
								       WHERE AG.ArtifactGuid = @HasImagesCodeYesGuid)

DECLARE @SQL VARCHAR(MAX)

SET @SQL = '

IF((SELECT COUNT(1) 
   FROM [EDDSDBO].[ZCodeArtifact_' + CONVERT(NVARCHAR, @HasImagesCodeType) + '] WITH (NOLOCK)
   WHERE [AssociatedArtifactID] = ' + CONVERT(NVARCHAR, @odocartifactID) + ') > 0)

BEGIN
		    UPDATE [EDDSDBO].[ZCodeArtifact_' + CONVERT(NVARCHAR, @HasImagesCodeType) + '] 
			SET [CodeArtifactID] = ' +  CONVERT(NVARCHAR, @HasImagesCodeYes) +
     	   'WHERE [AssociatedArtifactID] = ' + CONVERT(NVARCHAR, @odocartifactID) + '
END
ELSE
BEGIN
	 INSERT INTO [EDDSDBO].[ZCodeArtifact_' + CONVERT(NVARCHAR, @HasImagesCodeType) + '] 
			VALUES(' + CONVERT(NVARCHAR, @HasImagesCodeYes) + ',' + CONVERT(NVARCHAR, @odocartifactID) + ')
END'



EXEC(@SQL)