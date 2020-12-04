using System;
using Relativity.API;

namespace kCura.SingleFileUpload.Core.Relativity
{
	public interface IRepository
	{
		IDBContext CaseDBContext
		{
			get;
		}

		IHelper Helper
		{
			get;
		}

		IDBContext MasterDBContext
		{
			get;
		}

		int WorkspaceID
		{
			get;
			set;
		}

		T CreateProxy<T>(ExecutionIdentity identity = 0) where T : IDisposable;

		ILogFactory GetLogFactory();
	}
}