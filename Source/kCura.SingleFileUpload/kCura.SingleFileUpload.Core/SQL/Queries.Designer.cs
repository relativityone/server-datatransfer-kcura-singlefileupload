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
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Queries {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Queries() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
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
        ///WHERE Document = @DocumentID OR Document = @DocumentTempID.
        /// </summary>
        internal static string DeleteDocumentRedactions {
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
        internal static string DocumentHasRedactions {
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
        internal static string GetArtifactTypeByArtifactGuid {
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
        internal static string GetClientCredentials {
            get {
                return ResourceManager.GetString("GetClientCredentials", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT ARTIFACTID
        ///FROM EDDSDBO.FIELD AS F WITH (NOLOCK)
        ///WHERE F.FieldArtifactTypeID = 10
        ///AND F.FieldCategoryID = @CATEGORYID.
        /// </summary>
        internal static string GetDocumentIdentifierField {
            get {
                return ResourceManager.GetString("GetDocumentIdentifierField", resourceCulture);
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
        internal static string GetDroppedFolder {
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
        internal static string GetFieldIDByNameAndType {
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
        internal static string GetFieldInfoByGuid {
            get {
                return ResourceManager.GetString("GetFieldInfoByGuid", resourceCulture);
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
        internal static string GetFileInfoByDocumentArtifactID {
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
        internal static string GetIsSystemAdminUser {
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
        internal static string GetMatchedFields {
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
        internal static string GetObjectTypeByGuid {
            get {
                return ResourceManager.GetString("GetObjectTypeByGuid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT DEFAULTFILELOCATIONNAME
        ///FROM EDDSDBO.EXTENDEDCASE AS EC WITH (NOLOCK)
        ///WHERE ARTIFACTID = @AID.
        /// </summary>
        internal static string GetRepoLocationByCaseID {
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
        internal static string GetWorkspaceGuidByArtifactID {
            get {
                return ResourceManager.GetString("GetWorkspaceGuidByArtifactID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT ARTIFACTID, NAME
        ///FROM EDDSDBO.[CASE] WITH (NOLOCK) -- ALWAYS USE WITH (NOLOCK).
        /// </summary>
        internal static string GetWorkspaceNames {
            get {
                return ResourceManager.GetString("GetWorkspaceNames", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @Value VARCHAR(MAX)= &apos;{
        ///                &quot;url&quot;:  &quot;%ApplicationPath%/custompages/1738ceb6-9546-44a7-8b9b-e64c88e47320/sfu.html?%AppID%&quot;,
        ///                &quot;id&quot;: &quot;documentCreateModal&quot;,        
        ///                &quot;height&quot;: 335,
        ///                &quot;width&quot;: 400,
        ///				&quot;hideClose&quot;: true
        ///}&apos;;
        ///DECLARE @Name VARCHAR(100)= &apos;DocumentCreateHref&apos;;
        ///DECLARE @Section VARCHAR(100)= &apos;kCura.EDDS.Web&apos;;
        ///IF EXISTS
        ///(
        ///    SELECT TOP 1 1
        ///    FROM eddsdbo.InstanceSetting WITH (nolock)
        ///    WHERE name = @Name
        ///          AND Section = [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string InsertInstanceSettings {
            get {
                return ResourceManager.GetString("InsertInstanceSettings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE 
        ///FROM 
        ///	[EDDSDBO].[File]
        ///WHERE
        ///	DocumentArtifactID = @odocartifactID
        ///	AND
        ///	[Type] = 1
        ///
        ///
        ///UPDATE
        ///	[EDDSDBO].[File]
        ///SET
        ///	DocumentArtifactID = @odocartifactID
        ///WHERE
        ///	DocumentArtifactID = @tdocartifactID
        ///	AND
        ///	[Type] = 1
        ///
        ///UPDATE
        ///	[EDDSDBO].[Document]
        ///SET
        ///	[RelativityImageCount] = (SELECT TOP 1 [RelativityImageCount]
        ///							  FROM [EDDSDBO].[Document] WITH (NOLOCK)
        ///							  WHERE ArtifactID = @tdocartifactID)
        ///WHERE
        ///	ArtifactID = @odocartifactID
        ///
        ///
        ///DECLARE @HasImagesCodeType INT = [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ReplaceDocumentImages {
            get {
                return ResourceManager.GetString("ReplaceDocumentImages", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM EDDSDBO.[FILE] WHERE FILEID = @FID
        ///INSERT INTO EDDSDBO.[FILE] ([GUID],[DOCUMENTARTIFACTID],[FILENAME],[ORDER],[TYPE],[ROTATION],[IDENTIFIER],[LOCATION],[INREPOSITORY],[SIZE]) 
        ///	VALUES (@RG, @AID, @FN, 0,0,-1,&apos;DOC&apos; + CAST(@AID AS VARCHAR(10)) + &apos;_NATIVE&apos;, @LOC, 1, @SZ)
        ///
        ///UPDATE EDDSDBO.[DOCUMENT]
        ///SET [FILEICON] = @FN,
        ///	[RELATIVITYNATIVETYPE] = @RNT
        ///WHERE ARTIFACTID = @AID.
        /// </summary>
        internal static string ReplaceNativeFile {
            get {
                return ResourceManager.GetString("ReplaceNativeFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///IF @Type = 0
        ///BEGIn
        ///	UPDATE EDDSDBO.[Artifact]
        ///	SET [CreatedBy] = @UserID,
        ///		[LastModifiedBy] = @UserID
        ///	WHERE ARTIFACTID = @AID
        ///END
        ///ELSE
        ///BEGIN
        ///	UPDATE EDDSDBO.[Artifact]
        ///	SET [LastModifiedBy] = @UserID
        ///	WHERE ARTIFACTID = @AID
        ///END.
        /// </summary>
        internal static string UpdateDocumentLastModificationFields {
            get {
                return ResourceManager.GetString("UpdateDocumentLastModificationFields", resourceCulture);
            }
        }
    }
}