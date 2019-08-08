INSERT INTO [EDDSDBO].[AuditRecord_PrimaryPartition] 
	([ArtifactID],[Action],[Details],[UserID],[TimeStamp],[RequestOrigination],[RecordOrigination])
VALUES 
	(@ArtifactID, @Action, @Details, @UserID, @TimeStamp, @RequestOrigination, @RecordOrigination)