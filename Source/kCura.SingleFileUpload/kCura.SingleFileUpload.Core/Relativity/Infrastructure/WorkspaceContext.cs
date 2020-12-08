using System;
using Relativity.API;

namespace kCura.SingleFileUpload.Core.Relativity.Infrastructure
{
	public sealed class WorkspaceContext : IDisposable
	{
		public IDBContext CaseDBContext
		{
			get;
			private set;
		}

		public WorkspaceContext(IDBContext caseDBContext)
		{
			CaseDBContext = caseDBContext;
		}

		public void Dispose()
		{
			CaseDBContext.ReleaseConnection();
			CaseDBContext = null;
		}
	}
}