using kCura.SingleFileUpload.Core.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Tests.Entities
{
	public class DocumentIdentifierFieldTest
	{
		private DocumentIdentifierField _documentField;

		[SetUp]
		public void Setup()
		{
			_documentField = new DocumentIdentifierField();
		}

		[Test]
		public void ArtifactId_Set_GetReturnsCorrectValue()
		{
			// Arrange
			int expectedArtifactId = 123;

			// Act
			_documentField.ArtifactId = expectedArtifactId;

			// Assert
			Assert.AreEqual(expectedArtifactId, _documentField.ArtifactId);
		}

		[Test]
		public void Name_Set_GetReturnsCorrectValue()
		{
			// Arrange
			string expectedName = "TestName";

			// Act
			_documentField.Name = expectedName;

			// Assert
			Assert.AreEqual(expectedName, _documentField.Name);
		}
	}
}