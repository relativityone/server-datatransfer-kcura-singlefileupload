using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Factories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Tests.Factories
{
	[TestFixture]
	public class ImportApiFactoryTests
	{

		private Mock<IImportAPI> mockImportApi;
		private Mock<IImportBulkArtifactJob> mockImportBulkArtifactJob;

		[SetUp]
		public void Setup()
		{
			// Initialize mocks
			mockImportApi = new Mock<IImportAPI>();
			mockImportBulkArtifactJob = new Mock<IImportBulkArtifactJob>();
			// Set up singleton instance
			ImportApiFactory.SetUpSingleton(mockImportApi.Object, mockImportBulkArtifactJob.Object);
		}

		[Test]
		public void GetImportAPI_Returns_Set_Up_ImportAPI_If_Set_Up()
		{
			// Arrange
			var settings = new ImportSettings();
			var factory = ImportApiFactory.Instance;

			// Act
			var importApi = factory.GetImportAPI(settings);

			// Assert
			Assert.AreEqual(mockImportApi.Object, importApi);
		}

		[Test]
		public void GetImportApiBulkArtifactJob_Returns_Set_Up_ImportBulkArtifactJob_If_Set_Up()
		{
			// Arrange
			var factory = ImportApiFactory.Instance;

			// Act
			var importBulkArtifactJob = factory.GetImportApiBulkArtifactJob(
				mockImportApi.Object,
				(args) => { }, // Providing a placeholder event handler
				null,
				(args) => { }, // Providing a placeholder event handler
				new ImportJobSettings());
			// Assert
			Assert.AreEqual(mockImportBulkArtifactJob.Object, importBulkArtifactJob);
		}
	}
}