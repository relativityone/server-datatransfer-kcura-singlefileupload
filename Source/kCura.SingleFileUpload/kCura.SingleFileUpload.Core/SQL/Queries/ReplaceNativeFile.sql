﻿IF @FID > -1
BEGIN
	DELETE 
	FROM EDDSDBO.[FILE] 
	WHERE FILEID = @FID
END

INSERT INTO EDDSDBO.[FILE] ([GUID],[DOCUMENTARTIFACTID],[FILENAME],[ORDER],[TYPE],[ROTATION],[IDENTIFIER],[LOCATION],[INREPOSITORY],[SIZE]) 
	VALUES (@RG, @AID, @FN, 0,0,-1,'DOC' + CAST(@AID AS VARCHAR(10)) + '_NATIVE', @LOC, 1, @SZ)

UPDATE EDDSDBO.[DOCUMENT]
SET [FILEICON] = @FN,
	[RELATIVITYNATIVETYPE] = @RNT,
	[HASNATIVE] = 1
WHERE 
	ARTIFACTID = @AID