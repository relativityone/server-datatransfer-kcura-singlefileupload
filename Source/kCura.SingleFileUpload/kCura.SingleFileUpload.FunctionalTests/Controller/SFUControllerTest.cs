using System;
using kcura.SingleFileUpload.FunctionalTests.Helper;
using kCura.Relativity.ImportAPI;
using kCura.SingleFileUpload.Core.Factories;
using kCura.SingleFileUpload.Core.Tests.Constants;
using kCura.SingleFileUpload.MVC.Controllers;
using kCura.SingleFileUpload.MVC.Models;
using Moq;
using NSerio.Relativity;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.SharedTestHelpers;
using Relativity.Testing.Identification;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace kcura.SingleFileUpload.FunctionalTests.Controller
{
	[Feature.DataTransfer.SingleFileUpload]
	[Category("TestType.CI")]
	[TestFixture]
	class SFUControllerTest
	{
		internal int TestWorkspaceID { get; private set; }

		internal Mock<HttpContextBase> MoqContext;
		Mock<HttpRequestBase> MoqRequest;
		Mock<HttpResponseBase> MoqResponse;
		Mock<HttpFileCollectionBase> MoqFiles;
		Mock<HttpPostedFileBase> MoqFile;
		SFUController Controller;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			TestWorkspaceID = StartUpFixture.TestWorkspaceID;
			SetUpController();
		}

		public void SetUpController()
		{
			SetUpMocks();

			HelperSettings settings = new HelperSettings();
			ICPHelper fakeHelper = SetUpCPHelper(settings);

			SetUpImportApi(fakeHelper);

			RepositoryHelper.ConfigureRepository(fakeHelper);
			RepositoryHelper.InitializeRepository(TestWorkspaceID);

			Controller = new SFUController(fakeHelper);
			Controller.ControllerContext = new ControllerContext(MoqContext.Object, new RouteData(), Controller);
			Controller.Request.Params.Add("AppID", TestWorkspaceID.ToString());
			Controller.Request.Headers.Add("Authorization", settings.RelAdminAuthToken);
		}

		public ICPHelper SetUpCPHelper(HelperSettings settings)
		{
			ConfigurationHelper.SetupConfiguration(settings);
			return new TestCustomPageHelper(TestWorkspaceID);
		}

		private void SetUpMocks()
		{
			MoqContext = new Mock<HttpContextBase>();
			MoqRequest = new Mock<HttpRequestBase>();
			MoqResponse = new Mock<HttpResponseBase>();
			MoqFiles = new Mock<HttpFileCollectionBase>();
			MoqFile = new Mock<HttpPostedFileBase>();
			MoqFile.Setup(d => d.FileName).Returns(TestsConstants._FILE_NAME);
			MoqFile.Setup(d => d.InputStream).Returns(File.OpenRead(TestsConstants._FILE_LOCATION));
			MoqContext.Setup(x => x.Request).Returns(MoqRequest.Object);
			MoqContext.Setup(x => x.Response).Returns(MoqResponse.Object);
			MoqRequest.Setup(x => x.Params).Returns(new NameValueCollection());
			MoqRequest.Setup(x => x.Headers).Returns(new NameValueCollection());
			MoqRequest.Setup(x => x.Files).Returns(MoqFiles.Object);
			MoqFiles.Setup(x => x.Get(It.IsAny<int>())).Returns(MoqFile.Object);
		}

		private void SetUpImportApi(ICPHelper helper)
		{
			string message = TestContext.Parameters["AdminUsername"]
						   + $"|{TestContext.Parameters["AdminPassword"]}"
						   + $"|{TestContext.Parameters["RestServicesHostAddress"]}"
						   + $"|{TestContext.Parameters["RsapiServicesHostAddress"]}"
						   + $"|{TestContext.Parameters["RelativityHostAddress"]}"
						   + $"|{helper.GetServicesManager().GetRESTServiceUrl()}";

			throw new Exception(message);

			//IImportAPI importApi = new ExtendedImportAPI(
			//	"relativity.admin@kcura.com", //TestContext.Parameters["AdminUsername"],
			//	"Test1234!", //TestContext.Parameters["AdminPassword"],
			//	TestContext.Parameters["RestServicesHostAddress"].Replace("/relativity.services", "/RelativityWebAPI"));



			//ImportApiFactory.SetUpSingleton(importApi, null);
		}

		[IdentifiedTest("26D369B2-B935-40D2-9F3E-B49BAD86C027")]
		public async Task UploadFileSuccess()
		{
			string expectedResponse = $"<script>sessionStorage['____pushNo'] = '{{\"Data\":\"{TestsConstants._DOC_CONTROL_NUMBER}\",\"Success\":true,\"Message\":null}}'</script>";
			string responseUpload = string.Empty;
			MoqResponse.Setup(x => x.Write(It.IsAny<string>())).Callback<string>((response) =>
			{
				responseUpload = response;
			});
			MetaUploadFile metaData = new MetaUploadFile { fdv = false };

			await Controller.Upload(metaData).ConfigureAwait(false);
			Assert.AreEqual(expectedResponse, responseUpload);
		}
	}
}
