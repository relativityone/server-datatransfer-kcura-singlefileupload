using kCura.Relativity.Client;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace NSerio.Relativity.Exceptions
{
	[Serializable]
	public class QueryException : Exception
	{
		public QueryResult FailedResult
		{
			get;
			set;
		}

		public QueryException(string message, QueryResult failedResult) : base(message, null)
		{
			this.FailedResult = failedResult;
		}

		protected QueryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}