using NSerio.Relativity;
using NSerio.Relativity.Infrastructure;
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

		[OneTimeTearDown]
		public void Teardown()
		{
			_cacheContextScope?.Dispose();
		}

	}
}
