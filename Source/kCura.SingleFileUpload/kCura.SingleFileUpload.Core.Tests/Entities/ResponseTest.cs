using NUnit.Framework;
using kCura.SingleFileUpload.Core.Entities;

namespace kCura.SingleFileUpload.Core.Tests.Entities
{
	[TestFixture]
	public class ResponseTest
	{
		private Response _response;

		[SetUp]
		public void SetUp()
		{
			_response = new Response();
		}

		[Test]
		public void TestSuccessProperty()
		{
			// Act
			_response.Success = true;

			// Assert
			Assert.IsTrue(_response.Success);
		}

		[Test]
		public void TestResultProperty()
		{
			// Arrange
			var expectedResult = "Test Result";

			// Act
			_response.Result = expectedResult;

			// Assert
			Assert.AreEqual(expectedResult, _response.Result);
		}
	}
}