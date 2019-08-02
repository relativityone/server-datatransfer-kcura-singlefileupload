using kCura.SingleFileUpload.Core.Helpers;
using NSerio.Relativity;
using System;
using System.Data.SqlClient;
using DTOs = kCura.Relativity.Client.DTOs;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
	public abstract class BaseManager : IManager
	{
		public Repository _Repository
		{
			get
			{
				return (Repository)Repository.Instance;
			}
		}

		public int WorkspaceID
		{
			get
			{
				return _Repository.WorkspaceID;
			}
			set
			{
				_Repository.WorkspaceID = value;
			}
		}
		public int UserID { get; set; }

		public int GetArtifactTypeByArtifactGuid(string guid)
		{
			return (int)_Repository.CaseDBContext.ExecuteSqlStatementAsScalar(SQL.Queries.GetArtifactTypeByArtifactGuid,
				new SqlParameter[]
				{
					new SqlParameter("@Guid", guid)
				}
			);
		}

		public void LogError(Exception e)
		{
			var error = new DTOs.Error
			{
				FullError = e.ToString(),
				Message = EventLogHelper.GetRecursiveExceptionMsg(e),
				Server = Environment.MachineName,
				Source = "WEB - Single File Upload",
				SendNotification = false,
				Workspace = new DTOs.Workspace(-1),
				URL = string.Empty
			};
			Repository.Instance.RSAPISystem.Repositories.Error.CreateSingle(error);
			Repository.Instance.GetLogFactory().GetLogger().ForContext<DocumentManager>().LogError(e, "Something occurred in Single File Upload {@message}", e.Message);
		}
	}
}
