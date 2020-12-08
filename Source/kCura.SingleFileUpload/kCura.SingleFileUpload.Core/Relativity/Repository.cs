using System;
using kCura.SingleFileUpload.Core.Relativity.Infrastructure;
using Relativity.API;

namespace kCura.SingleFileUpload.Core.Relativity
{
	public class Repository : IRepository
	{
		private static IRepository _instance;
		private static readonly object _lock;

		public static IRepository Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = new Repository();
						}
					}
				}
				return _instance;
			}
		}

		public IDBContext CaseDBContext => WorkspaceContextProvider.GetWorkspaceContext().CaseDBContext;

		public IHelper Helper => WorkspaceContextProvider.GetHelperResolver();

		public IDBContext MasterDBContext => WorkspaceContextProvider.GetMasterDBContext();

		public int WorkspaceID
		{
			get => WorkspaceContextProvider.GetCurrentWorkSpaceID();
			set => WorkspaceContextProvider.SetCurrentWorkspaceID(value);
		}

		public T CreateProxy<T>(ExecutionIdentity identity = 0) where T : IDisposable
		{
			return WorkspaceContextProvider.GetEndpointResolver().CreateProxy<T>(identity);
		}

		public ILogFactory GetLogFactory()
		{
			return WorkspaceContextProvider.GetLogFactoryResolver();
		}

		static Repository()
		{
			_lock = new object();
		}

		private Repository()
		{
		}
	}
}