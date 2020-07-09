using FluentAssertions;
using kCura.SingleFileUpload.Core.Tests.Constants;
using NUnit.Framework;
using Relativity.SimpleFileUpload.Tests.Core.Templates;
using Relativity.Testing.Identification;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;

namespace kcura.SingleFileUpload.FunctionalTests
{
	[Feature.DataTransfer.SingleFileUpload]
	[Category("TestType.CI")]
	[TestFixture]
	public class ExtractionsOfExtractedTextTests : HttpFunctionalTestsTemplate
	{
		public ExtractionsOfExtractedTextTests() : base(nameof(ExtractionsOfExtractedTextTests))
		{
		}

		[IdentifiedTestCase("533C649E-B681-48F4-99A3-703CD45DD83D", "SampleDOC.doc")]
		[IdentifiedTestCase("9739E405-148E-4800-8392-09ADDEDBCD69", "SampleXLSX.xlsx")]
		[IdentifiedTestCase("5793CE0A-5B36-4D1C-80EA-E6584A2DC185", "SamplePPTX.pptx")]
		[IdentifiedTestCase("E14C14CB-80AB-45EB-84EF-D614960CEE93", "SamplePDF.pdf")]
		//Sample Extracted Text Lorem Ipsum \r\n
		[IdentifiedTestCase("3CB4C037-FD75-4F36-BAD7-A72DF66C74F6", "SampleMSG.msg")]
		public async Task ShouldProperlyExtractExtractedText_FromNativeFile(string fileName)
		{
			// Arrange
			string expectedExtractedText = "Sample Extracted Text Lorem Ipsum\r\n";
			bool fdv = false;
			bool img = false;
			FileInfo file = new FileInfo(FileHelper.GetFileLocation(fileName));

			// Act
			var result = await UploadFileAsync(file, fdv, img).ConfigureAwait(false);

			// Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			var client = RelativityFacade.Instance.Resolve<IDocumentService>();
			var documentsInWorkspace = client.GetAll(WorkspaceId).ToList();
			string actualExtractedText = documentsInWorkspace
				.First(x => x.ControlNumber.Equals(Path.GetFileNameWithoutExtension(fileName))).ExtractedText;
			actualExtractedText.Should().Be(expectedExtractedText);
		}

		[IdentifiedTestCase("B266EEFC-5B61-4D33-BEA2-50C8F8F0FE3D", "SamplePNG.png")]
		[IdentifiedTestCase("B7B2CF57-B242-4DFD-B3DB-CF5478A1684D", "SampleGIF.gif")]
		// ?? :(
		public async Task ShouldProperlyExtractExtractedText_FromImageFile(string fileName)
		{
			// Arrange
			string expectedExtractedText = "Sample Extracted Text Lorem Ipsum\r\n";
			bool fdv = false;
			bool img = false;
			FileInfo file = new FileInfo(FileHelper.GetFileLocation(fileName));

			// Act
			var result = await UploadFileAsync(file, fdv, img).ConfigureAwait(false);

			// Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			var client = RelativityFacade.Instance.Resolve<IDocumentService>();
			var documentsInWorkspace = client.GetAll(WorkspaceId).ToList();
			string actualExtractedText = documentsInWorkspace
				.First(x => x.ControlNumber.Equals(Path.GetFileNameWithoutExtension(fileName))).ExtractedText;
			actualExtractedText.Should().Be(expectedExtractedText);
		}
	}
}