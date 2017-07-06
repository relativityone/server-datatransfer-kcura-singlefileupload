	SELECT OT.[DescriptorArtifactTypeID] ,
			OT.[Name]
				FROM eddsdbo.[ObjectType] OT
				inner join eddsdbo.ArtifactGuid AG on AG.ArtifactID = OT.ArtifactID
				WHERE [ArtifactGuid] = @artifactGuid