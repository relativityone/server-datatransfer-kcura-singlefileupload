BEGIN TRAN

DECLARE @Value VARCHAR(MAX)= '{
                "url":  "%ApplicationPath%/custompages/1738ceb6-9546-44a7-8b9b-e64c88e47320/sfu.html?%AppID%",
                "id": "documentCreateModal",        
                "height": 440,
                "width": 400,
				"hideClose": true
}';
DECLARE @Name VARCHAR(100)= 'DocumentCreateHref';
DECLARE @Section VARCHAR(100)= 'kCura.EDDS.Web';
IF EXISTS
(
    SELECT TOP 1 1
    FROM eddsdbo.InstanceSetting WITH (nolock)
    WHERE name = @Name
)
    BEGIN
        UPDATE eddsdbo.InstanceSetting
          SET
              Value = @Value
        WHERE name = @Name
    END;
ELSE
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
        DECLARE @ArtifactID INT= @@identity;
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

COMMIT TRAN