using System;
using kCura.SingleFileUpload.Core.Relativity;
using kCura.SingleFileUpload.Core.Relativity.Infrastructure;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using NUnit.Framework;
using Relativity.API;

namespace kCura.SingleFileUpload.Core.Tests
{
	public abstract class TestBase
	{
		private CacheContextScope _cacheContextScope;

		protected internal void ConfigureSingletoneRepositoryScope(IHelper helper)
		{
			RepositoryHelper.ConfigureRepository(helper);
			_cacheContextScope = RepositoryHelper.InitializeRepository();
		}

		[TearDown]
		public void Teardown()
		{
			_cacheContextScope?.Dispose();
			FileHelper.DeleteTestTempFolder();
			Console.WriteLine("hello");
		}
	}
}
