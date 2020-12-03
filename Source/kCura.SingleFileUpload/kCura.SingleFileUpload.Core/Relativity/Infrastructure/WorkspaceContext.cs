using kCura.Relativity.Client;
using Relativity.API;
using System;
using System.Runtime.CompilerServices;

namespace NSerio.Relativity.Infrastructure
{
	public sealed class WorkspaceContext : IDisposable
	{
		public IDBContext CaseDBContext
		{
			get;
			private set;
		}

		public IRSAPIClient RSAPIClient
		{
			get;
			private set;
		}

		public IRSAPIClient RSAPISystem
		{
			get;
			private set;
		}

		public WorkspaceContext(int workspaceID, IRSAPIClient apiClient, IRSAPIClient apiSystem, IDBContext caseDBContext)
		{
			this.RSAPIClient = apiClient;
			this.RSAPISystem = apiSystem;
			this.CaseDBContext = caseDBContext;
		}

		public void Dispose()
		{
			this.RSAPIClient.Dispose();
			this.RSAPIClient = null;
			this.RSAPISystem.Dispose();
			this.RSAPISystem = null;
			this.CaseDBContext.ReleaseConnection();
			this.CaseDBContext = null;
		}
	}
}