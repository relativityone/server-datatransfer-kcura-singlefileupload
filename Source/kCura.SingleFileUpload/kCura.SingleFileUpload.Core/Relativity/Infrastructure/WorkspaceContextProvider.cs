using Relativity.API;
using System;

namespace NSerio.Relativity.Infrastructure
{
	public static class WorkspaceContextProvider
	{
		private static Func<int> _getCurrentWorksapceIDFnc;

		private static Action<int> _setCurrentWorkspaceIDFnc;

		private static Func<IDBContext> _masterDbContextFnc;

		private static Func<WorkspaceContext> _workspaceContextFnc;

		private static Func<IServicesMgr> _endpointResolverFnc;

		private static Func<ILogFactory> _logFactoryResolverFnc;

		private static Func<IHelper> _helperResolverFnc;

		public static void ConfigureEndpointResolverFnc(Func<IServicesMgr> fnc)
		{
			WorkspaceContextProvider._endpointResolverFnc = fnc;
		}

		public static void ConfigureGetCurrentWorkspaceIDFnc(Func<int> fnc)
		{
			WorkspaceContextProvider._getCurrentWorksapceIDFnc = fnc;
		}

		public static void ConfigureHelperResolverFnc(Func<IHelper> fnc)
		{
			WorkspaceContextProvider._helperResolverFnc = fnc;
		}

		public static void ConfigureLogFactoryResolverFnc(Func<ILogFactory> fnc)
		{
			WorkspaceContextProvider._logFactoryResolverFnc = fnc;
		}

		public static void ConfigureMasterDbContextFnc(Func<IDBContext> fnc)
		{
			WorkspaceContextProvider._masterDbContextFnc = fnc;
		}

		public static void ConfigureSetCurrentWorkspaceIDFnc(Action<int> act)
		{
			WorkspaceContextProvider._setCurrentWorkspaceIDFnc = act;
		}

		public static void ConfigureWorkspaceContextFnc(Func<WorkspaceContext> fnc)
		{
			WorkspaceContextProvider._workspaceContextFnc = fnc;
		}

		public static int GetCurrentWorkSpaceID()
		{
			return WorkspaceContextProvider._getCurrentWorksapceIDFnc();
		}

		public static IServicesMgr GetEndpointResolver()
		{
			return WorkspaceContextProvider._endpointResolverFnc();
		}

		public static IHelper GetHelperResolver()
		{
			return WorkspaceContextProvider._helperResolverFnc();
		}

		public static ILogFactory GetLogFactoryResolver()
		{
			return WorkspaceContextProvider._logFactoryResolverFnc();
		}

		public static IDBContext GetMasterDBContext()
		{
			return WorkspaceContextProvider._masterDbContextFnc();
		}

		public static WorkspaceContext GetWorkspaceContext()
		{
			return WorkspaceContextProvider._workspaceContextFnc();
		}

		public static void SetCurrentWorkspaceID(int workspaceID)
		{
			WorkspaceContextProvider._setCurrentWorkspaceIDFnc(workspaceID);
		}
	}
}