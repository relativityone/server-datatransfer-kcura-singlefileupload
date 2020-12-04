using NSerio.Relativity.Infrastructure;
using Relativity.API;
using System;

namespace NSerio.Relativity
{
	public static class RepositoryHelper
	{
		public static void ConfigureRepository(IHelper helper)
		{
			ConfigureRepository(helper.GetDBContext, helper.GetServicesManager, helper.GetLoggerFactory, () => helper);
		}

		public static void ConfigureRepository(Func<int, IDBContext> getDBContext, Func<IServicesMgr> getEndpointResolver, Func<ILogFactory> getLogFactoryResolver, Func<IHelper> getHelperResolver)
		{
			WorkspaceContextProvider.ConfigureSetCurrentWorkspaceIDFnc((int currentWorkspaceID) => CacheContextScope.SetData<int>("CurrentWorkspaceID", currentWorkspaceID, false));
			WorkspaceContextProvider.ConfigureGetCurrentWorkspaceIDFnc(() => CacheContextScope.GetData<int>("CurrentWorkspaceID", false));
			WorkspaceContextProvider.ConfigureWorkspaceContextFnc(() => {
				int currentWorkSpaceID = WorkspaceContextProvider.GetCurrentWorkSpaceID();
				string str = string.Concat("WorkspaceContext", currentWorkSpaceID);
				WorkspaceContext data = CacheContextScope.GetData<WorkspaceContext>(str, true);
				if (data == null)
				{
					data = new WorkspaceContext(getDBContext(currentWorkSpaceID));
					CacheContextScope.SetData<WorkspaceContext>(str, data, true);
				}
				return data;
			});
			WorkspaceContextProvider.ConfigureMasterDbContextFnc(() => {
				IDBContext data = CacheContextScope.GetData<IDBContext>("masterDBContext", true);
				if (data == null)
				{
					data = getDBContext(-1);
					CacheContextScope.SetData<IDBContext>("masterDBContext", data, true);
				}
				return data;
			});
			WorkspaceContextProvider.ConfigureEndpointResolverFnc(() => {
				IServicesMgr data = CacheContextScope.GetData<IServicesMgr>("serviceManagerContext", true);
				if (data == null)
				{
					data = getEndpointResolver();
					CacheContextScope.SetData<IServicesMgr>("serviceManagerContext", data, true);
				}
				return data;
			});
			WorkspaceContextProvider.ConfigureLogFactoryResolverFnc(() => {
				ILogFactory data = CacheContextScope.GetData<ILogFactory>("logFactoryContext", true);
				if (data == null)
				{
					data = getLogFactoryResolver();
					CacheContextScope.SetData<ILogFactory>("logFactoryContext", data, true);
				}
				return data;
			});
			WorkspaceContextProvider.ConfigureHelperResolverFnc(getHelperResolver);
		}

		public static CacheContextScope InitializeRepository(int workspaceID = -1)
		{
			CacheContextScope cacheContextScope = new CacheContextScope();
			WorkspaceContextProvider.SetCurrentWorkspaceID(workspaceID);
			return cacheContextScope;
		}
	}
}