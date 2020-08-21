﻿using NUnit.Framework;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
using Relativity.Testing.Framework.Web;
using System.IO;
using System.Reflection;

namespace Relativity.SimpleFileUpload.Tests.Core
{
	public class SetUpFixture
	{
		public SetUpFixture()
		{
			RelativityFacade.Instance.RelyOn<CoreComponent>();
			RelativityFacade.Instance.RelyOn<ApiComponent>();
			RelativityFacade.Instance.RelyOn<WebComponent>();

			RelativityFacade.Instance.GetComponent<WebComponent>().Configuration.ChromeBinaryFilePath = 
				Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SharedVariables.ChromeBinaryLocation);
	}
	}
}