using kCura.SingleFileUpload.Core.Helpers;
using NSerio.Relativity;
using Polly;
using Polly.Retry;
using Relativity.Kepler.Exceptions;
using Relativity.Services.Exceptions;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Relativity.Services.Error;
using Relativity.Services.Workspace;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
	public abstract class BaseManager : IManager
	{
		private int _secondsBetweenHttpRetriesBase = 3;

		private const int _MAX_NUMBER_OF_HTTP_RETRIES = 4;

		private readonly Random _random = new Random();

		public Repository _Repository => (Repository)Repository.Instance;

		public int WorkspaceID
		{
			get => _Repository.WorkspaceID;
			set => _Repository.WorkspaceID = value;
		}

		public int GetArtifactTypeByArtifactGuid(string guid)
		{
			return (int)_Repository.CaseDBContext.ExecuteSqlStatementAsScalar(SQL.Queries.GetArtifactTypeByArtifactGuid,
				new SqlParameter[]
				{
					new SqlParameter("@Guid", guid)
				}
			);
		}

		public void LogError(Exception e)
		{
			var error = new Error
			{
				FullError = e.ToString(),
				Message = EventLogHelper.GetRecursiveExceptionMsg(e),
				Server = Environment.MachineName,
				Source = "WEB - Single File Upload",
				SendNotification = false,
				Workspace = new WorkspaceRef(Repository.Instance.WorkspaceID),
				URL = string.Empty
			};

			using (IErrorManager errorManager = Repository.Instance.CreateProxy<IErrorManager>())
			{
				ExecuteWithServiceRetriesAsync(() => errorManager.CreateSingleAsync(error)).GetAwaiter().GetResult();
			}
			
			Repository.Instance.GetLogFactory().GetLogger().ForContext<DocumentManager>().LogError(e, "Something occurred in Single File Upload {0}", e.Message);
		}

		protected Task<TResult> ExecuteWithServiceRetriesAsync<TResult>(Func<Task<TResult>> action) 
		{
			RetryPolicy httpErrorsPolicy = Policy
				.Handle<ServiceNotFoundException>()                                             // Thrown when the service does not exist, the service isn't running yet or there are bad routing entries.
				.Or<TemporarilyUnavailableException>()                                          // Thrown when the service is temporarily unavailable.
				.Or<ServiceException>(ex => ex.Message.Contains("Failed to determine route"))   // Thrown when there are bad routing entries.
				.Or<TimeoutException>()                                                         // Thrown when there is an infrastructure level timeout.
				.WaitAndRetryAsync(_MAX_NUMBER_OF_HTTP_RETRIES, retryAttempt =>
				{
					const int maxJitterMs = 100;
					TimeSpan delay = TimeSpan.FromSeconds(Math.Pow(_secondsBetweenHttpRetriesBase, retryAttempt));
					TimeSpan jitter = TimeSpan.FromMilliseconds(_random.Next(0, maxJitterMs));
					return delay + jitter;
				});

			return httpErrorsPolicy.ExecuteAsync(action);
		}
	}
}
