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
		const string ExpectedExtractedTextCommon = "Sample Extracted Text Lorem Ipsum\r\n";
		const string ExpectedExtractedTextPdf = "Sample Extracted Text Lorem Ipsum \r\n";

		const string ExpectedExtractedTextMsg =
			" To: Kamil Makarowski[kamil.makarowski@relativity.com]\r\n From: Kamil Bizoń[/O=EXCHANGELABS/OU=EXCHANGE ADMINISTRATIVE GROUP (FYDIBOHF23SPDLT)/CN=RECIPIENTS/CN=A475145505004DD5B6FB62BBD3641BF6-USER]\r\n Sender (Email) /O=EXCHANGELABS/OU=EXCHANGE ADMINISTRATIVE GROUP (FYDIBOHF23SPDLT)/CN=RECIPIENTS/CN=A475145505004DD5B6FB62BBD3641BF6-USER\r\n Sent: Thur 7/9/2020 11:41:09 AM (UTC) \r\n Importance: Normal\r\n Sensitivity: Normal\r\n Subject: TestEmail\r\n Alternate Recipient Allowed True\r\n Message Class IPM.Note\r\n Originator Delivery Report Requested False\r\n Priority Normal\r\n Read Receipt Requested False\r\n Sent Representing (Search Key) 45583A2F4F3D45584348414E47454C4142532F4F553D45584348414E47452041444D494E4953545241544956452047524F5550202846594449424F484632335350444C54292F434E3D524543495049454E54532F434E3D41343735313435353035303034444435423646423632424244333634314246362D5553455200\r\n Sent Representing (Entryid) 00000000DCA740C8C042101AB4B908002B2FE18201000000000000002F4F3D45584348414E47454C4142532F...";

		public ExtractionsOfExtractedTextTests() : base(nameof(ExtractionsOfExtractedTextTests))
		{
		}

		[IdentifiedTestCase("533C649E-B681-48F4-99A3-703CD45DD83D", TestsConstants._FILE_NAME_DOC,
			ExpectedExtractedTextCommon)]
		[IdentifiedTestCase("7441218B-4AD6-4392-8110-9F7098BD4FD4", TestsConstants._FILE_NAME_TXT,
			ExpectedExtractedTextCommon)]
		[IdentifiedTestCase("9739E405-148E-4800-8392-09ADDEDBCD69", TestsConstants._FILE_NAME_XLSX,
			ExpectedExtractedTextCommon)]
		[IdentifiedTestCase("5793CE0A-5B36-4D1C-80EA-E6584A2DC185", TestsConstants._FILE_NAME_PPTX,
			ExpectedExtractedTextCommon)]
		[IdentifiedTestCase("E14C14CB-80AB-45EB-84EF-D614960CEE93", TestsConstants._FILE_NAME_PDF,
			ExpectedExtractedTextPdf)]
		[IdentifiedTestCase("3CB4C037-FD75-4F36-BAD7-A72DF66C74F6", TestsConstants._FILE_NAME_MSG,
			ExpectedExtractedTextMsg)]
		public async Task ShouldProperlyExtractExtractedText_FromNativeFile(string fileName,
			string expectedExtractedText)
		{
			// Arrange
			bool fdv = false;
			bool img = false;
			FileInfo file = new FileInfo(FileHelper.GetFileLocation(fileName));

			// Act
			var result = await UploadFileAsync(file, fdv, img).ConfigureAwait(false);
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			var client = RelativityFacade.Instance.Resolve<IDocumentService>();
			var documentsInWorkspace = client.GetAll(WorkspaceId).ToList();

			// Assert
			string actualExtractedText = documentsInWorkspace
				.First(x => x.ControlNumber.Equals(Path.GetFileNameWithoutExtension(fileName))).ExtractedText;
			actualExtractedText.Should().Be(expectedExtractedText);
		}

		[IdentifiedTestCase("B266EEFC-5B61-4D33-BEA2-50C8F8F0FE3D", "SamplePNG.png")]
		[IdentifiedTestCase("B7B2CF57-B242-4DFD-B3DB-CF5478A1684D", "SampleGIF.gif")]
		[IdentifiedTestCase("0A19A280-D46A-419B-A74C-961BCFFC4E71", "SampleTIF.tif")]
		[Ignore("Not working")]
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