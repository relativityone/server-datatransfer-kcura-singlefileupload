using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Vendor.Castle.Core.Internal;
using kCura.SingleFileUpload.Core.Helpers;

namespace kCura.SingleFileUpload.Core.Tests.Helpers
{
	public class EventLogHelperTest
	{


		private Exception _exception1;
		private Exception _exception2;

		[SetUp]
		public void Setup()
		{
			_exception1 = new Exception("Outer exception");
			_exception2 = new Exception("Inner exception", _exception1);
		}

		[Test]
		public void GetRecursiveExceptionMsg_NoInnerException_ReturnsSingleErrorMessage()
		{
			// Act
			var result = EventLogHelper.GetRecursiveExceptionMsg(_exception1);

			// Assert
			Assert.IsTrue(result.Contains("Outer exception"));
		}

		[Test]
		public void GetRecursiveExceptionMsg_WithInnerException_ReturnsRecursiveErrorMessage()
		{
			// Act
			var result = EventLogHelper.GetRecursiveExceptionMsg(_exception2);

			// Assert
			Assert.IsTrue(result.Contains("Outer exception"));
			Assert.IsTrue(result.Contains("Inner exception"));
		}
		[Test]
		public void GetRecursiveExceptionMsg_WithNestedInnerException_ReturnsNestedRecursiveErrorMessage()
		{
			// Arrange
			var innermostException = new Exception("Innermost exception");
			var innerException = new Exception("Inner exception", innermostException);
			var outerException = new Exception("Outer exception", innerException);

			// Act
			var result = EventLogHelper.GetRecursiveExceptionMsg(outerException);

			// Assert
			Assert.IsTrue(result.Contains("Outer exception"));
			Assert.IsTrue(result.Contains("Inner exception"));
			Assert.IsTrue(result.Contains("Innermost exception"));
		}

	}
}