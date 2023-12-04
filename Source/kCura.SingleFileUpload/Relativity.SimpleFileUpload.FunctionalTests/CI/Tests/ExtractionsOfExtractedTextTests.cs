using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Relativity.SimpleFileUpload.FunctionalTests.Common;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api.Services;
using Relativity.Testing.Framework.Models;
using Relativity.Testing.Identification;

namespace Relativity.SimpleFileUpload.FunctionalTests.CI.Tests
{
	[TestFixture]
	[TestExecutionCategory.CI, TestLevel.L3]
	public class ExtractionsOfExtractedTextTests : FunctionalTestsTemplate
	{
		const string _EXPECTED_EXTRACTED_TEXT_COMMON = "Sample Extracted Text Lorem Ipsum\r\n";
		const string _EXPECTED_EXTRACTED_TEXT_PDF = "Sample Extracted Text Lorem Ipsum \r\n";

		const string _EXPECTED_EXTRACTED_TEXT_MSG =
			" To: Kamil Makarowski[kamil.makarowski@relativity.com]\r\n From: Kamil Bizoń[/O=EXCHANGELABS/OU=EXCHANGE ADMINISTRATIVE GROUP (FYDIBOHF23SPDLT)/CN=RECIPIENTS/CN=A475145505004DD5B6FB62BBD3641BF6-USER]\r\n Sender (Email) /O=EXCHANGELABS/OU=EXCHANGE ADMINISTRATIVE GROUP (FYDIBOHF23SPDLT)/CN=RECIPIENTS/CN=A475145505004DD5B6FB62BBD3641BF6-USER\r\n Sent: Thur 7/9/2020 11:41:09 AM (UTC) \r\n Importance: Normal\r\n Sensitivity: Normal\r\n Subject: TestEmail\r\n Alternate Recipient Allowed True\r\n Message Class IPM.Note\r\n Originator Delivery Report Requested False\r\n Priority Normal\r\n Read Receipt Requested False\r\n Sent Representing (Search Key) 45583A2F4F3D45584348414E47454C4142532F4F553D45584348414E47452041444D494E4953545241544956452047524F5550202846594449424F484632335350444C54292F434E3D524543495049454E54532F434E3D41343735313435353035303034444435423646423632424244333634314246362D5553455200\r\n Sent Representing (Entryid) 00000000DCA740C8C042101AB4B908002B2FE18201000000000000002F4F3D45584348414E47454C4142532F...";

		public ExtractionsOfExtractedTextTests() : base(nameof(ExtractionsOfExtractedTextTests))
		{ }

		[IdentifiedTestCase("533C649E-B681-48F4-99A3-703CD45DD83D", Const.File._FILE_NAME_DOC,
			_EXPECTED_EXTRACTED_TEXT_COMMON), Ignore("")]
		[IdentifiedTestCase("7441218B-4AD6-4392-8110-9F7098BD4FD4", Const.File._FILE_NAME_TXT,
			_EXPECTED_EXTRACTED_TEXT_COMMON)]
		[IdentifiedTestCase("9739E405-148E-4800-8392-09ADDEDBCD69", Const.File._FILE_NAME_XLSX,
			_EXPECTED_EXTRACTED_TEXT_COMMON)]
		[IdentifiedTestCase("5793CE0A-5B36-4D1C-80EA-E6584A2DC185", Const.File._FILE_NAME_PPTX,
			_EXPECTED_EXTRACTED_TEXT_COMMON)]
		[IdentifiedTestCase("E14C14CB-80AB-45EB-84EF-D614960CEE93", Const.File._FILE_NAME_PDF,
			_EXPECTED_EXTRACTED_TEXT_PDF)]
		[IdentifiedTestCase("3CB4C037-FD75-4F36-BAD7-A72DF66C74F6", Const.File._FILE_NAME_MSG,
			_EXPECTED_EXTRACTED_TEXT_MSG)]
		public async Task ShouldProperlyExtractExtractedText_FromNativeFile(string fileName,
			string expectedExtractedText)
		{
			// Arrange
			bool fdv = false;
			bool img = false;
			string filePath = TestFileHelper.GetFileLocation(fileName);
			FileInfo file = new FileInfo(filePath);

			// Act
			var result = await SimpleFileUploadHelper.UploadFileAsync(Client, WorkspaceId, file, fdv, img).ConfigureAwait(false);
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			var client = RelativityFacade.Instance.Resolve<IDocumentService>();
			IEnumerable<Document> documentsInWorkspace = client.GetAll(WorkspaceId).ToList();

			// Assert
			string actualExtractedText = documentsInWorkspace
				.First(x => x.ControlNumber.Equals(Path.GetFileNameWithoutExtension(fileName))).ExtractedText;
			actualExtractedText.Should().Be(expectedExtractedText);
		}
	}
}