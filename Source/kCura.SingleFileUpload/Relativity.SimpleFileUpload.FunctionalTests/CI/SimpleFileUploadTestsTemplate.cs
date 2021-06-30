using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
using Relativity.Testing.Framework.Api.Services;
using Relativity.Testing.Framework.Models;
using Relativity.Testing.Framework.Web;
using Relativity.Testing.Identification;

namespace Relativity.SimpleFileUpload.FunctionalTests.CI
{
	[Feature.DataTransfer.SimpleFileUpload]
	public abstract class SimpleFileUploadTestsTemplate : UITestFixture
	{
		private readonly string _workspaceName;
		private readonly string _workspaceTemplateName;

		private readonly IWorkspaceService _workspaceService;

		public int WorkspaceId { get; private set; }
		public int ImagingProfileId { get; private set; }

		protected SimpleFileUploadTestsTemplate(string workspaceName, string workspaceTemplateName)
		{
			_workspaceName = workspaceName;
			_workspaceTemplateName = workspaceTemplateName;

			_workspaceService = RelativityFacade.Instance.Resolve<IWorkspaceService>();
		}

		protected override void OnSetUpFixture()
		{
			base.OnSetUpFixture();

			Workspace workspace = new Workspace
			{
				Name = _workspaceName,
				TemplateWorkspace = new NamedArtifact { Name = _workspaceTemplateName }
			};

			WorkspaceId = _workspaceService.Create(workspace).ArtifactID;
			ImagingProfileId = GetDefaultProcessingProfileAsync(WorkspaceId).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		protected override void OnTearDownFixture()
		{
			base.OnTearDownFixture();

			_workspaceService.Delete(WorkspaceId);
		}

		protected override void OnSetUpTest()
		{
			base.OnSetUpTest();

			RetryPolicy loginAsStandardAccountPolicy = Policy
				.Handle<HttpRequestException>(ex => ex.Message.Contains("The entered E-Mail Address is already associated with another user in the system."))
				.Retry(3);

			loginAsStandardAccountPolicy.Execute(() => LoginAsStandardAccount());
		}

		private async Task<int> GetDefaultProcessingProfileAsync(int workspaceId)
		{
			using (var objectManagerProxy = RelativityFacade.Instance.Resolve<ApiComponent>().ServiceFactory.GetServiceProxy<IObjectManager>())
			{
				IEnumerable<FieldRef> queryFields = new List<FieldRef>() {
					new FieldRef() { Name = "ArtifactID" },
				};

				string condition = $"'Name' == 'Basic Default'";

				QueryRequest queryRequest = BuildQueryRequest(queryFields, Const.File._IMAGING_PROFILE_ARTIFACT_ID, condition);
				var queryResult = await BuildQueryAsync(objectManagerProxy, workspaceId, queryRequest, 0, 0);

				return queryResult.Objects.FirstOrDefault().ArtifactID;
			}
		}

		private Task<QueryResult> BuildQueryAsync(IObjectManager objectManagerProxy, int workspaceID, QueryRequest queryRequest, int start, int length)
		{
			return objectManagerProxy.QueryAsync(workspaceID, queryRequest, start, length);
		}

		private QueryRequest BuildQueryRequest(IEnumerable<FieldRef> queryFields, int typeId, string condition)
		{
			QueryRequest queryRequest = new QueryRequest()
			{
				ObjectType = new ObjectTypeRef() { ArtifactTypeID = typeId },
				Condition = condition,
				Fields = queryFields,
				RelationalField = null
			};
			return queryRequest;
		}
	}
}
