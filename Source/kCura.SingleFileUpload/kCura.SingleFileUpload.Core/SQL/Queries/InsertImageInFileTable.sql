INSERT INTO [EDDSDBO].[File] ([Guid]
      ,[DocumentArtifactID]
      ,[Filename]
      ,[Order]
      ,[Type]
      ,[Rotation]
      ,[Identifier]
      ,[Location]
      ,[InRepository]
      ,[Size]
      ,[Details])
VALUES (NEWID(), @DocumentID, @FileName, @Order, @Type, -1, @DocIdentifier, @Location, 1, @Size, '')