using System;
using Polly;

namespace Relativity.SimpleFileUpload.FunctionalTests.Common
{
	public static class Retry
	{
		private const int _DEFAULT_RETRY_COUNT = 3;
		private const int _DEFAULT_RETRY_INTERVAL_IN_SECONDS = 5;

		public static TResult Do<TResult>(Func<TResult> action)
		{
			var retryPolicy = Policy<TResult>
				.Handle<Exception>()
				.WaitAndRetryAsync(_DEFAULT_RETRY_COUNT, i => TimeSpan.FromSeconds(_DEFAULT_RETRY_INTERVAL_IN_SECONDS));

			return retryPolicy.Execute(action);
		}
	}
}
