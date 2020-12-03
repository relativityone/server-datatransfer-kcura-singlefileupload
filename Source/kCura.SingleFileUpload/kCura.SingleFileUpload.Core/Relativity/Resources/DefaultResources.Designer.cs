using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace NSerio.Relativity.Resources
{
	[CompilerGenerated]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	internal class DefaultResources
	{
		private static System.Resources.ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return DefaultResources.resourceCulture;
			}
			set
			{
				DefaultResources.resourceCulture = value;
			}
		}

		internal static string ErrorRunningQuery
		{
			get
			{
				return DefaultResources.ResourceManager.GetString("ErrorRunningQuery", DefaultResources.resourceCulture);
			}
		}

		internal static string ErrorTryingToSetWorkspace
		{
			get
			{
				return DefaultResources.ResourceManager.GetString("ErrorTryingToSetWorkspace", DefaultResources.resourceCulture);
			}
		}

		internal static string GetUserInfo
		{
			get
			{
				return DefaultResources.ResourceManager.GetString("GetUserInfo", DefaultResources.resourceCulture);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if (DefaultResources.resourceMan == null)
				{
					DefaultResources.resourceMan = new System.Resources.ResourceManager("NSerio.Relativity.Resources.DefaultResources", typeof(DefaultResources).Assembly);
				}
				return DefaultResources.resourceMan;
			}
		}

		internal DefaultResources()
		{
		}
	}
}