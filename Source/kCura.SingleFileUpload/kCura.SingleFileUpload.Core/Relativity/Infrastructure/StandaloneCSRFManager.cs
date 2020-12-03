using System;

namespace NSerio.Relativity.Infrastructure
{
	public class StandaloneCSRFManager
	{
		public string CSRFHeaderName
		{
			get
			{
				return "X-CSRF-Header";
			}
		}

		public string CSRFToken
		{
			get
			{
				return "--Nothing--";
			}
		}

		public StandaloneCSRFManager()
		{
		}

		public void CheckAgainstCSRF(string csrf)
		{
		}

		public void CheckCSRF()
		{
		}
	}
}