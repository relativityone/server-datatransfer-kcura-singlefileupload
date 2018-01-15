DECLARE @Value VARCHAR(MAX)= '{  
   "fileExtension":{  
      "value":"File%Extension",
      "default":"File Extension"
   },
   "fileName":{  
      "value":"File%Name",
      "default":"File Name"
   },
   "fileSize":{  
      "value":"File%Size",
      "default":"File Size"
   }
}';
DECLARE @Name VARCHAR(100)= 'SFUDefaultFieldNames';
DECLARE @Section VARCHAR(100)= 'kCura.EDDS.Web';
DECLARE @ArtifactID INT;

IF NOT EXISTS ( SELECT TOP 1 1 FROM eddsdbo.InstanceSetting WITH (nolock) WHERE name = @Name AND Section = @Section)
    BEGIN
        INSERT INTO eddsdbo.Artifact
        (ArtifactTypeID,
         ParentArtifactID,
         AccessControlListID,
         AccessControlListIsInherited,
         CreatedOn,
         LastModifiedOn,
         LastModifiedBy,
         CreatedBy,
         TextIdentifier,
         ContainerID,
         Keywords,
         Notes,
         DeleteFlag
        )
        VALUES
        (42,
         62,
         1,
         1,
         GETUTCDATE(),
         GETUTCDATE(),
         9,
         9,
         @Name,
         62,
         '',
         '',
         0
        );

         SET @ArtifactID= @@identity;
        
		INSERT INTO eddsdbo.InstanceSetting
        (Section,
         Name,
         MachineName,
         Value,
         Description,
         InitialValue,
         ArtifactID
        )
        VALUES
        (@Section,
         @Name,
         '',
         @Value,
         '',
         'blank',
         @ArtifactID
        );
		INSERT INTO EDDSDBO.ZCODEARTIFACT_9 (CODEARTIFACTID, ASSOCIATEDARTIFACTID)
		SELECT TOP 1 ARTIFACTID, @ARTIFACTID FROM EDDSDBO.CODE WITH (NOLOCK) WHERE CODETYPEID = 9 AND NAME = 'Text'
    END;