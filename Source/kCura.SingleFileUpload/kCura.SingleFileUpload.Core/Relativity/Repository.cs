using kCura.Relativity.Client;
using NSerio.Relativity.Infrastructure;
using Relativity.API;
using System;

namespace NSerio.Relativity
{
	public class Repository : IRepository
	{
		private readonly static object _lock;

		private static IRepository _instance;

		public IDBContext CaseDBContext
		{
			get
			{
				return WorkspaceContextProvider.GetWorkspaceContext().CaseDBContext;
			}
		}

		public IHelper Helper
		{
			get
			{
				return WorkspaceContextProvider.GetHelperResolver();
			}
		}

		public static IRepository Instance
		{
			get
			{
				if (Repository._instance == null)
				{
					lock (Repository._lock)
					{
						if (Repository._instance == null)
						{
							Repository._instance = new Repository();
						}
					}
				}
				return Repository._instance;
			}
		}

		public IDBContext MasterDBContext
		{
			get
			{
				return WorkspaceContextProvider.GetMasterDBContext();
			}
		}

		public IRSAPIClient RSAPIClient
		{
			get
			{
				return WorkspaceContextProvider.GetWorkspaceContext().RSAPIClient;
			}
		}

		public IRSAPIClient RSAPISystem
		{
			get
			{
				return WorkspaceContextProvider.GetWorkspaceContext().RSAPISystem;
			}
		}

		public int WorkspaceID
		{
			get
			{
				return WorkspaceContextProvider.GetCurrentWorkSpaceID();
			}
			set
			{
				WorkspaceContextProvider.SetCurrentWorkspaceID(value);
			}
		}

		static Repository()
		{
			Repository._lock = new object();
		}

		private Repository()
		{
		}

		public T CreateProxy<T>(ExecutionIdentity identity = 0)
		where T : IDisposable
		{
			return WorkspaceContextProvider.GetEndpointResolver().CreateProxy<T>(identity);
		}

		public ILogFactory GetLogFactory()
		{
			return WorkspaceContextProvider.GetLogFactoryResolver();
		}
	}
}