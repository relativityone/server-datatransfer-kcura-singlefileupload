using NUnit.Framework;
using kCura.SingleFileUpload.Core.Entities;
using Moq;
using System.Data;

namespace kCura.SingleFileUpload.Core.Tests.Entities
{
	[TestFixture]
	public class ImportJobSettingsTests
	{
		private Mock<IDataReader> mockDataReader;
		private ImportJobSettings importJobSettings;

		[SetUp]
		public void SetUp()
		{
			mockDataReader = new Mock<IDataReader>();
			importJobSettings = new ImportJobSettings
			{
				WorkspaceID = 123,
				FolderId = 456,
				IdentityField = new DocumentIdentifierField(),
				DocumentsDataReader = mockDataReader.Object
			};
		}

		[Test]
		public void WorkspaceID_ShouldGetAndSet()
		{
			// Arrange
			var expected = 789;

			// Act
			importJobSettings.WorkspaceID = expected;
			var actual = importJobSettings.WorkspaceID;

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void FolderId_ShouldGetAndSet()
		{
			// Arrange
			var expected = 101112;

			// Act
			importJobSettings.FolderId = expected;
			var actual = importJobSettings.FolderId;

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void IdentityField_ShouldGetAndSet()
		{
			// Arrange
			var expected = new DocumentIdentifierField();

			// Act
			importJobSettings.IdentityField = expected;
			var actual = importJobSettings.IdentityField;

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DocumentsDataReader_ShouldGetAndSet()
		{
			// Arrange
			var mockNewDataReader = new Mock<IDataReader>();
			var expected = mockNewDataReader.Object;

			// Act
			importJobSettings.DocumentsDataReader = expected;
			var actual = importJobSettings.DocumentsDataReader;

			// Assert
			Assert.AreEqual(expected, actual);
		}
	}
}