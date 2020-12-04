using Relativity.API;
using System;

namespace NSerio.Relativity
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