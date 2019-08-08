﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace kCura.SingleFileUpload.Core.SQL {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Queries {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Queries() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("kCura.SingleFileUpload.Core.SQL.Queries", typeof(Queries).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///DELETE FROM [EDDSDBO].[File] 
        ///WHERE [DocumentArtifactID] = @DocumentID
        ///AND [Type] = 1
        ///
        ///.
        /// </summary>
        public static string DeleteDocumentImages {
            get {
                return ResourceManager.GetString("DeleteDocumentImages", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  DELETE FROM [EDDSDBO].[Redaction]
        /// WHERE ID IN (
        ///			 SELECT R.ID
        ///			 FROM [EDDSDBO].[Document] AS D WITH(NOLOCK)
        ///			 INNER JOIN [EDDSDBO].[File] AS F WITH(NOLOCK)
        ///			 ON D.ArtifactID = F.DocumentArtifactID
        ///			 INNER JOIN [EDDSDBO].[Redaction] AS R WITH(NOLOCK)
        ///			 ON R.FileGuid = F.[Guid]
        ///			 WHERE D.ArtifactID = @DocumentID)
        ///
        ///UPDATE [EDDSDBO].[ProductionInformation]
        ///SET HasRedactions = 0
        ///WHERE Document = @DocumentID.
        /// </summary>
        public static string DeleteDocumentRedactions {
            get {
                return ResourceManager.GetString("DeleteDocumentRedactions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT COUNT(1)
        /// FROM [EDDSDBO].[Document] AS D WITH(NOLOCK)
        /// INNER JOIN [EDDSDBO].[File] AS F WITH(NOLOCK)
        /// ON D.ArtifactID = F.DocumentArtifactID
        /// INNER JOIN [EDDSDBO].[Redaction] AS R WITH(NOLOCK)
        /// ON R.FileGuid = F.[Guid]
        /// WHERE D.ArtifactID = @DocumentID.
        /// </summary>
        public static string DocumentHasRedactions {
            get {
                return ResourceManager.GetString("DocumentHasRedactions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT O.DescriptorArtifactTypeID
        ///FROM [EDDSDBO].[ArtifactGuid] AS AG WITH (NOLOCK)
        ///INNER JOIN [EDDSDBO].ObjectType AS O WITH (NOLOCK)
        ///ON AG.ArtifactID = O.ArtifactID
        ///WHERE AG.[ArtifactGuid] = @Guid
        ///
        ///.
        /// </summary>
        public static string GetArtifactTypeByArtifactGuid {
            get {
                return ResourceManager.GetString("GetArtifactTypeByArtifactGuid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT OC.Id, OC.Secrets
        ///FROM EDDSDBO.OAuth2Client AS OC WITH (NOLOCK)
        ///INNER JOIN EDDSDBO.SystemArtifact AS SA WITH (NOLOCK) ON OC.ArtifactID = SA.ArtifactID
        ///WHERE SystemArtifactIdentifier = &apos;SystemOAuth2Client&apos;.
        /// </summary>
        public static string GetClientCredentials {
            get {
                return ResourceManager.GetString("GetClientCredentials", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  DECLARE 
        ///	     @CNField VARCHAR(200)
        ///
        /// SELECT 
        ///	@CNField = AVM.ColumnName 
        /// FROM 
        ///	EDDSDBO.Field AS F WITH (NOLOCK)
        ///INNER JOIN 
        ///	EDDSDBO.ArtifactViewField AS AVM WITH (NOLOCK) 
        ///	ON 
        ///	F.ArtifactViewFieldID = AVM.ArtifactViewFieldID
        /// WHERE 
        ///	FieldArtifactTypeID = 10 
        ///	AND 
        ///	FieldCategoryID = 2
        ///
        ///EXEC(&apos;SELECT ArtifactID FROM EDDSDBO.Document WITH (NOLOCK) WHERE &apos;+@CNField+&apos;=&apos;&apos;&apos;+@ControlNumber+&apos;&apos;&apos;&apos;).
        /// </summary>
        public static string GetDocumentArtifactIdByControlNumber {
            get {
                return ResourceManager.GetString("GetDocumentArtifactIdByControlNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT ARTIFACTID
        ///FROM EDDSDBO.FIELD AS F WITH (NOLOCK)
        ///WHERE F.FieldArtifactTypeID = 10
        ///AND F.FieldCategoryID = @CATEGORYID.
        /// </summary>
        public static string GetDocumentIdentifierField {
            get {
                return ResourceManager.GetString("GetDocumentIdentifierField", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @WsID INT 
        ///
        ///SET @WsID = (SELECT TOP 1 ArtifactID
        /// 					  FROM [EDDSDBO].[Artifact] WITH(NOLOCK)
        ///					  WHERE ArtifactTypeID = 8)
        ///
        ///
        ///
        ///  SELECT ArtifactID, Name
        ///  FROM (
        ///	  SELECT DISTINCT(AC.[PermissionID]) AS ArtifactID, P.Name, [EDDSDBO].HasPermission(@UserID, @WsID, AC.[PermissionID]) AS HasPermission
        ///	  FROM [EDDSDBO].[AccessControlListPermission] AS AC WITH(NOLOCK)
        ///	  INNER JOIN [EDDSDBO].[Permission] AS P WITH(NOLOCK)
        ///	  ON P.[PermissionID] = AC.[PermissionID]
        ///	  AND AC.[Permission [rest of string was truncated]&quot;;.
        /// </summary>
        public static string GetDocumentPermissions {
            get {
                return ResourceManager.GetString("GetDocumentPermissions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @FID INT = 0
        ///
        ///SELECT @FID = ArtifactID
        ///FROM EDDSDBO.Folder AS F WITH (NOLOCK)
        ///WHERE ArtifactID = @SupID
        ///
        ///IF @FID = 0
        ///BEGIN
        ///	SELECT TOP 1  @FID = ArtifactID
        ///	FROM EDDSDBO.FOLDER AS F WITH (NOLOCK)
        ///	ORDER BY ARTIFACTID ASC
        ///END
        ///
        ///SELECT @FID AS FID.
        /// </summary>
        public static string GetDroppedFolder {
            get {
                return ResourceManager.GetString("GetDroppedFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT ARTIFACTID
        ///FROM EDDSDBO.FIELD AS F WITH (NOLOCK)
        ///WHERE F.FieldArtifactTypeID = 10
        ///AND F.FieldTypeID = @Type
        ///AND F.DisplayName IN ({0}).
        /// </summary>
        public static string GetFieldIDByNameAndType {
            get {
                return ResourceManager.GetString("GetFieldIDByNameAndType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT
        ///	F.ArtifactID,
        ///	F.DisplayName
        ///FROM 
        ///	EDDSDBO.Field F
        ///INNER JOIN
        ///	EDDSDBO.ArtifactGuid AF
        ///	ON
        ///	AF.ArtifactID = F.ArtifactID
        ///WHERE
        ///	AF.ArtifactGuid = @artifactGuid.
        /// </summary>
        public static string GetFieldInfoByGuid {
            get {
                return ResourceManager.GetString("GetFieldInfoByGuid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT TOP 1
        ///	&apos;{0}&apos; AS [Key],
        ///	DisplayName AS [Value]
        ///FROM 
        ///	EDDSDBO.Field WITH (NOLOCK)
        ///WHERE 
        ///	FieldArtifactTypeID=10
        ///	AND
        ///	DisplayName LIKE &apos;{1}&apos;.
        /// </summary>
        public static string GetFieldItem {
            get {
                return ResourceManager.GetString("GetFieldItem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT
        ///	[Value]
        ///FROM
        ///	EDDSDBO.InstanceSetting WITH (NOLOCK)
        ///WHERE
        ///	NAME = &apos;SFUDefaultFieldNames&apos;.
        /// </summary>
        public static string GetFieldsInstanceSetting {
            get {
                return ResourceManager.GetString("GetFieldsInstanceSetting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT 
        ///	[Value] 
        ///FROM 
        ///	EDDSDBO.Settings WITH (NOLOCK)
        ///WHERE 
        ///	[Name] = &apos;SFUFieldValues&apos;.
        /// </summary>
        public static string GetFieldsWorspaceSetting {
            get {
                return ResourceManager.GetString("GetFieldsWorspaceSetting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT TOP 1
        ///	   FileID
        ///      ,DocumentArtifactID
        ///      ,[Filename]
        ///      ,[Location]
        ///  FROM 
        ///	[EDDSDBO].[File]
        ///  WHERE
        ///	DocumentArtifactID = @documentArtifactId
        ///	AND
        ///	[Type] = 0.
        /// </summary>
        public static string GetFileInfoByDocumentArtifactID {
            get {
                return ResourceManager.GetString("GetFileInfoByDocumentArtifactID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT 
        ///	COUNT(1)
        ///FROM 
        ///	eddsdbo.[GroupUser] GU WITH(NOLOCK)
        ///WHERE 
        ///	GU.GroupArtifactID = 20
        ///	AND
        ///	GU.UserArtifactID = @UserId.
        /// </summary>
        public static string GetIsSystemAdminUser {
            get {
                return ResourceManager.GetString("GetIsSystemAdminUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT AVF.COLUMNNAME
        ///FROM EDDSDBO.FIELD AS F WITH (NOLOCK)
        ///INNER JOIN EDDSDBO.ARTIFACTVIEWFIELD AS AVF WITH (NOLOCK) ON F.ARTIFACTVIEWFIELDID = AVF.ARTIFACTVIEWFIELDID
        ///WHERE F.FIELDARTIFACTTYPEID = 10
        ///AND DISPLAYNAME IN ({0}).
        /// </summary>
        public static string GetMatchedFields {
            get {
                return ResourceManager.GetString("GetMatchedFields", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 	SELECT OT.[DescriptorArtifactTypeID] ,
        ///			OT.[Name]
        ///				FROM eddsdbo.[ObjectType] OT
        ///				inner join eddsdbo.ArtifactGuid AG on AG.ArtifactID = OT.ArtifactID
        ///				WHERE [ArtifactGuid] = @artifactGuid.
        /// </summary>
        public static string GetObjectTypeByGuid {
            get {
                return ResourceManager.GetString("GetObjectTypeByGuid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT DEFAULTFILELOCATIONNAME
        ///FROM EDDSDBO.EXTENDEDCASE AS EC WITH (NOLOCK)
        ///WHERE ARTIFACTID = @AID.
        /// </summary>
        public static string GetRepoLocationByCaseID {
            get {
                return ResourceManager.GetString("GetRepoLocationByCaseID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT
        ///      ArtifactGuid
        ///FROM 
        ///	eddsdbo.ArtifactGuid
        ///WHERE
        ///	ArtifactID = @artifactId.
        /// </summary>
        public static string GetWorkspaceGuidByArtifactID {
            get {
                return ResourceManager.GetString("GetWorkspaceGuidByArtifactID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT ARTIFACTID, NAME
        ///FROM EDDSDBO.[CASE] WITH (NOLOCK) -- ALWAYS USE WITH (NOLOCK).
        /// </summary>
        public static string GetWorkspaceNames {
            get {
                return ResourceManager.GetString("GetWorkspaceNames", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO [EDDSDBO].[AuditRecord_PrimaryPartition] 
        ///	([ArtifactID],[Action],[Details],[UserID],[TimeStamp],[RequestOrigination],[RecordOrigination])
        ///VALUES 
        ///	(@ArtifactID, @Action, @Details, @UserID, @TimeStamp, @RequestOrigination, @RecordOrigination).
        /// </summary>
        public static string InsertAuditRecord {
            get {
                return ResourceManager.GetString("InsertAuditRecord", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @Value VARCHAR(MAX)= &apos;{  
        ///   &quot;fileExtension&quot;:{  
        ///      &quot;value&quot;:&quot;File%Extension&quot;,
        ///      &quot;default&quot;:&quot;File Extension&quot;
        ///   },
        ///   &quot;fileName&quot;:{  
        ///      &quot;value&quot;:&quot;File%Name&quot;,
        ///      &quot;default&quot;:&quot;File Name&quot;
        ///   },
        ///   &quot;fileSize&quot;:{  
        ///      &quot;value&quot;:&quot;File%Size&quot;,
        ///      &quot;default&quot;:&quot;File Size&quot;
        ///   }
        ///}&apos;;
        ///DECLARE @Name VARCHAR(100)= &apos;SFUDefaultFieldNames&apos;;
        ///DECLARE @Section VARCHAR(100)= &apos;kCura.EDDS.Web&apos;;
        ///DECLARE @ArtifactID INT;
        ///
        ///IF NOT EXISTS ( SELECT TOP 1 1 FROM eddsdbo.InstanceSetting WITH (nolock) WHER [rest of string was truncated]&quot;;.
        /// </summary>
        public static string InsertFieldsInstanceSetting {
            get {
                return ResourceManager.GetString("InsertFieldsInstanceSetting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IF EXISTS(SELECT TOP 1 1 FROM EDDSDBO.Settings WITH (NOLOCK) WHERE [Name] = &apos;SFUFieldValues&apos;)
        ///BEGIN
        ///	UPDATE 
        ///		EDDSDBO.Settings 
        ///	SET 
        ///		[Value] = &apos;{0}&apos;
        ///	WHERE
        ///		[Name] = &apos;SFUFieldValues&apos;
        ///
        ///END
        ///ELSE
        ///BEGIN
        ///	INSERT INTO
        ///		EDDSDBO.Settings
        ///		(
        ///		[Name],
        ///		[Value]
        ///		)
        ///		VALUES(
        ///		&apos;SFUFieldValues&apos;,
        ///		&apos;{0}&apos;
        ///	)
        ///END	.
        /// </summary>
        public static string InsertFieldsWorspaceSetting {
            get {
                return ResourceManager.GetString("InsertFieldsWorspaceSetting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO [EDDSDBO].[File] ([Guid]
        ///      ,[DocumentArtifactID]
        ///      ,[Filename]
        ///      ,[Order]
        ///      ,[Type]
        ///      ,[Rotation]
        ///      ,[Identifier]
        ///      ,[Location]
        ///      ,[InRepository]
        ///      ,[Size]
        ///      ,[Details])
        ///VALUES (NEWID(), @DocumentID, @FileName, @Order, @Type, -1, @DocIdentifier, @Location, 1, @Size, &apos;&apos;).
        /// </summary>
        public static string InsertImageInFileTable {
            get {
                return ResourceManager.GetString("InsertImageInFileTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BEGIN TRAN
        ///
        ///DECLARE @Value VARCHAR(MAX)= &apos;{
        ///                &quot;url&quot;:  &quot;%ApplicationPath%/custompages/1738ceb6-9546-44a7-8b9b-e64c88e47320/sfu.html?%AppID%&quot;,
        ///                &quot;id&quot;: &quot;documentCreateModal&quot;,        
        ///                &quot;height&quot;: 440,
        ///                &quot;width&quot;: 400,
        ///				&quot;hideClose&quot;: true
        ///}&apos;;
        ///DECLARE @Name VARCHAR(100)= &apos;DocumentCreateHref&apos;;
        ///DECLARE @Section VARCHAR(100)= &apos;kCura.EDDS.Web&apos;;
        ///IF EXISTS
        ///(
        ///    SELECT TOP 1 1
        ///    FROM eddsdbo.InstanceSetting WITH (nolock)
        ///    WHERE name = @Name AN [rest of string was truncated]&quot;;.
        /// </summary>
        public static string InsertInstanceSettings {
            get {
                return ResourceManager.GetString("InsertInstanceSettings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --Validation if the user belong of workspace group administrator
        ///DECLARE @GroupAdminArtifactID INT
        ///SELECT	@GroupAdminArtifactID = C.[WorkspaceAdminGroupID]
        ///FROM	[eddsdbo].[Case] AS C WITH(NOLOCK)
        ///WHERE	C.ArtifactID = @workspaceArtifactID
        ///
        ///--return true o false if is user administrator or his group is workspace group administrator
        ///SELECT	CAST(CASE WHEN COUNT(G.ArtifactID) &gt; 0 THEN 1 ELSE 0 END AS BIT) AS HasPermission
        ///FROM	EDDSDBO.[Group] AS G WITH(NOLOCK)
        ///WHERE	G.ArtifactID IN (
        ///			--User group
        ///	 [rest of string was truncated]&quot;;.
        /// </summary>
        public static string IsUserAdministrator {
            get {
                return ResourceManager.GetString("IsUserAdministrator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @EHArtictID INT = (SELECT TOP 1 ArtifactID
        ///							FROM [EDDSDBO].[ActiveSyncs] WITH (NOLOCK)
        ///							WHERE ClassName =&apos;kCura.SingleFileUpload.Resources.EventHandlers.DocumentPageInteractionEventHandler&apos;)
        ///
        ///IF OBJECT_ID(N&apos;EDDSDBO.ApplicationEventHandler&apos;) IS NOT NULL
        ///BEGIN
        ///	DELETE
        ///	FROM [EDDSDBO].[ApplicationEventHandler]
        ///	WHERE EventHandlerArtifactID = @EHArtictID
        ///END
        ///
        ///DELETE
        ///FROM [EDDSDBO].[ActiveSyncs]
        ///WHERE ArtifactID = @EHArtictID.
        /// </summary>
        public static string RemovePageInteractionEvenHandlerFromDocumentObject {
            get {
                return ResourceManager.GetString("RemovePageInteractionEvenHandlerFromDocumentObject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IF @FID &gt; -1
        ///BEGIN
        ///	DELETE 
        ///	FROM EDDSDBO.[FILE] 
        ///	WHERE FILEID = @FID
        ///END
        ///
        ///INSERT INTO EDDSDBO.[FILE] ([GUID],[DOCUMENTARTIFACTID],[FILENAME],[ORDER],[TYPE],[ROTATION],[IDENTIFIER],[LOCATION],[INREPOSITORY],[SIZE]) 
        ///	VALUES (@RG, @AID, @FN, 0,0,-1,&apos;DOC&apos; + CAST(@AID AS VARCHAR(10)) + &apos;_NATIVE&apos;, @LOC, 1, @SZ)
        ///
        ///UPDATE EDDSDBO.[DOCUMENT]
        ///SET [FILEICON] = @FN,
        ///	[RELATIVITYNATIVETYPE] = @RNT,
        ///	[HASNATIVE] = 1
        ///WHERE 
        ///	ARTIFACTID = @AID.
        /// </summary>
        public static string ReplaceNativeFile {
            get {
                return ResourceManager.GetString("ReplaceNativeFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///IF @New = 1
        ///BEGIn
        ///	UPDATE EDDSDBO.[Artifact]
        ///	SET [CreatedBy] = @UserID,
        ///		[LastModifiedBy] = @UserID
        ///	WHERE ARTIFACTID = @DocumentID
        ///END
        ///ELSE
        ///BEGIN
        ///	UPDATE EDDSDBO.[Artifact]
        ///	SET [LastModifiedBy] = @UserID
        ///	WHERE ARTIFACTID = @DocumentID
        ///END.
        /// </summary>
        public static string UpdateDocumentLastModificationFields {
            get {
                return ResourceManager.GetString("UpdateDocumentLastModificationFields", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///UPDATE
        ///	[EDDSDBO].[Document]
        ///SET
        ///	[RelativityImageCount] = 1
        ///WHERE
        ///	ArtifactID = @DocumentID
        ///
        ///
        ///DECLARE @HasImagesCodeType INT = ( SELECT TOP 1 F.CodeTypeID
        ///								   FROM EDDSDBO.Field AS F WITH (NOLOCK)
        ///								   INNER JOIN EDDSDBO.ArtifactGuid AS AG WITH (NOLOCK)
        ///								   ON AG.ArtifactID = F.ArtifactID
        ///								   WHERE AG.ArtifactGuid = @HasImagesFieldGuid)
        ///
        ///DECLARE @HasImagesCodeYes INT = ( SELECT TOP 1 AG.ArtifactID
        ///								       FROM EDDSDBO.ArtifactGuid AS AG WITH (NOLOCK)
        ///				 [rest of string was truncated]&quot;;.
        /// </summary>
        public static string UpdateHasImages {
            get {
                return ResourceManager.GetString("UpdateHasImages", resourceCulture);
            }
        }
    }
}
