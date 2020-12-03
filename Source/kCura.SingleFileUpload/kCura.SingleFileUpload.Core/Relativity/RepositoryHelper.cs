using kCura.Relativity.Client;
using NSerio.Relativity.Infrastructure;
using Relativity.API;
using System;

namespace NSerio.Relativity
{
	public static class RepositoryHelper
	{
		public static void ConfigureRepository(IHelper helper)
		{
			Func<IRSAPIClient> func = () => helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.CurrentUser);
			Func<IRSAPIClient> func1 = () => helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System);
			IHelper helper1 = helper;
			Func<int, IDBContext> func2 = new Func<int, IDBContext>(helper1.GetDBContext);
			IHelper helper2 = helper;
			Func<IServicesMgr> func3 = new Func<IServicesMgr>(helper2.GetServicesManager);
			IHelper helper3 = helper;
			RepositoryHelper.ConfigureRepository(func, func1, func2, func3, new Func<ILogFactory>(helper3.GetLoggerFactory), () => helper);
		}

		public static void ConfigureRepository(Func<IRSAPIClient> getRSAPIClientForCurrentUser, Func<IRSAPIClient> getRSAPIClientForSystemUser, Func<int, IDBContext> getDBContext, Func<IServicesMgr> getEndpointResolver, Func<ILogFactory> getLogFactoryResolver, Func<IHelper> getHelperResolver)
		{
			WorkspaceContextProvider.ConfigureSetCurrentWorkspaceIDFnc((int currentWorkspaceID) => CacheContextScope.SetData<int>("CurrentWorkspaceID", currentWorkspaceID, false));
			WorkspaceContextProvider.ConfigureGetCurrentWorkspaceIDFnc(() => CacheContextScope.GetData<int>("CurrentWorkspaceID", false));
			WorkspaceContextProvider.ConfigureWorkspaceContextFnc(() => {
				int currentWorkSpaceID = WorkspaceContextProvider.GetCurrentWorkSpaceID();
				string str = string.Concat("WorkspaceContext", currentWorkSpaceID);
				WorkspaceContext data = CacheContextScope.GetData<WorkspaceContext>(str, true);
				if (data == null)
				{
					IRSAPIClient rSAPIClient = getRSAPIClientForCurrentUser();
					IRSAPIClient rSAPIClient1 = getRSAPIClientForSystemUser();
					rSAPIClient.APIOptions.WorkspaceID =currentWorkSpaceID;
					rSAPIClient1.APIOptions.WorkspaceID =currentWorkSpaceID;
					data = new WorkspaceContext(currentWorkSpaceID, rSAPIClient, rSAPIClient1, getDBContext(currentWorkSpaceID));
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