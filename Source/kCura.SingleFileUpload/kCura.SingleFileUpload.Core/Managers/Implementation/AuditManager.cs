﻿using kCura.SingleFileUpload.Core.Entities.Enumerations;
using kCura.SingleFileUpload.Core.SQL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Management;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
	public class AuditManager : BaseManager, IAuditManager
	{
		private string _recordOrigination;

		private static readonly Lazy<IAuditManager> _INSTANCE = new Lazy<IAuditManager>(() => new AuditManager());
		public static IAuditManager instance => _INSTANCE.Value;

		public void CreateAuditRecord(int workspaceId, int artifactID, AuditAction action, string details, int userID)
		{
			SqlParameter[] parameters =
				{
					new SqlParameter("@ArtifactID", SqlDbType.Int) {Value = artifactID},
					new SqlParameter("@Action", SqlDbType.Int) {Value = action},
					new SqlParameter("@Details", SqlDbType.NVarChar) {Value = details},
					new SqlParameter("@UserID", SqlDbType.Int) {Value = userID},
					new SqlParameter("@TimeStamp", SqlDbType.DateTime) {Value = DateTime.UtcNow},
					new SqlParameter("@RequestOrigination", SqlDbType.NVarChar) {Value = GetRequestOrigination()},
					new SqlParameter("@RecordOrigination", SqlDbType.NVarChar) {Value = GetRecordOrigination()},
				};
			_Repository.MasterDBContext.ExecuteNonQuerySQLStatement(Queries.InsertAuditRecord, parameters);
		}

		/// <summary>
		/// GenerateAuditDetailsForFileUpload - this creates the content for the Details field
		/// </summary>
		/// <param name="filePath">Full path and file name of the file being replaced</param>
		/// <param name="fileId">An Invariant File ID, typically only supplied when uploading via a processing error (use the file ID field on the error object).  Set to zero otherwise.</param>
		/// <param name="message">A message indicating the operation being performed, for example "Replacing Image File" or "Replacing Processing Error File"</param>
		///  Please make the message useful and something an administrator will understand.
		/// <returns> the details to use in CreateAuditRecord</returns>
		public string GenerateAuditDetailsForFileUpload(string filePath, int fileId, string message)
		{
			System.IO.StringWriter stringWriter = new StringWriter();
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(stringWriter);
			xmlWriter.WriteStartElement("auditElement");
			xmlWriter.WriteStartElement("fileUpload");
			xmlWriter.WriteAttributeString("filePath", filePath);
			xmlWriter.WriteAttributeString("fileId", fileId.ToString());
			xmlWriter.WriteAttributeString("message", message);
			xmlWriter.WriteEndElement();
			xmlWriter.Flush();
			xmlWriter.Close();
			stringWriter.Flush();
			return stringWriter.ToString();

		}


		private string GetRequestOrigination()
		{
			//			var ip = System.Web.HttpContext.Current.Request.UserHostAddress;
			//			var page = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
			return string.Format("<auditElement><RequestOrigination><IP>{0}</IP><Prefix /><Page>{1}</Page></RequestOrigination></auditElement>", "ip", "SFU");
		}


		private string GetRecordOrigination()
		{
			if (string.IsNullOrWhiteSpace(_recordOrigination))
			{
				string mac = string.Empty,
				ip = string.Empty,
				machine = Environment.MachineName;
				try
				{
					var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
					foreach (ManagementObject mo in mc.GetInstances())
					{
						if ((bool)(mo["IPEnabled"]))
						{
							mac = mo["MacAddress"].ToString();
							ip = ((string[])(mo["IPAddress"]))[0].ToString();
						}
					}
				}
				catch (Exception)
				{
					mac = ip = "unknown";
				}

				_recordOrigination = string.Format("<auditElement><RecordOrigination><MAC>{0}</MAC><IP>{1}</IP><Server>{2}</Server></RecordOrigination></auditElement>",
					mac, ip, machine);
			}

			return _recordOrigination;
		}
	}
}
