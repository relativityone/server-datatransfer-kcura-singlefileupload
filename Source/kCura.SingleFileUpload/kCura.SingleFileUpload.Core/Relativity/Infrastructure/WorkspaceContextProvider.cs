using System;
using Relativity.API;

namespace kCura.SingleFileUpload.Core.Relativity.Infrastructure
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
			_endpointResolverFnc = fnc;
		}

		public static void ConfigureGetCurrentWorkspaceIDFnc(Func<int> fnc)
		{
			_getCurrentWorksapceIDFnc = fnc;
		}

		public static void ConfigureHelperResolverFnc(Func<IHelper> fnc)
		{
			_helperResolverFnc = fnc;
		}

		public static void ConfigureLogFactoryResolverFnc(Func<ILogFactory> fnc)
		{
			_logFactoryResolverFnc = fnc;
		}

		public static void ConfigureMasterDbContextFnc(Func<IDBContext> fnc)
		{
			_masterDbContextFnc = fnc;
		}

		public static void ConfigureSetCurrentWorkspaceIDFnc(Action<int> act)
		{
			_setCurrentWorkspaceIDFnc = act;
		}

		public static void ConfigureWorkspaceContextFnc(Func<WorkspaceContext> fnc)
		{
			_workspaceContextFnc = fnc;
		}

		public static int GetCurrentWorkSpaceID()
		{
			return _getCurrentWorksapceIDFnc();
		}

		public static IServicesMgr GetEndpointResolver()
		{
			return _endpointResolverFnc();
		}

		public static IHelper GetHelperResolver()
		{
			return _helperResolverFnc();
		}

		public static ILogFactory GetLogFactoryResolver()
		{
			return _logFactoryResolverFnc();
		}

		public static IDBContext GetMasterDBContext()
		{
			return _masterDbContextFnc();
		}

		public static WorkspaceContext GetWorkspaceContext()
		{
			return _workspaceContextFnc();
		}

		public static void SetCurrentWorkspaceID(int workspaceID)
		{
			_setCurrentWorkspaceIDFnc(workspaceID);
		}
	}
}