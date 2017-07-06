
IF @New = 1
BEGIn
	UPDATE EDDSDBO.[Artifact]
	SET [CreatedBy] = @UserID,
		[LastModifiedBy] = @UserID
	WHERE ARTIFACTID = @DocumentID
END
ELSE
BEGIN
	UPDATE EDDSDBO.[Artifact]
	SET [LastModifiedBy] = @UserID
	WHERE ARTIFACTID = @DocumentID
END